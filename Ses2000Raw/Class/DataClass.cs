using System.Collections.Concurrent;
using System.Linq;
using System.Numerics;

namespace Ses2000Raw
{
    public class DataClass
    {
        #region Public Method
        /// <summary>
        /// 信号処理
        /// </summary>
        /// <param name="fullWave">信号処理する全波形データ</param>
        /// <param name="fftNs">FFTを行うサンプル数（2のべき乗）</param>
        /// <param name="bpfFlag">BPF実行フラグ</param>
        /// <param name="hpf">HPFカットオフ周波数</param>
        /// <param name="lpf">LPFカットオフ周波数</param>
        /// <param name="envelopeFlag">Envelope実行フラグ</param>
        /// <returns>信号処理済みの波形データ</returns>
        public static short[] SignalProcess(short[] fullWave, int sampleFreq, int fftNs, bool bpfFlag, int hpf, int lpf, bool envelopeFlag, bool deconvoFlag)
        {
            int iNs = fullWave.Length;                            // サンプル数

            // 入力を Complex[] 化
            Complex[] xTime = new Complex[fftNs];
            for (int i = 0; i < iNs; i++)
                xTime[i] = new Complex(fullWave[i], 0.0);   // 実数成分だけ入れる

            // FFT
            var complexFFT = new Complex[fftNs];
            FourierTransformClass.Fourier(xTime, ref complexFFT, 1.0);  // FFT


            // BPF
            if (bpfFlag)
            {
                double fLow = hpf * 1000.0;
                double fHigh = lpf * 1000.0;
                ApplyBpfInPlace(fftNs, sampleFreq, fLow, fHigh, complexFFT);
            }

            // Envelope
            if (envelopeFlag)
                ApplyHilbertMaskInPlace(fftNs, complexFFT);

            // IFFT
            var complexIFFT = new Complex[fftNs];
            FourierTransformClass.Fourier(complexFFT, ref complexIFFT, -1.0);  // IFFT


            var ret = new short[iNs];
            for (int z = 0; z < iNs; z++)
            {
                if (envelopeFlag)
                    //ret[z] = (short)(complexIFFT[z].Magnitude * 2);
                    ret[z] = (short)complexIFFT[z].Magnitude;
                else
                    //ret[z] = complexIFFT[z].Real;
                    ret[z] = (short)complexIFFT[z].Real;
            }
            return ret;
        }
        /// <summary>
        /// BPF適用
        /// </summary>
        /// <param name="n"></param>
        /// <param name="fsHz"></param>
        /// <param name="fLowHz"></param>
        /// <param name="fHighHz"></param>
        /// <param name="X"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static void ApplyBpfInPlace(int n, double fsHz, double fLowHz, double fHighHz, Complex[] X)
        {
            if (X.Length != n) throw new ArgumentException("length mismatch");
            if (!(0 <= fLowHz && fLowHz < fHighHz && fHighHz <= fsHz * 0.5))
                throw new ArgumentOutOfRangeException(nameof(fHighHz), "0 <= fLow < fHigh <= fs/2");

            double df = fsHz / n;
            int n2 = n / 2;

            // 遷移帯域（例）：それぞれ 0.02*fs （必要に応じて調整）
            double twLow = 0.02 * fsHz;         // HPF側テーパ幅[Hz]
            double twHigh = 0.02 * fsHz;         // LPF側テーパ幅[Hz]

            for (int k = 0; k < n; k++)
            {
                double f = (k <= n2) ? (k * df) : ((n - k) * df);

                double gain;
                if (f <= fLowHz - twLow || f >= fHighHz + twHigh)
                {
                    gain = 0.0; // 完全遮断
                }
                else if (f < fLowHz) // HPFの立上がり
                {
                    double t = (f - (fLowHz - twLow)) / twLow; // 0..1
                    gain = 0.5 * (1 - Math.Cos(Math.PI * t)); // raised-cosine
                }
                else if (f > fHighHz) // LPFの立下り
                {
                    double t = 1.0 - (f - fHighHz) / twHigh; // 1..0
                    gain = 0.5 * (1 - Math.Cos(Math.PI * t));
                }
                else
                {
                    gain = 1.0; // 通過域
                }

                X[k] *= gain;
            }
        }


        /// <summary>
        /// Envelope用ヒルベルトマスク適用
        /// </summary>
        /// <param name="n"></param>
        /// <param name="X"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void ApplyHilbertMaskInPlace(int n, Complex[] X)
        {
            if (X.Length != n) throw new ArgumentException("length mismatch");
            int n2 = n / 2;

            for (int k = 0; k < n; k++)
            {
                double g = (k == 0 || k == n2) ? 1.0    // DC と Nyquist は 1
                         : (k < n2) ? 2.0               // 正側は 2
                         : 0.0;                         // 負側は 0
                X[k] *= g;
            }
        }
        /// <summary>
        /// Envelopeを並列実行する
        /// </summary>
        /// <param name="context"></param>
        /// <param name="progress"></param>
        /// <param name="prop"></param>
        /// <param name="fullWaves"></param>
        /// <param name="fftNs"></param>
        /// <param name="bpfFlag"></param>
        /// <param name="hpf"></param>
        /// <param name="lpf"></param>
        /// <param name="envelopeFlag"></param>
        /// <returns></returns>
        public static Result SignalProcessParallel(SynchronizationContext context, IProgress<int> progress, List<DataBlock> dataBlockList, int sampleFreq, bool bpfFlag, int hpf, int lpf, bool envelopeFlag, bool deconvoFlag, double sigmaTimeSamples, double gammaTime)
        {
            Result ret = new Result();
            int iNumPing = dataBlockList.Count;
            var processed = new short[iNumPing][];
            int iProgressCnt = 0;
            try
            {
                if (deconvoFlag)
                {
                    var psfCache = new ConcurrentDictionary<(int sampleLength, double sigmaTimeSamples), Complex[]>();

                    Parallel.For(0, iNumPing, (y, state) =>
                    {
                        if (ProgressForm.Cancel) state.Break();

                        int sampleLength = dataBlockList[y].Lf.Length;
                        Complex[] psfSpectrum = psfCache.GetOrAdd((sampleLength, sigmaTimeSamples), key => BuildTimePsfSpectrum(key.sampleLength, key.sigmaTimeSamples));

                        short[] deconvolved = DeconvolveWave(dataBlockList[y].Lf, sampleFreq, bpfFlag, hpf, lpf, envelopeFlag, psfSpectrum, gammaTime);
                        processed[y] = deconvolved;

                        // 進捗（Progress<T> はUIに自動マーシャル）
                        progress.Report(Interlocked.Increment(ref iProgressCnt));

                    });
                }
                else
                {
                    Parallel.For(0, iNumPing, (y, state) =>
                    {
                        if (ProgressForm.Cancel) state.Break();

                        int iNumSample = dataBlockList[y].Lf.Length;
                        int iPower = (int)Math.Log(iNumSample, 2);
                        int iFftNs;
                        if ((iNumSample & (iNumSample - 1)) == 0)
                            iFftNs = (int)Math.Pow(2, iPower);      // サンプル数が2のべき乗の場合
                        else
                            iFftNs = (int)Math.Pow(2, iPower + 1);  // サンプル数が2のべき乗でない場合


                        // 信号処理
                        short[] envWaves = SignalProcess(dataBlockList[y].Lf, sampleFreq, iFftNs, bpfFlag, hpf, lpf, envelopeFlag, deconvoFlag);
                        processed[y] = envWaves;

                        // 進捗（Progress<T> はUIに自動マーシャル）
                        progress.Report(Interlocked.Increment(ref iProgressCnt));

                    });
                }
                if (ProgressForm.Cancel)
                {
                    ret.RetCode = ResultCode.Cancel;
                    ret.Message = "インポート処理がキャンセルされました。";
                }
                else
                {
                    ret.RetCode = ResultCode.Ok;
                    ret.Message = "インポート処理が正常終了しました。";
                    ret.Obj = processed;
                }
                return ret;
            }
            catch (Exception er)
            {
                ret.RetCode = ResultCode.Error;
                ret.Message = "インポート処理中に予期せぬエラーが発生しました。\n" + er.Message;
                return ret;
            }
        }
        #endregion

        #region Deconvolution Helpers
        public const double DefaultSigmaTimeSamples = 1.5;     // 時間方向PSFのσ（サンプル数）
        public const double DefaultGammaTime = 1e-4;           // 時間方向デコンボ用正則化パラメータ

        /// <summary>
        /// 時間方向のPSFスペクトルを生成する
        /// </summary>
        /// <param name="sampleLength">データ長</param>
        /// <returns>PSFの周波数領域表現</returns>
        private static Complex[] BuildTimePsfSpectrum(int sampleLength, double sigmaTimeSamples)
        {
            if (sampleLength <= 0) return Array.Empty<Complex>();

            int lt = Math.Min((int)Math.Ceiling(6 * sigmaTimeSamples), sampleLength - 1);
            if (lt <= 0)
            {
                var flat = new Complex[sampleLength];
                for (int i = 0; i < sampleLength; i++) flat[i] = Complex.One;
                return flat;
            }

            if (lt % 2 == 0) lt += 1;

            int half = (lt - 1) / 2;
            var gt = new Complex[sampleLength];
            for (int i = 0; i < lt; i++)
            {
                int tIdx = i - half;
                double value = Math.Exp(-(tIdx * tIdx) / (2 * sigmaTimeSamples * sigmaTimeSamples));
                gt[i] = new Complex(value, 0);
            }

            double sum = gt.Take(lt).Sum(c => c.Real);
            if (sum > 0)
            {
                for (int i = 0; i < lt; i++)
                    gt[i] /= sum;
            }

            Complex[] spectrum = new Complex[sampleLength];
            FourierTransformClass.Fourier(gt, ref spectrum, 1.0);

            return spectrum;
        }

        /// <summary>
        /// 1Ping分の波形に対し、BPF→時間方向デコンボリューション→必要に応じEnvelopeを適用する
        /// </summary>
        private static short[] DeconvolveWave(short[] lf, int sampleFreq, bool bpfFlag, int hpf, int lpf, bool envelopeFlag, Complex[] psfSpectrum, double gammaTime)
        {
            int n = lf.Length;
            if (n == 0) return Array.Empty<short>();

            var time = new Complex[n];
            for (int i = 0; i < n; i++)
                time[i] = new Complex(lf[i], 0);

            Complex[] signalFreq = new Complex[n];
            FourierTransformClass.Fourier(time, ref signalFreq, 1.0);

            if (bpfFlag)
            {
                double fLow = hpf * 1000.0;
                double fHigh = lpf * 1000.0;
                ApplyBpfInPlace(n, sampleFreq, fLow, fHigh, signalFreq);
            }

            var deconvFreq = new Complex[n];
            for (int k = 0; k < n; k++)
            {
                double denom = psfSpectrum[k].Magnitude * psfSpectrum[k].Magnitude + gammaTime;
                deconvFreq[k] = denom > 0 ? signalFreq[k] * Complex.Conjugate(psfSpectrum[k]) / denom : Complex.Zero;
            }

            Complex[] deconvTime = new Complex[n];
            FourierTransformClass.Fourier(deconvFreq, ref deconvTime, -1.0);

            if (envelopeFlag)
            {
                Complex[] analyticFreq = new Complex[n];
                FourierTransformClass.Fourier(deconvTime, ref analyticFreq, 1.0);
                ApplyHilbertMaskInPlace(n, analyticFreq);

                Complex[] analyticTime = new Complex[n];
                FourierTransformClass.Fourier(analyticFreq, ref analyticTime, -1.0);

                double[] magnitude = new double[n];
                for (int i = 0; i < n; i++)
                    magnitude[i] = analyticTime[i].Magnitude;

                return ToInt16(magnitude);
            }

            double[] real = new double[n];
            for (int i = 0; i < n; i++)
                real[i] = deconvTime[i].Real;

            return ToInt16(real);
        }

        private static short[] ToInt16(double[] values)
        {
            var ret = new short[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                double v = Math.Round(values[i]);
                v = Math.Clamp(v, short.MinValue, short.MaxValue);
                ret[i] = (short)v;
            }
            return ret;
        }
        #endregion
    }
}

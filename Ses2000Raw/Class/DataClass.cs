using MathNet.Numerics.Statistics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using ScottPlot.Plottable;
//using static MultiSBP.WaveData;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace Ses2000Raw
{
    public class DataClass
    {
        private static object m_objLock = new object();  // ロック処理要オブジェクト

        #region Public Method
        /// <summary>==
        /// 
        /// </summary>
        /// <param name="btRaw"></param>
        /// <param name="sign">符号</param>
        public static Complex[] FFT(int numSamples, ref double[] aryRaw/*, int sign*/)
        {
            // byte[]を複素数配列に変換
            Complex[] complexSrc = new Complex[numSamples];
            Complex[] complexFFT = new Complex[numSamples];

            // サンプル数が2のべき乗でないとFFTが使用できないので2048点処理する
            for (int s = 0; s < numSamples; s++)
            {
                // 複素数配列を生成
                complexSrc[s] = new Complex(aryRaw[s], 0);
            }
            double dFft = 1.0; // フーリエ変換
            FourierTransformClass.Fourier(complexSrc, ref complexFFT, dFft);
            return complexFFT;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpf">LPFのカットオフ周波数</param>
        /// <param name="hpf">HPFのカットオフ周波数</param>
        /// <param name="complexFFT"></param>
        /// <returns></returns>
        public static Complex[] Envelope(int n, ref Complex[] fft)
        {
            ApplyHilbertMaskInPlace(n, fft);

            var ifft = new Complex[n];
            FourierTransformClass.Fourier(fft, ref ifft, -1.0);
            return ifft;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numSamples">サンプル数（2のべき乗）</param>
        /// <param name="fs">サンプリングレート（Ksps）</param>
        /// <param name="lpf">LPFのカットオフ周波数</param>
        /// <param name="hpf">HPFのカットオフ周波数</param>
        /// <param name="applyEnvelope">エンベロープ実施フラグ</param>
        /// <param name="complexFFT"></param>
        /// <returns></returns>
        public static Complex[] Bpf(int n, double fsHz, int lpf_kHz, int hpf_kHz, ref Complex[] fft)
        {
            double fLow = hpf_kHz * 1000.0;
            double fHigh = lpf_kHz * 1000.0;

            ApplyBpfInPlace(n, fsHz, fLow, fHigh, fft);

            var ifft = new Complex[n];
            FourierTransformClass.Fourier(fft, ref ifft, -1.0);
            return ifft;
        }
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
        //private static void ApplyBpfInPlace(int n, double fsHz, double fLowHz, double fHighHz, Complex[] X)
        //{
        //    if (X.Length != n) throw new ArgumentException("length mismatch");
        //    if (!(0 <= fLowHz && fLowHz < fHighHz && fHighHz <= fsHz * 0.5))
        //        throw new ArgumentOutOfRangeException(nameof(fHighHz), "0 <= fLow < fHigh <= fs/2");

        //    double df = fsHz / n;
        //    int n2 = n / 2;

        //    for (int k = 0; k < n; k++)
        //    {
        //        // 負側も正側に写して判定
        //        double f = (k <= n2) ? (k * df) : ((n - k) * df);
        //        bool pass = (f >= fLowHz && f <= fHighHz);
        //        if (!pass) X[k] = Complex.Zero;
        //    }
        //}

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
        public static Result SignalProcessParallel(SynchronizationContext context, IProgress<int> progress, List<DataBlock> dataBlockList, int sampleFreq, bool bpfFlag, int hpf, int lpf, bool envelopeFlag, bool deconvoFlag)
        {
            Result ret = new Result();
            int iNumPing = dataBlockList.Count;
            var processed = new short[iNumPing][];
            int iProgressCnt = 0;
            try
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
    }
}

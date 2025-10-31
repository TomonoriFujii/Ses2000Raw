using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;
using System.Collections.Concurrent;
using static System.Windows.Forms.AxHost;
using System.Net.NetworkInformation;
//using ScottPlot.Plottable;
//using static MultiSBP.WaveData;
using System.Diagnostics;
using MathNet.Numerics.Statistics;

namespace Ses2000Raw
{
    public class DataClass
    {
        private static object m_objLock = new object();  // ロック処理要オブジェクト

        #region 定数
        /// <summary>
        /// 1ユニットあたりのチャンネル数(Full)
        /// </summary>
        public const short RX_CH_FULL = 48;
        /// <summary>
        /// 1ユニットあたりのチャンネル数(Half)
        /// </summary>
        public const short RX_CH_HALF = 24;
        /// <summary>
        /// バイナリデータのヘッダ部（時刻情報部）のサイズ(byte)
        /// </summary>
        public const short HEADER_SIZE = 512;
        /// <summary>
        /// 12チャンネル分のデータサイズ(byte)
        /// <remarks>12CH * 2560samples * 2byte</remarks>
        /// </summary>
        public const int DATA_SIZE_12CH = 61440;
        /// <summary>
        /// 48チャンネル分のデータサイズ(byte)
        /// <remarks>48CH * 2560samples * 2byte</remarks>
        /// </summary>
        public const int DATA_SIZE_48CH = 245760;
        /// <summary>
        /// Pingデータサイズ（byte)
        /// </summary>
        public const int PING_DATA_SIZE = HEADER_SIZE + DATA_SIZE_48CH;
        /// <summary>
        /// 前後のCH、左右のCHと符合が反転しているための対処
        /// </summary>
        public static int[] SIGN_SIGNAL = { 1, -1,  1, -1,  1, -1,  1, -1,  1, -1,  1, -1,
                                           -1,  1, -1,  1, -1,  1, -1,  1, -1,  1, -1,  1,
                                            1, -1,  1, -1,  1, -1,  1, -1,  1, -1,  1, -1,
                                           -1,  1, -1,  1, -1,  1, -1,  1, -1,  1, -1,  1 };
        /// <summary>
        /// MB-P-SBP収録パラメータファイル名
        /// </summary>
        public static string PARAMETER_FILE_NAME = "Parameter.txt";

        //public static int BEAM_55 = 55;

        /// <summary>
        /// MB-P-SBPの55ビームのオフセット情報
        /// </summary>
        public static double[] PSBP_LINE_OFFSET = {-1.045, -1.0075, -0.970, -0.9325, -0.895, -0.8575, -0.820, -0.7825,
                                                   -0.745, -0.7075, -0.670, -0.6325, -0.595, -0.5575, -0.520, -0.4825,
                                                   -0.445, -0.4075, -0.370, -0.3325, -0.295, -0.2575, -0.220, -0.1825,
                                                   -0.145, -0.1075, -0.070, -0.0325,  0.005,  0.0425,  0.080,  0.1175,
                                                    0.155,  0.1925,  0.230,  0.2675,  0.305,  0.3425,  0.380,  0.4175,
                                                    0.455,  0.4925,  0.530,  0.5675,  0.605,  0.6425,  0.680,  0.7175,
                                                    0.755,  0.7925,  0.830,  0.8675,  0.905,  0.9425,  0.908};
        /// <summary>
        /// Acoustic Moleの64ビームのオフセット情報
        /// </summary>
        public static double[] AMOLE_LINE_OFFSET = {-0.063, -0.061, -0.059, -0.057, -0.055, -0.053, -0.051, -0.049,
                                                    -0.047, -0.045, -0.043, -0.041, -0.039, -0.037, -0.035, -0.033,
                                                    -0.031, -0.029, -0.027, -0.025, -0.023, -0.021, -0.019, -0.017,
                                                    -0.015, -0.013, -0.011, -0.009, -0.007, -0.005, -0.003, -0.001,
                                                     0.001,  0.003,  0.005,  0.007,  0.009,  0.011,  0.013,  0.015,
                                                     0.017,  0.019,  0.021,  0.023,  0.025,  0.027,  0.029,  0.031,
                                                     0.033,  0.035,  0.037,  0.039,  0.041,  0.043,  0.045,  0.047,
                                                     0.049,  0.051,  0.053,  0.055,  0.057,  0.059,  0.061,  0.063};
        #endregion

        #region プロパティ
        /// <summary>
        /// Propertyを取得または設定します。
        /// </summary>
        public static PropertyClass Property { get; set; }
        /// <summary>
        /// Eventリストを取得または設定します。
        /// </summary>
        //public static EventClass Event { get; set; }
        public static List<TargetClass> TargetList { get; set; }
        /// <summary>
        /// Waveを取得または設定します
        /// </summary>
        public static List<WaveClass> WaveList { get; set; }
        public static double[,,] Waves { get; set; }
        //public static List<ProcessedWave> ProcessedWaveList { get; set; }

        public static List<ProcessedHistory> ProcessedHistoryList { get; set; }

        public static int ProcessedHistoryId { get; set; }

        public static ImportParam ImportParam { get; set; }
        #endregion

        #region Public Method for MB-P-SBP
        /// <summary>
        /// Parameter.txtを読み込みPropertyClassへ格納する
        /// </summary>
        /// <param name="paramFile">Parameter.txtファイルのフルパス</param>
        /// <param name="property">結果を格納するPropertyClass</param>
        /// <param name="numUnit"></param>
        //public static bool ReadParameterFile(string paramFile, PropertyClass property,
        //                                        ref int numUnit, ref int pingCycle, ref RecordModeKind recordMode, ref int numRxCh)
        //{
        //    try
        //    {
        //        // 「Setting.log」読み込み
        //        using (StreamReader sr = new StreamReader(paramFile, Encoding.GetEncoding("shift_jis")))
        //        {
        //            string strLine = null;
        //            while ((strLine = sr.ReadLine()) != null)
        //            {
        //                string[] aryLine = strLine.Split(',');
        //                for (int i = 0; i < aryLine.Length; i++)
        //                {
        //                    strLine = aryLine[i].Trim();

        //                    if (strLine.Contains("Tx Wave Form:"))
        //                        // 送信波形フォーム名
        //                        //property.TxWaveForm = strLine.Split(':').Last().Trim();
        //                        continue;
        //                    else if (strLine.Contains("Unit:"))
        //                    {
        //                        // ユニット数
        //                        //property.NumUnit = int.Parse(strLine.Split(':').Last().Trim());
        //                        numUnit = int.Parse(strLine.Split(':').Last().Trim());
        //                        property.NumLine = (numUnit * 8) - 1;
        //                    }
        //                    else if (strLine.Contains("Sampling Rate:"))
        //                        // サンプリングレート
        //                        property.SamplingRate = int.Parse(Regex.Replace(strLine, @"[^0-9]", ""));
        //                    else if (strLine.Contains("Channel:"))
        //                    {
        //                        // 受信CH数
        //                        if (strLine.Split(':').Last().Trim() == "Full") numRxCh = 48;
        //                        else numRxCh = 24;
        //                        continue;
        //                    }
        //                    else if (strLine.Contains("Offset:"))
        //                        // 受信開始遅延時間
        //                        property.RxOffset = int.Parse(Regex.Replace(strLine.Split('(').Last().Trim(), @"[^0-9]", ""));
        //                    else if (strLine.Contains("Term:"))
        //                        // 受信期間
        //                        property.RxTerm = int.Parse(Regex.Replace(strLine.Split('(').Last().Trim(), @"[^0-9]", ""));
        //                    else if (strLine.Contains("Rec Mode:"))
        //                    {
        //                        // 収録モード
        //                        if (strLine.Split(':').Last().Trim() == "1st + 2nd + 3rd") recordMode = RecordModeKind.Ping_1st_2nd_3rd;
        //                        else recordMode = RecordModeKind.Ping_2nd_Only;
        //                        continue;
        //                    }
        //                    else if (strLine.Contains("Ping:"))
        //                    {
        //                        // PINGモード
        //                        //if (strLine.Split(':').Last().Trim() == "Single") property.PingMode = PingModeKind.Single;
        //                        //else property.PingMode = PingModeKind.Multi;
        //                        continue;
        //                    }
        //                    else if (strLine.Contains("Ping Interval:"))
        //                    {
        //                        string[] aryTmp = strLine.Split("]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        //                        //// PING間隔
        //                        //property.PingInterval = int.Parse(Regex.Replace(aryTmp.First(), @"[^0-9]", ""));
        //                        // PING周期
        //                        pingCycle = int.Parse(Regex.Replace(aryTmp.Last(), @"[^0-9]", ""));
        //                        continue;
        //                    }
        //                    else if (strLine.Contains("Rx Gain:"))
        //                        //property.RxGain = strLine.Split(':').Last().Trim();
        //                        continue;
        //                    else if (strLine.Contains("Tx Level:"))
        //                        //property.TxLevel = strLine.Split(':').Last().Trim();
        //                        continue;
        //                    else if (strLine.Contains("Lower limit of Ping cycle:"))
        //                        // PING周期下限値
        //                        //property.PingCycleLowLimit = int.Parse(Regex.Replace(strLine.Split(':').Last().Trim(), @"[^0-9]", ""));
        //                        continue;
        //                }
        //            }
        //            property.NumSample = (int)(property.SamplingRate * property.RxTerm);  // サンプル数
        //            sr.Close();
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Msg.ErrProcess(true, true, "「" + DataClass.PARAMETER_FILE_NAME + "」のフォーマットが不正です。", "DataClass.ReadParameterFile", ex.Message, string.Empty);
        //        return false;
        //    }
        //}
        ///// <summary>
        ///// HSXファイルの読み込み
        ///// </summary>
        ///// <param name="hsxFilePath">HSXファイルのフルパス</param>
        ///// <param name="navDeviceNo">Navigation Device No</param>
        ///// <param name="positionList">Positionを格納するList配列</param>
        ///// <param name="eventClass">Eventを格納するList配列</param>
        ///// <param name="speedList">Speedを格納するList配列</param>
        //public static bool ReadHypack(string hsxFilePath, int navDeviceNo, List<PositionClass> positionList, List<HeadingClass> headingList, 
        //                            List<JyroClass> jyroList, List<EventClass> eventList, List<SpeedClass> speedList, double lineStartTime = 0, double lineEndTime = 86400)
        //{
        //    try
        //    {
        //        string strLine;
        //        int iEventCnt = 0;
        //        using (System.IO.StreamReader sr = new StreamReader(hsxFilePath/*, Encoding.GetEncoding("Shift_JIS")*/))
        //        {
        //            while ((strLine = sr.ReadLine()) != null)
        //            {
        //                string[] strCol = strLine.Split(' ');

        //                // イベント
        //                if (eventList != null)
        //                {
        //                    if (strCol[0].Equals("FIX", StringComparison.OrdinalIgnoreCase) && strCol.Length == 4)
        //                    {
        //                        if (iEventCnt == 0)
        //                        {
        //                            // 収録開始のFIXはスルー
        //                            iEventCnt++;
        //                            continue;
        //                        }
        //                        else
        //                        {
        //                            EventClass evnt = new EventClass();
        //                            evnt.time = double.Parse(strCol[2]);
        //                            evnt.event_no = int.Parse(strCol[3]);

        //                            if (lineStartTime > evnt.time || lineEndTime < evnt.time) continue;

        //                            eventList.Add(evnt);
        //                            iEventCnt++;
        //                            continue;
        //                        }
        //                    }
        //                }
        //                // XY座標
        //                if (positionList != null)
        //                {
        //                    if (strCol[0].Equals("POS", StringComparison.OrdinalIgnoreCase) &&
        //                        (navDeviceNo >= 0 ? strCol[1].Equals(navDeviceNo.ToString()) : true) &&
        //                        strCol.Length == 5)
        //                    {
        //                        PositionClass pos = new PositionClass();
        //                        pos.time = double.Parse(strCol[2]);
        //                        pos.easting = double.Parse(strCol[3]);
        //                        pos.northing = double.Parse(strCol[4]);

        //                        if (lineStartTime > pos.time || lineEndTime < pos.time) continue;

        //                        #region ひとつ前の座標との間を内分補間する（5等分 10Hz→50Hz相当へ）
        //                        if (positionList.Count > 0)
        //                        {
        //                            PositionClass prePos = positionList.Last();
        //                            //PositionClass interpolatePos = new PositionClass();

        //                            int iDevCnt = 5;
        //                            //int iDevCnt = 10;
        //                            double dTimeOffset = (pos.time - prePos.time) / (double)iDevCnt;
        //                            int m, n;

        //                            // 線分ABをm:nに内分する座標P = (nA + mB) / (m + n) で求まる
        //                            for (int i = 0; i < iDevCnt - 1; i++)
        //                            {
        //                                m = i + 1;
        //                                n = iDevCnt - m;

        //                                PositionClass interpolatePos = new PositionClass();
        //                                interpolatePos.easting = ((n * prePos.easting) + (m * pos.easting)) / (m + n);
        //                                interpolatePos.northing = ((n * prePos.northing) + (m * pos.northing)) / (m + n);
        //                                interpolatePos.time = prePos.time + (m * dTimeOffset);
        //                                positionList.Add(interpolatePos);

        //                                //Debug.WriteLine("Interpolate\t{0}\t{1}\t{2}", interpolatePos.time, interpolatePos.easting, interpolatePos.northing);
        //                            }
        //                        }
        //                        #endregion

        //                        positionList.Add(pos);
        //                        //Debug.WriteLine("org\t{0}\t{1}\t{2}", pos.time, pos.easting, pos.northing);
        //                        continue;
        //                    }
        //                }
        //                // 方位
        //                if (headingList != null)
        //                {
        //                    if (strCol[0].Equals("GYR", StringComparison.OrdinalIgnoreCase) &&
        //                        strCol.Length == 4)
        //                    {
        //                        HeadingClass h = new HeadingClass();
        //                        h.time = double.Parse(strCol[2]);
        //                        h.heading = double.Parse(strCol[3]);

        //                        if (lineStartTime > h.time || lineEndTime < h.time) continue;

        //                        #region ひとつ前の座標との間を内分補間する（5等分 10Hz→50Hz相当へ）
        //                        if (headingList.Count > 0)
        //                        {
        //                            HeadingClass preHead = headingList.Last();

        //                            int iDevCnt = 5;
        //                            double dTimeOffset = (h.time - preHead.time) / (double)iDevCnt;

        //                            double dDeltaH;
        //                            if(h.heading - preHead.heading >180)
        //                            {
        //                                if(h.heading > preHead.heading)
        //                                    dDeltaH = -(((360d - h.heading) + preHead.heading) / (double)iDevCnt);
        //                                else
        //                                    dDeltaH = ((360d - preHead.heading) + h.heading) / (double)iDevCnt;
        //                            }
        //                            else
        //                            {
        //                                dDeltaH = (h.heading - preHead.heading) / (double)iDevCnt;
        //                            }

        //                            for (int i = 0; i < iDevCnt - 1; i++)
        //                            {
        //                                HeadingClass interpolateHead = new HeadingClass();
        //                                interpolateHead.heading = preHead.heading + ((i + 1d) * dDeltaH);
        //                                if (interpolateHead.heading < 0) interpolateHead.heading += 360d;
                                        
        //                                interpolateHead.time = preHead.time + ((i + 1) * dTimeOffset);
        //                                headingList.Add(interpolateHead);
        //                                //Debug.WriteLine("interpolate," + interpolateHead.heading);
        //                            }
        //                        }
        //                        #endregion

        //                        headingList.Add(h);
        //                        //Debug.WriteLine("orginal," + h.heading);
        //                        continue;
        //                    }
        //                }
        //                // モーション
        //                if (jyroList != null)
        //                {
        //                    if (strCol[0].Equals("HCP", StringComparison.OrdinalIgnoreCase) &&
        //                        strCol.Length == 6)
        //                    {
        //                        JyroClass jyro = new JyroClass();
        //                        jyro.time = double.Parse(strCol[2]);
        //                        jyro.heave = double.Parse(strCol[3]);   // 単位:m
        //                        jyro.roll = double.Parse(strCol[4]);    // + port side up
        //                        jyro.pitch = double.Parse(strCol[5]);   // + bow up

        //                        if (lineStartTime > jyro.time || lineEndTime < jyro.time) continue;

        //                        jyroList.Add(jyro);
        //                        continue;
        //                    }
        //                }
        //                // 対地船速
        //                if (speedList != null)
        //                {
        //                    if (strCol[0].Equals("GPS", StringComparison.OrdinalIgnoreCase) &&
        //                        (navDeviceNo >= 0 ? strCol[1].Equals(navDeviceNo.ToString()) : true) &&
        //                        strCol.Length == 8)
        //                    {
        //                        SpeedClass spd = new SpeedClass();
        //                        spd.time = double.Parse(strCol[2]);
        //                        //spd.speed = double.Parse(strCol[4]) * 1852.0 / 3600;  // speed over ground(m/s)
        //                        spd.speed = double.Parse(strCol[4]);                    // speed over ground(knot)

        //                        if (lineStartTime > spd.time || lineEndTime < spd.time) continue;

        //                        speedList.Add(spd);
        //                        continue;
        //                    }
        //                }
        //            }
        //            sr.Close();
        //        }
        //        if (positionList.Count == 0)
        //        {
        //            Msg.MsgErrWarning("HSXファイルからNavigation情報が取得出来ませんでした。\nNavigation Device Numberが誤っている可能性があります。");
        //            return false;
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Msg.ErrProcess(true, true, "HSXファイルの読み込みに失敗しました。", "DataClass.ReadHsx", ex.Message, string.Empty);
        //        return false;
        //    }
        //}
        /// <summary>
        /// XY座標から船速を計算
        /// </summary>
        /// <param name="positionList"></param>
        /// <param name="speedList"></param>
        public static void CalcShipSpeed(List<PositionClass> positionList, List<SpeedClass> speedList, out double speedAvg)
        {
            double t0, t1;
            double x0, y0;
            double x1, y1;
            double dDistance;
            double dSec;
            SpeedClass spd;
            double[] tmpArray = new double[positionList.Count];
            for (int i = 1; i < positionList.Count; i++)
            {
                t0 = positionList[i - 1].time;
                x0 = positionList[i - 1].easting;
                y0 = positionList[i - 1].northing;

                t1 = positionList[i].time;
                x1 = positionList[i].easting;
                y1 = positionList[i].northing;

                dSec = t1 - t0;
                dDistance = Math.Sqrt(((x0 - x1) * (x0 - x1)) + ((y0 - y1) * (y0 - y1)));

                double dSpeed = (dDistance / dSec) / 0.514; // knot

                if(i == 1)
                {
                    spd = new SpeedClass();
                    spd.time = t0;
                    spd.speed = dSpeed;
                    speedList.Add(spd);
                    tmpArray[0] = dSpeed;

                    //Debug.WriteLine("Ping:{0},Speed:{1}", 1, dSpeed);
                }
                spd = new SpeedClass();
                spd.time = t1;
                spd.speed = dSpeed;
                speedList.Add(spd);
                tmpArray[i] = dSpeed;

                //Debug.WriteLine("Ping:{0},Speed:{1}", i+1, dSpeed);
            }


            double dAve = tmpArray.Mean();
            double dStdDev = tmpArray.StandardDeviation();
            List<double> tmpList= new List<double>();
            for(int i = 0; i < tmpArray.Length; i++)
            {
                double dTmp = tmpArray[i];
                if(dTmp >= (dAve - (1 * dStdDev)) && dTmp <= (dAve +(1 * dStdDev)))
                {
                    // 1σ区間に含まれる場合のみ追加
                    tmpList.Add(dTmp);
                }
            }
            speedAvg = tmpList.Average();
        }

        /// <summary>
        /// 収録した全UNITの全Pingデータを連想配列に読み込む
        /// </summary>
        /// <param name="progress">Progressクラス</param>
        /// <param name="dicDatFiles">読み込み対象のdatファイルパス配列</param>
        /// <param name="property">Propertyクラス</param>
        /// <param name="waveDataArray">Waveデータを格納する連想配列</param>
        /// <returns>Resultクラス</returns>
        //public static Result ReadAllWaveData(IProgress<int> progress, Dictionary<int, IEnumerable<string>> dicDatFiles, PropertyClass property, Dictionary<int, WaveData>[] waveDataArray)
        //{
        //    Result ret = new Result();
        //    int iProgressCnt = 0;
        //    int iRecordingNumPing = (int)property.RecordMode;
        //    int iNumRxChPerColumn = property.NumRxCh / 4;
        //    int iDataSizePerColumn = iNumRxChPerColumn * property.NumSamples * 2; // アレイ1列あたりのデータサイズ = CH数 × サンプル数 × 2byte

        //    try
        //    {
        //        //var sw = new Stopwatch();
        //        //sw.Start();

        //        /*
        //         * 通常ループ
        //         */
        //        for (int unit = 0; unit < dicDatFiles.Count; unit++)
        //        {
        //            if (frmProgress.Cancel) break;
        //            if (dicDatFiles[unit] == null) continue;

        //            IEnumerable<string> datFiles = dicDatFiles[unit];

        //            waveDataArray[unit] = new Dictionary<int, WaveData>();

        //            for (int ping = 0; ping < datFiles.Count(); ping++)
        //            {
        //                if (frmProgress.Cancel) break;

        //                string strDat = datFiles.ElementAt(ping);
        //                WaveData wave = new WaveData(property.RecordMode);

        //                //var sw = new Stopwatch();
        //                //sw.Start();
        //                ReadBinary(strDat, unit, iRecordingNumPing, property.NumSamples, iNumRxChPerColumn, iDataSizePerColumn, property.TimeDiff, ref wave);
        //                //sw.Stop();
        //                //Console.WriteLine(sw.ElapsedMilliseconds);

        //                waveDataArray[unit][ping] = wave;
        //            }
        //            iProgressCnt++;
        //            progress.Report(iProgressCnt);
        //        }
        //        //sw.Stop();
        //        //Console.WriteLine(sw.ElapsedMilliseconds);


        //        if (frmProgress.Cancel)
        //        {
        //            ret.RetCode = ResultKind.Cancel;
        //            ret.Message = "インポート処理がキャンセルされました。";
        //        }
        //        else
        //        {
        //            ret.RetCode = ResultKind.Ok;
        //            ret.Message = "インポート処理が正常終了しました。";
        //        }
        //        return ret;
        //    }
        //    catch (Exception er)
        //    {
        //        ret.RetCode = ResultKind.Error;
        //        ret.Message = "インポート処理中に予期せぬエラーが発生しました。\n" + er.Message;
        //        return ret;
        //    }
        //}
        /// <summary>
        /// 収録した全UNITの全Pingデータを連想配列に読み込む
        /// </summary>
        /// <param name="progress">Progressクラス</param>
        /// <param name="dicDatFiles">読み込み対象のdatファイルパス配列</param>
        /// <param name="property">Propertyクラス</param>
        /// <param name="waveDataTableArray">Waveデータを格納する連想配列</param>
        /// <returns>Resultクラス</returns>
        //public static Result ReadPSbpDataParallel(SynchronizationContext context, IProgress<int> progress, Dictionary<int, IEnumerable<string>> dicDatFiles, PropertyClass property,
        //                                            ConcurrentDictionary<int, WaveData>[] waveDataTableArray, List<HeadingClass> headingList, List<JyroClass> jyroList,
        //                                            RecordModeKind recordMode, int numRxCh, double diffTime)
        //{
        //    Result ret = new Result();
        //    int iProgressCnt = 0;
        //    int iRecordingNumPing = (int)recordMode;
        //    int iNumRxChPerColumn = numRxCh / 4;
        //    int iDataSizePerColumn = iNumRxChPerColumn * property.NumSample * 2; // アレイ1列あたりのデータサイズ = CH数 × サンプル数 × 2byte

        //    try
        //    {
        //        /*
        //         * 並列ループ
        //         */
        //        //ParallelOptions option = new ParallelOptions();
        //        //option.MaxDegreeOfParallelism = 3;    // Thread数を制限

        //        //Parallel.For(0, dicDatFiles.Count, option, (unit, state) =>
        //        Parallel.For(0, dicDatFiles.Count, (unit, state) =>
        //        {
                    

        //            if (FormProgress.Cancel) state.Break();

        //            if (dicDatFiles[unit] != null)
        //            {
        //                IEnumerable<string> datFiles = dicDatFiles[unit];

        //                // ConcurrentDictionary … スレッドセーフな連想配列
        //                waveDataTableArray[unit] = new ConcurrentDictionary<int, WaveData>();

        //                for (int ping = 0; ping < datFiles.Count(); ping++)
        //                {
        //                    if (FormProgress.Cancel) break;

        //                    // UIに同期させて通知
        //                    context.Post(progressBar =>
        //                    {
        //                        //this.progressBar1.Value = (int)progressBar;
        //                        progress.Report((int)progressBar);
        //                    }, Interlocked.Increment(ref iProgressCnt)); // ロックしてインクリメント

        //                    string strDat = datFiles.ElementAt(ping);
        //                    WaveData wave = new WaveData(recordMode);

        //                    //var sw = new Stopwatch();
        //                    //sw.Start();
        //                    ReadBinary(strDat, unit, iRecordingNumPing, property.NumSample, iNumRxChPerColumn, iDataSizePerColumn, diffTime, ref wave, headingList, jyroList);
        //                    //sw.Stop();
        //                    //Console.WriteLine(sw.ElapsedMilliseconds);

        //                    waveDataTableArray[unit][ping] = wave;
        //                }
        //            }
        //        });

        //        if (FormProgress.Cancel)
        //        {
        //            ret.RetCode = ResultCode.Cancel;
        //            ret.Message = "インポート処理がキャンセルされました。";
        //        }
        //        else
        //        {
        //            ret.RetCode = ResultCode.Ok;
        //            ret.Message = "インポート処理が正常終了しました。";
        //        }
        //        return ret;
        //    }
        //    catch (Exception er)
        //    {
        //        ret.RetCode = ResultCode.Error;
        //        ret.Message = "インポート処理中に予期せぬエラーが発生しました。\n" + er.Message;
        //        return ret;
        //    }
        //}
        ///// <summary>
        ///// 指定した受信ユニットの
        ///// </summary>
        ///// <param name="strDatFile">datファイルのフルパス</param>
        ///// <param name="unitNo">ユニット番号(0～6)</param>
        ///// <param name="recordNumPing">収録PING数(1st+2nd+3rd:3, 2ndPngOnly:1)</param>
        ///// <param name="numSamples">1CHあたりのサンプル数</param>
        ///// <param name="numRxChPerColumn">アレイ1列あたりのCH数</param>
        ///// <param name="dataSizePerColumn">アレイ1列辺りのデータ数</param>
        ///// <param name="timeDiff">Hypack PCとCST時間の時刻差</param>
        ///// <param name="waveData"></param>
        //public static bool ReadBinary(string strDatFile, int unitNo, int recordNumPing, int numSamples, int numRxChPerColumn, int dataSizePerColumn, double timeDiff,
        //    ref WaveData waveData, List<HeadingClass> headingList, List<JyroClass> jyroList)
        //{
        //    try
        //    {
        //        int iFftNs = (int)Math.Pow(2, (int)Math.Log(numSamples, 2));   // FFT実行サンプル数(2のべき乗）

        //        // .datを読み込み用に開く
        //        using (Stream stream = File.OpenRead(strDatFile))
        //        {
        //            // streamから読み込むためのBinaryReaderを作成
        //            using (BinaryReader reader = new BinaryReader(stream))
        //            {
        //                // 1stPing～3rdPingまでループ
        //                for (int i = 0; i < recordNumPing; i++)
        //                {
        //                    #region 時刻情報部読み込み
        //                    byte[] btHeader = new byte[HEADER_SIZE];
        //                    reader.Read(btHeader, 0, HEADER_SIZE);

        //                    int iHour = Convert.ToInt32(btHeader[0]);
        //                    int iMin = Convert.ToInt32(btHeader[3]);
        //                    int iSec = Convert.ToInt32(btHeader[2]);
        //                    short shMsec = BitConverter.ToInt16(btHeader, 4);

        //                    waveData.Pings[i].CstTime = (double)((iHour * 3600) + (iMin * 60) + iSec + (shMsec / 1000.0));
        //                    waveData.Pings[i].Time = waveData.Pings[i].CstTime + timeDiff;

        //                    //Console.WriteLine("Unit{0}, File:{1}, CstTime:{2}sec, Time:{3}", 
        //                    //    unitNo, Path.GetFileName(strDatFile), waveData.Pings[i].CstTime, waveData.Pings[i].Time);
        //                    #endregion

        //                    #region Jyro情報読み込み
        //                    if (unitNo == 0 && i == 0)
        //                    {
        //                        // Jyro情報は、UNIT0の1stPingのみ格納されている
        //                        string strHEHDT, strPHTRH;
        //                        HeadingClass h;
        //                        JyroClass j;
        //                        // $HEHDT(ex. 001/00:10:18,048 $HEHDT,298.29,T*17)
        //                        strHEHDT = System.Text.Encoding.ASCII.GetString(btHeader, 128, 64);
        //                        // $PHTRH(ex. 001/00:10:17,958 $PHTRH,1.81,M,1.69,B,0.16,O*09)
        //                        strPHTRH = System.Text.Encoding.ASCII.GetString(btHeader, 192, 64);

        //                        AnalizeJYRO(strHEHDT, strPHTRH, timeDiff, out h, out j);

        //                        headingList.Add(h);
        //                        jyroList.Add(j);
        //                    }
        //                    #endregion

        //                    #region PINGデータ部読み込み
        //                    byte[] btData;
        //                    int iIndex;

        //                    //var sw = new System.Diagnostics.Stopwatch();
        //                    //sw.Start();

        //                    // アレイ1列ごとに読み込む
        //                    for (int column = 0; column < 4; column++)
        //                    {
        //                        iIndex = 0;
        //                        btData = new byte[dataSizePerColumn];
        //                        reader.Read(btData, 0, dataSizePerColumn);

        //                        for (int trsIndex = 0; trsIndex < numSamples; trsIndex++)
        //                        {
        //                            for (int ch = 0; ch < numRxChPerColumn; ch++)
        //                            {
        //                                int iChIdx;
        //                                if (numRxChPerColumn == 12) iChIdx = ch + (12 * column);    // Rx 48CH受信の場合
        //                                else iChIdx = ch + 3 + (12 * column);                       // Rx 24CH受信の場合

        //                                if (trsIndex == 0)
        //                                    waveData.Pings[i].RawData[iChIdx] = new byte[numSamples * 2];

        //                                Array.Copy(btData, iIndex, waveData.Pings[i].RawData[iChIdx], trsIndex * 2, 2);
        //                                iIndex += 2;
        //                            }
        //                        }
        //                    }
        //                    //sw.Stop();
        //                    //Console.WriteLine(sw.ElapsedMilliseconds);
        //                }
        //                #endregion
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception er)
        //    {
        //        Console.WriteLine(er.Message);
        //        return false;
        //    }
        //}
        /// <summary>
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
        public static Complex[] Envelope(int numSamples, ref Complex[] complexFFT)
        {
            Complex[] complexIFFT = new Complex[numSamples];

            List<int> envelopeList = new List<int>(numSamples);
            int iNS2 = complexFFT.Length / 2;
            for (int i = 0; i < complexFFT.Length; i++)
            {
                if (i >= iNS2)
                    // 負の周波数域に0をセット(Envelope)
                    complexFFT[i] = new Complex(0.0, 0.0);
                //else
                //    // 正の周波数域を2倍
                //    complexFFT[i] = new Complex(complexFFT[i].Real * 2, complexFFT[i].Imaginary * 2);
            }

            double dFft = -1.0; // 逆フーリエ変換
            FourierTransformClass.Fourier(complexFFT, ref complexIFFT, dFft);
            return complexIFFT;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numSamples">サンプル数（2のべき乗）</param>
        /// <param name="f">サンプリングレート（Ksps）</param>
        /// <param name="lpf">LPFのカットオフ周波数</param>
        /// <param name="hpf">HPFのカットオフ周波数</param>
        /// <param name="applyEnvelope">エンベロープ実施フラグ</param>
        /// <param name="complexFFT"></param>
        /// <returns></returns>
        public static Complex[] Bpf(int numSamples, double f, int lpf, int hpf, ref Complex[] complexFFT)
        {
            Complex[] complexIFFT = new Complex[numSamples];

            List<int> bpfList = new List<int>(numSamples);
            double dF = f / numSamples;
            double dFreq;

            int iNS2 = complexFFT.Length / 2;
            for (int i = 0; i < complexFFT.Length; i++)
            {
                dFreq = (double)i * dF;     // 周波数

                if (i >= iNS2)
                {
                    if (dFreq > (f - hpf) || dFreq < (f - lpf))
                        // BPFの遮断帯域に0をセット
                        complexFFT[i] = new Complex(0.0, 0.0);
                }
                else
                {
                    if (dFreq > lpf || dFreq < hpf)
                        // BPFの遮断帯域に0をセット
                        complexFFT[i] = new Complex(0.0, 0.0);
                }
            }
            double dFft = -1.0; // 逆フーリエ変換
            FourierTransformClass.Fourier(complexFFT, ref complexIFFT, dFft);
            return complexIFFT;
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
        public static double[] SignalProcess(double[] fullWave, int fftNs, bool bpfFlag, int hpf, int lpf, bool envelopeFlag)
        {
            int iNs = fullWave.Length;                            // サンプル数
            double[] ret = new double[iNs];

            double[] aryTmp = new double[fftNs];
            fullWave.CopyTo(aryTmp, 0);

            // FFT
            Complex[] complexFFT = FFT(fftNs, ref aryTmp);
            Complex[] complexIFFT = null;

            // BPF
            if (bpfFlag)
                complexIFFT = Bpf(fftNs, Property.SamplingRate, lpf, hpf, ref complexFFT);

            // Envelope
            if (envelopeFlag)
                complexIFFT = Envelope(fftNs, ref complexFFT);

            for (int z = 0; z < iNs; z++)
            {
                if (envelopeFlag)
                    ret[z] = complexIFFT[z].Magnitude * 2;
                else
                    ret[z] = complexIFFT[z].Real;
            }
            return ret;
        }

        /// <summary>
        /// NMEA形式のStringからモーション情報を取得する
        /// </summary>
        /// <param name="strHEHDT">$HEHDT</param>
        /// <param name="strPHTRH">$PHTRH</param>
        /// <param name="waveData"></param>
        private static void AnalizeJYRO(string strHEHDT, string strPHTRH, double timeDiff, out HeadingClass heading, out JyroClass jyro)
        {
            int ifind;
            string[] aryStr;
            int iH, iM, iS, iMs;
            heading = new HeadingClass();
            jyro = new JyroClass();

            // 方位
            aryStr = strHEHDT.Split(new char[] { ',', ' ' });
            ifind = Array.IndexOf(aryStr, "$HEHDT");
            if (ifind > 0)
            {
                string strTmp = aryStr[0];  // ex. 001/00:10:18
                iH = int.Parse(strTmp.Substring(4, 2));
                iM = int.Parse(strTmp.Substring(7, 2));
                iS = int.Parse(strTmp.Substring(10, 2));
                iMs = int.Parse(aryStr[1]);
                heading.time = (double)((iH * 3600) + (iM * 60) + iS + (iMs / 1000.0)) + timeDiff;
                heading.heading = double.Parse(aryStr[ifind + 1]);
            }

            // JYRO
            aryStr = strPHTRH.Split(new char[] { ',', ' ' });
            ifind = Array.IndexOf(aryStr, "$PHTRH");
            if (ifind > 0)
            {
                string strTmp = aryStr[0];  // ex. 001/00:10:18
                iH = int.Parse(strTmp.Substring(4, 2));
                iM = int.Parse(strTmp.Substring(7, 2));
                iS = int.Parse(strTmp.Substring(10, 2));
                iMs = int.Parse(aryStr[1]);
                jyro.time = (double)((iH * 3600) + (iM * 60) + iS + (iMs / 1000.0)) + timeDiff;

                jyro.pitch = double.Parse(aryStr[ifind + 1]);
                if (aryStr[ifind + 2] == "P") jyro.pitch = -jyro.pitch;

                jyro.roll = double.Parse(aryStr[ifind + 3]);
                if (aryStr[ifind + 4] == "B") jyro.roll = -jyro.roll;

                jyro.heave = double.Parse(aryStr[ifind + 5]);
                if (aryStr[ifind + 6].Substring(0, 1) == "U")
                    jyro.heave = -jyro.heave;
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
        //public static Result SignalProcessParallel(SynchronizationContext context, IProgress<int> progress, PropertyClass prop, double[,,] fullWaves, int fftNs, bool bpfFlag, int hpf, int lpf, bool envelopeFlag)
        //{
        //    Result ret = new Result();
        //    List<WaveClass> envWaveList = new List<WaveClass>();
        //    int iProgressCnt = 0;
        //    try
        //    {
        //        Parallel.For(0, prop.NumPing, (y, state) =>
        //        {
        //            if (FormProgress.Cancel) state.Break();

        //            //int iLineIdx = y * prop.NumLine;

        //            for (int x = 0; x < prop.NumLine; x++)
        //            {
        //                if (FormProgress.Cancel) break;

        //                // UIに同期させて通知
        //                context.Post(progressBar =>
        //                {
        //                    progress.Report((int)progressBar);
        //                }, Interlocked.Increment(ref iProgressCnt)); // ロックしてインクリメント

        //                double[] fullWave = new double[prop.NumSample];
        //                for(int z = 0; z < prop.NumSample; z++)
        //                {
        //                    fullWave[z] = fullWaves[x, y, z];
        //                }

        //                double[] envWaves = SignalProcess(fullWave, fftNs, false, 0, 0, true);
        //                WaveClass envWaveClass = new WaveClass();
        //                envWaveClass.PingNo = y;
        //                envWaveClass.LineNo = x;
        //                envWaveClass.Samples = envWaves;

        //                lock (m_objLock)
        //                {
        //                    // List配列はスレッドセーフではないため、
        //                    // 並列実行時はLockしないと意図しない結果になる
        //                    // 【補足】ConcurrentBag配列はロック不要
        //                    //DataClass.WaveList.Add(waveClass);
        //                    envWaveList.Add(envWaveClass);
        //                }
        //            }
        //        });
        //        if (FormProgress.Cancel)
        //        {
        //            ret.RetCode = ResultCode.Cancel;
        //            ret.Message = "インポート処理がキャンセルされました。";
        //        }
        //        else
        //        {
        //            ret.RetCode = ResultCode.Ok;
        //            ret.Message = "インポート処理が正常終了しました。";
        //            ret.Obj = envWaveList;
        //        }
        //        return ret;
        //    }
        //    catch (Exception er)
        //    {
        //        ret.RetCode = ResultCode.Error;
        //        ret.Message = "インポート処理中に予期せぬエラーが発生しました。\n" + er.Message;
        //        return ret;
        //    }
        //}
        #endregion
    }

    /// <summary>
    /// MB-P-SBPの波形およびジャイロデータを格納するためのクラス
    /// </summary>
    public class WaveData
    {
        public WaveData(RecordModeKind recMode)
        {
            Pings = new _Ping[(int)recMode];
            for (int i = 0; i < (int)recMode; i++)
            {
                Pings[i].RawData = new Dictionary<int, byte[]>();
            }
            Jyro = new _Jyro();
        }

        public _Ping[] Pings;
        public _Jyro Jyro;

        public struct _Ping
        {
            /// <summary>
            /// セシウム時計時刻
            /// </summary>
            public double CstTime;
            /// <summary>
            /// 補正済み時刻
            /// </summary>
            public double Time;
            public Dictionary<int, byte[]> RawData;　// key : ch, value : samples
        }

        public struct _Jyro
        {
            public double Heading;
            public double Roll;
            public double Pitch;
            public double Heave;
        }
    }

    /// <summary>
    /// t_propertyのデータ保持クラス
    /// </summary>
    [TypeConverter(typeof(DefinitionOrderTypeConverter))]
    public class PropertyClass
    {
        /// <summary>
        /// ID
        /// </summary>
        [Browsable(false)]
        public int Id { get; set; }
        /// <summary>
        /// プロジェクト名
        /// </summary>
        [Category("\t\t\t\tProject")]
        //[DisplayName("Survey Name")]
        [DisplayName("Project Name")]
        [ReadOnly(true)]
        public string SurveyName { get; set; }
        /// <summary>
        /// データ名
        /// </summary>
        [Category("\t\t\t\tProject")]
        //[DisplayName("Data Name")]
        [DisplayName("Discription")]
        [ReadOnly(true)]
        public string DataName { get; set; }
        /// <summary>
        /// バイナリブロックサイズ
        /// </summary>
        [Category("\t\t\t\tProject")]
        [DisplayName("Binary Block Size(bytes)")]
        [ReadOnly(true)]
        public int BinaryBlockSize { get; set; }
        /// <summary>
        /// ファイルフォーマットVer.
        /// </summary>
        [Category("\t\t\t\tProject")]
        [DisplayName("File Formtat Version")]
        [ReadOnly(true)]
        public double FormatVersion { get; set; }

        /// <summary>
        /// サンプリングレート
        /// </summary>
        [Category("\t\t\tRx Parameter")]
        [DisplayName("Sampling Rate(Ksps)")]
        [ReadOnly(true)]
        public int SamplingRate { get; set; }
        /// <summary>
        /// 受信開始遅延時間
        /// </summary>
        [Category("\t\t\tRx Parameter")]
        [DisplayName("Rx Offset Time(ms)")]
        [ReadOnly(true)]
        public double RxOffset { get; set; }
        /// <summary>
        /// 受信期間
        /// </summary>
        [Category("\t\t\tRx Parameter")]
        [DisplayName("RX Term(ms)")]
        [ReadOnly(true)]
        public double RxTerm { get; set; }

        [Category("\t\tData")]
        [DisplayName("X-Samples")]
        [ReadOnly(true)]
        public int NumLine { get; set; }
        [Category("\t\tData")]
        [DisplayName("Y-Samples")]
        [ReadOnly(true)]
        public int NumPing { get; set; }
        /// <summary>
        /// サンプル数/CH
        /// </summary>
        [Category("\t\tData")]
        [DisplayName("Z-Samples")]
        [ReadOnly(true)]
        public int NumSample { get; set; }

        [Category("\t\tData")]
        [DisplayName("X-Resolution(cm)")]
        [ReadOnly(true)]
        public double GridSizeX { get; set; }
        [Category("\t\tData")]
        [DisplayName("Y-Resolution(cm)")]
        [ReadOnly(true)]
        public double GridSizeY { get; set; }
        [Category("\t\tData")]
        [DisplayName("Z-Resolution(cm)")]
        [ReadOnly(true)]
        public double GridSizeZ { get; set; }

        /// <summary>
        /// 堆積物音速
        /// </summary>
        [Category("\tOther")]
        [DisplayName("Sediment SV(m/s)")]
        [ReadOnly(true)]
        public int SedimentSv { get; set; }
        

        /// <summary>
        /// 備考
        /// </summary>
        [Category("\tOther")]
        [DisplayName("Memo")]
        [ReadOnly(true)]
        public string Memo { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PropertyClass()
        {
            SurveyName = String.Empty;
            DataName = String.Empty;
            Memo = String.Empty;
        }
    }
    /// <summary>
    /// 収録パラメータを格納するクラス
    /// </summary>
    public class WaveClass
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int LineNo { get; set; }
        public int PingNo { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
        //public double[] Samples { get; set; }
        public double[] Samples { get; set; }

        public WaveClass()
        {
            Easting = null;
            Northing = null;
        }
    }
    public class EventClass
    {
        /// <summary>
        /// Time Tag
        /// </summary>
        public double time { get; set; }
        /// <summary>
        /// PING番号
        /// </summary>
        public int ping_no { get; set; }
        /// <summary>
        /// イベント番号
        /// </summary>
        public int event_no { get; set; }
        /// <summary>
        /// X座標
        /// </summary>
        public double easting { get; set; }
        /// <summary>
        /// Y座標
        /// </summary>
        public double northing { get; set; }
        /// <summary>
        /// 船速
        /// </summary>
        public double speed { get; set; }
    }

    public class TargetClass
    {
        /// <summary>
        /// Target名
        /// </summary>
        public string TargetName { get; set; }
        /// <summary>
        /// PingNo
        /// </summary>
        public int PingNo { get; set; }
        /// <summary>
        /// X座標
        /// </summary>
        public double Easting { get; set; }
        /// <summary>
        /// Y座標
        /// </summary>
        public double Northing { get; set; }
        /// <summary>
        /// Targetからの距離
        /// </summary>
        public double? Distance { get; set; }
    }
    public class PingAttr
    {
        public int pingNo { get; set; }
        public double heading { get; set; }
        public double roll { get; set; }
        public double pitch { get; set; }
        public double heave { get; set; }
        public double speed { get; set; }
    }

    public class ProcessedHistory
    {
        public int Id;
        public string ProcessName;
    }
    //public class ProcessedWave
    //{
    //    public int Id { get; set; }
    //    public int ProcessedHistoryId { get; set; }
    //    public int WaveId { get; set; }
    //    public int[] ProcessedWaves { get; set; }
    //    public int LineNo { get; set; }
    //    public int PingNo { get; set; }
    //}

    public class ProcessedWave
    {
        public int Id { get; set; }
        public int ProcessedHistoryId { get; set; }
        public int WaveId { get; set; }
        public int[] ProcessedWaves { get; set; }
        public int LineNo { get; set; }
        public int PingNo { get; set; }

    }

    public class HeadingClass
    {
        public double time { get; set; }
        public double heading { get; set; }
    }
    public class JyroClass
    {
        public double time { get; set; }
        public double roll { get; set; }
        public double pitch { get; set; }
        public double heave { get; set; }
    }
    public class PositionClass
    {
        /// <summary>
        /// Time Tag
        /// </summary>
        public double time { get; set; }
        /// <summary>
        /// X座標
        /// </summary>
        public double easting { get; set; }
        /// <summary>
        /// Y座標
        /// </summary>
        public double northing { get; set; }
    }


    public class SpeedClass
    {
        /// <summary>
        /// Time Tag
        /// </summary>
        public double time { get; set; }
        /// <summary>
        /// 船速(単位:knot)
        /// </summary>
        public double speed { get; set; }
    }

    public class LinePosition
    {
        /// <summary>
        /// PING番号
        /// </summary>
        public int pingNo { get; set; }
        /// <summary>
        /// ビーム番号 
        /// </summary>
        public int lineNo { get; set; }
        /// <summary>
        /// X座標
        /// </summary>
        public double easting { get; set; }
        /// <summary>
        /// Y座標
        /// </summary>
        public double northing { get; set; }
    }

    public class ImportParam
    {
        public bool ImportAll { get; set; }
        public int LineStart { get; set; }
        public int LineEnd { get; set; }
        public bool CenteredEvent { get; set; }
        public int PingRange { get; set; }
        public int PingStart { get; set; }
        public int PingEnd { get; set; }
        public int TraceStart { get; set; }
        public int TraceEnd { get; set; }
    }

    #region 列挙型
    public enum RecordModeKind : int
    {
        /// <summary>
        /// 2nd Only
        /// </summary>
        Ping_2nd_Only = 1,
        /// <summary>
        /// 1st+2nd+3rd
        /// </summary>
        Ping_1st_2nd_3rd = 3,
    }
    /// <summary>
    /// 
    /// </summary>
    public enum ExportKind : int
    {
        Navigation,
        Attitude
    }
    #endregion
}

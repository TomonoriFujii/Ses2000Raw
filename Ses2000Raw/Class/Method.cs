using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using OpenTK.Mathematics;

namespace Ses2000Raw
{
    public static class Method
    {
        #region 列挙型定数
        /// <summary>
        /// ShowWindowのコマンド定数
        /// </summary>
        public enum ShowWindowEnum : int
        {
            SW_HIDE = 0,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_MAX = 11
        }
        #endregion

        public static string ConvertDateString(string ddMMyyyy, string format)
        {
            if (DateTime.TryParseExact(
                                        ddMMyyyy,
                                        "dd.MM.yyyy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None,
                                        out DateTime dt))
            {
                return dt.ToString(format);
            }
            else
            {
                // 変換失敗
                return ddMMyyyy;
            }
        }

        /// <summary>
        /// short[] → double[] への変換
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static double[] ToDouble(short[] src)
        {
            int n = src.Length;
            double[] dst = new double[n];
            for (int i = 0; i < n; i++)
                dst[i] = src[i];
            return dst;
        }


        /// <summary>
        /// サンプリング周波数と音速からサンプリング間隔を計算する
        /// </summary>
        /// <param name="samplingFreqHz">サンプリング周波数 [Hz]</param>
        /// <param name="soundSpeed">音速 [m/s]</param>
        /// <returns>サンプリング間隔 [cm]</returns>
        public static double CalcSampleInterval(double samplingFreqHz, double soundSpeed)
        {
            // サンプリング間隔(cm) = 音速[m/s] * 100 / サンプリング周波数[Hz]
            return (soundSpeed * 100.0) / samplingFreqHz / 2.0d;
        }

        /// <summary>
        /// 指定したディレクトリとその中身を全て削除する
        /// </summary>
        public static void DeleteDirectory(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            //ディレクトリ以外の全ファイルを削除
            string[] filePaths = Directory.GetFiles(targetDirectoryPath);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            //ディレクトリの中のディレクトリも再帰的に削除
            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
            foreach (string directoryPath in directoryPaths)
            {
                DeleteDirectory(directoryPath);
            }

            //中が空になったらディレクトリ自身も削除
            Directory.Delete(targetDirectoryPath, false);
        }
        /// <summary>
        /// (srcX, srcY)を原点(0, 0)を中心に回転する
        /// </summary>
        /// <param name="heading"></param>
        /// <param name="srcX"></param>
        /// <param name="srcY"></param>
        /// <param name="destX"></param>
        /// <param name="destY"></param>
        public static void RotatePoint(double srcX, double srcY, double heading, out double destX, out double destY)
        {
            //double dHdtRadian = Method.ToRadian(-heading); // 数学座標系にするためマイナスを付ける
            double dHdtRadian = MathHelper.DegreesToRadians(-heading);// 数学座標系にするためマイナスを付ける

            // (dX,dY)を原点(0,0)を中心に回転
            destX = srcX * Math.Cos(dHdtRadian) - srcY * Math.Sin(dHdtRadian);
            destY = srcX * Math.Sin(dHdtRadian) + srcY * Math.Cos(dHdtRadian);
        }

        /// <summary>
        /// 実行中の同じアプリケーションのプロセスを取得する
        /// </summary>
        /// <returns></returns>
        public static Process GetPreviousProcess()
        {
            Process curProcess = Process.GetCurrentProcess();
            Process[] allProcess = Process.GetProcessesByName(curProcess.ProcessName);

            foreach (Process checkProcess in allProcess)
            {
                // 自分自身のプロセスIDは無視する
                if (checkProcess.Id != curProcess.Id)
                {
                    if (string.Compare(checkProcess.MainModule.FileName, curProcess.MainModule.FileName, true) == 0)
                        return checkProcess;
                }
            }
            return null;
        }
        /// ---------------------------------------------------------------------------
        /// <summary>
        /// 外部プロセスのウィンドウを起動する
        /// </summary>
        /// <param name="hWnd"></param>
        /// ---------------------------------------------------------------------------
        public static void WakeupWindow(IntPtr hWnd)
        {
            // メイン・ウィンドウが最小化されていれば元に戻す
            if (User32.IsIconic(hWnd))
            {
                User32.ShowWindowAsync(hWnd, (int)ShowWindowEnum.SW_RESTORE);
            }

            // メイン・ウィンドウを最前面に表示する
            User32.SetForegroundWindow(hWnd);
        }
        /// <summary>
        /// ディレクトリのサイズを取得する
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <returns></returns>
        public static long GetDirSize(DirectoryInfo dirInfo)
        {
            long fileSize = 0;

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                fileSize += file.Length;
            }
            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            {
                fileSize += GetDirSize(dir);
            }
            return fileSize;
        }

        /// <summary>
        /// ローテーションされたファイル名を返す
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="delimiter"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        public static string GetNumberingFileName(string srcFile, string delimiter, int digit)
        {
            string strDir = Path.GetDirectoryName(srcFile);
            string strFileName = Path.GetFileNameWithoutExtension(srcFile);
            string strExtension = Path.GetExtension(srcFile);

            int iIndex = 1;
            while (true)
            {
                string strWork = strDir + @"\" + strFileName + delimiter + iIndex.ToString().PadLeft(digit, '0') + strExtension;
                if(File.Exists(strWork))
                {
                    iIndex++;
                }
                else
                {
                    return strWork;
                }
            }
        }
    }

    /// <summary>
    /// プロパティグリッドのプロパティの並び順をソート
    /// </summary>
    public class DefinitionOrderTypeConverter : TypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            // TypeDescriptorを使用してプロパティ一覧を取得する
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);

            // プロパティ一覧をリフレクションから取得
            Type type = value.GetType();
            List<string> list = new List<string>();

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                list.Add(propertyInfo.Name);
            }

            // リフレクションから取得した順でソート
            return pdc.Sort(list.ToArray());
        }

        /// <summary>
        /// GetPropertiesをサポートしていることを表明する。
        /// </summary>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    /// <summary>
    /// User32クラス
    /// </summary>
    public class User32
    {
        #region 定数
        public const Int32 WM_COPYDATA = 0x4A;          // Value of WM_COPYDATA
        public const Int32 WM_SETREDRAW = 0x000B;
        public const int RET_SENDMESSAGE_FAILED = 0;    // SendMessage 失敗
        public const int RET_SENDMESSAGE_SUCCESS = 1;   // SendMessage 失敗
        public const int RET_SENDMESSAGE_INPROCESS = 2; // SendMessage 別のプロセスが処理中のため、受理できない
        #endregion

        #region 構造体
        /// ---------------------------------------------------------------------------
        /// <summary>
        /// ウィンドウサイズ構造体
        /// </summary>
        /// ---------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        /// <summary>
        /// COPYDATASTRUCT
        /// </summary>
        public struct COPYDATASTRUCT
        {
            public Int32 dwData;
            public Int32 cbData;
            public string lpData;

        }
        public struct COPYDATASTRUCT_EX
        {
            public Int32 dwData;
            public Int32 cbData;
            public IntPtr lpData;
        }
        #endregion


        #region 外部DLL関数
        /// ---------------------------------------------------------------------------
        /// <summary>
        /// 1 つまたは複数のウィンドウへ、指定されたメッセージを送信します
        /// </summary>
        /// <param name="hWnd">送信先ウィンドウのハンドル</param>
        /// <param name="Msg">メッセージ</param>
        /// <param name="wParam">メッセージの最初のパラメータ</param>
        /// <param name="lParam">メッセージの 2 番目のパラメータ</param>
        /// <returns></returns>
        /// ---------------------------------------------------------------------------
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern Int32 SendMessage(IntPtr hWnd, UInt32 Msg, UInt32 wParam, ref COPYDATASTRUCT lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern Int32 SendMessage(IntPtr hWnd, UInt32 Msg, UInt32 wParam, ref COPYDATASTRUCT_EX lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, UInt32 wParam, int lParam);

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// デスクトップウィンドウハンドル取得関数（プラットフォームSDK）
        /// </summary>
        /// <returns>デスクトップウィンドウのハンドルが返ります</returns>
        /// ---------------------------------------------------------------------------
        [DllImport("User32.Dll")]
        public static extern IntPtr GetDesktopWindow();

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// ウィンドウの移動
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="x">横方向の位置</param>
        /// <param name="y">縦方向の位置</param>
        /// <param name="nWidth">幅</param>
        /// <param name="nHeight">高さ</param>
        /// <param name="bRepaint">再描画オプション</param>
        /// <returns>
        /// 関数が成功すると、0 以外の値が返ります
        /// 関数が失敗すると、0 が返ります
        /// </returns>
        /// ---------------------------------------------------------------------------
        [DllImport("User32.dll")]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, int bRepaint);

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// クラス・ウィンドウ名からハンドルを取得：Unicode：Windows NT/2000 は Unicode 版と ANSI 版を実装
        /// </summary>
        /// <param name="lpszClass">クラス名</param>
        /// <param name="lpszWindow">ウィンドウ名</param>
        /// <returns>
        /// 関数が成功すると、指定したクラス名とウィンドウ名を持つウィンドウのハンドルが返ります
        /// 関数が失敗すると、NULL が返ります
        /// </returns>
        /// ---------------------------------------------------------------------------
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpszClass, string lpszWindow);

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// FindWindowの拡張関数
        /// </summary>
        /// <param name="hParent">検索する子ウィンドウの親ウィンドウのハンドル</param>
        /// <param name="hChildAfter">
        /// 子ウィンドウのハンドル
        /// NULL を指定すると、最初の子ウィンドウから検索が開始されます
        /// </param>
        /// <param name="lpszClass">クラス名</param>
        /// <param name="lpszWindow">ウィンドウ名</param>
        /// <returns>
        /// 関数が成功すると、指定したクラスとウィンドウ名を持つウィンドウのハンドルが返ります
        /// 関数が失敗すると、NULL が返ります
        /// </returns>
        /// ---------------------------------------------------------------------------
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hParent, IntPtr hChildAfter, string lpszClass, string lpszWindow);

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// 指定されたウィンドウが属するクラスの名前を取得します
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="sbClassName">バッファへのポインタ</param>
        /// <param name="lngMaxCount">バッファの長さ</param>
        /// <returns>
        /// 関数が成功すると、バッファにコピーされた TCHAR 値の数が返ります
        /// 関数が失敗すると、0 が返ります
        /// </returns>
        /// ---------------------------------------------------------------------------
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern long GetClassName(IntPtr hWnd, System.Text.StringBuilder sbClassName, long lngMaxCount);

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// ウィンドウハンドルよりウィンドウサイズを取得
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="rect">ウィンドウの座標値</param>
        /// <returns>
        /// 関数が成功すると、0 以外の値が返ります
        /// 関数が失敗すると、0 が返ります
        /// </returns>
        /// ---------------------------------------------------------------------------
        [DllImport("User32.Dll")]
        public static extern int GetWindowRect(IntPtr hWnd, out RECT rect);

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// ウィンドウをフォアグラウンドにし、アクティブにする
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <returns>
        /// ウィンドウがフォアグラウンドになったら、0 以外の値が返ります
        /// ウィンドウがフォアグラウンドにならなかった場合は、0 が返ります
        /// </returns>
        /// ---------------------------------------------------------------------------
        [DllImport("User32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// 指定されたウィンドウまたはコントロールで、
        /// マウス入力とキーボード入力を有効または無効にします
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="bEnable">入力を有効または無効にする</param>
        /// <returns>
        /// ウィンドウが既に無効になっている場合、0 以外の値が返ります
        /// ウィンドウが無効になっていなかった場合は 0 が返ります
        /// </returns>
        /// ---------------------------------------------------------------------------
        [DllImport("user32.dll")]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// 別のスレッドによって作成されたウィンドウの表示状態を設定します
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="nCmdShow">ウィンドウの表示状態</param>
        /// <returns>
        /// 設定前にウィンドウが可視だった場合、0 以外の値が返ります
        /// 設定前にウィンドウが隠れていた場合、0 が返ります
        /// </returns>
        /// ---------------------------------------------------------------------------
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// 指定されたウィンドウが最小化（ アイコン化）されているかどうかを調べます
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <returns>
        /// ウィンドウが最小化されているときは、0 以外の値が返ります
        /// ウィンドウが最小化されていないときは、0 が返ります
        /// </returns>
        /// ---------------------------------------------------------------------------
        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// 1 個のアイコンを破棄し、そのアイコンに割り当てられていたメモリを解放します。
        /// </summary>
        /// <param name="hIcon">アイコンのハンドル</param>
        /// <returns>関数が成功すると、0 以外の値が返ります。</returns>
        /// ---------------------------------------------------------------------------
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);
        #endregion
    }
}

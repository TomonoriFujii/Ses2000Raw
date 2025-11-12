using DarkForm;
using System.Security.Policy;

namespace Ses2000Raw
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            ToolStripProfessionalRenderer renderer = new VS2019Renderer(Color.FromArgb(241, 241, 241), new VS2019ColorTable());
            renderer.RoundedEdges = false;
            ToolStripManager.Renderer = renderer;

            if (!Properties.Settings.Default.IsSettingUpgrade)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.IsSettingUpgrade = true;
                Properties.Settings.Default.Save();
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
            //Application.Run(new Form1());
        }
    }

    public class Constant
    {
        /// <summary>
        /// 背景色
        /// </summary>
        public static Color BACKCOLOR = Color.FromArgb(37, 37, 38);
        /// <summary>
        /// 文字色
        /// </summary>
        public static Color FORECOLOR = Color.FromArgb(240, 240, 240);
        /// <summary>
        /// ボタンの背景色
        /// </summary>
        public static Color BUTTON_BACKCOLOR = Color.FromArgb(49, 54, 58);
        /// <summary>
        /// コンボボックス背景色
        /// </summary>
        public static Color COMBO_BACKCOLOR = Color.FromArgb(61, 61, 61);
        public static Color COMBO_FORECOLOR = Color.FromArgb(229, 229, 229);

        /// <summary>
        /// 共通ファイルフォーマットの拡張子
        /// </summary>
        public const string PROJECT_EXTENSION = ".msf";
        /// <summary>
        /// 共通ファイルフォーマットのバージョン
        /// </summary>
        public const double FORMAT_VERSION = 1.1;
        /// <summary>
        /// MB-P-SBPバイナリブロックサイズ(2byte)
        /// </summary>
        public const int MBPSBP_BIN_BLOCK_SIZE = 2;
        /// <summary>
        /// a-coreバイナリブロックサイズ(8byte)
        /// </summary>
        //public const int ACORE_BIN_BLOCK_SIZE = 8;
        public const int ACORE_BIN_BLOCK_SIZE = 4;
        public const int AMOLE_BIN_BLOCK_SIZE = 4;

    }

    public class Result
    {
        public ResultCode RetCode { get; set; }
        public string Message { get; set; }
        public object Obj { get; set; }

        public Result()
        {
            RetCode = ResultCode.Error;
            Message = String.Empty;
        }
    }

    public enum ResultCode : int
    {
        Ok = 0,
        Cancel,
        Error
    }
    /// <summary>
    /// 
    /// </summary>
    public class ExtractionInfo
    {
        public Channel Channel { get; set; }
        public int BeamId { get; set; }
        public int Angle { get; set; }
        public int FreqId { get; set; }
        public int Frequency { get; set; } // kHz
    }
    /// <summary>
    /// 
    /// </summary>
    public enum Channel
    {
        LF = 0,
        HF = 1
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ses2000Raw
{
    internal class ColorPalette
    {
        public static Dictionary<int, Color> Fire { get; set; }
        public static Dictionary<int, Color> Spectrum { get; set; }
        public static Dictionary<int, Color> RGB3_3_2 { get; set; }
        public static Dictionary<int, Color> Colors16 { get; set; }
        public static Dictionary<int, Color> BlueOrangeIcb { get; set; }
        public static Dictionary<int, Color> Gem { get; set; }
        public static Dictionary<int, Color> GreenFireBlue { get; set; }
        public static Dictionary<int, Color> Jet { get; set; }
        public static Dictionary<int, Color> OrangeHot { get; set; }
        public static Dictionary<int, Color> Phase { get; set; }
        public static Dictionary<int, Color> Royal { get; set; }
        public static Dictionary<int, Color> Sepia { get; set; }
        public static Dictionary<int, Color> Smart { get; set; }
        public static Dictionary<int, Color> Thal { get; set; }
        public static Dictionary<int, Color> UnionJack { get; set; }
        public static Dictionary<int, Color> Viridis { get; set; }

        /// <summary>
        /// 値をRGBカラーに変換
        /// </summary>
        /// <param name="value"></param>
        public static void ToColor2(double value, ref float grayR, ref float grayG, ref float grayB)
        {
            double dTmpVal = Math.Cos(4 * Math.PI * value);
            double dColVal = -dTmpVal / 2.0 + 0.5;

            if (value >= (4.0 / 4.0))
            {
                // 赤
                grayR = 1;
                grayG = 0;
                grayB = 0;
            }
            else if (value >= (3.0 / 4.0))
            {
                // 黄～赤
                grayR = 1;
                grayG = (float)dColVal;
                grayB = 0;
            }
            else if (value >= (2.0 / 4.0))
            {
                // 緑～黄
                grayR = (float)dColVal;
                grayG = 1;
                grayB = 0;
            }
            else if (value >= (1.0 / 4.0))
            {
                // 水～緑
                grayR = 0;
                grayG = 1;
                grayB = (float)dColVal;
            }
            else if (value >= (0.0 / 4.0))
            {
                // 青～水
                grayR = 0;
                grayG = (float)dColVal;
                grayB = 1;
            }
            else
            {
                // 青
                grayR = 0;
                grayG = 0;
                grayB = 1;
            }

            // Dr.Mizuno Original Color
            /*
            if (0 <= value && value <= 0.25)
            {
                grayR = 0;
                grayG = (float)(1 * Math.Sin(value * 2.0 * Math.PI));
                grayB = 1;
            }
            else if (0.25 < value && value <= 0.5)
            {
                grayR = 0;
                grayG = 1;
                grayB = (float)(1 * Math.Sin(value * 2.0 * Math.PI));
            }
            else if (0.5 < value && value <= 0.75)
            {
                grayR = (float)(-1 * Math.Sin(value * 2.0 * Math.PI));
                grayG = 1;
                grayB = 0;
            }
            else if (0.75 < value && value <= 1)
            {
                grayR = 1;
                grayG = (float)(-1 * Math.Sin(value * 2.0 * Math.PI));
                grayB = 0;
            }
            */
        }
        /// <summary>
        /// 値をカラーに変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="grayR"></param>
        /// <param name="grayG"></param>
        /// <param name="grayB"></param>
        public static void ToColor(double value, ColorMode type, ref float grayR, ref float grayG, ref float grayB)
        {
            if (type == ColorMode.Color1)
            {
                double dTmpVal = Math.Cos(4 * Math.PI * value);
                double dColVal = -dTmpVal / 2.0 + 0.5;

                if (value >= (4.0 / 4.0))
                {
                    // 赤
                    grayR = 1;
                    grayG = 0;
                    grayB = 0;
                }
                else if (value >= (3.0 / 4.0))
                {
                    // 黄～赤
                    grayR = 1;
                    grayG = (float)dColVal;
                    grayB = 0;
                }
                else if (value >= (2.0 / 4.0))
                {
                    // 緑～黄
                    grayR = (float)dColVal;
                    grayG = 1;
                    grayB = 0;
                }
                else if (value >= (1.0 / 4.0))
                {
                    // 水～緑
                    grayR = 0;
                    grayG = 1;
                    grayB = (float)dColVal;
                }
                else if (value >= (0.0 / 4.0))
                {
                    // 青～水
                    grayR = 0;
                    grayG = (float)dColVal;
                    grayB = 1;
                }
                else
                {
                    // 青
                    grayR = 0;
                    grayG = 0;
                    grayB = 1;
                }

            }
            else if (type == ColorMode.Color2)
            {
                // Dr. Mizuno Original Color
                if (0 <= value && value <= 0.25)
                {
                    grayR = 0;
                    grayG = (float)(1 * Math.Sin(value * 2.0 * Math.PI));
                    grayB = 1;
                }
                else if (0.25 < value && value <= 0.5)
                {
                    grayR = 0;
                    grayG = 1;
                    grayB = (float)(1 * Math.Sin(value * 2.0 * Math.PI));
                }
                else if (0.5 < value && value <= 0.75)
                {
                    grayR = (float)(-1 * Math.Sin(value * 2.0 * Math.PI));
                    grayG = 1;
                    grayB = 0;
                }
                else if (0.75 < value && value <= 1)
                {
                    grayR = 1;
                    grayG = (float)(-1 * Math.Sin(value * 2.0 * Math.PI));
                    grayB = 0;
                }
            }
            else
            {
                int key = (int)(value * 255d);
                if (key > 255) key = 255;
                if (key < 0) key = 0;
                Color col;
                switch (type)
                {
                    case ColorMode.Fire: col = Fire[key]; break;
                    case ColorMode.Spectrum: col = Spectrum[key]; break;
                    case ColorMode.RGB3_3_2: col = RGB3_3_2[key]; break;
                    case ColorMode.Colors16: col = Colors16[key]; break;
                    case ColorMode.BlueOrangeIcb: col = BlueOrangeIcb[key]; break;
                    case ColorMode.Gem: col = Gem[key]; break;
                    case ColorMode.GreenFireBlue: col = GreenFireBlue[key]; break;
                    case ColorMode.Jet: col = Jet[key]; break;
                    case ColorMode.OrangeHot: col = OrangeHot[key]; break;
                    case ColorMode.Phase: col = Phase[key]; break;
                    case ColorMode.Royal: col = Royal[key]; break;
                    case ColorMode.Sepia: col = Sepia[key]; break;
                    case ColorMode.Smart: col = Smart[key]; break;
                    case ColorMode.Thal: col = Thal[key]; break;
                    case ColorMode.UnionJack: col = UnionJack[key]; break;
                    default: col = Viridis[key]; break;
                }
                grayR = col.R / 255f;
                grayG = col.G / 255f;
                grayB = col.B / 255f;
            }
        }
        /// <summary>
        /// カラーテーブルを取得
        /// </summary>
        public static void LoadColorTable()
        {
            string[] list;
            #region カラーテーブルを連想配列に格納
            // Fire
            list = Properties.Resources.Fire.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Fire = ParseColorTable(list);
            // Spectrum
            list = Properties.Resources.Spectrum.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Spectrum = ParseColorTable(list);
            // 3-3-2 RGB
            list = Properties.Resources._3_3_2RGB.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            RGB3_3_2 = ParseColorTable(list);
            // 16 Colors
            list = Properties.Resources._16Colors.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Colors16 = ParseColorTable(list);
            // Blue Orange icb
            list = Properties.Resources.BlueOrangeIcb.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            BlueOrangeIcb = ParseColorTable(list);
            // Gem
            list = Properties.Resources.Gem.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Gem = ParseColorTable(list);
            // Green Fire Blue
            list = Properties.Resources.GreenFireBlue.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            GreenFireBlue = ParseColorTable(list);
            // Jet
            list = Properties.Resources.Jet.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Jet = ParseColorTable(list);
            // Orange Hot
            list = Properties.Resources.OrangeHot.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            OrangeHot = ParseColorTable(list);
            // Phase
            list = Properties.Resources.Phase.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Phase = ParseColorTable(list);
            // Royal
            //list = Properties.Resources.Royal.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            //list = Properties.Resources.Royal_lowBoost_gamma0p4.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            list = Properties.Resources.Royal_lowBoost_gamma0p5.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            //list = Properties.Resources.Royal_lowBoost_gamma0p7.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            //list = Properties.Resources.Royal_lowBoost_logA9.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Royal = ParseColorTable(list);
            // Sepia
            list = Properties.Resources.Sepia.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Sepia = ParseColorTable(list);
            // Smart
            list = Properties.Resources.Smart.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Smart = ParseColorTable(list);
            // Thal
            list = Properties.Resources.Thal.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Thal = ParseColorTable(list);
            // UnionJack
            list = Properties.Resources.UnionJack.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            UnionJack = ParseColorTable(list);
            // Viridis
            list = Properties.Resources.Viridis.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Viridis = ParseColorTable(list);
            #endregion
        }
        /// <summary>
        /// カラーをテーブルに格納
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static Dictionary<int, Color> ParseColorTable(string[] list)
        {
            Dictionary<int, Color> dicColor = new Dictionary<int, Color>();
            for (int i = 0; i < list.Length; i++)
            {
                if (i == 0) continue;   // ヘッダはスキップ

                string[] rgb = list[i].Split('\t');
                dicColor[int.Parse(rgb[0])] = Color.FromArgb(int.Parse(rgb[1]), int.Parse(rgb[2]), int.Parse(rgb[3]));
            }
            return dicColor;
        }
    }
    public enum ColorMode : int
    {
        Color1 = 0,
        Color2,
        Gray,
        Fire,
        Spectrum,
        RGB3_3_2,
        Colors16,
        BlueOrangeIcb,
        Gem,
        GreenFireBlue,
        Jet,
        OrangeHot,
        Phase,
        Royal,
        Sepia,
        Smart,
        Thal,
        UnionJack,
        Viridis
    }
}

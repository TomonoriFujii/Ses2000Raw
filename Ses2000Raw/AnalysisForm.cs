using DotSpatial.Symbology.Forms;
using MathNet.Numerics.LinearAlgebra.Factorization;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Diagnostics;
using ScottPlot;
using ScottPlot.Rendering;
using DotSpatial.Topology.Operation.Valid;
using PointShape = DotSpatial.Symbology.PointShape;

namespace Ses2000Raw
{
    public partial class AnalysisForm : DockContent
    {
        #region フィールド
        private int[] m_bottomIdx;         // Pingごとのボトム（サンプルindex）
        private bool m_bBottomDirty = true; // データ読込時や閾値変更時に再検出フラグ
        private double m_dSampleFreqHz;     // サンプリング周波数[Hz]
        private bool m_bApplyBpf = false;    // BPF適用フラグ
        private string? m_strScreenshotPath = null;
        private short m_sFullWaveAmpMax;
        private short m_sEnvelopeAmpMax;
        private string m_rawFileName = "";
        //private string m_rawFilePath = "";
        private string? m_CSVFilePath = null;

        private int m_clickStep = 0;
        private double? m_dBottomDepth = null;


        // サンプル間隔[m]の半分を許容幅に（端数吸収用）
        private double MeterTolerance() => (m_dZDistance / 100.0) * 0.5;

        // 左右反転フラグ
        private bool m_bFlipX = false;
        // 直近のコンテンツ全幅(px)を保持（反転マッピングで使用）
        private double m_contentW = 0.0;

        // MeasureStart の最小/最大[m]
        private double m_dMinStartMeters;
        private double m_dMaxStartMeters;

        // 縦オフセット（各Pingの開始深度をpxへ変換）
        private double[] m_offsetPxPerPing;
        private double m_dMinOffsetPx, m_dMaxOffsetPx;

        // 拡大率と比率
        private double m_dZoom = 1.0;   // 共通ズーム倍率
        private double m_dRatioX = 1.0; // 横方向の相対倍率（numScaleY）
        private double m_dRatioY = 1.0; // 縦方向の相対倍率（numScaleZ）
        private double m_dScaleX => m_dZoom * m_dRatioX;
        private double m_dScaleY => m_dZoom * m_dRatioY;

        // 距離（進行方向）
        private double[] m_cumDistM;  // 累積距離[m]
        private double m_dTotalDistM;  // 総距離[m]

        // テキストラベルのテクスチャキャッシュ
        private readonly Dictionary<string, (int tex, int w, int h)> _labelCache = new();

        // 波形ビューの状態
        private double? m_mouseDepthMeters;
        private int m_lastPlottedPing = -1;
        private ScottPlot.Plottables.HorizontalLine? m_depthGuideLine;
        private double? m_mouseContentX;
        private double? m_mouseContentY;

        private MapForm? m_frmMap;
        private (double X, double Y)?[]? m_pingPositions;
        private int m_lastMapCursorPing = -1;

        // ドラッグ・スクロール
        private bool m_bDragging = false;
        private Point m_dragStart;
        private double m_dScrollStartX, m_dScrollStartY;
        private double m_dScrollX; // 表示原点X（拡大後px）
        private double m_dScrollY; // 表示原点Y（拡大後px）

        private bool m_bDraggingAddContact = false;//! スクロール判定のためm_bDraggingAddContactを追加 1119で追加場所確認
        private bool m_doNotShowUsageAddContactForm = false; //! 再表示判定用
        private bool m_prevAddContactChecked = false;



        // スクロールバー
        private HScrollBar hScroll;
        private VScrollBar vScroll;

        // ビューポート（使うのは幅高のみ）
        private int m_iVpW, m_iVpH;

        // テクスチャ＆画像
        private int m_iTexId = 0;
        private int m_iTexWidth;   // = m_iPingNo
        private int m_iTexHeight;  // = m_iSampleNo
        private byte[] m_rgba;    // RGBA8
        private byte[] m_lut;     // 256*4 RGBA LUT
        private double[] m_attZ;  // 減衰係数Zごとのテーブル
        private bool m_bTextureDirty = true;
        private bool m_bLutDirty = true;

        // データメタ
        private int m_iPingNo;
        private int m_iSampleNo;
        private double m_dZDistance; // サンプル間隔[cm]

        // ファイル
        private FileHeader m_fileHeader;
        private List<BlockHeader> m_blockHeaderList;
        private List<DataBlock> m_dataBlockList;

        // プロパティ
        public FileHeader FileHeader
        {
            get { return m_fileHeader; }
            set { m_fileHeader = value; }
        }
        public List<BlockHeader> BlockHeaderList
        {
            get { return m_blockHeaderList; }
            set { m_blockHeaderList = value; }
        }
        public List<DataBlock> DataBlockList
        {
            get { return m_dataBlockList; }
            set { m_dataBlockList = value; }
        }
        public MapForm? MapView
        {
            get { return m_frmMap; }
            set { m_frmMap = value; }
        }

        public bool ToolStripButtonAddContactChecked
        {
            get => toolStripButtonAddContact.Checked;
            set => toolStripButtonAddContact.Checked = value;

        }

        public bool ToolStripButtonAddContactEnabled
        {
            get => toolStripButtonAddContact.Enabled;
            set => toolStripButtonAddContact.Enabled = value;
        }

        public DemodulationMode DemodulateMode
        {
            get
            {
                if (this.lblDemodulate.Tag == null) return DemodulationMode.None;
                switch (Convert.ToInt32(this.lblDemodulate.Tag))
                {
                    case 0: return DemodulationMode.None;
                    case 1: return DemodulationMode.Deconvolution;
                    case 2: return DemodulationMode.Envelope;
                    case 3: return DemodulationMode.DeconvoEnvelope;
                    default: return DemodulationMode.None;
                }
            }
            set
            {
                this.lblDemodulate.Tag = value;
                this.lblDemodulate.Text = value == DemodulationMode.Envelope ?
                    Properties.Resources.Envelope : Properties.Resources.FullWave;
            }
        }
        public int Hpf_kHz
        {
            get { return Convert.ToInt32(this.lblHPF.Text.Replace(" kHz", "")); }
            set { this.lblHPF.Text = $"{value} kHz"; }
        }
        public int Lpf_kHz
        {
            get { return Convert.ToInt32(this.lblLPF.Text.Replace(" kHz", "")); }
            set { this.lblLPF.Text = $"{value} kHz"; }
        }
        public bool ApplyBpf
        {
            get { return m_bApplyBpf; }
            set
            {
                m_bApplyBpf = value;
                this.lblHPF.Enabled = m_bApplyBpf;
                this.lblLPF.Enabled = m_bApplyBpf;
            }
        }
        public string? CSVFilePath
        {
            get => m_CSVFilePath;
        }

        // GLコントロール状態
        private bool m_bLoadTK = false;
        private Channel m_channel;
        #endregion



        #region 初期化
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="title"></param>
        public AnalysisForm(string title, Channel channel, string RawFileName)
        {
            InitializeComponent();
            this.Text = title;
            m_rawFileName = RawFileName;
            //m_rawFilePath = Path.GetDirectoryName(RawFileName);



            m_blockHeaderList = new List<BlockHeader>();
            m_dataBlockList = new List<DataBlock>();

            this.BackColor = Constant.BACKCOLOR;
            this.ForeColor = Constant.FORECOLOR;

            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += (s, e) =>
            {
                var tab = tabControl1.TabPages[e.Index];
                var rect = e.Bounds;

                // 背景色
                using (var brush = new SolidBrush(Constant.BACKCOLOR))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                // テキスト
                TextRenderer.DrawText(
                    e.Graphics,
                    tab.Text,
                    tab.Font,
                    rect,
                    Constant.FORECOLOR,
                    TextFormatFlags.WordEllipsis | TextFormatFlags.VerticalCenter
                );
            };
            tabControl1.DrawItem += tabControl1_DrawItem;
            foreach (TabPage tab in this.tabControl1.TabPages)
            {
                tab.BackColor = Constant.BACKCOLOR;
            }
            this.grpBoxSignal.ForeColor = Constant.FORECOLOR;
            this.grpBox3DFrustum.ForeColor = Constant.FORECOLOR;
            this.grpBox3DMoving.ForeColor = Constant.FORECOLOR;
            //this.groupBox3.ForeColor = Constant.FORECOLOR;
            this.grpBoxColor.ForeColor = Constant.FORECOLOR;
            this.grpBoxDisplay.ForeColor = Constant.FORECOLOR;
            this.grpBoxSignal.ForeColor = Constant.FORECOLOR;
            this.cmbColor.BackColor = Constant.COMBO_BACKCOLOR;
            this.cmbColor.ForeColor = Constant.COMBO_FORECOLOR;
            this.btnChooseColor.BackColor = Constant.BUTTON_BACKCOLOR;
            this.btnScaleSetting.BackColor = Constant.BUTTON_BACKCOLOR;

            // 1) 線色のパレットをダーク向けに
            formsPlot1.Plot.Add.Palette = new ScottPlot.Palettes.Penumbra();

            // 2) 背景色（図全体・データ領域）
            formsPlot1.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#181818");
            formsPlot1.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#1f1f1f");

            // 3) 軸とグリッド（明るい色でコントラスト）
            formsPlot1.Plot.Axes.Color(ScottPlot.Color.FromHex("#d7d7d7"));
            formsPlot1.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#404040");

            // 4) 凡例
            formsPlot1.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#404040");
            formsPlot1.Plot.Legend.FontColor = ScottPlot.Color.FromHex("#d7d7d7");
            formsPlot1.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#d7d7d7");

            // 5) 反映
            formsPlot1.Refresh();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalysisForm_Load(object sender, EventArgs e)
        {
            m_dSampleFreqHz = m_blockHeaderList[0].SampleFrequencyForLf; // Hz
            double dSV = m_blockHeaderList[0].SoundVelocity;    // m/s
            m_dZDistance = Method.CalcSampleInterval(m_dSampleFreqHz, dSV);
            m_iSampleNo = (m_channel == Channel.LF) ? m_blockHeaderList[0].LfDataLength : m_blockHeaderList[0].HfDataLength;
            m_iPingNo = m_blockHeaderList.Count;
            m_sFullWaveAmpMax = (short)m_dataBlockList
                                    .Where(b => b.Lf != null && b.Lf.Length > 0)
                                    .SelectMany(b => b.Lf)
                                    .Max(v => Math.Abs(v));

            //this.formsPlot1.Plot.Axes.SetLimitsX(-m_sFullWaveAmpMax, m_sFullWaveAmpMax);

            this.lblDemodulate.Text = Convert.ToInt32(this.lblDemodulate.Tag) == (int)DemodulationMode.Envelope ?
                    Properties.Resources.Envelope : Properties.Resources.FullWave;

            this.lblHPF.Text = "0 kHz";
            this.lblLPF.Text = $"{(int)(m_dSampleFreqHz / 1000 * 0.5)} kHz";
            this.lblHPF.Enabled = this.lblLPF.Enabled = m_bApplyBpf;

            chkInvert.Checked = false;
            numR.Value = numG.Value = numB.Value = 1.0M;
            lblBackColor.BackColor = System.Drawing.Color.Black;
            cmbColor.SelectedIndex = (int)ColorMode.Royal;
            cmbColor_SelectedIndexChanged(null, null);
            numIntensity.Value = 0.00003M;
            numAttDb.Value = 20;
            numScaleX.Value = 1M;
            numScaleY.Value = 1M;
            numScaleZ.Value = 1M;

            glControl2D.MouseWheel += glControl2D_MouseWheel;


            m_dRatioX = (double)numScaleY.Value;
            m_dRatioY = (double)numScaleZ.Value;
            //m_dZoom = 1.0;
            m_dZoom = 0.7;

            RebuildPingPositionsFromHeaders();
            UpdateMapTrack();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalysisForm_Shown(object sender, EventArgs e)
        {
            glControl2D.Visible = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl2D_Load(object sender, EventArgs e)
        {
            m_bLoadTK = true;
            glControl2D.MakeCurrent();

            GL.ClearColor(System.Drawing.Color.Black);

            InitTexture();
            UpdateViewportPreserveAspect();

            BuildCumulativeTrackMeters();
            m_bTextureDirty = true;
            m_bBottomDirty = true;

            CreateScrollBars();
            SetupScreenOrtho();
            RebuildOffsetsPerPing();
            UpdateScrollRanges();

            glControl2D.Refresh();


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl2D_Resize(object sender, EventArgs e)
        {
            if (!m_bLoadTK) return;

            UpdateViewportPreserveAspect();
            SetupScreenOrtho();
            UpdateScrollRanges();
            glControl2D.Refresh();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tc = (TabControl)sender;
            var tab = tc.TabPages[e.Index];
            var rect = e.Bounds;
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            // 背景
            //using (var bg = new SolidBrush(selected ? Color.FromArgb(200, 220, 255)
            //                                        : Color.FromArgb(235, 235, 235)))
            using (var bg = new SolidBrush(Constant.BUTTON_BACKCOLOR))
                e.Graphics.FillRectangle(bg, rect);

            // ボーダー
            using (var pen = new Pen(System.Drawing.Color.FromArgb(113, 96, 232)))
                e.Graphics.DrawRectangle(pen, rect);

            // アンチエイリアス等
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 文字（横書きのまま回転して中央に配置）
            using (var fmt = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            using (var font = new Font(tab.Font, System.Drawing.FontStyle.Regular))
            using (var brush = new SolidBrush(Constant.FORECOLOR))
            {
                // 中心を原点に移動して -90°回転（左タブ）
                e.Graphics.TranslateTransform(rect.Left + rect.Width / 2f,
                                              rect.Top + rect.Height / 2f);
                e.Graphics.RotateTransform(-90f);

                // 回転後の描画領域（幅<->高さが入れ替わる点がミソ）
                var textRect = new RectangleF(-rect.Height / 2f, -rect.Width / 2f,
                                               rect.Height, rect.Width);

                e.Graphics.DrawString(tab.Text, font, brush, textRect, fmt);

                // 変換を戻す
                e.Graphics.ResetTransform();
            }
        }

        #endregion

        #region 描画

        private void glControl2D_Paint(object sender, PaintEventArgs e)
        {
            if (!m_bLoadTK) return;
            if (m_blockHeaderList == null || m_dataBlockList == null) return;
            if (m_blockHeaderList.Count == 0 || m_dataBlockList.Count == 0) return;

            glControl2D.MakeCurrent();
            RebuildOffsetsPerPing();
            SetupScreenOrtho();
            UpdateScrollRanges();

            RenderSceneCore(applyScroll: true, drawLabels: true);

            glControl2D.SwapBuffers();

        }
        #endregion

        #region 入力イベント

        private void glControl2D_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) != Keys.Shift) return;

            // マウス位置を基準にズーム
            double dataX = (e.X + m_dScrollX) / m_dScaleX;
            double dataY = (e.Y + m_dScrollY) / m_dScaleY;

            double step = 0.1;
            m_dZoom = (e.Delta > 0) ? m_dZoom + step : Math.Max(step, m_dZoom - step);

            double newScaleX = m_dScaleX;
            double newScaleY = m_dScaleY;

            m_dScrollX = dataX * newScaleX - e.X;
            m_dScrollY = dataY * newScaleY - e.Y;

            UpdateScrollRanges();
            glControl2D.Refresh();
        }

        private void glControl2D_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            m_bDraggingAddContact = false; //1119
            m_bDragging = true;
            m_dragStart = e.Location;
            m_dScrollStartX = m_dScrollX;
            m_dScrollStartY = m_dScrollY;
            Cursor = Cursors.Hand;
            if (m_mouseContentX.HasValue)
            {
                m_mouseContentX = null;
                m_mouseContentY = null;
                glControl2D.Refresh();
            }
        }

        private void glControl2D_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_bDragging)
            {
                int dx = e.X - m_dragStart.X;
                int dy = e.Y - m_dragStart.Y;
                m_dScrollX = m_dScrollStartX - dx;
                m_dScrollY = m_dScrollStartY - dy;
                UpdateScrollRanges();
                glControl2D.Refresh();
                m_bDraggingAddContact = true; //1119
                m_mouseContentY = null;
                return;
            }

            int ping = GetPingIndexAtMouseX(e.X);
            double? prevMouseContentX = m_mouseContentX;
            double? prevMouseContentY = m_mouseContentY;
            m_mouseContentX = (ping >= 0) ? m_dScrollX + e.X : null;
            m_mouseContentY = (ping >= 0) ? m_dScrollY + e.Y : null;
            if (ping >= 0)
            {
                m_mouseDepthMeters = GetDepthMetersAtMouse(ping, e.Y);
                PlotPingWave(ping);
                ShowInfo(ping, BlockHeaderList[ping]);
                //ShowInfoToDataGridView(ping, BlockHeaderList[ping]);
            }
            else
            {
                if (m_mouseDepthMeters.HasValue)
                    ClearWaveformDepthGuide();
                UpdateMapCursorMarker(-1);
                m_mouseContentX = null;
                m_mouseContentY = null;
            }

            if (prevMouseContentX != m_mouseContentX || prevMouseContentY != m_mouseContentY)
            {
                glControl2D.Refresh();
            }

        }
        private void glControl2D_MouseLeave(object sender, EventArgs e)
        {
            if (m_mouseDepthMeters.HasValue)
                ClearWaveformDepthGuide();
            UpdateMapCursorMarker(-1);
            if (m_mouseContentX.HasValue)
            {
                m_mouseContentX = null;
                m_mouseContentY = null;
                glControl2D.Refresh();
            }
        }
        private void glControl2D_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            m_bDragging = false;
            Cursor = Cursors.Default;
        }

        private void cmbColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool gray = (cmbColor.SelectedIndex == (int)ColorMode.Gray);
            numR.Enabled = numG.Enabled = numB.Enabled = gray;

            m_bLutDirty = true;
            m_bTextureDirty = true;
            glControl2D.Refresh();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            var tag = (sender as NumericUpDown)?.Tag as string;
            if (tag == null) return;

            switch (tag)
            {
                case "R":
                case "G":
                case "B":
                    m_bLutDirty = true;
                    break;

                case "Intensity":
                case "Threshold":
                case "Alpha":
                case "Bottom":
                case "AttDb":
                    break;

                case "ScaleY":
                    {
                        double cyScreen = glControl2D.ClientSize.Height * 0.5;
                        double dataY = (cyScreen + m_dScrollY) / m_dScaleY;  // 縦は中心アンカーを維持
                        m_dRatioX = (double)numScaleY.Value;
                        m_dRatioY = (double)numScaleZ.Value;

                        m_dScrollX = 0; // 左端固定
                        double newScaleY = m_dScaleY;
                        m_dScrollY = dataY * newScaleY - cyScreen;

                        UpdateViewportPreserveAspect();
                        UpdateScrollRanges();
                    }
                    break;

                case "ScaleZ":
                    m_dRatioY = (double)numScaleZ.Value;
                    m_dScrollY = 0; // 上端固定
                    UpdateViewportPreserveAspect();
                    UpdateScrollRanges();
                    break;

                default:
                    return;
            }

            m_bTextureDirty = true;
            if (tag == "ScaleZ" || tag == "ScaleY") UpdateScrollRanges();
            glControl2D.Refresh();
        }

        private void btnChooseColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = lblBackColor.BackColor;
            if (colorDialog1.ShowDialog(this) != DialogResult.OK) return;

            lblBackColor.BackColor = colorDialog1.Color;
            SetBackgroundColor(colorDialog1.Color);
        }

        private void chkBox_CheckedChanged(object sender, EventArgs e)
        {
            var tag = ((CheckBox)sender).Tag?.ToString();
            if (tag == null) return;

            switch (tag)
            {
                case "Invert":
                    m_bLutDirty = true;
                    m_bTextureDirty = true;
                    break;
                case "DrawDepthScale":
                case "DrawDistanceScale":
                    break;
                case "HeaveCorrection":
                    m_bTextureDirty = true;
                    m_bBottomDirty = true;
                    break;
                case "FlipX":
                    m_bFlipX = ((CheckBox)sender).Checked;
                    break;
                case "ShowBottomTrack":
                    break;
                default:
                    return;
            }
            glControl2D.Refresh();
        }
        /// <summary>
        /// Signal Processing button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSignalProcessing_Click(object sender, EventArgs e)
        {
            SignalProcessingForm frmSp = new SignalProcessingForm(this, m_dSampleFreqHz);
            if (frmSp.ShowDialog(this) == DialogResult.OK)
            {
                m_bTextureDirty = true;
                glControl2D.Refresh();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag == null) return;

            switch (e.ClickedItem.Tag.ToString())
            {
                case "SignalProcessing":
                    SignalProcess();
                    break;
                case "SaveImage":
                    SaveImageProcess();
                    break;
            }
        }
        #endregion

        #region 計算・描画ヘルパ
        /// <summary>
        /// 現在の表示倍率（px/m）そのままで、画面外も含めた全体をPNG出力します。
        /// includeLabels=true で横/縦スケール線＆ラベルも含めます。
        /// GPU上限を超える場合は等比で軽く縮小します（※タイル描画も可能。必要なら言ってください）
        /// </summary>
        public void SaveFullImageAtCurrentScreenScale(string filePath, bool includeLabels = true)
        {
            glControl2D.MakeCurrent();

            // 全域描画用にスクロール固定
            double oldScrollX = m_dScrollX, oldScrollY = m_dScrollY;
            m_dScrollX = 0; m_dScrollY = 0;

            // オフセットなど最新化
            RebuildOffsetsPerPing();

            // コンテンツ全体のpxサイズ（今のスケールのまま）
            GetContentSize(out double contentW_px, out double contentH_px, out double contentBottom_px);
            int W = Math.Max(1, (int)Math.Ceiling(contentW_px));
            int H = Math.Max(1, (int)Math.Ceiling(contentH_px));

            // GPUの最大レンダバッファを確認し、でかすぎるなら等比縮小
            GL.GetInteger(GetPName.MaxRenderbufferSize, out int maxRb);
            if (W > maxRb || H > maxRb)
            {
                double s = Math.Min((double)maxRb / W, (double)maxRb / H);
                W = Math.Max(1, (int)Math.Floor(W * s));
                H = Math.Max(1, (int)Math.Floor(H * s));
                contentBottom_px *= s;  // 後続の縦線レンジで使用
            }

            // --- FBO 構築 ---
            int fbo = 0, colorTex = 0, depthRb = 0;
            GL.GenFramebuffers(1, out fbo);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

            GL.GenTextures(1, out colorTex);
            GL.BindTexture(TextureTarget.Texture2D, colorTex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, W, H, 0,
                          PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.GenRenderbuffers(1, out depthRb);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthRb);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, W, H);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2D, colorTex, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                                       RenderbufferTarget.Renderbuffer, depthRb);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                if (depthRb != 0) GL.DeleteRenderbuffer(depthRb);
                if (colorTex != 0) GL.DeleteTexture(colorTex);
                if (fbo != 0) GL.DeleteFramebuffer(fbo);
                m_dScrollX = oldScrollX; m_dScrollY = oldScrollY;
                throw new InvalidOperationException($"FBO incomplete: {status}");
            }

            // このFBOに全域描画
            GL.Viewport(0, 0, W, H);
            GL.ClearColor(lblBackColor.BackColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // 射影：コンテンツpx座標系そのまま（左上原点）
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, W, H, 0, -1, 1);
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
            GL.LoadIdentity();

            // 本体（テクスチャ）描画：スクロール無し・ラベル無し
            RenderSceneCore(applyScroll: false, drawLabels: false);

            // スケール線＆ラベル（必要なら）
            if (includeLabels)
            {
                // 深度の横線（全域）
                if (chkDrawDepthScale.Checked)
                    DrawDepthScale(W);

                // 距離スケール縦線（全域レンジ指定）
                if (chkDrawDistScale.Checked)
                    DrawDistanceScaleRange(contentBottom_px, viewLeftPx: 0.0, viewRightPx: W);

                // 左固定の深度ラベル（px座標で左固定になる）
                if (chkDrawDepthScale.Checked)
                    DrawDepthLabelsPinnedLeft();

                // 上部の距離ラベル（全域レンジ指定）
                if (chkDrawDistScale.Checked)
                    DrawDistanceLabelsPinnedTopRange(viewLeftPx: 0.0, viewRightPx: W);
            }

            // 読み出し＆保存
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            byte[] pixels = new byte[W * H * 4];
            GL.ReadPixels(0, 0, W, H, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

            FlipVerticallyInPlace(pixels, W, H);

            using (var bmp = new Bitmap(W, H, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var data = bmp.LockBits(new Rectangle(0, 0, W, H),
                                        System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                try { System.Runtime.InteropServices.Marshal.Copy(pixels, 0, data.Scan0, pixels.Length); }
                finally { bmp.UnlockBits(data); }
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }

            // 後片付け
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteRenderbuffer(depthRb);
            GL.DeleteTexture(colorTex);
            GL.DeleteFramebuffer(fbo);

            // スクロール戻す
            m_dScrollX = oldScrollX; m_dScrollY = oldScrollY;
        }

        /// <summary>
        /// 全域を FBO に描いて保存するメソッド
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="outputWidthPx"></param>
        /// <param name="includeLabels"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SaveFullImagePng(string filePath, int outputWidthPx = 4000, bool includeLabels = true)
        {
            glControl2D.MakeCurrent();

            // 現在の状態を保存してあとで戻す
            double oldScrollX = m_dScrollX, oldScrollY = m_dScrollY;

            // 全域描画なのでスクロール無効化
            m_dScrollX = 0; m_dScrollY = 0;

            // コンテンツ実寸（画面座標系）
            RebuildOffsetsPerPing(); // 念のため
            GetContentSize(out double contentW, out double contentH, out _);

            if (contentW <= 0 || contentH <= 0) { m_dScrollX = oldScrollX; m_dScrollY = oldScrollY; return; }

            // 出力サイズ決定（width 指定、高さは等倍率）
            double scale = outputWidthPx / contentW;
            int W = outputWidthPx;
            int H = (int)Math.Ceiling(contentH * scale);

            // GPU上限チェック（必要なら縮小）
            int maxRb; GL.GetInteger(GetPName.MaxRenderbufferSize, out maxRb);
            if (W > maxRb || H > maxRb)
            {
                double sW = (double)maxRb / W;
                double sH = (double)maxRb / H;
                scale *= Math.Min(sW, sH);
                W = Math.Max(1, (int)Math.Floor(contentW * scale));
                H = Math.Max(1, (int)Math.Floor(contentH * scale));
            }

            // --- FBO 構築 ---
            int fbo = 0, colorTex = 0, depthRb = 0;
            GL.GenFramebuffers(1, out fbo);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

            GL.GenTextures(1, out colorTex);
            GL.BindTexture(TextureTarget.Texture2D, colorTex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, W, H, 0,
                          PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.GenRenderbuffers(1, out depthRb);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthRb);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, W, H);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2D, colorTex, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                                       RenderbufferTarget.Renderbuffer, depthRb);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                // 後始末して戻る
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                if (depthRb != 0) GL.DeleteRenderbuffer(depthRb);
                if (colorTex != 0) GL.DeleteTexture(colorTex);
                if (fbo != 0) GL.DeleteFramebuffer(fbo);
                m_dScrollX = oldScrollX; m_dScrollY = oldScrollY;
                throw new InvalidOperationException($"FBO incomplete: {status}");
            }

            // --- この FBO に全域を描く ---
            GL.Viewport(0, 0, W, H);
            GL.ClearColor(lblBackColor.BackColor); // UIと同じ背景にしたい場合
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // 射影を「全コンテンツ座標」に合わせる（0..contentW, 0..contentH）
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, contentW, contentH, 0, -1, 1);
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
            GL.LoadIdentity();

            // まず本体だけ描く（ラベル/スケールは一旦オフ）
            RenderSceneCore(applyScroll: false, drawLabels: false);

            // 全域サイズを取得
            GetContentSize(out contentW, out _, out double contentBottom);

            if (includeLabels)
            {
                // 深度の横線
                if (chkDrawDepthScale.Checked)
                    DrawDepthScale(contentW);

                // 距離スケールの縦線（全域）
                if (chkDrawDistScale.Checked)
                    DrawDistanceScaleRange(contentBottom, viewLeftPx: 0.0, viewRightPx: contentW);

                // 左の深度ラベル（画面座標固定系）
                if (chkDrawDepthScale.Checked)
                    DrawDepthLabelsPinnedLeft();

                // 上の距離ラベル（全域）
                if (chkDrawDistScale.Checked)
                    DrawDistanceLabelsPinnedTopRange(viewLeftPx: 0.0, viewRightPx: contentW);
            }

            // --- 読み出して保存 ---
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);

            byte[] pixels = new byte[W * H * 4];
            GL.ReadPixels(0, 0, W, H, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

            // 上下反転
            FlipVerticallyInPlace(pixels, W, H);

            using (var bmp = new Bitmap(W, H, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var data = bmp.LockBits(new Rectangle(0, 0, W, H),
                                        System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                try { System.Runtime.InteropServices.Marshal.Copy(pixels, 0, data.Scan0, pixels.Length); }
                finally { bmp.UnlockBits(data); }
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }

            // 後片付け＆元に戻す
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteRenderbuffer(depthRb);
            GL.DeleteTexture(colorTex);
            GL.DeleteFramebuffer(fbo);

            m_dScrollX = oldScrollX; m_dScrollY = oldScrollY;
        }

        private void RebuildOffsetsPerPing()
        {
            if (m_blockHeaderList == null || m_blockHeaderList.Count == 0) return;

            double dz_m = m_dZDistance / 100.0;  // 1 sample [m]
            double samplesPerMeter = 1.0 / dz_m;

            m_offsetPxPerPing ??= new double[m_iPingNo];

            m_dMinOffsetPx = double.PositiveInfinity;
            m_dMaxOffsetPx = double.NegativeInfinity;

            for (int i = 0; i < m_iPingNo; i++)
            {
                double px = m_blockHeaderList[i].MeasureStart * samplesPerMeter * m_dScaleY;
                m_offsetPxPerPing[i] = px;
                if (px < m_dMinOffsetPx) m_dMinOffsetPx = px;
                if (px > m_dMaxOffsetPx) m_dMaxOffsetPx = px;
            }

            // 左上へ貼り付くよう最小を0に正規化
            for (int i = 0; i < m_iPingNo; i++) m_offsetPxPerPing[i] -= m_dMinOffsetPx;
            m_dMaxOffsetPx -= m_dMinOffsetPx; // レンジ更新
            m_dMinOffsetPx = 0.0;

            m_dMinStartMeters = m_blockHeaderList.Min(b => b.MeasureStart);
            m_dMaxStartMeters = m_blockHeaderList.Max(b => b.MeasureStart);
        }

        private void BuildCumulativeTrackMeters()
        {
            if (m_blockHeaderList == null)
            {
                m_cumDistM = Array.Empty<double>();
                m_dTotalDistM = 0.0;
                return;
            }

            int n = m_blockHeaderList.Count;
            m_cumDistM = new double[n];
            if (n == 0)
            {
                m_dTotalDistM = 0.0;
                m_pingPositions = Array.Empty<(double X, double Y)?>();
                UpdateMapTrack();
                return;
            }
            m_cumDistM[0] = 0.0;

            RebuildPingPositionsFromHeaders();

            if (m_pingPositions == null || m_pingPositions.Length == 0 || !m_pingPositions[0].HasValue)
            {
                // 等間隔フォールバック
                for (int i = 0; i < n; i++) m_cumDistM[i] = i;
                m_dTotalDistM = Math.Max(0, n - 1);
                UpdateMapTrack();
                return;
            }

            var prev = m_pingPositions[0]!.Value;
            double sum = 0.0;
            for (int i = 1; i < n; i++)
            {
                var pos = m_pingPositions[i];
                if (!pos.HasValue)
                {
                    pos = prev;
                    m_pingPositions[i] = prev;
                }
                double dx = pos.Value.X - prev.X;
                double dy = pos.Value.Y - prev.Y;

                sum += Math.Sqrt(dx * dx + dy * dy);
                m_cumDistM[i] = sum;
                //prevX = x; prevY = y;
                prev = pos.Value;
            }
            m_dTotalDistM = sum;

            UpdateMapTrack();
        }

        private bool TryParseXY(BlockHeader h, out double x, out double y)
        {
            bool okX = double.TryParse(h.SisString5, System.Globalization.NumberStyles.Float,
                                       System.Globalization.CultureInfo.InvariantCulture, out x);
            bool okY = double.TryParse(h.SisString6, System.Globalization.NumberStyles.Float,
                                       System.Globalization.CultureInfo.InvariantCulture, out y);
            return okX && okY;
        }
        private void RebuildPingPositionsFromHeaders()
        {
            if (m_blockHeaderList == null)
            {
                m_pingPositions = null;
                return;
            }

            int n = m_blockHeaderList.Count;
            var positions = new (double X, double Y)?[n];
            double? lastX = null;
            double? lastY = null;

            for (int i = 0; i < n; i++)
            {
                if (TryParseXY(m_blockHeaderList[i], out double x, out double y))
                {
                    lastX = x;
                    lastY = y;
                    positions[i] = (x, y);
                }
                else if (lastX.HasValue && lastY.HasValue)
                {
                    positions[i] = (lastX.Value, lastY.Value);
                }
            }

            m_pingPositions = positions;
        }

        private void UpdateMapTrack()
        {
            if (m_frmMap == null)
                return;

            if (m_pingPositions == null)
            {
                m_frmMap.SetTrack(Array.Empty<(double X, double Y)>());
                return;
            }

            var points = new List<(double X, double Y)>(m_pingPositions.Length);
            foreach (var pos in m_pingPositions)
            {
                if (pos.HasValue)
                    points.Add(pos.Value);
            }

            m_frmMap.SetTrack(points);

            if (m_lastMapCursorPing >= 0)
            {
                UpdateMapCursorMarker(m_lastMapCursorPing);
            }
        }

        private void UpdateMapCursorMarker(int pingIndex)
        {
            if (m_frmMap == null)
                return;

            if (m_pingPositions == null || pingIndex < 0 || pingIndex >= m_pingPositions.Length)
            {
                if (m_lastMapCursorPing != -1)
                {
                    m_frmMap.ClearCursor();
                    m_lastMapCursorPing = -1;
                }
                return;
            }

            var pos = m_pingPositions[pingIndex];
            if (!pos.HasValue)
            {
                if (m_lastMapCursorPing != -1)
                {
                    m_frmMap.ClearCursor();
                    m_lastMapCursorPing = -1;
                }
                return;
            }

            m_frmMap.UpdateCursorPosition(pos.Value.X, pos.Value.Y);
            m_lastMapCursorPing = pingIndex;
        }
        private double GetPixelsPerMeterY()
        {
            double dz_m = m_dZDistance / 100.0;
            double samplesPerMeter = 1.0 / dz_m;
            return m_dScaleY * samplesPerMeter;
        }

        private double GetPixelsPerMeterX()
        {
            double pxPerM_Y = GetPixelsPerMeterY();
            return pxPerM_Y * (m_dRatioX / m_dRatioY);
        }

        private (int tex, int w, int h) GetOrCreateLabelTexture(string text)
        {
            if (_labelCache.TryGetValue(text, out var t)) return t;

            using var bmp = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                var size = TextRenderer.MeasureText(g, text, this.Font, new Size(int.MaxValue, int.MaxValue),
                                                    TextFormatFlags.NoPadding);
                using var bmp2 = new Bitmap(Math.Max(1, size.Width), Math.Max(1, size.Height),
                                            System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                using (var g2 = Graphics.FromImage(bmp2))
                {
                    g2.Clear(System.Drawing.Color.Transparent);
                    TextRenderer.DrawText(g2, text, this.Font, new Point(0, 0), System.Drawing.Color.White,
                                          TextFormatFlags.NoPadding);
                }

                int texId; GL.GenTextures(1, out texId);
                GL.BindTexture(TextureTarget.Texture2D, texId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
                var data = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height),
                                         System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                         System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                              bmp2.Width, bmp2.Height, 0,
                              PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bmp2.UnlockBits(data);

                t = (texId, bmp2.Width, bmp2.Height);
                _labelCache[text] = t;
                return t;
            }
        }

        /// <summary>
        /// 画面上部の距離ラベル
        /// </summary>
        private void DrawDistanceLabelsPinnedTop()
        {
            double viewLeftPx = m_dScrollX;
            double viewRightPx = m_dScrollX + glControl2D.ClientSize.Width;
            DrawDistanceLabelsPinnedTopRange(viewLeftPx, viewRightPx);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewLeftPx"></param>
        /// <param name="viewRightPx"></param>
        private void DrawDistanceLabelsPinnedTopRange(double viewLeftPx, double viewRightPx)
        {
            if (m_dTotalDistM <= 0.0) return;

            double stepM = 5.0;
            double totalM = m_dTotalDistM;

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Color4(1f, 1f, 1f, 1f);

            float y = 6f, padX = 4f, padY = 2f, labelOffsetX = 6f;

            for (double m = 0.0; m <= totalM + 1e-9; m += stepM)
            {
                double x = MapXByMeter(m);                // ★反転対応
                if (x < viewLeftPx - 1 || x > viewRightPx + 1) continue;

                float xScreen = (float)(x - m_dScrollX) + labelOffsetX;
                string text = $"{m:0} m";
                var (tex, w, h) = GetOrCreateLabelTexture(text);

                // 影
                GL.Disable(EnableCap.Texture2D);
                GL.Color4(0f, 0f, 0f, 0.35f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(xScreen - padX, y - padY);
                GL.Vertex2(xScreen + w + padX, y - padY);
                GL.Vertex2(xScreen + w + padX, y + h + padY);
                GL.Vertex2(xScreen - padX, y + h + padY);
                GL.End();

                // 本体
                GL.Enable(EnableCap.Texture2D);
                GL.Color4(1f, 1f, 1f, 1f);
                GL.BindTexture(TextureTarget.Texture2D, tex);
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 0); GL.Vertex2(xScreen, y);
                GL.TexCoord2(1, 0); GL.Vertex2(xScreen + w, y);
                GL.TexCoord2(1, 1); GL.Vertex2(xScreen + w, y + h);
                GL.TexCoord2(0, 1); GL.Vertex2(xScreen, y + h);
                GL.End();
            }

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            GL.Color4(1f, 1f, 1f, 1f);
        }


        /// <summary>
        /// 進行方向距離スケール（縦線）
        /// 既存関数の中身は温存し、入口だけ拡張
        /// </summary>
        /// <param name="contentBottom"></param>
        private void DrawDistanceScale(double contentBottom)
        {
            double viewLeftPx = m_dScrollX;
            double viewRightPx = m_dScrollX + glControl2D.ClientSize.Width;
            DrawDistanceScaleRange(contentBottom, viewLeftPx, viewRightPx);
        }
        /// <summary>
        /// 全域/任意範囲用
        /// </summary>
        /// <param name="contentBottom"></param>
        /// <param name="viewLeftPx"></param>
        /// <param name="viewRightPx"></param>
        private void DrawDistanceScaleRange(double contentBottom, double viewLeftPx, double viewRightPx)
        {
            if (m_dTotalDistM <= 0.0) return;

            double stepM = 5.0; // 目盛間隔
            double totalM = m_dTotalDistM;

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.LineWidth(1f);
            GL.Color4(0.8f, 0.8f, 1f, 0.8f);
            GL.Begin(PrimitiveType.Lines);

            for (double m = 0.0; m <= totalM + 1e-9; m += stepM)
            {
                double x = MapXByMeter(m); // ★反転対応
                if (x < viewLeftPx - 1 || x > viewRightPx + 1) continue; // ビュー外はスキップ
                GL.Vertex2(x, 0);
                GL.Vertex2(x, contentBottom);
            }

            GL.End();
            GL.Disable(EnableCap.Blend);
        }



        // 左固定の深度ラベル
        private void DrawDepthLabelsPinnedLeft()
        {
            if (m_dZDistance <= 0) return;

            double dz_m = m_dZDistance / 100.0;
            double samplesPerMeter = 1.0 / dz_m;
            double tol = MeterTolerance();

            int viewH = glControl2D.ClientSize.Height;
            float x = (float)Math.Max(0.0, -m_dScrollX) + 6f;
            float padY = 2f;

            int startMeters = (int)Math.Ceiling(m_dMinStartMeters - 1e-9);
            double maxDepthMetersExact = m_dMaxStartMeters + m_iSampleNo * dz_m;
            int endMeters = (int)Math.Ceiling(maxDepthMetersExact - 1e-9);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Color4(1f, 1f, 1f, 1f);

            for (int m = startMeters; m <= endMeters; m++)
            {
                int iRef = FindRefPingForMeter(m, dz_m);
                double s = m_blockHeaderList[iRef].MeasureStart;
                double bandM = m_iSampleNo * dz_m;

                double relM = m - s;
                if (relM < -tol || relM > bandM + tol) continue;

                double relM_clamped = Math.Max(0.0, Math.Min(bandM, relM));
                double yContent = m_offsetPxPerPing[iRef] + (relM_clamped * samplesPerMeter * m_dScaleY);
                float yScreen = (float)(yContent - m_dScrollY);
                if (yScreen < 0 || yScreen > viewH + 1) continue;

                string text = $"{m} m";
                var (tex, w, h) = GetOrCreateLabelTexture(text);

                GL.Disable(EnableCap.Texture2D);
                GL.Color4(0f, 0f, 0f, 0.35f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(0, yScreen - h - padY);
                GL.Vertex2(x + w + 4, yScreen - h - padY);
                GL.Vertex2(x + w + 4, yScreen + padY);
                GL.Vertex2(0, yScreen + padY);
                GL.End();

                GL.Enable(EnableCap.Texture2D);
                GL.Color4(1f, 1f, 1f, 1f);
                GL.BindTexture(TextureTarget.Texture2D, tex);
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 0); GL.Vertex2(x, yScreen - h - padY);
                GL.TexCoord2(1, 0); GL.Vertex2(x + w, yScreen - h - padY);
                GL.TexCoord2(1, 1); GL.Vertex2(x + w, yScreen - padY);
                GL.TexCoord2(0, 1); GL.Vertex2(x, yScreen - padY);
                GL.End();
            }

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
        }

        // 深度スケール（横線）
        private void DrawDepthScale(double contentW)
        {
            if (m_dZDistance <= 0) return;

            double dz_m = m_dZDistance / 100.0;
            double samplesPerMeter = 1.0 / dz_m;
            double pxPerM_X = GetPixelsPerMeterX();
            double tol = MeterTolerance();

            int startMeters = (int)Math.Ceiling(m_dMinStartMeters - 1e-9);
            double maxDepthMetersExact = m_dMaxStartMeters + m_iSampleNo * dz_m;
            int endMeters = (int)Math.Ceiling(maxDepthMetersExact - 1e-9);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.LineWidth(1f);
            GL.Color4(0.8f, 0.8f, 1f, 0.8f);

            for (int m = startMeters; m <= endMeters; m++)
            {
                bool drawing = false;
                GL.Begin(PrimitiveType.LineStrip);
                for (int i = 0; i < m_iPingNo; i++)
                {
                    double startM = m_blockHeaderList[i].MeasureStart;
                    double bandM = m_iSampleNo * dz_m;
                    double relM = m - startM;

                    if (relM < -tol || relM > bandM + tol)
                    {
                        if (drawing) { GL.End(); drawing = false; GL.Begin(PrimitiveType.LineStrip); }
                        continue;
                    }

                    double relM_clamped = Math.Max(0.0, Math.Min(bandM, relM));

                    //double x = (m_dTotalDistM > 0.0) ? m_cumDistM[i] * pxPerM_X : i * m_dScaleX;
                    double x = MapXByPing(i);
                    double y = m_offsetPxPerPing[i] + (relM_clamped * samplesPerMeter * m_dScaleY);
                    GL.Vertex2(x, y);
                    drawing = true;
                }
                GL.End();
            }

            GL.Disable(EnableCap.Blend);
        }

        private int FindRefPingForMeter(double m, double dz_m)
        {
            double tol = MeterTolerance();
            int best = -1;
            double bestStart = double.NegativeInfinity;
            double bandM = m_iSampleNo * dz_m;

            for (int i = 0; i < m_iPingNo; i++)
            {
                double s = m_blockHeaderList[i].MeasureStart;
                if (s - tol <= m && m <= s + bandM + tol)
                {
                    if (s > bestStart) { bestStart = s; best = i; }
                }
                else if (best == -1 && s <= m && s > bestStart)
                {
                    bestStart = s; best = i;
                }
            }
            return (best >= 0) ? best : 0;
        }
        /// <summary>
        /// 検出したボトム位置をラインで可視化。
        /// </summary>
        private void DrawBottomLine()
        {
            if (m_bottomIdx == null || m_bottomIdx.Length != m_iPingNo) return;

            double pxPerM_X = GetPixelsPerMeterX();
            int w = m_iTexWidth;
            int h = m_iTexHeight;

            // 線のスタイル
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.LineWidth(2f);
            GL.Color4(1f, 0.6f, 0f, 0.95f); // 暖色で視認性（必要ならUIで変更可）

            // MeasureStart 段差を考慮した座標に変換
            GL.Begin(PrimitiveType.LineStrip);
            for (int i = 0; i < w; i++)
            {
                //double x = (m_dTotalDistM > 0.0) ? m_cumDistM[i] * pxPerM_X : i * m_dScaleX;
                double x = MapXByPing(i);

                // 画像上のY：各Pingの縦オフセット + ボトムサンプル * 縦スケール
                int b = m_bottomIdx[i];

                // 表示はヒーブの有無どちらでもよいが、画像と整合を取るならオフセット
                if (chkHeaveCorrection.Checked)
                {
                    double sampleIntervalCm = m_dZDistance;
                    double heave_cm = m_blockHeaderList[i].HeaveFromMotionSensor / 10.0;
                    int heaveOffset = (int)Math.Round((-1.0 * heave_cm) / sampleIntervalCm);
                    b = Math.Clamp(b + heaveOffset, 0, h - 1);
                }

                double y = (m_offsetPxPerPing[i]) + m_bottomIdx[i] * m_dScaleY;
                GL.Vertex2(x, y);
            }
            GL.End();

            GL.Disable(EnableCap.Blend);
        }
        /// <summary>
        /// 波形グラフの描画
        /// </summary>
        private void PlotWave(short[] wave, uint measureStart, uint measureLength)
        {
            double[] amps = wave.Select(v => (double)v).ToArray();
            double[] depths = Enumerable.Range(0, amps.Length)
                                .Select(i => measureStart + (i * m_dZDistance / 100.0d))   // 片道距離[m]
                                .ToArray();

            var plt = formsPlot1.Plot;
            plt.Clear();
            m_depthGuideLine = null;

            // X=振幅, Y=深度
            var sc = plt.Add.Scatter(amps, depths, ScottPlot.Color.FromColor(System.Drawing.Color.GreenYellow));

            sc.MarkerSize = 0;

            // Y軸を上→下の正方向に反転
            plt.Axes.InvertY();

            //const short AMP_MIN = short.MinValue;   // -32768
            //const short AMP_MAX = short.MaxValue;   // +32767

            plt.Axes.SetLimitsX(-m_sFullWaveAmpMax, m_sFullWaveAmpMax);
            plt.Axes.SetLimitsY(measureStart + measureLength, measureStart);

            // オートスケール時にも常に反転を維持（推奨）
            plt.Axes.AutoScaler.InvertedY = true;

            // 体裁
            plt.Axes.Bottom.Label.Text = "Amplitude";
            plt.Axes.Left.Label.Text = "Depth (m)";
            //plt.Add.VerticalLine(0);   // 0 振幅の基準線（任意）

            UpdateWaveformDepthGuide(m_mouseDepthMeters, refreshPlot: false);

            formsPlot1.Refresh();

        }
        private void PlotPingWave(int pingIndex)
        {
            UpdateMapCursorMarker(pingIndex);

            if (pingIndex < 0 || pingIndex >= m_dataBlockList.Count) return;

            short[]? wave = GetActiveWaveform(pingIndex);

            if (wave == null) return;

            PlotWave(wave,
                     BlockHeaderList[pingIndex].MeasureStart,
                     BlockHeaderList[pingIndex].MeasureLength);

            m_lastPlottedPing = pingIndex;
        }

        private short[]? GetActiveWaveform(int pingIndex)
        {
            if (pingIndex < 0 || pingIndex >= m_dataBlockList.Count) return null;

            return DemodulateMode == DemodulationMode.None
                ? DataBlockList[pingIndex].Lf
                : DataBlockList[pingIndex].Processed;
        }
        /// <summary>
        /// Ping情報の表示
        /// </summary>
        /// <param name="date"></param>

        private void ShowInfo(int pingNumber, BlockHeader ping)//! ping: BlockHeader[ping]のこと
        {



            //日付表示
            string date = ping.Date;
            labelDate.Text = Method.ConvertDateString(date, "yyyy/MM/dd");



            //時間表示
            string time = ping.Time;
            List<String> timeCut = time.Split('.').ToList();
            labelTime.Text = timeCut[0];

            //Ping Number表示
            labelPingNumber.Text = pingNumber.ToString();


            //SisString1~8表示
            String sisStirng1 = ping.SisString1;
            String sisStirng2 = ping.SisString2;
            String sisStirng3 = ping.SisString3;
            String sisStirng4 = ping.SisString4;
            String sisStirng5 = ping.SisString5;
            String sisStirng6 = ping.SisString6;
            String sisStirng7 = ping.SisString7;
            String sisStirng8 = ping.SisString8;

            labelSisString1.Text = sisStirng1;
            labelSisString2.Text = sisStirng2;
            labelSisString3.Text = sisStirng3;
            labelSisString4.Text = sisStirng4;
            labelSisString5.Text = sisStirng5;
            labelSisString6.Text = sisStirng6;
            labelSisString7.Text = sisStirng7;
            labelSisString8.Text = sisStirng8;




            //Measure 表示
            String measureStart = ping.MeasureStart.ToString();
            String measureLength = ping.MeasureLength.ToString();

            labelMeasureStart.Text = measureStart;
            labelMeasureLength.Text = measureLength;




            //Motion Sensor Angle表示
            String headingMotionSensor = ping.HeadingAngleFromMotionSensor.ToString();
            String rollMotionSensor = ping.RollAngleFromMotionSensor.ToString();
            String pitchMotionSensor = ping.PitchAngleFromMotionSensor.ToString();
            String yawMotionSensor = ping.YawAngleFromMotionSensor.ToString();
            String heaveMotionSensor = ping.HeaveFromMotionSensor.ToString();

            labelHeadingMotionSensor.Text = headingMotionSensor;
            labelRollMotionSensor.Text = rollMotionSensor;
            labelPitchMotionSensor.Text = pitchMotionSensor;
            labelYawMotionSensor.Text = yawMotionSensor;
            labelHeaveMotionSensor.Text = heaveMotionSensor;


            //steering表示
            String rollAngleSteering = ping.SteeringRollAngle.ToString();
            String pitchAngleSteering = ping.SteeringPitchAngle.ToString();

            labelRollSteering.Text = rollAngleSteering;
            labelPitchSteering.Text = pitchAngleSteering;


            //Frequency表示
            String lfFrequency = ping.Frequency1.ToString();  //? lf?
            String hfFrequency = ping.HfFrequency1.ToString();//? hf?
            String SampleFrequencyLf = ping.SampleFrequencyForLf.ToString();

            labelLfFrequency.Text = lfFrequency;
            labelHfFrequency.Text = hfFrequency;
            labelSampleFreqLf.Text = SampleFrequencyLf;


            //Pulse表示

            String pulses1 = ping.Pulses1.ToString();//? pulses1?何を表している？
            String PulseToPulseDistance = ping.PulseToPulseDistance.ToString();
            String pulseLength = ping.PulseLength.ToString();

            labelPulses1.Text = pulses1;
            labelPulseToPulseDistance.Text = PulseToPulseDistance;
            labelPulseLength.Text = pulseLength;


            //Gain Value表示
            String gainValueLf = ping.GainValueOfLf.ToString();
            String gainValueHf = ping.GainValueOfHf.ToString();

            labelGainValueLf.Text = gainValueLf;
            labelGainValueHf.Text = gainValueHf;


            //Sound Velocity表示
            String soundVelocity = ping.SoundVelocity.ToString();

            labelSoundVelocity.Text = soundVelocity;


        }
        #endregion

        #region スクロール・ビューポート
        /// <summary>
        /// 描画コア
        /// </summary>
        private void RenderSceneCore(bool applyScroll, bool drawLabels)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (m_bLutDirty) { BuildPaletteLut(); m_bLutDirty = false; }
            if (m_bTextureDirty)
            {
                if (m_bBottomDirty)
                {
                    //ComputeBottomIndices();
                    /*
                    ComputeBottomIndicesByStdDev(
                        //winVar: 31,       // データが粗ければ広げる（例: 41～61）
                        winVar: 61,       // データが粗ければ広げる（例: 41～61）
                        startSkipM: 0.30, // 表層除外
                        endGuardM: 0.50,  // 末尾ガード
                        useAbs: true,
                        medianW: 3
                    );*/

                    //        ComputeBottomIndicesByEnvelopePeak(
                    //startSkipM: 0.1,       // 表層ノイズを避ける
                    //guardBelowFB_M: 0.10,   // FB直下の乱れを避ける
                    //searchDepth_M: 1.00,    // FBから下へ最大2mだけ見る（任意で調整）
                    //envWin: 25,             // 包絡平滑窓（奇数）
                    //sta: 15, lta: 50, staLtaThr: 3.0);


                    /* これが一番まし*/
                    ComputeBottomIndicesByDerivativeEdge(
                                                            startSkipM: 0.20,   // 表層回避
                                                            endGuardM: 0.50,   // 末尾ガード
                                                            smoothWin: 25,     // 15〜31 で調整
                                                            kMad: 3.0,    // 厳しめにしたい時は 4.5〜5.0
                                                            ampPct: 0.85,   // 誤検知多いなら 0.75〜0.85
                                                            refineWinM: 0.30,   // 到来点の直下0.3mでピークへ寄せる
                                                            useAbsLF: true,
                                                            applyHeaveInDetection: this.chkHeaveCorrection.Checked
                                                        );
                    /**/

                    //ComputeBottomIndicesByDerivativeEdge_EdgeIsBottom(
                    //    startSkipM: 0.20,
                    //    endGuardM: 0.50,
                    //    smoothWin: 19,
                    //    kMad: 4.0,
                    //    ampPct: 0.70,
                    //    confirmLen: 2,
                    //    minGapSamples: 8,
                    //    useAbsLF: true
                    //);
                }

                //EnsureAttenuationTable();
                BuildRgbaImage();
                UploadTexture();
                m_bTextureDirty = false;
            }

            GetContentSize(out double contentW, out double contentH, out double contentBottom);
            m_contentW = contentW;

            // スクロール反映
            GL.PushMatrix();
            if (applyScroll) GL.Translate(-m_dScrollX, -m_dScrollY, 0.0);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // テクスチャ描画（MeasureStart の段差をスムーズに）
            const int subdiv = 4;
            int w = m_iTexWidth;

            GL.Enable(EnableCap.Texture2D);
            GL.Color4(1f, 1f, 1f, 1f);
            GL.BindTexture(TextureTarget.Texture2D, m_iTexId);

            double pxPerM_X = GetPixelsPerMeterX();

            for (int i = 0; i < w - 1; i++)
            {
                //double x0 = (m_dTotalDistM > 0.0) ? m_cumDistM[i] * pxPerM_X : i * m_dScaleX;
                //double x1 = (m_dTotalDistM > 0.0) ? m_cumDistM[i + 1] * pxPerM_X : (i + 1) * m_dScaleX;
                double x0 = MapXByPing(i);
                double x1 = MapXByPing(i + 1);
                double y00 = (m_offsetPxPerPing != null) ? m_offsetPxPerPing[i] : 0.0;
                double y01 = (m_offsetPxPerPing != null) ? m_offsetPxPerPing[i + 1] : 0.0;

                GL.Begin(PrimitiveType.TriangleStrip);
                for (int s = 0; s <= subdiv; s++)
                {
                    double t = (double)s / subdiv;
                    double xt = x0 + (x1 - x0) * t;
                    double y0 = y00 + (y01 - y00) * t;
                    double u = (i + t) / (w - 1);

                    GL.TexCoord2(u, 0.0); GL.Vertex2(xt, y0);
                    GL.TexCoord2(u, 1.0); GL.Vertex2(xt, y0 + contentH);
                }
                GL.End();
            }

            GL.Disable(EnableCap.Texture2D);

            if(this.chkShowBtk.Checked) DrawBottomLine();

            if (drawLabels)
            {
                if (chkDrawDepthScale.Checked) DrawDepthScale(contentW);
                if (chkDrawDistScale.Checked) DrawDistanceScale(contentBottom);
            }

            if (m_mouseContentX is double mouseX)
            {
                GL.Color4(1f, 0f, 0f, 0.6f);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(mouseX, 0.0);
                GL.Vertex2(mouseX, contentBottom);
                GL.End();
            }

            if (m_mouseContentY is double mouseY)
            {
                GL.Color4(1f, 0f, 0f, 0.6f);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(0.0, mouseY);
                GL.Vertex2(contentW, mouseY);
                GL.End();
            }

            GL.PopMatrix(); // 画面座標へ

            if (drawLabels)
            {
                if (chkDrawDepthScale.Checked) DrawDepthLabelsPinnedLeft();
                if (chkDrawDistScale.Checked) DrawDistanceLabelsPinnedTop();
            }
            GL.Disable(EnableCap.Blend);
        }
        /// <summary>
        /// コンテンツ全体の幅高さを求める
        /// </summary>
        /// <param name="pxPerM_X"></param>
        /// <param name="contentW"></param>
        /// <param name="contentH"></param>
        /// <param name="contentBottom"></param>
        private void GetContentSize(out double contentW, out double contentH, out double contentBottom)
        {
            double pxPerM_X = GetPixelsPerMeterX();
            contentW = (m_cumDistM != null && m_dTotalDistM > 0)
                        ? m_dTotalDistM * pxPerM_X
                        : m_iPingNo * m_dScaleX;
            contentH = m_iSampleNo * m_dScaleY;
            contentBottom = contentH + m_dMaxOffsetPx;
        }

        private void CreateScrollBars()
        {
            if (hScroll != null) return;

            hScroll = new HScrollBar { Dock = DockStyle.Bottom, SmallChange = 32, LargeChange = 256 };
            vScroll = new VScrollBar { Dock = DockStyle.Right, SmallChange = 32, LargeChange = 256 };

            hScroll.Scroll += (_, __) => { m_dScrollX = hScroll.Value; glControl2D.Refresh(); };
            vScroll.Scroll += (_, __) => { m_dScrollY = vScroll.Value; glControl2D.Refresh(); };

            splitContainer1.Panel2.Controls.Add(hScroll);
            splitContainer1.Panel2.Controls.Add(vScroll);
        }

        private void UpdateScrollRanges()
        {
            m_iVpW = glControl2D.ClientSize.Width;
            m_iVpH = glControl2D.ClientSize.Height;

            double contentH = m_iSampleNo * m_dScaleY + m_dMaxOffsetPx;
            double contentW = (m_dTotalDistM > 0.0 && m_cumDistM != null && m_cumDistM.Length == m_iPingNo)
                                ? m_dTotalDistM * GetPixelsPerMeterX()
                                : m_iPingNo * m_dScaleX;

            int maxH = (int)Math.Max(0, Math.Ceiling(contentW - m_iVpW));
            int maxV = (int)Math.Max(0, Math.Ceiling(contentH - m_iVpH));

            hScroll.Visible = maxH > 0;
            vScroll.Visible = maxV > 0;

            hScroll.Minimum = vScroll.Minimum = 0;
            hScroll.LargeChange = Math.Max(1, m_iVpW);
            vScroll.LargeChange = Math.Max(1, m_iVpH);

            hScroll.Maximum = maxH + hScroll.LargeChange;
            vScroll.Maximum = maxV + vScroll.LargeChange;

            if (!hScroll.Visible) m_dScrollX = 0;
            if (!vScroll.Visible) m_dScrollY = 0;

            hScroll.Value = (int)Math.Max(hScroll.Minimum, Math.Min(hScroll.Maximum - hScroll.LargeChange, m_dScrollX));
            vScroll.Value = (int)Math.Max(vScroll.Minimum, Math.Min(vScroll.Maximum - vScroll.LargeChange, m_dScrollY));
        }

        private void UpdateViewportPreserveAspect()
        {
            EnsureTextureDimensions();

            int W = glControl2D.Width, H = glControl2D.Height;
            if (W <= 0 || H <= 0 || m_iTexWidth <= 0 || m_iTexHeight <= 0) return;

            // 2Dのため viewport 比率調整だけ（Projection は固定Ortho）
            GL.Viewport(0, 0, W, H);
        }

        private void SetupScreenOrtho()
        {
            m_iVpW = glControl2D.ClientSize.Width;
            m_iVpH = glControl2D.ClientSize.Height;

            GL.Viewport(0, 0, m_iVpW, m_iVpH);
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, m_iVpW, m_iVpH, 0, -1, 1); // 画面ピクセル座標
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        #endregion

        #region 画像生成

        private void InitTexture()
        {
            EnsureTextureDimensions();

            if (m_iTexId == 0)
            {
                GL.GenTextures(1, out m_iTexId);
                GL.BindTexture(TextureTarget.Texture2D, m_iTexId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            }

            m_rgba = new byte[m_iTexWidth * m_iTexHeight * 4];
            m_bLutDirty = true;
            m_bTextureDirty = true;
        }

        private void EnsureAttenuationTable()
        {
            if (m_attZ == null || m_attZ.Length != m_iTexHeight)
                m_attZ = new double[m_iTexHeight];

            double dAtt = Convert.ToDouble(numAttDb.Value);
            double attCoef = dAtt / 667.0 / 20.0;
            int iBottom = Math.Clamp(Convert.ToInt32(numBottom.Value), 0, m_iTexHeight);

            for (int z = 0; z < m_iTexHeight; z++)
                m_attZ[z] = (z < iBottom) ? 1.0 : Math.Pow(10.0, attCoef * (z - iBottom));
        }

        private void BuildPaletteLut()
        {
            m_lut = new byte[256 * 4];

            var colorMode = (ColorMode)cmbColor.SelectedIndex;
            bool invert = chkInvert.Checked;
            double dR = Convert.ToDouble(numR.Value);
            double dG = Convert.ToDouble(numG.Value);
            double dB = Convert.ToDouble(numB.Value);

            float r = 0, g = 0, b = 0;

            for (int i = 0; i < 256; i++)
            {
                float v = i / 255f;

                if (colorMode == ColorMode.Gray)
                {
                    float rr = (float)(v * dR);
                    float gg = (float)(v * dG);
                    float bb = (float)(v * dB);
                    if (invert) { rr = 1f - rr; gg = 1f - gg; bb = 1f - bb; }

                    m_lut[i * 4 + 0] = (byte)(Math.Clamp(rr, 0f, 1f) * 255);
                    m_lut[i * 4 + 1] = (byte)(Math.Clamp(gg, 0f, 1f) * 255);
                    m_lut[i * 4 + 2] = (byte)(Math.Clamp(bb, 0f, 1f) * 255);
                    m_lut[i * 4 + 3] = 255;
                }
                else
                {
                    float vv = invert ? (1f - v) : v;
                    ColorPalette.ToColor(vv, colorMode, ref r, ref g, ref b);

                    m_lut[i * 4 + 0] = (byte)(Math.Clamp(r, 0f, 1f) * 255);
                    m_lut[i * 4 + 1] = (byte)(Math.Clamp(g, 0f, 1f) * 255);
                    m_lut[i * 4 + 2] = (byte)(Math.Clamp(b, 0f, 1f) * 255);
                    m_lut[i * 4 + 3] = 255;
                }
            }
        }

        private void BuildRgbaImage()
        {
            int w = m_iTexWidth;
            int h = m_iTexHeight;

            if (m_rgba == null || m_rgba.Length != w * h * 4)
                m_rgba = new byte[w * h * 4];

            double dIntensity = Convert.ToDouble(numIntensity.Value);
            double thr = Convert.ToDouble(numThreshold.Value);
            byte alphaFull = (byte)(Math.Clamp((float)numAlpha.Value, 0f, 1f) * 255);

            // 減衰パラメータ（dAtt[db] を 667cm/20dBスケールへ換算して指数化）
            double dAttDb = Convert.ToDouble(numAttDb.Value);
            double attCoef = dAttDb / 667.0 / 20.0; // 既存式を流用：delta[サンプル] と組み合わせる

            double sampleIntervalCm = m_dZDistance;

            Parallel.For(0, h, z =>
            {
                for (int y = 0; y < w; y++)
                {
                    short[] wave = (DemodulateMode != DemodulationMode.None || this.lblHPF.Enabled) ?
                                        m_dataBlockList[y].Processed :
                                        m_dataBlockList[y].Lf;

                    bool hasData = true;
                    int zShifted = z;

                    // ヒーブ補正はサンプル選択に反映
                    int heaveOffset = 0;
                    if (chkHeaveCorrection.Checked)
                    {
                        double heave_cm = m_blockHeaderList[y].HeaveFromMotionSensor / 10.0;
                        heaveOffset = (int)Math.Round((-1.0 * heave_cm) / sampleIntervalCm);
                        zShifted = z + heaveOffset;
                        if (zShifted < 0 || zShifted >= h) hasData = false;
                    }

                    int p = (z * w + y) * 4;

                    if (!hasData)
                    {
                        m_rgba[p + 3] = 0; // 完全透明
                        continue;
                    }

                    double s = wave[zShifted];
                    if (Math.Abs(s) < thr) s = 0.0;

                    // --- ★ここが重要：ボトムから下だけ減衰補正 ---
                    int bottom = (m_bottomIdx != null && y < m_bottomIdx.Length) ? m_bottomIdx[y] : 0;
                    // 画素側でヒーブを適用したなら、ボトム側も同じだけシフトして整合
                    bottom += heaveOffset;
                    bottom = Math.Clamp(bottom, 0, h - 1);

                    int delta = zShifted - bottom; // ボトムからのサンプル差
                    double att = (delta > 0) ? Math.Pow(10.0, attCoef * delta) : 1.0;

                    double v = s * dIntensity * att;
                    double nv = Math.Min(Math.Abs(v), 1.0);
                    int idx = (int)(nv * 255.0);
                    int lut = idx * 4;

                    m_rgba[p + 0] = m_lut[lut + 2];
                    m_rgba[p + 1] = m_lut[lut + 1];
                    m_rgba[p + 2] = m_lut[lut + 0];
                    m_rgba[p + 3] = alphaFull;
                }
            });
        }


        private void UploadTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, m_iTexId);
            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba,
                m_iTexWidth, m_iTexHeight, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte,
                m_rgba);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);

            float maxAniso;
            GL.GetFloat((GetPName)All.MaxTextureMaxAnisotropyExt, out maxAniso);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)All.TextureMaxAnisotropyExt, MathF.Min(8f, maxAniso));
        }

        private void EnsureTextureDimensions()
        {
            m_iTexWidth = m_iPingNo;
            m_iTexHeight = m_iSampleNo;
        }

        #endregion

        #region その他
        private void SetBackgroundColor(System.Drawing.Color color)
        {
            glControl2D.MakeCurrent();
            GL.ClearColor(color);
            glControl2D.Refresh();
        }
        /// <summary>
        /// SignalProcessing
        /// </summary>
        private void SignalProcess()
        {
            SignalProcessingForm frmSp = new SignalProcessingForm(this, m_dSampleFreqHz);
            if (frmSp.ShowDialog(this) == DialogResult.OK)
            {
                m_bTextureDirty = true;
                glControl2D.Refresh();
            }
        }
        /// <summary>
        /// SaveImage
        /// </summary>
        private void SaveImageProcess()
        {
            using (var sfd = new SaveFileDialog()
            {
                Filter = "PNG Image|*.png",
                //FileName = "section.png"
                FileName = this.Text.Replace(".raw ", "") + ".png"
            })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    //SaveFullImagePng(sfd.FileName, outputWidthPx: 6000, includeLabels: true);
                    SaveFullImageAtCurrentScreenScale(sfd.FileName, includeLabels: true);
                }
            }
        }
        /// <summary>
        /// 次の描画フレームで GL 内容を PNG 保存します。
        /// 画像サイズは現在の glControl2D のピクセルサイズになります。
        /// </summary>
        public void SaveCurrentViewPng(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;
            m_strScreenshotPath = filePath;
            glControl2D.Refresh();  // 次の Paint で保存実行
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rgba"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void FlipVerticallyInPlace(byte[] rgba, int width, int height)
        {
            int stride = width * 4;
            byte[] line = new byte[stride];
            for (int y = 0; y < height / 2; y++)
            {
                int top = y * stride;
                int bottom = (height - 1 - y) * stride;
                System.Buffer.BlockCopy(rgba, top, line, 0, stride);
                System.Buffer.BlockCopy(rgba, bottom, rgba, top, stride);
                System.Buffer.BlockCopy(line, 0, rgba, bottom, stride);
            }
        }
        // ======== X座標の統一マッピング（反転対応の要）========
        /// <summary>直近の全コンテンツ幅(px)に対して水平反転を適用する。</summary>
        private double MapContentX(double xContent)
        {
            return m_bFlipX ? (m_contentW - xContent) : xContent;
        }
        /// <summary>Ping index → X(px)</summary>
        private double MapXByPing(int i)
        {
            double pxPerM_X = GetPixelsPerMeterX();
            double x = (m_dTotalDistM > 0.0) ? (m_cumDistM[i] * pxPerM_X) : (i * m_dScaleX);
            return MapContentX(x);
        }

        /// <summary>距離[m] → X(px)</summary>
        private double MapXByMeter(double m)
        {
            double pxPerM_X = GetPixelsPerMeterX();
            double x = m * pxPerM_X;
            return MapContentX(x);
        }
        // =====================================================

        /// <summary>
        /// 画面上のマウスX（pixels, 左上原点）から Ping index を求める。
        /// ビュー外なら -1 を返す。
        /// </summary>
        private int GetPingIndexAtMouseX(int mouseX)
        {
            // 1) 画面→コンテンツX（スクロール補正）
            double xContent = m_dScrollX + mouseX;

            // 最新の全コンテンツ幅を持っていない場合に備えて更新
            if (m_contentW <= 0)
            {
                GetContentSize(out double cw, out _, out _);
                m_contentW = cw;
            }

            // 範囲外は除外
            if (xContent < 0 || xContent > m_contentW) return -1;

            // 2) 左右反転の補正（描画時と逆変換）
            //   ※描画側は MapContentX(x) = m_bFlipX ? (m_contentW - x) : x
            //     よって逆変換は同じ式をもう一度適用（可逆）
            double xUnflipped = m_bFlipX ? (m_contentW - xContent) : xContent;

            int ping;
            if (m_dTotalDistM > 0.0 && m_cumDistM != null && m_cumDistM.Length == m_iPingNo)
            {
                // 3A) 測線距離ベースの横軸：コンテンツX(px) → 距離[m] → 最近傍のPing
                double pxPerM = GetPixelsPerMeterX();
                double meters = xUnflipped / pxPerM; // 0..m_dTotalDistM

                // m_cumDistM は単調増加なので二分探索
                // Array.BinarySearch は負値で ~ を取ると挿入位置が得られる
                int idx = Array.BinarySearch(m_cumDistM, meters);
                if (idx >= 0)
                {
                    ping = idx;
                }
                else
                {
                    int ins = ~idx;                 // meters を入れるべき位置
                    ping = Math.Max(0, ins - 1);    // 左側のPingを採用（区間の下端）
                }
            }
            else
            {
                // 3B) ピクセル等間隔の横軸：コンテンツX(px) → 列番号
                ping = (int)Math.Floor(xUnflipped / m_dScaleX);
            }

            // 4) クランプ
            if (ping < 0 || ping >= m_iPingNo) return -1;
            return ping;
        }
        private double? GetDepthMetersAtMouse(int pingIndex, int mouseY)
        {
            if (m_offsetPxPerPing == null) return null;
            if (pingIndex < 0 || pingIndex >= m_offsetPxPerPing.Length) return null;
            if (m_iSampleNo <= 0) return null;
            if (m_dScaleY <= 0 || m_dZDistance <= 0) return null;

            short[]? wave = GetActiveWaveform(pingIndex);
            if (wave == null || wave.Length == 0) return null;

            double yContent = m_dScrollY + mouseY;
            double relPx = yContent - m_offsetPxPerPing[pingIndex];

            double maxPx = (wave.Length - 1) * m_dScaleY;
            const double pxTolerance = 1e-6;
            if (relPx < -pxTolerance || relPx > maxPx + pxTolerance) return null;

            relPx = Math.Clamp(relPx, 0.0, maxPx);

            double sampleIndex = relPx / m_dScaleY;

            if (chkHeaveCorrection.Checked)
            {
                double sampleIntervalCm = m_dZDistance;
                if (sampleIntervalCm > 0.0)
                {
                    double heave_cm = m_blockHeaderList[pingIndex].HeaveFromMotionSensor / 10.0;
                    double heaveSamples = (-1.0 * heave_cm) / sampleIntervalCm;
                    sampleIndex += heaveSamples;
                }
            }

            double maxSampleIndex = wave.Length - 1;
            sampleIndex = Math.Clamp(sampleIndex, 0.0, maxSampleIndex);

            double dz_m = m_dZDistance / 100.0;
            double depth = BlockHeaderList[pingIndex].MeasureStart + sampleIndex * dz_m;

            double start = BlockHeaderList[pingIndex].MeasureStart;
            double length = BlockHeaderList[pingIndex].MeasureLength;
            if (length > 0)
            {
                double end = start + length;
                depth = Math.Clamp(depth, start, end);
            }

            return depth;
        }

        private void UpdateWaveformDepthGuide(double? depthMeters, bool refreshPlot = true)
        {
            if (formsPlot1 == null) return;

            if (depthMeters is double depth)
            {
                if (m_depthGuideLine == null)
                {
                    m_depthGuideLine = formsPlot1.Plot.Add.HorizontalLine(depth,
                                                                          color: ScottPlot.Color.FromColor(System.Drawing.Color.Gray),
                                                                          pattern: ScottPlot.LinePattern.Solid);

                    m_depthGuideLine.LineWidth = 1;
                }
                else
                {
                    m_depthGuideLine.Y = depth;
                    m_depthGuideLine.IsVisible = true;
                }
            }
            else if (m_depthGuideLine != null)
            {
                m_depthGuideLine.IsVisible = false;
            }

            if (refreshPlot)
            {
                formsPlot1.Refresh();
            }
        }

        private void ClearWaveformDepthGuide()
        {
            m_mouseDepthMeters = null;
            UpdateWaveformDepthGuide(null);
        }
        #endregion

        #region 減衰補正
        /// <summary>
        /// 分散（標本分散の期待値に相当；実装は母分散）最大の z をボトムとみなす。
        /// 1) |LF| で符号影響を抑制（useAbs=false で生波形）
        /// 2) 探索窓: startSkipM ～ (H - endGuardM) の間
        /// 3) 窓分散: prefix sums & prefix squares でO(h)算出
        /// 4) 出力後にPing方向の中央値フィルタで平滑
        /// </summary>
        private void ComputeBottomIndicesByStdDev(
            int winVar = 31,     // 分散計算窓（奇数推奨；デフォ 31サンプル）
            double startSkipM = 0.20,   // 上端スキップ[m]
            double endGuardM = 0.50,   // 下端ガード[m]
            bool useAbs = true,   // |LF| を使う
            int medianW = 5       // 出力のPing方向中央値フィルタ幅（奇数）
        )
        {
            int w = m_iPingNo;
            int h = m_iSampleNo;
            if (m_dataBlockList == null || w <= 0 || h <= 0) return;

            m_bottomIdx ??= new int[w];

            // パラメータ整形
            if (winVar < 5) winVar = 5;
            if (winVar % 2 == 0) winVar++;     // 奇数化
            int half = winVar / 2;

            int startSkip = (int)Math.Round((startSkipM * 100.0) / m_dZDistance);
            int endGuard = (int)Math.Round((endGuardM * 100.0) / m_dZDistance);

            Parallel.For(0, w, y =>
            {
                short[] lfRaw = m_dataBlockList[y].Lf;
                if (lfRaw == null || lfRaw.Length < h)
                {
                    m_bottomIdx[y] = Math.Min(10, h - 1);
                    return;
                }

                // 1) 信号を double 配列へ（必要なら絶対値）
                double[] s = new double[h];
                if (useAbs)
                {
                    for (int i = 0; i < h; i++) s[i] = Math.Abs((double)lfRaw[i]);
                }
                else
                {
                    for (int i = 0; i < h; i++) s[i] = lfRaw[i];
                }

                // 2) prefix sums と prefix squares（分散用）
                //    sum[z]   = s[0..z-1] の和（sum[0]=0）
                //    sum2[z]  = s^2 の和
                double[] sum = new double[h + 1];
                double[] sum2 = new double[h + 1];
                for (int i = 0; i < h; i++)
                {
                    sum[i + 1] = sum[i] + s[i];
                    sum2[i + 1] = sum2[i] + s[i] * s[i];
                }

                // 3) 探索範囲（窓がはみ出さないよう制限）
                int zMin = Math.Max(startSkip + half, half);
                int zMax = Math.Min(h - 1 - endGuard - half, h - 1 - half);
                if (zMin > zMax) { zMin = half; zMax = h - 1 - half; }

                // 4) 窓ごとの分散 = E[x^2] - (E[x])^2
                int zPick = zMin;
                double bestVar = double.NegativeInfinity;
                int winN = winVar;

                for (int z = zMin; z <= zMax; z++)
                {
                    int a = z - half;            // inclusive
                    int b = z + half + 1;        // exclusive
                    double S = sum[b] - sum[a];
                    double S2 = sum2[b] - sum2[a];
                    double mean = S / winN;
                    double var = (S2 / winN) - (mean * mean); // 母分散相当（Nで割る）

                    if (var > bestVar)
                    {
                        bestVar = var;
                        zPick = z;
                    }
                }

                m_bottomIdx[y] = Math.Clamp(zPick, 0, h - 1);
            });

            // 5) Ping方向の小さな跳ねを抑えるため中央値フィルタ（奇数幅）
            if (medianW >= 3 && medianW % 2 == 1)
            {
                int[] dst = new int[w];
                int mh = medianW / 2;
                int[] buf = new int[medianW];

                for (int i = 0; i < w; i++)
                {
                    int a = Math.Max(0, i - mh);
                    int b = Math.Min(w - 1, i + mh);
                    int n = 0;
                    for (int k = a; k <= b; k++) buf[n++] = m_bottomIdx[k];
                    Array.Sort(buf, 0, n);
                    dst[i] = buf[n / 2];
                }
                System.Buffer.BlockCopy(dst, 0, m_bottomIdx, 0, w * sizeof(int));
            }

            m_bBottomDirty = false;
        }

        /// <summary>
        /// 各PingのLF波形から海底面（最初の強反射）位置を検出し、m_bottomIdx[y] に格納。
        /// ロバスト化のために |signal| → 移動平均 → しきい値超えの最初のピーク近傍を採用。
        /// </summary>
        private void ComputeBottomIndices()
        {
            int w = m_iPingNo;
            int h = m_iSampleNo;
            if (m_dataBlockList == null || w <= 0 || h <= 0) return;

            m_bottomIdx ??= new int[w];

            // 検出パラメータ（必要なら NumericUpDown にすることも可能）
            int smoothWin = Math.Clamp(h / 200, 7, 31); // 画像サイズに応じた平滑窓（奇数）
            if (smoothWin % 2 == 0) smoothWin++;
            double startSkipM = 0.20;                   // 開始直後は除外（例：0.20 m）
            double endGuardM = 0.50;                   // 末尾ガード（例：0.50 m）
            int startSkip = (int)Math.Round((startSkipM * 100.0) / m_dZDistance);
            int endGuard = (int)Math.Round((endGuardM * 100.0) / m_dZDistance);

            Parallel.For(0, w, y =>
            {
                var lf = m_dataBlockList[y].Lf;
                //var lf = m_dataBlockList[y].Hf;
                if (lf == null || lf.Length < h) { m_bottomIdx[y] = Math.Min(10, h - 1); return; }

                // 1) 絶対値 & 平滑
                Span<double> env = stackalloc double[h];
                for (int z = 0; z < h; z++) env[z] = Math.Abs((double)lf[z]);

                // 移動平均（単純で十分）
                int half = smoothWin / 2;
                double run = 0;
                for (int z = 0; z < h; z++)
                {
                    int z0 = Math.Max(0, z - half);
                    int z1 = Math.Min(h - 1, z + half);
                    if (z == 0)
                    {
                        for (int k = z0; k <= z1; k++) run += env[k];
                    }
                    else
                    {
                        int prev0 = Math.Max(0, (z - 1) - half);
                        int prev1 = Math.Min(h - 1, (z - 1) + half);
                        // 窓が1つ下にずれるときの差分更新
                        if (z1 > prev1) run += env[z1];
                        if (z0 > prev0) run -= env[prev0];
                    }
                    int win = (Math.Min(h - 1, z + half) - Math.Max(0, z - half) + 1);
                    env[z] = run / Math.Max(1, win);
                }

                // 2) ロバストなしきい値（メディアン + k*MAD）
                //    k=3.5 程度が経験的に安定（必要なら調整）
                double median = Median(env);
                double mad = MedianAbsoluteDeviation(env, median) + 1e-9;
                double thr = median + 3.5 * mad;
                //double thr = median + 5.0 * mad;

                int searchStart = Math.Clamp(startSkip, 0, h - 1);
                int searchEnd = Math.Clamp(h - 1 - endGuard, 0, h - 1);

                // 3) しきい値超えの「最初の強い領域」のピークを海底とする
                int zPick = searchEnd;
                bool foundBand = false;
                int bandStart = -1;

                for (int z = searchStart; z <= searchEnd; z++)
                {
                    if (!foundBand && env[z] > thr)
                    {
                        foundBand = true;
                        bandStart = z;
                    }
                    if (foundBand && env[z] <= thr)
                    {
                        // バンド終了 → バンド内最大の位置
                        int bandEnd = z - 1;
                        zPick = ArgMax(env, bandStart, bandEnd);
                        break;
                    }
                }

                // しきい値超えが最後まで続いた場合
                if (foundBand && env[searchEnd] > thr)
                    zPick = ArgMax(env, bandStart, searchEnd);

                // フォールバック
                if (!foundBand)
                {
                    // 単純に env の最大
                    zPick = ArgMax(env, searchStart, searchEnd);
                }

                m_bottomIdx[y] = Math.Clamp(zPick, 0, h - 1);
            });

            m_bBottomDirty = false;

            // --- ローカル関数 ---
            static int ArgMax(Span<double> a, int s, int e)
            {
                s = Math.Max(0, s); e = Math.Min(a.Length - 1, e);
                int ix = s; double best = a[s];
                for (int i = s + 1; i <= e; i++)
                    if (a[i] > best) { best = a[i]; ix = i; }
                return ix;
            }
            static double Median(Span<double> a)
            {
                double[] tmp = a.ToArray();
                Array.Sort(tmp);
                int n = tmp.Length;
                return (n % 2 == 1) ? tmp[n / 2] : 0.5 * (tmp[n / 2 - 1] + tmp[n / 2]);
            }
            static double MedianAbsoluteDeviation(Span<double> a, double med)
            {
                double[] dev = new double[a.Length];
                for (int i = 0; i < a.Length; i++) dev[i] = Math.Abs(a[i] - med);
                Array.Sort(dev);
                int n = dev.Length;
                double mad = (n % 2 == 1) ? dev[n / 2] : 0.5 * (dev[n / 2 - 1] + dev[n / 2]);
                // 正規分布での尺度補正はここでは使用しない（しきい値kで吸収）
                return mad;
            }
        }
        private int EstimateFirstBreak_STA_LTA(double[] sAbs, int sta = 12, int lta = 60, double thr = 3.0, int searchStart = 0)
        {
            int H = sAbs.Length;
            double[] S = new double[H + 1];
            for (int i = 0; i < H; i++) S[i + 1] = S[i] + sAbs[i] * sAbs[i];

            for (int z = Math.Max(searchStart, lta + 1); z < H; z++)
            {
                int l0 = z - lta, l1 = z;             // [l0, l1)
                int s0 = Math.Max(0, z - sta), s1 = z; // [s0, s1)
                double L = (S[l1] - S[l0]) / lta;
                double Sshort = (S[s1] - S[s0]) / Math.Max(1, s1 - s0);
                if (L > 1e-12 && (Sshort / L) >= thr) return z;
            }
            return Math.Min(searchStart + lta, H - 1); // 見つからない場合のフォールバック
        }
        /// <summary>
        /// 「強い反射のピーク」をボトムとして選ぶ：
        /// 1) |LF| を移動平均で平滑した包絡 env を作る
        /// 2) (FB + guard) ～ (FB + depthMax) の範囲で env が最大の z を採用
        /// </summary>
        private void ComputeBottomIndicesByEnvelopePeak(
            double startSkipM = 0.20,   // 上端除外（表層ノイズ避け）
            double guardBelowFB_M = 0.10, // FB直下のガード（すぐ上の乱れを避ける）
            double searchDepth_M = 2.00, // FBから下へ探索する最大深さ
            int envWin = 21,          // 包絡の移動平均窓（奇数）
            int sta = 12, int lta = 60, double staLtaThr = 3.0
        )
        {
            int W = m_iPingNo, H = m_iSampleNo;
            if (m_dataBlockList == null || W == 0 || H == 0) return;
            m_bottomIdx ??= new int[W];

            if (envWin < 5) envWin = 5;
            if (envWin % 2 == 0) envWin++;

            int startSkip = (int)Math.Round((startSkipM * 100.0) / m_dZDistance);
            int guardFB = (int)Math.Round((guardBelowFB_M * 100.0) / m_dZDistance);
            int searchMax = (int)Math.Round((searchDepth_M * 100.0) / m_dZDistance);

            Parallel.For(0, W, y =>
            {
                var lfRaw = m_dataBlockList[y].Lf;
                if (lfRaw == null || lfRaw.Length < H) { m_bottomIdx[y] = Math.Min(10, H - 1); return; }

                // 絶対値にしてから包絡（=移動平均）
                double[] sAbs = new double[H];
                for (int i = 0; i < H; i++) sAbs[i] = Math.Abs((double)lfRaw[i]);

                // まず軽量 STA/LTA で到来点（FB）を概算
                int fb = EstimateFirstBreak_STA_LTA(sAbs, sta, lta, staLtaThr, searchStart: startSkip);

                // 包絡（移動平均）
                int half = envWin / 2;
                double run = 0;
                double[] env = new double[H];
                for (int z = 0; z < H; z++)
                {
                    int a = Math.Max(0, z - half);
                    int b = Math.Min(H - 1, z + half);
                    if (z == 0)
                    {
                        for (int k = a; k <= b; k++) run += sAbs[k];
                    }
                    else
                    {
                        int pa = Math.Max(0, (z - 1) - half);
                        int pb = Math.Min(H - 1, (z - 1) + half);
                        if (b > pb) run += sAbs[b];
                        if (a > pa) run -= sAbs[pa];
                    }
                    int winN = (Math.Min(H - 1, z + half) - Math.Max(0, z - half) + 1);
                    env[z] = run / Math.Max(1, winN);
                }

                // 探索窓：FBの少し下から、一定深さまで
                int z0 = Math.Clamp(fb + guardFB, 0, H - 1);
                int z1 = Math.Clamp(fb + searchMax, 0, H - 1);
                if (z1 <= z0) z1 = Math.Min(H - 1, z0 + Math.Max(10, envWin));

                // 最大ピークをボトムに
                int zPick = z0;
                double best = -1.0;
                for (int z = z0; z <= z1; z++)
                {
                    double v = env[z];
                    if (v > best) { best = v; zPick = z; }
                }

                m_bottomIdx[y] = Math.Clamp(zPick, 0, H - 1);
            });

            // 連続性のためにごく弱い中央値平滑（3～5）
            int Wm = 5;
            if (Wm % 2 == 1 && Wm >= 3 && W >= Wm)
            {
                int[] dst = new int[W];
                int r = Wm / 2;
                for (int i = 0; i < W; i++)
                {
                    int a = Math.Max(0, i - r), b = Math.Min(W - 1, i + r);
                    int n = b - a + 1;
                    int[] buf = new int[n];
                    for (int k = 0; k < n; k++) buf[k] = m_bottomIdx[a + k];
                    Array.Sort(buf);
                    dst[i] = buf[n / 2];
                }
                System.Buffer.BlockCopy(dst, 0, m_bottomIdx, 0, W * sizeof(int));
            }

            m_bBottomDirty = false;
        }


        /// <summary>
        /// 微分エッジ法：|LF| を平滑 → 中心差分で微分 →
        /// 「最初に |d| が適応閾値を超え、かつ強度がフロア以上」の z を到来点、
        /// さらに直後の小窓で包絡ピークへ寄せて最終的なボトム z を確定。
        /// </summary>
        /// 微分エッジ法（ヒーブ補正後の座標系で検出）
        private void ComputeBottomIndicesByDerivativeEdge(
            double startSkipM = 0.20,
            double endGuardM = 0.50,
            int smoothWin = 19,
            double kMad = 4.0,
            double ampPct = 0.70,
            double refineWinM = 0.30,
            bool useAbsLF = true,
            bool applyHeaveInDetection = true // ★検出にもヒーブを適用
        )
        {
            int W = m_iPingNo, H = m_iSampleNo;
            if (m_dataBlockList == null || W == 0 || H == 0) return;
            m_bottomIdx ??= new int[W];

            if (smoothWin < 5) smoothWin = 5;
            if (smoothWin % 2 == 0) smoothWin++;
            int half = smoothWin / 2;

            int startSkip = (int)Math.Round((startSkipM * 100.0) / m_dZDistance);
            int endGuard = (int)Math.Round((endGuardM * 100.0) / m_dZDistance);
            int refineN = Math.Max(5, (int)Math.Round((refineWinM * 100.0) / m_dZDistance));

            // 表示側の「ヒーブ適用」チェックに追従（固定で適用したいなら true に）
            bool heaveOn = applyHeaveInDetection && chkHeaveCorrection.Checked;

            Parallel.For(0, W, y =>
            {
                var lf = m_dataBlockList[y].Lf;
                if (lf == null || lf.Length < H) { m_bottomIdx[y] = Math.Min(10, H - 1); return; }

                // ★ ピングごとの heave オフセット（サンプル単位：表示zに対して元波形の参照は z+offset）
                int heaveOffset = 0;
                if (heaveOn)
                {
                    double heave_cm = m_blockHeaderList[y].HeaveFromMotionSensor / 10.0; // mm→cm の想定
                    heaveOffset = (int)Math.Round((-1.0 * heave_cm) / m_dZDistance);
                }

                // 有効な「表示z」範囲（z+heaveOffset が [0,H-1] になる区間）
                int validMin = Math.Max(0, -heaveOffset);
                int validMax = Math.Min(H - 1, H - 1 - heaveOffset);

                // 平滑窓・微分を安全にするための余白を考慮
                int zMin = Math.Max(validMin + half + 1, startSkip + half + 1);
                int zMax = Math.Min(validMax - half - 1, H - 2 - endGuard);
                if (zMin >= zMax) { m_bottomIdx[y] = Math.Clamp(startSkip, 0, H - 1); return; }

                // 1) env（|LF| の移動平均）※参照は常に src = z + heaveOffset
                double[] env = new double[H];
                double run = 0;

                // ヘルパ：ソースの安全参照
                double Src(int z)
                {
                    int zs = z + heaveOffset;
                    if ((uint)zs < (uint)H) return useAbsLF ? Math.Abs((double)lf[zs]) : (double)lf[zs];
                    return 0.0; // はみ出しは0扱い（他に pad/hold にしてもよい）
                }

                for (int z = 0; z < H; z++)
                {
                    int a = Math.Max(0, z - half);
                    int b = Math.Min(H - 1, z + half);
                    if (z == 0)
                    {
                        run = 0;
                        for (int k = a; k <= b; k++) run += Src(k);
                    }
                    else
                    {
                        int pa = Math.Max(0, (z - 1) - half);
                        int pb = Math.Min(H - 1, (z - 1) + half);
                        if (b > pb) run += Src(b);
                        if (a > pa) run -= Src(pa);
                    }
                    int winN = (Math.Min(H - 1, z + half) - Math.Max(0, z - half) + 1);
                    env[z] = run / Math.Max(1, winN);
                }

                // 2) 中心差分の絶対値 |d|
                double[] dabs = new double[H];
                for (int z = 1; z < H - 1; z++)
                    dabs[z] = Math.Abs(0.5 * (env[z + 1] - env[z - 1]));
                dabs[0] = dabs[1]; dabs[H - 1] = dabs[H - 2];

                // 3) 適応しきい値（MAD）と強度フロア（百分位）
                (double mad, double medD) = MadOfRange(dabs, zMin, zMax);
                double thrD = Math.Max(1e-12, medD + kMad * mad);
                double ampFloor = Percentile(env, zMin, zMax, ampPct);

                // 4) 最初のエッジ（|d| >= thr && env >= floor）
                int zEdge = -1;
                for (int z = zMin; z <= zMax; z++)
                {
                    if (dabs[z] >= thrD && env[z] >= ampFloor)
                    {
                        // 簡易ヒステリシス
                        int ok = 0, need = 2;
                        for (int k = 1; k <= need && z + k <= zMax; k++)
                            if (dabs[z + k] >= 0.6 * thrD) ok++;
                        if (ok >= need - 1) { zEdge = z; break; }
                    }
                }
                if (zEdge < 0) zEdge = zMin;

                // 5) “最初の変化の直後で小窓ピークへ寄せる”ロジック（従来どおり）
                int a2 = Math.Min(zEdge + 1, zMax);
                int b2 = Math.Min(zEdge + refineN, zMax);
                int zPick = a2; double best = env[a2];
                for (int z = a2; z <= b2; z++)
                    if (env[z] > best) { best = env[z]; zPick = z; }

                m_bottomIdx[y] = Math.Clamp(zPick, 0, H - 1); // ★この z は“ヒーブ補正後の表示z”
            });

            // 進行方向の軽い中央値平滑（任意）
            int Wm = 5;
            if (W >= Wm && (Wm % 2 == 1))
            {
                int[] dst = new int[W];
                int r = Wm / 2;
                for (int i = 0; i < W; i++)
                {
                    int a = Math.Max(0, i - r), b = Math.Min(W - 1, i + r);
                    int n = b - a + 1;
                    int[] buf = new int[n];
                    for (int k = 0; k < n; k++) buf[k] = m_bottomIdx[a + k];
                    Array.Sort(buf);
                    dst[i] = buf[n / 2];
                }
                System.Buffer.BlockCopy(dst, 0, m_bottomIdx, 0, W * sizeof(int));
            }

            m_bBottomDirty = false;

            // --- ヘルパ ---
            static (double mad, double med) MadOfRange(double[] a, int s, int e)
            {
                int n = e - s + 1;
                double[] tmp = new double[n];
                for (int i = 0; i < n; i++) tmp[i] = a[s + i];
                Array.Sort(tmp);
                double med = (n % 2 == 1) ? tmp[n / 2] : 0.5 * (tmp[n / 2 - 1] + tmp[n / 2]);
                for (int i = 0; i < n; i++) tmp[i] = Math.Abs(tmp[i] - med);
                Array.Sort(tmp);
                double mad = (n % 2 == 1) ? tmp[n / 2] : 0.5 * (tmp[n / 2 - 1] + tmp[n / 2]);
                return (mad, med);
            }
            static double Percentile(double[] a, int s, int e, double p)
            {
                p = Math.Clamp(p, 0.0, 1.0);
                int n = e - s + 1;
                double[] tmp = new double[n];
                for (int i = 0; i < n; i++) tmp[i] = a[s + i];
                Array.Sort(tmp);
                double idx = p * (n - 1);
                int i0 = (int)Math.Floor(idx), i1 = Math.Min(n - 1, i0 + 1);
                double t = idx - i0;
                return tmp[i0] * (1 - t) + tmp[i1] * t;
            }
        }

        /// <summary>
        /// 微分エッジ法（ボトム＝最初の大変化）: 
        /// |LF| を平滑 → 中心差分の絶対値 |d| → 適応しきい値を初めて超えた z をボトムに採用。
        /// ピークへの寄せは行わない。
        /// </summary>
        private void ComputeBottomIndicesByDerivativeEdge_EdgeIsBottom(
            double startSkipM = 0.20,   // 上端除外[m]
            double endGuardM = 0.50,   // 末尾ガード[m]
            int smoothWin = 19,     // 平滑窓（奇数 15–31）
            double kMad = 4.0,    // 閾値 = median(|d|) + kMad * MAD（2.5–5.0）
            double ampPct = 0.70,   // 強度フロア（env の百分位 0.6–0.85）
            int confirmLen = 2,      // ヒステリシス：後続 confirmLen サンプルも高め継続
            int minGapSamples = 8,      // デバウンス：複数エッジの最小間隔
            bool useAbsLF = true    // 平滑前に絶対値化
        )
        {
            int W = m_iPingNo, H = m_iSampleNo;
            if (m_dataBlockList == null || W == 0 || H == 0) return;
            m_bottomIdx ??= new int[W];

            if (smoothWin < 5) smoothWin = 5;
            if (smoothWin % 2 == 0) smoothWin++;

            int startSkip = (int)Math.Round((startSkipM * 100.0) / m_dZDistance);
            int endGuard = (int)Math.Round((endGuardM * 100.0) / m_dZDistance);
            int half = smoothWin / 2;

            Parallel.For(0, W, y =>
            {
                var lfRaw = m_dataBlockList[y].Lf;
                if (lfRaw == null || lfRaw.Length < H) { m_bottomIdx[y] = Math.Min(10, H - 1); return; }

                // 1) 包絡っぽく |LF| を平滑（移動平均）
                double[] env = new double[H];
                double run = 0;
                for (int z = 0; z < H; z++)
                {
                    double v = useAbsLF ? Math.Abs((double)lfRaw[z]) : (double)lfRaw[z];
                    int a = Math.Max(0, z - half);
                    int b = Math.Min(H - 1, z + half);
                    if (z == 0)
                    {
                        run = 0;
                        for (int k = a; k <= b; k++)
                            run += (useAbsLF ? Math.Abs((double)lfRaw[k]) : (double)lfRaw[k]);
                    }
                    else
                    {
                        int pa = Math.Max(0, (z - 1) - half);
                        int pb = Math.Min(H - 1, (z - 1) + half);
                        if (b > pb) run += (useAbsLF ? Math.Abs((double)lfRaw[b]) : (double)lfRaw[b]);
                        if (a > pa) run -= (useAbsLF ? Math.Abs((double)lfRaw[pa]) : (double)lfRaw[pa]);
                    }
                    int winN = (Math.Min(H - 1, z + half) - Math.Max(0, z - half) + 1);
                    env[z] = run / Math.Max(1, winN);
                }

                // 2) 中心差分で微分の絶対値 |d|
                double[] dabs = new double[H];
                for (int z = 1; z < H - 1; z++)
                    dabs[z] = Math.Abs(0.5 * (env[z + 1] - env[z - 1]));
                dabs[0] = dabs[1]; dabs[H - 1] = dabs[H - 2];

                // 3) 探索範囲
                int zMin = Math.Max(startSkip + half + 1, 1);
                int zMax = Math.Min(H - 1 - endGuard - 1, H - 2);
                if (zMin >= zMax) { m_bottomIdx[y] = Math.Clamp(startSkip, 0, H - 1); return; }

                // 4) 適応しきい値（MAD）と強度フロア
                (double mad, double medD) = MadOfRange(dabs, zMin, zMax);
                double thrD = Math.Max(1e-12, medD + kMad * mad);
                double ampFloor = Percentile(env, zMin, zMax, ampPct);

                // 5) 最初のエッジ（ヒステリシス＋デバウンス）＝ボトム
                int lastEdge = -minGapSamples - 1;
                int zEdge = -1;
                for (int z = zMin; z <= zMax; z++)
                {
                    if (dabs[z] >= thrD && env[z] >= ampFloor && (z - lastEdge) >= minGapSamples)
                    {
                        // 連続確認（confirmLen サンプル後も高め継続）
                        int ok = 0;
                        for (int k = 1; k <= confirmLen && z + k <= zMax; k++)
                            if (dabs[z + k] >= 0.6 * thrD) ok++;
                        if (ok >= Math.Max(1, confirmLen - 1))
                        {
                            zEdge = z;
                            break; // ★最初のものを採用
                        }
                    }
                    if (dabs[z] >= thrD) lastEdge = z;
                }

                if (zEdge < 0) zEdge = zMin; // フォールバック
                m_bottomIdx[y] = Math.Clamp(zEdge, 0, H - 1);
            });

            // 進行方向の軽い平滑（任意・入れすぎ注意）
            int mw = 3; // 3～5 くらい。線がギザるなら5
            if (W >= mw && mw % 2 == 1)
            {
                int[] dst = new int[W];
                int r = mw / 2;
                for (int i = 0; i < W; i++)
                {
                    int a = Math.Max(0, i - r), b = Math.Min(W - 1, i + r);
                    int n = b - a + 1;
                    int[] buf = new int[n];
                    for (int k = 0; k < n; k++) buf[k] = m_bottomIdx[a + k];
                    Array.Sort(buf);
                    dst[i] = buf[n / 2];
                }
                System.Buffer.BlockCopy(dst, 0, m_bottomIdx, 0, W * sizeof(int));
            }

            m_bBottomDirty = false;

            // --- ヘルパ ---
            static (double mad, double med) MadOfRange(double[] a, int s, int e)
            {
                int n = e - s + 1;
                double[] tmp = new double[n];
                for (int i = 0; i < n; i++) tmp[i] = a[s + i];
                Array.Sort(tmp);
                double med = (n % 2 == 1) ? tmp[n / 2] : 0.5 * (tmp[n / 2 - 1] + tmp[n / 2]);
                for (int i = 0; i < n; i++) tmp[i] = Math.Abs(tmp[i] - med);
                Array.Sort(tmp);
                double mad = (n % 2 == 1) ? tmp[n / 2] : 0.5 * (tmp[n / 2 - 1] + tmp[n / 2]);
                return (mad, med);
            }
            static double Percentile(double[] a, int s, int e, double p)  // p∈[0,1]
            {
                p = Math.Clamp(p, 0.0, 1.0);
                int n = e - s + 1;
                double[] tmp = new double[n];
                for (int i = 0; i < n; i++) tmp[i] = a[s + i];
                Array.Sort(tmp);
                double idx = p * (n - 1);
                int i0 = (int)Math.Floor(idx), i1 = Math.Min(n - 1, i0 + 1);
                double t = idx - i0;
                return tmp[i0] * (1 - t) + tmp[i1] * t;
            }
        }

        #endregion

        public double? AddContactProcess(double depth, int ping)//ここ仮名。変更予定
        {
            double dAnomaryDepth = 0.0;

            // 1回目のクリック
            if (m_clickStep == 0)
            {
                m_dBottomDepth = depth;

                m_clickStep = 1;
                this.toolStripButtonAddContact.Enabled = false;

                return null;
            }
            // 2回目のクリック
            else if (m_clickStep == 1)
            {
                if (m_dBottomDepth.HasValue)
                {
                    dAnomaryDepth = Math.Round(Math.Abs(m_dBottomDepth.Value - depth), 2);//深さのため、絶対値計算。小数点第２位まで表示
                    m_clickStep = 0;
                    this.toolStripButtonAddContact.Checked = false;
                    this.toolStripButtonAddContact.Enabled = true;
                    m_frmMap.AddClickedCurSor(ping, this);//? この方法でいいのか？もしAnalysisFormを渡すとき

                    return dAnomaryDepth;
                }
                else
                {
                    MessageBox.Show("最初の高さ情報が取得できません。処理を中断します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    m_clickStep = 0;
                    this.toolStripButtonAddContact.Checked = false;
                    this.toolStripButtonAddContact.Enabled = true;
                    return null;
                }
                //MessageBox.Show($"埋没深度 : {objectDepth}");
                //analysisForm.ToolStripButtonAddContactChecked = false; // ボタンの状態を戻す
                //analysisForm.ToolStripButtonAddContactEnabled = true;
            }
            return null;//到達予定はないが、警告回避のために記述
        }



        #region クリックイベントなど
        private void glControl2D_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_bDraggingAddContact)//! スクロール判定のためm_bDraggingAddContactを追加。1119
            {
                // ドラッグ後のクリックは無視
                return;
            }
            if (toolStripButtonAddContact.Checked)
            {
                int pingNo = GetPingIndexAtMouseX(e.X);

                glControl2D.Cursor = Cursors.Cross;//二回目のクリックに切り替わったことを示すカーソル変更

                if (m_mouseDepthMeters == null)
                {
                    return;
                }
                else if (m_mouseDepthMeters.HasValue)
                {
                    double dAnomaryDepth = m_mouseDepthMeters.Value;
                    double? dBurialDepth = AddContactProcess(dAnomaryDepth, pingNo);
                    m_bDraggingAddContact = false;

                    if (dBurialDepth.HasValue)
                    {
                        int iAnomaryNo = m_frmMap.AnomaryList.Count + 1;
                        var (noCurSorFilePath, withCurSorFilePath) = GenerateScreenshotFilePath(iAnomaryNo);//filepath作成

                        Anomary anomary = new Anomary()
                        {
                            FileName = Path.GetFileName(m_rawFileName),
                            AnonaryNo = iAnomaryNo,
                            Date = Method.ConvertDateString(BlockHeaderList[pingNo].Date, "yyyy/MM/dd"),
                            Time = BlockHeaderList[pingNo].Time,
                            PingNo = pingNo,
                            Easting = double.Parse(BlockHeaderList[pingNo].SisString5),
                            Northing = double.Parse(BlockHeaderList[pingNo].SisString6),
                            Latitude = double.Parse(BlockHeaderList[pingNo].SisString2),
                            Longitude = double.Parse(BlockHeaderList[pingNo].SisString3),
                            BottomDepth = m_dBottomDepth.Value,
                            AnomaryDepth = dAnomaryDepth,
                            BurialDepth = dBurialDepth.Value,
                            Screenshot1 = Path.GetFileName(noCurSorFilePath),
                            Screenshot2 = Path.GetFileName(withCurSorFilePath),
                        };
                        m_frmMap.AnomaryList.Add(anomary); // AnomaryList
                        m_frmMap.UpdateDataGridView();

                        CaptureWindowWithFrame(this.ParentForm, noCurSorFilePath);
                        CaptureWindowWithFrameWithCursor(this.ParentForm, withCurSorFilePath);

                        MessageBox.Show($"コンタクトが追加されました。深度 : {dBurialDepth}", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        glControl2D.Cursor = Cursors.Default;

                    }
                }
            }
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        #endregion





        private void toolStripButtonAddContact_CheckedChanged(object sender, EventArgs e)
        {
            // 現在の状態
            bool nowChecked = toolStripButtonAddContact.Checked;

            // false→trueの時だけ処理
            if (!m_prevAddContactChecked && nowChecked)
            {
                if (m_doNotShowUsageAddContactForm)
                    return;

                UsageAddContactForm usageForm = new UsageAddContactForm();
                DialogResult result = usageForm.ShowDialog();
                if (usageForm.DoNotShowAgain)
                {
                    m_doNotShowUsageAddContactForm = true;
                }
                if (result == DialogResult.OK)
                {
                    toolStripButtonAddContact.Checked = true;

                }
                else
                {
                    toolStripButtonAddContact.Checked = false;
                    //m_prevAddContactChecked = nowChecked;
                    return;
                }
            }

            // 状態を更新
            m_prevAddContactChecked = nowChecked;
        }




        #region スクリーンショット保存関連 ファイルパス生成など
        private (string noCursorFilePath, string withCursorFilePath) GenerateScreenshotFilePath(int anomaryNo)
        {
            string dir = Properties.Settings.Default.RawDir + @"\Anomary";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string noCursorFilePath = Path.Combine(dir, $"anomary{anomaryNo.ToString("D2")}.png");
            string withCurSorFilePath = Path.Combine(dir, $"anomary{anomaryNo.ToString("D2")}_cursor.png");

            return (noCursorFilePath, withCurSorFilePath);
        }
        private void CaptureWindowWithFrame(Form targetForm, string filePath)
        {

            glControl2D.Refresh();
            Application.DoEvents();
            // フォームの位置とサイズを取得
            Rectangle formRect = new Rectangle(targetForm.Location, targetForm.Size);

            using (Bitmap bmp = new Bitmap(formRect.Width, formRect.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // フォームの左上座標からフォームサイズ分だけキャプチャ
                    g.CopyFromScreen(formRect.Location, Point.Empty, formRect.Size);
                }

                bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }//マウスカーソルなしウィンドウすべて


        /*
        private void CaptureWindowWithFrameWithCursor(Form targetForm, string filePath)
        {
            glControl2D.Refresh();
            Application.DoEvents();



            

            // フォームのスクリーン座標とサイズを取得
            Rectangle screenRect = targetForm.RectangleToScreen(targetForm.ClientRectangle);

            using (Bitmap bmp = new Bitmap(screenRect.Width, screenRect.Height))
            {

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    //! カスタムカーソルの作成(緑色十字)
                    g.Clear(System.Drawing.Color.Transparent);
                    using (Pen pen = new Pen(System.Drawing.Color.LightGreen, 2))
                    {
                        g.DrawLine(pen, 16, 0, 16, 32);
                        g.DrawLine(pen, 0, 16, 32, 16);
                    }

                    Cursor customCross = new Cursor(bmp.GetHicon());







                    // スクリーン座標でキャプチャ
                    g.CopyFromScreen(screenRect.Location, Point.Empty, screenRect.Size);

                    // カーソルの絶対座標（スクリーン座標）
                    Point absPos = Cursor.Position;

                    // 画像内の座標（絶対座標をそのまま使う場合は注意）
                    // 画像内に描画するには↓
                    Point relPos = new Point(absPos.X - screenRect.Left, absPos.Y - screenRect.Top);

                    // relPosが画像内なら描画
                    if (relPos.X >= 0 && relPos.X < bmp.Width && relPos.Y >= 0 && relPos.Y < bmp.Height)
                    {
                        //Cursors.Cross.Draw(g, new Rectangle(relPos, Cursors.Cross.Size));
                        customCross.Draw(g, new Rectangle(relPos, customCross.Size));
                    }
                }
                bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }





        }//マウスカーソルありウィンドウすべて

        */


        private void CaptureWindowWithFrameWithCursor(Form targetForm, string filePath)
        {
            glControl2D.Refresh();
            Application.DoEvents();

            Rectangle screenRect = targetForm.RectangleToScreen(targetForm.ClientRectangle);

            using (Bitmap bmp = new Bitmap(screenRect.Width, screenRect.Height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // 画面キャプチャ
                g.CopyFromScreen(screenRect.Location, Point.Empty, screenRect.Size);

                // カーソル位置計算
                Point absPos = Cursor.Position;
                Point relPos = new Point(absPos.X - screenRect.Left, absPos.Y - screenRect.Top);



                using (Bitmap crossBmp = new Bitmap(32, 32))
                using (Graphics cg = Graphics.FromImage(crossBmp))
                {
                    cg.Clear(System.Drawing.Color.Transparent);
                    using (Pen pen = new Pen(System.Drawing.Color.OrangeRed, 5))
                    {
                        cg.DrawLine(pen, 16, 0, 16, 32);
                        cg.DrawLine(pen, 0, 16, 32, 16);
                    }
                    // relPosが画像内なら描画
                    if (relPos.X >= 0 && relPos.X < bmp.Width && relPos.Y >= 0 && relPos.Y < bmp.Height)
                    {
                        g.DrawImage(crossBmp, new Rectangle(relPos, crossBmp.Size));
                    }
                }

                bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }



        public void SaveGLControl2DPng(string filePath)
        {


            if (glControl2D == null || glControl2D.Width <= 0 || glControl2D.Height <= 0)
                return;

            glControl2D.Refresh();
            Application.DoEvents();

            glControl2D.MakeCurrent();
            int w = glControl2D.Width;
            int h = glControl2D.Height;
            byte[] pixels = new byte[w * h * 4];
            GL.ReadPixels(0, 0, w, h, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

            // 上下反転
            FlipVerticallyInPlace(pixels, w, h);

            using (var bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var data = bmp.LockBits(new Rectangle(0, 0, w, h),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
                }
                finally
                {
                    bmp.UnlockBits(data);
                }
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }//マウスカーソルなしcontroll2Dのみ

        private void CaptureGLControlWithCursor(string filePath)//マウスカーソルありGLControl2Dのみ
        {

            var rect = glControl2D.RectangleToScreen(glControl2D.ClientRectangle);

            using (Bitmap bmp = new Bitmap(rect.Width, rect.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(rect.Location, Point.Empty, rect.Size);


                    var cursorPos = Cursor.Position;//! マウスカーソルのスクリーン座標
                    var relPos = new Point(cursorPos.X - rect.Left, cursorPos.Y - rect.Top);//フォーム内の座標？
                    Cursors.Cross.Draw(g, new Rectangle(relPos, Cursors.Cross.Size));//カーソルを描画
                }
                bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }


        public void SaveGLAndMapFormCombinedPng(string filePath)
        {
            // GLControl2Dの内容を取得

            glControl2D.Refresh();
            Application.DoEvents();

            Bitmap glBitmap = null;
            if (glControl2D != null && glControl2D.Width > 0 && glControl2D.Height > 0)
            {
                glControl2D.MakeCurrent();
                int w = glControl2D.Width;
                int h = glControl2D.Height;
                byte[] pixels = new byte[w * h * 4];
                GL.ReadPixels(0, 0, w, h, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

                // 上下反転
                FlipVerticallyInPlace(pixels, w, h);

                glBitmap = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var data = glBitmap.LockBits(new Rectangle(0, 0, w, h),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
                }
                finally
                {

                    glBitmap.UnlockBits(data);
                }
            }

            // MapFormの描画
            Bitmap mapBitmap = null;
            if (m_frmMap != null)
            {
                m_frmMap.Refresh();
                m_frmMap.Update();
                Application.DoEvents();

                mapBitmap = new Bitmap(m_frmMap.Width, m_frmMap.Height);
                m_frmMap.DrawToBitmap(mapBitmap, new Rectangle(0, 0, mapBitmap.Width, mapBitmap.Height));
            }

            // 合成（横並び）
            int totalW = (glBitmap?.Width ?? 0) + (mapBitmap?.Width ?? 0);
            int totalH = Math.Max(glBitmap?.Height ?? 0, mapBitmap?.Height ?? 0);

            using (var combined = new Bitmap(Math.Max(1, totalW), Math.Max(1, totalH)))


            using (var g = Graphics.FromImage(combined))
            {
                if (glBitmap != null)
                    g.DrawImage(glBitmap, 0, 0);
                if (mapBitmap != null)
                    g.DrawImage(mapBitmap, glBitmap?.Width ?? 0, 0);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                combined.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }

            glBitmap?.Dispose();
            mapBitmap?.Dispose();
        }//マウスカーソルなしcontroll2D+mapform(表、ドット付き地図？)

        #endregion
    }
}

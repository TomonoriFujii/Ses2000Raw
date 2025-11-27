using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Formats.Tar;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Ses2000Raw
{
    public partial class MainForm : DarkForm.BaseForm
    {
        // Block Header offsets
        private const int OFF_NUM_CHANNELS = 116;
        private const int OFF_LF_SAMPLES = 268;
        private const int OFF_HF_SAMPLES = 272;

        private MapForm m_frmMap;


        public MainForm()
        {
            InitializeComponent();
        }

        #region Event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.BackColor = Constant.BACKCOLOR;
            this.ForeColor = Constant.FORECOLOR;

            this.lblTitle.Text = "SES-Reflect";
            m_frmMap = new MapForm();
            m_frmMap.Show(this.dockPanel1, DockState.DockRight);

            ColorPalette.LoadColorTable();
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // AnalysisFormが表示されているか確認
            foreach (var content in this.dockPanel1.Contents)
            {
                if (content is AnalysisForm analysisForm)
                {
                    // m_CSVFilePathがnullの場合、警告を表示
                    if (string.IsNullOrEmpty(analysisForm.CSVFilePath))
                    {
                        var result = MessageBox.Show(
                            "CSVファイルが保存されていません。保存せずに終了しますか？",
                            "警告",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );
                        if (result == DialogResult.No)
                        {
                            e.Cancel = true; // 終了をキャンセル
                            return;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// [File] menu item clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsMenutemFile_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag == null) return;
            switch (e.ClickedItem.Tag.ToString())
            {
                case "Open":
                    OpenRawFile();
                    //btnBrowse_Click(sender, e);
                    break;
                case "Exit":
                    Application.Exit();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// Open Raw File
        /// </summary>


        public string? RawFileName { get; private set; }


        private void OpenRawFile()
        {
            string strRaw;
            if (Directory.Exists(Properties.Settings.Default.RawDir))
                openFileDialog1.InitialDirectory = Properties.Settings.Default.RawDir;

            openFileDialog1.Filter = "SES2000 Raw File (*.raw)|*.raw";
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            
            strRaw = openFileDialog1.FileName;

            RawFileName = strRaw;
            Properties.Settings.Default.RawDir = Path.GetDirectoryName(strRaw);
            Properties.Settings.Default.Save();

            FileHeader fileHeader;
            List<BlockHeader> blockHeaderList;
            List<DataBlock> dataBlockList;

            #region .RAW File Read
            bool bRet = ReadRaw(strRaw, out fileHeader, out blockHeaderList, out dataBlockList);
            if (!bRet) return;
            if (dataBlockList.Count == 0)
            {
                MessageBox.Show(this, "No data blocks found in the file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            List<string> angles = new List<string>();
            List<string> frequencys = new List<string>();
            
            // Angle
            if (fileHeader.BeamSteeringType == 0)
            {
                angles.Add("0");
            }
            else
            {
                // Beam Steering Mode
                for (int i = 0; i < fileHeader.NumBeams; i++)
                {
                    angles.Add(fileHeader.BeamSteeringAngles[i].ToString());
                }
            }

            // Frequency
            if (fileHeader.MultiFreqFile == 0)
            {
                frequencys.Add((blockHeaderList[0].Frequency1 / 1000).ToString());
            }
            else
            {
                // Multi Frequency
                blockHeaderList
                    .GroupBy(bh => bh.Frequency1)
                    .Select(g => g.OrderBy(bh => bh.MultiFreqId).First()) // 各Frequency1で最小のFrequencyIdを持つ要素を代表に
                    .OrderBy(bh => bh.MultiFreqId) // 全体をFrequencyId順にソート
                    .ToList()
                    .ForEach(bh => frequencys.Add((bh.Frequency1 / 1000).ToString()));
            }
            
            LoadParamForm paramForm = new LoadParamForm(angles, frequencys);
            if (paramForm.ShowDialog(this) != DialogResult.OK) return;

            //string strTitle = System.IO.Path.GetFileName(strRaw) + " [" + paramForm.ExtractionInfo.Channel.ToString() + ", " + paramForm.ExtractionInfo.Angle.ToString() + "°, " + paramForm.ExtractionInfo.Frequency.ToString() + "kHz]";
            StringBuilder sbTitle = new StringBuilder();
            sbTitle.Append(Path.GetFileName(strRaw));
            if( fileHeader.BeamSteeringType != 0)
            {
                sbTitle.Append(" [");
                sbTitle.Append(paramForm.ExtractionInfo.Angle.ToString());
                sbTitle.Append("°, ");
                sbTitle.Append(paramForm.ExtractionInfo.Frequency.ToString());
                sbTitle.Append("kHz]");
            }
            else
            {
                sbTitle.Append(" [");
                sbTitle.Append(paramForm.ExtractionInfo.Frequency.ToString());
                sbTitle.Append("kHz]");
            }

            AnalysisForm analysisForm = new AnalysisForm(sbTitle.ToString(), paramForm.ExtractionInfo.Channel,RawFileName);
            analysisForm.MapView = m_frmMap;
            analysisForm.FileHeader = (FileHeader)fileHeader.Clone();
            for (int i = 0; i < blockHeaderList.Count; i++)
            {
                if (fileHeader.BeamSteeringType != 0)
                {
                    // Beam Steering Mode
                    if (blockHeaderList[i].BeamIdForBS == (byte)paramForm.ExtractionInfo.BeamId)
                    {
                        analysisForm.BlockHeaderList.Add(((BlockHeader)blockHeaderList[i]).DeepClone());
                        analysisForm.DataBlockList.Add(((DataBlock)dataBlockList[i]).Clone());
                    }

                }
                else if (fileHeader.MultiFreqFile != 0)
                {
                    // Multi Frequency
                    if (blockHeaderList[i].Frequency1 == (paramForm.ExtractionInfo.Frequency * 1000))
                    {
                        analysisForm.BlockHeaderList.Add(((BlockHeader)blockHeaderList[i]).DeepClone());
                        analysisForm.DataBlockList.Add(((DataBlock)dataBlockList[i]).Clone());
                    }
                }
                else
                {
                    analysisForm.BlockHeaderList.Add(((BlockHeader)blockHeaderList[i]).DeepClone());
                    analysisForm.DataBlockList.Add(((DataBlock)dataBlockList[i]).Clone());
                }
                //analysisForm.BlockHeaderList.Add(((BlockHeader)blockHeaderList[i]).DeepClone());
                //analysisForm.DataBlockList.Add(((DataBlock)dataBlockList[i]).Clone());
            }
            analysisForm.Show(this.dockPanel1, DockState.Document);
            //analysisForm.Show(this.dockPanel1, DockState.DockLeft);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileHeader"></param>
        /// <param name="blockHeaderList"></param>
        /// <param name="dataBlockList"></param>
        /// <returns></returns>

        private bool ReadRaw(string path, out FileHeader fileHeader, out List<BlockHeader> blockHeaderList, out List<DataBlock> dataBlockList)
        {
            fileHeader = null;
            dataBlockList = new List<DataBlock>();
            blockHeaderList = new List<BlockHeader>();

            try
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    fileHeader = FileHeader.ReadFrom(fs);
                    int bhSize = fileHeader.BlockHeaderSize; // 320を想定
                    if (bhSize != 320)
                    {
                        MessageBox.Show(this, "Warning: BlockHeaderSize = " + bhSize + " (expected 320)", "Warning",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    int blockIndex = 0;
                    while (fs.Position + bhSize <= fs.Length)
                    {
                        BlockHeaderRaw raw = SesReader.ReadNextBlockHeaderRaw(fs, bhSize);
                        BlockHeader blockHeader = new BlockHeader(raw);

                        int numCh = raw.U8(OFF_NUM_CHANNELS);
                        int lfSamples = Math.Max(0, raw.I32(OFF_LF_SAMPLES));
                        int hfSamples = Math.Max(0, raw.I32(OFF_HF_SAMPLES));

                        DataBlock dataBlock = DataBlock.ReadFrom(fs, lfSamples, (numCh >= 2) ? hfSamples : 0);

                        blockHeaderList.Add(blockHeader);
                        dataBlockList.Add(dataBlock);
                        blockIndex++;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //AnalysisForm activeForm = this.dockPanel1.ActiveDocument as AnalysisForm;
            //if (activeForm != null)
            //{
            //    activeForm.label1.Text = "Clicked!";
            //}
            var active = this.dockPanel1.ActiveContent as DockContent;
            AnalysisForm activeForm = active as AnalysisForm;
            if (activeForm != null)
            {
                //activeForm.label22.Text = "Clicked!";
            }
        }









    }
}

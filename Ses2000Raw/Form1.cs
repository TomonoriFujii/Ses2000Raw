using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Threading.Channels;

namespace Ses2000Raw
{
    public partial class Form1 : Form
    {
        private FileHeader m_fileHeader;
        private List<BlockHeader> m_blockHeaderList;
        private List<DataBlock> m_dataBlockList;

        // プロパティ
        public FileHeader FileHeader { get { return m_fileHeader; } }
        public List<BlockHeader> BlockHeaderList { get { return m_blockHeaderList; } }
        public List<DataBlock> DataBlockList { get { return m_dataBlockList; } }

        public ExtractionInfo ExtractionInfo { get; set; } = new ExtractionInfo();


        // Block Header内のオフセット（仕様より）
        private const int OFF_NUM_CHANNELS = 116;   // unsigned char: data block channels (通常2)
        private const int OFF_LF_SAMPLES = 268;     // int (4B): LF – Data Length [samples]
        private const int OFF_HF_SAMPLES = 272;     // int (4B): HF – Data Length [samples]
        private const int BYTES_PER_SAMPLE = 2;     // 16bit

        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// [Browse]ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (File.Exists(this.txtBoxRaw.Text))
            {
                this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(this.txtBoxRaw.Text);
                this.openFileDialog1.FileName = Path.GetFileName(this.txtBoxRaw.Text);
            }
            this.openFileDialog1.Filter = "SES files (*.raw)|*.raw";
            if (this.openFileDialog1.ShowDialog(this) != DialogResult.OK) return;

            this.txtBoxRaw.Text = this.openFileDialog1.FileName;
        }
        /// <summary>
        /// [Output CSV]ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOutputCSV_Click(object sender, EventArgs e)
        {
            string path = this.txtBoxRaw.Text;
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                MessageBox.Show(this, "Rawファイルを指定してください。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool bRet = ReadRaw(path, out m_fileHeader, out m_blockHeaderList, out m_dataBlockList);
            if (!bRet) return;

            List<string> angles = new List<string>();
            List<string> frequencys = new List<string>();

            // Angle
            if (m_fileHeader.BeamSteeringType == 0)
            {
                angles.Add("0");
            }
            else
            {
                // Beam Steering Mode
                for (int i = 0; i < m_fileHeader.NumBeams; i++)
                {
                    angles.Add(m_fileHeader.BeamSteeringAngles[i].ToString());
                }
            }

            // Frequency
            if (m_fileHeader.MultiFreqFile == 0)
            {
                frequencys.Add((m_blockHeaderList[0].Frequency1 / 1000).ToString());
            }
            else
            {
                // Multi Frequency
                m_blockHeaderList
                    .GroupBy(bh => bh.Frequency1)
                    .Select(g => g.OrderBy(bh => bh.MultiFreqId).First()) // 各Frequency1で最小のFrequencyIdを持つ要素を代表に
                    .OrderBy(bh => bh.MultiFreqId) // 全体をFrequencyId順にソート
                    .ToList()
                    .ForEach(bh => frequencys.Add((bh.Frequency1 / 1000).ToString()));
            }

            LoadParamForm paramForm = new LoadParamForm(angles, frequencys);


            if (paramForm.ShowDialog(this) == DialogResult.OK)
            {
                string outPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)
                    + $"_{(ExtractionInfo.Channel == Channel.LF ? "LF" : "HF")}_{ExtractionInfo.Angle}deg_{ExtractionInfo.Frequency}Hz.csv");

                using (var writer = new StreamWriter(outPath))
                {
                    // CSV Header
                    //writer.WriteLine("Index,Date,Time,BeamId,MultiFreqId,Frequency1,Frequency2,LfSamples,HfSamples,LfData,HfData");

                    for (int i = 0; i < BlockHeaderList.Count; i++)
                    {
                        if (BlockHeaderList[i].BeamIdForBS == ExtractionInfo.BeamId &&
                            BlockHeaderList[i].MultiFreqId == ExtractionInfo.FreqId)
                        {
                            writer.Write($"{BlockHeaderList[i].Date},");
                            writer.Write($"{BlockHeaderList[i].Time},");
                            writer.Write($"{BlockHeaderList[i].SisString5},");  // X座標
                            writer.Write($"{BlockHeaderList[i].SisString6},");  // Y座標
                            writer.Write($"{BlockHeaderList[i].SisString4},");  // Heading
                            writer.Write($"{ExtractionInfo.Angle},");           // Angle
                            writer.Write($"{ExtractionInfo.Frequency},");       // kHz
                            if (ExtractionInfo.Channel == Channel.LF)
                            {
                                writer.WriteLine(string.Join(",", DataBlockList[i].Lf));
                            }
                            else
                            {
                                writer.WriteLine(string.Join(",", DataBlockList[i].Hf));
                            }
                        }
                    }
                }
                if (MessageBox.Show("CSVファイルの出力が完了しました。\nファイルを開きますか?", "完了", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo(outPath) { UseShellExecute = true });
                }
            }
        }

        /// <summary>
        /// SES Rawファイルの読み込み
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool ReadRaw(string path, out FileHeader fileHeader, out List<BlockHeader> blockHeaderList, out List<DataBlock> dataBlockList)
        {
            fileHeader = null;
            dataBlockList = new List<DataBlock>();
            blockHeaderList = new List<BlockHeader>();

            using (FileStream fs = File.OpenRead(path))
            {
                // File Headerの読み込み
                fileHeader = FileHeader.ReadFrom(fs);
                int bhSize = fileHeader.BlockHeaderSize; // 仕様が320のはず
                if (bhSize != 320)
                {
                    MessageBox.Show(this, "Warning: BlockHeaderSize = " + bhSize + " (expected 320)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                Console.WriteLine("=== SES Header ===");
                Console.WriteLine("SES ID        : " + fileHeader.SesId);
                Console.WriteLine("SubFormat     : " + fileHeader.SubFormat);
                Console.WriteLine("BlockHeaderSize: " + fileHeader.BlockHeaderSize);
                Console.WriteLine("Date / Time   : " + fileHeader.Date + " " + fileHeader.Time);
                Console.WriteLine("File Counter  : " + fileHeader.FileCounter);
                Console.WriteLine("Ship/Travel   : " + fileHeader.ShipName + " / " + fileHeader.TravelName);
                Console.WriteLine("Area          : " + fileHeader.AreaName);
                Console.WriteLine("SIS-Flag      : " + fileHeader.SisFlag);
                Console.WriteLine("SIS-Codes    : " + BitConverter.ToString(fileHeader.SisCodes));
                Console.WriteLine("MultiFreqFile  : " + fileHeader.MultiFreqFile);
                Console.WriteLine("MultiFreqMode  : " + fileHeader.MultiFreqMode);
                Console.WriteLine("BeamSteering   : " + fileHeader.BeamSteeringType);
                Console.WriteLine("NumBeams      : " + fileHeader.NumBeams);
                Console.WriteLine("Angles (deg) : " + string.Join(", ", fileHeader.BeamSteeringAngles));
                //var deg = header.GetBeamSteeringAnglesDegrees();
                //Console.WriteLine("Angles (deg)  : " + string.Join(", ", deg));
                Console.WriteLine("HighEnergy    : " + fileHeader.HighEnergyFile);
                Console.WriteLine("SideScanFile  : " + fileHeader.SideScanFile + " (mode=" + fileHeader.SideScanMode + ")");
                Console.WriteLine("Chirp         : " + fileHeader.ChirpMode);
                Console.WriteLine("Rewritten     : " + fileHeader.RewrittenFlag);


                int blockIndex = 0;
                while (fs.Position + bhSize <= fs.Length)
                {
                    // Block Headerの読み込み
                    BlockHeaderRaw raw = SesReader.ReadNextBlockHeaderRaw(fs, bhSize);
                    BlockHeader blockHeader = new BlockHeader(raw);

                    Console.WriteLine(blockHeader.SisString4); // Heading

                    // サンプル数取得（32bit）
                    int numCh = raw.U8(OFF_NUM_CHANNELS);
                    int lfSamples = raw.I32(OFF_LF_SAMPLES);
                    int hfSamples = raw.I32(OFF_HF_SAMPLES);
                    if (lfSamples < 0) lfSamples = 0;
                    if (hfSamples < 0) hfSamples = 0;

                    Console.WriteLine(
                    $"[{blockIndex}] {blockHeader.Date} {blockHeader.Time}  BSMode={blockHeader.BeamSteeringMode} BeamId={blockHeader.BeamIdForBS}  RealSteeredFanAngle(deg)={blockHeader.RealSteeredFanAngle}  " +
                    $"MFId={blockHeader.MultiFreqId} LfFreq={blockHeader.Frequency1} " +
                    $"LF={lfSamples} HF={hfSamples} ch={numCh}");

                    // DataBlockの読み込み（LF→HFの順）
                    DataBlock dataBlock = DataBlock.ReadFrom(fs, lfSamples, (numCh >= 2) ? hfSamples : 0);

                    //// ここで data.Lf / data.Hf を処理（例: 最初の数サンプルを表示）
                    //if (data.Lf.Length > 0)
                    //    Console.WriteLine($"    LF[0..4]: {Preview(data.Lf, 5)}");
                    //if (data.Hf.Length > 0)
                    //    Console.WriteLine($"    HF[0..4]: {Preview(data.Hf, 5)}");


                    BlockHeaderList.Add(blockHeader);
                    DataBlockList.Add(dataBlock);
                    blockIndex++;
                }
            }
            return true;
        }
    }
    
}

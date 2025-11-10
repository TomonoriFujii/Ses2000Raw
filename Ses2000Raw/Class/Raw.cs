using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ses2000Raw
{
    /// <summary>
    /// .RawファイルのFile Headerを保持するクラス
    /// </summary>
    public sealed class FileHeader
    {
        public string SesId { get; private set; } = "";       // 0-2   char[3] "SES"
        public ushort SubFormat { get; private set; }         // 3-4   ushort (LE)
        public ushort BlockHeaderSize { get; private set; }   // 5-6   ushort (LE)
        public ushort Reserved1 { get; private set; }         // 7-8   ushort (LE)
        public string Date { get; private set; } = "";        // 9-18  char[10]
        public byte Reserved2 { get; private set; }           // 19    0x00
        public string Time { get; private set; } = "";        // 20-27 char[8]
        public byte Reserved3 { get; private set; }           // 28    0x00
        public ushort FileCounter { get; private set; }       // 29-30 ushort (LE)
        public string ShipName { get; private set; } = "";    // 31-49 char[19]
        public byte Reserved4 { get; private set; }           // 50    0x00
        public string TravelName { get; private set; } = "";  // 51-69 char[19]
        public byte Reserved5 { get; private set; }           // 70    0x00
        public string AreaName { get; private set; } = "";    // 71-89 char[19]
        public byte Reserved6 { get; private set; }           // 90    0x00
        public ushort SisFlag { get; private set; }           // 91-92 ushort (LE)
        public byte[] SisCodes { get; private set; } = new byte[8]; // 93-100
        public byte MultiFreqFile { get; private set; }       // 101   1=on,0=off
        public byte MultiFreqMode { get; private set; }       // 102   例: 1;3=6/12 ...
        public byte BeamSteeringType { get; private set; }    // 103   1=BeamSteeringFile,2=FAN
        public byte NumBeams { get; private set; }            // 104   最大9
        public short[] BeamSteeringAngles { get; private set; } = new short[9];
        public byte HighEnergyFile { get; private set; }      // 123   1=on,0=off
        public byte SideScanFile { get; private set; }        // 124   1=on,0=off
        public byte SideScanMode { get; private set; }        // 125   0=right,1=left,2=both
        public byte ChirpMode { get; private set; }           // 126   1=on,0=off
        public byte RewrittenFlag { get; private set; }       // 127   1=processing applied

        public object Clone()
        {
            return new FileHeader
            {
                SubFormat = this.SubFormat,
                BlockHeaderSize = this.BlockHeaderSize,
                Reserved1 = this.Reserved1,
                Date = this.Date,
                Reserved2 = this.Reserved2,
                Time = this.Time,
                Reserved3 = this.Reserved3,
                FileCounter = this.FileCounter,
                ShipName = this.ShipName,
                Reserved4 = this.Reserved4,
                TravelName = this.TravelName,
                Reserved5 = this.Reserved5,
                AreaName = this.AreaName,
                Reserved6 = this.Reserved6,
                SisFlag = this.SisFlag,
                SisCodes = (byte[])this.SisCodes.Clone(),
                MultiFreqFile = this.MultiFreqFile,
                MultiFreqMode = this.MultiFreqMode,
                BeamSteeringType = this.BeamSteeringType,
                NumBeams = this.NumBeams,
                BeamSteeringAngles = (short[])this.BeamSteeringAngles.Clone(),
                HighEnergyFile = this.HighEnergyFile,
                SideScanFile = this.SideScanFile,
                SideScanMode = this.SideScanMode,
                ChirpMode = this.ChirpMode,
                RewrittenFlag = this.RewrittenFlag,
            };
        }

        /// 便宜プロパティ：ビーム角を度に換算（1カウント=0.5度）
        public double[] GetBeamSteeringAnglesDegrees()
        {
            var deg = new double[BeamSteeringAngles.Length];
            for (int i = 0; i < BeamSteeringAngles.Length; i++)
                deg[i] = BeamSteeringAngles[i] * 0.5;
            return deg;
        }

        // ===== 読み込み =====
        public static FileHeader ReadFrom(Stream stream)
        {
            var buf = new byte[128];
            int read = 0;
            while (read < buf.Length)
            {
                int r = stream.Read(buf, read, buf.Length - read);
                if (r <= 0) throw new EndOfStreamException("Unexpected EOF while reading 128-byte header.");
                read += r;
            }
            return Parse(buf);
        }

        public static FileHeader Parse(byte[] buf)
        {
            if (buf == null) throw new ArgumentNullException("buf");
            if (buf.Length != 128) throw new ArgumentException("Header must be exactly 128 bytes.", "buf");

            var h = new FileHeader();
            int pos = 0;

            // 0–103
            h.SesId = ReadAscii(buf, pos, 3); pos += 3;
            h.SubFormat = ReadUInt16LE(buf, pos); pos += 2;
            h.BlockHeaderSize = ReadUInt16LE(buf, pos); pos += 2;
            h.Reserved1 = ReadUInt16LE(buf, pos); pos += 2;
            h.Date = ReadAscii(buf, pos, 10); pos += 10;
            h.Reserved2 = buf[pos++];
            h.Time = ReadAscii(buf, pos, 8); pos += 8;
            h.Reserved3 = buf[pos++];
            h.FileCounter = ReadUInt16LE(buf, pos); pos += 2;
            h.ShipName = ReadAscii(buf, pos, 19); pos += 19;
            h.Reserved4 = buf[pos++];
            h.TravelName = ReadAscii(buf, pos, 19); pos += 19;
            h.Reserved5 = buf[pos++];
            h.AreaName = ReadAscii(buf, pos, 19); pos += 19;
            h.Reserved6 = buf[pos++];
            h.SisFlag = ReadUInt16LE(buf, pos); pos += 2;
            Array.Copy(buf, pos, h.SisCodes, 0, 8); pos += 8;
            h.MultiFreqFile = buf[pos++];
            h.MultiFreqMode = buf[pos++];
            h.BeamSteeringType = buf[pos++];

            // 104–127（今回の核心）
            h.NumBeams = buf[pos++]; // 104

            // 105–121: 9本 × 2バイト(short, LE)
            for (int i = 0; i < 9; i++)
            {
                short angle = ReadInt16LE(buf, pos);
                h.BeamSteeringAngles[i] = (short)(angle / 2);
                pos += 2;
            }

            h.HighEnergyFile = buf[pos++]; // 123
            h.SideScanFile = buf[pos++]; // 124
            h.SideScanMode = buf[pos++]; // 125
            h.ChirpMode = buf[pos++]; // 126
            h.RewrittenFlag = buf[pos++]; // 127

            // 念のため整合チェック
            if (pos != 128) throw new InvalidOperationException("Parsing error: position mismatch.");

            return h;
        }

        // ===== ユーティリティ =====
        private static ushort ReadUInt16LE(byte[] buf, int offset)
        {
            return (ushort)(buf[offset] | (buf[offset + 1] << 8));
        }

        private static short ReadInt16LE(byte[] buf, int offset)
        {
            return (short)(buf[offset] | (buf[offset + 1] << 8));
        }

        private static string ReadAscii(byte[] buf, int offset, int count)
        {
            // 固定長ASCII: 0x00まで＋末尾空白トリム
            int end = offset + count;
            int n = 0;
            for (int i = offset; i < end; i++)
            {
                if (buf[i] == 0) break;
                n++;
            }
            var s = Encoding.ASCII.GetString(buf, offset, n);
            return s.TrimEnd(' ');
        }
    }
    /// <summary>
    /// Block Headerを「生」で保持するクラス
    /// </summary>
    public sealed class BlockHeaderRaw : ICloneable
    {
        private readonly byte[] _buf;

        public int Length { get { return _buf.Length; } }

        public BlockHeaderRaw(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            _buf = bytes;
        }

        // ========= 基本ゲッター（Little Endian） =========
        public byte U8(int off) { return _buf[off]; }
        public sbyte I8(int off) { return unchecked((sbyte)_buf[off]); }
        public ushort U16(int off) { return (ushort)(_buf[off] | (_buf[off + 1] << 8)); }
        public short I16(int off) { return (short)(_buf[off] | (_buf[off + 1] << 8)); }
        public uint U32(int off) { return (uint)(_buf[off] | (_buf[off + 1] << 8) | (_buf[off + 2] << 16) | (_buf[off + 3] << 24)); }
        public int I32(int off) { return (int)(_buf[off] | (_buf[off + 1] << 8) | (_buf[off + 2] << 16) | (_buf[off + 3] << 24)); }

        public float F32(int off)
        {
            // Windows/x64はLEなのでOK
            return BitConverter.ToSingle(_buf, off);
        }
        public double F64(int off)
        {
            return BitConverter.ToDouble(_buf, off);
        }

        public string AsciiFixed(int off, int count)
        {
            // NULL(0x00)まで、末尾空白トリム
            int end = off + count;
            int n = 0;
            for (int i = off; i < end; i++)
            {
                if (_buf[i] == 0) break;
                n++;
            }
            var s = Encoding.ASCII.GetString(_buf, off, n);
            //return s.TrimEnd(' ');
            return s.TrimEnd(' ', ',');
        }

        public BlockHeaderRaw DeepClone()
        {
            var copy = new byte[_buf.Length];
            Buffer.BlockCopy(_buf, 0, copy, 0, _buf.Length);
            return new BlockHeaderRaw(copy);
        }

        object ICloneable.Clone() => DeepClone();
    }
    /// <summary>
    /// Block Headerの型付きラッパークラス
    /// </summary>
    public sealed partial class BlockHeader : ICloneable
    {
        private readonly BlockHeaderRaw m_bhRaw;
        private const int MinSize = 295; // 末尾フィールド終端
        public BlockHeader(BlockHeaderRaw raw)
        {
            if (raw == null) throw new ArgumentNullException("raw");
            m_bhRaw = raw;
            if (m_bhRaw.Length < MinSize) throw new ArgumentException("Block header too short (" + m_bhRaw.Length + ")");
        }

        // コピーコンストラクタ（private/ internal でも可）
        private BlockHeader(BlockHeader other)
            : this(other.m_bhRaw.DeepClone()) // ★中身を深コピーして再構築
        { }

        // ユーザー向け DeepClone
        public BlockHeader DeepClone() => new BlockHeader(this);

        // ICloneable 実装（必要なら）
        object ICloneable.Clone() => DeepClone();


        // ===== BlockHeaderの先頭からのオフセット位置 =====
        // Date  (char, 10B)
        private const int Off_Date = 0;
        // Time  (char, 8B)
        private const int Off_Time = 11;
        // SIS-String 1  (char, 12B)
        private const int Off_SisString1 = 20;
        // SIS-String 2  (char, 12B)
        private const int Off_SisString2 = 32;
        // SIS-String 3  (char, 12B)
        private const int Off_SisString3 = 44;
        // SIS-String 4  (char, 12B)
        private const int Off_SisString4 = 56;
        // SIS-String 5  (char, 12B)
        private const int Off_SisString5 = 68;
        // SIS-String 6  (char, 12B)
        private const int Off_SisString6 = 80;
        // SIS-String 7  (char, 12B)
        private const int Off_SisString7 = 92;
        // SIS-String 8  (char, 12B)
        private const int Off_SisString8 = 104;
        // Number of Channels in Data Block(default = 2)  (unsigned char, 1B)
        private const int Off_NumberOfChannels = 116;
        // Profile Number (1…999)  (unsigned short int, 2B)
        private const int Off_ProfileNumber = 117;
        // Marker Number (0=off, 1…999=MarkerOn)  (unsigned short int, 2B)
        private const int Off_MarkerNumber = 119;
        // Beam ID for Beam Steering or FAN file  (unsigned char, 1B)
        private const int Off_BeamIdForBS = 121;
        // Side Scan Block (0=right, 1=left)  (unsigned char, 1B)
        private const int Off_SideScanBlock = 122;
        // Measure Start [m] (Range)  (unsigned int, 4B)
        private const int Off_MeasureStart = 125;
        // Measure Length [m] (Range)  (unsigned int, 4B)
        private const int Off_MeasureLength = 129;
        // Multi Ping Mode Flag (0 = off, 1 = on, 2 =BurstMode)  (unsigned char, 1B)
        private const int Off_MultiPingModeFlag = 133;
        // DSP-Number  (unsigned char, 1B)
        private const int Off_DspNumber = 134;
        // actual FAN Angle [1/2°] (e.g. –22° == -44)  (short, 2B)
        private const int Off_ActualFanAngle = 135;
        // real steered FAN Angle [1/2°]  (short, 2B)
        private const int Off_RealSteeredFanAngle = 137;
        // Deep Water Gain Flag  (unsigned char, 1B)
        private const int Off_DeepWaterGainFlag = 139;
        // Maximum Value of LF-Channel  (unsigned short int, 2B)
        private const int Off_MaximumValueOfLfChannel = 140;
        // Maximum Value of HF-Channel  (unsigned short int, 2B)
        private const int Off_MaximumValueOfHfChannel = 142;
        // Position of Maximum of LF-Channel  (unsigned int, 4B)
        private const int Off_PositionOfMaximumOfLfChannel = 144;
        // Position of Maximum of LF-Channel  (unsigned int, 4B)
        private const int Off_PositionOfMaximumOfHfChannel = 148;
        // Calculated Slope AnglePitch (not all models)  (float, 4B)
        private const int Off_CalculatedSlopeAnglepitch = 158;
        // Calculated Slope AngleRoll (not all models)  (float, 4B)
        private const int Off_CalculatedSlopeAngleroll = 162;
        // DSP-Software-Release  (unsigned char, 1B)
        private const int Off_DspSoftwareRelease = 166;
        // Used EPSG Code  (int, 4B)
        private const int Off_UsedEpsgCode = 167;
        // Software Build Number  (int, 4B)
        private const int Off_SoftwareBuildNumber = 171;
        // PreGain Amplifier (model AR only) [dB]  (unsigned char, 1B)
        private const int Off_PregainAmplifier = 175;
        // Transducer Depth from Pressure Sensor [m]  (float, 4B)
        private const int Off_TransducerDepthFromPressureSensor = 176;
        // Multi Ping Mode Counter (not all models)  (unsigned char, 1B)
        private const int Off_MultiPingModeCounter = 180;
        // Heading Angle from Motion Sensor [1/10°] (SubFormat<14) / [1 / 50°] (SubFormat>=14)  (short, 2B)
        private const int Off_HeadingAngleFromMotionSensor = 181;
        // Roll Angle from Motion Sensor [1 / 10°] (SubFormat<14) / [1 / 100°] (SubFormat>=14)  (short, 2B)
        private const int Off_RollAngleFromMotionSensor = 183;
        // Pitch Angle from Motion Sensor [1 / 10°] (SubFormat<14) / [1 / 100°] (SubFormat>=14)  (short, 2B)
        private const int Off_PitchAngleFromMotionSensor = 185;
        // Yaw Angle from Motion Sensor [1 / 10°] (SubFormat<14) / [1 / 100°] (SubFormat>=14)  (short, 2B)
        private const int Off_YawAngleFromMotionSensor = 187;
        // Transducer Depth [cm]  (unsigned short int, 2B)
        private const int Off_TransducerDepth = 189;
        // Heave from Motion Sensor [mm]  (short, 2B)
        private const int Off_HeaveFromMotionSensor = 191;
        // Beam Steering Mode  (unsigned char, 1B)
        private const int Off_BeamSteeringMode = 193;
        // Steering Roll Angle [1/2°]  (short, 2B)
        private const int Off_SteeringRollAngle = 194;
        // Steering Pitch Angle [1/2°]  (short, 2B)
        private const int Off_SteeringPitchAngle = 196;
        // Data Compression Rate before Recording  (unsigned char, 1B)
        private const int Off_DataCompressionRateBeforeRecording = 198;
        // Half Power Beam Width [1/10°]  (unsigned char, 1B)
        private const int Off_HalfPowerBeamWidth = 199;
        // External Trigger On/Off [1 = On]  (unsigned char, 1B)
        private const int Off_ExternalTriggerOnOff = 200;
        // Frequency ID for Multi Frequency  (unsigned char, 1B)
        private const int Off_FrequencyIdForMultiFrequency = 201;
        // Transducer ID in Multi Transducer Systems  (unsigned char, 1B)
        private const int Off_TransducerIdInMultiTransducerSystems = 202;
        // Frequency1 [Hz]  (unsigned short int, 2B)
        private const int Off_Frequency1 = 203;
        // Frequency2 [Hz] (actually=0x00)  (unsigned short int, 2B)
        private const int Off_Frequency2 = 205;
        // Pulses1  (unsigned short int, 2B)
        private const int Off_Pulses1 = 207;
        // Pulses2 (actually=0x00)  (unsigned short int, 2B)
        private const int Off_Pulses2 = 209;
        // Pulse to Pulse Distance [m]  (float, 4B)
        private const int Off_PulseToPulseDistance = 211;
        // High Energy Pulses  (unsigned char, 1B)
        private const int Off_HighEnergyPulses = 215;
        // Side Scan Pulse Length  (unsigned short int, 2B)
        private const int Off_SideScanPulseLength = 216;
        // HF Frequency1 [Hz]  (unsigned int, 4B)
        private const int Off_HfFrequency1 = 218;
        // Gain Value of LF-Channel [dB]  (short, 2B)
        private const int Off_GainValueOfLfChannel = 222;
        // Gain Value of HF-Channel [dB]  (short, 2B)
        private const int Off_GainValueOfHfChannel = 224;
        // Code for Amplifier A (LF) (actually=0x00)  (unsigned short int, 2B)
        private const int Off_CodeForAmplifierA = 226;
        // Code for Amplifier B (LF) (actually=0x00)  (unsigned short int, 2B)
        private const int Off_CodeForAmplifierB = 228;
        // Code for Amplifier C (LF) (actually=0x00)  (unsigned short int, 2B)
        private const int Off_CodeForAmplifierC = 230;
        // Code for Amplifier A (HF) (actually=0x00)  (unsigned short int, 2B)
        private const int Off_CodeForAmplifierAHf = 232;
        // Code for Amplifier B (HF) (actually=0x00)  (unsigned short int, 2B)
        private const int Off_CodeForAmplifierBHf = 234;
        // Heave - Motion Sensor, not processed [mm]  (short, 2B)
        private const int Off_HeaveMotionSensorNotProcessed = 236;
        // Sound Velocity [m/s]  (unsigned short int, 2B)
        private const int Off_SoundVelocity = 238;
        // Pulse Length [μs]  (unsigned short int, 2B)
        private const int Off_PulseLength = 240;
        // Time Before Pulse [m] (actually=0x00)  (float, 4B)
        private const int Off_TimeBeforePulse = 242;
        // Time After Measure [m] (actually=0x00)  (unsigned short int, 2B)
        private const int Off_TimeAfterMeasure = 246;
        // Start Sample for Down Mute Processing  (unsigned int, 4B)
        private const int Off_StartSampleForDownMuteProcessing = 248;
        // LF Signal Source (0=Transducer, 1=Analogue Input, 2 = AR-Receiver)  (unsigned char, 1B)
        private const int Off_LfSignalSource = 252;
        // Data Block Counter (actually=0x00)  (unsigned short int, 2B)
        private const int Off_DataBlockCounter = 253;
        // Transmitter Status (0=off, 1=on)  (unsigned char, 1B)
        private const int Off_TransmitterStatus = 255;
        // Sample Frequency for LF [Hz]  (unsigned int, 4B)
        private const int Off_SampleFrequencyForLf = 256;
        // ZF from HF Frequency (e.g. 14924 Hz) [Hz]  (unsigned short int, 2B)
        private const int Off_ZfFromHfFrequency = 260;
        // LF-Frequency [Hz] – MultiFreqMode 1 or Single Frequency  (unsigned short int, 2B)
        private const int Off_LfFrequencyMultifreqmode1OrSingleFrequency = 262;
        // LF-Frequency [Hz] – MultiFreqMode 2  (unsigned short int, 2B)
        private const int Off_LfFrequencyMultifreqmode2 = 264;
        // LF-Frequency [Hz] – MultiFreqMode 3  (unsigned short int, 2B)
        private const int Off_LfFrequencyMultifreqmode3 = 266;
        // LF – Data Length [samples]  (int, 4B)
        private const int Off_LfDataLength = 268;
        // HF – Data Length [samples]  (int, 4B)
        private const int Off_HfDataLength = 272;
        // Output Power (1 to 7, since Nov2017)  (char, 1B)
        private const int Off_OutputPower = 286;

        // ===== Accessors =====
        public string Date { get { return m_bhRaw.AsciiFixed(Off_Date, 10); } }
        public string Time { get { return m_bhRaw.AsciiFixed(Off_Time, 8); } }
        public string SisString1 { get { return m_bhRaw.AsciiFixed(Off_SisString1, 12); } }
        public string SisString2 { get { return m_bhRaw.AsciiFixed(Off_SisString2, 12); } }
        public string SisString3 { get { return m_bhRaw.AsciiFixed(Off_SisString3, 12); } }
        public string SisString4 { get { return m_bhRaw.AsciiFixed(Off_SisString4, 12); } }
        public string SisString5 { get { return m_bhRaw.AsciiFixed(Off_SisString5, 12); } }
        public string SisString6 { get { return m_bhRaw.AsciiFixed(Off_SisString6, 12); } }
        public string SisString7 { get { return m_bhRaw.AsciiFixed(Off_SisString7, 12); } }
        public string SisString8 { get { return m_bhRaw.AsciiFixed(Off_SisString8, 12); } }
        public byte NumberOfChannels { get { return m_bhRaw.U8(Off_NumberOfChannels); } }
        public ushort ProfileNumber { get { return m_bhRaw.U16(Off_ProfileNumber); } }
        public ushort MarkerNumber { get { return m_bhRaw.U16(Off_MarkerNumber); } }
        public byte BeamIdForBS { get { return m_bhRaw.U8(Off_BeamIdForBS); } }
        public byte SideScanBlock { get { return m_bhRaw.U8(Off_SideScanBlock); } }
        public uint MeasureStart { get { return m_bhRaw.U32(Off_MeasureStart); } }
        public uint MeasureLength { get { return m_bhRaw.U32(Off_MeasureLength); } }
        public byte MultiPingModeFlag { get { return m_bhRaw.U8(Off_MultiPingModeFlag); } }
        public byte DspNumber { get { return m_bhRaw.U8(Off_DspNumber); } }
        public short ActualFanAngle { get { return (short)(m_bhRaw.I16(Off_ActualFanAngle) / 2); } }
        public short RealSteeredFanAngle { get { return (short)(m_bhRaw.I16(Off_RealSteeredFanAngle) / 2); } }
        public byte DeepWaterGainFlag { get { return m_bhRaw.U8(Off_DeepWaterGainFlag); } }
        public ushort MaxValueOfLf { get { return m_bhRaw.U16(Off_MaximumValueOfLfChannel); } }
        public ushort MaxValueOfHf { get { return m_bhRaw.U16(Off_MaximumValueOfHfChannel); } }
        public uint PositionOfMaxOfLf { get { return m_bhRaw.U32(Off_PositionOfMaximumOfLfChannel); } }
        public uint PositionOfMaxOfHf { get { return m_bhRaw.U32(Off_PositionOfMaximumOfHfChannel); } }
        public float SlopeAnglePitch { get { return m_bhRaw.F32(Off_CalculatedSlopeAnglepitch); } }
        public float SlopeAngleRoll { get { return m_bhRaw.F32(Off_CalculatedSlopeAngleroll); } }
        public byte DspSoftwareRelease { get { return m_bhRaw.U8(Off_DspSoftwareRelease); } }
        public int UsedEpsgCode { get { return m_bhRaw.I32(Off_UsedEpsgCode); } }
        public int SoftwareBuildNumber { get { return m_bhRaw.I32(Off_SoftwareBuildNumber); } }
        public byte PreGainAmplifier { get { return m_bhRaw.U8(Off_PregainAmplifier); } }
        public float TransducerDepthFromPressureSensor { get { return m_bhRaw.F32(Off_TransducerDepthFromPressureSensor); } }
        public byte MultiPingModeCounter { get { return m_bhRaw.U8(Off_MultiPingModeCounter); } }
        public short HeadingAngleFromMotionSensor { get { return m_bhRaw.I16(Off_HeadingAngleFromMotionSensor); } }
        public short RollAngleFromMotionSensor { get { return m_bhRaw.I16(Off_RollAngleFromMotionSensor); } }
        public short PitchAngleFromMotionSensor { get { return m_bhRaw.I16(Off_PitchAngleFromMotionSensor); } }
        public short YawAngleFromMotionSensor { get { return m_bhRaw.I16(Off_YawAngleFromMotionSensor); } }
        public ushort TransducerDepth { get { return m_bhRaw.U16(Off_TransducerDepth); } }
        public short HeaveFromMotionSensor { get { return m_bhRaw.I16(Off_HeaveFromMotionSensor); } }
        public byte BeamSteeringMode { get { return m_bhRaw.U8(Off_BeamSteeringMode); } }
        public short SteeringRollAngle { get { return (short)(m_bhRaw.I16(Off_SteeringRollAngle) / 2); } }
        public short SteeringPitchAngle { get { return (short)(m_bhRaw.I16(Off_SteeringPitchAngle) / 2); } }
        public byte DataCompressionRateBeforeRecording { get { return m_bhRaw.U8(Off_DataCompressionRateBeforeRecording); } }
        public byte HalfPowerBeamWidth { get { return (byte)(m_bhRaw.U8(Off_HalfPowerBeamWidth) / 10); } }
        public byte ExternalTriggerOnOff { get { return m_bhRaw.U8(Off_ExternalTriggerOnOff); } }
        public byte MultiFreqId { get { return m_bhRaw.U8(Off_FrequencyIdForMultiFrequency); } }
        public byte TransducerIdInMultiTransducerSystems { get { return m_bhRaw.U8(Off_TransducerIdInMultiTransducerSystems); } }
        public ushort Frequency1 { get { return m_bhRaw.U16(Off_Frequency1); } }
        public ushort Frequency2 { get { return m_bhRaw.U16(Off_Frequency2); } }
        public ushort Pulses1 { get { return m_bhRaw.U16(Off_Pulses1); } }
        public ushort Pulses2 { get { return m_bhRaw.U16(Off_Pulses2); } }
        public float PulseToPulseDistance { get { return m_bhRaw.F32(Off_PulseToPulseDistance); } }
        public byte HighEnergyPulses { get { return m_bhRaw.U8(Off_HighEnergyPulses); } }
        public ushort SideScanPulseLength { get { return m_bhRaw.U16(Off_SideScanPulseLength); } }
        public uint HfFrequency1 { get { return m_bhRaw.U32(Off_HfFrequency1); } }
        public short GainValueOfLf { get { return m_bhRaw.I16(Off_GainValueOfLfChannel); } }
        public short GainValueOfHf { get { return m_bhRaw.I16(Off_GainValueOfHfChannel); } }
        public ushort CodeForAmplifierALf { get { return m_bhRaw.U16(Off_CodeForAmplifierA); } }
        public ushort CodeForAmplifierBLf { get { return m_bhRaw.U16(Off_CodeForAmplifierB); } }
        public ushort CodeForAmplifierCLf { get { return m_bhRaw.U16(Off_CodeForAmplifierC); } }
        public ushort CodeForAmplifierAHf { get { return m_bhRaw.U16(Off_CodeForAmplifierAHf); } }
        public ushort CodeForAmplifierBHf { get { return m_bhRaw.U16(Off_CodeForAmplifierBHf); } }
        public short HeaveMotionSensorNotProcessed { get { return m_bhRaw.I16(Off_HeaveMotionSensorNotProcessed); } }
        public ushort SoundVelocity { get { return m_bhRaw.U16(Off_SoundVelocity); } }
        public ushort PulseLength { get { return m_bhRaw.U16(Off_PulseLength); } }
        public float TimeBeforePulse { get { return m_bhRaw.F32(Off_TimeBeforePulse); } }
        public ushort TimeAfterMeasure { get { return m_bhRaw.U16(Off_TimeAfterMeasure); } }
        public uint StartSampleForDownMuteProcessing { get { return m_bhRaw.U32(Off_StartSampleForDownMuteProcessing); } }
        public byte LfSignalSource { get { return m_bhRaw.U8(Off_LfSignalSource); } }
        public ushort DataBlockCounter { get { return m_bhRaw.U16(Off_DataBlockCounter); } }
        public byte TransmitterStatus { get { return m_bhRaw.U8(Off_TransmitterStatus); } }
        public uint SampleFrequencyForLf { get { return m_bhRaw.U32(Off_SampleFrequencyForLf); } }
        public ushort ZfFromHfFrequency { get { return m_bhRaw.U16(Off_ZfFromHfFrequency); } }
        public ushort LfFrequency1 { get { return m_bhRaw.U16(Off_LfFrequencyMultifreqmode1OrSingleFrequency); } }
        public ushort LfFrequency2 { get { return m_bhRaw.U16(Off_LfFrequencyMultifreqmode2); } }
        public ushort LfFrequency3 { get { return m_bhRaw.U16(Off_LfFrequencyMultifreqmode3); } }
        public int LfDataLength { get { return m_bhRaw.I32(Off_LfDataLength); } }
        public int HfDataLength { get { return m_bhRaw.I32(Off_HfDataLength); } }
        public string OutputPower { get { return m_bhRaw.AsciiFixed(Off_OutputPower, 1); } }
    }
    /// <summary>
    /// ストリームから順にブロックヘッダを読む
    /// </summary>
    public static class SesReader
    {
        public static BlockHeaderRaw ReadNextBlockHeaderRaw(Stream s, int blockHeaderSize)
        {
            if (s == null) throw new ArgumentNullException("s");
            var buf = new byte[blockHeaderSize];
            int read = 0;
            while (read < buf.Length)
            {
                int r = s.Read(buf, read, buf.Length - read);
                if (r <= 0) throw new EndOfStreamException("Unexpected EOF while reading block header.");
                read += r;
            }
            return new BlockHeaderRaw(buf);
        }
    }
    /// <summary>
    /// Data Block読み取りクラス
    /// </summary>
    public sealed class DataBlock : ICloneable
    {
        public short[] Lf { get; private set; } = new short[0];
        public short[] Hf { get; private set; } = new short[0];
        public short[] Processed { get; private set; } = new short[0];

        public static DataBlock ReadFrom(Stream s, int lfSamples, int hfSamples)
        {
            if (lfSamples < 0) lfSamples = 0;
            if (hfSamples < 0) hfSamples = 0;

            long need = (long)lfSamples * 2 + (long)hfSamples * 2;
            if (s.CanSeek)
            {
                long remain = s.Length - s.Position;
                if (need > remain)
                    throw new EndOfStreamException(
                        "Payload size exceeds remaining stream. " +
                        $"need={need}, remain={remain}, lf={lfSamples}, hf={hfSamples}");
            }

            var db = new DataBlock();
            db.Lf = ReadInt16ArrayLE(s, lfSamples);
            db.Hf = ReadInt16ArrayLE(s, hfSamples);
            return db;
        }

        private static short[] ReadInt16ArrayLE(Stream s, int count)
        {
            if (count <= 0) return new short[0];

            int bytes = checked(count * 2);
            var buf = new byte[bytes];
            ReadExact(s, buf, 0, bytes);

            var arr = new short[count];
            int j = 0;
            for (int i = 0; i < count; i++, j += 2)
            {
                arr[i] = (short)(buf[j] | (buf[j + 1] << 8));
            }
            return arr;
        }

        private static void ReadExact(Stream s, byte[] b, int off, int cnt)
        {
            int read = 0;
            while (read < cnt)
            {
                int r = s.Read(b, off + read, cnt - read);
                if (r <= 0) throw new EndOfStreamException("Unexpected EOF while reading payload.");
                read += r;
            }
        }

        public void SetProcessesd(short[] data, bool clone = false)
        {
            Processed = clone ? (short[])data?.Clone() : data;
        }
        // ====== Clone メソッド ======
        public DataBlock Clone()
        {
            return new DataBlock
            {
                Lf = (short[])this.Lf.Clone(),
                Hf = (short[])this.Hf.Clone(),
                Processed = (short[])this.Processed.Clone()
            };
        }

        object ICloneable.Clone() => Clone();



    }

}

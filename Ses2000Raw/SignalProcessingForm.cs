using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ses2000Raw
{
    public partial class SignalProcessingForm : Form
    {
        AnalysisForm m_frmParent = null;

        public SignalProcessingForm(AnalysisForm parent, double samplingFreqHz)
        {
            m_frmParent = parent;

            InitializeComponent();

            int iMaxFreq = (int)((samplingFreqHz / 2) / 1000);

            this.trackBarHpf.Maximum = this.trackBarLpf.Maximum = iMaxFreq;
            this.lblHpfMax.Text = this.lblLpfMax.Text = iMaxFreq.ToString();
        }
        /// <summary>
        /// Form Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SignalProcessingForm_Load(object sender, EventArgs e)
        {
            this.cmbDemodulate.SelectedIndex = (int)m_frmParent.DemodulateMode;
            this.chkBpf.Checked = m_frmParent.ApplyBpf;
            this.chkBpf_CheckedChanged(this.chkBpf, EventArgs.Empty);
            this.trackBarHpf.Value = m_frmParent.Hpf_kHz;
            this.trackBarLpf.Value = m_frmParent.Lpf_kHz;
            this.lblHpf.Text = this.trackBarHpf.Value.ToString();
            this.lblLpf.Text = this.trackBarLpf.Value.ToString();
        }
        /// <summary>
        /// chkBpf Checked Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBpf_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBox1.Enabled = this.chkBpf.Checked;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarBpf_Scroll(object sender, EventArgs e)
        {
            if (((TrackBar)sender).Tag == null) return;

            switch (((TrackBar)sender).Tag.ToString())
            {
                case "Hpf":
                    this.lblHpf.Text = trackBarHpf.Value.ToString();
                    break;
                case "Lpf":
                    this.lblLpf.Text = trackBarLpf.Value.ToString();
                    break;
            }

        }
        /// <summary>
        /// 「OK」ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                bool bEnvelope = (this.cmbDemodulate.SelectedIndex == (int)DemodulationMode.Envelope ||
                                  this.cmbDemodulate.SelectedIndex == (int)DemodulationMode.DeconvoEnvelope) ? true : false;
                bool bDeconvo = (this.cmbDemodulate.SelectedIndex == (int)DemodulationMode.Deconvolution ||
                                  this.cmbDemodulate.SelectedIndex == (int)DemodulationMode.DeconvoEnvelope) ? true : false;
                bool bBpf = this.chkBpf.Checked;

                if(this.cmbDemodulate.SelectedIndex == (int)DemodulationMode.None && !bBpf)
                {
                    m_frmParent.DemodulateMode = (DemodulationMode)this.cmbDemodulate.SelectedIndex;
                    m_frmParent.ApplyBpf = bBpf;
                    m_frmParent.Hpf_kHz = this.trackBarHpf.Value;
                    m_frmParent.Lpf_kHz = this.trackBarLpf.Value;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }

                // 入力取得（単位は一貫して Hz/kHz）
                int fsHz = (int)m_frmParent.BlockHeaderList[0].SampleFrequencyForLf; // Hz
                int hpf_kHz = this.trackBarHpf.Value;
                int lpf_kHz = this.trackBarLpf.Value;

                // BPFの妥当性チェック
                if (bBpf)
                {
                    if (hpf_kHz < 0 || lpf_kHz <= 0 || hpf_kHz >= lpf_kHz)
                        throw new ArgumentException("Band-pass設定が不正です（HPF < LPF を満たす必要があります）。");
                    if (lpf_kHz * 1000.0 > fsHz / 2.0)
                        throw new ArgumentException("LPFがナイキスト周波数を超えています。LPF を下げてください。");
                }

                // 進捗UI
                using var frmProgress = new ProgressForm();
                frmProgress.SetDiscription("Processing...");
                frmProgress.SetProgressValue(0);
                frmProgress.SetMaxValue(m_frmParent.DataBlockList.Count);
                frmProgress.Show(this);

                // IProgress は UI スレッドに自動マーシャルされる
                var progress = new Progress<int>(v => frmProgress.SetProgressValue(v));

                // 非同期実行
                var ret = await Task.Run(() =>
                    DataClass.SignalProcessParallel(
                        /*context:*/ null,         // ← 不要
                        progress,
                        m_frmParent.DataBlockList,
                        fsHz,
                        bBpf,
                        hpf_kHz,
                        lpf_kHz,
                        bEnvelope,
                        bDeconvo
                    )
                ).ConfigureAwait(true); // WinFormsなのでUIに戻す

                // 結果処理
                if (ret.RetCode == ResultCode.Ok)
                {
                    var proc = ret.Obj as short[][];
                    if (proc == null) throw new InvalidOperationException("内部エラー: 戻り値の型が short[][] ではありません。");

                    int n = Math.Min(m_frmParent.DataBlockList.Count, proc.Length);
                    for (int i = 0; i < n; i++)
                        m_frmParent.DataBlockList[i].SetProcessesd(proc[i], clone: true);
                }
                else if (ret.RetCode == ResultCode.Cancel)
                {
                    // ユーザーキャンセル
                    return;
                }
                else // Error
                {
                    throw new InvalidOperationException(ret.Message);
                }

#if false
                // 重いのでDEBUGのみ
                try
                {
                    var sb = new StringBuilder();
                    var db0 = m_frmParent.DataBlockList[0];
                    int len = Math.Min(db0.Lf.Length, db0.Processed.Length);
                    for (int i = 0; i < len; i++)
                    {
                        sb.Append(db0.Lf[i]).Append(", ").Append(db0.Processed[i]).Append('\n');
                    }
                    Clipboard.SetText(sb.ToString());
                }
                catch { /* ignore clipboard errors in debug */ }
#endif

                // 最後にUI側設定を確定
                m_frmParent.DemodulateMode = (DemodulationMode)this.cmbDemodulate.SelectedIndex;
                m_frmParent.ApplyBpf = bBpf;
                m_frmParent.Hpf_kHz = hpf_kHz;
                m_frmParent.Lpf_kHz = lpf_kHz;

                this.DialogResult = DialogResult.OK;
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}

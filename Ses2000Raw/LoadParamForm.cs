using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ses2000Raw
{
    public partial class LoadParamForm : Form
    {
        public ExtractionInfo ExtractionInfo { get; set; } = new ExtractionInfo();

        public LoadParamForm(List<string> angles, List<string> frequencys)
        {
            InitializeComponent();

            this.cmbBoxAngle.Items.Clear();
            this.cmbBoxFreq.Items.Clear();

            this.cmbBoxAngle.Items.AddRange(angles.ToArray());
            this.cmbBoxFreq.Items.AddRange(frequencys.ToArray());
        }
        /// <summary>
        /// LoadParamFormロード時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadParamForm_Load(object sender, EventArgs e)
        {
            cmbBoxCH.SelectedIndex = 0;
            cmbBoxAngle.SelectedIndex = 0;
            cmbBoxFreq.SelectedIndex = 0;
        }
        /// <summary>
        /// [OK]ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cmbBoxCH.SelectedIndex < 0 || cmbBoxAngle.SelectedIndex < 0 || cmbBoxFreq.SelectedIndex < 0)
            {
                MessageBox.Show(this, "パラメータを選択してください。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ExtractionInfo.Channel = (Channel)cmbBoxCH.SelectedIndex;
            ExtractionInfo.BeamId = cmbBoxAngle.SelectedIndex;
            ExtractionInfo.Angle = int.Parse(cmbBoxAngle.SelectedItem.ToString());
            ExtractionInfo.FreqId = cmbBoxFreq.SelectedIndex;
            ExtractionInfo.Frequency = int.Parse(cmbBoxFreq.SelectedItem.ToString()); // kHz
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        /// <summary>
        /// [Cancel]ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

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
    public partial class ScaleSettingForm : Form
    {
        AnalysisForm m_frmAnalysis;

        public ScaleSettingForm(AnalysisForm form)
        {
            InitializeComponent();

            m_frmAnalysis = form;
        }
        /// <summary>
        /// Form Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScaleSettingForm_Load(object sender, EventArgs e)
        {
            this.numDistScaleInterval.Value = (decimal)m_frmAnalysis.DistScaleIntervalM;
            this.numDepScaleInterval.Value = (decimal)m_frmAnalysis.DepScaleIntervalM;
            this.lblDistScaleColor.BackColor = m_frmAnalysis.DistScaleColor;
            this.lblDepScaleColor.BackColor = m_frmAnalysis.DepScaleColor;
        }

        private void lblScaleColor_Click(object sender, EventArgs e)
        {
            if (((Label)sender).Tag == null) return;

            string strTag = ((Label)sender).Tag.ToString();
            switch (strTag)
            {
                case "DistScaleColor":
                    colorDialog1.Color = lblDistScaleColor.BackColor;
                    lblDistScaleColor.BackColor = colorDialog1.ShowDialog() == DialogResult.OK ? colorDialog1.Color : lblDistScaleColor.BackColor;
                    break;
                case "DepScaleColor":
                    colorDialog1.Color = lblDepScaleColor.BackColor;
                    lblDepScaleColor.BackColor = colorDialog1.ShowDialog() == DialogResult.OK ? colorDialog1.Color : lblDepScaleColor.BackColor;
                    break;
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            m_frmAnalysis.DistScaleIntervalM = (double)this.numDistScaleInterval.Value;
            m_frmAnalysis.DepScaleIntervalM = (double)this.numDepScaleInterval.Value;
            m_frmAnalysis.DistScaleColor = this.lblDistScaleColor.BackColor;
            m_frmAnalysis.DepScaleColor = this.lblDepScaleColor.BackColor;
        }
    }
}

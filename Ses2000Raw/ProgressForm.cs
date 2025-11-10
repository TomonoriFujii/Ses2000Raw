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
    public partial class ProgressForm : Form
    {
        public static bool Cancel { get; set; }

        public ProgressForm()
        {
            InitializeComponent();
            this.progressBar.Style = ProgressBarStyle.Blocks;
        }
        //public FormProgress(ProgressBarStyle style)
        //{
        //    InitializeComponent();
        //    this.progressBar.Style = style;

        //    if(this.progressBar.Style == ProgressBarStyle.Marquee)
        //    {
        //        //Cancelボタンを非表示
        //        this.btnCancel.Visible = false;
        //        this.Size = new System.Drawing.Size(329, 140);
        //    }
        //}
        /// <summary>
        /// Form Loadイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmProgress_Load(object sender, EventArgs e)
        {
            Cancel = false;
            this.progressBar.Value = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormProgress_Shown(object sender, EventArgs e)
        {
            this.Activate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormProgress_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        /// <summary>
        /// 「キャンセル」ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancel = true;
        }


        #region 公開メソッド
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prg"></param>
        public void SetProgressValue(int value)
        {
            // Unit
            if (this.progressBar.Value != value)
            {
                this.progressBar.Value = value;
                this.progressBar.Update();
            }
            //this.label1.Text = value + "/" + this.progressBar.Maximum;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxValue"></param>
        public void SetMaxValue(int maxValue)
        {
            this.progressBar.Maximum = maxValue;
        }
        /// <summary>
        /// プログレスバーの説明文を設定します。
        /// </summary>
        /// <param name="unitNum"></param>
        public void SetDiscription(string discription)
        {
            this.labelDiscritption.Text = discription;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        public void SetProgressStyle(ProgressBarStyle style)
        {
            this.progressBar.Style = style;
            if (this.progressBar.Style == ProgressBarStyle.Marquee)
            {
                //Cancelボタンを非表示
                this.btnCancel.Visible = false;
                //this.Size = new System.Drawing.Size(329, 140);
            }
            else
            {
                this.btnCancel.Visible = true;
            }
        }
        #endregion

    }
}

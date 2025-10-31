namespace DarkForm
{
    partial class BaseForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseForm));
            imageList1 = new ImageList(components);
            titleBar = new TitleStrip();
            lblIcon = new ToolStripLabel();
            lblTitle = new ToolStripLabel();
            btnCloseForm = new ToolStripButton();
            btnMaxForm = new ToolStripButton();
            btnMinForm = new ToolStripButton();
            titleBar.SuspendLayout();
            SuspendLayout();
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth8Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "TitleNormal_white.png");
            imageList1.Images.SetKeyName(1, "TitleMax_white.png");
            // 
            // titleBar
            // 
            titleBar.AutoSize = false;
            titleBar.CanOverflow = false;
            titleBar.GripMargin = new Padding(0);
            titleBar.GripStyle = ToolStripGripStyle.Hidden;
            titleBar.Items.AddRange(new ToolStripItem[] { lblIcon, lblTitle, btnCloseForm, btnMaxForm, btnMinForm });
            titleBar.Location = new Point(0, 0);
            titleBar.Name = "titleBar";
            titleBar.Padding = new Padding(0);
            titleBar.Size = new Size(780, 31);
            titleBar.TabIndex = 0;
            titleBar.MouseDown += titleStrip1_MouseDown;
            // 
            // lblIcon
            // 
            lblIcon.DisplayStyle = ToolStripItemDisplayStyle.Image;
            lblIcon.Image = (Image)resources.GetObject("lblIcon.Image");
            lblIcon.Margin = new Padding(8, 0, 0, 0);
            lblIcon.Name = "lblIcon";
            lblIcon.Size = new Size(16, 31);
            lblIcon.Click += lblIcon_Click;
            // 
            // lblTitle
            // 
            lblTitle.Margin = new Padding(2, 0, 0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(29, 31);
            lblTitle.Text = "Title";
            // 
            // btnCloseForm
            // 
            btnCloseForm.Alignment = ToolStripItemAlignment.Right;
            btnCloseForm.AutoSize = false;
            btnCloseForm.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnCloseForm.Image = (Image)resources.GetObject("btnCloseForm.Image");
            btnCloseForm.ImageTransparentColor = Color.Magenta;
            btnCloseForm.Margin = new Padding(0);
            btnCloseForm.Name = "btnCloseForm";
            btnCloseForm.Overflow = ToolStripItemOverflow.Never;
            btnCloseForm.Size = new Size(45, 28);
            btnCloseForm.Text = "×";
            btnCloseForm.ToolTipText = "閉じる";
            btnCloseForm.Click += btnCloseForm_Click;
            // 
            // btnMaxForm
            // 
            btnMaxForm.Alignment = ToolStripItemAlignment.Right;
            btnMaxForm.AutoSize = false;
            btnMaxForm.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnMaxForm.Image = (Image)resources.GetObject("btnMaxForm.Image");
            btnMaxForm.ImageTransparentColor = Color.Magenta;
            btnMaxForm.Margin = new Padding(0);
            btnMaxForm.Name = "btnMaxForm";
            btnMaxForm.Overflow = ToolStripItemOverflow.Never;
            btnMaxForm.Size = new Size(45, 28);
            btnMaxForm.Text = "□";
            btnMaxForm.Click += btnMaxForm_Click;
            // 
            // btnMinForm
            // 
            btnMinForm.Alignment = ToolStripItemAlignment.Right;
            btnMinForm.AutoSize = false;
            btnMinForm.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnMinForm.Image = (Image)resources.GetObject("btnMinForm.Image");
            btnMinForm.ImageTransparentColor = Color.Magenta;
            btnMinForm.Margin = new Padding(0);
            btnMinForm.Name = "btnMinForm";
            btnMinForm.Overflow = ToolStripItemOverflow.Never;
            btnMinForm.Size = new Size(45, 28);
            btnMinForm.Text = "_";
            btnMinForm.ToolTipText = "最小化";
            btnMinForm.Click += btnMinForm_Click;
            // 
            // BaseForm
            // 
            ClientSize = new Size(780, 471);
            Controls.Add(titleBar);
            Name = "BaseForm";
            FormClosing += frmMain_FormClosing;
            FormClosed += frmMain_FormClosed;
            Load += frmMain_Load;
            SizeChanged += frmMain_SizeChanged;
            Resize += frmMain_Resize;
            titleBar.ResumeLayout(false);
            titleBar.PerformLayout();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolStripLabel lblIcon;
        private System.Windows.Forms.ToolStripButton btnCloseForm;
        private System.Windows.Forms.ToolStripButton btnMaxForm;
        private System.Windows.Forms.ToolStripButton btnMinForm;
        protected System.Windows.Forms.ToolStripLabel lblTitle;
        private System.Windows.Forms.ImageList imageList1;
        protected TitleStrip titleBar;
    }
}

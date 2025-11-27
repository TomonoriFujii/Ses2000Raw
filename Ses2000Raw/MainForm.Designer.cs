namespace Ses2000Raw
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip1 = new MenuStrip();
            tsMenutemFile = new ToolStripMenuItem();
            tsMenuItemOpenRaw = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            tsMenuItemExit = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            toolStripButton1 = new ToolStripButton();
            statusStrip1 = new StatusStrip();
            dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            vS2015DarkTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme();
            openFileDialog1 = new OpenFileDialog();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { tsMenutemFile });
            menuStrip1.Location = new Point(1, 32);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1630, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // tsMenutemFile
            // 
            tsMenutemFile.DropDownItems.AddRange(new ToolStripItem[] { tsMenuItemOpenRaw, toolStripSeparator1, tsMenuItemExit });
            tsMenutemFile.Name = "tsMenutemFile";
            tsMenutemFile.Size = new Size(51, 20);
            tsMenutemFile.Text = "File(&F)";
            tsMenutemFile.DropDownItemClicked += tsMenutemFile_DropDownItemClicked;
            // 
            // tsMenuItemOpenRaw
            // 
            tsMenuItemOpenRaw.Name = "tsMenuItemOpenRaw";
            tsMenuItemOpenRaw.Size = new Size(201, 22);
            tsMenuItemOpenRaw.Tag = "Open";
            tsMenuItemOpenRaw.Text = "Open SES-2000-RAW(&O)";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(198, 6);
            // 
            // tsMenuItemExit
            // 
            tsMenuItemExit.Name = "tsMenuItemExit";
            tsMenuItemExit.Size = new Size(201, 22);
            tsMenuItemExit.Tag = "Exit";
            tsMenuItemExit.Text = "Exit(&X)";
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton1 });
            toolStrip1.Location = new Point(1, 56);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1630, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(23, 22);
            toolStripButton1.Text = "toolStripButton1";
            toolStripButton1.Click += toolStripButton1_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new Point(1, 955);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            statusStrip1.Size = new Size(1630, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // dockPanel1
            // 
            dockPanel1.Dock = DockStyle.Fill;
            dockPanel1.DockBackColor = Color.FromArgb(45, 45, 48);
            dockPanel1.Location = new Point(1, 81);
            dockPanel1.Name = "dockPanel1";
            dockPanel1.Padding = new Padding(6);
            dockPanel1.ShowAutoHideContentOnHover = false;
            dockPanel1.Size = new Size(1630, 874);
            dockPanel1.TabIndex = 3;
            dockPanel1.Theme = vS2015DarkTheme1;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1632, 978);
            Controls.Add(dockPanel1);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            ForeColor = Color.FromArgb(240, 240, 240);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Padding = new Padding(1);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SES-Reflect";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Controls.SetChildIndex(menuStrip1, 0);
            Controls.SetChildIndex(toolStrip1, 0);
            Controls.SetChildIndex(statusStrip1, 0);
            Controls.SetChildIndex(dockPanel1, 0);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem tsMenutemFile;
        private ToolStripMenuItem tsMenuItemOpenRaw;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem tsMenuItemExit;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton1;
        private StatusStrip statusStrip1;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
        private WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme vS2015DarkTheme1;
        private OpenFileDialog openFileDialog1;
    }
}
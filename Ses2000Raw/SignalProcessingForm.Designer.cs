namespace Ses2000Raw
{
    partial class SignalProcessingForm
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
            label1 = new Label();
            cmbDemodulate = new ComboBox();
            lblSigmaTimeSamples = new Label();
            txtSigmaTimeSamples = new TextBox();
            lblGammaTime = new Label();
            txtGammaTime = new TextBox();
            groupBox1 = new GroupBox();
            lblLpfMax = new Label();
            lblLpfMin = new Label();
            trackBarLpf = new TrackBar();
            lblLpf = new Label();
            label9 = new Label();
            lblHpfMax = new Label();
            lblHpfMin = new Label();
            trackBarHpf = new TrackBar();
            lblHpf = new Label();
            label2 = new Label();
            chkBpf = new CheckBox();
            btnOk = new Button();
            btnCancel = new Button();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarLpf).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHpf).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(37, 23);
            label1.Name = "label1";
            label1.Size = new Size(71, 15);
            label1.TabIndex = 0;
            label1.Text = "Demodulate";
            // 
            // cmbDemodulate
            // 
            cmbDemodulate.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDemodulate.FormattingEnabled = true;
            cmbDemodulate.Items.AddRange(new object[] { "None (Full Wave)", "Deconvolution", "Envelope", "Deconvolution + Envelope" });
            cmbDemodulate.Location = new Point(118, 20);
            cmbDemodulate.Name = "cmbDemodulate";
            cmbDemodulate.Size = new Size(201, 23);
            cmbDemodulate.TabIndex = 1;
            cmbDemodulate.SelectedIndexChanged += cmbDemodulate_SelectedIndexChanged;
            // 
            // lblSigmaTimeSamples
            // 
            lblSigmaTimeSamples.AutoSize = true;
            lblSigmaTimeSamples.Location = new Point(37, 52);
            lblSigmaTimeSamples.Name = "lblSigmaTimeSamples";
            lblSigmaTimeSamples.Size = new Size(113, 15);
            lblSigmaTimeSamples.TabIndex = 2;
            lblSigmaTimeSamples.Text = "Sigma Time Samples";
            // 
            // txtSigmaTimeSamples
            // 
            txtSigmaTimeSamples.Location = new Point(180, 49);
            txtSigmaTimeSamples.Name = "txtSigmaTimeSamples";
            txtSigmaTimeSamples.Size = new Size(139, 23);
            txtSigmaTimeSamples.TabIndex = 3;
            // 
            // lblGammaTime
            // 
            lblGammaTime.AutoSize = true;
            lblGammaTime.Location = new Point(37, 81);
            lblGammaTime.Name = "lblGammaTime";
            lblGammaTime.Size = new Size(75, 15);
            lblGammaTime.TabIndex = 4;
            lblGammaTime.Text = "Gamma Time";
            // 
            // txtGammaTime
            // 
            txtGammaTime.Location = new Point(180, 78);
            txtGammaTime.Name = "txtGammaTime";
            txtGammaTime.Size = new Size(139, 23);
            txtGammaTime.TabIndex = 5;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lblLpfMax);
            groupBox1.Controls.Add(lblLpfMin);
            groupBox1.Controls.Add(trackBarLpf);
            groupBox1.Controls.Add(lblLpf);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(lblHpfMax);
            groupBox1.Controls.Add(lblHpfMin);
            groupBox1.Controls.Add(trackBarHpf);
            groupBox1.Controls.Add(lblHpf);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(23, 117);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(355, 141);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            // 
            // lblLpfMax
            // 
            lblLpfMax.Location = new Point(306, 103);
            lblLpfMax.Name = "lblLpfMax";
            lblLpfMax.Size = new Size(28, 23);
            lblLpfMax.TabIndex = 10;
            lblLpfMax.Text = "48";
            lblLpfMax.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblLpfMin
            // 
            lblLpfMin.Location = new Point(95, 103);
            lblLpfMin.Name = "lblLpfMin";
            lblLpfMin.Size = new Size(28, 23);
            lblLpfMin.TabIndex = 9;
            lblLpfMin.Text = "0";
            lblLpfMin.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // trackBarLpf
            // 
            trackBarLpf.Location = new Point(95, 81);
            trackBarLpf.Name = "trackBarLpf";
            trackBarLpf.Size = new Size(235, 45);
            trackBarLpf.TabIndex = 8;
            trackBarLpf.Tag = "Lpf";
            trackBarLpf.TickStyle = TickStyle.None;
            trackBarLpf.Scroll += trackBarBpf_Scroll;
            // 
            // lblLpf
            // 
            lblLpf.BorderStyle = BorderStyle.FixedSingle;
            lblLpf.Location = new Point(49, 81);
            lblLpf.Name = "lblLpf";
            lblLpf.Size = new Size(40, 23);
            lblLpf.TabIndex = 7;
            lblLpf.Text = "0";
            lblLpf.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(14, 85);
            label9.Name = "label9";
            label9.Size = new Size(26, 15);
            label9.TabIndex = 6;
            label9.Text = "LPF";
            // 
            // lblHpfMax
            // 
            lblHpfMax.Location = new Point(306, 52);
            lblHpfMax.Name = "lblHpfMax";
            lblHpfMax.Size = new Size(28, 23);
            lblHpfMax.TabIndex = 5;
            lblHpfMax.Text = "48";
            lblHpfMax.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblHpfMin
            // 
            lblHpfMin.Location = new Point(95, 52);
            lblHpfMin.Name = "lblHpfMin";
            lblHpfMin.Size = new Size(28, 23);
            lblHpfMin.TabIndex = 4;
            lblHpfMin.Text = "0";
            lblHpfMin.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // trackBarHpf
            // 
            trackBarHpf.Location = new Point(95, 30);
            trackBarHpf.Name = "trackBarHpf";
            trackBarHpf.Size = new Size(235, 45);
            trackBarHpf.TabIndex = 3;
            trackBarHpf.Tag = "Hpf";
            trackBarHpf.TickStyle = TickStyle.None;
            trackBarHpf.Scroll += trackBarBpf_Scroll;
            // 
            // lblHpf
            // 
            lblHpf.BorderStyle = BorderStyle.FixedSingle;
            lblHpf.Location = new Point(49, 30);
            lblHpf.Name = "lblHpf";
            lblHpf.Size = new Size(40, 23);
            lblHpf.TabIndex = 2;
            lblHpf.Text = "0";
            lblHpf.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 34);
            label2.Name = "label2";
            label2.Size = new Size(29, 15);
            label2.TabIndex = 1;
            label2.Text = "HPF";
            // 
            // chkBpf
            // 
            chkBpf.AutoSize = true;
            chkBpf.Location = new Point(37, 115);
            chkBpf.Name = "chkBpf";
            chkBpf.Size = new Size(109, 19);
            chkBpf.TabIndex = 6;
            chkBpf.Text = "Band-pass-filter";
            chkBpf.UseVisualStyleBackColor = true;
            chkBpf.CheckedChanged += chkBpf_CheckedChanged;
            // 
            // btnOk
            // 
            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOk.Location = new Point(235, 278);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 8;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(316, 278);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 9;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // SignalProcessingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(403, 318);
            Controls.Add(txtGammaTime);
            Controls.Add(lblGammaTime);
            Controls.Add(txtSigmaTimeSamples);
            Controls.Add(lblSigmaTimeSamples);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(chkBpf);
            Controls.Add(groupBox1);
            Controls.Add(cmbDemodulate);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "SignalProcessingForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Signal Processing";
            Load += SignalProcessingForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarLpf).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHpf).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox cmbDemodulate;
        private GroupBox groupBox1;
        private Label lblHpfMin;
        private TrackBar trackBarHpf;
        private Label lblHpf;
        private Label label2;
        private CheckBox chkBpf;
        private Label lblLpfMax;
        private Label lblLpfMin;
        private TrackBar trackBarLpf;
        private Label lblLpf;
        private Label label9;
        private Label lblHpfMax;
        private Button btnOk;
        private Button btnCancel;
        private Label lblSigmaTimeSamples;
        private TextBox txtSigmaTimeSamples;
        private Label lblGammaTime;
        private TextBox txtGammaTime;
    }
}
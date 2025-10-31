namespace Ses2000Raw
{
    partial class LoadParamForm
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
            groupBox1 = new GroupBox();
            label4 = new Label();
            label3 = new Label();
            cmbBoxFreq = new ComboBox();
            cmbBoxAngle = new ComboBox();
            label2 = new Label();
            cmbBoxCH = new ComboBox();
            label1 = new Label();
            btnOk = new Button();
            btnCancel = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(cmbBoxFreq);
            groupBox1.Controls.Add(cmbBoxAngle);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(cmbBoxCH);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(249, 135);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Channel";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(195, 89);
            label4.Name = "label4";
            label4.Size = new Size(35, 15);
            label4.TabIndex = 6;
            label4.Text = "[kHz]";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(18, 89);
            label3.Name = "label3";
            label3.Size = new Size(64, 15);
            label3.TabIndex = 5;
            label3.Text = "Frequency:";
            // 
            // cmbBoxFreq
            // 
            cmbBoxFreq.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBoxFreq.FormattingEnabled = true;
            cmbBoxFreq.Items.AddRange(new object[] { "LF", "HF" });
            cmbBoxFreq.Location = new Point(88, 86);
            cmbBoxFreq.Name = "cmbBoxFreq";
            cmbBoxFreq.Size = new Size(101, 23);
            cmbBoxFreq.TabIndex = 4;
            // 
            // cmbBoxAngle
            // 
            cmbBoxAngle.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBoxAngle.FormattingEnabled = true;
            cmbBoxAngle.Items.AddRange(new object[] { "LF", "HF" });
            cmbBoxAngle.Location = new Point(88, 57);
            cmbBoxAngle.Name = "cmbBoxAngle";
            cmbBoxAngle.Size = new Size(101, 23);
            cmbBoxAngle.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 60);
            label2.Name = "label2";
            label2.Size = new Size(41, 15);
            label2.TabIndex = 2;
            label2.Text = "Angle:";
            // 
            // cmbBoxCH
            // 
            cmbBoxCH.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBoxCH.FormattingEnabled = true;
            cmbBoxCH.Items.AddRange(new object[] { "LF", "HF" });
            cmbBoxCH.Location = new Point(88, 28);
            cmbBoxCH.Name = "cmbBoxCH";
            cmbBoxCH.Size = new Size(101, 23);
            cmbBoxCH.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 31);
            label1.Name = "label1";
            label1.Size = new Size(53, 15);
            label1.TabIndex = 0;
            label1.Text = "Channel:";
            // 
            // btnOk
            // 
            btnOk.Location = new Point(267, 18);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 1;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(267, 47);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // LoadParamForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(360, 164);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "LoadParamForm";
            Text = "Load RAW-File";
            Load += LoadParamForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private ComboBox cmbBoxFreq;
        private ComboBox cmbBoxAngle;
        private Label label2;
        private ComboBox cmbBoxCH;
        private Label label1;
        private Label label4;
        private Label label3;
        private Button btnOk;
        private Button btnCancel;
    }
}
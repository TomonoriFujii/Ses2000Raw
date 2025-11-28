namespace Ses2000Raw
{
    partial class ScaleSettingForm
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
            label2 = new Label();
            numDistScaleInterval = new NumericUpDown();
            numDepScaleInterval = new NumericUpDown();
            label3 = new Label();
            lblDistScaleColor = new Label();
            label17 = new Label();
            lblDepScaleColor = new Label();
            btnOK = new Button();
            btnCancel = new Button();
            colorDialog1 = new ColorDialog();
            ((System.ComponentModel.ISupportInitialize)numDistScaleInterval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDepScaleInterval).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 41);
            label1.Name = "label1";
            label1.Size = new Size(85, 15);
            label1.TabIndex = 0;
            label1.Text = "Distance Scale:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(26, 70);
            label2.Name = "label2";
            label2.Size = new Size(72, 15);
            label2.TabIndex = 1;
            label2.Text = "Depth Scale:";
            // 
            // numDistScaleInterval
            // 
            numDistScaleInterval.DecimalPlaces = 1;
            numDistScaleInterval.Location = new Point(104, 39);
            numDistScaleInterval.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            numDistScaleInterval.Name = "numDistScaleInterval";
            numDistScaleInterval.Size = new Size(94, 23);
            numDistScaleInterval.TabIndex = 2;
            numDistScaleInterval.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // numDepScaleInterval
            // 
            numDepScaleInterval.DecimalPlaces = 1;
            numDepScaleInterval.Location = new Point(104, 68);
            numDepScaleInterval.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            numDepScaleInterval.Name = "numDepScaleInterval";
            numDepScaleInterval.Size = new Size(94, 23);
            numDepScaleInterval.TabIndex = 3;
            numDepScaleInterval.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(104, 21);
            label3.Name = "label3";
            label3.Size = new Size(67, 15);
            label3.TabIndex = 4;
            label3.Text = "Interval [m]";
            // 
            // lblDistScaleColor
            // 
            lblDistScaleColor.BackColor = Color.Black;
            lblDistScaleColor.BorderStyle = BorderStyle.FixedSingle;
            lblDistScaleColor.Location = new Point(205, 39);
            lblDistScaleColor.Name = "lblDistScaleColor";
            lblDistScaleColor.Size = new Size(76, 23);
            lblDistScaleColor.TabIndex = 12;
            lblDistScaleColor.Tag = "DistScaleColor";
            lblDistScaleColor.Click += lblScaleColor_Click;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(205, 21);
            label17.Name = "label17";
            label17.Size = new Size(60, 15);
            label17.TabIndex = 11;
            label17.Text = "Line Color";
            // 
            // lblDepScaleColor
            // 
            lblDepScaleColor.BackColor = Color.Black;
            lblDepScaleColor.BorderStyle = BorderStyle.FixedSingle;
            lblDepScaleColor.Location = new Point(205, 68);
            lblDepScaleColor.Name = "lblDepScaleColor";
            lblDepScaleColor.Size = new Size(76, 23);
            lblDepScaleColor.TabIndex = 13;
            lblDepScaleColor.Tag = "DepScaleColor";
            lblDepScaleColor.Click += lblScaleColor_Click;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(179, 112);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 23);
            btnOK.TabIndex = 14;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(260, 112);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 15;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // ScaleSettingForm
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(347, 147);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(lblDepScaleColor);
            Controls.Add(lblDistScaleColor);
            Controls.Add(label17);
            Controls.Add(label3);
            Controls.Add(numDepScaleInterval);
            Controls.Add(numDistScaleInterval);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "ScaleSettingForm";
            Text = "Scale Setting";
            Load += ScaleSettingForm_Load;
            ((System.ComponentModel.ISupportInitialize)numDistScaleInterval).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDepScaleInterval).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private NumericUpDown numDistScaleInterval;
        private NumericUpDown numDepScaleInterval;
        private Label label3;
        private Label lblDistScaleColor;
        private Label label17;
        private Label lblDepScaleColor;
        private Button btnOK;
        private Button btnCancel;
        private ColorDialog colorDialog1;
    }
}
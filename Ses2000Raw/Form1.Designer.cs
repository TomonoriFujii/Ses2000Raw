namespace Ses2000Raw
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            txtBoxRaw = new TextBox();
            btnBrowse = new Button();
            btnOutputCSV = new Button();
            openFileDialog1 = new OpenFileDialog();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(98, 15);
            label1.TabIndex = 0;
            label1.Text = "SES2000 Raw File:";
            // 
            // txtBoxRaw
            // 
            txtBoxRaw.Location = new Point(12, 27);
            txtBoxRaw.Name = "txtBoxRaw";
            txtBoxRaw.Size = new Size(464, 23);
            txtBoxRaw.TabIndex = 1;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(482, 27);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 23);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnOutputCSV
            // 
            btnOutputCSV.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOutputCSV.Location = new Point(469, 78);
            btnOutputCSV.Name = "btnOutputCSV";
            btnOutputCSV.Size = new Size(123, 43);
            btnOutputCSV.TabIndex = 3;
            btnOutputCSV.Text = "Output CSV";
            btnOutputCSV.UseVisualStyleBackColor = true;
            btnOutputCSV.Click += btnOutputCSV_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(604, 133);
            Controls.Add(btnOutputCSV);
            Controls.Add(btnBrowse);
            Controls.Add(txtBoxRaw);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtBoxRaw;
        private Button btnBrowse;
        private Button btnOutputCSV;
        private OpenFileDialog openFileDialog1;
    }
}

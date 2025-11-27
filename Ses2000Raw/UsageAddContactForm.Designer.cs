namespace Ses2000Raw
{
    partial class UsageAddContactForm
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
            buttonOK = new Button();
            buttonCancel = new Button();
            label1 = new Label();
            checkBox1 = new CheckBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            SuspendLayout();
            // 
            // buttonOK
            // 
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Location = new Point(544, 372);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(115, 47);
            buttonOK.TabIndex = 0;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(674, 372);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(103, 47);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Yu Gothic UI", 20F);
            label1.Location = new Point(31, 33);
            label1.Name = "label1";
            label1.Size = new Size(408, 46);
            label1.TabIndex = 2;
            label1.Text = "how to use \"Add Contact\"";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Yu Gothic UI", 12F);
            checkBox1.Location = new Point(31, 377);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(188, 32);
            checkBox1.TabIndex = 3;
            checkBox1.Text = "次からは表示しない";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Yu Gothic UI", 15F);
            label2.Location = new Point(31, 117);
            label2.Name = "label2";
            label2.Size = new Size(534, 35);
            label2.TabIndex = 4;
            label2.Text = "1.はじめに、反射体の真上の海底面をクリックします。";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Yu Gothic UI", 15F);
            label3.Location = new Point(31, 152);
            label3.Name = "label3";
            label3.Size = new Size(405, 35);
            label3.TabIndex = 5;
            label3.Text = "2.次に、反射体の中心をクリックします。";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Yu Gothic UI", 15F);
            label4.Location = new Point(31, 187);
            label4.Name = "label4";
            label4.Size = new Size(524, 35);
            label4.TabIndex = 6;
            label4.Text = "3.二回目のクリックの後、表に情報が表示されます。";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.FromArgb(255, 192, 192);
            label5.Font = new Font("Yu Gothic UI", 10F);
            label5.ForeColor = Color.Navy;
            label5.Location = new Point(31, 253);
            label5.Name = "label5";
            label5.Size = new Size(635, 69);
            label5.TabIndex = 7;
            label5.Text = "【注意】\r\n操作を始めると、途中で終わることができません。必ず二回目のクリックまで完了させてください。\r\nもし追加したデータを削除したい場合は、表にデータを追加後、表から削除してください。\r\n";
            // 
            // UsageAddContactForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(checkBox1);
            Controls.Add(label1);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Name = "UsageAddContactForm";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonOK;
        private Button buttonCancel;
        private Label label1;
        private CheckBox checkBox1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
    }
}
namespace Ses2000Raw
{
    partial class MapForm
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
            map1 = new DotSpatial.Controls.Map();
            SuspendLayout();
            // 
            // map1
            // 
            map1.AllowDrop = true;
            map1.BackColor = Color.White;
            map1.CollisionDetection = false;
            map1.Dock = DockStyle.Fill;
            map1.ExtendBuffer = false;
            map1.FunctionMode = DotSpatial.Controls.FunctionMode.None;
            map1.IsBusy = false;
            map1.IsZoomedToMaxExtent = false;
            map1.Legend = null;
            map1.Location = new Point(0, 0);
            map1.Name = "map1";
            map1.ProgressHandler = null;
            map1.ProjectionModeDefine = DotSpatial.Controls.ActionMode.Prompt;
            map1.ProjectionModeReproject = DotSpatial.Controls.ActionMode.Prompt;
            map1.RedrawLayersWhileResizing = false;
            map1.SelectionEnabled = true;
            map1.Size = new Size(800, 450);
            map1.TabIndex = 0;
            map1.ZoomOutFartherThanMaxExtent = false;
            // 
            // MapForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(map1);
            Name = "MapForm";
            Text = "MapForm";
            ResumeLayout(false);
        }

        #endregion

        private DotSpatial.Controls.Map map1;
    }
}
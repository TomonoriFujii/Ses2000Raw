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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapForm));
            map1 = new DotSpatial.Controls.Map();
            spatialStatusStrip1 = new DotSpatial.Controls.SpatialStatusStrip();
            spatialToolStrip1 = new DotSpatial.Controls.SpatialToolStrip();
            appManager1 = new DotSpatial.Controls.AppManager();
            SuspendLayout();
            // 
            // map1
            // 
            map1.AllowDrop = true;
            map1.AutoScroll = true;
            map1.BackColor = SystemColors.ControlDarkDark;
            map1.CollisionDetection = false;
            map1.Dock = DockStyle.Fill;
            map1.ExtendBuffer = false;
            map1.FunctionMode = DotSpatial.Controls.FunctionMode.None;
            map1.IsBusy = false;
            map1.IsZoomedToMaxExtent = false;
            map1.Legend = null;
            map1.Location = new Point(0, 25);
            map1.Name = "map1";
            map1.ProgressHandler = null;
            map1.ProjectionModeDefine = DotSpatial.Controls.ActionMode.Prompt;
            map1.ProjectionModeReproject = DotSpatial.Controls.ActionMode.Prompt;
            map1.RedrawLayersWhileResizing = false;
            map1.SelectionEnabled = true;
            map1.Size = new Size(800, 403);
            map1.TabIndex = 0;
            map1.ZoomOutFartherThanMaxExtent = false;
            // 
            // spatialStatusStrip1
            // 
            spatialStatusStrip1.Location = new Point(0, 428);
            spatialStatusStrip1.Name = "spatialStatusStrip1";
            spatialStatusStrip1.ProgressBar = null;
            spatialStatusStrip1.ProgressLabel = null;
            spatialStatusStrip1.Size = new Size(800, 22);
            spatialStatusStrip1.TabIndex = 1;
            spatialStatusStrip1.Text = "spatialStatusStrip1";
            // 
            // spatialToolStrip1
            // 
            spatialToolStrip1.ApplicationManager = null;
            spatialToolStrip1.Location = new Point(0, 0);
            spatialToolStrip1.Map = map1;
            spatialToolStrip1.Name = "spatialToolStrip1";
            spatialToolStrip1.Size = new Size(800, 25);
            spatialToolStrip1.TabIndex = 2;
            spatialToolStrip1.Text = "spatialToolStrip1";
            // 
            // appManager1
            // 
            appManager1.Directories = (List<string>)resources.GetObject("appManager1.Directories");
            appManager1.DockManager = null;
            appManager1.HeaderControl = null;
            appManager1.Legend = null;
            appManager1.Map = map1;
            appManager1.ProgressHandler = null;
            // 
            // MapForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(map1);
            Controls.Add(spatialToolStrip1);
            Controls.Add(spatialStatusStrip1);
            Name = "MapForm";
            Text = "Map";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DotSpatial.Controls.Map map1;
        private DotSpatial.Controls.SpatialStatusStrip spatialStatusStrip1;
        private DotSpatial.Controls.SpatialToolStrip spatialToolStrip1;
        private DotSpatial.Controls.AppManager appManager1;
    }
}
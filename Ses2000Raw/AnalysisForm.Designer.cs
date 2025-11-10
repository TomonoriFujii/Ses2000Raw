namespace Ses2000Raw
{
    partial class AnalysisForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalysisForm));
            splitContainer1 = new SplitContainer();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            grpBox3DFrustum = new GroupBox();
            numNear = new NumericUpDown();
            label20 = new Label();
            numFov = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            numViewX = new NumericUpDown();
            numFar = new NumericUpDown();
            label4 = new Label();
            numViewY = new NumericUpDown();
            label5 = new Label();
            numViewZ = new NumericUpDown();
            grpBox3DMoving = new GroupBox();
            label41 = new Label();
            numScaleX = new NumericUpDown();
            label42 = new Label();
            numScaleY = new NumericUpDown();
            label43 = new Label();
            numScaleZ = new NumericUpDown();
            label6 = new Label();
            numTranX = new NumericUpDown();
            label7 = new Label();
            numTranY = new NumericUpDown();
            label8 = new Label();
            numTranZ = new NumericUpDown();
            label9 = new Label();
            numRotateX = new NumericUpDown();
            label10 = new Label();
            numRotateY = new NumericUpDown();
            label11 = new Label();
            numRotateZ = new NumericUpDown();
            grpBoxSignal = new GroupBox();
            btnSignalProcessing = new Button();
            lblLPF = new Label();
            lblHPF = new Label();
            label23 = new Label();
            label22 = new Label();
            lblDemodulate = new Label();
            label12 = new Label();
            grpBoxDisplay = new GroupBox();
            chkFlipX = new CheckBox();
            btnScaleSetting = new Button();
            chkHeaveCorrection = new CheckBox();
            chkDrawDistScale = new CheckBox();
            chkDrawDepthScale = new CheckBox();
            numBottom = new NumericUpDown();
            label15 = new Label();
            numAttDb = new NumericUpDown();
            label14 = new Label();
            label18 = new Label();
            numAlpha = new NumericUpDown();
            numThreshold = new NumericUpDown();
            label13 = new Label();
            numIntensity = new NumericUpDown();
            label19 = new Label();
            grpBoxColor = new GroupBox();
            chkInvert = new CheckBox();
            btnChooseColor = new Button();
            lblBackColor = new Label();
            label17 = new Label();
            cmbColor = new ComboBox();
            label16 = new Label();
            numR = new NumericUpDown();
            label27 = new Label();
            numB = new NumericUpDown();
            label28 = new Label();
            numG = new NumericUpDown();
            label26 = new Label();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            tabPage5 = new TabPage();
            glControl2D = new OpenTK.GLControl.GLControl();
            colorDialog1 = new ColorDialog();
            toolStrip1 = new ToolStrip();
            tsBtnSaveImage = new ToolStripButton();
            formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            grpBox3DFrustum.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numNear).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFov).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numViewX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numViewY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numViewZ).BeginInit();
            grpBox3DMoving.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numScaleX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numScaleY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numScaleZ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTranX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTranY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTranZ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRotateX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRotateY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRotateZ).BeginInit();
            grpBoxSignal.SuspendLayout();
            grpBoxDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numBottom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numAttDb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numAlpha).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numThreshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numIntensity).BeginInit();
            grpBoxColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numR).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numB).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numG).BeginInit();
            tabPage2.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.BackColor = Color.Transparent;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(0, 31);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(glControl2D);
            splitContainer1.Size = new Size(1219, 785);
            splitContainer1.SplitterDistance = 290;
            splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            tabControl1.Alignment = TabAlignment.Left;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Margin = new Padding(0);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(290, 785);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.AutoScroll = true;
            tabPage1.Controls.Add(grpBox3DFrustum);
            tabPage1.Controls.Add(grpBox3DMoving);
            tabPage1.Controls.Add(grpBoxSignal);
            tabPage1.Controls.Add(grpBoxDisplay);
            tabPage1.Controls.Add(grpBoxColor);
            tabPage1.Location = new Point(27, 4);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(259, 777);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Setting";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // grpBox3DFrustum
            // 
            grpBox3DFrustum.Controls.Add(numNear);
            grpBox3DFrustum.Controls.Add(label20);
            grpBox3DFrustum.Controls.Add(numFov);
            grpBox3DFrustum.Controls.Add(label1);
            grpBox3DFrustum.Controls.Add(label2);
            grpBox3DFrustum.Controls.Add(label3);
            grpBox3DFrustum.Controls.Add(numViewX);
            grpBox3DFrustum.Controls.Add(numFar);
            grpBox3DFrustum.Controls.Add(label4);
            grpBox3DFrustum.Controls.Add(numViewY);
            grpBox3DFrustum.Controls.Add(label5);
            grpBox3DFrustum.Controls.Add(numViewZ);
            grpBox3DFrustum.Location = new Point(3, 711);
            grpBox3DFrustum.Name = "grpBox3DFrustum";
            grpBox3DFrustum.Size = new Size(242, 113);
            grpBox3DFrustum.TabIndex = 7;
            grpBox3DFrustum.TabStop = false;
            grpBox3DFrustum.Text = "3D View Frustum";
            // 
            // numNear
            // 
            numNear.Location = new Point(90, 35);
            numNear.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            numNear.Minimum = new decimal(new int[] { 360, 0, 0, int.MinValue });
            numNear.Name = "numNear";
            numNear.Size = new Size(65, 23);
            numNear.TabIndex = 3;
            numNear.Tag = "Near";
            numNear.Value = new decimal(new int[] { 2, 0, 0, 0 });
            numNear.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(90, 20);
            label20.Name = "label20";
            label20.Size = new Size(32, 15);
            label20.TabIndex = 2;
            label20.Text = "Near";
            // 
            // numFov
            // 
            numFov.Location = new Point(12, 35);
            numFov.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            numFov.Minimum = new decimal(new int[] { 360, 0, 0, int.MinValue });
            numFov.Name = "numFov";
            numFov.Size = new Size(65, 23);
            numFov.TabIndex = 1;
            numFov.Tag = "Fov";
            numFov.Value = new decimal(new int[] { 150, 0, 0, 0 });
            numFov.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 20);
            label1.Name = "label1";
            label1.Size = new Size(26, 15);
            label1.TabIndex = 0;
            label1.Text = "Fov";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(165, 20);
            label2.Name = "label2";
            label2.Size = new Size(22, 15);
            label2.TabIndex = 4;
            label2.Text = "Far";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 61);
            label3.Name = "label3";
            label3.Size = new Size(39, 15);
            label3.TabIndex = 6;
            label3.Text = "ViewX";
            // 
            // numViewX
            // 
            numViewX.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            numViewX.Location = new Point(12, 76);
            numViewX.Maximum = new decimal(new int[] { 2000000, 0, 0, 0 });
            numViewX.Minimum = new decimal(new int[] { 1600000, 0, 0, int.MinValue });
            numViewX.Name = "numViewX";
            numViewX.Size = new Size(65, 23);
            numViewX.TabIndex = 7;
            numViewX.Tag = "ViewX";
            numViewX.ValueChanged += numericUpDown_ValueChanged;
            // 
            // numFar
            // 
            numFar.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            numFar.Location = new Point(165, 35);
            numFar.Maximum = new decimal(new int[] { 1500000, 0, 0, 0 });
            numFar.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numFar.Name = "numFar";
            numFar.Size = new Size(65, 23);
            numFar.TabIndex = 5;
            numFar.Tag = "Far";
            numFar.Value = new decimal(new int[] { 15000, 0, 0, 0 });
            numFar.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(90, 61);
            label4.Name = "label4";
            label4.Size = new Size(39, 15);
            label4.TabIndex = 8;
            label4.Text = "ViewY";
            // 
            // numViewY
            // 
            numViewY.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            numViewY.Location = new Point(90, 76);
            numViewY.Maximum = new decimal(new int[] { 1600000, 0, 0, 0 });
            numViewY.Minimum = new decimal(new int[] { 16000000, 0, 0, int.MinValue });
            numViewY.Name = "numViewY";
            numViewY.Size = new Size(65, 23);
            numViewY.TabIndex = 9;
            numViewY.Tag = "ViewY";
            numViewY.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(165, 61);
            label5.Name = "label5";
            label5.Size = new Size(39, 15);
            label5.TabIndex = 10;
            label5.Text = "ViewZ";
            // 
            // numViewZ
            // 
            numViewZ.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numViewZ.Location = new Point(165, 76);
            numViewZ.Maximum = new decimal(new int[] { 1600000, 0, 0, 0 });
            numViewZ.Minimum = new decimal(new int[] { 16000000, 0, 0, int.MinValue });
            numViewZ.Name = "numViewZ";
            numViewZ.Size = new Size(65, 23);
            numViewZ.TabIndex = 11;
            numViewZ.Tag = "ViewZ";
            numViewZ.Value = new decimal(new int[] { 1300, 0, 0, 0 });
            numViewZ.ValueChanged += numericUpDown_ValueChanged;
            // 
            // grpBox3DMoving
            // 
            grpBox3DMoving.Controls.Add(label41);
            grpBox3DMoving.Controls.Add(numScaleX);
            grpBox3DMoving.Controls.Add(label42);
            grpBox3DMoving.Controls.Add(numScaleY);
            grpBox3DMoving.Controls.Add(label43);
            grpBox3DMoving.Controls.Add(numScaleZ);
            grpBox3DMoving.Controls.Add(label6);
            grpBox3DMoving.Controls.Add(numTranX);
            grpBox3DMoving.Controls.Add(label7);
            grpBox3DMoving.Controls.Add(numTranY);
            grpBox3DMoving.Controls.Add(label8);
            grpBox3DMoving.Controls.Add(numTranZ);
            grpBox3DMoving.Controls.Add(label9);
            grpBox3DMoving.Controls.Add(numRotateX);
            grpBox3DMoving.Controls.Add(label10);
            grpBox3DMoving.Controls.Add(numRotateY);
            grpBox3DMoving.Controls.Add(label11);
            grpBox3DMoving.Controls.Add(numRotateZ);
            grpBox3DMoving.Location = new Point(6, 554);
            grpBox3DMoving.Name = "grpBox3DMoving";
            grpBox3DMoving.Size = new Size(242, 151);
            grpBox3DMoving.TabIndex = 5;
            grpBox3DMoving.TabStop = false;
            grpBox3DMoving.Text = "3D View Moving";
            // 
            // label41
            // 
            label41.AutoSize = true;
            label41.Location = new Point(13, 102);
            label41.Name = "label41";
            label41.Size = new Size(41, 15);
            label41.TabIndex = 12;
            label41.Text = "ScaleX";
            // 
            // numScaleX
            // 
            numScaleX.DecimalPlaces = 2;
            numScaleX.Enabled = false;
            numScaleX.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numScaleX.Location = new Point(13, 117);
            numScaleX.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            numScaleX.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            numScaleX.Name = "numScaleX";
            numScaleX.Size = new Size(65, 23);
            numScaleX.TabIndex = 13;
            numScaleX.Tag = "ScaleX";
            numScaleX.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numScaleX.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label42
            // 
            label42.AutoSize = true;
            label42.Location = new Point(90, 102);
            label42.Name = "label42";
            label42.Size = new Size(41, 15);
            label42.TabIndex = 14;
            label42.Text = "ScaleY";
            // 
            // numScaleY
            // 
            numScaleY.DecimalPlaces = 2;
            numScaleY.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numScaleY.Location = new Point(90, 117);
            numScaleY.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            numScaleY.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            numScaleY.Name = "numScaleY";
            numScaleY.Size = new Size(65, 23);
            numScaleY.TabIndex = 15;
            numScaleY.Tag = "ScaleY";
            numScaleY.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numScaleY.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label43
            // 
            label43.AutoSize = true;
            label43.Location = new Point(165, 102);
            label43.Name = "label43";
            label43.Size = new Size(41, 15);
            label43.TabIndex = 16;
            label43.Text = "ScaleZ";
            // 
            // numScaleZ
            // 
            numScaleZ.DecimalPlaces = 2;
            numScaleZ.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numScaleZ.Location = new Point(165, 117);
            numScaleZ.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            numScaleZ.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            numScaleZ.Name = "numScaleZ";
            numScaleZ.Size = new Size(65, 23);
            numScaleZ.TabIndex = 17;
            numScaleZ.Tag = "ScaleZ";
            numScaleZ.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numScaleZ.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(13, 20);
            label6.Name = "label6";
            label6.Size = new Size(36, 15);
            label6.TabIndex = 0;
            label6.Text = "TranX";
            // 
            // numTranX
            // 
            numTranX.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numTranX.Location = new Point(13, 35);
            numTranX.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            numTranX.Minimum = new decimal(new int[] { 2000, 0, 0, int.MinValue });
            numTranX.Name = "numTranX";
            numTranX.Size = new Size(65, 23);
            numTranX.TabIndex = 1;
            numTranX.Tag = "TranX";
            numTranX.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(90, 20);
            label7.Name = "label7";
            label7.Size = new Size(36, 15);
            label7.TabIndex = 2;
            label7.Text = "TranY";
            // 
            // numTranY
            // 
            numTranY.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numTranY.Location = new Point(90, 35);
            numTranY.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            numTranY.Minimum = new decimal(new int[] { 2000, 0, 0, int.MinValue });
            numTranY.Name = "numTranY";
            numTranY.Size = new Size(65, 23);
            numTranY.TabIndex = 3;
            numTranY.Tag = "TranY";
            numTranY.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(165, 19);
            label8.Name = "label8";
            label8.Size = new Size(36, 15);
            label8.TabIndex = 4;
            label8.Text = "TranZ";
            // 
            // numTranZ
            // 
            numTranZ.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numTranZ.Location = new Point(165, 35);
            numTranZ.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            numTranZ.Minimum = new decimal(new int[] { 2000, 0, 0, int.MinValue });
            numTranZ.Name = "numTranZ";
            numTranZ.Size = new Size(65, 23);
            numTranZ.TabIndex = 5;
            numTranZ.Tag = "TranZ";
            numTranZ.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(13, 61);
            label9.Name = "label9";
            label9.Size = new Size(48, 15);
            label9.TabIndex = 6;
            label9.Text = "RotateX";
            // 
            // numRotateX
            // 
            numRotateX.DecimalPlaces = 1;
            numRotateX.Increment = new decimal(new int[] { 25, 0, 0, 65536 });
            numRotateX.Location = new Point(13, 76);
            numRotateX.Maximum = new decimal(new int[] { 180, 0, 0, 0 });
            numRotateX.Minimum = new decimal(new int[] { 180, 0, 0, int.MinValue });
            numRotateX.Name = "numRotateX";
            numRotateX.Size = new Size(65, 23);
            numRotateX.TabIndex = 7;
            numRotateX.Tag = "RotateX";
            numRotateX.Value = new decimal(new int[] { 125, 0, 0, int.MinValue });
            numRotateX.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(90, 61);
            label10.Name = "label10";
            label10.Size = new Size(48, 15);
            label10.TabIndex = 8;
            label10.Text = "RotateY";
            // 
            // numRotateY
            // 
            numRotateY.DecimalPlaces = 1;
            numRotateY.Increment = new decimal(new int[] { 25, 0, 0, 65536 });
            numRotateY.Location = new Point(90, 76);
            numRotateY.Maximum = new decimal(new int[] { 180, 0, 0, 0 });
            numRotateY.Minimum = new decimal(new int[] { 180, 0, 0, int.MinValue });
            numRotateY.Name = "numRotateY";
            numRotateY.Size = new Size(65, 23);
            numRotateY.TabIndex = 9;
            numRotateY.Tag = "RotateY";
            numRotateY.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(165, 61);
            label11.Name = "label11";
            label11.Size = new Size(48, 15);
            label11.TabIndex = 10;
            label11.Text = "RotateZ";
            // 
            // numRotateZ
            // 
            numRotateZ.DecimalPlaces = 1;
            numRotateZ.Increment = new decimal(new int[] { 25, 0, 0, 65536 });
            numRotateZ.Location = new Point(165, 79);
            numRotateZ.Maximum = new decimal(new int[] { 180, 0, 0, 0 });
            numRotateZ.Minimum = new decimal(new int[] { 180, 0, 0, int.MinValue });
            numRotateZ.Name = "numRotateZ";
            numRotateZ.Size = new Size(65, 23);
            numRotateZ.TabIndex = 11;
            numRotateZ.Tag = "RotateZ";
            numRotateZ.Value = new decimal(new int[] { 70, 0, 0, int.MinValue });
            numRotateZ.ValueChanged += numericUpDown_ValueChanged;
            // 
            // grpBoxSignal
            // 
            grpBoxSignal.Controls.Add(btnSignalProcessing);
            grpBoxSignal.Controls.Add(lblLPF);
            grpBoxSignal.Controls.Add(lblHPF);
            grpBoxSignal.Controls.Add(label23);
            grpBoxSignal.Controls.Add(label22);
            grpBoxSignal.Controls.Add(lblDemodulate);
            grpBoxSignal.Controls.Add(label12);
            grpBoxSignal.Location = new Point(4, 8);
            grpBoxSignal.Name = "grpBoxSignal";
            grpBoxSignal.Size = new Size(241, 151);
            grpBoxSignal.TabIndex = 6;
            grpBoxSignal.TabStop = false;
            grpBoxSignal.Text = "Signal Processing";
            // 
            // btnSignalProcessing
            // 
            btnSignalProcessing.BackColor = Color.Transparent;
            btnSignalProcessing.FlatStyle = FlatStyle.Flat;
            btnSignalProcessing.Location = new Point(11, 111);
            btnSignalProcessing.Name = "btnSignalProcessing";
            btnSignalProcessing.Size = new Size(219, 27);
            btnSignalProcessing.TabIndex = 16;
            btnSignalProcessing.Text = "Signal Processing";
            btnSignalProcessing.UseVisualStyleBackColor = false;
            btnSignalProcessing.Click += btnSignalProcessing_Click;
            // 
            // lblLPF
            // 
            lblLPF.BorderStyle = BorderStyle.FixedSingle;
            lblLPF.Location = new Point(129, 76);
            lblLPF.Name = "lblLPF";
            lblLPF.Size = new Size(100, 23);
            lblLPF.TabIndex = 15;
            lblLPF.Text = "48";
            lblLPF.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblHPF
            // 
            lblHPF.BorderStyle = BorderStyle.FixedSingle;
            lblHPF.Location = new Point(11, 76);
            lblHPF.Name = "lblHPF";
            lblHPF.Size = new Size(100, 23);
            lblHPF.TabIndex = 14;
            lblHPF.Text = "0";
            lblHPF.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(129, 61);
            label23.Name = "label23";
            label23.Size = new Size(84, 15);
            label23.TabIndex = 13;
            label23.Text = "Low Pass Filter";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(11, 61);
            label22.Name = "label22";
            label22.Size = new Size(88, 15);
            label22.TabIndex = 12;
            label22.Text = "High Pass Filter";
            // 
            // lblDemodulate
            // 
            lblDemodulate.BorderStyle = BorderStyle.FixedSingle;
            lblDemodulate.Location = new Point(88, 23);
            lblDemodulate.Name = "lblDemodulate";
            lblDemodulate.Size = new Size(142, 23);
            lblDemodulate.TabIndex = 11;
            lblDemodulate.Tag = "0";
            lblDemodulate.Text = "None (Full Wave)";
            lblDemodulate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(11, 27);
            label12.Name = "label12";
            label12.Size = new Size(71, 15);
            label12.TabIndex = 1;
            label12.Text = "Demodulate";
            // 
            // grpBoxDisplay
            // 
            grpBoxDisplay.Controls.Add(chkFlipX);
            grpBoxDisplay.Controls.Add(btnScaleSetting);
            grpBoxDisplay.Controls.Add(chkHeaveCorrection);
            grpBoxDisplay.Controls.Add(chkDrawDistScale);
            grpBoxDisplay.Controls.Add(chkDrawDepthScale);
            grpBoxDisplay.Controls.Add(numBottom);
            grpBoxDisplay.Controls.Add(label15);
            grpBoxDisplay.Controls.Add(numAttDb);
            grpBoxDisplay.Controls.Add(label14);
            grpBoxDisplay.Controls.Add(label18);
            grpBoxDisplay.Controls.Add(numAlpha);
            grpBoxDisplay.Controls.Add(numThreshold);
            grpBoxDisplay.Controls.Add(label13);
            grpBoxDisplay.Controls.Add(numIntensity);
            grpBoxDisplay.Controls.Add(label19);
            grpBoxDisplay.Location = new Point(6, 338);
            grpBoxDisplay.Name = "grpBoxDisplay";
            grpBoxDisplay.Size = new Size(241, 210);
            grpBoxDisplay.TabIndex = 3;
            grpBoxDisplay.TabStop = false;
            grpBoxDisplay.Text = "Display";
            // 
            // chkFlipX
            // 
            chkFlipX.AutoSize = true;
            chkFlipX.Location = new Point(12, 184);
            chkFlipX.Name = "chkFlipX";
            chkFlipX.Size = new Size(140, 19);
            chkFlipX.TabIndex = 15;
            chkFlipX.Tag = "FlipX";
            chkFlipX.Text = "Flip Image End to End";
            chkFlipX.UseVisualStyleBackColor = true;
            chkFlipX.CheckedChanged += chkBox_CheckedChanged;
            // 
            // btnScaleSetting
            // 
            btnScaleSetting.BackColor = Color.Transparent;
            btnScaleSetting.FlatStyle = FlatStyle.Flat;
            btnScaleSetting.Location = new Point(150, 109);
            btnScaleSetting.Name = "btnScaleSetting";
            btnScaleSetting.Size = new Size(87, 47);
            btnScaleSetting.TabIndex = 13;
            btnScaleSetting.Text = "Scale Setting";
            btnScaleSetting.UseVisualStyleBackColor = false;
            // 
            // chkHeaveCorrection
            // 
            chkHeaveCorrection.AutoSize = true;
            chkHeaveCorrection.Checked = true;
            chkHeaveCorrection.CheckState = CheckState.Checked;
            chkHeaveCorrection.Location = new Point(12, 159);
            chkHeaveCorrection.Name = "chkHeaveCorrection";
            chkHeaveCorrection.Size = new Size(151, 19);
            chkHeaveCorrection.TabIndex = 14;
            chkHeaveCorrection.Tag = "HeaveCorrection";
            chkHeaveCorrection.Text = "Apply Heave Correction";
            chkHeaveCorrection.UseVisualStyleBackColor = true;
            chkHeaveCorrection.CheckedChanged += chkBox_CheckedChanged;
            // 
            // chkDrawDistScale
            // 
            chkDrawDistScale.AutoSize = true;
            chkDrawDistScale.Checked = true;
            chkDrawDistScale.CheckState = CheckState.Checked;
            chkDrawDistScale.Location = new Point(13, 109);
            chkDrawDistScale.Name = "chkDrawDistScale";
            chkDrawDistScale.Size = new Size(131, 19);
            chkDrawDistScale.TabIndex = 11;
            chkDrawDistScale.Tag = "DrawDistanceScale";
            chkDrawDistScale.Text = "Draw Distance Scale";
            chkDrawDistScale.UseVisualStyleBackColor = true;
            chkDrawDistScale.CheckedChanged += chkBox_CheckedChanged;
            // 
            // chkDrawDepthScale
            // 
            chkDrawDepthScale.AutoSize = true;
            chkDrawDepthScale.Checked = true;
            chkDrawDepthScale.CheckState = CheckState.Checked;
            chkDrawDepthScale.Location = new Point(13, 134);
            chkDrawDepthScale.Name = "chkDrawDepthScale";
            chkDrawDepthScale.Size = new Size(118, 19);
            chkDrawDepthScale.TabIndex = 12;
            chkDrawDepthScale.Tag = "DrawDepthScale";
            chkDrawDepthScale.Text = "Draw Depth Scale";
            chkDrawDepthScale.UseVisualStyleBackColor = true;
            chkDrawDepthScale.CheckedChanged += chkBox_CheckedChanged;
            // 
            // numBottom
            // 
            numBottom.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numBottom.Location = new Point(90, 76);
            numBottom.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            numBottom.Minimum = new decimal(new int[] { 2000000, 0, 0, int.MinValue });
            numBottom.Name = "numBottom";
            numBottom.Size = new Size(65, 23);
            numBottom.TabIndex = 7;
            numBottom.Tag = "Bottom";
            numBottom.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(90, 61);
            label15.Name = "label15";
            label15.Size = new Size(46, 15);
            label15.TabIndex = 6;
            label15.Text = "Bottom";
            // 
            // numAttDb
            // 
            numAttDb.DecimalPlaces = 1;
            numAttDb.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            numAttDb.Location = new Point(165, 76);
            numAttDb.Name = "numAttDb";
            numAttDb.Size = new Size(65, 23);
            numAttDb.TabIndex = 9;
            numAttDb.Tag = "AttDb";
            numAttDb.Value = new decimal(new int[] { 20, 0, 0, 0 });
            numAttDb.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(165, 58);
            label14.Name = "label14";
            label14.Size = new Size(23, 15);
            label14.TabIndex = 8;
            label14.Text = "Att";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(140, 20);
            label18.Name = "label18";
            label18.Size = new Size(59, 15);
            label18.TabIndex = 2;
            label18.Text = "Threshold";
            // 
            // numAlpha
            // 
            numAlpha.DecimalPlaces = 3;
            numAlpha.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numAlpha.Location = new Point(13, 76);
            numAlpha.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            numAlpha.Name = "numAlpha";
            numAlpha.Size = new Size(65, 23);
            numAlpha.TabIndex = 5;
            numAlpha.Tag = "Alpha";
            numAlpha.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numAlpha.ValueChanged += numericUpDown_ValueChanged;
            // 
            // numThreshold
            // 
            numThreshold.DecimalPlaces = 4;
            numThreshold.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            numThreshold.Location = new Point(140, 35);
            numThreshold.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numThreshold.Name = "numThreshold";
            numThreshold.Size = new Size(90, 23);
            numThreshold.TabIndex = 3;
            numThreshold.Tag = "Threshold";
            numThreshold.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(12, 61);
            label13.Name = "label13";
            label13.Size = new Size(38, 15);
            label13.TabIndex = 4;
            label13.Text = "Alpha";
            // 
            // numIntensity
            // 
            numIntensity.DecimalPlaces = 6;
            numIntensity.Increment = new decimal(new int[] { 1, 0, 0, 393216 });
            numIntensity.Location = new Point(13, 35);
            numIntensity.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numIntensity.Name = "numIntensity";
            numIntensity.Size = new Size(90, 23);
            numIntensity.TabIndex = 1;
            numIntensity.Tag = "Intensity";
            numIntensity.Value = new decimal(new int[] { 5, 0, 0, 327680 });
            numIntensity.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(12, 20);
            label19.Name = "label19";
            label19.Size = new Size(52, 15);
            label19.TabIndex = 0;
            label19.Text = "Intensity";
            // 
            // grpBoxColor
            // 
            grpBoxColor.Controls.Add(chkInvert);
            grpBoxColor.Controls.Add(btnChooseColor);
            grpBoxColor.Controls.Add(lblBackColor);
            grpBoxColor.Controls.Add(label17);
            grpBoxColor.Controls.Add(cmbColor);
            grpBoxColor.Controls.Add(label16);
            grpBoxColor.Controls.Add(numR);
            grpBoxColor.Controls.Add(label27);
            grpBoxColor.Controls.Add(numB);
            grpBoxColor.Controls.Add(label28);
            grpBoxColor.Controls.Add(numG);
            grpBoxColor.Controls.Add(label26);
            grpBoxColor.Location = new Point(4, 165);
            grpBoxColor.Name = "grpBoxColor";
            grpBoxColor.Size = new Size(241, 167);
            grpBoxColor.TabIndex = 2;
            grpBoxColor.TabStop = false;
            grpBoxColor.Text = "Color";
            // 
            // chkInvert
            // 
            chkInvert.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkInvert.AutoSize = true;
            chkInvert.Location = new Point(161, 39);
            chkInvert.Name = "chkInvert";
            chkInvert.Size = new Size(56, 19);
            chkInvert.TabIndex = 2;
            chkInvert.Tag = "Invert";
            chkInvert.Text = "Invert";
            chkInvert.UseVisualStyleBackColor = true;
            chkInvert.CheckedChanged += chkBox_CheckedChanged;
            // 
            // btnChooseColor
            // 
            btnChooseColor.BackColor = Color.Transparent;
            btnChooseColor.FlatStyle = FlatStyle.Flat;
            btnChooseColor.Location = new Point(129, 128);
            btnChooseColor.Name = "btnChooseColor";
            btnChooseColor.Size = new Size(101, 27);
            btnChooseColor.TabIndex = 11;
            btnChooseColor.Text = "Choose Color";
            btnChooseColor.UseVisualStyleBackColor = false;
            btnChooseColor.Click += btnChooseColor_Click;
            // 
            // lblBackColor
            // 
            lblBackColor.BackColor = Color.Black;
            lblBackColor.BorderStyle = BorderStyle.FixedSingle;
            lblBackColor.Location = new Point(12, 128);
            lblBackColor.Name = "lblBackColor";
            lblBackColor.Size = new Size(111, 27);
            lblBackColor.TabIndex = 10;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(10, 112);
            label17.Name = "label17";
            label17.Size = new Size(102, 15);
            label17.TabIndex = 9;
            label17.Text = "Background Color";
            // 
            // cmbColor
            // 
            cmbColor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbColor.FlatStyle = FlatStyle.Flat;
            cmbColor.FormattingEnabled = true;
            cmbColor.Items.AddRange(new object[] { "Color1", "Color2", "Gray", "Fire", "Spectrum", "3-3-2 RGB", "16 Colors", "Blue Orange icb", "Gem", "Green Fire Blue", "Jet", "Orange Hot", "Phase", "Royal", "Sepia", "Smart", "Thal", "UnionJack", "Viridis" });
            cmbColor.Location = new Point(12, 37);
            cmbColor.Name = "cmbColor";
            cmbColor.Size = new Size(143, 23);
            cmbColor.TabIndex = 1;
            cmbColor.SelectedIndexChanged += cmbColor_SelectedIndexChanged;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(10, 22);
            label16.Name = "label16";
            label16.Size = new Size(43, 15);
            label16.TabIndex = 0;
            label16.Text = "Palette";
            // 
            // numR
            // 
            numR.DecimalPlaces = 1;
            numR.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numR.Location = new Point(12, 82);
            numR.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numR.Name = "numR";
            numR.Size = new Size(50, 23);
            numR.TabIndex = 4;
            numR.Tag = "R";
            numR.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numR.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Location = new Point(123, 67);
            label27.Name = "label27";
            label27.Size = new Size(14, 15);
            label27.TabIndex = 7;
            label27.Text = "B";
            // 
            // numB
            // 
            numB.DecimalPlaces = 1;
            numB.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numB.Location = new Point(124, 82);
            numB.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numB.Name = "numB";
            numB.Size = new Size(50, 23);
            numB.TabIndex = 8;
            numB.Tag = "B";
            numB.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numB.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Location = new Point(69, 67);
            label28.Name = "label28";
            label28.Size = new Size(15, 15);
            label28.TabIndex = 5;
            label28.Text = "G";
            // 
            // numG
            // 
            numG.DecimalPlaces = 1;
            numG.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numG.Location = new Point(68, 82);
            numG.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numG.Name = "numG";
            numG.Size = new Size(50, 23);
            numG.TabIndex = 6;
            numG.Tag = "G";
            numG.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numG.ValueChanged += numericUpDown_ValueChanged;
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new Point(13, 67);
            label26.Name = "label26";
            label26.Size = new Size(14, 15);
            label26.TabIndex = 3;
            label26.Text = "R";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(formsPlot1);
            tabPage2.Location = new Point(27, 4);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(259, 777);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Signal";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(27, 4);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(259, 777);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "FFT";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            tabPage5.Location = new Point(27, 4);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(259, 777);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "Ping Info";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // glControl2D
            // 
            glControl2D.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl2D.APIVersion = new Version(4, 6, 4, 0);
            glControl2D.Dock = DockStyle.Fill;
            glControl2D.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glControl2D.IsEventDriven = true;
            glControl2D.Location = new Point(0, 0);
            glControl2D.Name = "glControl2D";
            glControl2D.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
            glControl2D.SharedContext = null;
            glControl2D.Size = new Size(925, 785);
            glControl2D.TabIndex = 0;
            glControl2D.Visible = false;
            glControl2D.Load += glControl2D_Load;
            glControl2D.Paint += glControl2D_Paint;
            glControl2D.MouseDown += glControl2D_MouseDown;
            glControl2D.MouseMove += glControl2D_MouseMove;
            glControl2D.MouseUp += glControl2D_MouseUp;
            glControl2D.Resize += glControl2D_Resize;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(24, 24);
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsBtnSaveImage });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1219, 31);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            toolStrip1.ItemClicked += toolStrip1_ItemClicked;
            // 
            // tsBtnSaveImage
            // 
            tsBtnSaveImage.Image = (Image)resources.GetObject("tsBtnSaveImage.Image");
            tsBtnSaveImage.ImageTransparentColor = Color.Magenta;
            tsBtnSaveImage.Name = "tsBtnSaveImage";
            tsBtnSaveImage.Size = new Size(94, 28);
            tsBtnSaveImage.Tag = "SaveImage";
            tsBtnSaveImage.Text = "Save Image";
            // 
            // formsPlot1
            // 
            formsPlot1.DisplayScale = 1F;
            formsPlot1.Dock = DockStyle.Fill;
            formsPlot1.Location = new Point(3, 3);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(253, 771);
            formsPlot1.TabIndex = 0;
            // 
            // AnalysisForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1219, 816);
            Controls.Add(splitContainer1);
            Controls.Add(toolStrip1);
            Name = "AnalysisForm";
            Text = "AnalysisForm";
            Load += AnalysisForm_Load;
            Shown += AnalysisForm_Shown;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            grpBox3DFrustum.ResumeLayout(false);
            grpBox3DFrustum.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numNear).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFov).EndInit();
            ((System.ComponentModel.ISupportInitialize)numViewX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFar).EndInit();
            ((System.ComponentModel.ISupportInitialize)numViewY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numViewZ).EndInit();
            grpBox3DMoving.ResumeLayout(false);
            grpBox3DMoving.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numScaleX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numScaleY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numScaleZ).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTranX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTranY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTranZ).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRotateX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRotateY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRotateZ).EndInit();
            grpBoxSignal.ResumeLayout(false);
            grpBoxSignal.PerformLayout();
            grpBoxDisplay.ResumeLayout(false);
            grpBoxDisplay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numBottom).EndInit();
            ((System.ComponentModel.ISupportInitialize)numAttDb).EndInit();
            ((System.ComponentModel.ISupportInitialize)numAlpha).EndInit();
            ((System.ComponentModel.ISupportInitialize)numThreshold).EndInit();
            ((System.ComponentModel.ISupportInitialize)numIntensity).EndInit();
            grpBoxColor.ResumeLayout(false);
            grpBoxColor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numR).EndInit();
            ((System.ComponentModel.ISupportInitialize)numB).EndInit();
            ((System.ComponentModel.ISupportInitialize)numG).EndInit();
            tabPage2.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainer1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private OpenTK.GLControl.GLControl glControl2D;
        private GroupBox grpBoxDisplay;
        private Button btnScaleSetting;
        private Label label15;
        private Label label14;
        private Label label18;
        private Label label13;
        private Label label19;
        private GroupBox grpBoxColor;
        private Button btnChooseColor;
        private Label lblBackColor;
        private Label label17;
        private Label label16;
        private Label label27;
        private Label label28;
        private Label label26;
        private GroupBox grpBox3DMoving;
        private Label label42;
        private Label label43;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private GroupBox grpBox3DFrustum;
        private Label label20;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private CheckBox chkHeaveCorrection;
        private CheckBox chkDrawDistScale;
        private CheckBox chkDrawDepthScale;
        private NumericUpDown numBottom;
        private NumericUpDown numAttDb;
        private NumericUpDown numAlpha;
        private NumericUpDown numThreshold;
        private NumericUpDown numIntensity;
        private CheckBox chkInvert;
        private ComboBox cmbColor;
        private NumericUpDown numR;
        private NumericUpDown numB;
        private NumericUpDown numG;
        private NumericUpDown numScaleY;
        private NumericUpDown numScaleZ;
        private NumericUpDown numTranX;
        private NumericUpDown numTranY;
        private NumericUpDown numTranZ;
        private NumericUpDown numRotateX;
        private NumericUpDown numRotateY;
        private NumericUpDown numRotateZ;
        private NumericUpDown numNear;
        private NumericUpDown numFov;
        private NumericUpDown numViewX;
        private NumericUpDown numFar;
        private NumericUpDown numViewY;
        private NumericUpDown numViewZ;
        private ColorDialog colorDialog1;
        private Label label41;
        private NumericUpDown numScaleX;
        private GroupBox grpBoxSignal;
        private Label lblDemodulate;
        private Label label12;
        private Label lblLPF;
        private Label lblHPF;
        private Label label23;
        private Label label22;
        private Button btnSignalProcessing;
        private ToolStrip toolStrip1;
        private ToolStripButton tsBtnSaveImage;
        private CheckBox chkFlipX;
        private TabPage tabPage3;
        private TabPage tabPage5;
        private ScottPlot.WinForms.FormsPlot formsPlot1;
    }
}
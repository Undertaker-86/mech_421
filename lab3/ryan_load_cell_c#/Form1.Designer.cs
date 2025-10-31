namespace ryan_load_cell_c_
{
    partial class Form1
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.panelTop = new System.Windows.Forms.Panel();
            this.grpCalibration = new System.Windows.Forms.GroupBox();
            this.btnClearCalibration = new System.Windows.Forms.Button();
            this.btnAddCalibrationPoint = new System.Windows.Forms.Button();
            this.labelSlope = new System.Windows.Forms.Label();
            this.txtSlope = new System.Windows.Forms.TextBox();
            this.labelIntercept = new System.Windows.Forms.Label();
            this.txtIntercept = new System.Windows.Forms.TextBox();
            this.labelKnownMass = new System.Windows.Forms.Label();
            this.numKnownMass = new System.Windows.Forms.NumericUpDown();
            this.lstCalibration = new System.Windows.Forms.ListBox();
            this.grpProcessing = new System.Windows.Forms.GroupBox();
            this.btnResetTare = new System.Windows.Forms.Button();
            this.labelStdDev = new System.Windows.Forms.Label();
            this.txtStdDev = new System.Windows.Forms.TextBox();
            this.lblStabilityStatus = new System.Windows.Forms.Label();
            this.labelStabilityThreshold = new System.Windows.Forms.Label();
            this.numStabilityThreshold = new System.Windows.Forms.NumericUpDown();
            this.btnTare = new System.Windows.Forms.Button();
            this.labelTare = new System.Windows.Forms.Label();
            this.txtTare = new System.Windows.Forms.TextBox();
            this.labelAverageWindow = new System.Windows.Forms.Label();
            this.numAverageWindow = new System.Windows.Forms.NumericUpDown();
            this.grpLatest = new System.Windows.Forms.GroupBox();
            this.labelAverageRaw = new System.Windows.Forms.Label();
            this.txtAverageRaw = new System.Windows.Forms.TextBox();
            this.labelAverageWeight = new System.Windows.Forms.Label();
            this.txtAverageWeight = new System.Windows.Forms.TextBox();
            this.labelWeight = new System.Windows.Forms.Label();
            this.txtWeight = new System.Windows.Forms.TextBox();
            this.labelVoltage = new System.Windows.Forms.Label();
            this.txtVoltage = new System.Windows.Forms.TextBox();
            this.labelRawAdc = new System.Windows.Forms.Label();
            this.txtRawAdc = new System.Windows.Forms.TextBox();
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnRefreshPorts = new System.Windows.Forms.Button();
            this.labelBaudRate = new System.Windows.Forms.Label();
            this.txtBaudRate = new System.Windows.Forms.TextBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.comboPorts = new System.Windows.Forms.ComboBox();
            this.chartData = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panelTop.SuspendLayout();
            this.grpCalibration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKnownMass)).BeginInit();
            this.grpProcessing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStabilityThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAverageWindow)).BeginInit();
            this.grpLatest.SuspendLayout();
            this.grpConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartData)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.grpCalibration);
            this.panelTop.Controls.Add(this.grpProcessing);
            this.panelTop.Controls.Add(this.grpLatest);
            this.panelTop.Controls.Add(this.grpConnection);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1120, 230);
            this.panelTop.TabIndex = 0;
            // 
            // grpCalibration
            // 
            this.grpCalibration.Controls.Add(this.btnClearCalibration);
            this.grpCalibration.Controls.Add(this.btnAddCalibrationPoint);
            this.grpCalibration.Controls.Add(this.labelSlope);
            this.grpCalibration.Controls.Add(this.txtSlope);
            this.grpCalibration.Controls.Add(this.labelIntercept);
            this.grpCalibration.Controls.Add(this.txtIntercept);
            this.grpCalibration.Controls.Add(this.labelKnownMass);
            this.grpCalibration.Controls.Add(this.numKnownMass);
            this.grpCalibration.Controls.Add(this.lstCalibration);
            this.grpCalibration.Location = new System.Drawing.Point(704, 14);
            this.grpCalibration.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpCalibration.Name = "grpCalibration";
            this.grpCalibration.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpCalibration.Size = new System.Drawing.Size(404, 205);
            this.grpCalibration.TabIndex = 3;
            this.grpCalibration.TabStop = false;
            this.grpCalibration.Text = "Calibration";
            // 
            // btnClearCalibration
            // 
            this.btnClearCalibration.Location = new System.Drawing.Point(274, 158);
            this.btnClearCalibration.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClearCalibration.Name = "btnClearCalibration";
            this.btnClearCalibration.Size = new System.Drawing.Size(116, 30);
            this.btnClearCalibration.TabIndex = 8;
            this.btnClearCalibration.Text = "Clear Points";
            this.btnClearCalibration.UseVisualStyleBackColor = true;
            this.btnClearCalibration.Click += new System.EventHandler(this.btnClearCalibration_Click);
            // 
            // btnAddCalibrationPoint
            // 
            this.btnAddCalibrationPoint.Location = new System.Drawing.Point(274, 121);
            this.btnAddCalibrationPoint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAddCalibrationPoint.Name = "btnAddCalibrationPoint";
            this.btnAddCalibrationPoint.Size = new System.Drawing.Size(116, 30);
            this.btnAddCalibrationPoint.TabIndex = 7;
            this.btnAddCalibrationPoint.Text = "Capture Point";
            this.btnAddCalibrationPoint.UseVisualStyleBackColor = true;
            this.btnAddCalibrationPoint.Click += new System.EventHandler(this.btnAddCalibrationPoint_Click);
            // 
            // labelSlope
            // 
            this.labelSlope.AutoSize = true;
            this.labelSlope.Location = new System.Drawing.Point(214, 26);
            this.labelSlope.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSlope.Name = "labelSlope";
            this.labelSlope.Size = new System.Drawing.Size(42, 15);
            this.labelSlope.TabIndex = 4;
            this.labelSlope.Text = "Slope:";
            // 
            // txtSlope
            // 
            this.txtSlope.Location = new System.Drawing.Point(274, 23);
            this.txtSlope.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSlope.Name = "txtSlope";
            this.txtSlope.ReadOnly = true;
            this.txtSlope.Size = new System.Drawing.Size(116, 23);
            this.txtSlope.TabIndex = 5;
            this.txtSlope.TabStop = false;
            // 
            // labelIntercept
            // 
            this.labelIntercept.AutoSize = true;
            this.labelIntercept.Location = new System.Drawing.Point(214, 59);
            this.labelIntercept.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelIntercept.Name = "labelIntercept";
            this.labelIntercept.Size = new System.Drawing.Size(60, 15);
            this.labelIntercept.TabIndex = 6;
            this.labelIntercept.Text = "Intercept:";
            // 
            // txtIntercept
            // 
            this.txtIntercept.Location = new System.Drawing.Point(274, 56);
            this.txtIntercept.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtIntercept.Name = "txtIntercept";
            this.txtIntercept.ReadOnly = true;
            this.txtIntercept.Size = new System.Drawing.Size(116, 23);
            this.txtIntercept.TabIndex = 7;
            this.txtIntercept.TabStop = false;
            // 
            // labelKnownMass
            // 
            this.labelKnownMass.AutoSize = true;
            this.labelKnownMass.Location = new System.Drawing.Point(214, 92);
            this.labelKnownMass.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelKnownMass.Name = "labelKnownMass";
            this.labelKnownMass.Size = new System.Drawing.Size(104, 15);
            this.labelKnownMass.TabIndex = 2;
            this.labelKnownMass.Text = "Known mass (kg):";
            // 
            // numKnownMass
            // 
            this.numKnownMass.DecimalPlaces = 3;
            this.numKnownMass.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numKnownMass.Location = new System.Drawing.Point(326, 90);
            this.numKnownMass.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numKnownMass.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            196608});
            this.numKnownMass.Name = "numKnownMass";
            this.numKnownMass.Size = new System.Drawing.Size(64, 23);
            this.numKnownMass.TabIndex = 6;
            this.numKnownMass.Value = new decimal(new int[] {
            500,
            0,
            0,
            196608});
            // 
            // lstCalibration
            // 
            this.lstCalibration.FormattingEnabled = true;
            this.lstCalibration.ItemHeight = 15;
            this.lstCalibration.Location = new System.Drawing.Point(16, 23);
            this.lstCalibration.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lstCalibration.Name = "lstCalibration";
            this.lstCalibration.Size = new System.Drawing.Size(186, 169);
            this.lstCalibration.TabIndex = 0;
            this.lstCalibration.TabStop = false;
            // 
            // grpProcessing
            // 
            this.grpProcessing.Controls.Add(this.btnResetTare);
            this.grpProcessing.Controls.Add(this.labelStdDev);
            this.grpProcessing.Controls.Add(this.txtStdDev);
            this.grpProcessing.Controls.Add(this.lblStabilityStatus);
            this.grpProcessing.Controls.Add(this.labelStabilityThreshold);
            this.grpProcessing.Controls.Add(this.numStabilityThreshold);
            this.grpProcessing.Controls.Add(this.btnTare);
            this.grpProcessing.Controls.Add(this.labelTare);
            this.grpProcessing.Controls.Add(this.txtTare);
            this.grpProcessing.Controls.Add(this.labelAverageWindow);
            this.grpProcessing.Controls.Add(this.numAverageWindow);
            this.grpProcessing.Location = new System.Drawing.Point(472, 14);
            this.grpProcessing.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpProcessing.Name = "grpProcessing";
            this.grpProcessing.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpProcessing.Size = new System.Drawing.Size(224, 205);
            this.grpProcessing.TabIndex = 2;
            this.grpProcessing.TabStop = false;
            this.grpProcessing.Text = "Processing";
            // 
            // btnResetTare
            // 
            this.btnResetTare.Location = new System.Drawing.Point(128, 122);
            this.btnResetTare.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnResetTare.Name = "btnResetTare";
            this.btnResetTare.Size = new System.Drawing.Size(76, 27);
            this.btnResetTare.TabIndex = 6;
            this.btnResetTare.Text = "Reset";
            this.btnResetTare.UseVisualStyleBackColor = true;
            this.btnResetTare.Click += new System.EventHandler(this.btnResetTare_Click);
            // 
            // labelStdDev
            // 
            this.labelStdDev.AutoSize = true;
            this.labelStdDev.Location = new System.Drawing.Point(10, 160);
            this.labelStdDev.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStdDev.Name = "labelStdDev";
            this.labelStdDev.Size = new System.Drawing.Size(99, 15);
            this.labelStdDev.TabIndex = 8;
            this.labelStdDev.Text = "Std dev (kg/0.5s):";
            // 
            // txtStdDev
            // 
            this.txtStdDev.Location = new System.Drawing.Point(128, 157);
            this.txtStdDev.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStdDev.Name = "txtStdDev";
            this.txtStdDev.ReadOnly = true;
            this.txtStdDev.Size = new System.Drawing.Size(76, 23);
            this.txtStdDev.TabIndex = 9;
            this.txtStdDev.TabStop = false;
            this.txtStdDev.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblStabilityStatus
            // 
            this.lblStabilityStatus.AutoSize = true;
            this.lblStabilityStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStabilityStatus.Location = new System.Drawing.Point(10, 183);
            this.lblStabilityStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStabilityStatus.Name = "lblStabilityStatus";
            this.lblStabilityStatus.Size = new System.Drawing.Size(84, 20);
            this.lblStabilityStatus.TabIndex = 10;
            this.lblStabilityStatus.Text = "Waiting...";
            // 
            // labelStabilityThreshold
            // 
            this.labelStabilityThreshold.AutoSize = true;
            this.labelStabilityThreshold.Location = new System.Drawing.Point(10, 95);
            this.labelStabilityThreshold.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStabilityThreshold.Name = "labelStabilityThreshold";
            this.labelStabilityThreshold.Size = new System.Drawing.Size(117, 15);
            this.labelStabilityThreshold.TabIndex = 4;
            this.labelStabilityThreshold.Text = "Stability threshold (kg):";
            // 
            // numStabilityThreshold
            // 
            this.numStabilityThreshold.DecimalPlaces = 3;
            this.numStabilityThreshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numStabilityThreshold.Location = new System.Drawing.Point(128, 93);
            this.numStabilityThreshold.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numStabilityThreshold.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numStabilityThreshold.Name = "numStabilityThreshold";
            this.numStabilityThreshold.Size = new System.Drawing.Size(76, 23);
            this.numStabilityThreshold.TabIndex = 5;
            this.numStabilityThreshold.Value = new decimal(new int[] {
            20,
            0,
            0,
            196608});
            this.numStabilityThreshold.ValueChanged += new System.EventHandler(this.numStabilityThreshold_ValueChanged);
            // 
            // btnTare
            // 
            this.btnTare.Location = new System.Drawing.Point(10, 122);
            this.btnTare.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnTare.Name = "btnTare";
            this.btnTare.Size = new System.Drawing.Size(104, 27);
            this.btnTare.TabIndex = 5;
            this.btnTare.Text = "Tare";
            this.btnTare.UseVisualStyleBackColor = true;
            this.btnTare.Click += new System.EventHandler(this.btnTare_Click);
            // 
            // labelTare
            // 
            this.labelTare.AutoSize = true;
            this.labelTare.Location = new System.Drawing.Point(10, 64);
            this.labelTare.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTare.Name = "labelTare";
            this.labelTare.Size = new System.Drawing.Size(66, 15);
            this.labelTare.TabIndex = 2;
            this.labelTare.Text = "Tare (kg):";
            // 
            // txtTare
            // 
            this.txtTare.Location = new System.Drawing.Point(128, 61);
            this.txtTare.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtTare.Name = "txtTare";
            this.txtTare.ReadOnly = true;
            this.txtTare.Size = new System.Drawing.Size(76, 23);
            this.txtTare.TabIndex = 3;
            this.txtTare.TabStop = false;
            this.txtTare.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelAverageWindow
            // 
            this.labelAverageWindow.AutoSize = true;
            this.labelAverageWindow.Location = new System.Drawing.Point(10, 30);
            this.labelAverageWindow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAverageWindow.Name = "labelAverageWindow";
            this.labelAverageWindow.Size = new System.Drawing.Size(111, 15);
            this.labelAverageWindow.TabIndex = 0;
            this.labelAverageWindow.Text = "Rolling avg (samples):";
            // 
            // numAverageWindow
            // 
            this.numAverageWindow.Location = new System.Drawing.Point(128, 28);
            this.numAverageWindow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numAverageWindow.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numAverageWindow.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAverageWindow.Name = "numAverageWindow";
            this.numAverageWindow.Size = new System.Drawing.Size(76, 23);
            this.numAverageWindow.TabIndex = 1;
            this.numAverageWindow.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numAverageWindow.ValueChanged += new System.EventHandler(this.numAverageWindow_ValueChanged);
            // 
            // grpLatest
            // 
            this.grpLatest.Controls.Add(this.labelAverageRaw);
            this.grpLatest.Controls.Add(this.txtAverageRaw);
            this.grpLatest.Controls.Add(this.labelAverageWeight);
            this.grpLatest.Controls.Add(this.txtAverageWeight);
            this.grpLatest.Controls.Add(this.labelWeight);
            this.grpLatest.Controls.Add(this.txtWeight);
            this.grpLatest.Controls.Add(this.labelVoltage);
            this.grpLatest.Controls.Add(this.txtVoltage);
            this.grpLatest.Controls.Add(this.labelRawAdc);
            this.grpLatest.Controls.Add(this.txtRawAdc);
            this.grpLatest.Location = new System.Drawing.Point(224, 14);
            this.grpLatest.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpLatest.Name = "grpLatest";
            this.grpLatest.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpLatest.Size = new System.Drawing.Size(240, 205);
            this.grpLatest.TabIndex = 1;
            this.grpLatest.TabStop = false;
            this.grpLatest.Text = "Latest sample";
            // 
            // labelAverageRaw
            // 
            this.labelAverageRaw.AutoSize = true;
            this.labelAverageRaw.Location = new System.Drawing.Point(16, 62);
            this.labelAverageRaw.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAverageRaw.Name = "labelAverageRaw";
            this.labelAverageRaw.Size = new System.Drawing.Size(85, 15);
            this.labelAverageRaw.TabIndex = 2;
            this.labelAverageRaw.Text = "Avg ADC (raw):";
            // 
            // txtAverageRaw
            // 
            this.txtAverageRaw.Location = new System.Drawing.Point(128, 58);
            this.txtAverageRaw.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtAverageRaw.Name = "txtAverageRaw";
            this.txtAverageRaw.ReadOnly = true;
            this.txtAverageRaw.Size = new System.Drawing.Size(92, 23);
            this.txtAverageRaw.TabIndex = 3;
            this.txtAverageRaw.TabStop = false;
            this.txtAverageRaw.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelAverageWeight
            // 
            this.labelAverageWeight.AutoSize = true;
            this.labelAverageWeight.Location = new System.Drawing.Point(16, 154);
            this.labelAverageWeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAverageWeight.Name = "labelAverageWeight";
            this.labelAverageWeight.Size = new System.Drawing.Size(103, 15);
            this.labelAverageWeight.TabIndex = 8;
            this.labelAverageWeight.Text = "Avg weight (kg):";
            // 
            // txtAverageWeight
            // 
            this.txtAverageWeight.Location = new System.Drawing.Point(128, 150);
            this.txtAverageWeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtAverageWeight.Name = "txtAverageWeight";
            this.txtAverageWeight.ReadOnly = true;
            this.txtAverageWeight.Size = new System.Drawing.Size(92, 23);
            this.txtAverageWeight.TabIndex = 9;
            this.txtAverageWeight.TabStop = false;
            this.txtAverageWeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelWeight
            // 
            this.labelWeight.AutoSize = true;
            this.labelWeight.Location = new System.Drawing.Point(16, 122);
            this.labelWeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelWeight.Name = "labelWeight";
            this.labelWeight.Size = new System.Drawing.Size(77, 15);
            this.labelWeight.TabIndex = 6;
            this.labelWeight.Text = "Weight (kg):";
            // 
            // txtWeight
            // 
            this.txtWeight.Location = new System.Drawing.Point(128, 118);
            this.txtWeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtWeight.Name = "txtWeight";
            this.txtWeight.ReadOnly = true;
            this.txtWeight.Size = new System.Drawing.Size(92, 23);
            this.txtWeight.TabIndex = 7;
            this.txtWeight.TabStop = false;
            this.txtWeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelVoltage
            // 
            this.labelVoltage.AutoSize = true;
            this.labelVoltage.Location = new System.Drawing.Point(16, 93);
            this.labelVoltage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelVoltage.Name = "labelVoltage";
            this.labelVoltage.Size = new System.Drawing.Size(71, 15);
            this.labelVoltage.TabIndex = 4;
            this.labelVoltage.Text = "Voltage (V):";
            // 
            // txtVoltage
            // 
            this.txtVoltage.Location = new System.Drawing.Point(128, 89);
            this.txtVoltage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtVoltage.Name = "txtVoltage";
            this.txtVoltage.ReadOnly = true;
            this.txtVoltage.Size = new System.Drawing.Size(92, 23);
            this.txtVoltage.TabIndex = 5;
            this.txtVoltage.TabStop = false;
            this.txtVoltage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelRawAdc
            // 
            this.labelRawAdc.AutoSize = true;
            this.labelRawAdc.Location = new System.Drawing.Point(16, 32);
            this.labelRawAdc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelRawAdc.Name = "labelRawAdc";
            this.labelRawAdc.Size = new System.Drawing.Size(107, 15);
            this.labelRawAdc.TabIndex = 0;
            this.labelRawAdc.Text = "Raw ADC (0-1023):";
            // 
            // txtRawAdc
            // 
            this.txtRawAdc.Location = new System.Drawing.Point(128, 28);
            this.txtRawAdc.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtRawAdc.Name = "txtRawAdc";
            this.txtRawAdc.ReadOnly = true;
            this.txtRawAdc.Size = new System.Drawing.Size(92, 23);
            this.txtRawAdc.TabIndex = 1;
            this.txtRawAdc.TabStop = false;
            this.txtRawAdc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.lblConnectionStatus);
            this.grpConnection.Controls.Add(this.btnConnect);
            this.grpConnection.Controls.Add(this.btnRefreshPorts);
            this.grpConnection.Controls.Add(this.labelBaudRate);
            this.grpConnection.Controls.Add(this.txtBaudRate);
            this.grpConnection.Controls.Add(this.labelPort);
            this.grpConnection.Controls.Add(this.comboPorts);
            this.grpConnection.Location = new System.Drawing.Point(16, 14);
            this.grpConnection.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpConnection.Size = new System.Drawing.Size(200, 205);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Serial connection";
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Location = new System.Drawing.Point(10, 173);
            this.lblConnectionStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(87, 15);
            this.lblConnectionStatus.TabIndex = 6;
            this.lblConnectionStatus.Text = "Disconnected";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(10, 135);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(176, 31);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnRefreshPorts
            // 
            this.btnRefreshPorts.Location = new System.Drawing.Point(10, 98);
            this.btnRefreshPorts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRefreshPorts.Name = "btnRefreshPorts";
            this.btnRefreshPorts.Size = new System.Drawing.Size(176, 27);
            this.btnRefreshPorts.TabIndex = 4;
            this.btnRefreshPorts.Text = "Refresh Ports";
            this.btnRefreshPorts.UseVisualStyleBackColor = true;
            this.btnRefreshPorts.Click += new System.EventHandler(this.btnRefreshPorts_Click);
            // 
            // labelBaudRate
            // 
            this.labelBaudRate.AutoSize = true;
            this.labelBaudRate.Location = new System.Drawing.Point(10, 66);
            this.labelBaudRate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelBaudRate.Name = "labelBaudRate";
            this.labelBaudRate.Size = new System.Drawing.Size(63, 15);
            this.labelBaudRate.TabIndex = 2;
            this.labelBaudRate.Text = "Baud rate:";
            // 
            // txtBaudRate
            // 
            this.txtBaudRate.Location = new System.Drawing.Point(100, 63);
            this.txtBaudRate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtBaudRate.Name = "txtBaudRate";
            this.txtBaudRate.ReadOnly = true;
            this.txtBaudRate.Size = new System.Drawing.Size(86, 23);
            this.txtBaudRate.TabIndex = 3;
            this.txtBaudRate.TabStop = false;
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(10, 33);
            this.labelPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(31, 15);
            this.labelPort.TabIndex = 0;
            this.labelPort.Text = "Port:";
            // 
            // comboPorts
            // 
            this.comboPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPorts.FormattingEnabled = true;
            this.comboPorts.Location = new System.Drawing.Point(60, 30);
            this.comboPorts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboPorts.Name = "comboPorts";
            this.comboPorts.Size = new System.Drawing.Size(126, 23);
            this.comboPorts.TabIndex = 1;
            // 
            // chartData
            // 
            chartArea1.AxisX.LabelStyle.Format = "HH:mm:ss";
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gainsboro;
            chartArea1.AxisX.Title = "Time";
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gainsboro;
            chartArea1.AxisY.Title = "Weight (kg)";
            chartArea1.Name = "ChartArea1";
            this.chartData.ChartAreas.Add(chartArea1);
            this.chartData.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartData.Legends.Add(legend1);
            this.chartData.Location = new System.Drawing.Point(0, 230);
            this.chartData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chartData.Name = "chartData";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Legend = "Legend1";
            series1.Name = "Weight";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Legend = "Legend1";
            series2.Name = "AvgWeight";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            this.chartData.Series.Add(series1);
            this.chartData.Series.Add(series2);
            this.chartData.Size = new System.Drawing.Size(1120, 470);
            this.chartData.TabIndex = 1;
            this.chartData.Text = "chart1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1120, 700);
            this.Controls.Add(this.chartData);
            this.Controls.Add(this.panelTop);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Load Cell Scale";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelTop.ResumeLayout(false);
            this.grpCalibration.ResumeLayout(false);
            this.grpCalibration.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKnownMass)).EndInit();
            this.grpProcessing.ResumeLayout(false);
            this.grpProcessing.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStabilityThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAverageWindow)).EndInit();
            this.grpLatest.ResumeLayout(false);
            this.grpLatest.PerformLayout();
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            // 
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.GroupBox grpCalibration;
        private System.Windows.Forms.Button btnClearCalibration;
        private System.Windows.Forms.Button btnAddCalibrationPoint;
        private System.Windows.Forms.Label labelSlope;
        private System.Windows.Forms.TextBox txtSlope;
        private System.Windows.Forms.Label labelIntercept;
        private System.Windows.Forms.TextBox txtIntercept;
        private System.Windows.Forms.Label labelKnownMass;
        private System.Windows.Forms.NumericUpDown numKnownMass;
        private System.Windows.Forms.ListBox lstCalibration;
        private System.Windows.Forms.GroupBox grpProcessing;
        private System.Windows.Forms.Button btnResetTare;
        private System.Windows.Forms.Label labelStdDev;
        private System.Windows.Forms.TextBox txtStdDev;
        private System.Windows.Forms.Label lblStabilityStatus;
        private System.Windows.Forms.Label labelStabilityThreshold;
        private System.Windows.Forms.NumericUpDown numStabilityThreshold;
        private System.Windows.Forms.Button btnTare;
        private System.Windows.Forms.Label labelTare;
        private System.Windows.Forms.TextBox txtTare;
        private System.Windows.Forms.Label labelAverageWindow;
        private System.Windows.Forms.NumericUpDown numAverageWindow;
        private System.Windows.Forms.GroupBox grpLatest;
        private System.Windows.Forms.Label labelAverageWeight;
        private System.Windows.Forms.TextBox txtAverageWeight;
        private System.Windows.Forms.Label labelWeight;
        private System.Windows.Forms.TextBox txtWeight;
        private System.Windows.Forms.Label labelVoltage;
        private System.Windows.Forms.TextBox txtVoltage;
        private System.Windows.Forms.Label labelRawAdc;
        private System.Windows.Forms.TextBox txtRawAdc;
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnRefreshPorts;
        private System.Windows.Forms.Label labelBaudRate;
        private System.Windows.Forms.TextBox txtBaudRate;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.ComboBox comboPorts;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartData;
        private System.Windows.Forms.Label labelAverageRaw;
        private System.Windows.Forms.TextBox txtAverageRaw;
    }
}

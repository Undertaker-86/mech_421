using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RyanSensorApp.Models;
using RyanSensorApp.Services;
using ScottPlot.WinForms;

namespace RyanSensorApp
{
    public partial class MainForm : Form
    {
        private SerialPortService _serialPort;
        private DataLogger _dataLogger;
        private CalibrationService _calibrationService;
        private NoiseAnalysisService _noiseAnalysisService;
        private CalibrationData? _currentCalibration;
        private System.Windows.Forms.Timer _chartUpdateTimer;
        private bool _isLogging = false;
        private int _currentAdcValue = 0;
        
        // Noise analysis
        private bool _isNoiseTestRunning = false;
        private NoiseTestResult? _currentNoiseTest;
        private DateTime _noiseTestStartTime;
        private System.Windows.Forms.Timer _noiseTestTimer;

        // Main UI
        private TabControl mainTabControl;
        
        // Monitoring Tab Controls
        private Panel monitorLeftPanel;
        private Panel monitorRightPanel;
        private GroupBox connectionGroup;
        private GroupBox readingsGroup;
        private GroupBox controlsGroup;
        private ComboBox cmbPortName;
        private ComboBox cmbBaudRate;
        private Button btnConnect;
        private Button btnDisconnect;
        private Label lblStatus;
        private Label lblConnectionIndicator;
        private Label lblAdcValue;
        private Label lblVoltage;
        private Label lblDistance;
        private Label lblRangeStatus;
        private Button btnStartLogging;
        private Button btnStopLogging;
        private Button btnExport;
        private Label lblSampleCount;
        private FormsPlot plotAdc;
        private FormsPlot plotVoltage;
        private FormsPlot plotDistance;
        private Label lblChart1Title;
        private Label lblChart2Title;
        private Label lblChart3Title;

        // Calibration Tab Controls
        private Panel calibLeftPanel;
        private Panel calibCenterPanel;
        private Panel calibRightPanel;
        private GroupBox calibPointsGroup;
        private GroupBox calibPointsListGroup;
        private GroupBox calibRangeGroup;
        private GroupBox calibFitGroup;
        private GroupBox calibVisualizationGroup;
        private NumericUpDown numDistance;
        private Button btnCapturePoint;
        private Button btnClearPoints;
        private DataGridView dgvCalibrationPoints;
        private NumericUpDown numMinAdcThreshold;
        private NumericUpDown numMaxAdcThreshold;
        private Button btnApplyThresholds;
        private Button btnPresetVeryClose;
        private Button btnPresetClose;
        private Button btnPresetExtended;
        private Label lblCurrentAdcIndicator;
        private ProgressBar pbAdcIndicator;
        private Label lblThresholdStatus;
        private ComboBox cmbFitType;
        private Button btnFitCurve;
        private Label lblEquation;
        private Label lblRSquared;
        private Button btnSaveCalib;
        private Button btnLoadCalib;
        private Label lblPointCount;
        private Label lblAdcRange;
        private FormsPlot plotCalibration;

        // Noise Analysis Tab Controls
        private Panel noiseLeftPanel;
        private Panel noiseCenterPanel;
        private Panel noiseRightPanel;
        private GroupBox noiseTestSetupGroup;
        private GroupBox noiseTestControlGroup;
        private GroupBox noiseTestResultsGroup;
        private GroupBox noiseVisualizationGroup;
        private GroupBox noiseHistoryGroup;
        private GroupBox noiseComparisonGroup;
        private ComboBox cmbTestPosition;
        private NumericUpDown numTestDuration;
        private Label lblTestStatus;
        private Label lblTestTimer;
        private ProgressBar pbTestProgress;
        private Button btnStartTest;
        private Button btnStopTest;
        private Label lblCurrentMean;
        private Label lblCurrentStdDev;
        private Label lblCurrentSamples;
        private Label lblTestMean;
        private Label lblTestStdDev;
        private Label lblTestRange;
        private Label lblTestSamples;
        private FormsPlot plotNoiseTest;
        private DataGridView dgvNoiseHistory;
        private TextBox txtComparison;
        private Button btnExportNoiseTests;
        private Button btnClearNoiseTests;
        private FormsPlot plotNoiseComparison;

        // Data storage
        private List<double> _timeData = new List<double>();
        private List<double> _adcData = new List<double>();
        private List<double> _voltageData = new List<double>();
        private List<double> _distanceData = new List<double>();
        private DateTime _startTime;
        private List<CalibrationPoint> _calibrationPoints = new List<CalibrationPoint>();

        public MainForm()
        {
            InitializeComponent();
            InitializeServices();
            InitializeCharts();
            LoadAvailablePorts();
        }

        private void InitializeComponent()
        {
            this.Text = "Ryan's Distance Sensor Monitor";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245);

            // Create main tab control
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Padding = new Point(20, 8)
            };

            // Create tabs
            TabPage monitoringTab = new TabPage("📊 Monitoring");
            TabPage calibrationTab = new TabPage("⚙️ Calibration");
            TabPage noiseAnalysisTab = new TabPage("📈 Noise Analysis");

            CreateMonitoringTab(monitoringTab);
            CreateCalibrationTab(calibrationTab);
            CreateNoiseAnalysisTab(noiseAnalysisTab);

            mainTabControl.TabPages.Add(monitoringTab);
            mainTabControl.TabPages.Add(calibrationTab);
            mainTabControl.TabPages.Add(noiseAnalysisTab);

            this.Controls.Add(mainTabControl);

            // Timer for chart updates
            _chartUpdateTimer = new System.Windows.Forms.Timer();
            _chartUpdateTimer.Interval = 100;
            _chartUpdateTimer.Tick += ChartUpdateTimer_Tick;
        }

        private void CreateMonitoringTab(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(240, 240, 245);

            // Create left panel (controls)
            monitorLeftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 400,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            // Create right panel (charts)
            monitorRightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 240, 245),
                Padding = new Padding(15)
            };

            CreateMonitoringLeftPanel();
            CreateMonitoringRightPanel();

            tab.Controls.Add(monitorRightPanel);
            tab.Controls.Add(monitorLeftPanel);
        }

        private void CreateMonitoringLeftPanel()
        {
            int yPos = 10;

            // Connection Group
            connectionGroup = new GroupBox
            {
                Text = "CONNECTION",
                Location = new Point(10, yPos),
                Size = new Size(370, 140),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            lblConnectionIndicator = new Label
            {
                Location = new Point(15, 25),
                Size = new Size(20, 20),
                BackColor = Color.Red,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblStatus = new Label
            {
                Text = "Disconnected",
                Location = new Point(45, 25),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.Red
            };

            var lblPort = new Label { Text = "COM Port:", Location = new Point(15, 55), AutoSize = true };
            cmbPortName = new ComboBox { Location = new Point(90, 52), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblBaud = new Label { Text = "Baud:", Location = new Point(220, 55), AutoSize = true };
            cmbBaudRate = new ComboBox { Location = new Point(265, 52), Width = 90, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbBaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            cmbBaudRate.SelectedIndex = 0;

            btnConnect = new Button
            {
                Text = "Connect",
                Location = new Point(15, 90),
                Size = new Size(165, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnConnect.Click += BtnConnect_Click;

            btnDisconnect = new Button
            {
                Text = "Disconnect",
                Location = new Point(190, 90),
                Size = new Size(165, 35),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Enabled = false
            };
            btnDisconnect.Click += BtnDisconnect_Click;

            connectionGroup.Controls.AddRange(new Control[] {
                lblConnectionIndicator, lblStatus, lblPort, cmbPortName, lblBaud, cmbBaudRate, btnConnect, btnDisconnect
            });

            yPos += 150;

            // Readings Group
            readingsGroup = new GroupBox
            {
                Text = "CURRENT READINGS",
                Location = new Point(10, yPos),
                Size = new Size(370, 180),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            lblAdcValue = new Label
            {
                Text = "ADC: ---",
                Location = new Point(15, 30),
                Size = new Size(340, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 200)
            };

            lblVoltage = new Label
            {
                Text = "Voltage: --- V",
                Location = new Point(15, 60),
                Size = new Size(340, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 100, 0)
            };

            lblDistance = new Label
            {
                Text = "Distance: --- cm",
                Location = new Point(15, 90),
                Size = new Size(340, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 150, 0)
            };

            lblRangeStatus = new Label
            {
                Text = "● In Range",
                Location = new Point(15, 125),
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Green
            };

            readingsGroup.Controls.AddRange(new Control[] {
                lblAdcValue, lblVoltage, lblDistance, lblRangeStatus
            });

            yPos += 190;

            // Controls Group
            controlsGroup = new GroupBox
            {
                Text = "DATA LOGGING",
                Location = new Point(10, yPos),
                Size = new Size(370, 180),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnStartLogging = new Button
            {
                Text = "▶ Start Logging",
                Location = new Point(15, 30),
                Size = new Size(165, 40),
                BackColor = Color.FromArgb(0, 150, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnStartLogging.Click += BtnStartLogging_Click;

            btnStopLogging = new Button
            {
                Text = "■ Stop Logging",
                Location = new Point(190, 30),
                Size = new Size(165, 40),
                BackColor = Color.FromArgb(200, 0, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Enabled = false
            };
            btnStopLogging.Click += BtnStopLogging_Click;

            lblSampleCount = new Label
            {
                Text = "Samples: 0",
                Location = new Point(15, 80),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            btnExport = new Button
            {
                Text = "Export to CSV",
                Location = new Point(15, 105),
                Size = new Size(340, 35),
                BackColor = Color.FromArgb(50, 100, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExport.Click += BtnExport_Click;

            controlsGroup.Controls.AddRange(new Control[] {
                btnStartLogging, btnStopLogging, lblSampleCount, btnExport
            });

            monitorLeftPanel.Controls.AddRange(new Control[] {
                connectionGroup, readingsGroup, controlsGroup
            });
        }

        private void CreateMonitoringRightPanel()
        {
            // Chart titles
            lblChart1Title = new Label
            {
                Text = "ADC VALUE (0-1023)",
                Location = new Point(15, 10),
                Size = new Size(950, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 200)
            };

            plotAdc = new FormsPlot
            {
                Location = new Point(15, 40),
                Size = new Size(950, 230),
                BackColor = Color.White
            };

            lblChart2Title = new Label
            {
                Text = "VOLTAGE (0-3.3V)",
                Location = new Point(15, 280),
                Size = new Size(950, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 100, 0)
            };

            plotVoltage = new FormsPlot
            {
                Location = new Point(15, 310),
                Size = new Size(950, 230),
                BackColor = Color.White
            };

            lblChart3Title = new Label
            {
                Text = "DISTANCE (cm)",
                Location = new Point(15, 550),
                Size = new Size(950, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 150, 0)
            };

            plotDistance = new FormsPlot
            {
                Location = new Point(15, 580),
                Size = new Size(950, 230),
                BackColor = Color.White
            };

            monitorRightPanel.Controls.AddRange(new Control[] {
                lblChart1Title, plotAdc,
                lblChart2Title, plotVoltage,
                lblChart3Title, plotDistance
            });
        }

        private void CreateCalibrationTab(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(240, 240, 245);

            // Create three-panel layout
            calibLeftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 350,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            calibRightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 350,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            calibCenterPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 240, 245),
                Padding = new Padding(15)
            };

            CreateCalibrationLeftPanel();
            CreateCalibrationCenterPanel();
            CreateCalibrationRightPanel();

            tab.Controls.Add(calibCenterPanel);
            tab.Controls.Add(calibRightPanel);
            tab.Controls.Add(calibLeftPanel);
        }

        private void CreateCalibrationLeftPanel()
        {
            int yPos = 10;

            // Point Capture Group
            calibPointsGroup = new GroupBox
            {
                Text = "CAPTURE CALIBRATION POINTS",
                Location = new Point(10, yPos),
                Size = new Size(320, 180),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var lblDist = new Label
            {
                Text = "Distance (cm):",
                Location = new Point(15, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            numDistance = new NumericUpDown
            {
                Location = new Point(15, 55),
                Width = 140,
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 500,
                Value = 1.0M,
                Font = new Font("Segoe UI", 11)
            };

            var lblCurrentAdc = new Label
            {
                Text = "Current ADC:",
                Location = new Point(165, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            lblCurrentAdcIndicator = new Label
            {
                Text = "---",
                Location = new Point(165, 55),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 200),
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnCapturePoint = new Button
            {
                Text = "📍 Capture Point",
                Location = new Point(15, 95),
                Size = new Size(290, 40),
                BackColor = Color.FromArgb(0, 150, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCapturePoint.Click += BtnCapturePoint_Click;

            btnClearPoints = new Button
            {
                Text = "Clear All Points",
                Location = new Point(15, 140),
                Size = new Size(290, 30),
                BackColor = Color.FromArgb(150, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClearPoints.Click += BtnClearPoints_Click;

            calibPointsGroup.Controls.AddRange(new Control[] {
                lblDist, numDistance, lblCurrentAdc, lblCurrentAdcIndicator, btnCapturePoint, btnClearPoints
            });

            yPos += 190;

            // Fit Curve Group
            calibFitGroup = new GroupBox
            {
                Text = "CURVE FITTING",
                Location = new Point(10, yPos),
                Size = new Size(320, 200),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var lblFit = new Label
            {
                Text = "Fit Type:",
                Location = new Point(15, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            cmbFitType = new ComboBox
            {
                Location = new Point(15, 55),
                Width = 290,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFitType.Items.AddRange(new object[] { "Linear", "Polynomial (2nd)", "Polynomial (3rd)", "Power", "Inverse" });
            cmbFitType.SelectedIndex = 1;

            btnFitCurve = new Button
            {
                Text = "Fit Curve",
                Location = new Point(15, 90),
                Size = new Size(290, 35),
                BackColor = Color.FromArgb(100, 100, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnFitCurve.Click += BtnFitCurve_Click;

            lblEquation = new Label
            {
                Text = "Equation: ---",
                Location = new Point(15, 135),
                Size = new Size(290, 20),
                Font = new Font("Segoe UI", 8)
            };

            lblRSquared = new Label
            {
                Text = "R² = ---",
                Location = new Point(15, 160),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            calibFitGroup.Controls.AddRange(new Control[] {
                lblFit, cmbFitType, btnFitCurve, lblEquation, lblRSquared
            });

            yPos += 210;

            // Save/Load Group
            var saveLoadGroup = new GroupBox
            {
                Text = "CALIBRATION FILES",
                Location = new Point(10, yPos),
                Size = new Size(320, 100),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnSaveCalib = new Button
            {
                Text = "Save Calibration",
                Location = new Point(15, 30),
                Size = new Size(290, 30),
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveCalib.Click += BtnSaveCalibration_Click;

            btnLoadCalib = new Button
            {
                Text = "Load Calibration",
                Location = new Point(15, 65),
                Size = new Size(290, 30),
                BackColor = Color.FromArgb(120, 120, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadCalib.Click += BtnLoadCalibration_Click;

            saveLoadGroup.Controls.AddRange(new Control[] { btnSaveCalib, btnLoadCalib });

            calibLeftPanel.Controls.AddRange(new Control[] {
                calibPointsGroup, calibFitGroup, saveLoadGroup
            });
        }

        private void CreateCalibrationCenterPanel()
        {
            int yPos = 10;

            // Points List Group
            calibPointsListGroup = new GroupBox
            {
                Text = "CALIBRATION POINTS",
                Location = new Point(10, yPos),
                Size = new Size(660, 400),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            lblPointCount = new Label
            {
                Text = "Points: 0",
                Location = new Point(15, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            lblAdcRange = new Label
            {
                Text = "ADC Range: ---",
                Location = new Point(150, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            dgvCalibrationPoints = new DataGridView
            {
                Location = new Point(15, 55),
                Size = new Size(630, 330),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };

            dgvCalibrationPoints.Columns.Add("Point", "Point #");
            dgvCalibrationPoints.Columns.Add("Distance", "Distance (cm)");
            dgvCalibrationPoints.Columns.Add("ADC", "ADC Value");
            dgvCalibrationPoints.Columns["Point"].FillWeight = 20;
            dgvCalibrationPoints.Columns["Distance"].FillWeight = 40;
            dgvCalibrationPoints.Columns["ADC"].FillWeight = 40;

            var deleteButtonColumn = new DataGridViewButtonColumn
            {
                Name = "Delete",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                FillWeight = 20
            };
            dgvCalibrationPoints.Columns.Add(deleteButtonColumn);
            dgvCalibrationPoints.CellContentClick += DgvCalibrationPoints_CellContentClick;

            calibPointsListGroup.Controls.AddRange(new Control[] {
                lblPointCount, lblAdcRange, dgvCalibrationPoints
            });

            yPos += 410;

            // Visualization Group
            calibVisualizationGroup = new GroupBox
            {
                Text = "CALIBRATION CURVE",
                Location = new Point(10, yPos),
                Size = new Size(660, 380),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            plotCalibration = new FormsPlot
            {
                Location = new Point(15, 30),
                Size = new Size(630, 335),
                BackColor = Color.White
            };

            calibVisualizationGroup.Controls.Add(plotCalibration);

            calibCenterPanel.Controls.AddRange(new Control[] {
                calibPointsListGroup, calibVisualizationGroup
            });
        }

        private void CreateCalibrationRightPanel()
        {
            int yPos = 10;

            // Range Configuration Group
            calibRangeGroup = new GroupBox
            {
                Text = "RANGE CONFIGURATION",
                Location = new Point(10, yPos),
                Size = new Size(320, 420),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var lblInfo = new Label
            {
                Text = "Configure ADC thresholds to define\nwhen sensor is in range:",
                Location = new Point(15, 25),
                Size = new Size(290, 35),
                Font = new Font("Segoe UI", 9)
            };

            var lblMinThreshold = new Label
            {
                Text = "Too Far (Min ADC):",
                Location = new Point(15, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            numMinAdcThreshold = new NumericUpDown
            {
                Location = new Point(15, 95),
                Width = 290,
                Minimum = 0,
                Maximum = 1023,
                Value = 200,
                Font = new Font("Segoe UI", 11)
            };
            numMinAdcThreshold.ValueChanged += NumThreshold_ValueChanged;

            var lblMinDesc = new Label
            {
                Text = "ADC below this = Too Far",
                Location = new Point(15, 125),
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };

            var lblMaxThreshold = new Label
            {
                Text = "Too Close (Max ADC):",
                Location = new Point(15, 155),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            numMaxAdcThreshold = new NumericUpDown
            {
                Location = new Point(15, 180),
                Width = 290,
                Minimum = 0,
                Maximum = 1023,
                Value = 800,
                Font = new Font("Segoe UI", 11)
            };
            numMaxAdcThreshold.ValueChanged += NumThreshold_ValueChanged;

            var lblMaxDesc = new Label
            {
                Text = "ADC above this = Too Close",
                Location = new Point(15, 210),
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };

            var lblPresets = new Label
            {
                Text = "Quick Presets:",
                Location = new Point(15, 240),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnPresetVeryClose = new Button
            {
                Text = "Very Close (0.3-2cm)",
                Location = new Point(15, 265),
                Size = new Size(290, 30),
                BackColor = Color.FromArgb(80, 140, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnPresetVeryClose.Click += (s, e) => { numMinAdcThreshold.Value = 500; numMaxAdcThreshold.Value = 900; };

            btnPresetClose = new Button
            {
                Text = "Close (0.5-4cm)",
                Location = new Point(15, 300),
                Size = new Size(290, 30),
                BackColor = Color.FromArgb(80, 140, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnPresetClose.Click += (s, e) => { numMinAdcThreshold.Value = 300; numMaxAdcThreshold.Value = 850; };

            btnPresetExtended = new Button
            {
                Text = "Extended (1-6cm)",
                Location = new Point(15, 335),
                Size = new Size(290, 30),
                BackColor = Color.FromArgb(80, 140, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnPresetExtended.Click += (s, e) => { numMinAdcThreshold.Value = 100; numMaxAdcThreshold.Value = 900; };

            btnApplyThresholds = new Button
            {
                Text = "✓ Apply Thresholds",
                Location = new Point(15, 375),
                Size = new Size(290, 35),
                BackColor = Color.FromArgb(0, 150, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnApplyThresholds.Click += BtnApplyThresholds_Click;

            calibRangeGroup.Controls.AddRange(new Control[] {
                lblInfo, lblMinThreshold, numMinAdcThreshold, lblMinDesc,
                lblMaxThreshold, numMaxAdcThreshold, lblMaxDesc,
                lblPresets, btnPresetVeryClose, btnPresetClose, btnPresetExtended, btnApplyThresholds
            });

            yPos += 430;

            // Current Status Group
            var statusGroup = new GroupBox
            {
                Text = "CURRENT STATUS",
                Location = new Point(10, yPos),
                Size = new Size(320, 180),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var lblCurrentStatus = new Label
            {
                Text = "Current ADC Value:",
                Location = new Point(15, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            var lblCurrentAdcValue = new Label
            {
                Text = "---",
                Location = new Point(145, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            lblThresholdStatus = new Label
            {
                Text = "● Status: ---",
                Location = new Point(15, 55),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Gray
            };

            var lblIndicator = new Label
            {
                Text = "ADC Indicator:",
                Location = new Point(15, 90),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            pbAdcIndicator = new ProgressBar
            {
                Location = new Point(15, 115),
                Size = new Size(290, 25),
                Minimum = 0,
                Maximum = 1023,
                Value = 0
            };

            var lblIndicatorScale = new Label
            {
                Text = "0           Too Far          In Range          Too Close          1023",
                Location = new Point(15, 145),
                Size = new Size(290, 15),
                Font = new Font("Segoe UI", 7),
                ForeColor = Color.Gray
            };

            statusGroup.Controls.AddRange(new Control[] {
                lblCurrentStatus, lblCurrentAdcValue, lblThresholdStatus, lblIndicator, pbAdcIndicator, lblIndicatorScale
            });

            calibRightPanel.Controls.AddRange(new Control[] {
                calibRangeGroup, statusGroup
            });
        }

        private void InitializeServices()
        {
            _serialPort = new SerialPortService();
            _serialPort.AdcDataReceived += SerialPort_AdcDataReceived;
            _serialPort.ErrorOccurred += SerialPort_ErrorOccurred;
            _serialPort.ConnectionLost += SerialPort_ConnectionLost;

            _dataLogger = new DataLogger();
            _calibrationService = new CalibrationService();
            InitializeNoiseAnalysisService();
            _startTime = DateTime.Now;
        }

        private void InitializeCharts()
        {
            // Monitoring charts
            plotAdc.Plot.Title("");
            plotAdc.Plot.XLabel("Time (s)");
            plotAdc.Plot.YLabel("ADC Value");

            plotVoltage.Plot.Title("");
            plotVoltage.Plot.XLabel("Time (s)");
            plotVoltage.Plot.YLabel("Voltage (V)");

            plotDistance.Plot.Title("");
            plotDistance.Plot.XLabel("Time (s)");
            plotDistance.Plot.YLabel("Distance (cm)");

            // Calibration chart
            plotCalibration.Plot.Title("");
            plotCalibration.Plot.XLabel("ADC Value");
            plotCalibration.Plot.YLabel("Distance (cm)");
            UpdateCalibrationPlot();
        }

        private void LoadAvailablePorts()
        {
            cmbPortName.Items.Clear();
            string[] ports = SerialPortService.GetAvailablePorts();
            if (ports.Length > 0)
            {
                cmbPortName.Items.AddRange(ports);
                cmbPortName.SelectedIndex = 0;
            }
            else
            {
                cmbPortName.Items.Add("No ports");
                cmbPortName.SelectedIndex = 0;
            }
        }

        private void BtnConnect_Click(object? sender, EventArgs e)
        {
            if (cmbPortName.SelectedItem?.ToString() == "No ports")
            {
                MessageBox.Show("No COM ports available!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string portName = cmbPortName.SelectedItem!.ToString()!;
            int baudRate = int.Parse(cmbBaudRate.SelectedItem!.ToString()!);

            if (_serialPort.Connect(portName, baudRate))
            {
                lblStatus.Text = $"Connected to {portName}";
                lblStatus.ForeColor = Color.Green;
                lblConnectionIndicator.BackColor = Color.LimeGreen;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                cmbPortName.Enabled = false;
                cmbBaudRate.Enabled = false;
                _chartUpdateTimer.Start();
                _startTime = DateTime.Now;
            }
        }

        private void BtnDisconnect_Click(object? sender, EventArgs e)
        {
            _serialPort.Disconnect();
            lblStatus.Text = "Disconnected";
            lblStatus.ForeColor = Color.Red;
            lblConnectionIndicator.BackColor = Color.Red;
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
            cmbPortName.Enabled = true;
            cmbBaudRate.Enabled = true;
            _chartUpdateTimer.Stop();
        }

        private void SerialPort_AdcDataReceived(object? sender, AdcDataReceivedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SerialPort_AdcDataReceived(sender, e)));
                return;
            }

            _currentAdcValue = e.AdcValue;
            double voltage = _currentAdcValue * 3.3 / 1023.0;
            double distance = 0;
            bool isInRange = true;
            string rangeStatus = "In Range";

            if (_currentCalibration != null && _currentCalibration.Coefficients.Length > 0)
            {
                distance = _currentCalibration.ConvertAdcToDistance(_currentAdcValue);
                isInRange = _currentCalibration.IsInRange(_currentAdcValue);
                rangeStatus = _currentCalibration.GetRangeStatus(_currentAdcValue);
            }

            // Update monitoring tab
            lblAdcValue.Text = $"ADC: {_currentAdcValue} / 1023";
            lblVoltage.Text = $"Voltage: {voltage:F3} V";
            lblDistance.Text = $"Distance: {distance:F2} cm";

            if (rangeStatus == "In Range")
            {
                lblRangeStatus.Text = "● In Range";
                lblRangeStatus.ForeColor = Color.Green;
            }
            else if (rangeStatus == "Too Close")
            {
                lblRangeStatus.Text = "⚠ Too Close";
                lblRangeStatus.ForeColor = Color.OrangeRed;
            }
            else
            {
                lblRangeStatus.Text = "⚠ Too Far";
                lblRangeStatus.ForeColor = Color.Red;
            }

            // Update calibration tab
            lblCurrentAdcIndicator.Text = _currentAdcValue.ToString();
            pbAdcIndicator.Value = Math.Min(Math.Max(_currentAdcValue, 0), 1023);

            if (rangeStatus == "In Range")
            {
                lblThresholdStatus.Text = "● Status: In Range";
                lblThresholdStatus.ForeColor = Color.Green;
            }
            else if (rangeStatus == "Too Close")
            {
                lblThresholdStatus.Text = "⚠ Status: Too Close";
                lblThresholdStatus.ForeColor = Color.OrangeRed;
            }
            else
            {
                lblThresholdStatus.Text = "⚠ Status: Too Far";
                lblThresholdStatus.ForeColor = Color.Red;
            }

            double elapsedSeconds = (DateTime.Now - _startTime).TotalSeconds;
            _timeData.Add(elapsedSeconds);
            _adcData.Add(_currentAdcValue);
            _voltageData.Add(voltage);
            _distanceData.Add(distance);

            if (_timeData.Count > 300)
            {
                _timeData.RemoveAt(0);
                _adcData.RemoveAt(0);
                _voltageData.RemoveAt(0);
                _distanceData.RemoveAt(0);
            }

            if (_isLogging)
            {
                var reading = new SensorReading(_currentAdcValue, distance, isInRange);
                _dataLogger.AddReading(reading);
                lblSampleCount.Text = $"Samples: {_dataLogger.Count}";
            }
        }

        private void ChartUpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (_timeData.Count > 0)
            {
                plotAdc.Plot.Clear();
                var scatterAdc = plotAdc.Plot.Add.Scatter(_timeData.ToArray(), _adcData.ToArray());
                scatterAdc.Color = ScottPlot.Color.FromHex("#0064C8");
                scatterAdc.LineWidth = 2;
                plotAdc.Plot.Axes.AutoScale();
                plotAdc.Refresh();

                plotVoltage.Plot.Clear();
                var scatterVolt = plotVoltage.Plot.Add.Scatter(_timeData.ToArray(), _voltageData.ToArray());
                scatterVolt.Color = ScottPlot.Color.FromHex("#C86400");
                scatterVolt.LineWidth = 2;
                plotVoltage.Plot.Axes.AutoScale();
                plotVoltage.Refresh();

                plotDistance.Plot.Clear();
                var scatterDist = plotDistance.Plot.Add.Scatter(_timeData.ToArray(), _distanceData.ToArray());
                scatterDist.Color = ScottPlot.Color.FromHex("#009600");
                scatterDist.LineWidth = 2;
                plotDistance.Plot.Axes.AutoScale();
                plotDistance.Refresh();
            }
        }

        private void SerialPort_ErrorOccurred(object? sender, string e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SerialPort_ErrorOccurred(sender, e)));
                return;
            }
            MessageBox.Show(e, "Serial Port Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SerialPort_ConnectionLost(object? sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SerialPort_ConnectionLost(sender, e)));
                return;
            }
            BtnDisconnect_Click(null, EventArgs.Empty);
            MessageBox.Show("Connection lost!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void BtnCapturePoint_Click(object? sender, EventArgs e)
        {
            if (!_serialPort.IsConnected)
            {
                MessageBox.Show("Please connect to the sensor first!", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            double distance = (double)numDistance.Value;
            _calibrationPoints.Add(new CalibrationPoint(distance, _currentAdcValue));
            
            UpdateCalibrationPointsList();
            UpdateCalibrationPlot();
            
            MessageBox.Show($"Point captured!\nDistance: {distance:F2} cm\nADC: {_currentAdcValue}",
                "Point Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnClearPoints_Click(object? sender, EventArgs e)
        {
            _calibrationPoints.Clear();
            _currentCalibration = null;
            lblEquation.Text = "Equation: ---";
            lblRSquared.Text = "R² = ---";
            
            UpdateCalibrationPointsList();
            UpdateCalibrationPlot();
            
            MessageBox.Show("All calibration points cleared!", "Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DgvCalibrationPoints_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvCalibrationPoints.Columns["Delete"].Index)
            {
                var result = MessageBox.Show(
                    $"Delete point {e.RowIndex + 1}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _calibrationPoints.RemoveAt(e.RowIndex);
                    _currentCalibration = null;
                    lblEquation.Text = "Equation: ---";
                    lblRSquared.Text = "R² = ---";
                    
                    UpdateCalibrationPointsList();
                    UpdateCalibrationPlot();
                }
            }
        }

        private void UpdateCalibrationPointsList()
        {
            dgvCalibrationPoints.Rows.Clear();
            
            for (int i = 0; i < _calibrationPoints.Count; i++)
            {
                var point = _calibrationPoints[i];
                dgvCalibrationPoints.Rows.Add(
                    (i + 1).ToString(),
                    point.Distance.ToString("F2"),
                    point.AdcValue.ToString()
                );
            }

            lblPointCount.Text = $"Points: {_calibrationPoints.Count}";
            
            if (_calibrationPoints.Count > 0)
            {
                int minAdc = _calibrationPoints.Min(p => p.AdcValue);
                int maxAdc = _calibrationPoints.Max(p => p.AdcValue);
                lblAdcRange.Text = $"ADC Range: {minAdc} - {maxAdc}";
            }
            else
            {
                lblAdcRange.Text = "ADC Range: ---";
            }
        }

        private void UpdateCalibrationPlot()
        {
            plotCalibration.Plot.Clear();

            if (_calibrationPoints.Count > 0)
            {
                double[] adcValues = _calibrationPoints.Select(p => (double)p.AdcValue).ToArray();
                double[] distances = _calibrationPoints.Select(p => p.Distance).ToArray();

                var scatter = plotCalibration.Plot.Add.Scatter(adcValues, distances);
                scatter.Color = ScottPlot.Color.FromHex("#0064C8");
                scatter.MarkerSize = 10;
                scatter.LineWidth = 0;
                scatter.LegendText = "Calibration Points";

                // If we have a fitted curve, plot it
                if (_currentCalibration != null && _currentCalibration.Coefficients.Length > 0)
                {
                    int minAdc = (int)adcValues.Min();
                    int maxAdc = (int)adcValues.Max();
                    int range = maxAdc - minAdc;
                    minAdc = Math.Max(0, minAdc - range / 4);
                    maxAdc = Math.Min(1023, maxAdc + range / 4);

                    List<double> fitAdcValues = new List<double>();
                    List<double> fitDistances = new List<double>();

                    for (int adc = minAdc; adc <= maxAdc; adc += 2)
                    {
                        double dist = _currentCalibration.ConvertAdcToDistance(adc);
                        fitAdcValues.Add(adc);
                        fitDistances.Add(dist);
                    }

                    var fitLine = plotCalibration.Plot.Add.Scatter(fitAdcValues.ToArray(), fitDistances.ToArray());
                    fitLine.Color = ScottPlot.Color.FromHex("#FF6600");
                    fitLine.LineWidth = 2;
                    fitLine.MarkerSize = 0;
                    fitLine.LegendText = "Fitted Curve";

                    plotCalibration.Plot.Legend.IsVisible = true;
                }
            }

            plotCalibration.Plot.Axes.AutoScale();
            plotCalibration.Refresh();
        }

        private void BtnFitCurve_Click(object? sender, EventArgs e)
        {
            if (_calibrationPoints.Count < 2)
            {
                MessageBox.Show("Need at least 2 calibration points!", "Insufficient Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FitType fitType = cmbFitType.SelectedIndex switch
            {
                0 => FitType.Linear,
                1 => FitType.Polynomial2,
                2 => FitType.Polynomial3,
                3 => FitType.Power,
                4 => FitType.Inverse,
                _ => FitType.Polynomial2
            };

            try
            {
                _currentCalibration = _calibrationService.PerformCalibration(_calibrationPoints, fitType);
                
                // Apply current threshold settings
                _currentCalibration.MinAdcThreshold = (int)numMinAdcThreshold.Value;
                _currentCalibration.MaxAdcThreshold = (int)numMaxAdcThreshold.Value;
                
                lblEquation.Text = $"Equation: {_currentCalibration.Equation}";
                lblRSquared.Text = $"R² = {_currentCalibration.RSquared:F6}";
                
                UpdateCalibrationPlot();
                
                MessageBox.Show($"Calibration successful!\nPoints: {_calibrationPoints.Count}\nR² = {_currentCalibration.RSquared:F6}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Calibration failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnApplyThresholds_Click(object? sender, EventArgs e)
        {
            if (_currentCalibration != null)
            {
                _currentCalibration.MinAdcThreshold = (int)numMinAdcThreshold.Value;
                _currentCalibration.MaxAdcThreshold = (int)numMaxAdcThreshold.Value;
                MessageBox.Show("Thresholds applied to current calibration!", "Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Thresholds will be applied when you fit a curve.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void NumThreshold_ValueChanged(object? sender, EventArgs e)
        {
            // Ensure min < max
            if (numMinAdcThreshold.Value >= numMaxAdcThreshold.Value)
            {
                if (sender == numMinAdcThreshold)
                {
                    numMaxAdcThreshold.Value = numMinAdcThreshold.Value + 1;
                }
                else
                {
                    numMinAdcThreshold.Value = numMaxAdcThreshold.Value - 1;
                }
            }
        }

        private void BtnStartLogging_Click(object? sender, EventArgs e)
        {
            _isLogging = true;
            _dataLogger.Clear();
            btnStartLogging.Enabled = false;
            btnStopLogging.Enabled = true;
            lblSampleCount.Text = "Samples: 0";
        }

        private void BtnStopLogging_Click(object? sender, EventArgs e)
        {
            _isLogging = false;
            btnStartLogging.Enabled = true;
            btnStopLogging.Enabled = false;
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            if (_dataLogger.Count == 0)
            {
                MessageBox.Show("No data to export!", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files (*.csv)|*.csv";
                sfd.FileName = $"sensor_data_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _dataLogger.ExportToCsv(sfd.FileName);
                        MessageBox.Show($"Data exported successfully!\n{sfd.FileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnSaveCalibration_Click(object? sender, EventArgs e)
        {
            if (_currentCalibration == null)
            {
                MessageBox.Show("No calibration to save!", "No Calibration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "JSON Files (*.json)|*.json";
                sfd.FileName = $"calibration_{DateTime.Now:yyyyMMdd}.json";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _calibrationService.SaveCalibration(_currentCalibration, sfd.FileName);
                        MessageBox.Show("Calibration saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Save failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnLoadCalibration_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON Files (*.json)|*.json";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _currentCalibration = _calibrationService.LoadCalibration(ofd.FileName);
                        _calibrationPoints = new List<CalibrationPoint>(_currentCalibration.Points);
                        
                        lblEquation.Text = $"Equation: {_currentCalibration.Equation}";
                        lblRSquared.Text = $"R² = {_currentCalibration.RSquared:F6}";
                        
                        cmbFitType.SelectedIndex = _currentCalibration.FitType switch
                        {
                            FitType.Linear => 0,
                            FitType.Polynomial2 => 1,
                            FitType.Polynomial3 => 2,
                            FitType.Power => 3,
                            FitType.Inverse => 4,
                            _ => 1
                        };

                        // Load thresholds
                        numMinAdcThreshold.Value = _currentCalibration.MinAdcThreshold;
                        numMaxAdcThreshold.Value = _currentCalibration.MaxAdcThreshold;
                        
                        UpdateCalibrationPointsList();
                        UpdateCalibrationPlot();
                        
                        MessageBox.Show("Calibration loaded!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Load failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _chartUpdateTimer.Stop();
            _serialPort.Disconnect();
            _serialPort.Dispose();
            base.OnFormClosing(e);
        }
    }
}

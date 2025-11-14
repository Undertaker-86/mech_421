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
        private CalibrationData? _currentCalibration;
        private System.Windows.Forms.Timer _chartUpdateTimer;
        private bool _isLogging = false;
        private int _currentAdcValue = 0;

        // Left Panel Controls
        private Panel leftPanel;
        private GroupBox connectionGroup;
        private GroupBox readingsGroup;
        private GroupBox calibrationGroup;
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
        private NumericUpDown numDistance;
        private Button btnCapturePoint;
        private Button btnClearPoints;
        private ComboBox cmbFitType;
        private Button btnFitCurve;
        private Label lblEquation;
        private Label lblRSquared;
        private Button btnStartLogging;
        private Button btnStopLogging;
        private Button btnExport;
        private Button btnSaveCalib;
        private Button btnLoadCalib;
        private Label lblSampleCount;

        // Right Panel Controls
        private Panel rightPanel;
        private FormsPlot plotAdc;
        private FormsPlot plotVoltage;
        private FormsPlot plotDistance;
        private Label lblChart1Title;
        private Label lblChart2Title;
        private Label lblChart3Title;

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

            // Create left panel (controls)
            leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 400,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            // Create right panel (charts)
            rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 240, 245),
                Padding = new Padding(15)
            };

            CreateLeftPanelControls();
            CreateRightPanelControls();

            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);

            // Timer for chart updates
            _chartUpdateTimer = new System.Windows.Forms.Timer();
            _chartUpdateTimer.Interval = 100;
            _chartUpdateTimer.Tick += ChartUpdateTimer_Tick;
        }

        private void CreateLeftPanelControls()
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
                Size = new Size(370, 140),
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
                Location = new Point(240, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Green
            };

            readingsGroup.Controls.AddRange(new Control[] {
                lblAdcValue, lblVoltage, lblDistance, lblRangeStatus
            });

            yPos += 150;

            // Calibration Group
            calibrationGroup = new GroupBox
            {
                Text = "CALIBRATION",
                Location = new Point(10, yPos),
                Size = new Size(370, 240),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var lblDist = new Label
            {
                Text = "Enter Distance (cm):",
                Location = new Point(15, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            numDistance = new NumericUpDown
            {
                Location = new Point(15, 55),
                Width = 150,
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 500,
                Value = 10,
                Font = new Font("Segoe UI", 11)
            };

            btnCapturePoint = new Button
            {
                Text = "📍 Capture Point",
                Location = new Point(175, 50),
                Size = new Size(180, 35),
                BackColor = Color.FromArgb(0, 150, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCapturePoint.Click += BtnCapturePoint_Click;

            btnClearPoints = new Button
            {
                Text = "Clear All Points",
                Location = new Point(15, 95),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(150, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClearPoints.Click += BtnClearPoints_Click;

            var lblFit = new Label
            {
                Text = "Fit Type:",
                Location = new Point(15, 135),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            cmbFitType = new ComboBox
            {
                Location = new Point(75, 132),
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFitType.Items.AddRange(new object[] { "Linear", "Polynomial (2nd)", "Polynomial (3rd)", "Power", "Inverse" });
            cmbFitType.SelectedIndex = 1;

            btnFitCurve = new Button
            {
                Text = "Fit Curve",
                Location = new Point(265, 128),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(100, 100, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnFitCurve.Click += BtnFitCurve_Click;

            lblEquation = new Label
            {
                Text = "Equation: ---",
                Location = new Point(15, 170),
                Size = new Size(340, 20),
                Font = new Font("Segoe UI", 8)
            };

            lblRSquared = new Label
            {
                Text = "R² = ---",
                Location = new Point(15, 195),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            calibrationGroup.Controls.AddRange(new Control[] {
                lblDist, numDistance, btnCapturePoint, btnClearPoints,
                lblFit, cmbFitType, btnFitCurve, lblEquation, lblRSquared
            });

            yPos += 250;

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
                Size = new Size(165, 35),
                BackColor = Color.FromArgb(50, 100, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExport.Click += BtnExport_Click;

            btnSaveCalib = new Button
            {
                Text = "Save Calibration",
                Location = new Point(190, 105),
                Size = new Size(165, 35),
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveCalib.Click += BtnSaveCalibration_Click;

            btnLoadCalib = new Button
            {
                Text = "Load Calibration",
                Location = new Point(190, 145),
                Size = new Size(165, 25),
                BackColor = Color.FromArgb(120, 120, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8)
            };
            btnLoadCalib.Click += BtnLoadCalibration_Click;

            controlsGroup.Controls.AddRange(new Control[] {
                btnStartLogging, btnStopLogging, lblSampleCount, btnExport, btnSaveCalib, btnLoadCalib
            });

            leftPanel.Controls.AddRange(new Control[] {
                connectionGroup, readingsGroup, calibrationGroup, controlsGroup
            });
        }

        private void CreateRightPanelControls()
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

            rightPanel.Controls.AddRange(new Control[] {
                lblChart1Title, plotAdc,
                lblChart2Title, plotVoltage,
                lblChart3Title, plotDistance
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
            _startTime = DateTime.Now;
        }

        private void InitializeCharts()
        {
            plotAdc.Plot.Title("");
            plotAdc.Plot.XLabel("Time (s)");
            plotAdc.Plot.YLabel("ADC Value");

            plotVoltage.Plot.Title("");
            plotVoltage.Plot.XLabel("Time (s)");
            plotVoltage.Plot.YLabel("Voltage (V)");

            plotDistance.Plot.Title("");
            plotDistance.Plot.XLabel("Time (s)");
            plotDistance.Plot.YLabel("Distance (cm)");
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

            if (_currentCalibration != null && _currentCalibration.Coefficients.Length > 0)
            {
                distance = _currentCalibration.ConvertAdcToDistance(_currentAdcValue);
                isInRange = _currentCalibration.IsInRange(_currentAdcValue);
            }

            lblAdcValue.Text = $"ADC: {_currentAdcValue} / 1023";
            lblVoltage.Text = $"Voltage: {voltage:F3} V";
            lblDistance.Text = $"Distance: {distance:F2} cm";

            if (isInRange)
            {
                lblRangeStatus.Text = "● In Range";
                lblRangeStatus.ForeColor = Color.Green;
            }
            else
            {
                lblRangeStatus.Text = "⚠ Out of Range";
                lblRangeStatus.ForeColor = Color.Red;
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
            MessageBox.Show($"Point captured!\nDistance: {distance:F2} cm\nADC: {_currentAdcValue}",
                "Point Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnClearPoints_Click(object? sender, EventArgs e)
        {
            _calibrationPoints.Clear();
            _currentCalibration = null;
            lblEquation.Text = "Equation: ---";
            lblRSquared.Text = "R² = ---";
            MessageBox.Show("All calibration points cleared!", "Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                lblEquation.Text = $"Equation: {_currentCalibration.Equation}";
                lblRSquared.Text = $"R² = {_currentCalibration.RSquared:F6}";
                MessageBox.Show($"Calibration successful!\nPoints: {_calibrationPoints.Count}\nR² = {_currentCalibration.RSquared:F6}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Calibration failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

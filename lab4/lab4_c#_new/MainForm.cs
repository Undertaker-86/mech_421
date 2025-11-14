using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DistanceSensorApp.Models;
using DistanceSensorApp.Services;
using ScottPlot.WinForms;
using WinFormsLabel = System.Windows.Forms.Label;
using WinFormsColor = System.Drawing.Color;
using WinFormsFontStyle = System.Drawing.FontStyle;

namespace DistanceSensorApp
{
    public partial class MainForm : Form
    {
        private SerialPortService _serialPort;
        private DataLogger _dataLogger;
        private CalibrationService _calibrationService;
        private CalibrationData? _currentCalibration;
        private System.Windows.Forms.Timer _chartUpdateTimer;
        private bool _isLogging = false;

        // UI Controls - Main Tab
        private TabControl tabControl;
        private ComboBox cmbPortName;
        private ComboBox cmbBaudRate;
        private Button btnConnect;
        private Button btnDisconnect;
        private Label lblStatus;
        private Label lblAdcValue;
        private Label lblDistance;
        private Label lblRangeStatus;
        private FormsPlot plotAdc;
        private FormsPlot plotDistance;
        private Button btnStartLogging;
        private Button btnStopLogging;
        private Button btnExport;
        private Label lblSampleCount;

        // UI Controls - Calibration Tab
        private DataGridView dgvCalibration;
        private Button btnAddPoint;
        private Button btnRemovePoint;
        private Button btnClearPoints;
        private NumericUpDown numDistance;
        private NumericUpDown numAdcValue;
        private ComboBox cmbFitType;
        private Button btnFitCurve;
        private Label lblEquation;
        private Label lblRSquared;
        private FormsPlot plotCalibration;
        private Button btnSaveCalibration;
        private Button btnLoadCalibration;
        private Button btnApplyCalibration;
        private NumericUpDown numMinAdc;
        private NumericUpDown numMaxAdc;

        // Data storage for charts
        private List<double> _adcTimeData = new List<double>();
        private List<double> _adcValueData = new List<double>();
        private List<double> _distanceTimeData = new List<double>();
        private List<double> _distanceValueData = new List<double>();
        private DateTime _startTime;

        public MainForm()
        {
            InitializeComponent();
            InitializeServices();
            InitializeCharts();
            LoadAvailablePorts();
        }

        private void InitializeComponent()
        {
            this.Text = "Distance Sensor Monitor";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };

            // Create Main Tab
            var mainTab = new TabPage("Monitor");
            CreateMainTab(mainTab);
            tabControl.TabPages.Add(mainTab);

            // Create Calibration Tab
            var calibTab = new TabPage("Calibration");
            CreateCalibrationTab(calibTab);
            tabControl.TabPages.Add(calibTab);

            this.Controls.Add(tabControl);

            // Timer for chart updates
            _chartUpdateTimer = new System.Windows.Forms.Timer();
            _chartUpdateTimer.Interval = 100; // 100ms refresh
            _chartUpdateTimer.Tick += ChartUpdateTimer_Tick;
        }

        private void CreateMainTab(TabPage tab)
        {
            var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            // Connection Panel
            var connectionPanel = new GroupBox
            {
                Text = "Connection",
                Location = new Point(10, 10),
                Size = new Size(1160, 80)
            };

            var lblPort = new Label { Text = "COM Port:", Location = new Point(10, 25), AutoSize = true };
            cmbPortName = new ComboBox { Location = new Point(80, 22), Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            
            var lblBaud = new Label { Text = "Baud Rate:", Location = new Point(200, 25), AutoSize = true };
            cmbBaudRate = new ComboBox { Location = new Point(280, 22), Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbBaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            cmbBaudRate.SelectedIndex = 0;

            btnConnect = new Button { Text = "Connect", Location = new Point(400, 20), Size = new Size(80, 30) };
            btnConnect.Click += BtnConnect_Click;
            
            btnDisconnect = new Button { Text = "Disconnect", Location = new Point(490, 20), Size = new Size(90, 30), Enabled = false };
            btnDisconnect.Click += BtnDisconnect_Click;

            lblStatus = new Label { Text = "Status: Disconnected", Location = new Point(600, 25), AutoSize = true, ForeColor = Color.Red };

            connectionPanel.Controls.AddRange(new Control[] { lblPort, cmbPortName, lblBaud, cmbBaudRate, btnConnect, btnDisconnect, lblStatus });

            // Readings Panel
            var readingsPanel = new GroupBox
            {
                Text = "Current Readings",
                Location = new Point(10, 100),
                Size = new Size(1160, 80)
            };

            lblAdcValue = new Label { Text = "ADC Value: ---", Location = new Point(20, 30), Font = new Font("Arial", 12, FontStyle.Bold), AutoSize = true };
            lblDistance = new Label { Text = "Distance: --- cm", Location = new Point(300, 30), Font = new Font("Arial", 12, FontStyle.Bold), AutoSize = true };
            lblRangeStatus = new Label { Text = "● In Range", Location = new Point(600, 30), Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = Color.Green, AutoSize = true };

            readingsPanel.Controls.AddRange(new Control[] { lblAdcValue, lblDistance, lblRangeStatus });

            // Charts
            plotAdc = new FormsPlot { Location = new Point(10, 190), Size = new Size(1160, 220) };
            plotDistance = new FormsPlot { Location = new Point(10, 420), Size = new Size(1160, 220) };

            // Data Logging Panel
            var loggingPanel = new GroupBox
            {
                Text = "Data Logging",
                Location = new Point(10, 650),
                Size = new Size(1160, 80)
            };

            btnStartLogging = new Button { Text = "Start Logging", Location = new Point(20, 25), Size = new Size(100, 35) };
            btnStartLogging.Click += BtnStartLogging_Click;

            btnStopLogging = new Button { Text = "Stop Logging", Location = new Point(130, 25), Size = new Size(100, 35), Enabled = false };
            btnStopLogging.Click += BtnStopLogging_Click;

            btnExport = new Button { Text = "Export to CSV", Location = new Point(240, 25), Size = new Size(110, 35) };
            btnExport.Click += BtnExport_Click;

            lblSampleCount = new Label { Text = "Samples: 0", Location = new Point(370, 35), AutoSize = true };

            loggingPanel.Controls.AddRange(new Control[] { btnStartLogging, btnStopLogging, btnExport, lblSampleCount });

            mainPanel.Controls.AddRange(new Control[] { connectionPanel, readingsPanel, plotAdc, plotDistance, loggingPanel });
            tab.Controls.Add(mainPanel);
        }

        private void CreateCalibrationTab(TabPage tab)
        {
            var calibPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            // Data Entry Panel
            var entryPanel = new GroupBox
            {
                Text = "Calibration Data Entry",
                Location = new Point(10, 10),
                Size = new Size(400, 300)
            };

            dgvCalibration = new DataGridView
            {
                Location = new Point(10, 25),
                Size = new Size(380, 200),
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvCalibration.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Distance (cm)", Name = "Distance", Width = 150 });
            dgvCalibration.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ADC Value", Name = "AdcValue", Width = 150 });

            var lblDist = new Label { Text = "Distance (cm):", Location = new Point(10, 235), AutoSize = true };
            numDistance = new NumericUpDown { Location = new Point(100, 232), Width = 80, DecimalPlaces = 2, Minimum = 0, Maximum = 500, Value = 10 };

            var lblAdc = new Label { Text = "ADC Value:", Location = new Point(190, 235), AutoSize = true };
            numAdcValue = new NumericUpDown { Location = new Point(265, 232), Width = 80, Maximum = 1023, Value = 512 };

            btnAddPoint = new Button { Text = "Add Point", Location = new Point(10, 265), Size = new Size(90, 25) };
            btnAddPoint.Click += BtnAddPoint_Click;

            btnRemovePoint = new Button { Text = "Remove", Location = new Point(110, 265), Size = new Size(90, 25) };
            btnRemovePoint.Click += BtnRemovePoint_Click;

            btnClearPoints = new Button { Text = "Clear All", Location = new Point(210, 265), Size = new Size(90, 25) };
            btnClearPoints.Click += BtnClearPoints_Click;

            entryPanel.Controls.AddRange(new Control[] { dgvCalibration, lblDist, numDistance, lblAdc, numAdcValue, btnAddPoint, btnRemovePoint, btnClearPoints });

            // Fitting Panel
            var fittingPanel = new GroupBox
            {
                Text = "Curve Fitting",
                Location = new Point(10, 320),
                Size = new Size(400, 180)
            };

            var lblFit = new Label { Text = "Fit Type:", Location = new Point(10, 25), AutoSize = true };
            cmbFitType = new ComboBox { Location = new Point(70, 22), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFitType.Items.AddRange(new object[] { "Linear", "Polynomial (2nd)", "Polynomial (3rd)", "Power", "Inverse" });
            cmbFitType.SelectedIndex = 1;

            btnFitCurve = new Button { Text = "Fit Curve", Location = new Point(240, 20), Size = new Size(100, 30) };
            btnFitCurve.Click += BtnFitCurve_Click;

            lblEquation = new Label { Text = "Equation: ---", Location = new Point(10, 65), AutoSize = true, MaximumSize = new Size(380, 0) };
            lblRSquared = new Label { Text = "R² = ---", Location = new Point(10, 95), AutoSize = true, Font = new Font("Arial", 10, FontStyle.Bold) };

            var lblMinAdc = new Label { Text = "Min ADC:", Location = new Point(10, 125), AutoSize = true };
            numMinAdc = new NumericUpDown { Location = new Point(80, 122), Width = 80, Maximum = 1023, Value = 50 };

            var lblMaxAdc = new Label { Text = "Max ADC:", Location = new Point(170, 125), AutoSize = true };
            numMaxAdc = new NumericUpDown { Location = new Point(240, 122), Width = 80, Maximum = 1023, Value = 950 };

            fittingPanel.Controls.AddRange(new Control[] { lblFit, cmbFitType, btnFitCurve, lblEquation, lblRSquared, lblMinAdc, numMinAdc, lblMaxAdc, numMaxAdc });

            // Calibration Chart
            plotCalibration = new FormsPlot { Location = new Point(420, 10), Size = new Size(750, 490) };

            // Actions Panel
            var actionsPanel = new GroupBox
            {
                Text = "Actions",
                Location = new Point(10, 510),
                Size = new Size(400, 80)
            };

            btnSaveCalibration = new Button { Text = "Save Calibration", Location = new Point(10, 25), Size = new Size(120, 35) };
            btnSaveCalibration.Click += BtnSaveCalibration_Click;

            btnLoadCalibration = new Button { Text = "Load Calibration", Location = new Point(140, 25), Size = new Size(120, 35) };
            btnLoadCalibration.Click += BtnLoadCalibration_Click;

            btnApplyCalibration = new Button { Text = "Apply to Monitor", Location = new Point(270, 25), Size = new Size(120, 35), Enabled = false };
            btnApplyCalibration.Click += BtnApplyCalibration_Click;

            actionsPanel.Controls.AddRange(new Control[] { btnSaveCalibration, btnLoadCalibration, btnApplyCalibration });

            calibPanel.Controls.AddRange(new Control[] { entryPanel, fittingPanel, plotCalibration, actionsPanel });
            tab.Controls.Add(calibPanel);
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
            // ADC Chart
            plotAdc.Plot.Title("ADC Value vs. Time");
            plotAdc.Plot.XLabel("Time (seconds)");
            plotAdc.Plot.YLabel("ADC Value (0-1023)");

            // Distance Chart
            plotDistance.Plot.Title("Distance vs. Time");
            plotDistance.Plot.XLabel("Time (seconds)");
            plotDistance.Plot.YLabel("Distance (cm)");

            // Calibration Chart
            plotCalibration.Plot.Title("Calibration Curve");
            plotCalibration.Plot.XLabel("ADC Value");
            plotCalibration.Plot.YLabel("Distance (cm)");
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
                cmbPortName.Items.Add("No ports available");
                cmbPortName.SelectedIndex = 0;
            }
        }

        private void BtnConnect_Click(object? sender, EventArgs e)
        {
            if (cmbPortName.SelectedItem == null || cmbPortName.SelectedItem.ToString() == "No ports available")
            {
                MessageBox.Show("Please select a valid COM port.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string portName = cmbPortName.SelectedItem.ToString()!;
            int baudRate = int.Parse(cmbBaudRate.SelectedItem.ToString()!);

            if (_serialPort.Connect(portName, baudRate))
            {
                lblStatus.Text = $"Status: Connected to {portName}";
                lblStatus.ForeColor = Color.Green;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                cmbPortName.Enabled = false;
                cmbBaudRate.Enabled = false;
                _chartUpdateTimer.Start();
                _startTime = DateTime.Now;
            }
            else
            {
                MessageBox.Show($"Failed to connect to {portName}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDisconnect_Click(object? sender, EventArgs e)
        {
            _serialPort.Disconnect();
            lblStatus.Text = "Status: Disconnected";
            lblStatus.ForeColor = Color.Red;
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

            int adcValue = e.AdcValue;
            double distance = 0;
            bool isInRange = true;

            if (_currentCalibration != null && _currentCalibration.Coefficients.Length > 0)
            {
                distance = _currentCalibration.ConvertAdcToDistance(adcValue);
                isInRange = _currentCalibration.IsInRange(adcValue);
            }

            // Update UI
            lblAdcValue.Text = $"ADC Value: {adcValue} / 1023";
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

            // Add to data storage
            double elapsedSeconds = (DateTime.Now - _startTime).TotalSeconds;
            _adcTimeData.Add(elapsedSeconds);
            _adcValueData.Add(adcValue);
            _distanceTimeData.Add(elapsedSeconds);
            _distanceValueData.Add(distance);

            // Keep only last 300 points (30 seconds at 10Hz)
            if (_adcTimeData.Count > 300)
            {
                _adcTimeData.RemoveAt(0);
                _adcValueData.RemoveAt(0);
                _distanceTimeData.RemoveAt(0);
                _distanceValueData.RemoveAt(0);
            }

            // Log data if logging is active
            if (_isLogging)
            {
                var reading = new SensorReading(adcValue, distance, isInRange);
                _dataLogger.AddReading(reading);
                lblSampleCount.Text = $"Samples: {_dataLogger.Count}";
            }
        }

        private void ChartUpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (_adcTimeData.Count > 0)
            {
                // Update ADC chart
                plotAdc.Plot.Clear();
                plotAdc.Plot.Add.Scatter(_adcTimeData.ToArray(), _adcValueData.ToArray());
                plotAdc.Plot.Axes.AutoScale();
                plotAdc.Refresh();

                // Update Distance chart
                plotDistance.Plot.Clear();
                plotDistance.Plot.Add.Scatter(_distanceTimeData.ToArray(), _distanceValueData.ToArray());
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
            MessageBox.Show("Connection to device lost.", "Connection Lost", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("No data to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        MessageBox.Show($"Data exported successfully to:\n{sfd.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export failed: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Calibration Tab Methods
        private void BtnAddPoint_Click(object? sender, EventArgs e)
        {
            double distance = (double)numDistance.Value;
            int adcValue = (int)numAdcValue.Value;

            dgvCalibration.Rows.Add(distance.ToString("F2"), adcValue.ToString());
        }

        private void BtnRemovePoint_Click(object? sender, EventArgs e)
        {
            if (dgvCalibration.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvCalibration.SelectedRows)
                {
                    if (!row.IsNewRow)
                        dgvCalibration.Rows.Remove(row);
                }
            }
        }

        private void BtnClearPoints_Click(object? sender, EventArgs e)
        {
            dgvCalibration.Rows.Clear();
        }

        private void BtnFitCurve_Click(object? sender, EventArgs e)
        {
            if (dgvCalibration.Rows.Count < 2)
            {
                MessageBox.Show("At least 2 calibration points are required.", "Insufficient Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<CalibrationPoint> points = new List<CalibrationPoint>();
            foreach (DataGridViewRow row in dgvCalibration.Rows)
            {
                if (!row.IsNewRow && row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    double dist = double.Parse(row.Cells[0].Value.ToString()!);
                    int adc = int.Parse(row.Cells[1].Value.ToString()!);
                    points.Add(new CalibrationPoint(dist, adc));
                }
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
                _currentCalibration = _calibrationService.PerformCalibration(points, fitType);
                _currentCalibration.MinAdcValue = (int)numMinAdc.Value;
                _currentCalibration.MaxAdcValue = (int)numMaxAdc.Value;

                lblEquation.Text = $"Equation: {_currentCalibration.Equation}";
                lblRSquared.Text = $"R² = {_currentCalibration.RSquared:F6}";
                btnApplyCalibration.Enabled = true;

                // Plot calibration curve
                PlotCalibrationCurve();

                MessageBox.Show("Calibration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Calibration failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PlotCalibrationCurve()
        {
            if (_currentCalibration == null) return;

            plotCalibration.Plot.Clear();

            // Plot data points
            double[] xPoints = _currentCalibration.Points.Select(p => (double)p.AdcValue).ToArray();
            double[] yPoints = _currentCalibration.Points.Select(p => p.Distance).ToArray();
            var scatter = plotCalibration.Plot.Add.Scatter(xPoints, yPoints);
            scatter.MarkerSize = 10;
            scatter.LineWidth = 0;
            scatter.Label = "Data Points";

            // Plot fitted curve
            int minAdc = _currentCalibration.Points.Min(p => p.AdcValue);
            int maxAdc = _currentCalibration.Points.Max(p => p.AdcValue);
            double[] xCurve = Enumerable.Range(minAdc, maxAdc - minAdc + 1).Select(x => (double)x).ToArray();
            double[] yCurve = xCurve.Select(x => _currentCalibration.ConvertAdcToDistance((int)x)).ToArray();
            var line = plotCalibration.Plot.Add.Scatter(xCurve, yCurve);
            line.LineWidth = 2;
            line.MarkerSize = 0;
            line.Label = "Fitted Curve";

            plotCalibration.Plot.ShowLegend();
            plotCalibration.Plot.Axes.AutoScale();
            plotCalibration.Refresh();
        }

        private void BtnSaveCalibration_Click(object? sender, EventArgs e)
        {
            if (_currentCalibration == null)
            {
                MessageBox.Show("No calibration to save. Please fit a curve first.", "No Calibration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        MessageBox.Show("Calibration saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        
                        // Update UI
                        dgvCalibration.Rows.Clear();
                        foreach (var point in _currentCalibration.Points)
                        {
                            dgvCalibration.Rows.Add(point.Distance.ToString("F2"), point.AdcValue.ToString());
                        }

                        lblEquation.Text = $"Equation: {_currentCalibration.Equation}";
                        lblRSquared.Text = $"R² = {_currentCalibration.RSquared:F6}";
                        numMinAdc.Value = _currentCalibration.MinAdcValue;
                        numMaxAdc.Value = _currentCalibration.MaxAdcValue;
                        
                        int fitTypeIndex = _currentCalibration.FitType switch
                        {
                            FitType.Linear => 0,
                            FitType.Polynomial2 => 1,
                            FitType.Polynomial3 => 2,
                            FitType.Power => 3,
                            FitType.Inverse => 4,
                            _ => 1
                        };
                        cmbFitType.SelectedIndex = fitTypeIndex;

                        btnApplyCalibration.Enabled = true;
                        PlotCalibrationCurve();

                        MessageBox.Show("Calibration loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Load failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnApplyCalibration_Click(object? sender, EventArgs e)
        {
            if (_currentCalibration != null)
            {
                MessageBox.Show("Calibration applied to Monitor tab!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tabControl.SelectedIndex = 0; // Switch to Monitor tab
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

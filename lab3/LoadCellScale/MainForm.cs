using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace LoadCellScale
{
    public partial class MainForm : Form
    {
        private const double ReferenceVoltage = 3.6;
        private const int AdcMaxValue = 1023;
        private const int ChartHistoryPoints = 600;
        private const int DefaultSmoothingWindow = 200;
        private const double StabilityWindowMs = 500.0;
        private const double DefaultStabilityThresholdKg = 0.01;
        private const int RecentSampleCapacity = 3000;

        private readonly SerialPort _serialPort = new SerialPort();
        private readonly FrameParser _frameParser = new FrameParser();
        private readonly ConcurrentQueue<AdcSample> _pendingSamples = new ConcurrentQueue<AdcSample>();
        private readonly Queue<WeightedSample> _stabilityWindow = new Queue<WeightedSample>();
        private readonly Queue<WeightedSample> _recentSamples = new Queue<WeightedSample>();
        private readonly Queue<DateTime> _sampleTimes = new Queue<DateTime>();
        private readonly RollingAverage _smoothing;
        private readonly Timer _uiTimer;
        private readonly List<CalibrationPoint> _calibrationPoints = new List<CalibrationPoint>();

        private CalibrationModel _calibration = CalibrationModel.Empty;
        private WeightedSample? _latestSample;
        private double _tareOffsetKg;
        private bool _isStable;
        private DateTime? _chartStart;

        private ComboBox cmbPorts;
        private Button btnRefreshPorts;
        private Button btnConnect;
        private Label lblConnectionStatus;
        private TextBox txtAdc;
        private TextBox txtVoltage;
        private TextBox txtWeight;
        private TextBox txtSmoothed;
        private TextBox txtSampleRate;
        private NumericUpDown nudSmoothingWindow;
        private NumericUpDown nudStabilityThreshold;
        private Button btnTare;
        private Label lblStability;
        private Chart chartLive;
        private TextBox txtReferenceMass;
        private Button btnCaptureCalibration;
        private Button btnComputeCalibration;
        private Button btnClearCalibration;
        private ListView lvCalibrationPoints;
        private Label lblCalibrationEquation;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabelConnection;
        private ToolStripStatusLabel statusLabelCalibration;
        private ToolStripStatusLabel statusLabelSamples;

        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;

            _smoothing = new RollingAverage(DefaultSmoothingWindow);
            _uiTimer = new Timer { Interval = 50 };
            _uiTimer.Tick += UiTimerOnTick;
            _uiTimer.Start();

            _serialPort.BaudRate = 115200;
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.DataReceived += SerialPortOnDataReceived;

            PopulateSerialPorts();
            UpdateConnectionStatus();
            UpdateCalibrationSummary();
            UpdateSampleStatus();
            UpdateStabilityStatus();
            ResetDisplayFields();

            FormClosing += OnFormClosing;
        }

        private void UiTimerOnTick(object sender, EventArgs e)
        {
            var processed = 0;
            while (_pendingSamples.TryDequeue(out var sample))
            {
                ProcessSample(sample);
                processed++;
            }

            if (processed > 0)
            {
                UpdateDisplay();
            }
            else if (_latestSample.HasValue)
            {
                EvaluateStability(DateTime.UtcNow);
                UpdateDisplayFields(_latestSample.Value);
            }
        }

        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (_serialPort.BytesToRead > 0)
                {
                    var nextByte = (byte)_serialPort.ReadByte();
                    if (_frameParser.TryPush(nextByte, out var adc))
                    {
                        _pendingSamples.Enqueue(new AdcSample(DateTime.UtcNow, adc));
                    }
                }
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action(() =>
                {
                    MessageBox.Show(this, $"Serial read error: {ex.Message}", "Serial port",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CloseSerialPort();
                }));
            }
        }

        private void ProcessSample(AdcSample sample)
        {
            if (!_chartStart.HasValue)
            {
                _chartStart = sample.Timestamp;
            }

            var adc = sample.Value;
            var voltage = adc / (double)AdcMaxValue * ReferenceVoltage;
            var weightRaw = _calibration.IsValid ? _calibration.Evaluate(adc) : 0.0;
            var weightDisplay = weightRaw - _tareOffsetKg;
            var smoothedWeight = _smoothing.Push(weightDisplay);

            var weightedSample = new WeightedSample(sample.Timestamp, adc, voltage, weightRaw, weightDisplay,
                smoothedWeight);
            _latestSample = weightedSample;

            _recentSamples.Enqueue(weightedSample);
            while (_recentSamples.Count > RecentSampleCapacity)
            {
                _recentSamples.Dequeue();
            }

            _stabilityWindow.Enqueue(weightedSample);
            _sampleTimes.Enqueue(sample.Timestamp);

            EvaluateStability(sample.Timestamp);
            UpdateChart(weightedSample);
        }

        private void EvaluateStability(DateTime now)
        {
            while (_stabilityWindow.Count > 0 &&
                   (now - _stabilityWindow.Peek().Timestamp).TotalMilliseconds > StabilityWindowMs)
            {
                _stabilityWindow.Dequeue();
            }

            while (_sampleTimes.Count > 0 &&
                   (now - _sampleTimes.Peek()).TotalSeconds > 1.0)
            {
                _sampleTimes.Dequeue();
            }

            if (_stabilityWindow.Count < 2)
            {
                _isStable = false;
                return;
            }

            var weights = _stabilityWindow.Select(s => s.SmoothedWeight).ToArray();
            var average = weights.Average();
            var variance = weights.Sum(w => Math.Pow(w - average, 2)) / weights.Length;
            var stdDev = Math.Sqrt(variance);

            var threshold = (double)nudStabilityThreshold.Value / 1000.0;
            _isStable = stdDev <= threshold;
        }

        private void UpdateChart(WeightedSample sample)
        {
            if (!_chartStart.HasValue)
            {
                return;
            }

            var elapsed = (sample.Timestamp - _chartStart.Value).TotalSeconds;

            var rawSeries = chartLive.Series["Raw weight"];
            rawSeries.Points.AddXY(elapsed, sample.DisplayWeight);
            while (rawSeries.Points.Count > ChartHistoryPoints)
            {
                rawSeries.Points.RemoveAt(0);
            }

            var smoothSeries = chartLive.Series["Smoothed weight"];
            smoothSeries.Points.AddXY(elapsed, sample.SmoothedWeight);
            while (smoothSeries.Points.Count > ChartHistoryPoints)
            {
                smoothSeries.Points.RemoveAt(0);
            }

            chartLive.ChartAreas[0].RecalculateAxesScale();
        }

        private void UpdateDisplay()
        {
            if (!_latestSample.HasValue)
            {
                ResetDisplayFields();
                return;
            }

            UpdateDisplayFields(_latestSample.Value);
        }

        private void UpdateDisplayFields(WeightedSample sample)
        {
            txtAdc.Text = sample.Adc.ToString(CultureInfo.InvariantCulture);
            txtVoltage.Text = sample.Voltage.ToString("F3", CultureInfo.InvariantCulture);
            txtWeight.Text = sample.DisplayWeight.ToString("F3", CultureInfo.InvariantCulture);
            txtSmoothed.Text = sample.SmoothedWeight.ToString("F3", CultureInfo.InvariantCulture);
            txtSampleRate.Text = _sampleTimes.Count.ToString(CultureInfo.InvariantCulture);

            lblStability.Text = _isStable ? "Stability: Stable" : "Stability: Settling";
            lblStability.BackColor = _isStable ? Color.FromArgb(30, 180, 115) : Color.Gold;
            lblStability.ForeColor = _isStable ? Color.White : Color.Black;

            UpdateSampleStatus();
        }

        private void PopulateSerialPorts()
        {
            var previous = cmbPorts.SelectedItem as string;
            var ports = SerialPort.GetPortNames().OrderBy(p => p).ToArray();

            cmbPorts.Items.Clear();
            cmbPorts.Items.AddRange(ports.Cast<object>().ToArray());

            if (ports.Length == 0)
            {
                cmbPorts.SelectedIndex = -1;
                lblConnectionStatus.Text = "No COM ports found";
            }
            else if (previous != null && ports.Contains(previous))
            {
                cmbPorts.SelectedItem = previous;
            }
            else
            {
                cmbPorts.SelectedIndex = 0;
            }
        }

        private void ToggleConnection()
        {
            if (_serialPort.IsOpen)
            {
                CloseSerialPort();
            }
            else
            {
                OpenSerialPort();
            }
        }

        private void OpenSerialPort()
        {
            var selected = cmbPorts.SelectedItem as string;
            if (selected == null)
            {
                MessageBox.Show(this, "Please select a serial port.", "Serial port",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _serialPort.PortName = selected;
                _serialPort.Open();
                btnConnect.Text = "Disconnect";
                btnConnect.BackColor = Color.Firebrick;
                btnConnect.ForeColor = Color.White;
                lblConnectionStatus.Text = $"Connected to {selected}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Unable to open {selected}: {ex.Message}", "Serial port",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseSerialPort();
            }

            UpdateConnectionStatus();
        }

        private void CloseSerialPort()
        {
            if (_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Close();
                }
                catch
                {
                    // ignored
                }
            }

            btnConnect.Text = "Connect";
            btnConnect.BackColor = Color.FromArgb(16, 124, 16);
            btnConnect.ForeColor = Color.White;
            lblConnectionStatus.Text = "Port closed";

            UpdateConnectionStatus();
        }

        private void UpdateConnectionStatus()
        {
            if (_serialPort.IsOpen)
            {
                statusLabelConnection.Text = $"Connected: {_serialPort.PortName}";
                statusLabelConnection.ForeColor = Color.DarkGreen;
            }
            else
            {
                statusLabelConnection.Text = "Port closed";
                statusLabelConnection.ForeColor = Color.Firebrick;
            }
        }

        private void ChangeSmoothingWindow()
        {
            _smoothing.Capacity = (int)nudSmoothingWindow.Value;
            UpdateSampleStatus();
        }

        private void UpdateSampleStatus()
        {
            statusLabelSamples.Text =
                $"Samples: {_sampleTimes.Count} Hz | Window: {_smoothing.Capacity} | Points: {_recentSamples.Count}";
        }

        private void UpdateStabilityStatus()
        {
            var thresholdKg = (double)nudStabilityThreshold.Value / 1000.0;
            statusLabelCalibration.Text = _calibration.IsValid
                ? $"Calibration: y = {_calibration.Slope:F6}·x + {_calibration.Intercept:F3} kg (R² = {_calibration.RSquared:F4}), stability ≤ {thresholdKg:F3} kg"
                : $"Calibration: none, stability ≤ {thresholdKg:F3} kg";
        }

        private void ApplyTare()
        {
            if (_latestSample.HasValue)
            {
                _tareOffsetKg = _latestSample.Value.WeightRaw;
                _smoothing.Reset();
                MessageBox.Show(this, "Tare applied using current reading.", "Tare",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CaptureCalibrationPoint()
        {
            if (!_latestSample.HasValue)
            {
                MessageBox.Show(this, "No live data available yet.", "Calibration",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!TryParseMass(txtReferenceMass.Text, out var massKg))
            {
                MessageBox.Show(this, "Enter a valid reference mass in kilograms, e.g. 0.500.", "Calibration",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReferenceMass.Focus();
                txtReferenceMass.SelectAll();
                return;
            }

            var samples = GetRecentSamplesForAveraging(Math.Max(50, (int)nudSmoothingWindow.Value));
            if (samples.Count == 0)
            {
                MessageBox.Show(this, "Not enough samples collected yet. Hold the mass steady for a moment.",
                    "Calibration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var avgAdc = samples.Average(s => s.Adc);
            var avgVoltage = samples.Average(s => s.Voltage);

            var point = new CalibrationPoint(massKg, avgAdc, avgVoltage, DateTime.Now);
            _calibrationPoints.Add(point);

            var item = new ListViewItem(new[]
            {
                massKg.ToString("F3", CultureInfo.InvariantCulture),
                avgAdc.ToString("F1", CultureInfo.InvariantCulture),
                avgVoltage.ToString("F4", CultureInfo.InvariantCulture),
                point.Timestamp.ToLocalTime().ToString("HH:mm:ss")
            });
            lvCalibrationPoints.Items.Add(item);
        }

        private List<WeightedSample> GetRecentSamplesForAveraging(int count)
        {
            return _recentSamples.Reverse().Take(count).ToList();
        }

        private void ComputeCalibration()
        {
            if (_calibrationPoints.Count < 2)
            {
                MessageBox.Show(this, "Capture at least two calibration points.", "Calibration",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var n = _calibrationPoints.Count;
            var sumX = _calibrationPoints.Sum(p => p.AverageAdc);
            var sumY = _calibrationPoints.Sum(p => p.MassKg);
            var sumXX = _calibrationPoints.Sum(p => p.AverageAdc * p.AverageAdc);
            var sumXY = _calibrationPoints.Sum(p => p.AverageAdc * p.MassKg);

            var denominator = n * sumXX - sumX * sumX;
            if (Math.Abs(denominator) < 1e-9)
            {
                MessageBox.Show(this, "Calibration points are too similar. Use a wider range of masses.",
                    "Calibration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var slope = (n * sumXY - sumX * sumY) / denominator;
            var intercept = (sumY - slope * sumX) / n;

            var avgY = sumY / n;
            var ssTot = _calibrationPoints.Sum(p => Math.Pow(p.MassKg - avgY, 2));
            var ssRes = _calibrationPoints.Sum(p =>
            {
                var fit = slope * p.AverageAdc + intercept;
                return Math.Pow(p.MassKg - fit, 2);
            });
            var rSquared = ssTot > 0 ? 1.0 - ssRes / ssTot : 1.0;

            _calibration = new CalibrationModel(slope, intercept, rSquared);
            _tareOffsetKg = 0;
            _smoothing.Reset();

            UpdateCalibrationSummary();
            MessageBox.Show(this, "Calibration updated.", "Calibration",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateCalibrationSummary()
        {
            if (_calibration.IsValid)
            {
                lblCalibrationEquation.Text =
                    $"Mass = {_calibration.Slope:F6}·ADC + {_calibration.Intercept:F3} (R² = {_calibration.RSquared:F4})";
            }
            else
            {
                lblCalibrationEquation.Text = "No calibration applied.";
            }

            UpdateStabilityStatus();
        }

        private void ClearCalibrationPoints()
        {
            if (MessageBox.Show(this, "Clear all calibration points?", "Calibration",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _calibrationPoints.Clear();
                lvCalibrationPoints.Items.Clear();
                _calibration = CalibrationModel.Empty;
                _tareOffsetKg = 0;
                UpdateCalibrationSummary();
            }
        }

        private static bool TryParseMass(string text, out double massKg)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out massKg) && massKg >= 0;
        }

        private void ResetDisplayFields()
        {
            txtAdc.Text = "-";
            txtVoltage.Text = "-";
            txtWeight.Text = "-";
            txtSmoothed.Text = "-";
            txtSampleRate.Text = "0";
            lblStability.Text = "Stability: Unknown";
            lblStability.BackColor = Color.LightGray;
            lblStability.ForeColor = Color.Black;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            _uiTimer.Stop();
            CloseSerialPort();
        }

        private sealed class FrameParser
        {
            private enum ParserState
            {
                WaitingMarker,
                WaitingMs,
                WaitingLs
            }

            private ParserState _state = ParserState.WaitingMarker;
            private byte _ms;

            public bool TryPush(byte value, out int adc)
            {
                adc = 0;

                switch (_state)
                {
                    case ParserState.WaitingMarker:
                        if (value == 0xFF)
                        {
                            _state = ParserState.WaitingMs;
                        }
                        break;

                    case ParserState.WaitingMs:
                        _ms = (byte)(value & 0x1F);
                        _state = ParserState.WaitingLs;
                        break;

                    case ParserState.WaitingLs:
                        var ls = (byte)(value & 0x1F);
                        adc = (_ms << 5) | ls;
                        _state = ParserState.WaitingMarker;
                        return true;
                }

                return false;
            }
        }

        private sealed class RollingAverage
        {
            private readonly Queue<double> _values = new Queue<double>();
            private double _sum;
            private int _capacity;

            public RollingAverage(int capacity)
            {
                Capacity = capacity;
            }

            public int Capacity
            {
                get => _capacity;
                set
                {
                    _capacity = Math.Max(1, value);
                    Trim();
                }
            }

            public double Push(double value)
            {
                _values.Enqueue(value);
                _sum += value;
                Trim();
                return _values.Count > 0 ? _sum / _values.Count : 0.0;
            }

            public void Reset()
            {
                _values.Clear();
                _sum = 0;
            }

            private void Trim()
            {
                while (_values.Count > _capacity)
                {
                    _sum -= _values.Dequeue();
                }
            }
        }

        private readonly struct AdcSample
        {
            public AdcSample(DateTime timestamp, int value)
            {
                Timestamp = timestamp;
                Value = value;
            }

            public DateTime Timestamp { get; }
            public int Value { get; }
        }

        private readonly struct WeightedSample
        {
            public WeightedSample(DateTime timestamp, int adc, double voltage, double weightRaw, double displayWeight,
                double smoothedWeight)
            {
                Timestamp = timestamp;
                Adc = adc;
                Voltage = voltage;
                WeightRaw = weightRaw;
                DisplayWeight = displayWeight;
                SmoothedWeight = smoothedWeight;
            }

            public DateTime Timestamp { get; }
            public int Adc { get; }
            public double Voltage { get; }
            public double WeightRaw { get; }
            public double DisplayWeight { get; }
            public double SmoothedWeight { get; }
        }

        private readonly struct CalibrationPoint
        {
            public CalibrationPoint(double massKg, double averageAdc, double averageVoltage, DateTime timestamp)
            {
                MassKg = massKg;
                AverageAdc = averageAdc;
                AverageVoltage = averageVoltage;
                Timestamp = timestamp;
            }

            public double MassKg { get; }
            public double AverageAdc { get; }
            public double AverageVoltage { get; }
            public DateTime Timestamp { get; }
        }

        private readonly struct CalibrationModel
        {
            public static readonly CalibrationModel Empty = new CalibrationModel(double.NaN, double.NaN, double.NaN);

            public CalibrationModel(double slope, double intercept, double rSquared)
            {
                Slope = slope;
                Intercept = intercept;
                RSquared = rSquared;
            }

            public double Slope { get; }
            public double Intercept { get; }
            public double RSquared { get; }
            public bool IsValid => !double.IsNaN(Slope) && !double.IsNaN(Intercept);

            public double Evaluate(double adc)
            {
                return Slope * adc + Intercept;
            }
        }
    }
}

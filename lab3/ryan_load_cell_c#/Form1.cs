using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ryan_load_cell_c_
{
    public partial class Form1 : Form
    {
        private readonly SerialPort _serialPort;
        private readonly object _sampleLock = new object();
        private readonly Queue<double> _weightRollingQueue = new Queue<double>();
        private double _weightRollingSum;
        private readonly Queue<double> _rawRollingQueue = new Queue<double>();
        private double _rawRollingSum;
        private readonly Queue<TimedValue> _stabilityQueue = new Queue<TimedValue>();
        private readonly List<CalibrationPoint> _calibrationPoints = new List<CalibrationPoint>();
        private readonly Queue<ChartPoint> _chartPending = new Queue<ChartPoint>();
        private readonly Timer _uiTimer;
        private ParserState _parserState = ParserState.WaitingForStart;
        private byte _pendingMs5;
        private bool _hasSample;
        private int _rollingWindowSize = 200;
        private double _stabilityThreshold = 0.02;
        private double _scaleSlope;
        private double _scaleIntercept;
        private double _tareOffset;
        private int _lastRaw;
        private double _lastVoltage;
        private double _lastWeight;
        private double _lastAverageWeight;
        private double _lastAverageRaw;
        private double _lastStdDev;
        private double _lastMass;
        private bool _lastStable;
        private long _sampleIndex;

        private const double ReferenceVoltage = 3.6;
        private const double MaxAdcValue = 1023.0;
        private const int StabilityWindowMilliseconds = 500;
        private const int ChartMaxPoints = 1200;

        private enum ParserState
        {
            WaitingForStart,
            ReadingMsb,
            ReadingLsb
        }

        private struct TimedValue
        {
            public TimedValue(DateTime timestamp, double value)
            {
                Timestamp = timestamp;
                Value = value;
            }

            public DateTime Timestamp { get; }
            public double Value { get; }
        }

        private struct CalibrationPoint
        {
            public CalibrationPoint(double raw, double mass)
            {
                Raw = raw;
                Mass = mass;
            }

            public double Raw { get; }
            public double Mass { get; }
        }

        private struct ChartPoint
        {
            public ChartPoint(DateTime timestamp, double weight, double avgWeight)
            {
                Timestamp = timestamp;
                Weight = weight;
                AverageWeight = avgWeight;
            }

            public DateTime Timestamp { get; }
            public double Weight { get; }
            public double AverageWeight { get; }
        }

        public Form1()
        {
            InitializeComponent();

            _serialPort = new SerialPort
            {
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            _serialPort.DataReceived += SerialPort_DataReceived;

            txtBaudRate.Text = _serialPort.BaudRate.ToString(CultureInfo.InvariantCulture);
            txtStdDev.Text = "--";
            UpdateTareDisplay();

            _uiTimer = new Timer { Interval = 100 };
            _uiTimer.Tick += UiTimer_Tick;
            _uiTimer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshSerialPorts();
            ConfigureChart();
            UpdateConnectionStatus(false, "Disconnected");

            _rollingWindowSize = (int)numAverageWindow.Value;
            _stabilityThreshold = (double)numStabilityThreshold.Value;

            UpdateCalibrationSummaryDisplay();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _uiTimer.Stop();
            _uiTimer.Tick -= UiTimer_Tick;
            _uiTimer.Dispose();

            CloseSerialPort();
            _serialPort.DataReceived -= SerialPort_DataReceived;
            _serialPort.Dispose();
        }

        private void ConfigureChart()
        {
            var area = chartData.ChartAreas.First();
            area.AxisX.LabelStyle.Format = "HH:mm:ss";
            area.AxisX.MajorGrid.LineColor = Color.Gainsboro;
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            area.AxisY.Title = "Weight (kg)";
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 2.2;

            var weightSeries = chartData.Series["Weight"];
            weightSeries.ChartType = SeriesChartType.FastLine;
            weightSeries.Color = Color.SteelBlue;
            weightSeries.BorderWidth = 2;
            weightSeries.Points.Clear();

            var avgSeries = chartData.Series["AvgWeight"];
            avgSeries.ChartType = SeriesChartType.FastLine;
            avgSeries.Color = Color.DarkOrange;
            avgSeries.BorderWidth = 2;
            avgSeries.BorderDashStyle = ChartDashStyle.Dash;
            avgSeries.Points.Clear();
        }

        private void RefreshSerialPorts()
        {
            string[] ports = SerialPort.GetPortNames()
                .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            string previousSelection = comboPorts.SelectedItem as string;

            comboPorts.Items.Clear();
            comboPorts.Items.AddRange(ports);

            if (!string.IsNullOrEmpty(previousSelection) && ports.Contains(previousSelection, StringComparer.OrdinalIgnoreCase))
            {
                comboPorts.SelectedItem = previousSelection;
            }
            else if (ports.Length > 0)
            {
                comboPorts.SelectedIndex = 0;
            }
        }

        private void btnRefreshPorts_Click(object sender, EventArgs e)
        {
            RefreshSerialPorts();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                CloseSerialPort();
                return;
            }

            if (comboPorts.SelectedItem is null)
            {
                MessageBox.Show(this, "Select a serial port before connecting.", "Serial connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string portName = comboPorts.SelectedItem.ToString();

            try
            {
                _serialPort.PortName = portName;
                _serialPort.Open();
                UpdateConnectionStatus(true, $"Connected to {portName}");
            }
            catch (Exception ex)
            {
                UpdateConnectionStatus(false, "Disconnected");
                MessageBox.Show(this, $"Unable to open {portName}:{Environment.NewLine}{ex.Message}", "Serial error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseSerialPort()
        {
            if (_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Close();
                }
                catch (Exception)
                {
                    // ignore shutdown exceptions
                }
            }

            UpdateConnectionStatus(false, "Disconnected");
        }

        private void UpdateConnectionStatus(bool isConnected, string message)
        {
            lblConnectionStatus.Text = message;
            lblConnectionStatus.ForeColor = isConnected ? Color.ForestGreen : Color.Firebrick;
            btnConnect.Text = isConnected ? "Disconnect" : "Connect";
            comboPorts.Enabled = !isConnected;
            btnRefreshPorts.Enabled = !isConnected;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (_serialPort.IsOpen && _serialPort.BytesToRead > 0)
                {
                    int next = _serialPort.ReadByte();
                    if (next < 0)
                    {
                        break;
                    }
                    ParseByte((byte)next);
                }
            }
            catch (TimeoutException)
            {
                // ignore transient timeouts
            }
            catch (InvalidOperationException)
            {
                // occurs when closing the port
            }
            catch (IOException)
            {
                // ignore IO errors from port closure
            }
        }

        private void ParseByte(byte value)
        {
            switch (_parserState)
            {
                case ParserState.WaitingForStart:
                    if (value == 0xFF)
                    {
                        _parserState = ParserState.ReadingMsb;
                    }
                    break;

                case ParserState.ReadingMsb:
                    if (value == 0xFF)
                    {
                        _parserState = ParserState.ReadingMsb;
                    }
                    else
                    {
                        _pendingMs5 = (byte)(value & 0x1F);
                        _parserState = ParserState.ReadingLsb;
                    }
                    break;

                case ParserState.ReadingLsb:
                    if (value == 0xFF)
                    {
                        _parserState = ParserState.ReadingMsb;
                    }
                    else
                    {
                        int raw = ((_pendingMs5 << 5) | (value & 0x1F)) & 0x3FF;
                        _parserState = ParserState.WaitingForStart;
                        ProcessSample(raw);
                    }
                    break;
            }
        }

        private void ProcessSample(int rawCount)
        {
            DateTime timestamp = DateTime.Now;
            double voltage;
            double weight;
            double avgWeight;
            double avgRaw;
            double stdDev;
            bool isStable;
            double mass;

            lock (_sampleLock)
            {
                _hasSample = true;
                _lastRaw = rawCount;
                voltage = rawCount * ReferenceVoltage / MaxAdcValue;
                _lastVoltage = voltage;

                mass = (_scaleSlope * rawCount) + _scaleIntercept;
                _lastMass = mass;
                weight = mass - _tareOffset;
                _lastWeight = weight;

                int windowSize = Math.Max(1, _rollingWindowSize);

                UpdateRollingQueue(_rawRollingQueue, ref _rawRollingSum, rawCount, windowSize);
                avgRaw = _rawRollingQueue.Count > 0 ? _rawRollingSum / _rawRollingQueue.Count : rawCount;
                _lastAverageRaw = avgRaw;

                UpdateRollingQueue(_weightRollingQueue, ref _weightRollingSum, weight, windowSize);
                avgWeight = _weightRollingQueue.Count > 0 ? _weightRollingSum / _weightRollingQueue.Count : weight;
                _lastAverageWeight = avgWeight;

                _stabilityQueue.Enqueue(new TimedValue(timestamp, weight));
                while (_stabilityQueue.Count > 0 && (timestamp - _stabilityQueue.Peek().Timestamp).TotalMilliseconds > StabilityWindowMilliseconds)
                {
                    _stabilityQueue.Dequeue();
                }

                stdDev = ComputeStandardDeviation(_stabilityQueue);
                _lastStdDev = stdDev;
                isStable = !double.IsNaN(stdDev) && _stabilityQueue.Count >= 5 && stdDev <= _stabilityThreshold;
                _lastStable = isStable;

                _chartPending.Enqueue(new ChartPoint(timestamp, weight, avgWeight));
                while (_chartPending.Count > ChartMaxPoints)
                {
                    _chartPending.Dequeue();
                }

                _sampleIndex++;
            }
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            int raw;
            double avgRaw;
            double voltage;
            double weight;
            double avgWeight;
            double stdDev;
            bool isStable;
            List<ChartPoint> pendingPoints = null;

            lock (_sampleLock)
            {
                if (!_hasSample)
                {
                    return;
                }

                raw = _lastRaw;
                avgRaw = _lastAverageRaw;
                voltage = _lastVoltage;
                weight = _lastWeight;
                avgWeight = _lastAverageWeight;
                stdDev = _lastStdDev;
                isStable = _lastStable;

                if (_chartPending.Count > 0)
                {
                    pendingPoints = new List<ChartPoint>(_chartPending.Count);
                    while (_chartPending.Count > 0)
                    {
                        pendingPoints.Add(_chartPending.Dequeue());
                    }
                }
            }

            UpdateUi(raw, avgRaw, voltage, weight, avgWeight, stdDev, isStable);

            if (pendingPoints != null && pendingPoints.Count > 0)
            {
                UpdateChart(pendingPoints);
            }
        }

        private void UpdateUi(int raw, double avgRaw, double voltage, double weight, double avgWeight, double stdDev, bool isStable)
        {
            txtRawAdc.Text = raw.ToString(CultureInfo.InvariantCulture);
            txtAverageRaw.Text = avgRaw.ToString("F1", CultureInfo.InvariantCulture);
            txtVoltage.Text = voltage.ToString("F3", CultureInfo.InvariantCulture);
            txtWeight.Text = weight.ToString("F3", CultureInfo.InvariantCulture);
            txtAverageWeight.Text = avgWeight.ToString("F3", CultureInfo.InvariantCulture);
            txtStdDev.Text = double.IsNaN(stdDev) ? "--" : stdDev.ToString("F4", CultureInfo.InvariantCulture);

            lblStabilityStatus.Text = isStable ? "Stable" : "Unstable";
            lblStabilityStatus.ForeColor = isStable ? Color.ForestGreen : Color.Firebrick;
        }

        private void UpdateChart(IEnumerable<ChartPoint> points)
        {
            var area = chartData.ChartAreas[0];
            Series weightSeries = chartData.Series["Weight"];
            Series avgSeries = chartData.Series["AvgWeight"];

            foreach (ChartPoint point in points)
            {
                double x = point.Timestamp.ToOADate();
                weightSeries.Points.AddXY(x, point.Weight);
                avgSeries.Points.AddXY(x, point.AverageWeight);
            }

            while (weightSeries.Points.Count > ChartMaxPoints)
            {
                weightSeries.Points.RemoveAt(0);
            }

            while (avgSeries.Points.Count > ChartMaxPoints)
            {
                avgSeries.Points.RemoveAt(0);
            }

            if (weightSeries.Points.Count > 0)
            {
                area.AxisX.Minimum = weightSeries.Points[0].XValue;
                area.AxisX.Maximum = weightSeries.Points[weightSeries.Points.Count - 1].XValue;
            }

            chartData.Invalidate();
        }

        private static double ComputeStandardDeviation(IEnumerable<TimedValue> samples)
        {
            int count = 0;
            double mean = 0;
            double m2 = 0;

            foreach (TimedValue sample in samples)
            {
                double value = sample.Value;
                count++;
                double delta = value - mean;
                mean += delta / count;
                m2 += delta * (value - mean);
            }

            if (count < 2)
            {
                return double.NaN;
            }

            return Math.Sqrt(m2 / (count - 1));
        }

        private static void UpdateRollingQueue(Queue<double> queue, ref double sum, double value, int maxLength)
        {
            queue.Enqueue(value);
            sum += value;

            while (queue.Count > maxLength)
            {
                sum -= queue.Dequeue();
            }
        }

        private static void TrimRollingQueue(Queue<double> queue, ref double sum, int maxLength)
        {
            while (queue.Count > maxLength)
            {
                sum -= queue.Dequeue();
            }
        }

        private void btnTare_Click(object sender, EventArgs e)
        {
            if (!_hasSample)
            {
                MessageBox.Show(this, "No samples received yet. Wait for data before taring.", "Tare unavailable", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            double newTare;
            lock (_sampleLock)
            {
                double avgRaw = _rawRollingQueue.Count > 0 ? _rawRollingSum / _rawRollingQueue.Count : _lastRaw;
                newTare = (_scaleSlope * avgRaw) + _scaleIntercept;
                _tareOffset = newTare;
            }

            UpdateTareDisplay(newTare);
        }

        private void btnResetTare_Click(object sender, EventArgs e)
        {
            lock (_sampleLock)
            {
                _tareOffset = 0;
            }

            UpdateTareDisplay(0);
        }

        private void numAverageWindow_ValueChanged(object sender, EventArgs e)
        {
            int newSize = (int)numAverageWindow.Value;

            lock (_sampleLock)
            {
                _rollingWindowSize = newSize;
                TrimRollingQueue(_rawRollingQueue, ref _rawRollingSum, newSize);
                TrimRollingQueue(_weightRollingQueue, ref _weightRollingSum, newSize);
            }
        }

        private void numStabilityThreshold_ValueChanged(object sender, EventArgs e)
        {
            double newThreshold = (double)numStabilityThreshold.Value;
            lock (_sampleLock)
            {
                _stabilityThreshold = newThreshold;
            }
        }

        private void btnAddCalibrationPoint_Click(object sender, EventArgs e)
        {
            if (!_hasSample)
            {
                MessageBox.Show(this, "No sensor samples yet. Wait until readings appear before capturing calibration data.", "Calibration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            double knownMass = (double)numKnownMass.Value;
            double avgRaw;
            double stdDev;
            bool isStable;

            lock (_sampleLock)
            {
                avgRaw = _rawRollingQueue.Count > 0 ? _rawRollingSum / _rawRollingQueue.Count : _lastRaw;
                stdDev = _lastStdDev;
                isStable = _lastStable;
            }

            if (!isStable)
            {
                DialogResult response = MessageBox.Show(
                    this,
                    $"Current measurement is unstable (std dev {stdDev:F4} kg). Capture anyway?",
                    "Unstable measurement",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (response != DialogResult.Yes)
                {
                    return;
                }
            }

            var point = new CalibrationPoint(avgRaw, knownMass);
            _calibrationPoints.Add(point);

            UpdateCalibrationListDisplay();
            RecomputeCalibration();
        }

        private void btnClearCalibration_Click(object sender, EventArgs e)
        {
            if (_calibrationPoints.Count == 0)
            {
                return;
            }

            DialogResult result = MessageBox.Show(this, "Clear all calibration points?", "Clear calibration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }

            _calibrationPoints.Clear();

            lock (_sampleLock)
            {
                _scaleSlope = 0;
                _scaleIntercept = 0;
            }

            UpdateCalibrationListDisplay();
            UpdateCalibrationSummaryDisplay();
        }

        private void UpdateCalibrationListDisplay()
        {
            lstCalibration.BeginUpdate();
            lstCalibration.Items.Clear();

            foreach (CalibrationPoint point in _calibrationPoints)
            {
                string entry = string.Format(CultureInfo.InvariantCulture, "ADC {0:F1} -> {1:F3} kg", point.Raw, point.Mass);
                lstCalibration.Items.Add(entry);
            }

            lstCalibration.EndUpdate();
        }

        private void RecomputeCalibration()
        {
            double slope;
            double intercept;

            if (_calibrationPoints.Count == 0)
            {
                slope = 0;
                intercept = 0;
            }
            else if (_calibrationPoints.Count == 1)
            {
                CalibrationPoint single = _calibrationPoints[0];
                slope = single.Raw != 0 ? single.Mass / single.Raw : 0;
                intercept = 0;
            }
            else
            {
                double n = _calibrationPoints.Count;
                double sumX = 0;
                double sumY = 0;
                double sumXY = 0;
                double sumX2 = 0;

                foreach (CalibrationPoint point in _calibrationPoints)
                {
                    sumX += point.Raw;
                    sumY += point.Mass;
                    sumXY += point.Raw * point.Mass;
                    sumX2 += point.Raw * point.Raw;
                }

                double denominator = (n * sumX2) - (sumX * sumX);
                if (Math.Abs(denominator) < 1e-9)
                {
                    slope = 0;
                    intercept = sumY / n;
                }
                else
                {
                    slope = ((n * sumXY) - (sumX * sumY)) / denominator;
                    intercept = (sumY - (slope * sumX)) / n;
                }
            }

            lock (_sampleLock)
            {
                _scaleSlope = slope;
                _scaleIntercept = intercept;
            }

            UpdateCalibrationSummaryDisplay();
        }

        private void UpdateCalibrationSummaryDisplay()
        {
            double slope;
            double intercept;

            lock (_sampleLock)
            {
                slope = _scaleSlope;
                intercept = _scaleIntercept;
            }

            txtSlope.Text = slope.ToString("F6", CultureInfo.InvariantCulture);
            txtIntercept.Text = intercept.ToString("F6", CultureInfo.InvariantCulture);
        }

        private void UpdateTareDisplay()
        {
            double tare;
            lock (_sampleLock)
            {
                tare = _tareOffset;
            }

            UpdateTareDisplay(tare);
        }

        private void UpdateTareDisplay(double tareValue)
        {
            txtTare.Text = tareValue.ToString("F3", CultureInfo.InvariantCulture);
        }
    }
}

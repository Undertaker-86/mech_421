using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Ryan_part1_ex2_c_
{
    public partial class Form1 : Form
    {
        private readonly SerialPort _port = new SerialPort();
        private readonly object _sync = new object();
        private readonly List<CaptureSample> _captureBuffer = new List<CaptureSample>();
        private readonly List<CaptureResult> _captureResults = new List<CaptureResult>();
        private StreamWriter _csvWriter;

        private byte _frameState;
        private byte _ms5;
        private bool _capturing;
        private DateTime _captureStartUtc;

        private const double Vref = 3.300;
        private const double RFixed = 10000.0;
        private const double R0 = 10000.0;
        private const double Beta = 3435.0;
        private const double T0K = 298.15;

        private double _a0 = 0.0;
        private double _a1 = 1.0;

        private const int ChartHistoryPoints = 600;

        public Form1()
        {
            InitializeComponent();
            InitialiseSerialPort();
            HookUiEvents();
            PopulatePortList();
            UpdateStatus("Idle");

            txtLog.ReadOnly = true;
            txtAdjusted.ReadOnly = true;
            txtRaw.ReadOnly = true;
            txtError.ReadOnly = true;
            txtVoltage.ReadOnly = true;
            txtResistance.ReadOnly = true;
            txtCsvPath.ReadOnly = true;
            txtAnalysis.ReadOnly = true;

            if (comboCaptureScenario.Items.Count > 0)
            {
                comboCaptureScenario.SelectedIndex = 0;
            }

            ConfigureChart();
        }

        private void InitialiseSerialPort()
        {
            _port.BaudRate = 9600;
            _port.DataBits = 8;
            _port.Parity = Parity.None;
            _port.StopBits = StopBits.One;
            _port.Handshake = Handshake.None;
            _port.DataReceived += PortOnDataReceived;
            FormClosing += Form1_FormClosing;
        }

        private void HookUiEvents()
        {
            btnRefreshPorts.Click += BtnRefreshPorts_Click;
            btnConnect.Click += BtnConnect_Click;
            btnApplyCalibration.Click += BtnApplyCalibration_Click;
            chkLogging.CheckedChanged += ChkLogging_CheckedChanged;
            btnStartCapture.Click += BtnStartCapture_Click;
            btnStopCapture.Click += BtnStopCapture_Click;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_port.IsOpen)
            {
                _port.DataReceived -= PortOnDataReceived;
                _port.Close();
            }

            if (_csvWriter != null)
            {
                _csvWriter.Flush();
                _csvWriter.Dispose();
                _csvWriter = null;
            }
        }

        private void BtnRefreshPorts_Click(object sender, EventArgs e)
        {
            PopulatePortList();
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            ToggleConnection();
        }

        private void BtnApplyCalibration_Click(object sender, EventArgs e)
        {
            ApplyCalibrationFromUi();
        }

        private void ChkLogging_CheckedChanged(object sender, EventArgs e)
        {
            EnsureCsvWriter(false);
        }

        private void BtnStartCapture_Click(object sender, EventArgs e)
        {
            StartCapture();
        }

        private void BtnStopCapture_Click(object sender, EventArgs e)
        {
            StopCapture();
        }

        private void ConfigureChart()
        {
            ChartArea area = chartTemp.ChartAreas[0];
            area.AxisX.LabelStyle.Format = "HH:mm:ss";
            area.AxisX.IntervalType = DateTimeIntervalType.Seconds;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.Title = "Temperature (degC)";
            area.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(60, System.Drawing.Color.Gray);
            chartTemp.Series.Clear();

            Series seriesRaw = new Series("Raw degC");
            seriesRaw.ChartType = SeriesChartType.Line;
            seriesRaw.XValueType = ChartValueType.DateTime;
            seriesRaw.BorderWidth = 2;
            seriesRaw.Color = System.Drawing.Color.FromArgb(120, System.Drawing.Color.SteelBlue);

            Series seriesAdj = new Series("Compensated degC");
            seriesAdj.ChartType = SeriesChartType.Line;
            seriesAdj.XValueType = ChartValueType.DateTime;
            seriesAdj.BorderWidth = 2;
            seriesAdj.Color = System.Drawing.Color.Crimson;

            chartTemp.Series.Add(seriesRaw);
            chartTemp.Series.Add(seriesAdj);
        }

        private void PopulatePortList()
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports, StringComparer.OrdinalIgnoreCase);

            comboPorts.Items.Clear();
            comboPorts.Items.AddRange(ports);
            if (ports.Length > 0)
            {
                comboPorts.SelectedIndex = 0;
            }
        }

        private void ToggleConnection()
        {
            try
            {
                if (_port.IsOpen)
                {
                    _port.Close();
                    btnConnect.Text = "Connect";
                    UpdateStatus("Disconnected");
                    EnsureCsvWriter(true);
                    return;
                }

                if (comboPorts.SelectedItem == null)
                {
                    MessageBox.Show("Select a COM port to connect.", "Thermistor",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _port.PortName = comboPorts.SelectedItem.ToString();
                _port.Open();
                _frameState = 0;
                btnConnect.Text = "Disconnect";
                UpdateStatus(string.Format("Streaming on {0}", _port.PortName));
                EnsureCsvWriter(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to toggle connection: " + ex.Message, "Thermistor",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Error");
            }
        }

        private void EnsureCsvWriter(bool closeOnly)
        {
            if (_csvWriter != null)
            {
                _csvWriter.Flush();
                _csvWriter.Dispose();
                _csvWriter = null;
            }

            if (closeOnly || !chkLogging.Checked)
            {
                txtCsvPath.Text = "(logging disabled)";
                return;
            }

            try
            {
                string capturesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "captures");
                Directory.CreateDirectory(capturesDir);
                string filePath = Path.Combine(capturesDir,
                    string.Format("thermistor_{0:yyyyMMdd_HHmmss}.csv", DateTime.Now));
                _csvWriter = new StreamWriter(filePath, false, Encoding.UTF8);
                _csvWriter.WriteLine("timestamp,code10,vout_v,resistance_ohm,temp_raw_c,temp_comp_c,error_c");
                txtCsvPath.Text = filePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show("CSV logging disabled: " + ex.Message, "Thermistor",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkLogging.Checked = false;
                txtCsvPath.Text = "(logging disabled)";
            }
        }

        private void PortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (_port.IsOpen && _port.BytesToRead > 0)
            {
                int value = _port.ReadByte();
                if (value < 0)
                {
                    return;
                }

                switch (_frameState)
                {
                    case 0:
                        if (value == 0xFF)
                        {
                            _frameState = 1;
                        }
                        break;
                    case 1:
                        _ms5 = (byte)(value & 0x1F);
                        _frameState = 2;
                        break;
                    case 2:
                        _frameState = 0;
                        byte ls5 = (byte)(value & 0x1F);
                        int code10 = (_ms5 << 5) | ls5;
                        HandleSample(code10);
                        break;
                }
            }
        }

        private void HandleSample(int code10)
        {
            if (code10 < 0 || code10 > 1023)
            {
                return;
            }

            DateTime timestamp = DateTime.Now;
            double vout = code10 * Vref / 1023.0;
            double resistance;
            if (vout <= 0.0001 || Math.Abs(Vref - vout) < 0.0001)
            {
                resistance = double.NaN;
            }
            else
            {
                resistance = RFixed * (vout / (Vref - vout));
            }

            double tempRaw = CalculateBetaTemperature(resistance);
            double tempComp = _a0 + _a1 * tempRaw;
            double error = tempComp - tempRaw;

            if (_csvWriter != null)
            {
                string line = string.Format(CultureInfo.InvariantCulture,
                    "{0:O},{1},{2:F6},{3:F2},{4:F3},{5:F3},{6:F3}",
                    timestamp, code10, vout, resistance, tempRaw, tempComp, error);
                try
                {
                    _csvWriter.WriteLine(line);
                }
                catch (IOException)
                {
                    BeginInvoke(new Action(delegate
                    {
                        AppendLog("Warning: unable to write to CSV (disk busy?).");
                    }));
                }
            }

            lock (_sync)
            {
                if (_capturing)
                {
                    double seconds = (timestamp.ToUniversalTime() - _captureStartUtc).TotalSeconds;
                    _captureBuffer.Add(new CaptureSample(seconds, tempComp));
                }
            }

            LiveSample sample = new LiveSample();
            sample.Timestamp = timestamp;
            sample.TempRaw = tempRaw;
            sample.TempComp = tempComp;
            sample.Voltage = vout;
            sample.Resistance = resistance;
            sample.Error = error;

            BeginInvoke(new Action(delegate
            {
                UpdateLiveDisplay(sample);
            }));
        }

        private static double CalculateBetaTemperature(double resistance)
        {
            if (double.IsNaN(resistance) || resistance <= 0.0)
            {
                return double.NaN;
            }

            double invT = (1.0 / T0K) + (1.0 / Beta) * Math.Log(resistance / R0);
            return (1.0 / invT) - 273.15;
        }

        private void UpdateLiveDisplay(LiveSample sample)
        {
            txtRaw.Text = double.IsNaN(sample.TempRaw)
                ? "Error"
                : sample.TempRaw.ToString("F2", CultureInfo.InvariantCulture);
            txtAdjusted.Text = double.IsNaN(sample.TempComp)
                ? "Error"
                : sample.TempComp.ToString("F2", CultureInfo.InvariantCulture);
            txtError.Text = double.IsNaN(sample.Error)
                ? "-"
                : sample.Error.ToString("F3", CultureInfo.InvariantCulture);
            txtVoltage.Text = sample.Voltage.ToString("F4", CultureInfo.InvariantCulture);
            txtResistance.Text = double.IsNaN(sample.Resistance)
                ? "-"
                : sample.Resistance.ToString("F0", CultureInfo.InvariantCulture);

            Series seriesRaw = chartTemp.Series["Raw degC"];
            Series seriesAdj = chartTemp.Series["Compensated degC"];
            double xValue = sample.Timestamp.ToOADate();

            seriesRaw.Points.AddXY(xValue, sample.TempRaw);
            seriesAdj.Points.AddXY(xValue, sample.TempComp);

            TrimSeries(seriesRaw);
            TrimSeries(seriesAdj);

            if (seriesAdj.Points.Count > 1)
            {
                ChartArea area = chartTemp.ChartAreas[0];
                area.AxisX.Minimum = seriesAdj.Points[0].XValue;
                area.AxisX.Maximum = seriesAdj.Points[seriesAdj.Points.Count - 1].XValue;
            }

            AppendLog(string.Format(
                "{0:HH:mm:ss.fff} | Raw {1:F2} degC | Comp {2:F2} degC | Err {3:F3} degC",
                sample.Timestamp, sample.TempRaw, sample.TempComp, sample.Error));
        }

        private void TrimSeries(Series series)
        {
            while (series.Points.Count > ChartHistoryPoints)
            {
                series.Points.RemoveAt(0);
            }
        }

        private void AppendLog(string message)
        {
            const int maxLines = 300;
            txtLog.AppendText(message + Environment.NewLine);
            string[] lines = txtLog.Lines;
            if (lines.Length > maxLines)
            {
                string[] trimmed = lines.Skip(lines.Length - maxLines).ToArray();
                txtLog.Lines = trimmed;
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }
        }

        private void ApplyCalibrationFromUi()
        {
            double newA0;
            double newA1;
            if (!double.TryParse(txtA0.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out newA0) ||
                !double.TryParse(txtA1.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out newA1))
            {
                MessageBox.Show("Enter valid numeric values for a0 and a1 (use '.' for decimals).",
                    "Thermistor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _a0 = newA0;
            _a1 = newA1;
            AppendLog(string.Format("Calibration updated: a0={0}, a1={1}", _a0, _a1));
        }

        private void StartCapture()
        {
            if (!_port.IsOpen)
            {
                MessageBox.Show("Connect to the MSP430 before starting a capture.", "Thermistor",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            lock (_sync)
            {
                _captureBuffer.Clear();
                _captureStartUtc = DateTime.UtcNow;
                _capturing = true;
            }

            AppendLog(string.Format("Capture started ({0})", comboCaptureScenario.Text));
            UpdateStatus("Capturing waveform...");
            btnStartCapture.Enabled = false;
            btnStopCapture.Enabled = true;
        }

        private void StopCapture()
        {
            List<CaptureSample> snapshot;
            lock (_sync)
            {
                if (!_capturing)
                {
                    return;
                }

                _capturing = false;
                snapshot = new List<CaptureSample>(_captureBuffer);
            }

            btnStartCapture.Enabled = true;
            btnStopCapture.Enabled = false;
            UpdateStatus("Capture complete");

            if (snapshot.Count < 10)
            {
                AppendLog("Capture aborted: insufficient samples.");
                return;
            }

            string scenario = comboCaptureScenario.Text;
            string captureFile = SaveCaptureCsv(snapshot, scenario);
            CaptureResult result = AnalyzeCapture(snapshot, scenario, captureFile);
            _captureResults.Add(result);
            AddCaptureToList(result);
            txtAnalysis.Text = BuildAnalysisText(result);

            AppendLog(string.Format(
                "Capture finished ({0}) -> Tau63={1:F1}s, TauFit={2:F2}s, R2={3:F3}",
                scenario, result.Tau63, result.TauFit, result.FitR2));
        }

        private CaptureResult AnalyzeCapture(IList<CaptureSample> samples, string label, string captureFile)
        {
            List<CaptureSample> ordered = new List<CaptureSample>(samples);
            ordered.Sort(delegate (CaptureSample a, CaptureSample b)
            {
                return a.Seconds.CompareTo(b.Seconds);
            });

            if (ordered.Count == 0)
            {
                return new CaptureResult(label, 0.0, double.NaN, double.NaN, double.NaN, double.NaN,
                    double.NaN, captureFile, "No samples captured.");
            }

            int lastIndex = ordered.Count - 1;
            double duration = ordered[lastIndex].Seconds - ordered[0].Seconds;
            double startTemp = ordered[0].Temperature;

            double finalTemp;
            if (ordered.Count >= 6)
            {
                double sum = 0.0;
                int count = 0;
                int startIndex = Math.Max(ordered.Count - 6, 0);
                for (int i = startIndex; i < ordered.Count; i++)
                {
                    sum += ordered[i].Temperature;
                    count++;
                }
                finalTemp = count > 0 ? sum / count : ordered[lastIndex].Temperature;
            }
            else
            {
                finalTemp = ordered[lastIndex].Temperature;
            }

            double delta = finalTemp - startTemp;

            List<string> notes = new List<string>();
            if (Math.Abs(delta) < 0.5)
            {
                notes.Add("Delta-T too small for reliable fit.");
            }

            double tau63 = EstimateTauBy63Percent(ordered, startTemp, finalTemp);
            if (double.IsNaN(tau63))
            {
                notes.Add("63% threshold not reached.");
            }

            RegressionResult regression = EstimateTauByRegression(ordered, startTemp, finalTemp);
            if (double.IsNaN(regression.TauFit) || double.IsNaN(regression.FitR2))
            {
                notes.Add("Regression fit failed.");
            }

            string notesCombined = string.Join(" ", notes.ToArray()).Trim();

            return new CaptureResult(label, duration, startTemp, finalTemp, tau63, regression.TauFit,
                regression.FitR2, captureFile, notesCombined);
        }

        private string SaveCaptureCsv(IEnumerable<CaptureSample> samples, string scenario)
        {
            try
            {
                string capturesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "captures");
                Directory.CreateDirectory(capturesDir);

                string sanitized = string.Join("_", scenario.Split(Path.GetInvalidFileNameChars()))
                    .Replace(' ', '_');
                while (sanitized.Contains("__"))
                {
                    sanitized = sanitized.Replace("__", "_");
                }
                sanitized = sanitized.Trim('_');
                if (string.IsNullOrEmpty(sanitized))
                {
                    sanitized = "capture";
                }

                string filePath = Path.Combine(capturesDir,
                    string.Format("capture_{0:yyyyMMdd_HHmmss}_{1}.csv", DateTime.Now, sanitized));

                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    writer.WriteLine("seconds,temp_comp_c");
                    List<CaptureSample> ordered = samples.OrderBy(s => s.Seconds).ToList();
                    for (int i = 0; i < ordered.Count; i++)
                    {
                        CaptureSample sample = ordered[i];
                        writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                            "{0:F3},{1:F3}", sample.Seconds, sample.Temperature));
                    }
                }

                return filePath;
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action(delegate
                {
                    AppendLog("Capture CSV save failed: " + ex.Message);
                }));
                return string.Empty;
            }
        }

        private static double EstimateTauBy63Percent(IList<CaptureSample> samples,
            double startTemp, double finalTemp)
        {
            double step = finalTemp - startTemp;
            if (Math.Abs(step) < 0.5)
            {
                return double.NaN;
            }

            double target = finalTemp - (finalTemp - startTemp) * Math.Exp(-1.0);
            bool heating = step > 0;

            for (int i = 0; i < samples.Count; i++)
            {
                CaptureSample sample = samples[i];
                if (heating && sample.Temperature >= target)
                {
                    return sample.Seconds;
                }

                if (!heating && sample.Temperature <= target)
                {
                    return sample.Seconds;
                }
            }

            return double.NaN;
        }

        private static RegressionResult EstimateTauByRegression(
            IList<CaptureSample> samples, double startTemp, double finalTemp)
        {
            double delta = finalTemp - startTemp;
            if (Math.Abs(delta) < 0.5)
            {
                return new RegressionResult(double.NaN, double.NaN);
            }

            List<double> times = new List<double>();
            List<double> lnValues = new List<double>();
            for (int i = 0; i < samples.Count; i++)
            {
                CaptureSample sample = samples[i];
                if (sample.Seconds <= 0.05)
                {
                    continue;
                }

                double diff = Math.Abs(finalTemp - sample.Temperature);
                if (diff <= 1e-3)
                {
                    continue;
                }

                if (diff > Math.Abs(delta))
                {
                    continue;
                }

                if (diff <= Math.Abs(delta) * 0.02)
                {
                    continue;
                }

                times.Add(sample.Seconds);
                lnValues.Add(Math.Log(diff));
            }

            if (times.Count < 5)
            {
                return new RegressionResult(double.NaN, double.NaN);
            }

            int n = times.Count;
            double sumX = 0.0;
            double sumY = 0.0;
            double sumXY = 0.0;
            double sumX2 = 0.0;
            for (int i = 0; i < n; i++)
            {
                double x = times[i];
                double y = lnValues[i];
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            double denominator = n * sumX2 - sumX * sumX;
            if (Math.Abs(denominator) < 1e-12)
            {
                return new RegressionResult(double.NaN, double.NaN);
            }

            double slope = (n * sumXY - sumX * sumY) / denominator;
            double intercept = (sumY - slope * sumX) / n;

            double tauFit = double.NaN;
            if (slope < 0.0)
            {
                tauFit = -1.0 / slope;
            }

            double meanY = sumY / n;
            double ssTot = 0.0;
            double ssRes = 0.0;
            for (int i = 0; i < n; i++)
            {
                double x = times[i];
                double y = lnValues[i];
                double est = slope * x + intercept;
                ssTot += Math.Pow(y - meanY, 2);
                ssRes += Math.Pow(y - est, 2);
            }

            double r2 = ssTot <= 1e-12 ? double.NaN : 1.0 - (ssRes / ssTot);
            return new RegressionResult(tauFit, r2);
        }

        private void AddCaptureToList(CaptureResult result)
        {
            ListViewItem item = new ListViewItem(result.Label);
            item.Tag = result;
            item.SubItems.Add(result.Duration.ToString("F1", CultureInfo.InvariantCulture));
            item.SubItems.Add(result.StartTemp.ToString("F2", CultureInfo.InvariantCulture));
            item.SubItems.Add(result.FinalTemp.ToString("F2", CultureInfo.InvariantCulture));
            item.SubItems.Add(double.IsNaN(result.Tau63) ? "-" : result.Tau63.ToString("F2", CultureInfo.InvariantCulture));
            item.SubItems.Add(double.IsNaN(result.TauFit) ? "-" : result.TauFit.ToString("F2", CultureInfo.InvariantCulture));
            item.SubItems.Add(double.IsNaN(result.FitR2) ? "-" : result.FitR2.ToString("F3", CultureInfo.InvariantCulture));
            item.SubItems.Add(string.IsNullOrEmpty(result.CsvPath) ? string.Empty : result.CsvPath);
            item.SubItems.Add(string.IsNullOrEmpty(result.Notes) ? string.Empty : result.Notes);
            listCaptures.Items.Add(item);
        }

        private static string BuildAnalysisText(CaptureResult result)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("Scenario: {0}", result.Label));
            builder.AppendLine(string.Format("Duration: {0:F1} s", result.Duration));
            builder.AppendLine(string.Format("Start temperature: {0:F2} degC", result.StartTemp));
            builder.AppendLine(string.Format("Final temperature: {0:F2} degC", result.FinalTemp));
            builder.AppendLine("Tau (63%): " + (double.IsNaN(result.Tau63) ? "N/A" : result.Tau63.ToString("F2")) + " s");
            builder.AppendLine("Tau (log regression): " + (double.IsNaN(result.TauFit) ? "N/A" : result.TauFit.ToString("F2")) + " s");
            builder.AppendLine("Regression R2: " + (double.IsNaN(result.FitR2) ? "N/A" : result.FitR2.ToString("F3")));
            if (!string.IsNullOrEmpty(result.CsvPath))
            {
                builder.AppendLine(string.Format("Capture file: {0}", result.CsvPath));
            }
            if (!string.IsNullOrEmpty(result.Notes))
            {
                builder.AppendLine();
                builder.AppendLine(result.Notes);
            }

            return builder.ToString();
        }

        private void UpdateStatus(string status)
        {
            lblStatus.Text = status;
        }

        private T InvokeIfRequired<T>(Func<T> func)
        {
            if (InvokeRequired)
            {
                return (T)Invoke(func);
            }

            return func();
        }

        private sealed class LiveSample
        {
            public DateTime Timestamp;
            public double TempRaw;
            public double TempComp;
            public double Voltage;
            public double Resistance;
            public double Error;
        }

        private sealed class CaptureSample
        {
            public CaptureSample(double seconds, double temperature)
            {
                Seconds = seconds;
                Temperature = temperature;
            }

            public double Seconds { get; private set; }
            public double Temperature { get; private set; }
        }

        private sealed class CaptureResult
        {
            public CaptureResult(string label, double duration, double startTemp, double finalTemp,
                double tau63, double tauFit, double fitR2, string csvPath, string notes)
            {
                Label = label;
                Duration = duration;
                StartTemp = startTemp;
                FinalTemp = finalTemp;
                Tau63 = tau63;
                TauFit = tauFit;
                FitR2 = fitR2;
                CsvPath = csvPath;
                Notes = notes;
            }

            public string Label { get; private set; }
            public double Duration { get; private set; }
            public double StartTemp { get; private set; }
            public double FinalTemp { get; private set; }
            public double Tau63 { get; private set; }
            public double TauFit { get; private set; }
            public double FitR2 { get; private set; }
            public string CsvPath { get; private set; }
            public string Notes { get; private set; }
        }

        private sealed class RegressionResult
        {
            public RegressionResult(double tauFit, double fitR2)
            {
                TauFit = tauFit;
                FitR2 = fitR2;
            }

            public double TauFit { get; private set; }
            public double FitR2 { get; private set; }
        }
    }
}

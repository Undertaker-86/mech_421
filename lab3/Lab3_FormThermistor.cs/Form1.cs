using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;

namespace Ryan_part1_ex2_c_
{
    public partial class FormThermistor : Form
    {
        // ===== Serial & parsing =====
        SerialPort sp = new SerialPort();
        byte state = 0, ms5, ls5;

        // ===== Calibration constants =====
        const double VREF = 3.300, R_FIXED = 10000.0, R0 = 10000.0, BETA = 3435.0, T0K = 298.15;
        double a0 = 0.0, a1 = 1.0;

        CultureInfo ci = CultureInfo.InvariantCulture;
        StreamWriter csv;
        int decim = 10, sampleCount = 0;
        bool logging = false;

        ComboBox comboPorts;
        Button btnRefresh, btnConnect, btnStart, btnStop, btnTau;
        NumericUpDown nudDecim, nudA0, nudA1;
        TextBox txtRawC, txtAdjC, txtErrC;
        Chart chart;

        List<DateTime> tBuf = new List<DateTime>(4096);
        List<double> TBuf = new List<double>(4096);

        public FormThermistor()
        {
            InitializeComponent();
            BuildUi();

            sp.BaudRate = 9600; sp.DataBits = 8; sp.Parity = Parity.None; sp.StopBits = StopBits.One;
            sp.DataReceived += Sp_DataReceived;

            RefreshPorts();
            this.FormClosing += (s, e) => { try { if (sp.IsOpen) sp.Close(); } catch { } csv?.Dispose(); };
        }

        void BuildUi()
        {
            Text = "Thermistor DAQ (MSP430FR5739)";
            Width = 1100; Height = 700;

            var p = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 3 };
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            p.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            p.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            p.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(p);

            comboPorts = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            btnRefresh = new Button { Text = "Refresh" };
            btnConnect = new Button { Text = "Connect" };
            btnStart = new Button { Text = "Start Log" };
            btnStop = new Button { Text = "Stop Log", Enabled = false };
            btnTau = new Button { Text = "Estimate τ" };

            p.Controls.Add(new Label { Text = "Port:", TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 0, 0);
            p.Controls.Add(comboPorts, 1, 0);
            p.Controls.Add(btnRefresh, 2, 0);
            p.Controls.Add(btnConnect, 3, 0);
            p.Controls.Add(btnStart, 4, 0);
            p.Controls.Add(btnStop, 5, 0);

            nudDecim = new NumericUpDown { Minimum = 1, Maximum = 1000, Value = decim, Dock = DockStyle.Fill };
            nudA0 = new NumericUpDown { DecimalPlaces = 3, Minimum = -1000, Maximum = 1000, Increment = 0.01M, Value = (decimal)a0, Dock = DockStyle.Fill };
            nudA1 = new NumericUpDown { DecimalPlaces = 4, Minimum = -10, Maximum = 10, Increment = 0.001M, Value = (decimal)a1, Dock = DockStyle.Fill };
            txtRawC = new TextBox { ReadOnly = true, Dock = DockStyle.Fill };
            txtAdjC = new TextBox { ReadOnly = true, Dock = DockStyle.Fill };
            txtErrC = new TextBox { ReadOnly = true, Dock = DockStyle.Fill };

            p.Controls.Add(new Label { Text = "Decimate N:", TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 0, 1);
            p.Controls.Add(nudDecim, 1, 1);
            p.Controls.Add(new Label { Text = "a0 (°C):", TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 2, 1);
            p.Controls.Add(nudA0, 3, 1);
            p.Controls.Add(new Label { Text = "a1 (×):", TextAlign = System.Drawing.ContentAlignment.MiddleRight }, 4, 1);
            p.Controls.Add(nudA1, 5, 1);

            var rp = new FlowLayoutPanel { Dock = DockStyle.Top, FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            rp.Controls.Add(new Label { Text = "Raw (°C):" }); rp.Controls.Add(txtRawC);
            rp.Controls.Add(new Label { Text = "Adjusted (°C):" }); rp.Controls.Add(txtAdjC);
            rp.Controls.Add(new Label { Text = "Error (°C):" }); rp.Controls.Add(txtErrC);
            rp.Controls.Add(btnTau);
            p.Controls.Add(rp, 0, 2); p.SetColumnSpan(rp, 6);

            chart = new Chart { Dock = DockStyle.Fill };
            var area = new ChartArea("a1"); area.AxisX.LabelStyle.Format = "HH:mm:ss";
            chart.ChartAreas.Add(area);
            chart.Series.Add(new Series("Temp °C") { ChartType = SeriesChartType.FastLine, XValueType = ChartValueType.DateTime });
            chart.Series.Add(new Series("Adj °C") { ChartType = SeriesChartType.FastLine, XValueType = ChartValueType.DateTime });
            chart.Legends.Add(new Legend());
            Controls.Add(chart);

            btnRefresh.Click += (s, e) => RefreshPorts();
            btnConnect.Click += (s, e) => ToggleConnect();
            btnStart.Click += (s, e) => StartLogging();
            btnStop.Click += (s, e) => StopLogging();
            nudDecim.ValueChanged += (s, e) => decim = (int)nudDecim.Value;
            nudA0.ValueChanged += (s, e) => a0 = (double)nudA0.Value;
            nudA1.ValueChanged += (s, e) => a1 = (double)nudA1.Value;
            btnTau.Click += (s, e) => EstimateTauFromBuffer();
        }

        void RefreshPorts()
        {
            comboPorts.Items.Clear();
            comboPorts.Items.AddRange(SerialPort.GetPortNames());
            if (comboPorts.Items.Count > 0) comboPorts.SelectedIndex = 0;
        }

        void ToggleConnect()
        {
            try
            {
                if (!sp.IsOpen)
                {
                    if (comboPorts.SelectedItem == null) { MessageBox.Show("Select a COM port"); return; }
                    sp.PortName = comboPorts.SelectedItem.ToString();
                    sp.Open(); btnConnect.Text = "Disconnect";
                }
                else { sp.Close(); btnConnect.Text = "Connect"; }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        void StartLogging()
        {
            if (logging) return;
            using (var sfd = new SaveFileDialog { Filter = "CSV|*.csv", FileName = "thermistor.csv" })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;
                csv = new StreamWriter(sfd.FileName, false, Encoding.UTF8);
            }
            csv.WriteLine("time,code10,Vout,Rntc,TempC,AdjC");
            logging = true; btnStart.Enabled = false; btnStop.Enabled = true;
            chart.Series[0].Points.Clear(); chart.Series[1].Points.Clear();
            tBuf.Clear(); TBuf.Clear(); sampleCount = 0;
        }

        void StopLogging()
        {
            logging = false; csv?.Dispose(); csv = null;
            btnStart.Enabled = true; btnStop.Enabled = false;
        }

        void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (sp.BytesToRead > 0)
            {
                int b = sp.ReadByte(); if (b < 0) return;
                switch (state)
                {
                    case 0: if (b == 0xFF) state = 1; break;
                    case 1: ms5 = (byte)b; state = 2; break;
                    case 2:
                        ls5 = (byte)b; state = 0;

                        int code10 = ((ms5 & 0x1F) << 5) | (ls5 & 0x1F);
                        double vout = code10 * VREF / 1023.0;
                        double rntc = R_FIXED * (vout / (VREF - vout));
                        double Tk = 1.0 / ((1.0 / T0K) + (1.0 / BETA) * Math.Log(rntc / R0));
                        double Tc = Tk - 273.15;
                        double TcAdj = a0 + a1 * Tc;
                        double errC = TcAdj - Tc;

                        if (++sampleCount % decim != 0) return;

                        var now = DateTime.Now;
                        if (logging)
                            csv?.WriteLine(string.Format(ci, "{0},{1},{2:F4},{3:F1},{4:F2},{5:F2}",
                                now.ToString("HH:mm:ss.fff"), code10, vout, rntc, Tc, TcAdj));

                        BeginInvoke(new Action(() =>
                        {
                            txtRawC.Text = Tc.ToString("F2", ci);
                            txtAdjC.Text = TcAdj.ToString("F2", ci);
                            txtErrC.Text = errC.ToString("F2", ci);
                            chart.Series[0].Points.AddXY(now, Tc);
                            chart.Series[1].Points.AddXY(now, TcAdj);
                            foreach (var s in chart.Series) while (s.Points.Count > 2000) s.Points.RemoveAt(0);
                            chart.ChartAreas[0].RecalculateAxesScale();
                            tBuf.Add(now); TBuf.Add(TcAdj);
                            while (tBuf.Count > 5000) { tBuf.RemoveAt(0); TBuf.RemoveAt(0); }
                        }));
                        break;
                }
            }
        }

        void EstimateTauFromBuffer()
        {
            if (TBuf.Count < 50) { MessageBox.Show("Not enough data."); return; }
            int idx = -1;
            for (int i = TBuf.Count - 2; i >= 5; --i)
                if (Math.Abs(TBuf[i + 1] - TBuf[i - 5]) > 2.0) { idx = i - 5; break; }
            if (idx < 0) { MessageBox.Show("No recent step found."); return; }

            double Tinf = TBuf.Last(); DateTime t0 = tBuf[idx];
            var xs = new List<double>(); var ys = new List<double>();
            for (int i = idx; i < TBuf.Count; ++i)
            {
                double dt = (tBuf[i] - t0).TotalSeconds; if (dt <= 0 || dt > 30) continue;
                double y = Math.Abs(Tinf - TBuf[i]); if (y <= 0) continue;
                xs.Add(dt); ys.Add(Math.Log(y));
            }
            if (xs.Count < 10) { MessageBox.Show("Not enough post-step data."); return; }

            double sx = xs.Sum(), sy = ys.Sum();
            double sxx = xs.Select(v => v * v).Sum();
            double sxy = xs.Zip(ys, (a, b) => a * b).Sum();
            int n = xs.Count;
            double slope = (n * sxy - sx * sy) / (n * sxx - sx * sx);
            double tau = -1.0 / slope;

            MessageBox.Show($"Estimated τ ≈ {tau:F2} s using {n} points.", "Time Constant");
        }
    }
}

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace LoadCellScale
{
    public partial class MainForm
    {
        private void InitializeComponent()
        {
            SuspendLayout();

            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Text = "Load Cell Scale";
            MinimumSize = new Size(1200, 720);

            var layoutRoot = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10)
            };
            layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 380F));
            layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var leftPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 260F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var connectionGroup = BuildConnectionGroup();
            var liveGroup = BuildLiveGroup();
            var calibrationGroup = BuildCalibrationGroup();

            leftPanel.Controls.Add(connectionGroup, 0, 0);
            leftPanel.Controls.Add(liveGroup, 0, 1);
            leftPanel.Controls.Add(calibrationGroup, 0, 2);

            chartLive = BuildChart();

            layoutRoot.Controls.Add(leftPanel, 0, 0);
            layoutRoot.Controls.Add(chartLive, 1, 0);

            statusStrip = new StatusStrip();
            statusLabelConnection = new ToolStripStatusLabel("Port closed");
            statusLabelCalibration = new ToolStripStatusLabel("Calibration: none");
            statusLabelSamples = new ToolStripStatusLabel("Samples: 0 Hz | Window: 0");
            statusStrip.Items.Add(statusLabelConnection);
            statusStrip.Items.Add(new ToolStripStatusLabel("|"));
            statusStrip.Items.Add(statusLabelCalibration);
            statusStrip.Items.Add(new ToolStripStatusLabel("|"));
            statusStrip.Items.Add(statusLabelSamples);

            Controls.Add(layoutRoot);
            Controls.Add(statusStrip);

            ResumeLayout(false);
            PerformLayout();
        }

        private GroupBox BuildConnectionGroup()
        {
            var group = new GroupBox
            {
                Text = "Connection",
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var lblPort = CreateFieldLabel("Serial port");

            cmbPorts = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnRefreshPorts = new Button
            {
                Dock = DockStyle.Fill,
                Text = "Refresh"
            };
            btnRefreshPorts.Click += (_, __) => PopulateSerialPorts();

            btnConnect = new Button
            {
                Dock = DockStyle.Fill,
                Text = "Connect",
                BackColor = Color.FromArgb(16, 124, 16),
                ForeColor = Color.White
            };
            btnConnect.Click += (_, __) => ToggleConnection();

            lblConnectionStatus = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Port closed",
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            layout.Controls.Add(lblPort, 0, 0);
            layout.Controls.Add(btnConnect, 1, 0);
            layout.Controls.Add(cmbPorts, 0, 1);
            layout.Controls.Add(btnRefreshPorts, 1, 1);
            layout.Controls.Add(lblConnectionStatus, 0, 2);
            layout.SetColumnSpan(lblConnectionStatus, 2);

            group.Controls.Add(layout);
            return group;
        }

        private GroupBox BuildLiveGroup()
        {
            var group = new GroupBox
            {
                Text = "Live Measurement",
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 9
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            for (var i = 0; i < 9; i++)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            }

            txtAdc = CreateReadOnlyTextBox();
            txtVoltage = CreateReadOnlyTextBox();
            txtWeight = CreateReadOnlyTextBox();
            txtSmoothed = CreateReadOnlyTextBox();
            txtSampleRate = CreateReadOnlyTextBox();

            nudSmoothingWindow = new NumericUpDown
            {
                Minimum = 5,
                Maximum = 1000,
                Value = DefaultSmoothingWindow,
                Dock = DockStyle.Fill
            };
            nudSmoothingWindow.ValueChanged += (_, __) => ChangeSmoothingWindow();

            nudStabilityThreshold = new NumericUpDown
            {
                DecimalPlaces = 3,
                Increment = 0.001M,
                Minimum = 1,
                Maximum = 1000,
                Value = (decimal)(DefaultStabilityThresholdKg * 1000),
                Dock = DockStyle.Fill
            };
            nudStabilityThreshold.ValueChanged += (_, __) => UpdateStabilityStatus();

            btnTare = new Button
            {
                Text = "Tare (Zero)",
                Dock = DockStyle.Fill
            };
            btnTare.Click += (_, __) => ApplyTare();

            lblStability = new Label
            {
                Text = "Stability: Unknown",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.LightGray,
                ForeColor = Color.Black
            };

            layout.Controls.Add(CreateFieldLabel("ADC (10-bit)"), 0, 0);
            layout.Controls.Add(txtAdc, 1, 0);
            layout.Controls.Add(CreateFieldLabel("Voltage (V)"), 0, 1);
            layout.Controls.Add(txtVoltage, 1, 1);
            layout.Controls.Add(CreateFieldLabel("Weight (kg)"), 0, 2);
            layout.Controls.Add(txtWeight, 1, 2);
            layout.Controls.Add(CreateFieldLabel("Smoothed (kg)"), 0, 3);
            layout.Controls.Add(txtSmoothed, 1, 3);
            layout.Controls.Add(CreateFieldLabel("Sample rate (Hz)"), 0, 4);
            layout.Controls.Add(txtSampleRate, 1, 4);
            layout.Controls.Add(CreateFieldLabel("Rolling window (samples)"), 0, 5);
            layout.Controls.Add(nudSmoothingWindow, 1, 5);
            layout.Controls.Add(CreateFieldLabel("Stability threshold (g)"), 0, 6);
            layout.Controls.Add(nudStabilityThreshold, 1, 6);
            layout.Controls.Add(CreateFieldLabel("Tare"), 0, 7);
            layout.Controls.Add(btnTare, 1, 7);
            layout.Controls.Add(lblStability, 0, 8);
            layout.SetColumnSpan(lblStability, 2);

            group.Controls.Add(layout);
            return group;
        }

        private GroupBox BuildCalibrationGroup()
        {
            var group = new GroupBox
            {
                Text = "Calibration",
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));

            txtReferenceMass = new TextBox
            {
                Dock = DockStyle.Fill
            };

            btnCaptureCalibration = new Button
            {
                Text = "Capture calibration point",
                Dock = DockStyle.Fill
            };
            btnCaptureCalibration.Click += (_, __) => CaptureCalibrationPoint();

            lvCalibrationPoints = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };
            lvCalibrationPoints.Columns.Add("Mass (kg)", 90);
            lvCalibrationPoints.Columns.Add("ADC avg", 80);
            lvCalibrationPoints.Columns.Add("Voltage (V)", 90);
            lvCalibrationPoints.Columns.Add("Captured", 100);

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            btnComputeCalibration = new Button
            {
                Text = "Compute fit",
                AutoSize = true
            };
            btnComputeCalibration.Click += (_, __) => ComputeCalibration();

            btnClearCalibration = new Button
            {
                Text = "Clear points",
                AutoSize = true
            };
            btnClearCalibration.Click += (_, __) => ClearCalibrationPoints();

            buttonPanel.Controls.Add(btnComputeCalibration);
            buttonPanel.Controls.Add(btnClearCalibration);

            lblCalibrationEquation = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.DimGray
            };

            layout.Controls.Add(CreateFieldLabel("Reference mass (kg)"), 0, 0);
            layout.Controls.Add(txtReferenceMass, 1, 0);
            layout.Controls.Add(btnCaptureCalibration, 0, 1);
            layout.SetColumnSpan(btnCaptureCalibration, 2);
            layout.Controls.Add(lvCalibrationPoints, 0, 2);
            layout.SetColumnSpan(lvCalibrationPoints, 2);
            layout.Controls.Add(buttonPanel, 0, 3);
            layout.SetColumnSpan(buttonPanel, 2);
            layout.Controls.Add(lblCalibrationEquation, 0, 4);
            layout.SetColumnSpan(lblCalibrationEquation, 2);

            group.Controls.Add(layout);
            return group;
        }

        private Chart BuildChart()
        {
            var chart = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            var area = new ChartArea("Main");
            area.AxisX.Title = "Time (s)";
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisX.LabelStyle.Format = "0.0";
            area.AxisY.Title = "Weight (kg)";
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisY.LabelStyle.Format = "0.00";
            chart.ChartAreas.Add(area);

            var seriesRaw = new Series("Raw weight")
            {
                ChartType = SeriesChartType.FastLine,
                ChartArea = "Main",
                Color = Color.FromArgb(120, 70, 130, 180),
                BorderWidth = 1
            };
            var seriesSmooth = new Series("Smoothed weight")
            {
                ChartType = SeriesChartType.FastLine,
                ChartArea = "Main",
                Color = Color.MediumSeaGreen,
                BorderWidth = 3
            };

            chart.Series.Add(seriesRaw);
            chart.Series.Add(seriesSmooth);

            return chart;
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static TextBox CreateReadOnlyTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }
    }
}

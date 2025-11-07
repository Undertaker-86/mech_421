using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GyanThermistor
{
    partial class Form1
    {
        private IContainer components;

        private ComboBox comboPorts;
        private Button btnRefreshPorts;
        private Button btnConnect;
        private CheckBox chkLogging;
        private TextBox txtCsvPath;
        private Label lblStatus;
        private TextBox txtA0;
        private TextBox txtA1;
        private Button btnApplyCalibration;
        private TextBox txtRaw;
        private TextBox txtAdjusted;
        private TextBox txtError;
        private TextBox txtVoltage;
        private TextBox txtResistance;
        private ComboBox comboCaptureScenario;
        private Button btnStartCapture;
        private Button btnStopCapture;
        private Chart chartTemp;
        private ListView listCaptures;
        private TextBox txtAnalysis;
        private TextBox txtLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            components = new Container();

            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = BaseBackColor;
            ForeColor = Color.Gainsboro;
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1400, 900);
            MinimumSize = new Size(1100, 820);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Gyan Thermistor Studio";

            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                BackColor = BaseBackColor
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Controls.Add(mainLayout);

            TableLayoutPanel leftLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                ColumnCount = 1,
                Padding = new Padding(16, 20, 16, 20)
            };
            leftLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(leftLayout, 0, 0);

            TableLayoutPanel rightLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                Padding = new Padding(16, 20, 20, 20)
            };
            rightLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 22F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 13F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            mainLayout.Controls.Add(rightLayout, 1, 0);

            GroupBox connectionGroup = CreateGroupBox("Serial Link");
            connectionGroup.Margin = new Padding(0, 0, 0, 16);
            leftLayout.Controls.Add(connectionGroup, 0, 0);

            TableLayoutPanel connectionLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            connectionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            connectionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            connectionGroup.Controls.Add(connectionLayout);

            comboPorts = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = FieldBackColor,
                ForeColor = Color.Gainsboro,
                Margin = new Padding(0, 3, 0, 6)
            };
            btnRefreshPorts = CreateAccentButton("Refresh", AccentRefreshColor, new Padding(0, 6, 6, 6));
            btnConnect = CreateAccentButton("Connect", AccentBlueColor, new Padding(6, 6, 0, 6));
            chkLogging = new CheckBox
            {
                Text = "Log samples to CSV file",
                ForeColor = Color.Gainsboro,
                AutoSize = true,
                Margin = new Padding(3, 12, 3, 6)
            };
            txtCsvPath = CreateReadOnlyField(false);
            txtCsvPath.Text = "(logging disabled)";
            lblStatus = new Label
            {
                Text = "Idle",
                ForeColor = StatusInfoColor,
                AutoSize = true,
                Margin = new Padding(3, 6, 3, 0)
            };

            connectionLayout.Controls.Add(CreateFieldLabel("COM Port"), 0, 0);
            connectionLayout.Controls.Add(comboPorts, 1, 0);
            connectionLayout.Controls.Add(btnRefreshPorts, 0, 1);
            connectionLayout.Controls.Add(btnConnect, 1, 1);
            connectionLayout.Controls.Add(chkLogging, 0, 2);
            connectionLayout.SetColumnSpan(chkLogging, 2);
            Label logFileLabel = CreateFieldLabel("Log file", new Padding(3, 12, 3, 6));
            connectionLayout.Controls.Add(logFileLabel, 0, 3);
            connectionLayout.SetColumnSpan(logFileLabel, 2);
            connectionLayout.Controls.Add(txtCsvPath, 0, 4);
            connectionLayout.SetColumnSpan(txtCsvPath, 2);
            connectionLayout.Controls.Add(lblStatus, 0, 5);
            connectionLayout.SetColumnSpan(lblStatus, 2);

            GroupBox calibrationGroup = CreateGroupBox("Calibration");
            calibrationGroup.Margin = new Padding(0, 0, 0, 16);
            leftLayout.Controls.Add(calibrationGroup, 0, 1);

            TableLayoutPanel calibrationLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            calibrationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            calibrationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            calibrationGroup.Controls.Add(calibrationLayout);

            txtA0 = CreateEditableField("0");
            txtA1 = CreateEditableField("1");
            btnApplyCalibration = CreateAccentButton("Apply Calibration", AccentGreenColor, new Padding(0, 12, 0, 0));

            calibrationLayout.Controls.Add(CreateFieldLabel("Offset (a0)"), 0, 0);
            calibrationLayout.Controls.Add(txtA0, 1, 0);
            calibrationLayout.Controls.Add(CreateFieldLabel("Slope (a1)"), 0, 1);
            calibrationLayout.Controls.Add(txtA1, 1, 1);
            calibrationLayout.Controls.Add(btnApplyCalibration, 0, 2);
            calibrationLayout.SetColumnSpan(btnApplyCalibration, 2);

            GroupBox readingsGroup = CreateGroupBox("Live Readings");
            readingsGroup.Margin = new Padding(0, 0, 0, 16);
            leftLayout.Controls.Add(readingsGroup, 0, 2);

            TableLayoutPanel readingsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            readingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            readingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            readingsGroup.Controls.Add(readingsLayout);

            txtRaw = CreateReadOnlyField();
            txtAdjusted = CreateReadOnlyField();
            txtError = CreateReadOnlyField();
            txtVoltage = CreateReadOnlyField();
            txtResistance = CreateReadOnlyField();

            readingsLayout.RowStyles.Add(new RowStyle());
            readingsLayout.Controls.Add(CreateFieldLabel("Raw (degC)"), 0, 0);
            readingsLayout.Controls.Add(txtRaw, 1, 0);

            readingsLayout.RowStyles.Add(new RowStyle());
            readingsLayout.Controls.Add(CreateFieldLabel("Adjusted (degC)"), 0, 1);
            readingsLayout.Controls.Add(txtAdjusted, 1, 1);

            readingsLayout.RowStyles.Add(new RowStyle());
            readingsLayout.Controls.Add(CreateFieldLabel("Error (degC)"), 0, 2);
            readingsLayout.Controls.Add(txtError, 1, 2);

            readingsLayout.RowStyles.Add(new RowStyle());
            readingsLayout.Controls.Add(CreateFieldLabel("Voltage (V)"), 0, 3);
            readingsLayout.Controls.Add(txtVoltage, 1, 3);

            readingsLayout.RowStyles.Add(new RowStyle());
            readingsLayout.Controls.Add(CreateFieldLabel("Resistance (ohm)"), 0, 4);
            readingsLayout.Controls.Add(txtResistance, 1, 4);

            GroupBox captureGroup = CreateGroupBox("Waveform Capture");
            captureGroup.Margin = new Padding(0);
            leftLayout.Controls.Add(captureGroup, 0, 3);

            TableLayoutPanel captureLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            captureLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            captureLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            captureGroup.Controls.Add(captureLayout);

            comboCaptureScenario = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = FieldBackColor,
                ForeColor = Color.Gainsboro,
                Margin = new Padding(0, 3, 0, 12)
            };
            comboCaptureScenario.Items.AddRange(new object[]
            {
                "0 -> 40 degC (heating)",
                "40 -> 0 degC (cooling)",
                "0 -> 60 degC (heating)",
                "60 -> 0 degC (cooling)",
                "Custom"
            });

            btnStartCapture = CreateAccentButton("Start Capture", AccentOrangeColor, new Padding(0, 6, 0, 6));
            btnStopCapture = CreateAccentButton("Stop Capture", AccentRedColor, new Padding(0, 6, 0, 0));
            btnStopCapture.Enabled = false;

            captureLayout.Controls.Add(CreateFieldLabel("Scenario"), 0, 0);
            captureLayout.Controls.Add(comboCaptureScenario, 1, 0);
            captureLayout.Controls.Add(btnStartCapture, 0, 1);
            captureLayout.SetColumnSpan(btnStartCapture, 2);
            captureLayout.Controls.Add(btnStopCapture, 0, 2);
            captureLayout.SetColumnSpan(btnStopCapture, 2);

            chartTemp = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = PanelBackColor,
                Palette = ChartColorPalette.None
            };
            chartTemp.Margin = new Padding(0, 0, 0, 16);
            ChartArea area = new ChartArea("ChartArea1")
            {
                BackColor = PanelBackColor
            };
            area.AxisX.LabelStyle.Format = "HH:mm:ss";
            area.AxisX.LabelStyle.ForeColor = Color.Gainsboro;
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(60, 80, 99);
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisX.LineColor = Color.SlateGray;
            area.AxisX.Title = "Time";
            area.AxisX.TitleForeColor = Color.Gainsboro;
            area.AxisY.LabelStyle.ForeColor = Color.Gainsboro;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(60, 80, 99);
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisY.LineColor = Color.SlateGray;
            area.AxisY.Title = "Temperature (degC)";
            area.AxisY.TitleForeColor = Color.Gainsboro;
            chartTemp.ChartAreas.Add(area);

            Legend legend = new Legend("Legend1")
            {
                Docking = Docking.Bottom,
                ForeColor = Color.Gainsboro
            };
            chartTemp.Legends.Add(legend);

            Series rawSeries = new Series("Raw degC")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                ChartArea = "ChartArea1",
                Legend = "Legend1",
                Color = Color.FromArgb(0, 191, 255),
                XValueType = ChartValueType.DateTime
            };
            Series compSeries = new Series("Compensated degC")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                ChartArea = "ChartArea1",
                Legend = "Legend1",
                Color = Color.FromArgb(255, 99, 132),
                XValueType = ChartValueType.DateTime
            };
            chartTemp.Series.Add(rawSeries);
            chartTemp.Series.Add(compSeries);
            rightLayout.Controls.Add(chartTemp, 0, 0);

            GroupBox resultsGroup = CreateGroupBox("Capture History");
            resultsGroup.AutoSize = false;
            resultsGroup.Dock = DockStyle.Fill;
            resultsGroup.Margin = new Padding(0, 0, 0, 16);
            rightLayout.Controls.Add(resultsGroup, 0, 1);

            listCaptures = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                BackColor = FieldBackColor,
                ForeColor = Color.Gainsboro,
                BorderStyle = BorderStyle.None,
                FullRowSelect = true,
                GridLines = true,
                HideSelection = false
            };
            listCaptures.Columns.AddRange(new[]
            {
                new ColumnHeader { Text = "Scenario", Width = 190 },
                new ColumnHeader { Text = "Duration (s)", Width = 110 },
                new ColumnHeader { Text = "Start (degC)", Width = 110 },
                new ColumnHeader { Text = "Final (degC)", Width = 110 },
                new ColumnHeader { Text = "Tau 63% (s)", Width = 110 },
                new ColumnHeader { Text = "Tau Fit (s)", Width = 110 },
                new ColumnHeader { Text = "R2", Width = 70 },
                new ColumnHeader { Text = "CSV File", Width = 160 },
                new ColumnHeader { Text = "Notes", Width = 220 }
            });
            resultsGroup.Controls.Add(listCaptures);

            GroupBox analysisGroup = CreateGroupBox("Analysis");
            analysisGroup.AutoSize = false;
            analysisGroup.Dock = DockStyle.Fill;
            analysisGroup.Margin = new Padding(0, 0, 0, 16);
            rightLayout.Controls.Add(analysisGroup, 0, 2);

            txtAnalysis = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = FieldBackColor,
                ForeColor = Color.Gainsboro,
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            analysisGroup.Controls.Add(txtAnalysis);

            GroupBox logGroup = CreateGroupBox("Event Log");
            logGroup.AutoSize = false;
            logGroup.Dock = DockStyle.Fill;
            logGroup.Margin = new Padding(0);
            rightLayout.Controls.Add(logGroup, 0, 3);

            txtLog = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = FieldBackColor,
                ForeColor = Color.Gainsboro,
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            logGroup.Controls.Add(txtLog);

            ResumeLayout(true);
        }
    }
}

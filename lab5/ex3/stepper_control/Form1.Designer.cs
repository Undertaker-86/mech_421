using System.Drawing;
using System.Windows.Forms;

namespace StepperControl
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            
            // Main Layout
            this.sidePanel = new System.Windows.Forms.Panel();
            this.navPanel = new System.Windows.Forms.Panel();
            this.singleStepModeBtn = new System.Windows.Forms.Button();
            this.continuousModeBtn = new System.Windows.Forms.Button();
            this.connectionPanel = new System.Windows.Forms.Panel();
            this.connectButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.portComboBox = new System.Windows.Forms.ComboBox();
            this.connectionLabel = new System.Windows.Forms.Label();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.subHeaderLabel = new System.Windows.Forms.Label();
            this.headerLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            
            this.contentPanel = new System.Windows.Forms.Panel();
            
            // Continuous Mode Controls
            this.continuousPanel = new System.Windows.Forms.Panel();
            this.velocityTelemetryPanel = new System.Windows.Forms.Panel();
            this.velocityRpmTextBox = new System.Windows.Forms.TextBox();
            this.velocityHzTextBox = new System.Windows.Forms.TextBox();
            this.velocityRpmLabel = new System.Windows.Forms.Label();
            this.velocityHzLabel = new System.Windows.Forms.Label();
            this.packetLabel = new System.Windows.Forms.Label();
            this.lastPacketLabel = new System.Windows.Forms.Label();
            this.packetPreviewTextBox = new System.Windows.Forms.TextBox();
            
            this.velocityControlPanel = new System.Windows.Forms.Panel();
            this.velocityTrackBar = new System.Windows.Forms.TrackBar();
            this.maxSpeedNumeric = new System.Windows.Forms.NumericUpDown();
            this.maxSpeedLabel = new System.Windows.Forms.Label();
            this.velocityInstructionLabel = new System.Windows.Forms.Label();
            this.velocitySummaryLabel = new System.Windows.Forms.Label();
            this.modeLabel = new System.Windows.Forms.Label();
            this.freqLabel = new System.Windows.Forms.Label();
            this.velocityTitleLabel = new System.Windows.Forms.Label();

            // Single Step Mode Controls
            this.singleStepPanel = new System.Windows.Forms.Panel();
            this.positionTelemetryPanel = new System.Windows.Forms.Panel();
            this.positionValueTextBox = new System.Windows.Forms.TextBox();
            this.positionValueLabel = new System.Windows.Forms.Label();
            this.positionTitleLabel = new System.Windows.Forms.Label();
            
            this.stepControlPanel = new System.Windows.Forms.Panel();
            this.cwStepButton = new System.Windows.Forms.Button();
            this.ccwStepButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.stepTitleLabel = new System.Windows.Forms.Label();

            // Initialization
            this.sidePanel.SuspendLayout();
            this.navPanel.SuspendLayout();
            this.connectionPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.continuousPanel.SuspendLayout();
            this.velocityTelemetryPanel.SuspendLayout();
            this.velocityControlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.velocityTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxSpeedNumeric)).BeginInit();
            this.singleStepPanel.SuspendLayout();
            this.positionTelemetryPanel.SuspendLayout();
            this.stepControlPanel.SuspendLayout();
            this.SuspendLayout();

            // 
            // serialPort
            // 
            this.serialPort.PortName = "COM1";

            // 
            // sidePanel
            // 
            this.sidePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.sidePanel.Controls.Add(this.statusLabel);
            this.sidePanel.Controls.Add(this.navPanel);
            this.sidePanel.Controls.Add(this.connectionPanel);
            this.sidePanel.Controls.Add(this.headerPanel);
            this.sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePanel.Location = new System.Drawing.Point(0, 0);
            this.sidePanel.Name = "sidePanel";
            this.sidePanel.Size = new System.Drawing.Size(280, 600);
            this.sidePanel.TabIndex = 0;

            // 
            // headerPanel
            // 
            this.headerPanel.Controls.Add(this.subHeaderLabel);
            this.headerPanel.Controls.Add(this.headerLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(280, 100);
            this.headerPanel.TabIndex = 0;

            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.headerLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.headerLabel.Location = new System.Drawing.Point(12, 20);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(220, 37);
            this.headerLabel.TabIndex = 0;
            this.headerLabel.Text = "Stepper Control";

            // 
            // subHeaderLabel
            // 
            this.subHeaderLabel.AutoSize = true;
            this.subHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.subHeaderLabel.ForeColor = System.Drawing.Color.Gray;
            this.subHeaderLabel.Location = new System.Drawing.Point(16, 60);
            this.subHeaderLabel.Name = "subHeaderLabel";
            this.subHeaderLabel.Size = new System.Drawing.Size(158, 20);
            this.subHeaderLabel.TabIndex = 1;
            this.subHeaderLabel.Text = "MECH 421/423 Lab 5";

            // 
            // connectionPanel
            // 
            this.connectionPanel.Controls.Add(this.connectButton);
            this.connectionPanel.Controls.Add(this.refreshButton);
            this.connectionPanel.Controls.Add(this.portComboBox);
            this.connectionPanel.Controls.Add(this.connectionLabel);
            this.connectionPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionPanel.Location = new System.Drawing.Point(0, 100);
            this.connectionPanel.Name = "connectionPanel";
            this.connectionPanel.Size = new System.Drawing.Size(280, 160);
            this.connectionPanel.TabIndex = 1;
            this.connectionPanel.Padding = new System.Windows.Forms.Padding(15);

            // 
            // connectionLabel
            // 
            this.connectionLabel.AutoSize = true;
            this.connectionLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.connectionLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.connectionLabel.Location = new System.Drawing.Point(12, 3);
            this.connectionLabel.Name = "connectionLabel";
            this.connectionLabel.Size = new System.Drawing.Size(98, 23);
            this.connectionLabel.TabIndex = 0;
            this.connectionLabel.Text = "Connection";

            // 
            // portComboBox
            // 
            this.portComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(60)))));
            this.portComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.portComboBox.ForeColor = System.Drawing.Color.White;
            this.portComboBox.FormattingEnabled = true;
            this.portComboBox.Location = new System.Drawing.Point(16, 35);
            this.portComboBox.Name = "portComboBox";
            this.portComboBox.Size = new System.Drawing.Size(150, 28);
            this.portComboBox.TabIndex = 1;

            // 
            // refreshButton
            // 
            this.refreshButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(60)))));
            this.refreshButton.FlatAppearance.BorderSize = 0;
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshButton.ForeColor = System.Drawing.Color.White;
            this.refreshButton.Location = new System.Drawing.Point(176, 34);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(80, 29);
            this.refreshButton.TabIndex = 2;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = false;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);

            // 
            // connectButton
            // 
            this.connectButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.connectButton.FlatAppearance.BorderSize = 0;
            this.connectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connectButton.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.connectButton.ForeColor = System.Drawing.Color.Black;
            this.connectButton.Location = new System.Drawing.Point(16, 80);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(240, 40);
            this.connectButton.TabIndex = 3;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = false;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);

            // 
            // navPanel
            // 
            this.navPanel.Controls.Add(this.singleStepModeBtn);
            this.navPanel.Controls.Add(this.continuousModeBtn);
            this.navPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.navPanel.Location = new System.Drawing.Point(0, 260);
            this.navPanel.Name = "navPanel";
            this.navPanel.Size = new System.Drawing.Size(280, 140);
            this.navPanel.TabIndex = 2;
            this.navPanel.Padding = new System.Windows.Forms.Padding(0, 20, 0, 0);

            // 
            // continuousModeBtn
            // 
            this.continuousModeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(40)))), ((int)(((byte)(55)))));
            this.continuousModeBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.continuousModeBtn.FlatAppearance.BorderSize = 0;
            this.continuousModeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.continuousModeBtn.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.continuousModeBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.continuousModeBtn.Location = new System.Drawing.Point(0, 20);
            this.continuousModeBtn.Name = "continuousModeBtn";
            this.continuousModeBtn.Size = new System.Drawing.Size(280, 50);
            this.continuousModeBtn.TabIndex = 0;
            this.continuousModeBtn.Text = "Continuous Control";
            this.continuousModeBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.continuousModeBtn.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.continuousModeBtn.UseVisualStyleBackColor = false;
            this.continuousModeBtn.Click += new System.EventHandler(this.continuousModeBtn_Click);

            // 
            // singleStepModeBtn
            // 
            this.singleStepModeBtn.BackColor = System.Drawing.Color.Transparent;
            this.singleStepModeBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.singleStepModeBtn.FlatAppearance.BorderSize = 0;
            this.singleStepModeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.singleStepModeBtn.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.singleStepModeBtn.ForeColor = System.Drawing.Color.Gray;
            this.singleStepModeBtn.Location = new System.Drawing.Point(0, 70);
            this.singleStepModeBtn.Name = "singleStepModeBtn";
            this.singleStepModeBtn.Size = new System.Drawing.Size(280, 50);
            this.singleStepModeBtn.TabIndex = 1;
            this.singleStepModeBtn.Text = "Single Step Control";
            this.singleStepModeBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.singleStepModeBtn.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.singleStepModeBtn.UseVisualStyleBackColor = false;
            this.singleStepModeBtn.Click += new System.EventHandler(this.singleStepModeBtn_Click);

            // 
            // statusLabel
            // 
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusLabel.ForeColor = System.Drawing.Color.Gray;
            this.statusLabel.Location = new System.Drawing.Point(0, 560);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Padding = new System.Windows.Forms.Padding(10);
            this.statusLabel.Size = new System.Drawing.Size(280, 40);
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "Status: Ready";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.contentPanel.Controls.Add(this.continuousPanel);
            this.contentPanel.Controls.Add(this.singleStepPanel);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(280, 0);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(586, 600);
            this.contentPanel.TabIndex = 1;

            // 
            // continuousPanel
            // 
            this.continuousPanel.Controls.Add(this.velocityTelemetryPanel);
            this.continuousPanel.Controls.Add(this.velocityControlPanel);
            this.continuousPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.continuousPanel.Location = new System.Drawing.Point(0, 0);
            this.continuousPanel.Name = "continuousPanel";
            this.continuousPanel.Size = new System.Drawing.Size(586, 600);
            this.continuousPanel.TabIndex = 0;

            // 
            // velocityControlPanel
            // 
            this.velocityControlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(25)))), ((int)(((byte)(35)))));
            this.velocityControlPanel.Controls.Add(this.velocityTitleLabel);
            this.velocityControlPanel.Controls.Add(this.velocityInstructionLabel);
            this.velocityControlPanel.Controls.Add(this.velocityTrackBar);
            this.velocityControlPanel.Controls.Add(this.maxSpeedLabel);
            this.velocityControlPanel.Controls.Add(this.maxSpeedNumeric);
            this.velocityControlPanel.Controls.Add(this.velocitySummaryLabel);
            this.velocityControlPanel.Controls.Add(this.modeLabel);
            this.velocityControlPanel.Controls.Add(this.freqLabel);
            this.velocityControlPanel.Location = new System.Drawing.Point(30, 30);
            this.velocityControlPanel.Name = "velocityControlPanel";
            this.velocityControlPanel.Size = new System.Drawing.Size(520, 300);
            this.velocityControlPanel.TabIndex = 0;

            // 
            // velocityTitleLabel
            // 
            this.velocityTitleLabel.AutoSize = true;
            this.velocityTitleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.velocityTitleLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.velocityTitleLabel.Location = new System.Drawing.Point(20, 20);
            this.velocityTitleLabel.Name = "velocityTitleLabel";
            this.velocityTitleLabel.Size = new System.Drawing.Size(183, 32);
            this.velocityTitleLabel.TabIndex = 0;
            this.velocityTitleLabel.Text = "Velocity Control";

            // 
            // velocityInstructionLabel
            // 
            this.velocityInstructionLabel.AutoSize = true;
            this.velocityInstructionLabel.ForeColor = System.Drawing.Color.Gray;
            this.velocityInstructionLabel.Location = new System.Drawing.Point(25, 60);
            this.velocityInstructionLabel.Name = "velocityInstructionLabel";
            this.velocityInstructionLabel.Size = new System.Drawing.Size(354, 20);
            this.velocityInstructionLabel.TabIndex = 1;
            this.velocityInstructionLabel.Text = "Drag slider to control speed. Center is stop.";

            // 
            // velocityTrackBar
            // 
            this.velocityTrackBar.Location = new System.Drawing.Point(25, 90);
            this.velocityTrackBar.Name = "velocityTrackBar";
            this.velocityTrackBar.Size = new System.Drawing.Size(470, 56);
            this.velocityTrackBar.TabIndex = 2;
            this.velocityTrackBar.Scroll += new System.EventHandler(this.velocityTrackBar_Scroll);

            // 
            // maxSpeedLabel
            // 
            this.maxSpeedLabel.AutoSize = true;
            this.maxSpeedLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.maxSpeedLabel.Location = new System.Drawing.Point(25, 160);
            this.maxSpeedLabel.Name = "maxSpeedLabel";
            this.maxSpeedLabel.Size = new System.Drawing.Size(130, 20);
            this.maxSpeedLabel.TabIndex = 3;
            this.maxSpeedLabel.Text = "Max Speed (steps/s)";

            // 
            // maxSpeedNumeric
            // 
            this.maxSpeedNumeric.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(60)))));
            this.maxSpeedNumeric.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maxSpeedNumeric.ForeColor = System.Drawing.Color.White;
            this.maxSpeedNumeric.Location = new System.Drawing.Point(170, 158);
            this.maxSpeedNumeric.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            this.maxSpeedNumeric.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            this.maxSpeedNumeric.Name = "maxSpeedNumeric";
            this.maxSpeedNumeric.Size = new System.Drawing.Size(100, 27);
            this.maxSpeedNumeric.TabIndex = 4;
            this.maxSpeedNumeric.Value = new decimal(new int[] { 1200, 0, 0, 0 });
            this.maxSpeedNumeric.ValueChanged += new System.EventHandler(this.maxSpeedNumeric_ValueChanged);

            // 
            // velocitySummaryLabel
            // 
            this.velocitySummaryLabel.AutoSize = true;
            this.velocitySummaryLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.velocitySummaryLabel.Location = new System.Drawing.Point(25, 200);
            this.velocitySummaryLabel.Name = "velocitySummaryLabel";
            this.velocitySummaryLabel.Size = new System.Drawing.Size(155, 20);
            this.velocitySummaryLabel.TabIndex = 5;
            this.velocitySummaryLabel.Text = "Status: Stopped";

            // 
            // modeLabel
            // 
            this.modeLabel.AutoSize = true;
            this.modeLabel.ForeColor = System.Drawing.Color.Gray;
            this.modeLabel.Location = new System.Drawing.Point(25, 230);
            this.modeLabel.Name = "modeLabel";
            this.modeLabel.Size = new System.Drawing.Size(100, 20);
            this.modeLabel.TabIndex = 6;
            this.modeLabel.Text = "Mode: Idle";

            // 
            // freqLabel
            // 
            this.freqLabel.AutoSize = true;
            this.freqLabel.ForeColor = System.Drawing.Color.Gray;
            this.freqLabel.Location = new System.Drawing.Point(25, 255);
            this.freqLabel.Name = "freqLabel";
            this.freqLabel.Size = new System.Drawing.Size(80, 20);
            this.freqLabel.TabIndex = 7;
            this.freqLabel.Text = "Freq: 0 Hz";

            // 
            // velocityTelemetryPanel
            // 
            this.velocityTelemetryPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(25)))), ((int)(((byte)(35)))));
            this.velocityTelemetryPanel.Controls.Add(this.packetLabel);
            this.velocityTelemetryPanel.Controls.Add(this.packetPreviewTextBox);
            this.velocityTelemetryPanel.Controls.Add(this.lastPacketLabel);
            this.velocityTelemetryPanel.Controls.Add(this.velocityHzLabel);
            this.velocityTelemetryPanel.Controls.Add(this.velocityHzTextBox);
            this.velocityTelemetryPanel.Controls.Add(this.velocityRpmLabel);
            this.velocityTelemetryPanel.Controls.Add(this.velocityRpmTextBox);
            this.velocityTelemetryPanel.Location = new System.Drawing.Point(30, 350);
            this.velocityTelemetryPanel.Name = "velocityTelemetryPanel";
            this.velocityTelemetryPanel.Size = new System.Drawing.Size(520, 200);
            this.velocityTelemetryPanel.TabIndex = 1;

            // 
            // packetLabel
            // 
            this.packetLabel.AutoSize = true;
            this.packetLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.packetLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.packetLabel.Location = new System.Drawing.Point(20, 20);
            this.packetLabel.Name = "packetLabel";
            this.packetLabel.Size = new System.Drawing.Size(106, 28);
            this.packetLabel.TabIndex = 0;
            this.packetLabel.Text = "Telemetry";

            // 
            // velocityHzLabel
            // 
            this.velocityHzLabel.AutoSize = true;
            this.velocityHzLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.velocityHzLabel.Location = new System.Drawing.Point(25, 70);
            this.velocityHzLabel.Name = "velocityHzLabel";
            this.velocityHzLabel.Size = new System.Drawing.Size(26, 20);
            this.velocityHzLabel.TabIndex = 1;
            this.velocityHzLabel.Text = "Hz";

            // 
            // velocityHzTextBox
            // 
            this.velocityHzTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(60)))));
            this.velocityHzTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.velocityHzTextBox.ForeColor = System.Drawing.Color.White;
            this.velocityHzTextBox.Location = new System.Drawing.Point(60, 68);
            this.velocityHzTextBox.Name = "velocityHzTextBox";
            this.velocityHzTextBox.ReadOnly = true;
            this.velocityHzTextBox.Size = new System.Drawing.Size(100, 27);
            this.velocityHzTextBox.TabIndex = 2;

            // 
            // velocityRpmLabel
            // 
            this.velocityRpmLabel.AutoSize = true;
            this.velocityRpmLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.velocityRpmLabel.Location = new System.Drawing.Point(180, 70);
            this.velocityRpmLabel.Name = "velocityRpmLabel";
            this.velocityRpmLabel.Size = new System.Drawing.Size(38, 20);
            this.velocityRpmLabel.TabIndex = 3;
            this.velocityRpmLabel.Text = "RPM";

            // 
            // velocityRpmTextBox
            // 
            this.velocityRpmTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(60)))));
            this.velocityRpmTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.velocityRpmTextBox.ForeColor = System.Drawing.Color.White;
            this.velocityRpmTextBox.Location = new System.Drawing.Point(225, 68);
            this.velocityRpmTextBox.Name = "velocityRpmTextBox";
            this.velocityRpmTextBox.ReadOnly = true;
            this.velocityRpmTextBox.Size = new System.Drawing.Size(100, 27);
            this.velocityRpmTextBox.TabIndex = 4;

            // 
            // lastPacketLabel
            // 
            this.lastPacketLabel.AutoSize = true;
            this.lastPacketLabel.ForeColor = System.Drawing.Color.Gray;
            this.lastPacketLabel.Location = new System.Drawing.Point(25, 120);
            this.lastPacketLabel.Name = "lastPacketLabel";
            this.lastPacketLabel.Size = new System.Drawing.Size(83, 20);
            this.lastPacketLabel.TabIndex = 5;
            this.lastPacketLabel.Text = "Last Packet";

            // 
            // packetPreviewTextBox
            // 
            this.packetPreviewTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(60)))));
            this.packetPreviewTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.packetPreviewTextBox.Font = new System.Drawing.Font("Consolas", 10F);
            this.packetPreviewTextBox.ForeColor = System.Drawing.Color.White;
            this.packetPreviewTextBox.Location = new System.Drawing.Point(25, 145);
            this.packetPreviewTextBox.Name = "packetPreviewTextBox";
            this.packetPreviewTextBox.ReadOnly = true;
            this.packetPreviewTextBox.Size = new System.Drawing.Size(470, 27);
            this.packetPreviewTextBox.TabIndex = 6;

            // 
            // singleStepPanel
            // 
            this.singleStepPanel.Controls.Add(this.positionTelemetryPanel);
            this.singleStepPanel.Controls.Add(this.stepControlPanel);
            this.singleStepPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.singleStepPanel.Location = new System.Drawing.Point(0, 0);
            this.singleStepPanel.Name = "singleStepPanel";
            this.singleStepPanel.Size = new System.Drawing.Size(586, 600);
            this.singleStepPanel.TabIndex = 1;
            this.singleStepPanel.Visible = false;

            // 
            // stepControlPanel
            // 
            this.stepControlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(25)))), ((int)(((byte)(35)))));
            this.stepControlPanel.Controls.Add(this.stepTitleLabel);
            this.stepControlPanel.Controls.Add(this.cwStepButton);
            this.stepControlPanel.Controls.Add(this.ccwStepButton);
            this.stepControlPanel.Controls.Add(this.stopButton);
            this.stepControlPanel.Location = new System.Drawing.Point(30, 30);
            this.stepControlPanel.Name = "stepControlPanel";
            this.stepControlPanel.Size = new System.Drawing.Size(520, 300);
            this.stepControlPanel.TabIndex = 0;

            // 
            // stepTitleLabel
            // 
            this.stepTitleLabel.AutoSize = true;
            this.stepTitleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.stepTitleLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.stepTitleLabel.Location = new System.Drawing.Point(20, 20);
            this.stepTitleLabel.Name = "stepTitleLabel";
            this.stepTitleLabel.Size = new System.Drawing.Size(144, 32);
            this.stepTitleLabel.TabIndex = 0;
            this.stepTitleLabel.Text = "Step Control";

            // 
            // cwStepButton
            // 
            this.cwStepButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.cwStepButton.FlatAppearance.BorderSize = 0;
            this.cwStepButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cwStepButton.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.cwStepButton.ForeColor = System.Drawing.Color.Black;
            this.cwStepButton.Location = new System.Drawing.Point(270, 80);
            this.cwStepButton.Name = "cwStepButton";
            this.cwStepButton.Size = new System.Drawing.Size(220, 80);
            this.cwStepButton.TabIndex = 1;
            this.cwStepButton.Text = "Step CW";
            this.cwStepButton.UseVisualStyleBackColor = false;
            this.cwStepButton.Click += new System.EventHandler(this.cwStepButton_Click);

            // 
            // ccwStepButton
            // 
            this.ccwStepButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(64)))), ((int)(((byte)(175)))));
            this.ccwStepButton.FlatAppearance.BorderSize = 0;
            this.ccwStepButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ccwStepButton.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.ccwStepButton.ForeColor = System.Drawing.Color.White;
            this.ccwStepButton.Location = new System.Drawing.Point(30, 80);
            this.ccwStepButton.Name = "ccwStepButton";
            this.ccwStepButton.Size = new System.Drawing.Size(220, 80);
            this.ccwStepButton.TabIndex = 2;
            this.ccwStepButton.Text = "Step CCW";
            this.ccwStepButton.UseVisualStyleBackColor = false;
            this.ccwStepButton.Click += new System.EventHandler(this.ccwStepButton_Click);

            // 
            // stopButton
            // 
            this.stopButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.stopButton.FlatAppearance.BorderSize = 0;
            this.stopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stopButton.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.stopButton.ForeColor = System.Drawing.Color.White;
            this.stopButton.Location = new System.Drawing.Point(30, 200);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(460, 60);
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "STOP MOTOR";
            this.stopButton.UseVisualStyleBackColor = false;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);

            // 
            // positionTelemetryPanel
            // 
            this.positionTelemetryPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(25)))), ((int)(((byte)(35)))));
            this.positionTelemetryPanel.Controls.Add(this.positionTitleLabel);
            this.positionTelemetryPanel.Controls.Add(this.positionValueLabel);
            this.positionTelemetryPanel.Controls.Add(this.positionValueTextBox);
            this.positionTelemetryPanel.Location = new System.Drawing.Point(30, 350);
            this.positionTelemetryPanel.Name = "positionTelemetryPanel";
            this.positionTelemetryPanel.Size = new System.Drawing.Size(520, 150);
            this.positionTelemetryPanel.TabIndex = 1;

            // 
            // positionTitleLabel
            // 
            this.positionTitleLabel.AutoSize = true;
            this.positionTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.positionTitleLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.positionTitleLabel.Location = new System.Drawing.Point(20, 20);
            this.positionTitleLabel.Name = "positionTitleLabel";
            this.positionTitleLabel.Size = new System.Drawing.Size(182, 28);
            this.positionTitleLabel.TabIndex = 0;
            this.positionTitleLabel.Text = "Position Telemetry";

            // 
            // positionValueLabel
            // 
            this.positionValueLabel.AutoSize = true;
            this.positionValueLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.positionValueLabel.Location = new System.Drawing.Point(25, 70);
            this.positionValueLabel.Name = "positionValueLabel";
            this.positionValueLabel.Size = new System.Drawing.Size(107, 20);
            this.positionValueLabel.TabIndex = 1;
            this.positionValueLabel.Text = "Current Angle:";

            // 
            // positionValueTextBox
            // 
            this.positionValueTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(60)))));
            this.positionValueTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.positionValueTextBox.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.positionValueTextBox.ForeColor = System.Drawing.Color.White;
            this.positionValueTextBox.Location = new System.Drawing.Point(150, 60);
            this.positionValueTextBox.Name = "positionValueTextBox";
            this.positionValueTextBox.ReadOnly = true;
            this.positionValueTextBox.Size = new System.Drawing.Size(200, 39);
            this.positionValueTextBox.TabIndex = 2;
            this.positionValueTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(12)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(866, 600);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.sidePanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stepper Motor Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            
            this.sidePanel.ResumeLayout(false);
            this.navPanel.ResumeLayout(false);
            this.connectionPanel.ResumeLayout(false);
            this.connectionPanel.PerformLayout();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.contentPanel.ResumeLayout(false);
            this.continuousPanel.ResumeLayout(false);
            this.velocityTelemetryPanel.ResumeLayout(false);
            this.velocityTelemetryPanel.PerformLayout();
            this.velocityControlPanel.ResumeLayout(false);
            this.velocityControlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.velocityTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxSpeedNumeric)).EndInit();
            this.singleStepPanel.ResumeLayout(false);
            this.positionTelemetryPanel.ResumeLayout(false);
            this.positionTelemetryPanel.PerformLayout();
            this.stepControlPanel.ResumeLayout(false);
            this.stepControlPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort;
        
        // Layout Panels
        private System.Windows.Forms.Panel sidePanel;
        private System.Windows.Forms.Panel navPanel;
        private System.Windows.Forms.Panel connectionPanel;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Panel contentPanel;
        
        // Navigation Controls
        private System.Windows.Forms.Button continuousModeBtn;
        private System.Windows.Forms.Button singleStepModeBtn;
        private System.Windows.Forms.Label statusLabel;
        
        // Header Controls
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Label subHeaderLabel;
        
        // Connection Controls
        private System.Windows.Forms.Label connectionLabel;
        private System.Windows.Forms.ComboBox portComboBox;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button connectButton;
        
        // Continuous Mode
        private System.Windows.Forms.Panel continuousPanel;
        private System.Windows.Forms.Panel velocityControlPanel;
        private System.Windows.Forms.Panel velocityTelemetryPanel;
        private System.Windows.Forms.Label velocityTitleLabel;
        private System.Windows.Forms.Label velocityInstructionLabel;
        private System.Windows.Forms.TrackBar velocityTrackBar;
        private System.Windows.Forms.Label maxSpeedLabel;
        private System.Windows.Forms.NumericUpDown maxSpeedNumeric;
        private System.Windows.Forms.Label velocitySummaryLabel;
        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.Label freqLabel;
        private System.Windows.Forms.Label packetLabel;
        private System.Windows.Forms.TextBox packetPreviewTextBox;
        private System.Windows.Forms.Label lastPacketLabel;
        private System.Windows.Forms.Label velocityHzLabel;
        private System.Windows.Forms.TextBox velocityHzTextBox;
        private System.Windows.Forms.Label velocityRpmLabel;
        private System.Windows.Forms.TextBox velocityRpmTextBox;
        
        // Single Step Mode
        private System.Windows.Forms.Panel singleStepPanel;
        private System.Windows.Forms.Panel stepControlPanel;
        private System.Windows.Forms.Panel positionTelemetryPanel;
        private System.Windows.Forms.Label stepTitleLabel;
        private System.Windows.Forms.Button cwStepButton;
        private System.Windows.Forms.Button ccwStepButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label positionTitleLabel;
        private System.Windows.Forms.Label positionValueLabel;
        private System.Windows.Forms.TextBox positionValueTextBox;
    }
}

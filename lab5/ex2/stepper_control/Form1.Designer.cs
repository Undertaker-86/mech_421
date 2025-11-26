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
            this.portComboBox = new System.Windows.Forms.ComboBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.ccwStepButton = new System.Windows.Forms.Button();
            this.cwStepButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.velocityTrackBar = new System.Windows.Forms.TrackBar();
            this.maxSpeedNumeric = new System.Windows.Forms.NumericUpDown();
            this.velocitySummaryLabel = new System.Windows.Forms.Label();
            this.packetPreviewTextBox = new System.Windows.Forms.TextBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.headerLabel = new System.Windows.Forms.Label();
            this.subHeaderLabel = new System.Windows.Forms.Label();
            this.connectionPanel = new System.Windows.Forms.Panel();
            this.connectionLabel = new System.Windows.Forms.Label();
            this.stepPanel = new System.Windows.Forms.Panel();
            this.stepLabel = new System.Windows.Forms.Label();
            this.velocityPanel = new System.Windows.Forms.Panel();
            this.modeLabel = new System.Windows.Forms.Label();
            this.freqLabel = new System.Windows.Forms.Label();
            this.velocityInstructionLabel = new System.Windows.Forms.Label();
            this.velocityLabel = new System.Windows.Forms.Label();
            this.maxSpeedLabel = new System.Windows.Forms.Label();
            this.telemetryPanel = new System.Windows.Forms.Panel();
            this.lastPacketLabel = new System.Windows.Forms.Label();
            this.packetLabel = new System.Windows.Forms.Label();
            this.velocityRpmTextBox = new System.Windows.Forms.TextBox();
            this.velocityHzTextBox = new System.Windows.Forms.TextBox();
            this.positionValueTextBox = new System.Windows.Forms.TextBox();
            this.velocityRpmLabel = new System.Windows.Forms.Label();
            this.velocityHzLabel = new System.Windows.Forms.Label();
            this.positionValueLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.velocityTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxSpeedNumeric)).BeginInit();
            this.connectionPanel.SuspendLayout();
            this.stepPanel.SuspendLayout();
            this.velocityPanel.SuspendLayout();
            this.telemetryPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // portComboBox
            // 
            this.portComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.portComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.portComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.portComboBox.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.portComboBox.FormattingEnabled = true;
            this.portComboBox.Location = new System.Drawing.Point(16, 48);
            this.portComboBox.Name = "portComboBox";
            this.portComboBox.Size = new System.Drawing.Size(194, 28);
            this.portComboBox.TabIndex = 0;
            // 
            // refreshButton
            // 
            this.refreshButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.refreshButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.refreshButton.ForeColor = System.Drawing.Color.White;
            this.refreshButton.Location = new System.Drawing.Point(218, 46);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(90, 32);
            this.refreshButton.TabIndex = 1;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = false;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // connectButton
            // 
            this.connectButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.connectButton.FlatAppearance.BorderSize = 0;
            this.connectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connectButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.connectButton.ForeColor = System.Drawing.Color.Black;
            this.connectButton.Location = new System.Drawing.Point(16, 92);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(292, 36);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = false;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // ccwStepButton
            // 
            this.ccwStepButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(64)))), ((int)(((byte)(175)))));
            this.ccwStepButton.FlatAppearance.BorderSize = 0;
            this.ccwStepButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ccwStepButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.ccwStepButton.ForeColor = System.Drawing.Color.White;
            this.ccwStepButton.Location = new System.Drawing.Point(16, 48);
            this.ccwStepButton.Name = "ccwStepButton";
            this.ccwStepButton.Size = new System.Drawing.Size(262, 40);
            this.ccwStepButton.TabIndex = 0;
            this.ccwStepButton.Text = "Single Step CCW (DirnByte 3)";
            this.ccwStepButton.UseVisualStyleBackColor = false;
            this.ccwStepButton.Click += new System.EventHandler(this.ccwStepButton_Click);
            // 
            // cwStepButton
            // 
            this.cwStepButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.cwStepButton.FlatAppearance.BorderSize = 0;
            this.cwStepButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cwStepButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.cwStepButton.ForeColor = System.Drawing.Color.Black;
            this.cwStepButton.Location = new System.Drawing.Point(16, 96);
            this.cwStepButton.Name = "cwStepButton";
            this.cwStepButton.Size = new System.Drawing.Size(262, 40);
            this.cwStepButton.TabIndex = 1;
            this.cwStepButton.Text = "Single Step CW (DirnByte 4)";
            this.cwStepButton.UseVisualStyleBackColor = false;
            this.cwStepButton.Click += new System.EventHandler(this.cwStepButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.stopButton.FlatAppearance.BorderSize = 0;
            this.stopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stopButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.stopButton.ForeColor = System.Drawing.Color.White;
            this.stopButton.Location = new System.Drawing.Point(16, 144);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(262, 40);
            this.stopButton.TabIndex = 2;
            this.stopButton.Text = "Stop (halts TimerA1)";
            this.stopButton.UseVisualStyleBackColor = false;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // velocityTrackBar
            // 
            this.velocityTrackBar.LargeChange = 5;
            this.velocityTrackBar.Location = new System.Drawing.Point(16, 66);
            this.velocityTrackBar.Name = "velocityTrackBar";
            this.velocityTrackBar.Size = new System.Drawing.Size(430, 56);
            this.velocityTrackBar.TabIndex = 0;
            this.velocityTrackBar.Scroll += new System.EventHandler(this.velocityTrackBar_Scroll);
            // 
            // maxSpeedNumeric
            // 
            this.maxSpeedNumeric.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.maxSpeedNumeric.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maxSpeedNumeric.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.maxSpeedNumeric.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.maxSpeedNumeric.Location = new System.Drawing.Point(216, 140);
            this.maxSpeedNumeric.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.maxSpeedNumeric.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.maxSpeedNumeric.Name = "maxSpeedNumeric";
            this.maxSpeedNumeric.Size = new System.Drawing.Size(120, 27);
            this.maxSpeedNumeric.TabIndex = 2;
            this.maxSpeedNumeric.Value = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.maxSpeedNumeric.ValueChanged += new System.EventHandler(this.maxSpeedNumeric_ValueChanged);
            // 
            // velocitySummaryLabel
            // 
            this.velocitySummaryLabel.AutoSize = true;
            this.velocitySummaryLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.velocitySummaryLabel.Location = new System.Drawing.Point(13, 180);
            this.velocitySummaryLabel.Name = "velocitySummaryLabel";
            this.velocitySummaryLabel.Size = new System.Drawing.Size(155, 20);
            this.velocitySummaryLabel.TabIndex = 3;
            this.velocitySummaryLabel.Text = "Slider -> Stopped, 0 s";
            // 
            // packetPreviewTextBox
            // 
            this.packetPreviewTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.packetPreviewTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.packetPreviewTextBox.Font = new System.Drawing.Font("Consolas", 10F);
            this.packetPreviewTextBox.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.packetPreviewTextBox.Location = new System.Drawing.Point(16, 116);
            this.packetPreviewTextBox.Name = "packetPreviewTextBox";
            this.packetPreviewTextBox.ReadOnly = true;
            this.packetPreviewTextBox.Size = new System.Drawing.Size(382, 27);
            this.packetPreviewTextBox.TabIndex = 1;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.statusLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.statusLabel.Location = new System.Drawing.Point(16, 568);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(118, 20);
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "Status: Waiting";
            // 
            // serialPort
            // 
            this.serialPort.PortName = "COM1";
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 20F, System.Drawing.FontStyle.Bold);
            this.headerLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.headerLabel.Location = new System.Drawing.Point(12, 9);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(394, 46);
            this.headerLabel.TabIndex = 7;
            this.headerLabel.Text = "Stepper Motor Commander";
            // 
            // subHeaderLabel
            // 
            this.subHeaderLabel.AutoSize = true;
            this.subHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.subHeaderLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.subHeaderLabel.Location = new System.Drawing.Point(16, 48);
            this.subHeaderLabel.Name = "subHeaderLabel";
            this.subHeaderLabel.Size = new System.Drawing.Size(396, 23);
            this.subHeaderLabel.TabIndex = 8;
            this.subHeaderLabel.Text = "MECH 421/423 Lab 5 - Half-step control over USB serial";
            // 
            // connectionPanel
            // 
            this.connectionPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.connectionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.connectionPanel.Controls.Add(this.connectionLabel);
            this.connectionPanel.Controls.Add(this.portComboBox);
            this.connectionPanel.Controls.Add(this.refreshButton);
            this.connectionPanel.Controls.Add(this.connectButton);
            this.connectionPanel.Location = new System.Drawing.Point(22, 88);
            this.connectionPanel.Name = "connectionPanel";
            this.connectionPanel.Size = new System.Drawing.Size(330, 150);
            this.connectionPanel.TabIndex = 9;
            // 
            // connectionLabel
            // 
            this.connectionLabel.AutoSize = true;
            this.connectionLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.connectionLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.connectionLabel.Location = new System.Drawing.Point(12, 12);
            this.connectionLabel.Name = "connectionLabel";
            this.connectionLabel.Size = new System.Drawing.Size(149, 25);
            this.connectionLabel.TabIndex = 3;
            this.connectionLabel.Text = "Board & Serial IO";
            // 
            // stepPanel
            // 
            this.stepPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.stepPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stepPanel.Controls.Add(this.stepLabel);
            this.stepPanel.Controls.Add(this.stopButton);
            this.stepPanel.Controls.Add(this.cwStepButton);
            this.stepPanel.Controls.Add(this.ccwStepButton);
            this.stepPanel.Location = new System.Drawing.Point(22, 256);
            this.stepPanel.Name = "stepPanel";
            this.stepPanel.Size = new System.Drawing.Size(330, 214);
            this.stepPanel.TabIndex = 10;
            // 
            // stepLabel
            // 
            this.stepLabel.AutoSize = true;
            this.stepLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.stepLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.stepLabel.Location = new System.Drawing.Point(12, 12);
            this.stepLabel.Name = "stepLabel";
            this.stepLabel.Size = new System.Drawing.Size(161, 25);
            this.stepLabel.TabIndex = 3;
            this.stepLabel.Text = "Single Half-Steps";
            // 
            // velocityPanel
            // 
            this.velocityPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.velocityPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.velocityPanel.Controls.Add(this.modeLabel);
            this.velocityPanel.Controls.Add(this.freqLabel);
            this.velocityPanel.Controls.Add(this.velocityInstructionLabel);
            this.velocityPanel.Controls.Add(this.velocityLabel);
            this.velocityPanel.Controls.Add(this.velocityTrackBar);
            this.velocityPanel.Controls.Add(this.maxSpeedLabel);
            this.velocityPanel.Controls.Add(this.maxSpeedNumeric);
            this.velocityPanel.Controls.Add(this.velocitySummaryLabel);
            this.velocityPanel.Location = new System.Drawing.Point(370, 88);
            this.velocityPanel.Name = "velocityPanel";
            this.velocityPanel.Size = new System.Drawing.Size(470, 264);
            this.velocityPanel.TabIndex = 11;
            // 
            // modeLabel
            // 
            this.modeLabel.AutoSize = true;
            this.modeLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.modeLabel.Location = new System.Drawing.Point(16, 210);
            this.modeLabel.Name = "modeLabel";
            this.modeLabel.Size = new System.Drawing.Size(128, 20);
            this.modeLabel.TabIndex = 7;
            this.modeLabel.Text = "Mode: Unknown";
            // 
            // freqLabel
            // 
            this.freqLabel.AutoSize = true;
            this.freqLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.freqLabel.Location = new System.Drawing.Point(16, 234);
            this.freqLabel.Name = "freqLabel";
            this.freqLabel.Size = new System.Drawing.Size(207, 20);
            this.freqLabel.TabIndex = 6;
            this.freqLabel.Text = "Step freq: 0 Hz (TA1CCR0 0)";
            // 
            // velocityInstructionLabel
            // 
            this.velocityInstructionLabel.AutoSize = true;
            this.velocityInstructionLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.velocityInstructionLabel.Location = new System.Drawing.Point(16, 48);
            this.velocityInstructionLabel.Name = "velocityInstructionLabel";
            this.velocityInstructionLabel.Size = new System.Drawing.Size(354, 20);
            this.velocityInstructionLabel.TabIndex = 5;
            this.velocityInstructionLabel.Text = "Left = CCW, right = CW. Center is zero / stop command.";
            // 
            // velocityLabel
            // 
            this.velocityLabel.AutoSize = true;
            this.velocityLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.velocityLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.velocityLabel.Location = new System.Drawing.Point(12, 12);
            this.velocityLabel.Name = "velocityLabel";
            this.velocityLabel.Size = new System.Drawing.Size(172, 25);
            this.velocityLabel.TabIndex = 4;
            this.velocityLabel.Text = "Continuous Speed";
            // 
            // maxSpeedLabel
            // 
            this.maxSpeedLabel.AutoSize = true;
            this.maxSpeedLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.maxSpeedLabel.Location = new System.Drawing.Point(16, 142);
            this.maxSpeedLabel.Name = "maxSpeedLabel";
            this.maxSpeedLabel.Size = new System.Drawing.Size(194, 20);
            this.maxSpeedLabel.TabIndex = 4;
            this.maxSpeedLabel.Text = "Max speed (steps/second)";
            // 
            // telemetryPanel
            // 
            this.telemetryPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.telemetryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.telemetryPanel.Controls.Add(this.lastPacketLabel);
            this.telemetryPanel.Controls.Add(this.packetLabel);
            this.telemetryPanel.Controls.Add(this.velocityRpmTextBox);
            this.telemetryPanel.Controls.Add(this.velocityHzTextBox);
            this.telemetryPanel.Controls.Add(this.positionValueTextBox);
            this.telemetryPanel.Controls.Add(this.velocityRpmLabel);
            this.telemetryPanel.Controls.Add(this.velocityHzLabel);
            this.telemetryPanel.Controls.Add(this.positionValueLabel);
            this.telemetryPanel.Controls.Add(this.packetPreviewTextBox);
            this.telemetryPanel.Location = new System.Drawing.Point(370, 370);
            this.telemetryPanel.Name = "telemetryPanel";
            this.telemetryPanel.Size = new System.Drawing.Size(470, 160);
            this.telemetryPanel.TabIndex = 12;
            // 
            // lastPacketLabel
            // 
            this.lastPacketLabel.AutoSize = true;
            this.lastPacketLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.lastPacketLabel.Location = new System.Drawing.Point(12, 92);
            this.lastPacketLabel.Name = "lastPacketLabel";
            this.lastPacketLabel.Size = new System.Drawing.Size(212, 20);
            this.lastPacketLabel.TabIndex = 6;
            this.lastPacketLabel.Text = "Last Packet [start, dirn, d1, d2]";
            // 
            // packetLabel
            // 
            this.packetLabel.AutoSize = true;
            this.packetLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.packetLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.packetLabel.Location = new System.Drawing.Point(12, 12);
            this.packetLabel.Name = "packetLabel";
            this.packetLabel.Size = new System.Drawing.Size(225, 25);
            this.packetLabel.TabIndex = 0;
            this.packetLabel.Text = "Instantaneous Telemetry";
            // 
            // velocityRpmTextBox
            // 
            this.velocityRpmTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.velocityRpmTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.velocityRpmTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.velocityRpmTextBox.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.velocityRpmTextBox.Location = new System.Drawing.Point(332, 54);
            this.velocityRpmTextBox.Name = "velocityRpmTextBox";
            this.velocityRpmTextBox.ReadOnly = true;
            this.velocityRpmTextBox.Size = new System.Drawing.Size(100, 30);
            this.velocityRpmTextBox.TabIndex = 6;
            // 
            // velocityHzTextBox
            // 
            this.velocityHzTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.velocityHzTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.velocityHzTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.velocityHzTextBox.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.velocityHzTextBox.Location = new System.Drawing.Point(214, 54);
            this.velocityHzTextBox.Name = "velocityHzTextBox";
            this.velocityHzTextBox.ReadOnly = true;
            this.velocityHzTextBox.Size = new System.Drawing.Size(80, 30);
            this.velocityHzTextBox.TabIndex = 5;
            // 
            // positionValueTextBox
            // 
            this.positionValueTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.positionValueTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.positionValueTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.positionValueTextBox.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.positionValueTextBox.Location = new System.Drawing.Point(96, 54);
            this.positionValueTextBox.Name = "positionValueTextBox";
            this.positionValueTextBox.ReadOnly = true;
            this.positionValueTextBox.Size = new System.Drawing.Size(80, 30);
            this.positionValueTextBox.TabIndex = 4;
            // 
            // velocityRpmLabel
            // 
            this.velocityRpmLabel.AutoSize = true;
            this.velocityRpmLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.velocityRpmLabel.Location = new System.Drawing.Point(300, 58);
            this.velocityRpmLabel.Name = "velocityRpmLabel";
            this.velocityRpmLabel.Size = new System.Drawing.Size(38, 20);
            this.velocityRpmLabel.TabIndex = 3;
            this.velocityRpmLabel.Text = "rpm:";
            // 
            // velocityHzLabel
            // 
            this.velocityHzLabel.AutoSize = true;
            this.velocityHzLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.velocityHzLabel.Location = new System.Drawing.Point(182, 58);
            this.velocityHzLabel.Name = "velocityHzLabel";
            this.velocityHzLabel.Size = new System.Drawing.Size(26, 20);
            this.velocityHzLabel.TabIndex = 2;
            this.velocityHzLabel.Text = "Hz";
            // 
            // positionValueLabel
            // 
            this.positionValueLabel.AutoSize = true;
            this.positionValueLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.positionValueLabel.Location = new System.Drawing.Point(12, 58);
            this.positionValueLabel.Name = "positionValueLabel";
            this.positionValueLabel.Size = new System.Drawing.Size(69, 20);
            this.positionValueLabel.TabIndex = 2;
            this.positionValueLabel.Text = "Position:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(12)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(866, 604);
            this.Controls.Add(this.telemetryPanel);
            this.Controls.Add(this.velocityPanel);
            this.Controls.Add(this.stepPanel);
            this.Controls.Add(this.connectionPanel);
            this.Controls.Add(this.subHeaderLabel);
            this.Controls.Add(this.headerLabel);
            this.Controls.Add(this.statusLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stepper Motor Control - MECH 421/423";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.velocityTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxSpeedNumeric)).EndInit();
            this.connectionPanel.ResumeLayout(false);
            this.connectionPanel.PerformLayout();
            this.stepPanel.ResumeLayout(false);
            this.stepPanel.PerformLayout();
            this.velocityPanel.ResumeLayout(false);
            this.velocityPanel.PerformLayout();
            this.telemetryPanel.ResumeLayout(false);
            this.telemetryPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox portComboBox;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button ccwStepButton;
        private System.Windows.Forms.Button cwStepButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.TrackBar velocityTrackBar;
        private System.Windows.Forms.NumericUpDown maxSpeedNumeric;
        private System.Windows.Forms.Label velocitySummaryLabel;
        private System.Windows.Forms.TextBox packetPreviewTextBox;
        private System.Windows.Forms.Label statusLabel;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Label subHeaderLabel;
        private System.Windows.Forms.Panel connectionPanel;
        private System.Windows.Forms.Label connectionLabel;
        private System.Windows.Forms.Panel stepPanel;
        private System.Windows.Forms.Label stepLabel;
        private System.Windows.Forms.Panel velocityPanel;
        private System.Windows.Forms.Label velocityLabel;
        private System.Windows.Forms.Label velocityInstructionLabel;
        private System.Windows.Forms.Label maxSpeedLabel;
        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.Label freqLabel;
        private System.Windows.Forms.Panel telemetryPanel;
        private System.Windows.Forms.Label packetLabel;
        private System.Windows.Forms.Label positionValueLabel;
        private System.Windows.Forms.Label velocityHzLabel;
        private System.Windows.Forms.Label velocityRpmLabel;
        private System.Windows.Forms.TextBox velocityRpmTextBox;
        private System.Windows.Forms.TextBox velocityHzTextBox;
        private System.Windows.Forms.TextBox positionValueTextBox;
        private System.Windows.Forms.Label lastPacketLabel;
    }
}

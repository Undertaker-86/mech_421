namespace PWM_Control
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.comboBoxCOMPorts = new System.Windows.Forms.ComboBox();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.connectDisconnectSerialButton = new System.Windows.Forms.Button();
            this.startByteLabel = new System.Windows.Forms.Label();
            this.intructionByteLabel = new System.Windows.Forms.Label();
            this.pwmMSBLabel = new System.Windows.Forms.Label();
            this.pwmLSBLabel = new System.Windows.Forms.Label();
            this.escapeByteLabel = new System.Windows.Forms.Label();
            this.startByteTextbox = new System.Windows.Forms.TextBox();
            this.instructionByteTextbox = new System.Windows.Forms.TextBox();
            this.pwmMSBTextbox = new System.Windows.Forms.TextBox();
            this.pwmLSBTextbox = new System.Windows.Forms.TextBox();
            this.escapeByteTextbox = new System.Windows.Forms.TextBox();
            this.transmitButton = new System.Windows.Forms.Button();
            this.pwmDutyScrollbar = new System.Windows.Forms.HScrollBar();
            this.scrollBarLabel = new System.Windows.Forms.Label();
            this.posLabel = new System.Windows.Forms.Label();
            this.velLabel = new System.Windows.Forms.Label();
            this.posTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.velHzTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.velRpmTextbox = new System.Windows.Forms.TextBox();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.motorChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dataStreamTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.StartSaveButton = new System.Windows.Forms.Button();
            this.EndSaveButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.motorChart)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxCOMPorts
            // 
            this.comboBoxCOMPorts.FormattingEnabled = true;
            this.comboBoxCOMPorts.Location = new System.Drawing.Point(8, 8);
            this.comboBoxCOMPorts.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBoxCOMPorts.Name = "comboBoxCOMPorts";
            this.comboBoxCOMPorts.Size = new System.Drawing.Size(82, 24);
            this.comboBoxCOMPorts.TabIndex = 0;
            this.comboBoxCOMPorts.SelectedIndexChanged += new System.EventHandler(this.comboBoxCOMPorts_SelectedIndexChanged);
            // 
            // serialPort
            // 
            this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_DataReceived);
            // 
            // connectDisconnectSerialButton
            // 
            this.connectDisconnectSerialButton.Location = new System.Drawing.Point(93, 1);
            this.connectDisconnectSerialButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.connectDisconnectSerialButton.Name = "connectDisconnectSerialButton";
            this.connectDisconnectSerialButton.Size = new System.Drawing.Size(384, 32);
            this.connectDisconnectSerialButton.TabIndex = 1;
            this.connectDisconnectSerialButton.Text = "Connect";
            this.connectDisconnectSerialButton.UseVisualStyleBackColor = true;
            this.connectDisconnectSerialButton.Click += new System.EventHandler(this.connectDisconnectSerialButton_Click);
            // 
            // startByteLabel
            // 
            this.startByteLabel.AutoSize = true;
            this.startByteLabel.Location = new System.Drawing.Point(5, 49);
            this.startByteLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.startByteLabel.Name = "startByteLabel";
            this.startByteLabel.Size = new System.Drawing.Size(64, 16);
            this.startByteLabel.TabIndex = 2;
            this.startByteLabel.Text = "Start Byte";
            // 
            // intructionByteLabel
            // 
            this.intructionByteLabel.AutoSize = true;
            this.intructionByteLabel.Location = new System.Drawing.Point(89, 49);
            this.intructionByteLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.intructionByteLabel.Name = "intructionByteLabel";
            this.intructionByteLabel.Size = new System.Drawing.Size(96, 16);
            this.intructionByteLabel.TabIndex = 3;
            this.intructionByteLabel.Text = "Instruction Byte";
            // 
            // pwmMSBLabel
            // 
            this.pwmMSBLabel.AutoSize = true;
            this.pwmMSBLabel.Location = new System.Drawing.Point(203, 49);
            this.pwmMSBLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.pwmMSBLabel.Name = "pwmMSBLabel";
            this.pwmMSBLabel.Size = new System.Drawing.Size(72, 16);
            this.pwmMSBLabel.TabIndex = 4;
            this.pwmMSBLabel.Text = "PWM MSB";
            // 
            // pwmLSBLabel
            // 
            this.pwmLSBLabel.AutoSize = true;
            this.pwmLSBLabel.Location = new System.Drawing.Point(299, 49);
            this.pwmLSBLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.pwmLSBLabel.Name = "pwmLSBLabel";
            this.pwmLSBLabel.Size = new System.Drawing.Size(68, 16);
            this.pwmLSBLabel.TabIndex = 5;
            this.pwmLSBLabel.Text = "PWM LSB";
            // 
            // escapeByteLabel
            // 
            this.escapeByteLabel.AutoSize = true;
            this.escapeByteLabel.Location = new System.Drawing.Point(388, 49);
            this.escapeByteLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.escapeByteLabel.Name = "escapeByteLabel";
            this.escapeByteLabel.Size = new System.Drawing.Size(84, 16);
            this.escapeByteLabel.TabIndex = 6;
            this.escapeByteLabel.Text = "Escape Byte";
            // 
            // startByteTextbox
            // 
            this.startByteTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.startByteTextbox.Location = new System.Drawing.Point(8, 67);
            this.startByteTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.startByteTextbox.Name = "startByteTextbox";
            this.startByteTextbox.Size = new System.Drawing.Size(68, 22);
            this.startByteTextbox.TabIndex = 7;
            // 
            // instructionByteTextbox
            // 
            this.instructionByteTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.instructionByteTextbox.Location = new System.Drawing.Point(93, 67);
            this.instructionByteTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.instructionByteTextbox.Name = "instructionByteTextbox";
            this.instructionByteTextbox.Size = new System.Drawing.Size(104, 22);
            this.instructionByteTextbox.TabIndex = 8;
            // 
            // pwmMSBTextbox
            // 
            this.pwmMSBTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pwmMSBTextbox.Location = new System.Drawing.Point(206, 67);
            this.pwmMSBTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pwmMSBTextbox.Name = "pwmMSBTextbox";
            this.pwmMSBTextbox.Size = new System.Drawing.Size(75, 22);
            this.pwmMSBTextbox.TabIndex = 9;
            // 
            // pwmLSBTextbox
            // 
            this.pwmLSBTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pwmLSBTextbox.Location = new System.Drawing.Point(291, 67);
            this.pwmLSBTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pwmLSBTextbox.Name = "pwmLSBTextbox";
            this.pwmLSBTextbox.Size = new System.Drawing.Size(81, 22);
            this.pwmLSBTextbox.TabIndex = 10;
            // 
            // escapeByteTextbox
            // 
            this.escapeByteTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.escapeByteTextbox.Location = new System.Drawing.Point(391, 67);
            this.escapeByteTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.escapeByteTextbox.Name = "escapeByteTextbox";
            this.escapeByteTextbox.Size = new System.Drawing.Size(86, 22);
            this.escapeByteTextbox.TabIndex = 11;
            // 
            // transmitButton
            // 
            this.transmitButton.Location = new System.Drawing.Point(8, 97);
            this.transmitButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.transmitButton.Name = "transmitButton";
            this.transmitButton.Size = new System.Drawing.Size(469, 32);
            this.transmitButton.TabIndex = 12;
            this.transmitButton.Text = "Transmit Data";
            this.transmitButton.UseVisualStyleBackColor = true;
            this.transmitButton.Click += new System.EventHandler(this.transmitButton_Click);
            // 
            // pwmDutyScrollbar
            // 
            this.pwmDutyScrollbar.Location = new System.Drawing.Point(8, 163);
            this.pwmDutyScrollbar.Name = "pwmDutyScrollbar";
            this.pwmDutyScrollbar.Size = new System.Drawing.Size(469, 53);
            this.pwmDutyScrollbar.TabIndex = 13;
            this.pwmDutyScrollbar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.pwmDutyScrollbar_Scroll);
            // 
            // scrollBarLabel
            // 
            this.scrollBarLabel.AutoSize = true;
            this.scrollBarLabel.Location = new System.Drawing.Point(8, 138);
            this.scrollBarLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.scrollBarLabel.Name = "scrollBarLabel";
            this.scrollBarLabel.Size = new System.Drawing.Size(293, 16);
            this.scrollBarLabel.TabIndex = 14;
            this.scrollBarLabel.Text = "Scroll to Change DC Motor Direction and Speed:";
            // 
            // posLabel
            // 
            this.posLabel.AutoSize = true;
            this.posLabel.Location = new System.Drawing.Point(9, 227);
            this.posLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.posLabel.Name = "posLabel";
            this.posLabel.Size = new System.Drawing.Size(144, 16);
            this.posLabel.TabIndex = 15;
            this.posLabel.Text = "Instantaneous Position:";
            // 
            // velLabel
            // 
            this.velLabel.AutoSize = true;
            this.velLabel.Location = new System.Drawing.Point(9, 259);
            this.velLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.velLabel.Name = "velLabel";
            this.velLabel.Size = new System.Drawing.Size(144, 16);
            this.velLabel.TabIndex = 16;
            this.velLabel.Text = "Instantaneous Velocity:";
            // 
            // posTextBox
            // 
            this.posTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.posTextBox.Location = new System.Drawing.Point(171, 227);
            this.posTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.posTextBox.Name = "posTextBox";
            this.posTextBox.Size = new System.Drawing.Size(67, 22);
            this.posTextBox.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(241, 227);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 16);
            this.label1.TabIndex = 18;
            this.label1.Text = "Deg";
            // 
            // velHzTextbox
            // 
            this.velHzTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.velHzTextbox.Location = new System.Drawing.Point(171, 259);
            this.velHzTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.velHzTextbox.Name = "velHzTextbox";
            this.velHzTextbox.Size = new System.Drawing.Size(67, 22);
            this.velHzTextbox.TabIndex = 19;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(241, 259);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 16);
            this.label2.TabIndex = 20;
            this.label2.Text = "Hz";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(351, 257);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 16);
            this.label3.TabIndex = 22;
            this.label3.Text = "rpm";
            // 
            // velRpmTextbox
            // 
            this.velRpmTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.velRpmTextbox.Location = new System.Drawing.Point(281, 257);
            this.velRpmTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.velRpmTextbox.Name = "velRpmTextbox";
            this.velRpmTextbox.Size = new System.Drawing.Size(67, 22);
            this.velRpmTextbox.TabIndex = 21;
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // motorChart
            // 
            chartArea2.Name = "ChartArea1";
            this.motorChart.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.motorChart.Legends.Add(legend2);
            this.motorChart.Location = new System.Drawing.Point(11, 294);
            this.motorChart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.motorChart.Name = "motorChart";
            this.motorChart.Size = new System.Drawing.Size(967, 319);
            this.motorChart.TabIndex = 23;
            this.motorChart.Text = "chart1";
            // 
            // dataStreamTextbox
            // 
            this.dataStreamTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataStreamTextbox.Location = new System.Drawing.Point(517, 35);
            this.dataStreamTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dataStreamTextbox.Multiline = true;
            this.dataStreamTextbox.Name = "dataStreamTextbox";
            this.dataStreamTextbox.Size = new System.Drawing.Size(442, 193);
            this.dataStreamTextbox.TabIndex = 24;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(513, 13);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(150, 16);
            this.label4.TabIndex = 25;
            this.label4.Text = "Test Data Transmission";
            // 
            // StartSaveButton
            // 
            this.StartSaveButton.Location = new System.Drawing.Point(517, 233);
            this.StartSaveButton.Name = "StartSaveButton";
            this.StartSaveButton.Size = new System.Drawing.Size(86, 40);
            this.StartSaveButton.TabIndex = 26;
            this.StartSaveButton.Text = "StartSave";
            this.StartSaveButton.UseVisualStyleBackColor = true;
            this.StartSaveButton.Click += new System.EventHandler(this.StartSaveButton_Click);
            // 
            // EndSaveButton
            // 
            this.EndSaveButton.Location = new System.Drawing.Point(609, 233);
            this.EndSaveButton.Name = "EndSaveButton";
            this.EndSaveButton.Size = new System.Drawing.Size(86, 40);
            this.EndSaveButton.TabIndex = 27;
            this.EndSaveButton.Text = "EndSave";
            this.EndSaveButton.UseVisualStyleBackColor = true;
            this.EndSaveButton.Click += new System.EventHandler(this.EndSaveButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 616);
            this.Controls.Add(this.EndSaveButton);
            this.Controls.Add(this.StartSaveButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dataStreamTextbox);
            this.Controls.Add(this.motorChart);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.velRpmTextbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.velHzTextbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.posTextBox);
            this.Controls.Add(this.velLabel);
            this.Controls.Add(this.posLabel);
            this.Controls.Add(this.scrollBarLabel);
            this.Controls.Add(this.pwmDutyScrollbar);
            this.Controls.Add(this.transmitButton);
            this.Controls.Add(this.escapeByteTextbox);
            this.Controls.Add(this.pwmLSBTextbox);
            this.Controls.Add(this.pwmMSBTextbox);
            this.Controls.Add(this.instructionByteTextbox);
            this.Controls.Add(this.startByteTextbox);
            this.Controls.Add(this.escapeByteLabel);
            this.Controls.Add(this.pwmLSBLabel);
            this.Controls.Add(this.pwmMSBLabel);
            this.Controls.Add(this.intructionByteLabel);
            this.Controls.Add(this.startByteLabel);
            this.Controls.Add(this.connectDisconnectSerialButton);
            this.Controls.Add(this.comboBoxCOMPorts);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.motorChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxCOMPorts;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.Button connectDisconnectSerialButton;
        private System.Windows.Forms.Label startByteLabel;
        private System.Windows.Forms.Label intructionByteLabel;
        private System.Windows.Forms.Label pwmMSBLabel;
        private System.Windows.Forms.Label pwmLSBLabel;
        private System.Windows.Forms.Label escapeByteLabel;
        private System.Windows.Forms.TextBox startByteTextbox;
        private System.Windows.Forms.TextBox instructionByteTextbox;
        private System.Windows.Forms.TextBox pwmMSBTextbox;
        private System.Windows.Forms.TextBox pwmLSBTextbox;
        private System.Windows.Forms.TextBox escapeByteTextbox;
        private System.Windows.Forms.Button transmitButton;
        private System.Windows.Forms.HScrollBar pwmDutyScrollbar;
        private System.Windows.Forms.Label scrollBarLabel;
        private System.Windows.Forms.Label posLabel;
        private System.Windows.Forms.Label velLabel;
        private System.Windows.Forms.TextBox posTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox velHzTextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox velRpmTextbox;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.DataVisualization.Charting.Chart motorChart;
        private System.Windows.Forms.TextBox dataStreamTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button StartSaveButton;
        private System.Windows.Forms.Button EndSaveButton;
    }
}


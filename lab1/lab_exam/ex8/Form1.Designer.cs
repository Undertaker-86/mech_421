namespace ex8
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
            this.comboBoxCOMPorts = new System.Windows.Forms.ComboBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxAx = new System.Windows.Forms.TextBox();
            this.textBoxAz = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxAy = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxAvgAy = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxAvgAz = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxAvgAx = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxQueueSize = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxOrientation = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxGesture = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.textBoxSelectFile = new System.Windows.Forms.TextBox();
            this.buttonSaveData = new System.Windows.Forms.Button();
            this.textBoxHistory = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxStateHistory = new System.Windows.Forms.TextBox();
            this.textBoxAvgAcceleration = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxMinAy = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxMinAz = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxMinAx = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxBufferSize = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxCOMPorts
            // 
            this.comboBoxCOMPorts.FormattingEnabled = true;
            this.comboBoxCOMPorts.Location = new System.Drawing.Point(37, 38);
            this.comboBoxCOMPorts.Name = "comboBoxCOMPorts";
            this.comboBoxCOMPorts.Size = new System.Drawing.Size(154, 33);
            this.comboBoxCOMPorts.TabIndex = 0;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(232, 38);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(198, 45);
            this.buttonConnect.TabIndex = 1;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(456, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ax (Byte)";
            // 
            // textBoxAx
            // 
            this.textBoxAx.Location = new System.Drawing.Point(562, 39);
            this.textBoxAx.Name = "textBoxAx";
            this.textBoxAx.Size = new System.Drawing.Size(109, 31);
            this.textBoxAx.TabIndex = 3;
            // 
            // textBoxAz
            // 
            this.textBoxAz.Location = new System.Drawing.Point(562, 147);
            this.textBoxAz.Name = "textBoxAz";
            this.textBoxAz.Size = new System.Drawing.Size(109, 31);
            this.textBoxAz.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(456, 150);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Az (Byte)";
            // 
            // textBoxAy
            // 
            this.textBoxAy.Location = new System.Drawing.Point(562, 92);
            this.textBoxAy.Name = "textBoxAy";
            this.textBoxAy.Size = new System.Drawing.Size(109, 31);
            this.textBoxAy.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(456, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 25);
            this.label3.TabIndex = 6;
            this.label3.Text = "Ay (Byte)";
            // 
            // textBoxAvgAy
            // 
            this.textBoxAvgAy.Location = new System.Drawing.Point(891, 89);
            this.textBoxAvgAy.Name = "textBoxAvgAy";
            this.textBoxAvgAy.Size = new System.Drawing.Size(198, 31);
            this.textBoxAvgAy.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(687, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(201, 25);
            this.label4.TabIndex = 12;
            this.label4.Text = "Max 100p Ay (Byte)";
            // 
            // textBoxAvgAz
            // 
            this.textBoxAvgAz.Location = new System.Drawing.Point(891, 144);
            this.textBoxAvgAz.Name = "textBoxAvgAz";
            this.textBoxAvgAz.Size = new System.Drawing.Size(198, 31);
            this.textBoxAvgAz.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(687, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(201, 25);
            this.label5.TabIndex = 10;
            this.label5.Text = "Max 100p Az (Byte)";
            // 
            // textBoxAvgAx
            // 
            this.textBoxAvgAx.Location = new System.Drawing.Point(891, 36);
            this.textBoxAvgAx.Name = "textBoxAvgAx";
            this.textBoxAvgAx.Size = new System.Drawing.Size(198, 31);
            this.textBoxAvgAx.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(687, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(201, 25);
            this.label6.TabIndex = 8;
            this.label6.Text = "Max 100p Ax (Byte)";
            // 
            // textBoxQueueSize
            // 
            this.textBoxQueueSize.Location = new System.Drawing.Point(232, 97);
            this.textBoxQueueSize.Name = "textBoxQueueSize";
            this.textBoxQueueSize.Size = new System.Drawing.Size(198, 31);
            this.textBoxQueueSize.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 100);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(187, 25);
            this.label7.TabIndex = 14;
            this.label7.Text = "Queue Size (Byte)";
            // 
            // textBoxOrientation
            // 
            this.textBoxOrientation.Location = new System.Drawing.Point(1247, 290);
            this.textBoxOrientation.Multiline = true;
            this.textBoxOrientation.Name = "textBoxOrientation";
            this.textBoxOrientation.Size = new System.Drawing.Size(526, 550);
            this.textBoxOrientation.TabIndex = 17;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1419, 262);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(190, 25);
            this.label8.TabIndex = 16;
            this.label8.Text = "Orientation History";
            // 
            // textBoxGesture
            // 
            this.textBoxGesture.Location = new System.Drawing.Point(232, 185);
            this.textBoxGesture.Name = "textBoxGesture";
            this.textBoxGesture.Size = new System.Drawing.Size(198, 31);
            this.textBoxGesture.TabIndex = 19;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(39, 188);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 25);
            this.label9.TabIndex = 18;
            this.label9.Text = "Gesture";
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.Location = new System.Drawing.Point(1667, 38);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(198, 57);
            this.buttonSelectFile.TabIndex = 20;
            this.buttonSelectFile.Text = "Select File";
            this.buttonSelectFile.UseVisualStyleBackColor = true;
            this.buttonSelectFile.Click += new System.EventHandler(this.buttonSelectFile_Click);
            // 
            // textBoxSelectFile
            // 
            this.textBoxSelectFile.Location = new System.Drawing.Point(1667, 115);
            this.textBoxSelectFile.Multiline = true;
            this.textBoxSelectFile.Name = "textBoxSelectFile";
            this.textBoxSelectFile.Size = new System.Drawing.Size(198, 57);
            this.textBoxSelectFile.TabIndex = 21;
            // 
            // buttonSaveData
            // 
            this.buttonSaveData.Location = new System.Drawing.Point(1667, 185);
            this.buttonSaveData.Name = "buttonSaveData";
            this.buttonSaveData.Size = new System.Drawing.Size(198, 55);
            this.buttonSaveData.TabIndex = 22;
            this.buttonSaveData.Text = "Save to CSV";
            this.buttonSaveData.UseVisualStyleBackColor = true;
            this.buttonSaveData.Click += new System.EventHandler(this.buttonSaveData_Click);
            // 
            // textBoxHistory
            // 
            this.textBoxHistory.Location = new System.Drawing.Point(668, 290);
            this.textBoxHistory.Multiline = true;
            this.textBoxHistory.Name = "textBoxHistory";
            this.textBoxHistory.Size = new System.Drawing.Size(526, 550);
            this.textBoxHistory.TabIndex = 24;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(827, 262);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(204, 25);
            this.label10.TabIndex = 23;
            this.label10.Text = "Acceleration History";
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(275, 262);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(135, 25);
            this.label11.TabIndex = 25;
            this.label11.Text = "State History";
            // 
            // textBoxStateHistory
            // 
            this.textBoxStateHistory.Location = new System.Drawing.Point(83, 290);
            this.textBoxStateHistory.Multiline = true;
            this.textBoxStateHistory.Name = "textBoxStateHistory";
            this.textBoxStateHistory.Size = new System.Drawing.Size(526, 550);
            this.textBoxStateHistory.TabIndex = 26;
            // 
            // textBoxAvgAcceleration
            // 
            this.textBoxAvgAcceleration.Location = new System.Drawing.Point(1347, 197);
            this.textBoxAvgAcceleration.Name = "textBoxAvgAcceleration";
            this.textBoxAvgAcceleration.Size = new System.Drawing.Size(198, 31);
            this.textBoxAvgAcceleration.TabIndex = 28;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(1106, 198);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(229, 25);
            this.label12.TabIndex = 27;
            this.label12.Text = "Avg 50p magnitude (g)";
            // 
            // textBoxMinAy
            // 
            this.textBoxMinAy.Location = new System.Drawing.Point(1347, 92);
            this.textBoxMinAy.Name = "textBoxMinAy";
            this.textBoxMinAy.Size = new System.Drawing.Size(198, 31);
            this.textBoxMinAy.TabIndex = 34;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(1106, 93);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(164, 25);
            this.label13.TabIndex = 33;
            this.label13.Text = "Min 100p Ay (g)";
            // 
            // textBoxMinAz
            // 
            this.textBoxMinAz.Location = new System.Drawing.Point(1347, 147);
            this.textBoxMinAz.Name = "textBoxMinAz";
            this.textBoxMinAz.Size = new System.Drawing.Size(198, 31);
            this.textBoxMinAz.TabIndex = 32;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(1106, 148);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(164, 25);
            this.label14.TabIndex = 31;
            this.label14.Text = "Min 100p Az (g)";
            // 
            // textBoxMinAx
            // 
            this.textBoxMinAx.Location = new System.Drawing.Point(1347, 39);
            this.textBoxMinAx.Name = "textBoxMinAx";
            this.textBoxMinAx.Size = new System.Drawing.Size(198, 31);
            this.textBoxMinAx.TabIndex = 30;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(1106, 40);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(164, 25);
            this.label15.TabIndex = 29;
            this.label15.Text = "Min 100p Ax (g)";
            // 
            // textBoxBufferSize
            // 
            this.textBoxBufferSize.Location = new System.Drawing.Point(232, 142);
            this.textBoxBufferSize.Name = "textBoxBufferSize";
            this.textBoxBufferSize.Size = new System.Drawing.Size(198, 31);
            this.textBoxBufferSize.TabIndex = 36;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(39, 145);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(180, 25);
            this.label16.TabIndex = 35;
            this.label16.Text = "Buffer Size (Byte)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1877, 868);
            this.Controls.Add(this.textBoxBufferSize);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.textBoxMinAy);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.textBoxMinAz);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.textBoxMinAx);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.textBoxAvgAcceleration);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.textBoxStateHistory);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBoxHistory);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.buttonSaveData);
            this.Controls.Add(this.textBoxSelectFile);
            this.Controls.Add(this.buttonSelectFile);
            this.Controls.Add(this.textBoxGesture);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxOrientation);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxQueueSize);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxAvgAy);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxAvgAz);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxAvgAx);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxAy);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxAz);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxAx);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.comboBoxCOMPorts);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxCOMPorts;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxAx;
        private System.Windows.Forms.TextBox textBoxAz;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxAy;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxAvgAy;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxAvgAz;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxAvgAx;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxQueueSize;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxOrientation;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxGesture;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.TextBox textBoxSelectFile;
        private System.Windows.Forms.Button buttonSaveData;
        private System.Windows.Forms.TextBox textBoxHistory;
        private System.Windows.Forms.Label label10;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxStateHistory;
        private System.Windows.Forms.TextBox textBoxAvgAcceleration;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxMinAy;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxMinAz;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxMinAx;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxBufferSize;
        private System.Windows.Forms.Label label16;
    }
}


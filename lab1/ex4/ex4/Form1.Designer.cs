namespace ex4
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
            this.buttonDisconnectSerial = new System.Windows.Forms.Button();
            this.comboBoxCOMPorts = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxBytesToRead = new System.Windows.Forms.TextBox();
            this.textBoxTempStringLength = new System.Windows.Forms.TextBox();
            this.textBoxItemsInQueue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxSerialDataStream = new System.Windows.Forms.TextBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxTimerTick_ms = new System.Windows.Forms.TextBox();
            this.buttonSetInterval = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonDisconnectSerial
            // 
            this.buttonDisconnectSerial.Location = new System.Drawing.Point(251, 41);
            this.buttonDisconnectSerial.Name = "buttonDisconnectSerial";
            this.buttonDisconnectSerial.Size = new System.Drawing.Size(233, 39);
            this.buttonDisconnectSerial.TabIndex = 0;
            this.buttonDisconnectSerial.Text = "Connect";
            this.buttonDisconnectSerial.UseVisualStyleBackColor = true;
            this.buttonDisconnectSerial.Click += new System.EventHandler(this.buttonDisconnectSerial_Click);
            // 
            // comboBoxCOMPorts
            // 
            this.comboBoxCOMPorts.FormattingEnabled = true;
            this.comboBoxCOMPorts.Location = new System.Drawing.Point(12, 41);
            this.comboBoxCOMPorts.Name = "comboBoxCOMPorts";
            this.comboBoxCOMPorts.Size = new System.Drawing.Size(215, 33);
            this.comboBoxCOMPorts.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 146);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Serial Bytes to Read";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 194);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(200, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Temp String Length";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 241);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "Items in Queue";
            // 
            // textBoxBytesToRead
            // 
            this.textBoxBytesToRead.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.textBoxBytesToRead.Location = new System.Drawing.Point(251, 146);
            this.textBoxBytesToRead.Name = "textBoxBytesToRead";
            this.textBoxBytesToRead.ReadOnly = true;
            this.textBoxBytesToRead.Size = new System.Drawing.Size(233, 31);
            this.textBoxBytesToRead.TabIndex = 5;
            // 
            // textBoxTempStringLength
            // 
            this.textBoxTempStringLength.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.textBoxTempStringLength.Location = new System.Drawing.Point(251, 194);
            this.textBoxTempStringLength.Name = "textBoxTempStringLength";
            this.textBoxTempStringLength.ReadOnly = true;
            this.textBoxTempStringLength.Size = new System.Drawing.Size(233, 31);
            this.textBoxTempStringLength.TabIndex = 6;
            // 
            // textBoxItemsInQueue
            // 
            this.textBoxItemsInQueue.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.textBoxItemsInQueue.Location = new System.Drawing.Point(251, 241);
            this.textBoxItemsInQueue.Name = "textBoxItemsInQueue";
            this.textBoxItemsInQueue.ReadOnly = true;
            this.textBoxItemsInQueue.Size = new System.Drawing.Size(233, 31);
            this.textBoxItemsInQueue.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 297);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(198, 25);
            this.label4.TabIndex = 8;
            this.label4.Text = "Serial Data Stream:";
            // 
            // textBoxSerialDataStream
            // 
            this.textBoxSerialDataStream.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.textBoxSerialDataStream.Location = new System.Drawing.Point(18, 334);
            this.textBoxSerialDataStream.Multiline = true;
            this.textBoxSerialDataStream.Name = "textBoxSerialDataStream";
            this.textBoxSerialDataStream.ReadOnly = true;
            this.textBoxSerialDataStream.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSerialDataStream.Size = new System.Drawing.Size(614, 500);
            this.textBoxSerialDataStream.TabIndex = 9;
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
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 25);
            this.label5.TabIndex = 10;
            this.label5.Text = "Timer Tick (ms)";
            // 
            // textBoxTimerTick_ms
            // 
            this.textBoxTimerTick_ms.Location = new System.Drawing.Point(251, 99);
            this.textBoxTimerTick_ms.Name = "textBoxTimerTick_ms";
            this.textBoxTimerTick_ms.Size = new System.Drawing.Size(233, 31);
            this.textBoxTimerTick_ms.TabIndex = 11;
            // 
            // buttonSetInterval
            // 
            this.buttonSetInterval.Location = new System.Drawing.Point(509, 95);
            this.buttonSetInterval.Name = "buttonSetInterval";
            this.buttonSetInterval.Size = new System.Drawing.Size(105, 39);
            this.buttonSetInterval.TabIndex = 12;
            this.buttonSetInterval.Text = "Send";
            this.buttonSetInterval.UseVisualStyleBackColor = true;
            this.buttonSetInterval.Click += new System.EventHandler(this.buttonSetInterval_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 846);
            this.Controls.Add(this.buttonSetInterval);
            this.Controls.Add(this.textBoxTimerTick_ms);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxSerialDataStream);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxItemsInQueue);
            this.Controls.Add(this.textBoxTempStringLength);
            this.Controls.Add(this.textBoxBytesToRead);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxCOMPorts);
            this.Controls.Add(this.buttonDisconnectSerial);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonDisconnectSerial;
        private System.Windows.Forms.ComboBox comboBoxCOMPorts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxBytesToRead;
        private System.Windows.Forms.TextBox textBoxTempStringLength;
        private System.Windows.Forms.TextBox textBoxItemsInQueue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxSerialDataStream;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxTimerTick_ms;
        private System.Windows.Forms.Button buttonSetInterval;
    }
}


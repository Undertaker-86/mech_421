
namespace PWM_Control
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Controls
        private System.Windows.Forms.HScrollBar pwmDutyScrollbar;
        private System.Windows.Forms.TextBox startByteTextbox;
        private System.Windows.Forms.Label labelStart;
        private System.Windows.Forms.TextBox instructionByteTextbox;
        private System.Windows.Forms.Label labelInstruction;
        private System.Windows.Forms.TextBox pwmMSBTextbox;
        private System.Windows.Forms.Label labelMSB;
        private System.Windows.Forms.TextBox pwmLSBTextbox;
        private System.Windows.Forms.Label labelLSB;
        private System.Windows.Forms.TextBox escapeByteTextbox;
        private System.Windows.Forms.Label labelEscape;
        private System.Windows.Forms.ComboBox comboBoxCOMPorts;
        private System.Windows.Forms.Button connectDisconnectSerialButton;
        private System.Windows.Forms.Button transmitButton;
        private System.IO.Ports.SerialPort serialPort;

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
            this.pwmDutyScrollbar = new System.Windows.Forms.HScrollBar();
            this.startByteTextbox = new System.Windows.Forms.TextBox();
            this.instructionByteTextbox = new System.Windows.Forms.TextBox();
            this.pwmMSBTextbox = new System.Windows.Forms.TextBox();
            this.pwmLSBTextbox = new System.Windows.Forms.TextBox();
            this.escapeByteTextbox = new System.Windows.Forms.TextBox();
            this.comboBoxCOMPorts = new System.Windows.Forms.ComboBox();
            this.connectDisconnectSerialButton = new System.Windows.Forms.Button();
            this.transmitButton = new System.Windows.Forms.Button();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.labelStart = new System.Windows.Forms.Label();
            this.labelInstruction = new System.Windows.Forms.Label();
            this.labelMSB = new System.Windows.Forms.Label();
            this.labelLSB = new System.Windows.Forms.Label();
            this.labelEscape = new System.Windows.Forms.Label();
            
            this.SuspendLayout();

            // 
            // pwmDutyScrollbar
            // 
            this.pwmDutyScrollbar.Location = new System.Drawing.Point(50, 20);
            this.pwmDutyScrollbar.Name = "pwmDutyScrollbar";
            this.pwmDutyScrollbar.Size = new System.Drawing.Size(300, 26);
            this.pwmDutyScrollbar.TabIndex = 0;
            this.pwmDutyScrollbar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.pwmDutyScrollbar_Scroll);

            // 
            // Label Layouts
            //
            this.labelStart.Text = "Start";
            this.labelStart.Location = new System.Drawing.Point(50, 60);
            this.labelStart.Size = new System.Drawing.Size(50, 20);

            this.labelInstruction.Text = "Inst";
            this.labelInstruction.Location = new System.Drawing.Point(110, 60);
            this.labelInstruction.Size = new System.Drawing.Size(50, 20);

            this.labelMSB.Text = "MSB";
            this.labelMSB.Location = new System.Drawing.Point(170, 60);
            this.labelMSB.Size = new System.Drawing.Size(50, 20);

            this.labelLSB.Text = "LSB";
            this.labelLSB.Location = new System.Drawing.Point(230, 60);
            this.labelLSB.Size = new System.Drawing.Size(50, 20);

            this.labelEscape.Text = "Esc";
            this.labelEscape.Location = new System.Drawing.Point(290, 60);
            this.labelEscape.Size = new System.Drawing.Size(50, 20);

            // 
            // startByteTextbox
            // 
            this.startByteTextbox.Location = new System.Drawing.Point(50, 80);
            this.startByteTextbox.Name = "startByteTextbox";
            this.startByteTextbox.Size = new System.Drawing.Size(50, 23);
            this.startByteTextbox.TabIndex = 1;

            // 
            // instructionByteTextbox
            // 
            this.instructionByteTextbox.Location = new System.Drawing.Point(110, 80);
            this.instructionByteTextbox.Name = "instructionByteTextbox";
            this.instructionByteTextbox.Size = new System.Drawing.Size(50, 23);
            this.instructionByteTextbox.TabIndex = 2;

            // 
            // pwmMSBTextbox
            // 
            this.pwmMSBTextbox.Location = new System.Drawing.Point(170, 80);
            this.pwmMSBTextbox.Name = "pwmMSBTextbox";
            this.pwmMSBTextbox.Size = new System.Drawing.Size(50, 23);
            this.pwmMSBTextbox.TabIndex = 3;

            // 
            // pwmLSBTextbox
            // 
            this.pwmLSBTextbox.Location = new System.Drawing.Point(230, 80);
            this.pwmLSBTextbox.Name = "pwmLSBTextbox";
            this.pwmLSBTextbox.Size = new System.Drawing.Size(50, 23);
            this.pwmLSBTextbox.TabIndex = 4;

            // 
            // escapeByteTextbox
            // 
            this.escapeByteTextbox.Location = new System.Drawing.Point(290, 80);
            this.escapeByteTextbox.Name = "escapeByteTextbox";
            this.escapeByteTextbox.Size = new System.Drawing.Size(50, 23);
            this.escapeByteTextbox.TabIndex = 5;

            // 
            // comboBoxCOMPorts
            // 
            this.comboBoxCOMPorts.FormattingEnabled = true;
            this.comboBoxCOMPorts.Location = new System.Drawing.Point(50, 150);
            this.comboBoxCOMPorts.Name = "comboBoxCOMPorts";
            this.comboBoxCOMPorts.Size = new System.Drawing.Size(121, 23);
            this.comboBoxCOMPorts.TabIndex = 6;
            this.comboBoxCOMPorts.SelectedIndexChanged += new System.EventHandler(this.comboBoxCOMPorts_SelectedIndexChanged);

            // 
            // connectDisconnectSerialButton
            // 
            this.connectDisconnectSerialButton.Location = new System.Drawing.Point(180, 150);
            this.connectDisconnectSerialButton.Name = "connectDisconnectSerialButton";
            this.connectDisconnectSerialButton.Size = new System.Drawing.Size(100, 23);
            this.connectDisconnectSerialButton.TabIndex = 7;
            this.connectDisconnectSerialButton.Text = "Connect";
            this.connectDisconnectSerialButton.UseVisualStyleBackColor = true;
            this.connectDisconnectSerialButton.Click += new System.EventHandler(this.connectDisconnectSerialButton_Click);

            // 
            // transmitButton
            // 
            this.transmitButton.Location = new System.Drawing.Point(50, 120);
            this.transmitButton.Name = "transmitButton";
            this.transmitButton.Size = new System.Drawing.Size(290, 23);
            this.transmitButton.TabIndex = 8;
            this.transmitButton.Text = "Transmit Manual Values";
            this.transmitButton.UseVisualStyleBackColor = true;
            this.transmitButton.Click += new System.EventHandler(this.transmitButton_Click);

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 250);
            
            this.Controls.Add(this.labelStart);
            this.Controls.Add(this.labelInstruction);
            this.Controls.Add(this.labelMSB);
            this.Controls.Add(this.labelLSB);
            this.Controls.Add(this.labelEscape);

            this.Controls.Add(this.transmitButton);
            this.Controls.Add(this.connectDisconnectSerialButton);
            this.Controls.Add(this.comboBoxCOMPorts);
            this.Controls.Add(this.escapeByteTextbox);
            this.Controls.Add(this.pwmLSBTextbox);
            this.Controls.Add(this.pwmMSBTextbox);
            this.Controls.Add(this.instructionByteTextbox);
            this.Controls.Add(this.startByteTextbox);
            this.Controls.Add(this.pwmDutyScrollbar);
            this.Name = "Form1";
            this.Text = "DC Motor Control";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}

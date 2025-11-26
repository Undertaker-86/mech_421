#nullable disable

namespace StepperController;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.portCombo = new System.Windows.Forms.ComboBox();
        this.refreshButton = new System.Windows.Forms.Button();
        this.connectButton = new System.Windows.Forms.Button();
        this.statusLabel = new System.Windows.Forms.Label();
        this.velocityTrackBar = new System.Windows.Forms.TrackBar();
        this.velocityLabel = new System.Windows.Forms.Label();
        this.velocityValueLabel = new System.Windows.Forms.Label();
        this.stepCwButton = new System.Windows.Forms.Button();
        this.stepCcwButton = new System.Windows.Forms.Button();
        this.stopButton = new System.Windows.Forms.Button();
        this.logTextBox = new System.Windows.Forms.TextBox();
        this.portLabel = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)(this.velocityTrackBar)).BeginInit();
        this.SuspendLayout();
        // 
        // portCombo
        // 
        this.portCombo.FormattingEnabled = true;
        this.portCombo.Location = new System.Drawing.Point(74, 14);
        this.portCombo.Name = "portCombo";
        this.portCombo.Size = new System.Drawing.Size(110, 23);
        this.portCombo.TabIndex = 0;
        // 
        // refreshButton
        // 
        this.refreshButton.Location = new System.Drawing.Point(190, 12);
        this.refreshButton.Name = "refreshButton";
        this.refreshButton.Size = new System.Drawing.Size(75, 27);
        this.refreshButton.TabIndex = 1;
        this.refreshButton.Text = "Refresh";
        this.refreshButton.UseVisualStyleBackColor = true;
        this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
        // 
        // connectButton
        // 
        this.connectButton.Location = new System.Drawing.Point(271, 12);
        this.connectButton.Name = "connectButton";
        this.connectButton.Size = new System.Drawing.Size(80, 27);
        this.connectButton.TabIndex = 2;
        this.connectButton.Text = "Connect";
        this.connectButton.UseVisualStyleBackColor = true;
        this.connectButton.Click += new System.EventHandler(this.ConnectButton_Click);
        // 
        // statusLabel
        // 
        this.statusLabel.AutoSize = true;
        this.statusLabel.Location = new System.Drawing.Point(369, 17);
        this.statusLabel.Name = "statusLabel";
        this.statusLabel.Size = new System.Drawing.Size(80, 15);
        this.statusLabel.TabIndex = 3;
        this.statusLabel.Text = "Disconnected";
        // 
        // velocityTrackBar
        // 
        this.velocityTrackBar.LargeChange = 50;
        this.velocityTrackBar.Location = new System.Drawing.Point(23, 74);
        this.velocityTrackBar.Maximum = 1000;
        this.velocityTrackBar.Minimum = -1000;
        this.velocityTrackBar.Name = "velocityTrackBar";
        this.velocityTrackBar.Size = new System.Drawing.Size(449, 45);
        this.velocityTrackBar.TabIndex = 4;
        this.velocityTrackBar.TickFrequency = 100;
        this.velocityTrackBar.ValueChanged += new System.EventHandler(this.VelocityTrackBar_ValueChanged);
        // 
        // velocityLabel
        // 
        this.velocityLabel.AutoSize = true;
        this.velocityLabel.Location = new System.Drawing.Point(20, 56);
        this.velocityLabel.Name = "velocityLabel";
        this.velocityLabel.Size = new System.Drawing.Size(113, 15);
        this.velocityLabel.TabIndex = 5;
        this.velocityLabel.Text = "Velocity (steps/s)";
        // 
        // velocityValueLabel
        // 
        this.velocityValueLabel.AutoSize = true;
        this.velocityValueLabel.Location = new System.Drawing.Point(478, 85);
        this.velocityValueLabel.Name = "velocityValueLabel";
        this.velocityValueLabel.Size = new System.Drawing.Size(13, 15);
        this.velocityValueLabel.TabIndex = 6;
        this.velocityValueLabel.Text = "0";
        // 
        // stepCwButton
        // 
        this.stepCwButton.Location = new System.Drawing.Point(154, 126);
        this.stepCwButton.Name = "stepCwButton";
        this.stepCwButton.Size = new System.Drawing.Size(125, 30);
        this.stepCwButton.TabIndex = 7;
        this.stepCwButton.Text = "Single Step CW";
        this.stepCwButton.UseVisualStyleBackColor = true;
        this.stepCwButton.Click += new System.EventHandler(this.StepCwButton_Click);
        // 
        // stepCcwButton
        // 
        this.stepCcwButton.Location = new System.Drawing.Point(23, 126);
        this.stepCcwButton.Name = "stepCcwButton";
        this.stepCcwButton.Size = new System.Drawing.Size(125, 30);
        this.stepCcwButton.TabIndex = 8;
        this.stepCcwButton.Text = "Single Step CCW";
        this.stepCcwButton.UseVisualStyleBackColor = true;
        this.stepCcwButton.Click += new System.EventHandler(this.StepCcwButton_Click);
        // 
        // stopButton
        // 
        this.stopButton.Location = new System.Drawing.Point(478, 55);
        this.stopButton.Name = "stopButton";
        this.stopButton.Size = new System.Drawing.Size(69, 28);
        this.stopButton.TabIndex = 9;
        this.stopButton.Text = "Stop";
        this.stopButton.UseVisualStyleBackColor = true;
        this.stopButton.Click += new System.EventHandler(this.StopButton_Click);
        // 
        // logTextBox
        // 
        this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
        this.logTextBox.Location = new System.Drawing.Point(23, 175);
        this.logTextBox.Multiline = true;
        this.logTextBox.Name = "logTextBox";
        this.logTextBox.ReadOnly = true;
        this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.logTextBox.Size = new System.Drawing.Size(593, 174);
        this.logTextBox.TabIndex = 10;
        // 
        // portLabel
        // 
        this.portLabel.AutoSize = true;
        this.portLabel.Location = new System.Drawing.Point(20, 17);
        this.portLabel.Name = "portLabel";
        this.portLabel.Size = new System.Drawing.Size(38, 15);
        this.portLabel.TabIndex = 11;
        this.portLabel.Text = "COM:";
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(639, 368);
        this.Controls.Add(this.portLabel);
        this.Controls.Add(this.logTextBox);
        this.Controls.Add(this.stopButton);
        this.Controls.Add(this.stepCcwButton);
        this.Controls.Add(this.stepCwButton);
        this.Controls.Add(this.velocityValueLabel);
        this.Controls.Add(this.velocityLabel);
        this.Controls.Add(this.velocityTrackBar);
        this.Controls.Add(this.statusLabel);
        this.Controls.Add(this.connectButton);
        this.Controls.Add(this.refreshButton);
        this.Controls.Add(this.portCombo);
        this.Name = "Form1";
        this.Text = "Stepper Controller";
        ((System.ComponentModel.ISupportInitialize)(this.velocityTrackBar)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.ComboBox portCombo;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button connectButton;
    private System.Windows.Forms.Label statusLabel;
    private System.Windows.Forms.TrackBar velocityTrackBar;
    private System.Windows.Forms.Label velocityLabel;
    private System.Windows.Forms.Label velocityValueLabel;
    private System.Windows.Forms.Button stepCwButton;
    private System.Windows.Forms.Button stepCcwButton;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.TextBox logTextBox;
    private System.Windows.Forms.Label portLabel;
}

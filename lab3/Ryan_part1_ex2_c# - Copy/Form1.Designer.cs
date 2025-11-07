namespace Ryan_part1_ex2_c_
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.groupBoxConnection = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtCsvPath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkLogging = new System.Windows.Forms.CheckBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnRefreshPorts = new System.Windows.Forms.Button();
            this.comboPorts = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBoxCalibration = new System.Windows.Forms.GroupBox();
            this.btnApplyCalibration = new System.Windows.Forms.Button();
            this.txtA1 = new System.Windows.Forms.TextBox();
            this.txtA0 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxReadings = new System.Windows.Forms.GroupBox();
            this.txtResistance = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtVoltage = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtError = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtAdjusted = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtRaw = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBoxCapture = new System.Windows.Forms.GroupBox();
            this.btnStopCapture = new System.Windows.Forms.Button();
            this.btnStartCapture = new System.Windows.Forms.Button();
            this.comboCaptureScenario = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.chartTemp = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBoxResults = new System.Windows.Forms.GroupBox();
            this.listCaptures = new System.Windows.Forms.ListView();
            this.columnScenario = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDuration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnStart = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnFinal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTau63 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTauFit = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnR2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnCsv = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnNotes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBoxAnalysis = new System.Windows.Forms.GroupBox();
            this.txtAnalysis = new System.Windows.Forms.TextBox();
            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBoxConnection.SuspendLayout();
            this.groupBoxCalibration.SuspendLayout();
            this.groupBoxReadings.SuspendLayout();
            this.groupBoxCapture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTemp)).BeginInit();
            this.groupBoxResults.SuspendLayout();
            this.groupBoxAnalysis.SuspendLayout();
            this.groupBoxLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxConnection
            // 
            this.groupBoxConnection.Controls.Add(this.lblStatus);
            this.groupBoxConnection.Controls.Add(this.txtCsvPath);
            this.groupBoxConnection.Controls.Add(this.label6);
            this.groupBoxConnection.Controls.Add(this.chkLogging);
            this.groupBoxConnection.Controls.Add(this.btnConnect);
            this.groupBoxConnection.Controls.Add(this.btnRefreshPorts);
            this.groupBoxConnection.Controls.Add(this.comboPorts);
            this.groupBoxConnection.Controls.Add(this.label4);
            this.groupBoxConnection.Location = new System.Drawing.Point(12, 12);
            this.groupBoxConnection.Name = "groupBoxConnection";
            this.groupBoxConnection.Size = new System.Drawing.Size(410, 210);
            this.groupBoxConnection.TabIndex = 0;
            this.groupBoxConnection.TabStop = false;
            this.groupBoxConnection.Text = "Connection";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(17, 173);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(68, 25);
            this.lblStatus.TabIndex = 9;
            this.lblStatus.Text = "Idle...";
            // 
            // txtCsvPath
            // 
            this.txtCsvPath.Location = new System.Drawing.Point(128, 131);
            this.txtCsvPath.Name = "txtCsvPath";
            this.txtCsvPath.Size = new System.Drawing.Size(265, 31);
            this.txtCsvPath.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 134);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 25);
            this.label6.TabIndex = 7;
            this.label6.Text = "CSV file";
            // 
            // chkLogging
            // 
            this.chkLogging.AutoSize = true;
            this.chkLogging.Checked = true;
            this.chkLogging.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLogging.Location = new System.Drawing.Point(21, 99);
            this.chkLogging.Name = "chkLogging";
            this.chkLogging.Size = new System.Drawing.Size(177, 29);
            this.chkLogging.TabIndex = 6;
            this.chkLogging.Text = "Record to CSV";
            this.chkLogging.UseVisualStyleBackColor = true;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(240, 18);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(153, 38);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            // 
            // btnRefreshPorts
            // 
            this.btnRefreshPorts.Location = new System.Drawing.Point(21, 59);
            this.btnRefreshPorts.Name = "btnRefreshPorts";
            this.btnRefreshPorts.Size = new System.Drawing.Size(174, 38);
            this.btnRefreshPorts.TabIndex = 2;
            this.btnRefreshPorts.Text = "Refresh Ports";
            this.btnRefreshPorts.UseVisualStyleBackColor = true;
            // 
            // comboPorts
            // 
            this.comboPorts.FormattingEnabled = true;
            this.comboPorts.Location = new System.Drawing.Point(112, 18);
            this.comboPorts.Name = "comboPorts";
            this.comboPorts.Size = new System.Drawing.Size(122, 33);
            this.comboPorts.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 25);
            this.label4.TabIndex = 0;
            this.label4.Text = "COM #";
            // 
            // groupBoxCalibration
            // 
            this.groupBoxCalibration.Controls.Add(this.btnApplyCalibration);
            this.groupBoxCalibration.Controls.Add(this.txtA1);
            this.groupBoxCalibration.Controls.Add(this.txtA0);
            this.groupBoxCalibration.Controls.Add(this.label3);
            this.groupBoxCalibration.Controls.Add(this.label2);
            this.groupBoxCalibration.Location = new System.Drawing.Point(12, 228);
            this.groupBoxCalibration.Name = "groupBoxCalibration";
            this.groupBoxCalibration.Size = new System.Drawing.Size(410, 138);
            this.groupBoxCalibration.TabIndex = 1;
            this.groupBoxCalibration.TabStop = false;
            this.groupBoxCalibration.Text = "Calibration (Exercise 1)";
            // 
            // btnApplyCalibration
            // 
            this.btnApplyCalibration.Location = new System.Drawing.Point(274, 81);
            this.btnApplyCalibration.Name = "btnApplyCalibration";
            this.btnApplyCalibration.Size = new System.Drawing.Size(119, 38);
            this.btnApplyCalibration.TabIndex = 4;
            this.btnApplyCalibration.Text = "Apply";
            this.btnApplyCalibration.UseVisualStyleBackColor = true;
            // 
            // txtA1
            // 
            this.txtA1.Location = new System.Drawing.Point(88, 84);
            this.txtA1.Name = "txtA1";
            this.txtA1.Size = new System.Drawing.Size(168, 31);
            this.txtA1.TabIndex = 3;
            this.txtA1.Text = "1.0";
            // 
            // txtA0
            // 
            this.txtA0.Location = new System.Drawing.Point(88, 40);
            this.txtA0.Name = "txtA0";
            this.txtA0.Size = new System.Drawing.Size(168, 31);
            this.txtA0.TabIndex = 2;
            this.txtA0.Text = "0.0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 25);
            this.label3.TabIndex = 1;
            this.label3.Text = "a1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 25);
            this.label2.TabIndex = 0;
            this.label2.Text = "a0";
            // 
            // groupBoxReadings
            // 
            this.groupBoxReadings.Controls.Add(this.txtResistance);
            this.groupBoxReadings.Controls.Add(this.label11);
            this.groupBoxReadings.Controls.Add(this.txtVoltage);
            this.groupBoxReadings.Controls.Add(this.label10);
            this.groupBoxReadings.Controls.Add(this.txtError);
            this.groupBoxReadings.Controls.Add(this.label9);
            this.groupBoxReadings.Controls.Add(this.txtAdjusted);
            this.groupBoxReadings.Controls.Add(this.label8);
            this.groupBoxReadings.Controls.Add(this.txtRaw);
            this.groupBoxReadings.Controls.Add(this.label7);
            this.groupBoxReadings.Location = new System.Drawing.Point(12, 372);
            this.groupBoxReadings.Name = "groupBoxReadings";
            this.groupBoxReadings.Size = new System.Drawing.Size(410, 204);
            this.groupBoxReadings.TabIndex = 2;
            this.groupBoxReadings.TabStop = false;
            this.groupBoxReadings.Text = "Latest Reading";
            // 
            // txtResistance
            // 
            this.txtResistance.Location = new System.Drawing.Point(144, 159);
            this.txtResistance.Name = "txtResistance";
            this.txtResistance.Size = new System.Drawing.Size(249, 31);
            this.txtResistance.TabIndex = 9;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(21, 162);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(117, 25);
            this.label11.TabIndex = 8;
            this.label11.Text = "R NTC (Ω)";
            // 
            // txtVoltage
            // 
            this.txtVoltage.Location = new System.Drawing.Point(144, 122);
            this.txtVoltage.Name = "txtVoltage";
            this.txtVoltage.Size = new System.Drawing.Size(249, 31);
            this.txtVoltage.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(21, 125);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(113, 25);
            this.label10.TabIndex = 6;
            this.label10.Text = "Vout (V)";
            // 
            // txtError
            // 
            this.txtError.Location = new System.Drawing.Point(144, 85);
            this.txtError.Name = "txtError";
            this.txtError.Size = new System.Drawing.Size(249, 31);
            this.txtError.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(21, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(117, 25);
            this.label9.TabIndex = 4;
            this.label9.Text = "Error (degC)";
            // 
            // txtAdjusted
            // 
            this.txtAdjusted.Location = new System.Drawing.Point(144, 48);
            this.txtAdjusted.Name = "txtAdjusted";
            this.txtAdjusted.Size = new System.Drawing.Size(249, 31);
            this.txtAdjusted.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 51);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 25);
            this.label8.TabIndex = 2;
            this.label8.Text = "Adj degC";
            // 
            // txtRaw
            // 
            this.txtRaw.Location = new System.Drawing.Point(144, 21);
            this.txtRaw.Name = "txtRaw";
            this.txtRaw.Size = new System.Drawing.Size(249, 31);
            this.txtRaw.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 25);
            this.label7.TabIndex = 0;
            this.label7.Text = "Raw degC";
            // 
            // groupBoxCapture
            // 
            this.groupBoxCapture.Controls.Add(this.btnStopCapture);
            this.groupBoxCapture.Controls.Add(this.btnStartCapture);
            this.groupBoxCapture.Controls.Add(this.comboCaptureScenario);
            this.groupBoxCapture.Controls.Add(this.label12);
            this.groupBoxCapture.Location = new System.Drawing.Point(12, 582);
            this.groupBoxCapture.Name = "groupBoxCapture";
            this.groupBoxCapture.Size = new System.Drawing.Size(410, 141);
            this.groupBoxCapture.TabIndex = 3;
            this.groupBoxCapture.TabStop = false;
            this.groupBoxCapture.Text = "Capture";
            // 
            // btnStopCapture
            // 
            this.btnStopCapture.Enabled = false;
            this.btnStopCapture.Location = new System.Drawing.Point(214, 85);
            this.btnStopCapture.Name = "btnStopCapture";
            this.btnStopCapture.Size = new System.Drawing.Size(179, 38);
            this.btnStopCapture.TabIndex = 3;
            this.btnStopCapture.Text = "Stop Capture";
            this.btnStopCapture.UseVisualStyleBackColor = true;
            // 
            // btnStartCapture
            // 
            this.btnStartCapture.Location = new System.Drawing.Point(21, 85);
            this.btnStartCapture.Name = "btnStartCapture";
            this.btnStartCapture.Size = new System.Drawing.Size(179, 38);
            this.btnStartCapture.TabIndex = 2;
            this.btnStartCapture.Text = "Start Capture";
            this.btnStartCapture.UseVisualStyleBackColor = true;
            // 
            // comboCaptureScenario
            // 
            this.comboCaptureScenario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCaptureScenario.FormattingEnabled = true;
            this.comboCaptureScenario.Items.AddRange(new object[] {
            "0 to 40 degC (heating)",
            "40 to 0 degC (cooling)",
            "0 to 60 degC (heating)",
            "60 to 0 degC (cooling)",
            "Custom"});
            this.comboCaptureScenario.Location = new System.Drawing.Point(138, 38);
            this.comboCaptureScenario.Name = "comboCaptureScenario";
            this.comboCaptureScenario.Size = new System.Drawing.Size(255, 33);
            this.comboCaptureScenario.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 41);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(98, 25);
            this.label12.TabIndex = 0;
            this.label12.Text = "Scenario";
            // 
            // chartTemp
            // 
            chartArea1.AxisX.LabelStyle.Format = "HH:mm:ss";
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.Title = "Time";
            chartArea1.AxisY.Title = "Temperature (degC)";
            chartArea1.Name = "ChartArea1";
            this.chartTemp.ChartAreas.Add(chartArea1);
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend1.Name = "Legend1";
            this.chartTemp.Legends.Add(legend1);
            this.chartTemp.Location = new System.Drawing.Point(436, 12);
            this.chartTemp.Name = "chartTemp";
            this.chartTemp.Size = new System.Drawing.Size(752, 368);
            this.chartTemp.TabIndex = 4;
            this.chartTemp.Text = "chart1";
            // 
            // groupBoxResults
            // 
            this.groupBoxResults.Controls.Add(this.listCaptures);
            this.groupBoxResults.Location = new System.Drawing.Point(436, 386);
            this.groupBoxResults.Name = "groupBoxResults";
            this.groupBoxResults.Size = new System.Drawing.Size(752, 222);
            this.groupBoxResults.TabIndex = 5;
            this.groupBoxResults.TabStop = false;
            this.groupBoxResults.Text = "Capture Results";
            // 
            // listCaptures
            // 
            this.listCaptures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnScenario,
            this.columnDuration,
            this.columnStart,
            this.columnFinal,
            this.columnTau63,
            this.columnTauFit,
            this.columnR2,
            this.columnCsv,
            this.columnNotes});
            this.listCaptures.FullRowSelect = true;
            this.listCaptures.HideSelection = false;
            this.listCaptures.Location = new System.Drawing.Point(15, 30);
            this.listCaptures.MultiSelect = false;
            this.listCaptures.Name = "listCaptures";
            this.listCaptures.Size = new System.Drawing.Size(723, 173);
            this.listCaptures.TabIndex = 0;
            this.listCaptures.UseCompatibleStateImageBehavior = false;
            this.listCaptures.View = System.Windows.Forms.View.Details;
            // 
            // columnScenario
            // 
            this.columnScenario.Text = "Scenario";
            this.columnScenario.Width = 160;
            // 
            // columnDuration
            // 
            this.columnDuration.Text = "Duration (s)";
            this.columnDuration.Width = 100;
            // 
            // columnStart
            // 
            this.columnStart.Text = "Start degC";
            this.columnStart.Width = 80;
            // 
            // columnFinal
            // 
            this.columnFinal.Text = "Final degC";
            this.columnFinal.Width = 80;
            // 
            // columnTau63
            // 
            this.columnTau63.Text = "Tau 63% (s)";
            this.columnTau63.Width = 110;
            // 
            // columnTauFit
            // 
            this.columnTauFit.Text = "Tau Fit (s)";
            this.columnTauFit.Width = 110;
            // 
            // columnR2
            // 
            this.columnR2.Text = "R2";
            this.columnR2.Width = 60;
            // 
            // columnNotes
            // 
            this.columnCsv.Text = "CSV";
            this.columnCsv.Width = 160;
            // 
            // columnNotes
            // 
            this.columnNotes.Text = "Notes";
            this.columnNotes.Width = 180;
            // 
            // groupBoxAnalysis
            // 
            this.groupBoxAnalysis.Controls.Add(this.txtAnalysis);
            this.groupBoxAnalysis.Location = new System.Drawing.Point(436, 614);
            this.groupBoxAnalysis.Name = "groupBoxAnalysis";
            this.groupBoxAnalysis.Size = new System.Drawing.Size(752, 166);
            this.groupBoxAnalysis.TabIndex = 6;
            this.groupBoxAnalysis.TabStop = false;
            this.groupBoxAnalysis.Text = "Latest Analysis";
            // 
            // txtAnalysis
            // 
            this.txtAnalysis.Location = new System.Drawing.Point(15, 30);
            this.txtAnalysis.Multiline = true;
            this.txtAnalysis.Name = "txtAnalysis";
            this.txtAnalysis.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAnalysis.Size = new System.Drawing.Size(723, 120);
            this.txtAnalysis.TabIndex = 0;
            // 
            // groupBoxLog
            // 
            this.groupBoxLog.Controls.Add(this.txtLog);
            this.groupBoxLog.Location = new System.Drawing.Point(12, 729);
            this.groupBoxLog.Name = "groupBoxLog";
            this.groupBoxLog.Size = new System.Drawing.Size(410, 167);
            this.groupBoxLog.TabIndex = 7;
            this.groupBoxLog.TabStop = false;
            this.groupBoxLog.Text = "Activity Log";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(15, 29);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(379, 121);
            this.txtLog.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 910);
            this.Controls.Add(this.groupBoxLog);
            this.Controls.Add(this.groupBoxAnalysis);
            this.Controls.Add(this.groupBoxResults);
            this.Controls.Add(this.chartTemp);
            this.Controls.Add(this.groupBoxCapture);
            this.Controls.Add(this.groupBoxReadings);
            this.Controls.Add(this.groupBoxCalibration);
            this.Controls.Add(this.groupBoxConnection);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thermistor Monitor";
            this.groupBoxConnection.ResumeLayout(false);
            this.groupBoxConnection.PerformLayout();
            this.groupBoxCalibration.ResumeLayout(false);
            this.groupBoxCalibration.PerformLayout();
            this.groupBoxReadings.ResumeLayout(false);
            this.groupBoxReadings.PerformLayout();
            this.groupBoxCapture.ResumeLayout(false);
            this.groupBoxCapture.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTemp)).EndInit();
            this.groupBoxResults.ResumeLayout(false);
            this.groupBoxAnalysis.ResumeLayout(false);
            this.groupBoxAnalysis.PerformLayout();
            this.groupBoxLog.ResumeLayout(false);
            this.groupBoxLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxConnection;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtCsvPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkLogging;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnRefreshPorts;
        private System.Windows.Forms.ComboBox comboPorts;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBoxCalibration;
        private System.Windows.Forms.Button btnApplyCalibration;
        private System.Windows.Forms.TextBox txtA1;
        private System.Windows.Forms.TextBox txtA0;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBoxReadings;
        private System.Windows.Forms.TextBox txtResistance;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtVoltage;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtError;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtAdjusted;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtRaw;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBoxCapture;
        private System.Windows.Forms.Button btnStopCapture;
        private System.Windows.Forms.Button btnStartCapture;
        private System.Windows.Forms.ComboBox comboCaptureScenario;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTemp;
        private System.Windows.Forms.GroupBox groupBoxResults;
        private System.Windows.Forms.ListView listCaptures;
        private System.Windows.Forms.ColumnHeader columnScenario;
        private System.Windows.Forms.ColumnHeader columnDuration;
        private System.Windows.Forms.ColumnHeader columnStart;
        private System.Windows.Forms.ColumnHeader columnFinal;
        private System.Windows.Forms.ColumnHeader columnTau63;
        private System.Windows.Forms.ColumnHeader columnTauFit;
        private System.Windows.Forms.ColumnHeader columnR2;
        private System.Windows.Forms.ColumnHeader columnCsv;
        private System.Windows.Forms.ColumnHeader columnNotes;
        private System.Windows.Forms.GroupBox groupBoxAnalysis;
        private System.Windows.Forms.TextBox txtAnalysis;
        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.TextBox txtLog;
    }
}

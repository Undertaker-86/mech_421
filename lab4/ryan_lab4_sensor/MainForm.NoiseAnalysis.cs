using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RyanSensorApp.Models;
using RyanSensorApp.Services;
using ScottPlot.WinForms;

namespace RyanSensorApp
{
    public partial class MainForm
    {
        private void CreateNoiseAnalysisTab(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(240, 240, 245);

            // Create three-panel layout
            noiseLeftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 350,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            noiseRightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 400,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            noiseCenterPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 240, 245),
                Padding = new Padding(15)
            };

            CreateNoiseAnalysisLeftPanel();
            CreateNoiseAnalysisCenterPanel();
            CreateNoiseAnalysisRightPanel();

            tab.Controls.Add(noiseCenterPanel);
            tab.Controls.Add(noiseRightPanel);
            tab.Controls.Add(noiseLeftPanel);
        }

        private void CreateNoiseAnalysisLeftPanel()
        {
            int yPos = 10;

            // Test Setup Group
            noiseTestSetupGroup = new GroupBox
            {
                Text = "TEST CONFIGURATION",
                Location = new Point(10, yPos),
                Size = new Size(320, 150),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var lblPosition = new Label
            {
                Text = "Test Position:",
                Location = new Point(15, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            cmbTestPosition = new ComboBox
            {
                Location = new Point(15, 55),
                Width = 290,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTestPosition.Items.AddRange(new object[] { 
                "Middle Range", 
                "Near Extreme (Close)", 
                "Far Extreme (Far)",
                "Custom Position"
            });
            cmbTestPosition.SelectedIndex = 0;

            var lblDuration = new Label
            {
                Text = "Test Duration (seconds):",
                Location = new Point(15, 90),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            numTestDuration = new NumericUpDown
            {
                Location = new Point(15, 115),
                Width = 120,
                Minimum = 1,
                Maximum = 60,
                Value = 10,
                Font = new Font("Segoe UI", 11)
            };

            var lblNote = new Label
            {
                Text = "Lab standard: 10s",
                Location = new Point(145, 118),
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };

            noiseTestSetupGroup.Controls.AddRange(new Control[] {
                lblPosition, cmbTestPosition, lblDuration, numTestDuration, lblNote
            });

            yPos += 160;

            // Test Control Group
            noiseTestControlGroup = new GroupBox
            {
                Text = "TEST CONTROL",
                Location = new Point(10, yPos),
                Size = new Size(320, 200),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            lblTestStatus = new Label
            {
                Text = "● Ready",
                Location = new Point(15, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Gray
            };

            lblTestTimer = new Label
            {
                Text = "0.0 s",
                Location = new Point(15, 60),
                Size = new Size(290, 30),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 200),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pbTestProgress = new ProgressBar
            {
                Location = new Point(15, 100),
                Size = new Size(290, 25),
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            btnStartTest = new Button
            {
                Text = "▶ START TEST",
                Location = new Point(15, 135),
                Size = new Size(290, 50),
                BackColor = Color.FromArgb(0, 150, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnStartTest.Click += BtnStartNoiseTest_Click;

            btnStopTest = new Button
            {
                Text = "■ STOP TEST",
                Location = new Point(15, 135),
                Size = new Size(290, 50),
                BackColor = Color.FromArgb(200, 0, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Visible = false
            };
            btnStopTest.Click += BtnStopNoiseTest_Click;

            noiseTestControlGroup.Controls.AddRange(new Control[] {
                lblTestStatus, lblTestTimer, pbTestProgress, btnStartTest, btnStopTest
            });

            yPos += 210;

            // Test Results Group
            noiseTestResultsGroup = new GroupBox
            {
                Text = "CURRENT TEST RESULTS",
                Location = new Point(10, yPos),
                Size = new Size(320, 240),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var lblLiveSamples = new Label
            {
                Text = "Samples:",
                Location = new Point(15, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            lblCurrentSamples = new Label
            {
                Text = "0",
                Location = new Point(100, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var lblLiveMean = new Label
            {
                Text = "Mean Position:",
                Location = new Point(15, 55),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            lblCurrentMean = new Label
            {
                Text = "--- cm",
                Location = new Point(120, 55),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var lblLiveStdDev = new Label
            {
                Text = "Std Dev (RMS):",
                Location = new Point(15, 80),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            lblCurrentStdDev = new Label
            {
                Text = "--- cm",
                Location = new Point(120, 80),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 0, 0)
            };

            var separator = new Label
            {
                Text = "Final Test Results:",
                Location = new Point(15, 115),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 150)
            };

            lblTestMean = new Label
            {
                Text = "Mean: ---",
                Location = new Point(15, 140),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            lblTestStdDev = new Label
            {
                Text = "RMS Noise: ---",
                Location = new Point(15, 165),
                Size = new Size(290, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 0, 0)
            };

            lblTestRange = new Label
            {
                Text = "Range: ---",
                Location = new Point(15, 190),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            lblTestSamples = new Label
            {
                Text = "Samples: ---",
                Location = new Point(15, 210),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            noiseTestResultsGroup.Controls.AddRange(new Control[] {
                lblLiveSamples, lblCurrentSamples, lblLiveMean, lblCurrentMean, 
                lblLiveStdDev, lblCurrentStdDev, separator,
                lblTestMean, lblTestStdDev, lblTestRange, lblTestSamples
            });

            noiseLeftPanel.Controls.AddRange(new Control[] {
                noiseTestSetupGroup, noiseTestControlGroup, noiseTestResultsGroup
            });
        }

        private void CreateNoiseAnalysisCenterPanel()
        {
            int yPos = 10;

            // Visualization Group
            noiseVisualizationGroup = new GroupBox
            {
                Text = "REAL-TIME NOISE VISUALIZATION",
                Location = new Point(10, yPos),
                Size = new Size(600, 800),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var lblInfo = new Label
            {
                Text = "Position readings over test duration with statistical bands",
                Location = new Point(15, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };

            plotNoiseTest = new FormsPlot
            {
                Location = new Point(15, 50),
                Size = new Size(570, 735),
                BackColor = Color.White
            };

            noiseVisualizationGroup.Controls.AddRange(new Control[] {
                lblInfo, plotNoiseTest
            });

            noiseCenterPanel.Controls.Add(noiseVisualizationGroup);
        }

        private void CreateNoiseAnalysisRightPanel()
        {
            int yPos = 10;

            // History Group
            noiseHistoryGroup = new GroupBox
            {
                Text = "TEST HISTORY",
                Location = new Point(10, yPos),
                Size = new Size(370, 350),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            dgvNoiseHistory = new DataGridView
            {
                Location = new Point(15, 30),
                Size = new Size(340, 280),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };

            dgvNoiseHistory.Columns.Add("Test", "Test#");
            dgvNoiseHistory.Columns.Add("Position", "Position");
            dgvNoiseHistory.Columns.Add("Mean", "Mean(cm)");
            dgvNoiseHistory.Columns.Add("StdDev", "RMS(cm)");
            dgvNoiseHistory.Columns.Add("Samples", "N");
            dgvNoiseHistory.Columns["Test"].FillWeight = 15;
            dgvNoiseHistory.Columns["Position"].FillWeight = 30;
            dgvNoiseHistory.Columns["Mean"].FillWeight = 20;
            dgvNoiseHistory.Columns["StdDev"].FillWeight = 20;
            dgvNoiseHistory.Columns["Samples"].FillWeight = 15;

            btnClearNoiseTests = new Button
            {
                Text = "Clear All Tests",
                Location = new Point(15, 315),
                Size = new Size(165, 25),
                BackColor = Color.FromArgb(150, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnClearNoiseTests.Click += BtnClearNoiseTests_Click;

            btnExportNoiseTests = new Button
            {
                Text = "Export to CSV",
                Location = new Point(190, 315),
                Size = new Size(165, 25),
                BackColor = Color.FromArgb(50, 100, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnExportNoiseTests.Click += BtnExportNoiseTests_Click;

            noiseHistoryGroup.Controls.AddRange(new Control[] {
                dgvNoiseHistory, btnClearNoiseTests, btnExportNoiseTests
            });

            yPos += 360;

            // Comparison Group
            noiseComparisonGroup = new GroupBox
            {
                Text = "COMPARISON & ANALYSIS",
                Location = new Point(10, yPos),
                Size = new Size(370, 430),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var lblCompInfo = new Label
            {
                Text = "RMS Noise Comparison:",
                Location = new Point(15, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            plotNoiseComparison = new FormsPlot
            {
                Location = new Point(15, 50),
                Size = new Size(340, 200),
                BackColor = Color.White
            };

            var lblSummary = new Label
            {
                Text = "Statistical Summary:",
                Location = new Point(15, 260),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            txtComparison = new TextBox
            {
                Location = new Point(15, 285),
                Size = new Size(340, 135),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Consolas", 8),
                BackColor = Color.FromArgb(250, 250, 250)
            };

            noiseComparisonGroup.Controls.AddRange(new Control[] {
                lblCompInfo, plotNoiseComparison, lblSummary, txtComparison
            });

            noiseRightPanel.Controls.AddRange(new Control[] {
                noiseHistoryGroup, noiseComparisonGroup
            });
        }

        private void InitializeNoiseAnalysisService()
        {
            _noiseAnalysisService = new NoiseAnalysisService();

            // Initialize noise test timer
            _noiseTestTimer = new System.Windows.Forms.Timer();
            _noiseTestTimer.Interval = 100;  // Update every 100ms
            _noiseTestTimer.Tick += NoiseTestTimer_Tick;

            // Initialize noise plot
            plotNoiseTest.Plot.Title("");
            plotNoiseTest.Plot.XLabel("Time (s)");
            plotNoiseTest.Plot.YLabel("Distance (cm)");

            // Initialize comparison plot
            plotNoiseComparison.Plot.Title("");
            plotNoiseComparison.Plot.XLabel("Test Position");
            plotNoiseComparison.Plot.YLabel("RMS Noise (cm)");
            
            UpdateNoiseComparisonPlot();
        }

        private void BtnStartNoiseTest_Click(object? sender, EventArgs e)
        {
            if (!_serialPort.IsConnected)
            {
                MessageBox.Show("Please connect to the sensor first!", "Not Connected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_currentCalibration == null)
            {
                MessageBox.Show("Please load a calibration first!", "No Calibration", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get test position
            TestPosition position = cmbTestPosition.SelectedIndex switch
            {
                0 => TestPosition.MiddleRange,
                1 => TestPosition.NearExtreme,
                2 => TestPosition.FarExtreme,
                3 => TestPosition.Custom,
                _ => TestPosition.MiddleRange
            };

            // Create new test
            _currentNoiseTest = _noiseAnalysisService.CreateNewTest(position);
            _currentNoiseTest.DurationSeconds = (double)numTestDuration.Value;
            _noiseTestStartTime = DateTime.Now;
            _isNoiseTestRunning = true;

            // Update UI
            btnStartTest.Visible = false;
            btnStopTest.Visible = true;
            cmbTestPosition.Enabled = false;
            numTestDuration.Enabled = false;
            lblTestStatus.Text = "● RECORDING...";
            lblTestStatus.ForeColor = Color.Red;
            lblTestTimer.Text = "0.0 s";
            pbTestProgress.Value = 0;

            // Clear current results
            lblCurrentSamples.Text = "0";
            lblCurrentMean.Text = "--- cm";
            lblCurrentStdDev.Text = "--- cm";

            // Start timer
            _noiseTestTimer.Start();
        }

        private void BtnStopNoiseTest_Click(object? sender, EventArgs e)
        {
            StopNoiseTest();
        }

        private void StopNoiseTest()
        {
            if (!_isNoiseTestRunning || _currentNoiseTest == null)
                return;

            _isNoiseTestRunning = false;
            _noiseTestTimer.Stop();

            // Calculate final statistics
            _currentNoiseTest.CalculateStatistics();

            // Save test
            _noiseAnalysisService.SaveTest(_currentNoiseTest);

            // Update UI
            lblTestStatus.Text = "● Test Complete";
            lblTestStatus.ForeColor = Color.Green;
            btnStartTest.Visible = true;
            btnStopTest.Visible = false;
            cmbTestPosition.Enabled = true;
            numTestDuration.Enabled = true;

            // Display final results
            lblTestMean.Text = $"Mean: {_currentNoiseTest.MeanDistance:F4} cm";
            lblTestStdDev.Text = $"RMS Noise: {_currentNoiseTest.StandardDeviation:F4} cm";
            lblTestRange.Text = $"Range: {_currentNoiseTest.MinDistance:F4} - {_currentNoiseTest.MaxDistance:F4} cm";
            lblTestSamples.Text = $"Samples: {_currentNoiseTest.SampleCount}";

            // Update test history
            UpdateNoiseHistoryTable();
            UpdateNoiseComparisonPlot();
            UpdateComparisonSummary();

            MessageBox.Show(_currentNoiseTest.GetSummary(), "Test Complete", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            _currentNoiseTest = null;
        }

        private void NoiseTestTimer_Tick(object? sender, EventArgs e)
        {
            if (!_isNoiseTestRunning || _currentNoiseTest == null)
                return;

            // Calculate elapsed time
            double elapsedSeconds = (DateTime.Now - _noiseTestStartTime).TotalSeconds;
            lblTestTimer.Text = $"{elapsedSeconds:F1} s";

            // Update progress bar
            int progress = (int)((elapsedSeconds / _currentNoiseTest.DurationSeconds) * 100);
            pbTestProgress.Value = Math.Min(progress, 100);

            // Add current reading to test
            if (_currentCalibration != null)
            {
                double distance = _currentCalibration.ConvertAdcToDistance(_currentAdcValue);
                _currentNoiseTest.DistanceReadings.Add(distance);
                _currentNoiseTest.AdcReadings.Add(_currentAdcValue);

                // Calculate running statistics
                if (_currentNoiseTest.DistanceReadings.Count > 1)
                {
                    double mean = _currentNoiseTest.DistanceReadings.Average();
                    double sumSqDiff = _currentNoiseTest.DistanceReadings.Sum(x => Math.Pow(x - mean, 2));
                    double stdDev = Math.Sqrt(sumSqDiff / _currentNoiseTest.DistanceReadings.Count);

                    lblCurrentSamples.Text = _currentNoiseTest.DistanceReadings.Count.ToString();
                    lblCurrentMean.Text = $"{mean:F4} cm";
                    lblCurrentStdDev.Text = $"{stdDev:F4} cm";
                }

                // Update plot
                UpdateNoiseTestPlot();
            }

            // Check if test duration reached
            if (elapsedSeconds >= _currentNoiseTest.DurationSeconds)
            {
                StopNoiseTest();
            }
        }

        private void UpdateNoiseTestPlot()
        {
            if (_currentNoiseTest == null || _currentNoiseTest.DistanceReadings.Count == 0)
                return;

            plotNoiseTest.Plot.Clear();

            // Create time array
            double[] times = Enumerable.Range(0, _currentNoiseTest.DistanceReadings.Count)
                .Select(i => i * 0.1)  // 100ms intervals
                .ToArray();
            double[] distances = _currentNoiseTest.DistanceReadings.ToArray();

            // Plot data
            var scatter = plotNoiseTest.Plot.Add.Scatter(times, distances);
            scatter.Color = ScottPlot.Color.FromHex("#0064C8");
            scatter.LineWidth = 2;
            scatter.MarkerSize = 3;
            scatter.LegendText = "Position Readings";

            // Add mean line if we have enough data
            if (_currentNoiseTest.DistanceReadings.Count > 2)
            {
                double mean = distances.Average();
                double stdDev = Math.Sqrt(distances.Sum(x => Math.Pow(x - mean, 2)) / distances.Length);

                var meanLine = plotNoiseTest.Plot.Add.HorizontalLine(mean);
                meanLine.Color = ScottPlot.Color.FromHex("#00AA00");
                meanLine.LineWidth = 2;
                meanLine.LinePattern = ScottPlot.LinePattern.Dashed;
                meanLine.LegendText = "Mean";

                // Add ±1σ band
                var plusSigma = plotNoiseTest.Plot.Add.HorizontalLine(mean + stdDev);
                plusSigma.Color = ScottPlot.Color.FromHex("#FF6600");
                plusSigma.LineWidth = 1;
                plusSigma.LinePattern = ScottPlot.LinePattern.Dotted;
                plusSigma.LegendText = "±1σ";

                var minusSigma = plotNoiseTest.Plot.Add.HorizontalLine(mean - stdDev);
                minusSigma.Color = ScottPlot.Color.FromHex("#FF6600");
                minusSigma.LineWidth = 1;
                minusSigma.LinePattern = ScottPlot.LinePattern.Dotted;
            }

            plotNoiseTest.Plot.Legend.IsVisible = true;
            plotNoiseTest.Plot.Axes.AutoScale();
            plotNoiseTest.Refresh();
        }

        private void UpdateNoiseHistoryTable()
        {
            dgvNoiseHistory.Rows.Clear();

            var tests = _noiseAnalysisService.GetAllTests();
            foreach (var test in tests)
            {
                dgvNoiseHistory.Rows.Add(
                    test.TestNumber.ToString(),
                    test.GetPositionString(),
                    test.MeanDistance.ToString("F4"),
                    test.StandardDeviation.ToString("F4"),
                    test.SampleCount.ToString()
                );
            }
        }

        private void UpdateNoiseComparisonPlot()
        {
            plotNoiseComparison.Plot.Clear();

            var avgRms = _noiseAnalysisService.GetAverageRMSByPosition();
            if (avgRms.Count == 0)
            {
                plotNoiseComparison.Refresh();
                return;
            }

            // Create bar chart data
            var positions = avgRms.Keys.ToList();
            var rmsValues = avgRms.Values.ToList();
            
            double[] posIndices = Enumerable.Range(0, positions.Count).Select(i => (double)i).ToArray();
            string[] posLabels = positions.Select(p => p switch
            {
                TestPosition.MiddleRange => "Middle",
                TestPosition.NearExtreme => "Near",
                TestPosition.FarExtreme => "Far",
                _ => "Custom"
            }).ToArray();

            var bar = plotNoiseComparison.Plot.Add.Bars(posIndices, rmsValues.ToArray());
            bar.Color = ScottPlot.Color.FromHex("#FF6600");

            plotNoiseComparison.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                posIndices, posLabels);
            plotNoiseComparison.Plot.Axes.AutoScale();
            plotNoiseComparison.Refresh();
        }

        private void UpdateComparisonSummary()
        {
            txtComparison.Text = _noiseAnalysisService.GetComparisonSummary();
        }

        private void BtnClearNoiseTests_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Clear all noise test data?",
                "Confirm Clear",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _noiseAnalysisService.ClearAllTests();
                UpdateNoiseHistoryTable();
                UpdateNoiseComparisonPlot();
                UpdateComparisonSummary();
                
                lblTestMean.Text = "Mean: ---";
                lblTestStdDev.Text = "RMS Noise: ---";
                lblTestRange.Text = "Range: ---";
                lblTestSamples.Text = "Samples: ---";
            }
        }

        private void BtnExportNoiseTests_Click(object? sender, EventArgs e)
        {
            if (_noiseAnalysisService.GetTestCount() == 0)
            {
                MessageBox.Show("No test data to export!", "No Data", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files (*.csv)|*.csv";
                sfd.FileName = $"noise_analysis_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _noiseAnalysisService.ExportToCSV(sfd.FileName);
                        MessageBox.Show($"Noise analysis exported successfully!\n{sfd.FileName}", 
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export failed: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

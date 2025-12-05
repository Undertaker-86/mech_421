using System;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace StepperControl
{
    public partial class Form1 : Form
    {
        private const byte StartByte = 255;
        private const int StepTimerClockHz = 1_000_000; // SMCLK/8 in firmware
        private readonly double _stepAngleDeg = 360.0 / StepperCommander.StepsPerRevolution;
        private StepperCommander commander;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            velocityTrackBar.Minimum = -100;
            velocityTrackBar.Maximum = 100;
            velocityTrackBar.TickFrequency = 10;
            velocityTrackBar.Value = 0;
            maxSpeedNumeric.Value = 1200; // steps per second at slider = 100
            PopulatePorts();
            UpdateVelocityLabels(0, 0, 0);
            UpdateModeAndFreq("Idle", 0);
            SetTelemetry(null, null);
            
            // Initialize Mode
            SwitchMode(true);
        }

        private void continuousModeBtn_Click(object sender, EventArgs e)
        {
            SwitchMode(true);
        }

        private void singleStepModeBtn_Click(object sender, EventArgs e)
        {
            SwitchMode(false);
            // Optional: Stop motor when switching to single step to prevent confusion
            StopMotor("Switching to Single Step Mode");
        }

        private void SwitchMode(bool continuous)
        {
            continuousPanel.Visible = continuous;
            singleStepPanel.Visible = !continuous;

            // Update button styles
            if (continuous)
            {
                continuousModeBtn.BackColor = Color.FromArgb(50, 55, 70);
                continuousModeBtn.ForeColor = Color.White;
                singleStepModeBtn.BackColor = Color.Transparent;
                singleStepModeBtn.ForeColor = Color.Gray;
            }
            else
            {
                singleStepModeBtn.BackColor = Color.FromArgb(50, 55, 70);
                singleStepModeBtn.ForeColor = Color.White;
                continuousModeBtn.BackColor = Color.Transparent;
                continuousModeBtn.ForeColor = Color.Gray;
            }
        }

        private void PopulatePorts()
        {
            portComboBox.Items.Clear();
            var ports = SerialPort.GetPortNames().OrderBy(p => p).ToArray();
            if (ports.Length == 0)
            {
                portComboBox.Text = "No COM ports found";
                connectButton.Enabled = false;
                statusLabel.Text = "Connect board before opening the port.";
            }
            else
            {
                portComboBox.Items.AddRange(ports);
                portComboBox.SelectedIndex = 0;
                connectButton.Enabled = true;
                statusLabel.Text = "Select a COM port and press Connect.";
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                commander = null;
                connectButton.Text = "Connect";
                statusLabel.Text = "Serial port closed.";
                return;
            }

            if (portComboBox.SelectedItem == null)
            {
                MessageBox.Show("Select a COM port first.");
                return;
            }

            serialPort.PortName = portComboBox.SelectedItem.ToString();
            serialPort.BaudRate = 9600;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;

            try
            {
                serialPort.Open();
                commander = new StepperCommander(serialPort);
                connectButton.Text = "Disconnect";
                statusLabel.Text = $"Connected to {serialPort.PortName} at {serialPort.BaudRate} baud.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open {serialPort.PortName}: {ex.Message}");
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            PopulatePorts();
        }

        private void ccwStepButton_Click(object sender, EventArgs e)
        {
            if (!EnsureCommander()) return;
            try
            {
                commander.MoveByAngle(-_stepAngleDeg);
                UpdateModeAndFreq("Single CCW step", 0);
                SetTelemetry(commander.CurrentAngleDeg, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to step CCW: {ex.Message}");
            }
        }

        private void cwStepButton_Click(object sender, EventArgs e)
        {
            if (!EnsureCommander()) return;
            try
            {
                commander.MoveByAngle(_stepAngleDeg);
                UpdateModeAndFreq("Single CW step", 0);
                SetTelemetry(commander.CurrentAngleDeg, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to step CW: {ex.Message}");
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopMotor("Stop (DirnByte 3 halts timer)");
        }

        private void velocityTrackBar_Scroll(object sender, EventArgs e)
        {
            HandleVelocityChange();
        }

        private void maxSpeedNumeric_ValueChanged(object sender, EventArgs e)
        {
            HandleVelocityChange();
        }

        private void HandleVelocityChange()
        {
            int sliderValue = velocityTrackBar.Value;
            double maxStepsPerSecond = (double)maxSpeedNumeric.Value;

            if (sliderValue == 0)
            {
                UpdateVelocityLabels(0, 0, 0);
                StopMotor("Slider at zero -> stop timer");
                UpdateModeAndFreq("Stopped", 0);
                SetTelemetry(commander?.CurrentAngleDeg, 0);
                return;
            }

            double fraction = Math.Abs(sliderValue) / 100.0;
            double commandedStepsPerSecond = Math.Max(1.0, fraction * maxStepsPerSecond);
            bool clockwise = sliderValue > 0;
            double rpm = commandedStepsPerSecond * 60.0 / StepperCommander.StepsPerRevolution;
            if (!clockwise) rpm *= -1;

            if (!EnsureCommander()) return;

            try
            {
                byte dir;
                ushort ccr0;
                byte[] packet = commander.BuildContinuousPacketFromRpm(rpm, out ccr0, out dir);
                commander.SendPacket(packet);

                UpdateVelocityLabels(sliderValue, commandedStepsPerSecond, ccr0);
                UpdateModeAndFreq($"Continuous {(clockwise ? "CW" : "CCW")}", ccr0);
                SetTelemetry(commander.CurrentAngleDeg, commandedStepsPerSecond);
                statusLabel.Text = $"Continuous {(clockwise ? "CW" : "CCW")} | {commandedStepsPerSecond:F1} steps/s | CCR0 {ccr0}";
                lastPacketLabel.Text = $"Last Packet [{string.Join(", ", packet.Select(b => b.ToString()))}]";
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Write failed.";
                MessageBox.Show($"Failed to send packet: {ex.Message}");
            }
        }

        private void StopMotor(string reason)
        {
            if (velocityTrackBar.Value != 0)
            {
                velocityTrackBar.Value = 0;
            }

            if (commander != null && serialPort.IsOpen)
            {
                byte[] packet = { StartByte, 3, 0, 0, 0 };
                try
                {
                    commander.SendPacket(packet);
                    lastPacketLabel.Text = $"Last Packet [{string.Join(", ", packet.Select(b => b.ToString()))}]";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to stop motor: {ex.Message}");
                }
            }

            UpdateVelocityLabels(0, 0, 0);
            UpdateModeAndFreq("Stopped", 0);
            SetTelemetry(commander?.CurrentAngleDeg, 0);
        }

        private void UpdateVelocityLabels(int sliderValue, double stepsPerSecond, ushort ccr0)
        {
            string direction = sliderValue > 0 ? "CW" : sliderValue < 0 ? "CCW" : "Stopped";
            velocitySummaryLabel.Text =
                $"Slider {sliderValue} -> {direction} | {stepsPerSecond:F1} steps/s | TA1CCR0 {ccr0}";
        }

        private void UpdateModeAndFreq(string modeText, ushort ccr0)
        {
            modeLabel.Text = $"Mode: {modeText}";
            double freq = ccr0 == 0 ? 0 : StepTimerClockHz / (ccr0 + 1.0);
            freqLabel.Text = $"Step freq: {freq:F1} Hz (TA1CCR0 {ccr0})";
        }

        private void SetTelemetry(double? positionDeg, double? velocityStepsPerSecond)
        {
            positionValueTextBox.Text = positionDeg.HasValue ? $"{NormalizeDeg(positionDeg.Value):F2} deg" : "N/A";

            if (velocityStepsPerSecond.HasValue)
            {
                double hz = velocityStepsPerSecond.Value;
                double rpm = hz * 60.0 / StepperCommander.StepsPerRevolution;
                velocityHzTextBox.Text = $"{hz:F1}";
                velocityRpmTextBox.Text = $"{rpm:F2}";
            }
            else
            {
                velocityHzTextBox.Text = "N/A";
                velocityRpmTextBox.Text = "N/A";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        private bool EnsureCommander()
        {
            if (commander == null || !serialPort.IsOpen)
            {
                statusLabel.Text = "Connect to the board first.";
                return false;
            }

            return true;
        }

        private static double NormalizeDeg(double deg)
        {
            double wrapped = deg % 360.0;
            if (wrapped < 0) wrapped += 360.0;
            return wrapped;
        }
    }
}

using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace GantryControl
{
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;
        // Calibration: Steps per CM
        // Assuming 200 steps/rev, maybe 0.5cm pitch lead screw? -> 400 steps/cm?
        // Let's assume 50 steps per cm for now as a placeholder.
        const double STEPS_PER_CM = 100.0; 

        public MainWindow()
        {
            InitializeComponent();
            LoadPorts();
        }

        private void LoadPorts()
        {
            PortSelector.ItemsSource = SerialPort.GetPortNames();
            if (PortSelector.Items.Count > 0) PortSelector.SelectedIndex = 0;
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                ConnectBtn.Content = "Connect";
                StatusText.Text = "Disconnected";
            }
            else
            {
                try
                {
                    _serialPort = new SerialPort(PortSelector.SelectedItem.ToString(), 9600, Parity.None, 8, StopBits.One);
                    _serialPort.Open();
                    ConnectBtn.Content = "Disconnect";
                    StatusText.Text = "Connected";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void SendPacket(int dxSteps, int dySteps, int velocity)
        {
            if (_serialPort == null || !_serialPort.IsOpen) return;

            // Packet Structure: [255][CMD][XH][XL][YH][YL][VEL][ESC]
            byte[] packet = new byte[8];
            packet[0] = 255;
            packet[1] = 1; // Move Command

            // Convert 16-bit signed to bytes
            byte xh = (byte)((dxSteps >> 8) & 0xFF);
            byte xl = (byte)(dxSteps & 0xFF);
            byte yh = (byte)((dySteps >> 8) & 0xFF);
            byte yl = (byte)(dySteps & 0xFF);
            byte vel = (byte)velocity;

            // Escape byte logic
            byte esc = 0;
            if (vel == 255) { esc |= 0x01; vel = 0; } // Note: Firmware logic needs to match this replacement
            if (yl == 255) { esc |= 0x02; yl = 0; }
            if (yh == 255) { esc |= 0x04; yh = 0; }
            if (xl == 255) { esc |= 0x08; xl = 0; }
            if (xh == 255) { esc |= 0x10; xh = 0; }

            packet[2] = xh;
            packet[3] = xl;
            packet[4] = yh;
            packet[5] = yl;
            packet[6] = vel;
            packet[7] = esc;

            _serialPort.Write(packet, 0, 8);
        }

        private void MoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(XInput.Text, out double xCm) && double.TryParse(YInput.Text, out double yCm))
            {
                int dx = (int)(xCm * STEPS_PER_CM);
                int dy = (int)(yCm * STEPS_PER_CM);
                int vel = (int)VelSlider.Value;
                
                SendPacket(dx, dy, vel);
                StatusText.Text = $"Sent Move: {xCm}cm, {yCm}cm @ {vel}%";
            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort == null || !_serialPort.IsOpen) return;
            byte[] packet = new byte[8];
            packet[0] = 255;
            packet[1] = 2; // Stop
            _serialPort.Write(packet, 0, 8);
            StatusText.Text = "Sent Stop";
        }

        private void DemoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                int id = int.Parse(tag);
                // Demo Locations from Table
                double x = 0, y = 0;
                int v = 50;

                switch (id)
                {
                    case 1: x = 5; y = 0; v = 100; break;
                    case 2: x = 0; y = 5; v = 50; break;
                    case 3: x = -5; y = -5; v = 20; break;
                    case 4: x = 7; y = 2; v = 60; break;
                    case 5: x = -7; y = 3; v = 80; break;
                    case 6: x = 0; y = -5; v = 10; break;
                }

                int dx = (int)(x * STEPS_PER_CM);
                int dy = (int)(y * STEPS_PER_CM);
                SendPacket(dx, dy, v);
                StatusText.Text = $"Demo {id}: {x}, {y} @ {v}%";
            }
        }

        private async void ShapeBtn_Click(object sender, RoutedEventArgs e)
        {
            // Draw a Star (5 points)
            // We need to send sequential commands. 
            // Since we don't have feedback from MCU when move is done, we will use delays.
            // This is a naive implementation. Ideally MCU sends "Done" byte.
            
            int[][] points = new int[][] {
                new int[] { 2, 6 },
                new int[] { 3, -6 },
                new int[] { -5, 4 },
                new int[] { 6, 0 },
                new int[] { -6, -4 }
            };

            foreach (var p in points)
            {
                int dx = (int)(p[0] * STEPS_PER_CM);
                int dy = (int)(p[1] * STEPS_PER_CM);
                SendPacket(dx, dy, 80);
                StatusText.Text = $"Shape: {p[0]}, {p[1]}";
                
                // Estimate time: Distance / Speed. 
                // Let's just wait 1 second for simplicity in this demo.
                await System.Threading.Tasks.Task.Delay(1500); 
            }
            StatusText.Text = "Shape Complete";
        }
    }
}

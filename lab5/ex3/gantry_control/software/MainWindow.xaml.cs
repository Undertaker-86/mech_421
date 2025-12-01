using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace GantryControl
{
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;
        // Calibration: Steps per CM
        // Motor 1 (Physical X, now UI Y): 100 steps/cm
        // Motor 2 (Physical Y, now UI X): 125 steps/cm
        const double STEPS_PER_CM_M1 = 100.0; 
        const double STEPS_PER_CM_M2 = 125.0;

        // Position Tracking (Relative to "Center")
        double _currentX = 0;
        double _currentY = 0;

        // Speeds
        const int TRACING_SPEED = 1;
        const int RETURN_SPEED = 1;

        private bool _isUpdatingVelocity = false;

        public MainWindow()
        {
            InitializeComponent();
            LoadPorts();
        }

        private void VelSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdatingVelocity) return;
            _isUpdatingVelocity = true;
            if (VelInput != null)
            {
                VelInput.Text = ((int)VelSlider.Value).ToString();
            }
            _isUpdatingVelocity = false;
        }

        private void VelInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingVelocity) return;
            _isUpdatingVelocity = true;
            if (int.TryParse(VelInput.Text, out int value))
            {
                if (value < 1) value = 1;
                if (value > 100) value = 100;
                VelSlider.Value = value;
            }
            _isUpdatingVelocity = false;
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
            if (vel == 255) { esc |= 0x01; vel = 0; } 
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

            // Update Position Tracking
            // dxSteps was sent to Motor 1 (Physical X / UI Y) -> Wait, let's check MoveBtn_Click mapping
            // In MoveBtn_Click: 
            // dx (Motor 1) = yCm * STEPS_PER_CM_M1
            // dy (Motor 2) = xCm * STEPS_PER_CM_M2
            // So dxSteps corresponds to UI Y, dySteps corresponds to UI X.
            
            double movedY = dxSteps / STEPS_PER_CM_M1;
            double movedX = dySteps / STEPS_PER_CM_M2;

            _currentX += movedX;
            _currentY += movedY;
        }

        private void MoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(XInput.Text, out double xCm) && double.TryParse(YInput.Text, out double yCm))
            {
                // Axis Flip:
                // UI X -> Motor 2 (Physical Y)
                // UI Y -> Motor 1 (Physical X)
                
                int dx = (int)(yCm * STEPS_PER_CM_M1); // Motor 1 is now Y input
                int dy = (int)(xCm * STEPS_PER_CM_M2); // Motor 2 is now X input
                
                // Remap speed: UI 1-100% -> Actual 1-12%
                double uiSpeed = VelSlider.Value;
                int actualSpeed = (int)Math.Round(1.0 + (uiSpeed - 1.0) * 11.0 / 99.0);
                
                SendPacket(dx, dy, actualSpeed);
                StatusText.Text = $"Sent Move: X={xCm}cm, Y={yCm}cm @ {uiSpeed:F0}% (actual: {actualSpeed}%)";
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

                int dx = (int)(y * STEPS_PER_CM_M1); 
                int dy = (int)(x * STEPS_PER_CM_M2); 
                SendPacket(dx, dy, v);
                StatusText.Text = $"Demo {id}: {x}, {y} @ {v}%";
            }
        }

        private async void ShapeBtn_Click(object sender, RoutedEventArgs e)
        {
            int[][] points = new int[][] {
                new int[] { 2, 6 },
                new int[] { 3, -6 },
                new int[] { -5, 4 },
                new int[] { 6, 0 },
                new int[] { -6, -4 }
            };

            foreach (var p in points)
            {
                int dx = (int)(p[1] * STEPS_PER_CM_M1); 
                int dy = (int)(p[0] * STEPS_PER_CM_M2); 
                SendPacket(dx, dy, 80);
                StatusText.Text = $"Shape: {p[0]}, {p[1]}";
                await Task.Delay(1500); 
            }
            StatusText.Text = "Shape Complete";
        }

        // --- New Drawing & Tracing Logic ---

        private void SetCenterBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentX = 0;
            _currentY = 0;
            StatusText.Text = "Center Set to Current Position (0,0)";
        }

        private async void GoToCenterBtn_Click(object sender, RoutedEventArgs e)
        {
            double moveX = -_currentX;
            double moveY = -_currentY;

            int dx = (int)(moveY * STEPS_PER_CM_M1);
            int dy = (int)(moveX * STEPS_PER_CM_M2);

            SendPacket(dx, dy, RETURN_SPEED); // Moderate speed for return
            StatusText.Text = $"Returning to Center: {moveX:F2}, {moveY:F2}";
            
            // Wait for move to complete (estimated)
            int delay = (int)(Math.Max(Math.Abs(moveX), Math.Abs(moveY)) * 200 + 500);
            await Task.Delay(delay);
        }

        private void ImportImageBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                TraceImage.Source = bitmap;
                StatusText.Text = "Image Imported";
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Strokes.Clear();
            TraceImage.Source = null;
            StatusText.Text = "Canvas Cleared";
        }

        // Drawing Limits
        const double DRAWING_SIZE_CM = 12.0;

        private async void TraceBtn_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Processing Image...";
            await Task.Delay(100); // UI Refresh

            // 1. Render Canvas to Bitmap
            int width = (int)DrawingCanvas.ActualWidth;
            int height = (int)DrawingCanvas.ActualHeight;
            
            // We need to render the parent Grid to capture both Image and InkCanvas
            Grid parentGrid = (Grid)DrawingCanvas.Parent;
            
            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(parentGrid);

            // 2. Extract Points (Downsampling)
            // Canvas is 300x300. Physical is 12x12cm.
            // 1 pixel = 0.04 cm = 0.4 mm.
            
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            rtb.CopyPixels(pixels, stride, 0);

            List<Point> points = new List<Point>();

            for (int y = 0; y < height; y += 2)
            {
                for (int x = 0; x < width; x += 2)
                {
                    int index = y * stride + x * 4;
                    // BGRA format
                    byte b = pixels[index];
                    byte g = pixels[index + 1];
                    byte r = pixels[index + 2];
                    // byte a = pixels[index + 3];

                    // Simple threshold: if dark enough, it's a point
                    if (r < 128 && g < 128 && b < 128)
                    {
                        // Map pixel (0-300) to cm (-6 to 6)
                        // x=0 -> -6, x=300 -> 6
                        double cmX = (x / (double)width) * DRAWING_SIZE_CM - (DRAWING_SIZE_CM / 2.0);
                        // y=0 -> 6 (Top), y=300 -> -6 (Bottom)
                        double cmY = -((y / (double)height) * DRAWING_SIZE_CM - (DRAWING_SIZE_CM / 2.0));

                        points.Add(new Point(cmX, cmY));
                    }
                }
            }

            if (points.Count == 0)
            {
                StatusText.Text = "No drawing found!";
                return;
            }

            StatusText.Text = $"Found {points.Count} points. Sorting...";
            await Task.Delay(100);

            // 3. Sort Points (Nearest Neighbor)
            List<Point> sortedPoints = SortPointsNearestNeighbor(points);

            // 4. Execute
            StatusText.Text = "Tracing...";
            
            // Move to first point
            Point currentPos = new Point(_currentX, _currentY);
            
            foreach (var target in sortedPoints)
            {
                double moveX = target.X - currentPos.X;
                double moveY = target.Y - currentPos.Y;

                // Skip tiny moves
                if (Math.Abs(moveX) < 0.05 && Math.Abs(moveY) < 0.05) continue;

                int dx = (int)(moveY * STEPS_PER_CM_M1);
                int dy = (int)(moveX * STEPS_PER_CM_M2);

                // Low speed for tracing
                SendPacket(dx, dy, TRACING_SPEED);

                currentPos = target;
                
                // Wait based on distance
                double dist = Math.Sqrt(moveX*moveX + moveY*moveY);
                int waitTime = (int)(dist * 100 + 50); // Heuristic
                await Task.Delay(waitTime);
            }

            StatusText.Text = "Tracing Complete";
        }

        private List<Point> SortPointsNearestNeighbor(List<Point> points)
        {
            List<Point> sorted = new List<Point>();
            HashSet<int> visited = new HashSet<int>();
            
            // Start with the point closest to current position
            Point current = new Point(_currentX, _currentY);
            
            while (sorted.Count < points.Count)
            {
                int nearestIndex = -1;
                double minDistSq = double.MaxValue;

                for (int i = 0; i < points.Count; i++)
                {
                    if (visited.Contains(i)) continue;

                    double dSq = (points[i].X - current.X) * (points[i].X - current.X) + 
                                 (points[i].Y - current.Y) * (points[i].Y - current.Y);
                    
                    if (dSq < minDistSq)
                    {
                        minDistSq = dSq;
                        nearestIndex = i;
                    }
                }

                if (nearestIndex != -1)
                {
                    visited.Add(nearestIndex);
                    current = points[nearestIndex];
                    sorted.Add(current);
                }
                else
                {
                    break; 
                }
            }
            return sorted;
        }

        // --- Image Processing Tab Logic ---

        private void ProcImportBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                ProcImage.Source = bitmap;
                ProcStatusText.Text = "Image Imported";
            }
        }

        private async void ProcConvertBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProcImage.Source == null) return;

            ProcStatusText.Text = "Processing...";
            await Task.Delay(10); // UI Refresh

            BitmapSource source = (BitmapSource)ProcImage.Source;

            // 1. Resize if needed
            bool highRes = HighResCheck.IsChecked == true;
            int targetWidth = highRes ? source.PixelWidth : 300;
            
            if (source.PixelWidth > targetWidth)
            {
                double scale = (double)targetWidth / source.PixelWidth;
                source = new TransformedBitmap(source, new ScaleTransform(scale, scale));
            }

            // 2. Convert to Gray8 for simpler processing
            FormatConvertedBitmap grayBitmap = new FormatConvertedBitmap();
            grayBitmap.BeginInit();
            grayBitmap.Source = source;
            grayBitmap.DestinationFormat = PixelFormats.Gray8;
            grayBitmap.EndInit();

            int width = grayBitmap.PixelWidth;
            int height = grayBitmap.PixelHeight;
            int stride = width; // 1 byte per pixel
            byte[] pixels = new byte[height * stride];
            grayBitmap.CopyPixels(pixels, stride, 0);

            byte[] resultPixels = new byte[height * stride];

            // 3. Sobel Edge Detection
            // Kernels
            // Gx: -1 0 1
            //     -2 0 2
            //     -1 0 1
            // Gy: -1 -2 -1
            //      0  0  0
            //      1  2  1

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int i = y * stride + x;

                    // Gx
                    int gx = -pixels[i - stride - 1] + pixels[i - stride + 1]
                             - 2 * pixels[i - 1] + 2 * pixels[i + 1]
                             - pixels[i + stride - 1] + pixels[i + stride + 1];

                    // Gy
                    int gy = -pixels[i - stride - 1] - 2 * pixels[i - stride] - pixels[i - stride + 1]
                             + pixels[i + stride - 1] + 2 * pixels[i + stride] + pixels[i + stride + 1];

                    int mag = (int)Math.Sqrt(gx * gx + gy * gy);
                    
                    // Threshold
                    resultPixels[i] = (byte)(mag > 128 ? 255 : 0);
                }
            }

            // 4. Create Result Bitmap
            WriteableBitmap result = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);
            result.WritePixels(new Int32Rect(0, 0, width, height), resultPixels, stride, 0);

            ProcImage.Source = result;
            ProcStatusText.Text = "Edge Detection Complete";
        }

        private async void ProcDrawBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProcImage.Source == null) return;
            
            ProcStatusText.Text = "Analyzing Edges...";
            await Task.Delay(10);

            BitmapSource source = (BitmapSource)ProcImage.Source;
            
            // Ensure it's Gray8 or convert
            if (source.Format != PixelFormats.Gray8)
            {
                FormatConvertedBitmap gray = new FormatConvertedBitmap();
                gray.BeginInit();
                gray.Source = source;
                gray.DestinationFormat = PixelFormats.Gray8;
                gray.EndInit();
                source = gray;
            }

            int width = source.PixelWidth;
            int height = source.PixelHeight;
            int stride = width; // 1 byte per pixel for Gray8
            byte[] pixels = new byte[height * stride];
            source.CopyPixels(pixels, stride, 0);

            List<Point> points = new List<Point>();

            // Collect points (White pixels are edges)
            // Downsample for drawing if high res? Maybe.
            // Let's sample every N pixels if it's huge, but the user asked for high res processing.
            // However, the robot has physical limits.
            // Let's just take every pixel that is an edge.

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (pixels[y * stride + x] > 128) // Edge
                    {
                        // Map to Physical Space (12x12 cm)
                        // Center (0,0) is middle of image
                        
                        // Scale to fit? The above maps full width to 12cm.
                        // If aspect ratio is not 1:1, we should preserve it.
                        // Let's fit the largest dimension to DRAWING_SIZE_CM.
                        
                        double maxDim = Math.Max(width, height);
                        double cmX = (x - width / 2.0) / maxDim * DRAWING_SIZE_CM;
                        double cmY = -(y - height / 2.0) / maxDim * DRAWING_SIZE_CM;

                        points.Add(new Point(cmX, cmY));
                    }
                }
            }

            if (points.Count == 0)
            {
                ProcStatusText.Text = "No edges found!";
                return;
            }

            // Optimization: If too many points, maybe simplify?
            // For now, let's just run it.
            
            ProcStatusText.Text = $"Found {points.Count} points. Sorting...";
            await Task.Delay(100);

            List<Point> sortedPoints = SortPointsNearestNeighbor(points);

            ProcStatusText.Text = "Drawing...";
            
            Point currentPos = new Point(_currentX, _currentY);
            
            foreach (var target in sortedPoints)
            {
                double moveX = target.X - currentPos.X;
                double moveY = target.Y - currentPos.Y;

                if (Math.Abs(moveX) < 0.05 && Math.Abs(moveY) < 0.05) continue;

                int dx = (int)(moveY * STEPS_PER_CM_M1);
                int dy = (int)(moveX * STEPS_PER_CM_M2);

                SendPacket(dx, dy, TRACING_SPEED);

                currentPos = target;
                
                double dist = Math.Sqrt(moveX*moveX + moveY*moveY);
                int waitTime = (int)(dist * 100 + 50);
                await Task.Delay(waitTime);
            }

            ProcStatusText.Text = "Drawing Complete";
        }
    }
}

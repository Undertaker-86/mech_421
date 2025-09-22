using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Collections.Concurrent;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;

namespace ex8
{
    public partial class Form1 : Form
    {
        // define variable
        private ConcurrentQueue<int> data_queue = new ConcurrentQueue<int>();
        private Stopwatch stopwatch = new Stopwatch();

        private int TIMER_INTERVAL = 100;

        // parsing state machine
        private enum ParseState
        {
            waiting_for_header,
            waiting_for_ax,
            waiting_for_ay,
            waiting_for_az
        }
        private ParseState current_parse_state = ParseState.waiting_for_header;

        private int current_ax, current_ay, current_az;

        // data points
        private struct DataPoint { public int ax, ay, az; public double t; }
        private Queue<DataPoint> data_history = new Queue<DataPoint>();
        private const int AVERAGE_WINDOW_SIZE = 100;
        private double avg_ax, avg_ay, avg_az;

        private enum GestureState { idle, x_detected, y_detected, z_detected, wait_after_x, wait_after_y, wait_after_z, cooldown }


        private GestureState current_gesture_state = GestureState.idle;
        private int gesture_timeout_counter = 0;
        private const int GESTURE_TIMEOUT_LIMIT = 300; // Timeout after x data points
        private const int WAIT_DURATION = 50; // Wait duration between sequential motions (in data points)
        private const int AFTER_DETECTION_TIMEOUT = 300;

        private string last_detected_gesture = "";
        private int gesture_display_counter = 0;

        private List<DataPoint> session_data = new List<DataPoint>();
        public Form1()
        {
            InitializeComponent();
        }
        private void PopulateCOMPorts()
        {
            comboBoxCOMPorts.Items.Clear();
            comboBoxCOMPorts.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            if (comboBoxCOMPorts.Items.Count <= 0)
            {
                comboBoxCOMPorts.Text = "No COM Ports";
            }
            else
            {
                comboBoxCOMPorts.SelectedIndex = 0;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateCOMPorts();
            UpdateUI();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ProcessDataQueue();
            if (serialPort1.IsOpen)
            {
                UpdateUI();
            }
        }

        private void ProcessDataQueue()
        {
            while (data_queue.TryDequeue(out int new_byte))
            {
                switch (current_parse_state)
                {
                    case ParseState.waiting_for_header:
                        if (new_byte == 255) current_parse_state = ParseState.waiting_for_ax;
                        break;

                    case ParseState.waiting_for_ax:
                        current_ax = new_byte;
                        current_parse_state = ParseState.waiting_for_ay;
                        break;

                    case ParseState.waiting_for_ay:
                        current_ay = new_byte;
                        current_parse_state = ParseState.waiting_for_az;
                        break;

                    case ParseState.waiting_for_az:
                        current_az = new_byte;
                        // complete cycle
                        var new_point = new DataPoint { ax = current_ax,
                                                        ay = current_ay,
                                                        az = current_az,
                                                        t = stopwatch.Elapsed.TotalSeconds
                                                        };
                        session_data.Add(new_point);
                        updateAverages(new_point);
                        ProcessGesture(new_point);

                        current_parse_state = ParseState.waiting_for_header;
                        break;
                }
            }
        }
        private void updateAverages (DataPoint new_point)
        {
            data_history.Enqueue(new_point);
            if (data_history.Count > AVERAGE_WINDOW_SIZE)
            {
                data_history.Dequeue();
            }

            if (data_history.Count > 0)
            {
                avg_ax = data_history.Average(p => p.ax);
                avg_ay = data_history.Average(p => p.ay);
                avg_az = data_history.Average(p => p.az);

            }    
        }

        private void ProcessGesture(DataPoint point)
        {
            const int POS_X_THRESHOLD = 150;
            const int POS_Y_THRESHOLD = 150;
            const int POS_Z_THRESHOLD = 185;

            // Skip processing during cooldown
            if (current_gesture_state == GestureState.cooldown)
            {
                gesture_timeout_counter++;
                if (gesture_timeout_counter >= AFTER_DETECTION_TIMEOUT)
                {
                    current_gesture_state = GestureState.idle;
                    LogHistory("READY: Now accepting new gestures");
                }
                return;
            }

            // Handle timeout for all non-idle states
            if (current_gesture_state != GestureState.idle)
            {
                gesture_timeout_counter++;
                if (gesture_timeout_counter > GESTURE_TIMEOUT_LIMIT)
                {
                    if (current_gesture_state == GestureState.wait_after_x)
                    {
                        DetectGesture("Simple Punch");
                        LogHistory("ENTER NEXT MOTION: Simple punch completed after timeout");
                        current_gesture_state = GestureState.cooldown;
                        gesture_timeout_counter = 0;
                        LogHistory("COOLDOWN: Waiting before accepting new gestures");
                    }
                    else
                    {
                        current_gesture_state = GestureState.idle;
                        LogHistory("--- Gesture Timed Out ---");
                    }
                    return;
                }
            }

            switch (current_gesture_state)
            {
                case GestureState.idle:
                    {
                        if (point.ax > POS_X_THRESHOLD)
                        {
                            current_gesture_state = GestureState.x_detected;
                            gesture_timeout_counter = 0;
                            LogHistory("ACCEPTED: +X motion detected");
                            LogHistory($"State change: Idle -> {current_gesture_state}");
                        }
                        else if (point.az > POS_Z_THRESHOLD)
                        {
                            current_gesture_state = GestureState.z_detected;
                            gesture_timeout_counter = 0;
                            LogHistory("ACCEPTED: +Z motion detected");
                            LogHistory($"State change: Idle -> {current_gesture_state}");
                        }
                        break;
                    }

                case GestureState.x_detected:
                    {
                        if (gesture_timeout_counter >= WAIT_DURATION)
                        {
                            current_gesture_state = GestureState.wait_after_x;
                            LogHistory("WAIT: Waiting for next motion after +X");
                        }
                        break;
                    }

                case GestureState.wait_after_x:
                    {
                        if (point.ay > POS_Y_THRESHOLD)
                        {
                            current_gesture_state = GestureState.y_detected;
                            gesture_timeout_counter = 0;
                            LogHistory("ACCEPTED: +Y motion detected");
                            LogHistory($"State change: wait_after_x -> {current_gesture_state}");
                        }
                        break;
                    }

                case GestureState.y_detected:
                    {
                        if (gesture_timeout_counter >= WAIT_DURATION)
                        {
                            current_gesture_state = GestureState.wait_after_y;
                            LogHistory("WAIT: Waiting for next motion after +Y");
                        }
                        break;
                    }

                case GestureState.wait_after_y:
                    {
                        if (point.az > POS_Z_THRESHOLD)
                        {
                            DetectGesture("Right-Hook");
                            LogHistory("ENTER NEXT MOTION: Right-hook sequence completed");
                            current_gesture_state = GestureState.cooldown;
                            gesture_timeout_counter = 0;
                            LogHistory("COOLDOWN: Waiting before accepting new gestures");
                        }
                        break;
                    }

                case GestureState.z_detected:
                    {
                        if (gesture_timeout_counter >= WAIT_DURATION)
                        {
                            current_gesture_state = GestureState.wait_after_z;
                            LogHistory("WAIT: Waiting for next motion after +Z");
                        }
                        break;
                    }

                case GestureState.wait_after_z:
                    {
                        if (point.ax > POS_X_THRESHOLD)
                        {
                            DetectGesture("High Punch");
                            LogHistory("ENTER NEXT MOTION: High punch sequence completed");
                            current_gesture_state = GestureState.cooldown;
                            gesture_timeout_counter = 0;
                            LogHistory("COOLDOWN: Waiting before accepting new gestures");
                        }
                        break;
                    }
            }
        }

        private void ResetToIdle()
        {
            current_gesture_state = GestureState.idle;
            gesture_timeout_counter = 0;
            LogHistory("Ready for new gesture input");
        }
        private void UpdateUI()
        {
            // 1
            textBoxAx.Text = current_ax.ToString();
            textBoxAy.Text = current_ay.ToString();
            textBoxAz.Text = current_az.ToString();
            string acceleration_text = $"ax: {current_ax.ToString()}, ay: {current_ay.ToString()}, az: {current_az.ToString()}";
            textBoxHistory.AppendText(acceleration_text + Environment.NewLine);
            // 2
            textBoxQueueSize.Text = data_queue.Count.ToString();

            // 3
            textBoxAvgAx.Text = avg_ax.ToString("F2");
            textBoxAvgAy.Text = avg_ay.ToString("F2");
            textBoxAvgAz.Text = avg_az.ToString("F2");

            // 4
            UpdateOrientation();

            if (gesture_display_counter > 0)
            {
                textBoxGesture.Text = last_detected_gesture;
                gesture_display_counter--;
            }
            else
            {
                textBoxGesture.Text = "---";
            }

        }


        // helper function

        private void UpdateOrientation()
        {
            const int RESTING_NEUTRAL = 127;
            const int RESTING_Z = 154;
            const int ORIENTATION_THRESHOLD = 10;

            int devX = Math.Abs(current_ax - RESTING_NEUTRAL);
            int devY = Math.Abs(current_ay - RESTING_NEUTRAL);
            int devZ = Math.Abs(current_az - RESTING_Z);

            string orientation = "Z+";

            if (devX > devY && devX > devZ && devX > ORIENTATION_THRESHOLD)
            {
                orientation = current_ax > RESTING_NEUTRAL ? "X+" : "X-"; // This is c syntax
                // it says is cond 1 satisfied? if yes go to option 1 else option 2
            }
            else if (devY > devX && devY > devZ && devY > ORIENTATION_THRESHOLD)
            {
                orientation = current_ay > RESTING_NEUTRAL ? "Y+" : "Y-"; 
            }
            else if (devZ > devX && devZ > devY && devZ > ORIENTATION_THRESHOLD)
            {
                orientation = current_az > RESTING_Z ? "Z+" : "Z-"; 
            }
            textBoxOrientation.AppendText(orientation + Environment.NewLine);
        }
        private void DetectGesture(string s)
        {
            last_detected_gesture = s;
            gesture_display_counter = 20;
            LogHistory($"***** {s} DETECTED *****");
        }


        private void LogHistory(string message)
        {
            if (textBoxStateHistory.InvokeRequired)
            {
                textBoxStateHistory.Invoke(new Action(() => LogHistory(message)));
            }
            else
            {
                textBoxStateHistory.AppendText(message + Environment.NewLine);
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                stopwatch.Stop();
                try
                {
                    serialPort1.Write("z");
                    serialPort1.Close();
                    buttonConnect.Text = "Connect";
                    timer1.Stop();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error {ex}", "Serial Port Disconnect Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    serialPort1.PortName = comboBoxCOMPorts.Text;
                    serialPort1.BaudRate = 9600;
                    serialPort1.Open();
                    serialPort1.Write("a");

                    data_queue = new ConcurrentQueue<int>();
                    data_history.Clear();
                    session_data.Clear();
                    current_gesture_state = GestureState.idle;

                    stopwatch.Restart();
                    timer1.Start();
                    timer1.Interval = TIMER_INTERVAL;
                    buttonConnect.Text = "Disconnect";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error {ex}", "Serial Port Connect Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV File (*.csv) | *.csv";
            sfd.Title = "Select File to Save Data";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                textBoxSelectFile.Text = sfd.FileName;
            }
        }
        private void buttonSaveData_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxSelectFile.Text))
            {
                MessageBox.Show("Please select a file", "Error no file save destination",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (session_data.Count == 0)
            {
                MessageBox.Show("No data detected", "No data error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                using (StreamWriter file = new StreamWriter(textBoxSelectFile.Text))
                {
                    file.WriteLine("Ax,Ay,Az,t_seconds");
                    foreach (var point in session_data)
                    {
                        file.WriteLine(string.Format(CultureInfo.InvariantCulture,
                            "{0},{1},{2},{3:F6}",
                            point.ax, point.ay, point.az, point.t));
                    }
                }
                MessageBox.Show($"Successfully saved {session_data.Count} points", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error saving file, exception: {ex}", "Saving file error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (serialPort1.IsOpen && serialPort1.BytesToRead > 0)
            {
                try
                {
                    data_queue.Enqueue(serialPort1.ReadByte());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file, exception: {ex}", "Saving file error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }
    }
}

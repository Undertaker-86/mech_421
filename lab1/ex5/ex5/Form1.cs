using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Collections.Concurrent;
using System.Text;
using System.Diagnostics;

namespace ex4
{
    public partial class Form1 : Form
    {
        private ConcurrentQueue<int> dataQueue = new ConcurrentQueue<int>();
        private Stopwatch stopwatch = new Stopwatch();
        string serial_string = "";

        // State machine for reading 255 value
        private enum ParseState
        {
            waiting_for_header,
            waiting_for_ax,
            waiting_for_ay,
            waiting_for_az
        }

        private ParseState current_state = ParseState.waiting_for_header;
        private int current_ax, current_ay, current_az;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxCOMPorts.Items.Clear();
            comboBoxCOMPorts.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            if (comboBoxCOMPorts.Items.Count <= 0)
            {
                comboBoxCOMPorts.Text = "No COM Ports Found!";
            }
            else
            {
                comboBoxCOMPorts.SelectedIndex = 0;
            }
            timer1.Interval = 100;
            textBoxTimerTick_ms.Text = timer1.Interval.ToString();
            timer1.Start();
        }

        private void buttonDisconnectSerial_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                // Ask TA, do we need ascii encoding?
                string transmit_close = "z";
                //byte[] TxByte = Encoding.ASCII.GetBytes(transmit_close);
                //serialPort1.Write(TxByte, 0, 1);
                serialPort1.Write(transmit_close);
                serialPort1.Close();
                stopwatch.Stop();
                buttonDisconnectSerial.Text = "Connect";
            }
            else if (!serialPort1.IsOpen)
            {
                serialPort1.PortName = comboBoxCOMPorts.Text;
                serialPort1.Open();
                string transmit = "a";
                //byte[] TxByte = Encoding.ASCII.GetBytes(transmit);
                //serialPort1.Write(TxByte, 0, 1);
                serialPort1.Write(transmit);
                stopwatch.Restart();
                buttonDisconnectSerial.Text = "Disconnect";
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Sometimes clicking disconnec while ReadByte() tries to read crashes the program? How to fix?
            // Also why doesn't why exception capture this?
            while (serialPort1.BytesToRead > 0 && serialPort1.IsOpen)
            {
                try
                {
                    int newByte = serialPort1.ReadByte();
                    serial_string = serial_string + newByte.ToString() + ", ";
                    dataQueue.Enqueue(newByte);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during enqueue, Exception: {ex}", "Enqueue Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ProcessDataQueue();
            UpdateUI();
        }

        private void ProcessDataQueue()
        {
            while (dataQueue.TryDequeue(out int current_byte))
            {
                switch (current_state)
                {
                    case ParseState.waiting_for_header:
                        if (current_byte == 255)
                        {
                            current_state = ParseState.waiting_for_ax;
                        }
                        break;

                    case ParseState.waiting_for_ax:
                        { 
                            current_ax = current_byte;
                            current_state = ParseState.waiting_for_ay;
                        }
                        break;

                    case ParseState.waiting_for_ay:
                        { 
                            current_ay = current_byte;
                            current_state = ParseState.waiting_for_az;
                        }
                        break;

                    case ParseState.waiting_for_az:
                        { 
                            current_az = current_byte;
                            current_state = ParseState.waiting_for_header;
                        }
                        break;
                }
            }
        }

        private void UpdateUI()
        {

            if (serialPort1.IsOpen)
            {
                textBoxBytesToRead.Text = serialPort1.BytesToRead.ToString();
                textBoxTime_s.Text = stopwatch.Elapsed.TotalSeconds.ToString("F3");
                DetermineOrientation();
            }
            textBoxItemsInQueue.Text = dataQueue.Count.ToString();
            textBoxAx.Text = current_ax.ToString();
            textBoxAy.Text = current_ay.ToString();
            textBoxAz.Text = current_az.ToString();

            textBoxTempStringLength.Text = serial_string.Length.ToString();
            serial_string = "";
            StringBuilder sb = new StringBuilder();
            while (dataQueue.TryDequeue(out int byteFromQueue))
            {
                sb.Append(byteFromQueue.ToString() + ", ");
            }

            if (sb.Length > 0)
            {
                textBoxSerialDataStream.AppendText(sb.ToString());
            }
        }

        private void DetermineOrientation()
        {
            const int neutral = 127;
            const int threshold = 20;

            //Technically I should have 8/4 orientation?
            if (current_az > neutral + threshold)
                textBoxOrientation.AppendText("Face Up" + Environment.NewLine);
            else if (current_az < neutral - threshold)
                textBoxOrientation.AppendText("Face Down" + Environment.NewLine);
            else if (current_ax > neutral + threshold)
                textBoxOrientation.AppendText("Tilted Left" + Environment.NewLine);
            else if (current_ax < neutral - threshold)
                textBoxOrientation.AppendText("Tilted Right" + Environment.NewLine);
            else if (current_ay > neutral + threshold)
                textBoxOrientation.AppendText("Tilted Backward" + Environment.NewLine);
            else if (current_ax < neutral - threshold)
                textBoxOrientation.AppendText("Tilted Forward" + Environment.NewLine);
            else
                textBoxOrientation.AppendText("Neutral" + Environment.NewLine);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("z");
                    serialPort1.Close();
                }
                catch (Exception ex)
                {
                    // Hope it closes?
                    MessageBox.Show($"Error during closing, Exception: {ex}", "closing Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                }
            }
        }

        private void buttonSetInterval_Click(object sender, EventArgs e)
        {
            int new_interval;

            if (int.TryParse(textBoxTimerTick_ms.Text, out new_interval))
            {
                if (new_interval >= 10)
                {
                    timer1.Interval = new_interval;
                    MessageBox.Show($"Timer interval set to {new_interval} ms",
                                    "Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please don't blow up my PC :(",
                                    "Value to small",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                }
            }

            else
            {
                MessageBox.Show("Please enter a valid number",
                                "Invalid Input",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}

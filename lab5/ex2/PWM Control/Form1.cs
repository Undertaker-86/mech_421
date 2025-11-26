using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PWM_Control
{
    public partial class Form1 : Form
    {
        // Define instructions for motor contriol
        byte startByte;
        byte instructionByte;
        byte pwmMSB;
        byte pwmLSB;
        byte escapeByte;
        // Get displacement value from encoder reader
        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>(); // Concurrent queue storing all data
        double currPos;
        double lastPos = 0;
        double dcMotorPos;
        double dcMotorVelRPM;
        double dcMotorVelHZ;
        double dcEncoderStepsPerRev = 252.0;
        // Get time elapsed
        double time=0; // Seconds
        ChartArea chartArea = new ChartArea("MotorDataArea");
        Series positionSeries = new Series("Position");
        Series velocitySeries = new Series("Velocity");
        int axisState;
        int displacementMSB;
        int displacementLSB;
        int direction;
        int escReceiveByte;
        StreamWriter csvWriter = null; // Writer for saving data
        bool isSaving = false; // Flag to indicate if saving is active


        public Form1()
        {
            InitializeComponent();

            // Sample data for time, position, and velocity
            double initialPos = 0; // in degrees or mm
            double initialVel = 0; // in degrees/sec or mm/sec
            // Configure the chart area
            chartArea.AxisX.Title = "Time (s)";
            chartArea.AxisY.Title = "Position / Velocity";
            chartArea.AxisX.Interval = 1; // Adjust the interval based on your data
            chartArea.AxisY.Interval = 10;
            motorChart.ChartAreas.Add(chartArea);
            // Position Series
            positionSeries.ChartType = SeriesChartType.Line;
            positionSeries.Color = System.Drawing.Color.Blue;
            positionSeries.BorderWidth = 2;
            // Velocity Series
            velocitySeries.ChartType = SeriesChartType.Line;
            velocitySeries.Color = System.Drawing.Color.Red;
            velocitySeries.BorderWidth = 2;
            // Get points to plot
            positionSeries.Points.AddXY(time, initialPos);
            velocitySeries.Points.AddXY(time, initialVel);
            // Add series to chart
            motorChart.Series.Add(positionSeries);
            motorChart.Series.Add(velocitySeries);
            // Add legend
            motorChart.Legends.Add(new Legend("Legend")
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Center
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the scroll bar with range from -65535 to 65535
            pwmDutyScrollbar.Minimum = -65535;  // Max speed CCW (negative direction)
            pwmDutyScrollbar.Maximum = 65535;   // Max speed CW (positive direction)
            pwmDutyScrollbar.Value = 0;         // Default to 0 (no movement)
            // Set initial values for motor control instructions
            startByteTextbox.Text = "255";
            instructionByteTextbox.Text = "1";
            pwmMSBTextbox.Text = "150";
            pwmLSBTextbox.Text = "100";
            escapeByteTextbox.Text = "0";

            // Get list of all available COM ports and display them in combobox
            comboBoxCOMPorts.Items.Clear();
            comboBoxCOMPorts.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            if (comboBoxCOMPorts.Items.Count == 0)
                comboBoxCOMPorts.Text = "No COM Ports!";
            else
                comboBoxCOMPorts.SelectedIndex = 0;
        }

        private void comboBoxCOMPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Define variables
            string selectedPort; // Store port selected from comboBox

            // Can only change COM if serial port is not open
            if (comboBoxCOMPorts.SelectedItem != null && !serialPort.IsOpen)
            {
                selectedPort = comboBoxCOMPorts.SelectedItem.ToString(); // Get selected port from combobox
                serialPort.PortName = selectedPort; // Set port in the serial port object
                //MessageBox.Show(serialPort.PortName); // DEBUG: Show that prot name changed successfully
            }
        }

        private void connectDisconnectSerialButton_Click(object sender, EventArgs e)
        {
            // If serial port is not open then open port, else close the port
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();
                    connectDisconnectSerialButton.Text = "Diconnect";
                    MessageBox.Show("Serial port opened successfully: " + serialPort.PortName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to open serial port: " + ex.Message);
                }
            }
            else
            {
                serialPort.Close();
                connectDisconnectSerialButton.Text = "Connect";
                MessageBox.Show("Serial port closed: " + serialPort.PortName);
            }
        }

        private void transmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Read values from textboxes and convert them to 8-bit byte values (0-255)
                startByte = byte.Parse(startByteTextbox.Text);       // 8-bit value
                instructionByte = byte.Parse(instructionByteTextbox.Text); // 8-bit value
                pwmMSB = byte.Parse(pwmMSBTextbox.Text);             // 8-bit value
                pwmLSB = byte.Parse(pwmLSBTextbox.Text);             // 8-bit value
                escapeByte = byte.Parse(escapeByteTextbox.Text);     // 8-bit value

                // Combine pwmMSB and pwmLSB to get the full 16-bit PWM value
                int pwmValue = (pwmMSB << 8) | pwmLSB;  // Combine MSB and LSB

                // Set slider position based on the instruction byte
                if (instructionByte == 1)
                {
                    // CCW Direction (negative range)
                    pwmDutyScrollbar.Value = -pwmValue;  // Set slider to negative value
                }
                else if (instructionByte == 2)
                {
                    // CW Direction (positive range)
                    pwmDutyScrollbar.Value = pwmValue;   // Set slider to positive value
                }

                // Send the byte array
                byte[] dataPacket = { startByte, instructionByte, pwmMSB, pwmLSB, escapeByte };
                sendData(dataPacket);
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid 8-bit values (0-255) in the textboxes.");
            }
            catch (OverflowException)
            {
                MessageBox.Show("Values must be between 0 and 255.");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Value out of range for scroll bar.");
            }
        }

        // Function to send byte array to the serial port
        private void sendData(byte[] data)
        {
           serialPort.Write(data, 0, data.Length);  // Send the data array
        }

        private void pwmDutyScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            int scrollValue = pwmDutyScrollbar.Value;  // Get the value from the scroll bar
            ushort pwmValue;

            // Determine direction and adjust pwmValue based on the scroll position
            if (scrollValue < 0)
            {
                instructionByte = 1; // CCW direction
                instructionByteTextbox.Text = "1";
                pwmValue = (ushort)Math.Abs(scrollValue); // Convert to positive value for PWM
            }
            else
            {
                instructionByte = 2; // CW direction
                instructionByteTextbox.Text = "2";
                pwmValue = (ushort)scrollValue; // Positive value for PWM
            }

            // Split the 16-bit pwmValue into MSB and LSB
            pwmMSB = (byte)(pwmValue >> 8); // Get the most significant byte
            pwmLSB = (byte)(pwmValue & 0xFF); // Get the least significant byte
            pwmMSBTextbox.Text = pwmMSB.ToString();
            pwmLSBTextbox.Text = pwmLSB.ToString();

            if (pwmLSB == 255 && pwmMSB == 0)
            {
                escapeByte = 1;
                escapeByteTextbox.Text = "1";
            }
            else if (pwmLSB == 0 && pwmMSB == 255)
            {
                escapeByte = 2;
                escapeByteTextbox.Text = "2";
            }
            else if (pwmLSB == 0 && pwmMSB == 0)
            {
                escapeByte = 0;
                escapeByteTextbox.Text = "0";
            }

            byte[] dataPacket = { startByte, instructionByte, pwmMSB, pwmLSB, escapeByte };
            sendData(dataPacket);
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Initialize variables
            int newByte; // Store new read byte
            int bytesToRead = 0; // Number of bytes to be read

            if (serialPort.IsOpen)
                bytesToRead = serialPort.BytesToRead; // Get number of bytes to be read

            // While there are bytes to be read, add them to serialDataString
            while (bytesToRead != 0)
            {
                newByte = serialPort.ReadByte(); // Read byte
                dataQueue.Enqueue(Convert.ToInt32(newByte)); // Add byte to dataQueue
                if (serialPort.IsOpen)
                    bytesToRead = serialPort.BytesToRead; // Read number of remaining bytes
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            // Initialize variables
            int retVal; // Returned value from dequeue operation

            // Display data stream from concurrent queue in textbox
            while (dataQueue.Count > 0)
            {
                if (dataQueue.TryDequeue(out retVal))
                {
                    //dataStreamTextbox.AppendText(retVal.ToString() + ", "); // Append returned byte to data stream

                    // If 255 received, then next 3 values are x-axis, y-axis, and z-axis accelerations
                    if (retVal == 255)
                    {
                        axisState = 1; // Change axis state to represent x-axis acceleration for next value
                    }
                    else if (axisState == 1)
                    {
                        displacementMSB = retVal;
                        axisState = 2; // Change axis state to represent y-axis acceleration for next value
                    }
                    else if (axisState == 2)
                    {
                        displacementLSB = retVal;
                        axisState = 3; // Change axis state to represent z-axis acceleration for next value
                    }
                    else if (axisState == 3)
                    {
                        direction = retVal;
                        axisState = 4; // Reset axis state as this marks the end of the sequence
                    }
                    else if (axisState == 4)
                    {
                        escReceiveByte = retVal;
                        axisState = 0; // Reset axis state as this marks the end of the sequence
                        updateVelandPos();
                    }
                }
            }
        }

        public void updateVelandPos()
        {
            currPos = (displacementMSB << 8) | (displacementLSB & 0xFF);

            // Calculate instantaneous position and print in textbox
            dcMotorPos = (currPos / dcEncoderStepsPerRev) * 360.0;


            if (direction == 1)
            {
                dcMotorPos = dcMotorPos * -1.0;
            }

            posTextBox.Text = dcMotorPos.ToString();

            // Calculate instantaneous velocity
            dcMotorVelRPM = (((dcMotorPos-lastPos) / 0.0025) * 60.0) / 360.0;
            velRpmTextbox.Text = dcMotorVelRPM.ToString();
            dcMotorVelHZ = ((dcMotorPos - lastPos) / 0.0025) / 360.0;
            velHzTextbox.Text = dcMotorVelHZ.ToString();

            // Get points to plot
            positionSeries.Points.AddXY(time, dcMotorPos);
            velocitySeries.Points.AddXY(time, dcMotorVelRPM);

            lastPos = dcMotorPos;

            //Save data to CSV is saving is enabled
            if (isSaving && csvWriter != null)
            {
                csvWriter.WriteLine($"{currPos}");
            }
        }

        private void StartSaveButton_Click(object sender, EventArgs e)
        {
            // Open a CSV file for writing
            csvWriter = new StreamWriter(@"C:\Users\bobsy\Desktop\Encoder Reader\motor_data.csv");
            csvWriter.WriteLine("Position(degrees)"); // Write the header
            isSaving = true; // Set saving flag
            MessageBox.Show("Data saving started.");
        }

        private void EndSaveButton_Click(object sender, EventArgs e)
        {
            csvWriter.Close(); // Close the file
            csvWriter = null; // Release the writer
            isSaving = false; // Reset saving flag
            MessageBox.Show("Data saving stopped.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PWM_Control
{
    public partial class Form1 : Form
    {
        byte startByte;
        byte instructionByte;
        byte pwmMSB;
        byte pwmLSB;
        byte escapeByte;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the scroll bar with range from -65535 to 65535
            // Note: HScrollBar/VScrollBar .Maximum is a bit tricky, the reachable max is Maximum - LargeChange + 1
            // But we will stick to user's logic for now. 
            // Standard scrollbars in WinForms are 0 to MAX. 
            // The user wants -65535 to 65535.
            pwmDutyScrollbar.Minimum = -65535;  // Max speed CCW (negative direction)
            pwmDutyScrollbar.Maximum = 65535;   // Max speed CW (positive direction)
            pwmDutyScrollbar.Value = 0;         // Default to 0 (no movement)
            
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
                //MessageBox.Show(serialPort.PortName); // DEBUG: Show that port name changed successfully
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
                    connectDisconnectSerialButton.Text = "Disconnect";
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
            if (serialPort.IsOpen)
            {
                serialPort.Write(data, 0, data.Length);  // Send the data array
            }
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
            
            // Note: Updating textboxes here might cause issues if user is typing, but for scroll it's fine
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
    }
}

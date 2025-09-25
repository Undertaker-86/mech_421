using System;
using System.IO.Ports;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class AccelerometerData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class AccelerometerManager : IDisposable
    {
        private SerialPort serialPort;
        private AccelerometerData currentData;
        private readonly object dataLock = new object();
        private bool isConnected = false;
        
        // Calibration values (you may need to adjust these based on your setup)
        private const float THRESHOLD = 0.3f; // Minimum acceleration threshold to register as input
        private const float SCALE_FACTOR = 1.0f; // Scaling factor for acceleration values
        
        public bool IsConnected => isConnected;
        
        public AccelerometerManager()
        {
            currentData = new AccelerometerData();
        }
        
        /// <summary>
        /// Attempts to connect to the MSP430EXP board on the specified COM port
        /// </summary>
        /// <param name="portName">COM port name (e.g., "COM3")</param>
        /// <param name="baudRate">Baud rate (typically 9600 or 115200)</param>
        /// <returns>True if connection successful, false otherwise</returns>
        public bool Connect(string portName = "COM3", int baudRate = 9600)
        {
            try
            {
                serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.ReadTimeout = 500;
                serialPort.WriteTimeout = 500;
                
                serialPort.Open();
                isConnected = true;
                
                Console.WriteLine($"Accelerometer connected on {portName} at {baudRate} baud");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to accelerometer: {ex.Message}");
                isConnected = false;
                return false;
            }
        }
        
        /// <summary>
        /// Attempts to auto-detect and connect to MSP430EXP board
        /// </summary>
        /// <returns>True if connection successful, false otherwise</returns>
        public bool AutoConnect()
        {
            string[] portNames = SerialPort.GetPortNames();
            
            foreach (string portName in portNames)
            {
                Console.WriteLine($"Trying to connect to {portName}...");
                if (Connect(portName))
                {
                    return true;
                }
            }
            
            Console.WriteLine("No accelerometer found on available COM ports");
            return false;
        }
        
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    string data = serialPort.ReadLine().Trim();
                    ParseAccelerometerData(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading accelerometer data: {ex.Message}");
            }
        }
        
        private void ParseAccelerometerData(string data)
        {
            try
            {
                // Expected format: "X:0.123,Y:0.456,Z:0.789" or similar
                // Adjust this parsing logic based on your MSP430 data format
                string[] parts = data.Split(',');
                
                if (parts.Length >= 3)
                {
                    float x = 0, y = 0, z = 0;
                    
                    foreach (string part in parts)
                    {
                        string[] keyValue = part.Split(':');
                        if (keyValue.Length == 2)
                        {
                            string axis = keyValue[0].Trim().ToUpper();
                            if (float.TryParse(keyValue[1].Trim(), out float value))
                            {
                                switch (axis)
                                {
                                    case "X":
                                        x = value;
                                        break;
                                    case "Y":
                                        y = value;
                                        break;
                                    case "Z":
                                        z = value;
                                        break;
                                }
                            }
                        }
                    }
                    
                    lock (dataLock)
                    {
                        currentData.X = x * SCALE_FACTOR;
                        currentData.Y = y * SCALE_FACTOR;
                        currentData.Z = z * SCALE_FACTOR;
                        currentData.Timestamp = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing accelerometer data: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Gets the current accelerometer data
        /// </summary>
        /// <returns>AccelerometerData object with current readings</returns>
        public AccelerometerData GetCurrentData()
        {
            lock (dataLock)
            {
                return new AccelerometerData
                {
                    X = currentData.X,
                    Y = currentData.Y,
                    Z = currentData.Z,
                    Timestamp = currentData.Timestamp
                };
            }
        }
        
        /// <summary>
        /// Checks if the Y-axis acceleration indicates moving right
        /// </summary>
        /// <returns>True if accelerometer indicates right movement</returns>
        public bool IsMoveRight()
        {
            var data = GetCurrentData();
            return data.Y > THRESHOLD;
        }
        
        /// <summary>
        /// Checks if the Y-axis acceleration indicates moving left
        /// </summary>
        /// <returns>True if accelerometer indicates left movement</returns>
        public bool IsMoveLeft()
        {
            var data = GetCurrentData();
            return data.Y < -THRESHOLD;
        }
        
        /// <summary>
        /// Checks if the X-axis acceleration indicates attacking
        /// </summary>
        /// <returns>True if accelerometer indicates attack action</returns>
        public bool IsAttack()
        {
            var data = GetCurrentData();
            return data.X > THRESHOLD;
        }
        
        /// <summary>
        /// Checks if the X-axis acceleration indicates jumping
        /// </summary>
        /// <returns>True if accelerometer indicates jump action</returns>
        public bool IsJump()
        {
            var data = GetCurrentData();
            return data.X < -THRESHOLD;
        }
        
        /// <summary>
        /// Gets the movement strength for smooth analog-like movement
        /// </summary>
        /// <returns>Movement strength between -1.0 (full left) and 1.0 (full right)</returns>
        public float GetMovementStrength()
        {
            var data = GetCurrentData();
            return Math.Max(-1.0f, Math.Min(1.0f, data.Y / 2.0f)); // Scale and clamp to [-1, 1]
        }
        
        public void Disconnect()
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    isConnected = false;
                    Console.WriteLine("Accelerometer disconnected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disconnecting accelerometer: {ex.Message}");
            }
        }
        
        public void Dispose()
        {
            Disconnect();
            serialPort?.Dispose();
        }
    }
}
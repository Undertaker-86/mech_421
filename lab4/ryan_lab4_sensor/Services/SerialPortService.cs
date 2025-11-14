using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace RyanSensorApp.Services
{
    public class SerialPortService : IDisposable
    {
        private SerialPort? _serialPort;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _readTask;
        private byte[] _buffer = new byte[3];
        private int _bufferIndex = 0;
        private const byte START_BYTE = 0xFF;

        public event EventHandler<AdcDataReceivedEventArgs>? AdcDataReceived;
        public event EventHandler<string>? ErrorOccurred;
        public event EventHandler? ConnectionLost;

        public bool IsConnected => _serialPort?.IsOpen ?? false;

        public bool Connect(string portName, int baudRate = 9600)
        {
            try
            {
                Disconnect();

                _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
                {
                    ReadTimeout = 1000,
                    WriteTimeout = 1000
                };

                _serialPort.Open();

                // Start reading in background thread
                _cancellationTokenSource = new CancellationTokenSource();
                _readTask = Task.Run(() => ReadDataAsync(_cancellationTokenSource.Token));

                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Connection error: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _readTask?.Wait(TimeSpan.FromSeconds(2));

                if (_serialPort?.IsOpen == true)
                {
                    _serialPort.Close();
                }

                _serialPort?.Dispose();
                _serialPort = null;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                _bufferIndex = 0;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Disconnect error: {ex.Message}");
            }
        }

        private async Task ReadDataAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_serialPort?.IsOpen == true && _serialPort.BytesToRead > 0)
                    {
                        int readByte = _serialPort.ReadByte();
                        if (readByte >= 0)
                        {
                            ProcessByte((byte)readByte);
                        }
                    }
                    else
                    {
                        await Task.Delay(1, cancellationToken);
                    }
                }
                catch (TimeoutException)
                {
                    // Normal timeout, continue
                }
                catch (InvalidOperationException)
                {
                    // Port closed
                    ConnectionLost?.Invoke(this, EventArgs.Empty);
                    break;
                }
                catch (Exception ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        ErrorOccurred?.Invoke(this, $"Read error: {ex.Message}");
                        await Task.Delay(100, cancellationToken);
                    }
                }
            }
        }

        private void ProcessByte(byte data)
        {
            // State machine for packet parsing
            if (_bufferIndex == 0)
            {
                // Looking for start byte
                if (data == START_BYTE)
                {
                    _buffer[0] = data;
                    _bufferIndex = 1;
                }
            }
            else if (_bufferIndex == 1)
            {
                // MS5B (most significant 5 bits)
                _buffer[1] = data;
                _bufferIndex = 2;
            }
            else if (_bufferIndex == 2)
            {
                // LS5B (least significant 5 bits)
                _buffer[2] = data;
                
                // Reassemble 10-bit ADC value
                int ms5b = _buffer[1] & 0x1F;  // Mask to 5 bits
                int ls5b = _buffer[2] & 0x1F;  // Mask to 5 bits
                int adcValue = (ms5b << 5) | ls5b;  // Combine into 10-bit value

                // Raise event with ADC data
                AdcDataReceived?.Invoke(this, new AdcDataReceivedEventArgs(adcValue));

                // Reset for next packet
                _bufferIndex = 0;
            }
        }

        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }

    public class AdcDataReceivedEventArgs : EventArgs
    {
        public int AdcValue { get; }
        public DateTime Timestamp { get; }

        public AdcDataReceivedEventArgs(int adcValue)
        {
            AdcValue = adcValue;
            Timestamp = DateTime.Now;
        }
    }
}

using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using DistanceMonitor.Models;

namespace DistanceMonitor.Services;

public sealed class SerialDataService : IDisposable
{
    private const byte StartByte = 0xFF;
    private SerialPort? _serialPort;
    private CancellationTokenSource? _cts;
    private Task? _readerTask;

    public event EventHandler<RawSample>? SampleReceived;
    public event EventHandler<string>? StatusMessage;

    public bool IsConnected => _serialPort is { IsOpen: true };

    public void Connect(string portName, int baudRate = 9600)
    {
        if (IsConnected)
        {
            throw new InvalidOperationException("Serial port already open.");
        }

        _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
        {
            ReadTimeout = 500
        };

        _serialPort.Open();

        _cts = new CancellationTokenSource();
        _readerTask = Task.Run(() => ReadLoop(_cts.Token), _cts.Token);
        OnStatus($"Connected to {portName} @ {baudRate} baud.");
    }

    private void ReadLoop(CancellationToken token)
    {
        if (_serialPort is null)
        {
            return;
        }

        var state = FrameState.WaitingForStart;
        byte ms5 = 0;

        try
        {
            while (!token.IsCancellationRequested)
            {
                int result;
                try
                {
                    result = _serialPort.ReadByte();
                }
                catch (TimeoutException)
                {
                    continue;
                }

                if (result < 0)
                {
                    continue;
                }

                byte b = (byte)result;

                switch (state)
                {
                    case FrameState.WaitingForStart:
                        if (b == StartByte)
                        {
                            state = FrameState.WaitingForMs5;
                        }
                        break;
                    case FrameState.WaitingForMs5:
                        ms5 = (byte)(b & 0x1F);
                        state = FrameState.WaitingForLs5;
                        break;
                    case FrameState.WaitingForLs5:
                        byte ls5 = (byte)(b & 0x1F);
                        int rawValue = (ms5 << 5) | ls5;
                        var sample = new RawSample(DateTime.Now, rawValue, ms5, ls5);
                        SampleReceived?.Invoke(this, sample);
                        state = FrameState.WaitingForStart;
                        break;
                }
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException)
        {
            OnStatus($"Serial port error: {ex.Message}");
        }
    }

    public void Disconnect()
    {
        if (!IsConnected)
        {
            return;
        }

        try
        {
            _cts?.Cancel();
            _serialPort?.Close();
            _readerTask?.Wait(TimeSpan.FromMilliseconds(200));
            OnStatus("Disconnected.");
        }
        finally
        {
            _readerTask = null;
            _cts?.Dispose();
            _cts = null;
            _serialPort?.Dispose();
            _serialPort = null;
        }
    }

    public void Dispose()
    {
        Disconnect();
    }

    private void OnStatus(string message)
    {
        StatusMessage?.Invoke(this, message);
    }

    private enum FrameState
    {
        WaitingForStart,
        WaitingForMs5,
        WaitingForLs5
    }
}

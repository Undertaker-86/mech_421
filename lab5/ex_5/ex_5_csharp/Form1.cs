using System.IO.Ports;

namespace StepperController;

public partial class Form1 : Form
{
    private const byte StartByte = 0xFF;
    private const byte CmdDcCw = 1;
    private const byte CmdDcCcw = 2;
    private const byte CmdStepperCw = 3;
    private const byte CmdStepperCcw = 4;
    private const byte CmdSingleStepCw = 5;
    private const byte CmdSingleStepCcw = 6;
    private const int MaxStepperSpeed = 1000;

    private SerialPort? _serial;

    public Form1()
    {
        InitializeComponent();
        RefreshPorts();
        UpdateVelocityLabel();
    }

    private void RefreshPorts()
    {
        var ports = SerialPort.GetPortNames()
            .OrderBy(p => p)
            .ToArray();

        portCombo.Items.Clear();
        portCombo.Items.AddRange(ports);

        if (ports.Length > 0 && portCombo.SelectedIndex < 0)
        {
            portCombo.SelectedIndex = 0;
            statusLabel.Text = "Select a COM port";
        }
        else if (ports.Length == 0)
        {
            statusLabel.Text = "No COM ports detected";
        }
    }

    private void ConnectButton_Click(object sender, EventArgs e)
    {
        if (_serial?.IsOpen == true)
        {
            Disconnect();
            return;
        }

        var portName = portCombo.Text.Trim();
        if (string.IsNullOrWhiteSpace(portName))
        {
            AppendLog("Pick or type a COM port first.");
            return;
        }

        try
        {
            _serial = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One)
            {
                Handshake = Handshake.None,
                DtrEnable = false,
                RtsEnable = false,
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            _serial.DataReceived += SerialOnDataReceived;
            _serial.Open();

            statusLabel.Text = $"Connected: {portName} @9600";
            connectButton.Text = "Disconnect";
            AppendLog($"Connected to {portName}");
            SendStepperVelocity(velocityTrackBar.Value);
        }
        catch (Exception ex)
        {
            AppendLog($"Connect failed: {ex.Message}");
            Disconnect();
        }
    }

    private void Disconnect()
    {
        if (_serial != null)
        {
            try
            {
                _serial.DataReceived -= SerialOnDataReceived;
                if (_serial.IsOpen)
                {
                    _serial.Close();
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
            finally
            {
                _serial.Dispose();
                _serial = null;
            }
        }

        statusLabel.Text = "Disconnected";
        connectButton.Text = "Connect";
    }

    private void SerialOnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            var sp = (SerialPort)sender;
            var buffer = new byte[sp.BytesToRead];
            var count = sp.Read(buffer, 0, buffer.Length);

            if (count > 0)
            {
                AppendLog($"RX: {BitConverter.ToString(buffer, 0, count)}");
            }
        }
        catch (Exception ex)
        {
            AppendLog($"RX error: {ex.Message}");
        }
    }

    private void AppendLog(string message)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string>(AppendLog), message);
            return;
        }

        logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }

    private void SendPacket(byte command, ushort data)
    {
        if (_serial?.IsOpen != true)
        {
            AppendLog("Not connected.");
            return;
        }

        var packet = new byte[5];
        packet[0] = StartByte;
        packet[1] = command;
        
        // Handle escaping for 0xFF
        // If data byte is 0xFF, send 0x00 and set corresponding flag bit
        byte dataHigh = (byte)((data >> 8) & 0xFF);
        byte dataLow = (byte)(data & 0xFF);
        byte flags = 0;

        if (dataHigh == 0xFF)
        {
            dataHigh = 0x00;
            flags |= 0x02; // Bit 1 for high byte
        }

        if (dataLow == 0xFF)
        {
            dataLow = 0x00;
            flags |= 0x01; // Bit 0 for low byte
        }

        packet[2] = dataHigh;
        packet[3] = dataLow;
        packet[4] = flags;

        try
        {
            _serial.Write(packet, 0, packet.Length);
            AppendLog($"TX: cmd={command}, data={data} (flags={flags:X2})");
        }
        catch (Exception ex)
        {
            AppendLog($"TX error: {ex.Message}");
        }
    }

    private void SendStepperVelocity(int sliderValue)
    {
        if (_serial?.IsOpen != true)
        {
            return;
        }

        var magnitude = (ushort)Math.Min(Math.Abs(sliderValue), MaxStepperSpeed);

        if (sliderValue == 0)
        {
            // Stop command - using direction 1 (CW) with speed 0
            SendPacket(CmdStepperCw, 0);
            return;
        }

        var command = sliderValue > 0 ? CmdStepperCw : CmdStepperCcw;
        SendPacket(command, magnitude);
    }

    private void VelocityTrackBar_ValueChanged(object sender, EventArgs e)
    {
        UpdateVelocityLabel();
        SendStepperVelocity(velocityTrackBar.Value);
    }

    private void StopButton_Click(object sender, EventArgs e)
    {
        velocityTrackBar.Value = 0;
        SendStepperVelocity(0);
    }

    private void StepCwButton_Click(object sender, EventArgs e)
    {
        // Single step CW: Command 3 (Stepper Control), Data 0
        // Wait, the firmware logic for single step was removed in the user's edit?
        // Let's check the firmware again.
        // The user removed case 5 and 6.
        // But how does command 3 with data 0 do a single step?
        // In stepper_control: if speed == 0 -> stepper_half_step().
        // So sending speed 0 triggers one half step?
        // Yes: "if (speed == 0) { ... stepper_half_step(); return; }"
        // So Command 3 (CW) with Data 0 -> Single Step CW.
        // Command 4 (CCW) with Data 0 -> Single Step CCW.
        
        SendPacket(CmdStepperCw, 0);
    }

    private void StepCcwButton_Click(object sender, EventArgs e)
    {
        SendPacket(CmdStepperCcw, 0);
    }

    private void RefreshButton_Click(object sender, EventArgs e)
    {
        RefreshPorts();
    }

    private void UpdateVelocityLabel()
    {
        velocityValueLabel.Text = $"{velocityTrackBar.Value} steps/s";
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        Disconnect();
    }
}

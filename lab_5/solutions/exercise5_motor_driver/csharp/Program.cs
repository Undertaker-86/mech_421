using System;
using System.IO.Ports;

namespace MotorDriverConsole;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: dotnet run -- <COM port>");
            return;
        }

        var portName = args[0];
        using var serial = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One)
        {
            NewLine = "\n"
        };

        try
        {
            serial.Open();
            Console.WriteLine($"Opened {portName}. Commands: d0/d1, p###, ?, q to quit.");
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line is null) continue;
                if (line.Trim().Equals("q", StringComparison.OrdinalIgnoreCase))
                    break;
                serial.WriteLine(line.Trim());

                System.Threading.Thread.Sleep(10);
                var response = serial.ReadExisting();
                if (!string.IsNullOrWhiteSpace(response))
                    Console.WriteLine(response.Replace("\r", ""));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Serial error: {ex.Message}");
        }
    }
}

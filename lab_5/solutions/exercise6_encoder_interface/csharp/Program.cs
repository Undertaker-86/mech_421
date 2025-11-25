using System;
using System.IO.Ports;
using System.Threading;

namespace EncoderReaderConsole;

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
            NewLine = "\n",
            ReadTimeout = 200
        };

        try
        {
            serial.Open();
            Console.WriteLine($"Opened {portName}. Commands: r=read once, s=stream, z=zero, q=quit.");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input is null) continue;
                input = input.Trim().ToLowerInvariant();
                if (input == "q") break;
                if (input == "r")
                {
                    serial.WriteLine("r");
                    Thread.Sleep(5);
                    Console.WriteLine(serial.ReadExisting().Replace("\r", ""));
                }
                else if (input == "z")
                {
                    serial.WriteLine("z");
                    Thread.Sleep(5);
                    Console.WriteLine(serial.ReadExisting().Replace("\r", ""));
                }
                else if (input == "s")
                {
                    Console.WriteLine("Streaming... press Enter to stop.");
                    while (!Console.KeyAvailable)
                    {
                        serial.WriteLine("r");
                        Thread.Sleep(20);
                        var resp = serial.ReadExisting().Replace("\r", "");
                        if (!string.IsNullOrWhiteSpace(resp))
                            Console.Write(resp);
                    }
                    if (Console.KeyAvailable) Console.ReadKey(true); // clear the key press
                }
                else
                {
                    Console.WriteLine("Unknown command.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Serial error: {ex.Message}");
        }
    }
}

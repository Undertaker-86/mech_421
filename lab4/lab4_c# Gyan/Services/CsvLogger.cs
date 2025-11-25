using System;
using System.Globalization;
using System.IO;
using DistanceMonitor.Models;

namespace DistanceMonitor.Services;

public sealed class CsvLogger : IDisposable
{
    private StreamWriter? _writer;
    private bool _headerWritten;

    public bool IsLogging => _writer is not null;

    public void Start(string filePath)
    {
        Stop();

        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");
        _writer = new StreamWriter(File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
            AutoFlush = true
        };
        _headerWritten = false;
    }

    public void WriteSample(SampleRecord sample)
    {
        if (_writer is null)
        {
            return;
        }

        if (!_headerWritten)
        {
            _writer.WriteLine("Timestamp,Raw,Voltage,Distance");
            _headerWritten = true;
        }

        string line = string.Format(CultureInfo.InvariantCulture,
            "{0:o},{1},{2:F4},{3:F4}",
            sample.Timestamp,
            sample.RawValue,
            sample.Voltage,
            sample.Distance);

        _writer.WriteLine(line);
    }

    public void Stop()
    {
        _writer?.Dispose();
        _writer = null;
        _headerWritten = false;
    }

    public void Dispose()
    {
        Stop();
    }
}

using System;

namespace DistanceMonitor.Models;

/// <summary>
/// Represents a single digitized sample from the MSP430 stream.
/// </summary>
public sealed class SampleRecord
{
    public SampleRecord(DateTime timestamp, int rawValue, double voltage, double distance)
    {
        Timestamp = timestamp;
        RawValue = rawValue;
        Voltage = voltage;
        Distance = distance;
    }

    public DateTime Timestamp { get; }
    public int RawValue { get; }
    public double Voltage { get; }
    public double Distance { get; }
}

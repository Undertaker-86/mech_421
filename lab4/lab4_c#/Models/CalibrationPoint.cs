using System;

namespace DistanceMonitor.Models;

public sealed class CalibrationPoint
{
    public CalibrationPoint(DateTime timestamp, int rawValue, double voltage, double measuredDistance)
    {
        Timestamp = timestamp;
        RawValue = rawValue;
        Voltage = voltage;
        MeasuredDistance = measuredDistance;
    }

    public DateTime Timestamp { get; }
    public int RawValue { get; }
    public double Voltage { get; }
    public double MeasuredDistance { get; }
}

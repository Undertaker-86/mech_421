using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DistanceSensorApp.Models;

namespace DistanceSensorApp.Services
{
    public class NoiseAnalysisService
    {
        private List<NoiseMeasurement> _measurements;
        private readonly object _lock = new object();

        public int Count => _measurements.Count;

        public NoiseAnalysisService()
        {
            _measurements = new List<NoiseMeasurement>();
        }

        public void AddMeasurement(NoiseMeasurement measurement)
        {
            lock (_lock)
            {
                _measurements.Add(measurement);
            }
        }

        public List<NoiseMeasurement> GetAllMeasurements()
        {
            lock (_lock)
            {
                return new List<NoiseMeasurement>(_measurements);
            }
        }

        public void RemoveMeasurement(int index)
        {
            lock (_lock)
            {
                if (index >= 0 && index < _measurements.Count)
                {
                    _measurements.RemoveAt(index);
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _measurements.Clear();
            }
        }

        public NoiseMeasurement CalculateNoiseMeasurement(List<SensorReading> readings, string positionLabel)
        {
            if (readings == null || readings.Count == 0)
            {
                throw new ArgumentException("No readings available for noise analysis.");
            }

            // Calculate statistics for distance
            double[] distances = readings.Select(r => r.Distance).ToArray();
            double meanDistance = distances.Average();
            double sumSquares = distances.Sum(d => Math.Pow(d - meanDistance, 2));
            double stdDev = Math.Sqrt(sumSquares / distances.Length);

            // Calculate mean ADC
            double meanAdc = readings.Average(r => r.AdcValue);

            return new NoiseMeasurement(
                positionLabel,
                meanDistance,
                stdDev,
                readings.Count,
                meanAdc
            );
        }

        public void ExportToCsv(string filePath)
        {
            lock (_lock)
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(NoiseMeasurement.GetCsvHeader());
                    foreach (var measurement in _measurements)
                    {
                        writer.WriteLine(measurement.ToString());
                    }
                }
            }
        }

        public string GetComparisonSummary()
        {
            lock (_lock)
            {
                if (_measurements.Count == 0)
                    return "No measurements available for comparison.";

                var summary = "=== NOISE ANALYSIS COMPARISON ===\n\n";
                
                foreach (var measurement in _measurements)
                {
                    summary += $"Position: {measurement.PositionLabel}\n";
                    summary += $"  Mean Distance: {measurement.MeanDistance:F4} cm\n";
                    summary += $"  Mean ADC: {measurement.MeanAdcValue:F2}\n";
                    summary += $"  RMS Noise (Std Dev): {measurement.StandardDeviation:F6} cm\n";
                    summary += $"  Sample Count: {measurement.SampleCount}\n";
                    summary += $"  Timestamp: {measurement.Timestamp:yyyy-MM-dd HH:mm:ss}\n\n";
                }

                // Find min and max noise
                var minNoise = _measurements.MinBy(m => m.StandardDeviation);
                var maxNoise = _measurements.MaxBy(m => m.StandardDeviation);

                if (minNoise != null && maxNoise != null)
                {
                    summary += "=== SUMMARY ===\n";
                    summary += $"Lowest RMS Noise: {minNoise.StandardDeviation:F6} cm at {minNoise.PositionLabel}\n";
                    summary += $"Highest RMS Noise: {maxNoise.StandardDeviation:F6} cm at {maxNoise.PositionLabel}\n";
                    
                    if (minNoise != maxNoise)
                    {
                        double ratio = maxNoise.StandardDeviation / minNoise.StandardDeviation;
                        summary += $"Noise Ratio (Max/Min): {ratio:F2}x\n";
                    }
                }

                return summary;
            }
        }
    }
}

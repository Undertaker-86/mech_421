using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DistanceSensorApp.Models;

namespace DistanceSensorApp.Services
{
    public class DataLogger
    {
        private List<SensorReading> _readings;
        private readonly object _lock = new object();

        public int Count => _readings.Count;

        public DataLogger()
        {
            _readings = new List<SensorReading>();
        }

        public void AddReading(SensorReading reading)
        {
            lock (_lock)
            {
                _readings.Add(reading);
            }
        }

        public List<SensorReading> GetAllReadings()
        {
            lock (_lock)
            {
                return new List<SensorReading>(_readings);
            }
        }

        public List<SensorReading> GetReadings(int count)
        {
            lock (_lock)
            {
                return _readings.Skip(Math.Max(0, _readings.Count - count)).ToList();
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _readings.Clear();
            }
        }

        public void ExportToCsv(string filePath)
        {
            lock (_lock)
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(SensorReading.GetCsvHeader());
                    foreach (var reading in _readings)
                    {
                        writer.WriteLine(reading.ToString());
                    }
                }
            }
        }

        public (double min, double max, double avg, double stdDev) GetStatistics(bool forAdc = true)
        {
            lock (_lock)
            {
                if (_readings.Count == 0)
                    return (0, 0, 0, 0);

                double[] values = forAdc 
                    ? _readings.Select(r => (double)r.AdcValue).ToArray()
                    : _readings.Select(r => r.Distance).ToArray();

                double min = values.Min();
                double max = values.Max();
                double avg = values.Average();
                
                // Calculate standard deviation
                double sumSquares = values.Sum(v => Math.Pow(v - avg, 2));
                double stdDev = Math.Sqrt(sumSquares / values.Length);

                return (min, max, avg, stdDev);
            }
        }

        public double GetRmsNoise(DateTime startTime, TimeSpan duration)
        {
            lock (_lock)
            {
                var filteredReadings = _readings
                    .Where(r => r.Timestamp >= startTime && r.Timestamp <= startTime.Add(duration))
                    .Select(r => r.Distance)
                    .ToArray();

                if (filteredReadings.Length == 0)
                    return 0;

                double avg = filteredReadings.Average();
                double sumSquares = filteredReadings.Sum(v => Math.Pow(v - avg, 2));
                return Math.Sqrt(sumSquares / filteredReadings.Length);
            }
        }
    }
}

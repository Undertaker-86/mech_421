using System;
using System.Collections.Generic;
using System.Linq;

namespace RyanSensorApp.Models
{
    public enum TestPosition
    {
        MiddleRange,
        NearExtreme,
        FarExtreme,
        Custom
    }

    public class NoiseTestResult
    {
        public int TestNumber { get; set; }
        public TestPosition Position { get; set; }
        public string PositionDescription { get; set; }
        public DateTime TestDateTime { get; set; }
        public double DurationSeconds { get; set; }
        public List<double> DistanceReadings { get; set; }
        public List<int> AdcReadings { get; set; }
        
        // Calculated statistics
        public double MeanDistance { get; set; }
        public double StandardDeviation { get; set; }
        public double MinDistance { get; set; }
        public double MaxDistance { get; set; }
        public int SampleCount { get; set; }
        public double MeanAdc { get; set; }

        public NoiseTestResult()
        {
            DistanceReadings = new List<double>();
            AdcReadings = new List<int>();
            TestDateTime = DateTime.Now;
            PositionDescription = string.Empty;
        }

        public void CalculateStatistics()
        {
            if (DistanceReadings.Count == 0)
            {
                MeanDistance = 0;
                StandardDeviation = 0;
                MinDistance = 0;
                MaxDistance = 0;
                SampleCount = 0;
                MeanAdc = 0;
                return;
            }

            SampleCount = DistanceReadings.Count;
            MeanDistance = DistanceReadings.Average();
            MinDistance = DistanceReadings.Min();
            MaxDistance = DistanceReadings.Max();
            MeanAdc = AdcReadings.Average();

            // Calculate standard deviation (RMS noise)
            if (DistanceReadings.Count > 1)
            {
                double sumSquaredDiff = DistanceReadings.Sum(x => Math.Pow(x - MeanDistance, 2));
                StandardDeviation = Math.Sqrt(sumSquaredDiff / DistanceReadings.Count);
            }
            else
            {
                StandardDeviation = 0;
            }
        }

        public string GetPositionString()
        {
            return Position switch
            {
                TestPosition.MiddleRange => "Middle Range",
                TestPosition.NearExtreme => "Near Extreme (Close)",
                TestPosition.FarExtreme => "Far Extreme (Far)",
                TestPosition.Custom => PositionDescription,
                _ => "Unknown"
            };
        }

        public string GetSummary()
        {
            return $"Test #{TestNumber} - {GetPositionString()}\n" +
                   $"Duration: {DurationSeconds:F1}s, Samples: {SampleCount}\n" +
                   $"Mean: {MeanDistance:F4} cm\n" +
                   $"Std Dev (RMS): {StandardDeviation:F4} cm\n" +
                   $"Range: {MinDistance:F4} - {MaxDistance:F4} cm";
        }
    }
}

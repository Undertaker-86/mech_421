using System;

namespace DistanceSensorApp.Models
{
    public class NoiseMeasurement
    {
        public DateTime Timestamp { get; set; }
        public string PositionLabel { get; set; }
        public double MeanDistance { get; set; }
        public double StandardDeviation { get; set; }
        public int SampleCount { get; set; }
        public double MeanAdcValue { get; set; }

        public NoiseMeasurement()
        {
            Timestamp = DateTime.Now;
            PositionLabel = string.Empty;
        }

        public NoiseMeasurement(string positionLabel, double meanDistance, double standardDeviation, int sampleCount, double meanAdcValue)
        {
            Timestamp = DateTime.Now;
            PositionLabel = positionLabel;
            MeanDistance = meanDistance;
            StandardDeviation = standardDeviation;
            SampleCount = sampleCount;
            MeanAdcValue = meanAdcValue;
        }

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss},{PositionLabel},{MeanAdcValue:F2},{MeanDistance:F4},{StandardDeviation:F6},{SampleCount}";
        }

        public static string GetCsvHeader()
        {
            return "Timestamp,Position Label,Mean ADC,Mean Distance (cm),Std Dev (RMS Noise) (cm),Sample Count";
        }
    }
}

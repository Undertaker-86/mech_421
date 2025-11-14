using System;

namespace DistanceSensorApp.Models
{
    public class SensorReading
    {
        public DateTime Timestamp { get; set; }
        public int AdcValue { get; set; }  // 0-1023 (10-bit)
        public double Distance { get; set; }  // Converted distance in cm
        public bool IsInRange { get; set; }

        public SensorReading()
        {
            Timestamp = DateTime.Now;
        }

        public SensorReading(int adcValue, double distance, bool isInRange)
        {
            Timestamp = DateTime.Now;
            AdcValue = adcValue;
            Distance = distance;
            IsInRange = isInRange;
        }

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff},{AdcValue},{Distance:F3},{IsInRange}";
        }

        public static string GetCsvHeader()
        {
            return "Timestamp,ADC Value,Distance (cm),In Range";
        }
    }
}

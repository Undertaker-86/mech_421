using System;

namespace RyanSensorApp.Models
{
    public class SensorReading
    {
        public DateTime Timestamp { get; set; }
        public int AdcValue { get; set; }  // 0-1023 (10-bit)
        public double Voltage { get; set; }  // 0-3.3V
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
            Voltage = AdcValue * 3.3 / 1023.0;  // Calculate voltage from ADC
            Distance = distance;
            IsInRange = isInRange;
        }

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff},{AdcValue},{Voltage:F4},{Distance:F3},{IsInRange}";
        }

        public static string GetCsvHeader()
        {
            return "Timestamp,ADC Value,Voltage (V),Distance (cm),In Range";
        }
    }
}

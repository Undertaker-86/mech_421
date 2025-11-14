using System;

namespace RyanSensorApp.Models
{
    public class CalibrationPoint
    {
        public double Distance { get; set; }  // in cm
        public int AdcValue { get; set; }      // 0-1023

        public CalibrationPoint()
        {
        }

        public CalibrationPoint(double distance, int adcValue)
        {
            Distance = distance;
            AdcValue = adcValue;
        }

        public override string ToString()
        {
            return $"Distance: {Distance:F2} cm, ADC: {AdcValue}";
        }
    }
}

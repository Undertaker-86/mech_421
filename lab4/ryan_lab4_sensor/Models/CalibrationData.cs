using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RyanSensorApp.Models
{
    public enum FitType
    {
        Linear,
        Polynomial2,
        Polynomial3,
        Power,
        Inverse
    }

    public class CalibrationData
    {
        public List<CalibrationPoint> Points { get; set; }
        public FitType FitType { get; set; }
        public double[] Coefficients { get; set; }
        public double RSquared { get; set; }
        public string Equation { get; set; }
        public int MinAdcThreshold { get; set; }  // ADC value below this = too far
        public int MaxAdcThreshold { get; set; }  // ADC value above this = too close

        public CalibrationData()
        {
            Points = new List<CalibrationPoint>();
            FitType = FitType.Polynomial2;
            Coefficients = Array.Empty<double>();
            Equation = string.Empty;
            MinAdcThreshold = 200;   // Default: too far threshold
            MaxAdcThreshold = 800;   // Default: too close threshold
        }

        public double ConvertAdcToDistance(int adcValue)
        {
            if (Coefficients == null || Coefficients.Length == 0)
                return 0;

            double x = adcValue;
            double distance = 0;

            switch (FitType)
            {
                case FitType.Linear:
                    // y = mx + b
                    if (Coefficients.Length >= 2)
                        distance = Coefficients[0] * x + Coefficients[1];
                    break;

                case FitType.Polynomial2:
                    // y = ax^2 + bx + c
                    if (Coefficients.Length >= 3)
                        distance = Coefficients[0] * x * x + Coefficients[1] * x + Coefficients[2];
                    break;

                case FitType.Polynomial3:
                    // y = ax^3 + bx^2 + cx + d
                    if (Coefficients.Length >= 4)
                        distance = Coefficients[0] * x * x * x + Coefficients[1] * x * x + 
                                   Coefficients[2] * x + Coefficients[3];
                    break;

                case FitType.Power:
                    // y = a * x^b
                    if (Coefficients.Length >= 2 && x > 0)
                        distance = Coefficients[0] * Math.Pow(x, Coefficients[1]);
                    break;

                case FitType.Inverse:
                    // y = a / (x - b) + c
                    if (Coefficients.Length >= 3 && Math.Abs(x - Coefficients[1]) > 1e-6)
                        distance = Coefficients[0] / (x - Coefficients[1]) + Coefficients[2];
                    break;
            }

            return distance;
        }

        public bool IsInRange(int adcValue)
        {
            return adcValue >= MinAdcThreshold && adcValue <= MaxAdcThreshold;
        }

        public string GetRangeStatus(int adcValue)
        {
            if (adcValue > MaxAdcThreshold)
                return "Too Close";
            else if (adcValue < MinAdcThreshold)
                return "Too Far";
            else
                return "In Range";
        }
    }
}

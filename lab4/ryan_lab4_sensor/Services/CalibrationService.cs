using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RyanSensorApp.Models;
using MathNet.Numerics;
using MathNet.Numerics.LinearRegression;
using Newtonsoft.Json;

namespace RyanSensorApp.Services
{
    public class CalibrationService
    {
        public CalibrationData PerformCalibration(List<CalibrationPoint> points, FitType fitType)
        {
            if (points == null || points.Count < 2)
            {
                throw new ArgumentException("At least 2 calibration points are required.");
            }

            var calibration = new CalibrationData
            {
                Points = new List<CalibrationPoint>(points),
                FitType = fitType
            };

            // Extract x (ADC) and y (Distance) values
            double[] xData = points.Select(p => (double)p.AdcValue).ToArray();
            double[] yData = points.Select(p => p.Distance).ToArray();

            try
            {
                switch (fitType)
                {
                    case FitType.Linear:
                        calibration.Coefficients = FitLinear(xData, yData, out double rSquaredLinear);
                        calibration.RSquared = rSquaredLinear;
                        calibration.Equation = $"y = {calibration.Coefficients[0]:F6}x + {calibration.Coefficients[1]:F6}";
                        break;

                    case FitType.Polynomial2:
                        calibration.Coefficients = FitPolynomial(xData, yData, 2, out double rSquared2);
                        calibration.RSquared = rSquared2;
                        calibration.Equation = $"y = {calibration.Coefficients[0]:E3}x² + {calibration.Coefficients[1]:F6}x + {calibration.Coefficients[2]:F6}";
                        break;

                    case FitType.Polynomial3:
                        calibration.Coefficients = FitPolynomial(xData, yData, 3, out double rSquared3);
                        calibration.RSquared = rSquared3;
                        calibration.Equation = $"y = {calibration.Coefficients[0]:E3}x³ + {calibration.Coefficients[1]:E3}x² + {calibration.Coefficients[2]:F6}x + {calibration.Coefficients[3]:F6}";
                        break;

                    case FitType.Power:
                        calibration.Coefficients = FitPower(xData, yData, out double rSquaredPower);
                        calibration.RSquared = rSquaredPower;
                        calibration.Equation = $"y = {calibration.Coefficients[0]:F6} × x^{calibration.Coefficients[1]:F6}";
                        break;

                    case FitType.Inverse:
                        calibration.Coefficients = FitInverse(xData, yData, out double rSquaredInv);
                        calibration.RSquared = rSquaredInv;
                        calibration.Equation = $"y = {calibration.Coefficients[0]:F6} / (x - {calibration.Coefficients[1]:F6}) + {calibration.Coefficients[2]:F6}";
                        break;
                }

                // Set min/max ADC values based on calibration points
                calibration.MinAdcValue = (int)xData.Min();
                calibration.MaxAdcValue = (int)xData.Max();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Curve fitting failed: {ex.Message}", ex);
            }

            return calibration;
        }

        private double[] FitLinear(double[] x, double[] y, out double rSquared)
        {
            var (slope, intercept) = SimpleRegression.Fit(x, y);
            rSquared = CalculateRSquared(x, y, new[] { slope, intercept }, FitType.Linear);
            return new[] { slope, intercept };
        }

        private double[] FitPolynomial(double[] x, double[] y, int order, out double rSquared)
        {
            double[] coefficients = Fit.Polynomial(x, y, order);
            // MathNet returns coefficients in ascending order (c0 + c1*x + c2*x^2 + ...)
            // We want descending order for our formula (a*x^n + b*x^(n-1) + ...)
            Array.Reverse(coefficients);
            rSquared = CalculateRSquared(x, y, coefficients, order == 2 ? FitType.Polynomial2 : FitType.Polynomial3);
            return coefficients;
        }

        private double[] FitPower(double[] x, double[] y, out double rSquared)
        {
            // Power fit: y = a * x^b
            // Transform to linear: ln(y) = ln(a) + b*ln(x)
            if (x.Any(val => val <= 0) || y.Any(val => val <= 0))
            {
                throw new ArgumentException("Power fit requires all positive values.");
            }

            double[] lnX = x.Select(val => Math.Log(val)).ToArray();
            double[] lnY = y.Select(val => Math.Log(val)).ToArray();

            var (b, lnA) = SimpleRegression.Fit(lnX, lnY);
            double a = Math.Exp(lnA);

            rSquared = CalculateRSquared(x, y, new[] { a, b }, FitType.Power);
            return new[] { a, b };
        }

        private double[] FitInverse(double[] x, double[] y, out double rSquared)
        {
            // Inverse fit: y = a / (x - b) + c
            // We'll use a simplified approach with b=0: y = a/x + c
            // Transform to linear: y = a*(1/x) + c

            double[] invX = x.Select(val => 1.0 / val).ToArray();
            var (a, c) = SimpleRegression.Fit(invX, y);
            double b = 0;

            rSquared = CalculateRSquared(x, y, new[] { a, b, c }, FitType.Inverse);
            return new[] { a, b, c };
        }

        private double CalculateRSquared(double[] x, double[] y, double[] coefficients, FitType fitType)
        {
            double meanY = y.Average();
            double ssTotal = y.Sum(yi => Math.Pow(yi - meanY, 2));
            double ssResidual = 0;

            for (int i = 0; i < x.Length; i++)
            {
                double predicted = 0;
                switch (fitType)
                {
                    case FitType.Linear:
                        predicted = coefficients[0] * x[i] + coefficients[1];
                        break;
                    case FitType.Polynomial2:
                        predicted = coefficients[0] * x[i] * x[i] + coefficients[1] * x[i] + coefficients[2];
                        break;
                    case FitType.Polynomial3:
                        predicted = coefficients[0] * Math.Pow(x[i], 3) + coefficients[1] * Math.Pow(x[i], 2) +
                                    coefficients[2] * x[i] + coefficients[3];
                        break;
                    case FitType.Power:
                        predicted = coefficients[0] * Math.Pow(x[i], coefficients[1]);
                        break;
                    case FitType.Inverse:
                        predicted = coefficients[0] / (x[i] - coefficients[1]) + coefficients[2];
                        break;
                }
                ssResidual += Math.Pow(y[i] - predicted, 2);
            }

            return 1 - (ssResidual / ssTotal);
        }

        public void SaveCalibration(CalibrationData calibration, string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(calibration, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to save calibration: {ex.Message}", ex);
            }
        }

        public CalibrationData LoadCalibration(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                var calibration = JsonConvert.DeserializeObject<CalibrationData>(json);
                if (calibration == null)
                {
                    throw new InvalidDataException("Failed to deserialize calibration data.");
                }
                return calibration;
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to load calibration: {ex.Message}", ex);
            }
        }
    }
}

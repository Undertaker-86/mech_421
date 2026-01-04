using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RyanSensorApp.Models;

namespace RyanSensorApp.Services
{
    public class NoiseAnalysisService
    {
        private List<NoiseTestResult> _testResults;
        private int _nextTestNumber;

        public NoiseAnalysisService()
        {
            _testResults = new List<NoiseTestResult>();
            _nextTestNumber = 1;
        }

        public List<NoiseTestResult> GetAllTests()
        {
            return new List<NoiseTestResult>(_testResults);
        }

        public int GetTestCount()
        {
            return _testResults.Count;
        }

        public NoiseTestResult CreateNewTest(TestPosition position, string customDescription = "")
        {
            var test = new NoiseTestResult
            {
                TestNumber = _nextTestNumber++,
                Position = position,
                PositionDescription = customDescription,
                TestDateTime = DateTime.Now
            };
            return test;
        }

        public void SaveTest(NoiseTestResult test)
        {
            test.CalculateStatistics();
            _testResults.Add(test);
        }

        public void DeleteTest(int testNumber)
        {
            _testResults.RemoveAll(t => t.TestNumber == testNumber);
        }

        public void ClearAllTests()
        {
            _testResults.Clear();
            _nextTestNumber = 1;
        }

        public string GetComparisonSummary()
        {
            if (_testResults.Count == 0)
                return "No tests available for comparison.";

            var sb = new StringBuilder();
            sb.AppendLine("=== NOISE ANALYSIS COMPARISON ===\n");

            // Group by position
            var middleTests = _testResults.Where(t => t.Position == TestPosition.MiddleRange).ToList();
            var nearTests = _testResults.Where(t => t.Position == TestPosition.NearExtreme).ToList();
            var farTests = _testResults.Where(t => t.Position == TestPosition.FarExtreme).ToList();

            if (middleTests.Any())
            {
                sb.AppendLine("MIDDLE RANGE:");
                foreach (var test in middleTests)
                {
                    sb.AppendLine($"  Test #{test.TestNumber}: Mean={test.MeanDistance:F4} cm, RMS Noise={test.StandardDeviation:F4} cm");
                }
                sb.AppendLine($"  Average RMS Noise: {middleTests.Average(t => t.StandardDeviation):F4} cm\n");
            }

            if (nearTests.Any())
            {
                sb.AppendLine("NEAR EXTREME (Close):");
                foreach (var test in nearTests)
                {
                    sb.AppendLine($"  Test #{test.TestNumber}: Mean={test.MeanDistance:F4} cm, RMS Noise={test.StandardDeviation:F4} cm");
                }
                sb.AppendLine($"  Average RMS Noise: {nearTests.Average(t => t.StandardDeviation):F4} cm\n");
            }

            if (farTests.Any())
            {
                sb.AppendLine("FAR EXTREME:");
                foreach (var test in farTests)
                {
                    sb.AppendLine($"  Test #{test.TestNumber}: Mean={test.MeanDistance:F4} cm, RMS Noise={test.StandardDeviation:F4} cm");
                }
                sb.AppendLine($"  Average RMS Noise: {farTests.Average(t => t.StandardDeviation):F4} cm\n");
            }

            // Comparison analysis
            if (middleTests.Any() && (nearTests.Any() || farTests.Any()))
            {
                double middleRms = middleTests.Average(t => t.StandardDeviation);
                sb.AppendLine("=== COMPARISON ===");

                if (nearTests.Any())
                {
                    double nearRms = nearTests.Average(t => t.StandardDeviation);
                    double nearDiff = nearRms - middleRms;
                    double nearRatio = middleRms > 0 ? nearRms / middleRms : 0;
                    sb.AppendLine($"Near Extreme vs Middle: {nearDiff:+0.0000;-0.0000} cm ({nearRatio:F2}x)");
                }

                if (farTests.Any())
                {
                    double farRms = farTests.Average(t => t.StandardDeviation);
                    double farDiff = farRms - middleRms;
                    double farRatio = middleRms > 0 ? farRms / middleRms : 0;
                    sb.AppendLine($"Far Extreme vs Middle: {farDiff:+0.0000;-0.0000} cm ({farRatio:F2}x)");
                }
            }

            return sb.ToString();
        }

        public void ExportToCSV(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("Test Number,Position,Date/Time,Duration (s),Samples,Mean Distance (cm),Std Dev (RMS) (cm),Min Distance (cm),Max Distance (cm),Mean ADC");

                // Write data
                foreach (var test in _testResults)
                {
                    writer.WriteLine($"{test.TestNumber}," +
                                   $"{test.GetPositionString()}," +
                                   $"{test.TestDateTime:yyyy-MM-dd HH:mm:ss}," +
                                   $"{test.DurationSeconds:F2}," +
                                   $"{test.SampleCount}," +
                                   $"{test.MeanDistance:F6}," +
                                   $"{test.StandardDeviation:F6}," +
                                   $"{test.MinDistance:F6}," +
                                   $"{test.MaxDistance:F6}," +
                                   $"{test.MeanAdc:F2}");
                }
            }
        }

        public void ExportDetailedData(string filePath, NoiseTestResult test)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write header info
                writer.WriteLine($"Test #{test.TestNumber} - {test.GetPositionString()}");
                writer.WriteLine($"Date/Time: {test.TestDateTime:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Duration: {test.DurationSeconds:F2} seconds");
                writer.WriteLine($"Samples: {test.SampleCount}");
                writer.WriteLine($"Mean Distance: {test.MeanDistance:F6} cm");
                writer.WriteLine($"Standard Deviation (RMS Noise): {test.StandardDeviation:F6} cm");
                writer.WriteLine($"Min Distance: {test.MinDistance:F6} cm");
                writer.WriteLine($"Max Distance: {test.MaxDistance:F6} cm");
                writer.WriteLine();
                writer.WriteLine("Sample #,ADC Value,Distance (cm)");

                // Write all samples
                for (int i = 0; i < test.DistanceReadings.Count; i++)
                {
                    writer.WriteLine($"{i + 1},{test.AdcReadings[i]},{test.DistanceReadings[i]:F6}");
                }
            }
        }

        public Dictionary<TestPosition, double> GetAverageRMSByPosition()
        {
            var result = new Dictionary<TestPosition, double>();

            var positions = new[] { TestPosition.MiddleRange, TestPosition.NearExtreme, TestPosition.FarExtreme };

            foreach (var pos in positions)
            {
                var tests = _testResults.Where(t => t.Position == pos).ToList();
                if (tests.Any())
                {
                    result[pos] = tests.Average(t => t.StandardDeviation);
                }
            }

            return result;
        }
    }
}

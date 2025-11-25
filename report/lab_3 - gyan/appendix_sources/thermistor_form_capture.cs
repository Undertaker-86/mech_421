        private CaptureResult AnalyzeCapture(IList<CaptureSample> samples, string label, string captureFile)
        {
            List<CaptureSample> ordered = new List<CaptureSample>(samples);
            ordered.Sort(delegate (CaptureSample a, CaptureSample b)
            {
                return a.Seconds.CompareTo(b.Seconds);
            });

            if (ordered.Count == 0)
            {
                return new CaptureResult(label, 0.0, double.NaN, double.NaN, double.NaN, double.NaN,
                    double.NaN, captureFile, "No samples captured.");
            }

            int lastIndex = ordered.Count - 1;
            double duration = ordered[lastIndex].Seconds - ordered[0].Seconds;
            double startTemp = ordered[0].Temperature;

            double finalTemp;
            if (ordered.Count >= 6)
            {
                double sum = 0.0;
                int count = 0;
                int startIndex = Math.Max(ordered.Count - 6, 0);
                for (int i = startIndex; i < ordered.Count; i++)
                {
                    sum += ordered[i].Temperature;
                    count++;
                }
                finalTemp = count > 0 ? sum / count : ordered[lastIndex].Temperature;
            }
            else
            {
                finalTemp = ordered[lastIndex].Temperature;
            }

            double delta = finalTemp - startTemp;

            List<string> notes = new List<string>();
            if (Math.Abs(delta) < 0.5)
            {
                notes.Add("Delta-T too small for reliable fit.");
            }

            double tau63 = EstimateTauBy63Percent(ordered, startTemp, finalTemp);
            if (double.IsNaN(tau63))
            {
                notes.Add("63% threshold not reached.");
            }

            RegressionResult regression = EstimateTauByRegression(ordered, startTemp, finalTemp);
            if (double.IsNaN(regression.TauFit) || double.IsNaN(regression.FitR2))
            {
                notes.Add("Regression fit failed.");
            }

            string notesCombined = string.Join(" ", notes.ToArray()).Trim();

            return new CaptureResult(label, duration, startTemp, finalTemp, tau63, regression.TauFit,
                regression.FitR2, captureFile, notesCombined);
        }

        private string SaveCaptureCsv(IEnumerable<CaptureSample> samples, string scenario)
        {
            try
            {
                string capturesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "captures");
                Directory.CreateDirectory(capturesDir);

                string sanitized = string.Join("_", scenario.Split(Path.GetInvalidFileNameChars()))
                    .Replace(' ', '_');
                while (sanitized.Contains("__"))
                {
                    sanitized = sanitized.Replace("__", "_");
                }
                sanitized = sanitized.Trim('_');
                if (string.IsNullOrEmpty(sanitized))
                {
                    sanitized = "capture";
                }

                string filePath = Path.Combine(capturesDir,
                    string.Format("capture_{0:yyyyMMdd_HHmmss}_{1}.csv", DateTime.Now, sanitized));

                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    writer.WriteLine("seconds,temp_comp_c");
                    List<CaptureSample> ordered = samples.OrderBy(s => s.Seconds).ToList();
                    for (int i = 0; i < ordered.Count; i++)
                    {
                        CaptureSample sample = ordered[i];
                        writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                            "{0:F3},{1:F3}", sample.Seconds, sample.Temperature));
                    }
                }

                return filePath;
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action(delegate
                {
                    AppendLog("Capture CSV save failed: " + ex.Message);
                }));
                return string.Empty;
            }
        }

        private static double EstimateTauBy63Percent(IList<CaptureSample> samples,
            double startTemp, double finalTemp)
        {
            double step = finalTemp - startTemp;
            if (Math.Abs(step) < 0.5)
            {
                return double.NaN;
            }

            double target = finalTemp - (finalTemp - startTemp) * Math.Exp(-1.0);
            bool heating = step > 0;

            for (int i = 0; i < samples.Count; i++)
            {
                CaptureSample sample = samples[i];
                if (heating && sample.Temperature >= target)
                {
                    return sample.Seconds;
                }

                if (!heating && sample.Temperature <= target)
                {
                    return sample.Seconds;
                }
            }

            return double.NaN;
        }

        private static RegressionResult EstimateTauByRegression(
            IList<CaptureSample> samples, double startTemp, double finalTemp)
        {
            double delta = finalTemp - startTemp;
            if (Math.Abs(delta) < 0.5)
            {
                return new RegressionResult(double.NaN, double.NaN);
            }

            List<double> times = new List<double>();
            List<double> lnValues = new List<double>();
            for (int i = 0; i < samples.Count; i++)
            {
                CaptureSample sample = samples[i];
                if (sample.Seconds <= 0.05)
                {
                    continue;
                }

                double diff = Math.Abs(finalTemp - sample.Temperature);
                if (diff <= 1e-3)
                {
                    continue;
                }

                if (diff > Math.Abs(delta))
                {
                    continue;
                }

                if (diff <= Math.Abs(delta) * 0.02)
                {
                    continue;
                }

                times.Add(sample.Seconds);
                lnValues.Add(Math.Log(diff));
            }

            if (times.Count < 5)
            {
                return new RegressionResult(double.NaN, double.NaN);
            }

            int n = times.Count;
            double sumX = 0.0;
            double sumY = 0.0;
            double sumXY = 0.0;
            double sumX2 = 0.0;
            for (int i = 0; i < n; i++)
            {
                double x = times[i];
                double y = lnValues[i];
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            double denominator = n * sumX2 - sumX * sumX;
            if (Math.Abs(denominator) < 1e-12)
            {
                return new RegressionResult(double.NaN, double.NaN);
            }

            double slope = (n * sumXY - sumX * sumY) / denominator;
            double intercept = (sumY - slope * sumX) / n;

            double tauFit = double.NaN;
            if (slope < 0.0)
            {
                tauFit = -1.0 / slope;
            }

            double meanY = sumY / n;
            double ssTot = 0.0;
            double ssRes = 0.0;
            for (int i = 0; i < n; i++)
            {
                double x = times[i];
                double y = lnValues[i];
                double est = slope * x + intercept;
                ssTot += Math.Pow(y - meanY, 2);
                ssRes += Math.Pow(y - est, 2);
            }

            double r2 = ssTot <= 1e-12 ? double.NaN : 1.0 - (ssRes / ssTot);
            return new RegressionResult(tauFit, r2);
        }

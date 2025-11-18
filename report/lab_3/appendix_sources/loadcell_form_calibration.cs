        private void btnAddCalibrationPoint_Click(object sender, EventArgs e)
        {
            if (!_hasSample)
            {
                MessageBox.Show(this, "No sensor samples yet. Wait until readings appear before capturing calibration data.", "Calibration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            double knownMass = (double)numKnownMass.Value;
            double avgRaw;
            double stdDev;
            bool isStable;

            lock (_sampleLock)
            {
                avgRaw = _rawRollingQueue.Count > 0 ? _rawRollingSum / _rawRollingQueue.Count : _lastRaw;
                stdDev = _lastStdDev;
                isStable = _lastStable;
            }

            if (!isStable)
            {
                DialogResult response = MessageBox.Show(
                    this,
                    $"Current measurement is unstable (std dev {stdDev:F4} kg). Capture anyway?",
                    "Unstable measurement",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (response != DialogResult.Yes)
                {
                    return;
                }
            }

            var point = new CalibrationPoint(avgRaw, knownMass);
            _calibrationPoints.Add(point);

            UpdateCalibrationListDisplay();
            RecomputeCalibration();
        }

        private void btnClearCalibration_Click(object sender, EventArgs e)
        {
            if (_calibrationPoints.Count == 0)
            {
                return;
            }

            DialogResult result = MessageBox.Show(this, "Clear all calibration points?", "Clear calibration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }

            _calibrationPoints.Clear();

            lock (_sampleLock)
            {
                _scaleSlope = 0;
                _scaleIntercept = 0;
            }

            UpdateCalibrationListDisplay();
            UpdateCalibrationSummaryDisplay();
        }

        private void UpdateCalibrationListDisplay()
        {
            lstCalibration.BeginUpdate();
            lstCalibration.Items.Clear();

            foreach (CalibrationPoint point in _calibrationPoints)
            {
                string entry = string.Format(CultureInfo.InvariantCulture, "ADC {0:F1} -> {1:F3} kg", point.Raw, point.Mass);
                lstCalibration.Items.Add(entry);
            }

            lstCalibration.EndUpdate();
        }

        private void RecomputeCalibration()
        {
            double slope;
            double intercept;

            if (_calibrationPoints.Count == 0)
            {
                slope = 0;
                intercept = 0;
            }
            else if (_calibrationPoints.Count == 1)
            {
                CalibrationPoint single = _calibrationPoints[0];
                slope = single.Raw != 0 ? single.Mass / single.Raw : 0;
                intercept = 0;
            }

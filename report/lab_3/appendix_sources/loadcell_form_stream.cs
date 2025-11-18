        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (_serialPort.IsOpen && _serialPort.BytesToRead > 0)
                {
                    int next = _serialPort.ReadByte();
                    if (next < 0)
                    {
                        break;
                    }
                    ParseByte((byte)next);
                }
            }
            catch (TimeoutException)
            {
                // ignore transient timeouts
            }
            catch (InvalidOperationException)
            {
                // occurs when closing the port
            }
            catch (IOException)
            {
                // ignore IO errors from port closure
            }
        }

        private void ParseByte(byte value)
        {
            switch (_parserState)
            {
                case ParserState.WaitingForStart:
                    if (value == 0xFF)
                    {
                        _parserState = ParserState.ReadingMsb;
                    }
                    break;

                case ParserState.ReadingMsb:
                    if (value == 0xFF)
                    {
                        _parserState = ParserState.ReadingMsb;
                    }
                    else
                    {
                        _pendingMs5 = (byte)(value & 0x1F);
                        _parserState = ParserState.ReadingLsb;
                    }
                    break;

                case ParserState.ReadingLsb:
                    if (value == 0xFF)
                    {
                        _parserState = ParserState.ReadingMsb;
                    }
                    else
                    {
                        int raw = ((_pendingMs5 << 5) | (value & 0x1F)) & 0x3FF;
                        _parserState = ParserState.WaitingForStart;
                        ProcessSample(raw);
                    }
                    break;
            }
        }

        private void ProcessSample(int rawCount)
        {
            DateTime timestamp = DateTime.Now;
            double voltage;
            double weight;
            double avgWeight;
            double avgRaw;
            double stdDev;
            bool isStable;
            double mass;

            lock (_sampleLock)
            {
                _hasSample = true;
                _lastRaw = rawCount;
                voltage = rawCount * ReferenceVoltage / MaxAdcValue;
                _lastVoltage = voltage;

                mass = (_scaleSlope * rawCount) + _scaleIntercept;
                _lastMass = mass;
                weight = mass - _tareOffset;
                _lastWeight = weight;

                int windowSize = Math.Max(1, _rollingWindowSize);

                UpdateRollingQueue(_rawRollingQueue, ref _rawRollingSum, rawCount, windowSize);
                avgRaw = _rawRollingQueue.Count > 0 ? _rawRollingSum / _rawRollingQueue.Count : rawCount;
                _lastAverageRaw = avgRaw;

                UpdateRollingQueue(_weightRollingQueue, ref _weightRollingSum, weight, windowSize);
                avgWeight = _weightRollingQueue.Count > 0 ? _weightRollingSum / _weightRollingQueue.Count : weight;
                _lastAverageWeight = avgWeight;

                _stabilityQueue.Enqueue(new TimedValue(timestamp, weight));
                while (_stabilityQueue.Count > 0 && (timestamp - _stabilityQueue.Peek().Timestamp).TotalMilliseconds > StabilityWindowMilliseconds)
                {
                    _stabilityQueue.Dequeue();
                }

                stdDev = ComputeStandardDeviation(_stabilityQueue);
                _lastStdDev = stdDev;
                isStable = !double.IsNaN(stdDev) && _stabilityQueue.Count >= 5 && stdDev <= _stabilityThreshold;
                _lastStable = isStable;

                _chartPending.Enqueue(new ChartPoint(timestamp, weight, avgWeight));
                while (_chartPending.Count > ChartMaxPoints)
                {
                    _chartPending.Dequeue();
                }

                _sampleIndex++;
            }
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            int raw;
            double avgRaw;
            double voltage;
            double weight;
            double avgWeight;
            double stdDev;
            bool isStable;
            List<ChartPoint> pendingPoints = null;

            lock (_sampleLock)
            {
                if (!_hasSample)
                {
                    return;
                }

                raw = _lastRaw;
                avgRaw = _lastAverageRaw;
                voltage = _lastVoltage;
                weight = _lastWeight;
                avgWeight = _lastAverageWeight;
                stdDev = _lastStdDev;
                isStable = _lastStable;

                if (_chartPending.Count > 0)
                {
                    pendingPoints = new List<ChartPoint>(_chartPending.Count);
                    while (_chartPending.Count > 0)
                    {
                        pendingPoints.Add(_chartPending.Dequeue());
                    }
                }
            }

            UpdateUi(raw, avgRaw, voltage, weight, avgWeight, stdDev, isStable);

            if (pendingPoints != null && pendingPoints.Count > 0)
            {
                UpdateChart(pendingPoints);
            }
        }

        private void UpdateUi(int raw, double avgRaw, double voltage, double weight, double avgWeight, double stdDev, bool isStable)
        {
            txtRawAdc.Text = raw.ToString(CultureInfo.InvariantCulture);
            txtAverageRaw.Text = avgRaw.ToString("F1", CultureInfo.InvariantCulture);
            txtVoltage.Text = voltage.ToString("F3", CultureInfo.InvariantCulture);
            txtWeight.Text = weight.ToString("F3", CultureInfo.InvariantCulture);
            txtAverageWeight.Text = avgWeight.ToString("F3", CultureInfo.InvariantCulture);
            txtStdDev.Text = double.IsNaN(stdDev) ? "--" : stdDev.ToString("F4", CultureInfo.InvariantCulture);

            lblStabilityStatus.Text = isStable ? "Stable" : "Unstable";
            lblStabilityStatus.ForeColor = isStable ? Color.ForestGreen : Color.Firebrick;
        }

        private void UpdateChart(IEnumerable<ChartPoint> points)
        {
            var area = chartData.ChartAreas[0];
            Series weightSeries = chartData.Series["Weight"];
            Series avgSeries = chartData.Series["AvgWeight"];

            foreach (ChartPoint point in points)
            {
                double x = point.Timestamp.ToOADate();
                weightSeries.Points.AddXY(x, point.Weight);
                avgSeries.Points.AddXY(x, point.AverageWeight);
            }

            while (weightSeries.Points.Count > ChartMaxPoints)
            {
                weightSeries.Points.RemoveAt(0);
            }

            while (avgSeries.Points.Count > ChartMaxPoints)
            {
                avgSeries.Points.RemoveAt(0);
            }

            if (weightSeries.Points.Count > 0)
            {
                area.AxisX.Minimum = weightSeries.Points[0].XValue;
                area.AxisX.Maximum = weightSeries.Points[weightSeries.Points.Count - 1].XValue;
            }

            chartData.Invalidate();
        }

        private static double ComputeStandardDeviation(IEnumerable<TimedValue> samples)
        {
            int count = 0;
            double mean = 0;
            double m2 = 0;

        private void PortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (_port.IsOpen && _port.BytesToRead > 0)
            {
                int value = _port.ReadByte();
                if (value < 0)
                {
                    return;
                }

                switch (_frameState)
                {
                    case 0:
                        if (value == 0xFF)
                        {
                            _frameState = 1;
                        }
                        break;
                    case 1:
                        _ms5 = (byte)(value & 0x1F);
                        _frameState = 2;
                        break;
                    case 2:
                        _frameState = 0;
                        byte ls5 = (byte)(value & 0x1F);
                        int code10 = (_ms5 << 5) | ls5;
                        HandleSample(code10);
                        break;
                }
            }
        }

        private void HandleSample(int code10)
        {
            if (code10 < 0 || code10 > 1023)
            {
                return;
            }

            DateTime timestamp = DateTime.Now;
            double vout = code10 * Vref / 1023.0;
            double resistance;
            if (vout <= 0.0001 || Math.Abs(Vref - vout) < 0.0001)
            {
                resistance = double.NaN;
            }
            else
            {
                resistance = RFixed * (vout / (Vref - vout));
            }

            double tempRaw = CalculateBetaTemperature(resistance);
            double tempComp = _a0 + _a1 * tempRaw + _a2 * tempRaw * tempRaw;
            double error = tempComp - tempRaw;

            if (_csvWriter != null)
            {
                string line = string.Format(CultureInfo.InvariantCulture,
                    "{0:O},{1},{2:F6},{3:F2},{4:F3},{5:F3},{6:F3}",
                    timestamp, code10, vout, resistance, tempRaw, tempComp, error);
                try
                {
                    _csvWriter.WriteLine(line);
                }
                catch (IOException)
                {
                    BeginInvoke(new Action(delegate
                    {
                        AppendLog("Warning: unable to write to CSV (disk busy?).");
                    }));
                }
            }

            lock (_sync)
            {
                if (_capturing)
                {
                    double seconds = (timestamp.ToUniversalTime() - _captureStartUtc).TotalSeconds;
                    _captureBuffer.Add(new CaptureSample(seconds, tempComp));
                }
            }

            LiveSample sample = new LiveSample();
            sample.Timestamp = timestamp;
            sample.TempRaw = tempRaw;
            sample.TempComp = tempComp;
            sample.Voltage = vout;
            sample.Resistance = resistance;
            sample.Error = error;

            BeginInvoke(new Action(delegate
            {
                UpdateLiveDisplay(sample);
            }));
        }

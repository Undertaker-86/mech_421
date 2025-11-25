using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DistanceMonitor.Models;
using DistanceMonitor.Services;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DistanceMonitor.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private const double ReferenceVoltage = 3.3;
    private const int AdcResolution = 1023;
    private const int SamplesPerSecond = 100;
    private const double ScopeWindowSeconds = 2.0;

    private int _chartCapacity;
    private int _scopeCapacity;

    private readonly SerialDataService _serialDataService = new();
    private readonly CsvLogger _csvLogger = new();

    private readonly ObservableCollection<double> _voltageValues = new();
    private readonly ObservableCollection<double> _distanceValues = new();
    private readonly ObservableCollection<double> _adcValues = new();
    private readonly ObservableCollection<double> _scopeDistanceValues = new();
    private readonly LinkedList<RawSample> _chartBuffer = new();
    private readonly Queue<double> _smoothingBuffer = new();
    private readonly Queue<double> _plotAverageBuffer = new();
    private double _plotAverageSum;

    private readonly LineSeries<double> _voltageSeries;
    private readonly LineSeries<double> _distanceSeries;
    private readonly LineSeries<double> _adcSeries;
    private readonly LineSeries<double> _scopeDistanceSeries;

    private readonly ObservableCollection<ISeries> _series;
    private readonly ObservableCollection<ISeries> _scopeSeries;

    private readonly Axis _voltageAxis;
    private readonly Axis _distanceAxis;
    private readonly Axis _adcAxis;
    private readonly Axis _scopeDistanceAxis;

    private readonly Axis _timeAxis;
    private readonly Axis _scopeTimeAxis;

    private double[] _calibrationCoefficients = Array.Empty<double>();
    private long _sampleIndex;

    public MainViewModel()
    {
        LogDirectory = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "DistanceMonitor");

        AvailablePorts = new ObservableCollection<string>(SerialPort.GetPortNames().OrderBy(p => p));
        CalibrationPoints.CollectionChanged += OnCalibrationPointsChanged;

        TimeWindowOptions = new ObservableCollection<TimeWindowOption>
        {
            new("10 s", 10),
            new("30 s", 30),
            new("120 s", 120)
        };
        SelectedTimeWindow = TimeWindowOptions.First();

        _chartCapacity = (int)(SamplesPerSecond * SelectedTimeWindow.Seconds);
        _scopeCapacity = (int)(SamplesPerSecond * ScopeWindowSeconds);

        _serialDataService.SampleReceived += OnSampleReceived;
        _serialDataService.StatusMessage += (_, message) => SetStatus(message);

        _voltageSeries = new LineSeries<double>
        {
            Name = "Voltage",
            Values = _voltageValues,
            GeometryFill = null,
            GeometryStroke = null,
            LineSmoothness = 0,
            GeometrySize = 0,
            Stroke = new SolidColorPaint(new SKColor(79, 129, 189)),
            ScalesYAt = 0
        };

        _distanceSeries = new LineSeries<double>
        {
            Name = "Distance",
            Values = _distanceValues,
            GeometryFill = null,
            GeometryStroke = null,
            LineSmoothness = 0,
            GeometrySize = 0,
            Stroke = new SolidColorPaint(new SKColor(180, 90, 50)),
            ScalesYAt = 1
        };

        _adcSeries = new LineSeries<double>
        {
            Name = "ADC",
            Values = _adcValues,
            GeometryFill = null,
            GeometryStroke = null,
            LineSmoothness = 0,
            GeometrySize = 0,
            Stroke = new SolidColorPaint(new SKColor(90, 90, 90)),
            ScalesYAt = 2
        };

        _scopeDistanceSeries = new LineSeries<double>
        {
            Name = "Distance",
            Values = _scopeDistanceValues,
            GeometryFill = null,
            GeometryStroke = null,
            LineSmoothness = 0,
            GeometrySize = 0,
            Stroke = new SolidColorPaint(new SKColor(120, 180, 90))
        };

        _series = new ObservableCollection<ISeries> { _voltageSeries, _distanceSeries, _adcSeries };
        _scopeSeries = new ObservableCollection<ISeries> { _scopeDistanceSeries };

        _timeAxis = new Axis
        {
            Name = "Time",
            Labeler = value => $"{value / SamplesPerSecond:F1}s"
        };

        _scopeTimeAxis = new Axis
        {
            Name = "Recent Time",
            Labeler = value => $"{value / SamplesPerSecond:F2}s"
        };

        _voltageAxis = new Axis
        {
            Name = "Voltage (V)",
            MinLimit = 0,
            MaxLimit = ReferenceVoltage,
            NamePadding = new Padding(0, 0, 0, 16)
        };

        _distanceAxis = new Axis
        {
            Name = "Distance (cm)",
            Position = AxisPosition.End,
            MinLimit = double.NaN,
            MaxLimit = double.NaN,
            NamePadding = new Padding(0, 16, 0, 24)
        };

        _adcAxis = new Axis
        {
            Name = "ADC (counts)",
            MinLimit = 0,
            MaxLimit = AdcResolution,
            Position = AxisPosition.Start,
            NamePadding = new Padding(0, 0, 0, 24),
            NameTextSize = 14,
            LabelsPaint = new SolidColorPaint(new SKColor(70, 70, 70))
        };

        _scopeDistanceAxis = new Axis
        {
            Name = "Distance (cm)",
            MinLimit = double.NaN,
            MaxLimit = double.NaN,
            NamePadding = new Padding(0, 16, 0, 24),
            NameTextSize = 14,
            LabelsPaint = new SolidColorPaint(new SKColor(90, 90, 90))
        };

        MeasurementMode = MeasurementDisplayMode.Distance;
        ShowVoltageSeries = true;
        ShowDistanceSeries = true;
        ShowAdcSeries = true;
        AutoScaleVoltage = false;
        AutoScaleDistance = true;

        RangeMinimum = 5;
        RangeMaximum = 50;

        SetStatus("Select a COM port to begin.");
    }

    public ObservableCollection<string> AvailablePorts { get; }
    public ObservableCollection<CalibrationPoint> CalibrationPoints { get; } = new();
    public ObservableCollection<ISeries> Series => _series;
    public ObservableCollection<ISeries> ScopeSeries => _scopeSeries;
    public ObservableCollection<TimeWindowOption> TimeWindowOptions { get; }

    public Axis[] XAxes => new[] { _timeAxis };
    public Axis[] YAxes => new[] { _voltageAxis, _distanceAxis, _adcAxis };
    public Axis[] ScopeXAxes => new[] { _scopeTimeAxis };
    public Axis[] ScopeYAxes => new[] { _scopeDistanceAxis };

    [ObservableProperty]
    private string? selectedPort;

    [ObservableProperty]
    private bool isConnected;

    partial void OnIsConnectedChanged(bool value)
    {
        ConnectCommand.NotifyCanExecuteChanged();
        DisconnectCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    private string statusText = "Select a COM port to begin.";

    [ObservableProperty]
    private int lastRawValue;

    [ObservableProperty]
    private double currentVoltage;

    [ObservableProperty]
    private double currentDistance;

    [ObservableProperty]
    private double displayValue;

    [ObservableProperty]
    private long totalSamples;

    [ObservableProperty]
    private MeasurementDisplayMode measurementMode;

    partial void OnMeasurementModeChanged(MeasurementDisplayMode value)
    {
        MeasurementLabel = value == MeasurementDisplayMode.Voltage ? "Voltage" : "Distance";
        MeasurementUnit = value == MeasurementDisplayMode.Voltage ? "V" : "cm";
        DisplayValue = value == MeasurementDisplayMode.Voltage ? CurrentVoltage : CurrentDistance;
        OnPropertyChanged(nameof(IsVoltageMode));
        OnPropertyChanged(nameof(IsDistanceMode));
    }

    public bool IsVoltageMode
    {
        get => MeasurementMode == MeasurementDisplayMode.Voltage;
        set
        {
            if (value)
            {
                MeasurementMode = MeasurementDisplayMode.Voltage;
            }
        }
    }

    public bool IsDistanceMode
    {
        get => MeasurementMode == MeasurementDisplayMode.Distance;
        set
        {
            if (value)
            {
                MeasurementMode = MeasurementDisplayMode.Distance;
            }
        }
    }

    [ObservableProperty]
    private string measurementLabel = "Distance";

    [ObservableProperty]
    private string measurementUnit = "cm";

    [ObservableProperty]
    private double linearScale = 30.0;

    partial void OnLinearScaleChanged(double value)
    {
        RecalculateDerivedValues();
    }

    [ObservableProperty]
    private double linearOffset;

    partial void OnLinearOffsetChanged(double value)
    {
        RecalculateDerivedValues();
    }

    [ObservableProperty]
    private bool showVoltageSeries;

    partial void OnShowVoltageSeriesChanged(bool value)
    {
        _voltageSeries.IsVisible = value;
    }

    [ObservableProperty]
    private bool showDistanceSeries;

    partial void OnShowDistanceSeriesChanged(bool value)
    {
        _distanceSeries.IsVisible = value;
    }

    [ObservableProperty]
    private bool showAdcSeries;

    partial void OnShowAdcSeriesChanged(bool value)
    {
        _adcSeries.IsVisible = value;
    }

    [ObservableProperty]
    private bool autoScaleVoltage;

    partial void OnAutoScaleVoltageChanged(bool value)
    {
        _voltageAxis.MinLimit = value ? double.NaN : 0;
        _voltageAxis.MaxLimit = value ? double.NaN : ReferenceVoltage;
    }

    [ObservableProperty]
    private bool autoScaleDistance;

    partial void OnAutoScaleDistanceChanged(bool value)
    {
        _distanceAxis.MinLimit = value ? double.NaN : RangeMinimum;
        _distanceAxis.MaxLimit = value ? double.NaN : RangeMaximum;
        _scopeDistanceAxis.MinLimit = _distanceAxis.MinLimit;
        _scopeDistanceAxis.MaxLimit = _distanceAxis.MaxLimit;
    }

    [ObservableProperty]
    private bool useSmoothing;

    partial void OnUseSmoothingChanged(bool value)
    {
        _smoothingBuffer.Clear();
    }

    [ObservableProperty]
    private int smoothingWindow = 5;

    partial void OnSmoothingWindowChanged(int value)
    {
        if (value < 1)
        {
            SmoothingWindow = 1;
        }
        else if (value > 250)
        {
            SmoothingWindow = 250;
        }
        _smoothingBuffer.Clear();
    }

    [ObservableProperty]
    private bool usePlotAverage;

    partial void OnUsePlotAverageChanged(bool value)
    {
        ResetPlotAverageState();
        RebuildChartValues();
    }

    [ObservableProperty]
    private int plotAverageWindow = 10;

    partial void OnPlotAverageWindowChanged(int value)
    {
        if (value < 1)
        {
            PlotAverageWindow = 1;
            return;
        }

        if (value > 500)
        {
            PlotAverageWindow = 500;
            return;
        }

        ResetPlotAverageState();
        RebuildChartValues();
    }

    private void ResetPlotAverageState()
    {
        _plotAverageBuffer.Clear();
        _plotAverageSum = 0;
    }

    [ObservableProperty]
    private bool isLogging;

    [ObservableProperty]
    private string lastLogFile = string.Empty;

    [ObservableProperty]
    private string logDirectory;

    [ObservableProperty]
    private double rangeMinimum;

    partial void OnRangeMinimumChanged(double value)
    {
        OnPropertyChanged(nameof(WorkingRangeSummary));
        AutoScaleDistanceChanged();
    }

    [ObservableProperty]
    private double rangeMaximum;

    partial void OnRangeMaximumChanged(double value)
    {
        OnPropertyChanged(nameof(WorkingRangeSummary));
        AutoScaleDistanceChanged();
    }

    private void AutoScaleDistanceChanged()
    {
        if (!AutoScaleDistance)
        {
            _distanceAxis.MinLimit = RangeMinimum;
            _distanceAxis.MaxLimit = RangeMaximum;
            _scopeDistanceAxis.MinLimit = RangeMinimum;
            _scopeDistanceAxis.MaxLimit = RangeMaximum;
        }
    }

    public string WorkingRangeSummary => $"{RangeMinimum:F1} cm to {RangeMaximum:F1} cm";

    [ObservableProperty]
    private bool isOutOfRange;

    [ObservableProperty]
    private string outOfRangeMessage = "Within range.";

    [ObservableProperty]
    private double noiseStandardDeviation;

    [ObservableProperty]
    private string noiseSummary = "Noise not measured yet.";

    [ObservableProperty]
    private CalibrationPoint? selectedCalibrationPoint;

    partial void OnSelectedCalibrationPointChanged(CalibrationPoint? value)
    {
        RemoveCalibrationPointCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    private double? calibrationMeasuredDistance;

    [ObservableProperty]
    private int calibrationOrder = 2;

    partial void OnCalibrationOrderChanged(int value)
    {
        if (value < 1)
        {
            CalibrationOrder = 1;
        }
        else if (value > 5)
        {
            CalibrationOrder = 5;
        }

        FitCalibrationCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    private string calibrationSummary = "No calibration applied.";

    [ObservableProperty]
    private bool calibrationApplied;

    [ObservableProperty]
    private TimeWindowOption selectedTimeWindow = new("10 s", 10);

    partial void OnSelectedTimeWindowChanged(TimeWindowOption value)
    {
        _chartCapacity = (int)(SamplesPerSecond * value.Seconds);
        RebuildChartValues();
    }

    [RelayCommand]
    private void RefreshPorts()
    {
        var ports = SerialPort.GetPortNames()
            .OrderBy(p => p)
            .ToArray();

        AvailablePorts.Clear();
        foreach (var port in ports)
        {
            AvailablePorts.Add(port);
        }

        if (!IsConnected)
        {
            if (ports.Length == 0)
            {
                SelectedPort = null;
            }
            else if (SelectedPort is null || !ports.Contains(SelectedPort))
            {
                SelectedPort = ports[0];
            }
        }

        SetStatus(ports.Length == 0 ? "No COM ports detected." : "Ports refreshed.");
    }

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private void Connect()
    {
        if (SelectedPort is null)
        {
            SetStatus("Select a COM port before connecting.");
            return;
        }

        try
        {
            ResetData();
            _serialDataService.Connect(SelectedPort, 9600);
            IsConnected = true;
            SetStatus($"Connected to {SelectedPort}.");
        }
        catch (Exception ex)
        {
            SetStatus($"Failed to connect: {ex.Message}");
        }
    }

    private bool CanConnect() => !IsConnected;

    [RelayCommand(CanExecute = nameof(CanDisconnect))]
    private void Disconnect()
    {
        _serialDataService.Disconnect();
        if (IsLogging)
        {
            ToggleLogging();
        }

        IsConnected = false;
        SetStatus("Disconnected.");
    }

    private bool CanDisconnect() => IsConnected;

    [RelayCommand]
    private void ToggleLogging()
    {
        if (!IsLogging)
        {
            try
            {
                System.IO.Directory.CreateDirectory(LogDirectory);
                string filePath = System.IO.Path.Combine(
                    LogDirectory,
                    $"distance_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                _csvLogger.Start(filePath);
                LastLogFile = filePath;
                IsLogging = true;
                SetStatus($"Logging to {filePath}");
            }
            catch (Exception ex)
            {
                SetStatus($"Unable to start logging: {ex.Message}");
            }
        }
        else
        {
            _csvLogger.Stop();
            IsLogging = false;
            SetStatus("Logging stopped.");
        }
    }

    [RelayCommand]
    private void CaptureCalibrationPoint()
    {
        if (CalibrationMeasuredDistance is null)
        {
            SetStatus("Enter the measured distance before capturing a calibration point.");
            return;
        }

        var point = new CalibrationPoint(
            DateTime.Now,
            LastRawValue,
            CurrentVoltage,
            CalibrationMeasuredDistance.Value);

        CalibrationPoints.Add(point);
        CalibrationPoints.SortInPlace((a, b) => a.RawValue.CompareTo(b.RawValue));
        CalibrationMeasuredDistance = null;
        SetStatus($"Captured calibration point at raw={point.RawValue} distance={point.MeasuredDistance:F2} cm.");
    }

    [RelayCommand(CanExecute = nameof(CanRemoveCalibrationPoint))]
    private void RemoveCalibrationPoint()
    {
        if (SelectedCalibrationPoint is not null)
        {
            CalibrationPoints.Remove(SelectedCalibrationPoint);
            SelectedCalibrationPoint = null;
        }
    }

    private bool CanRemoveCalibrationPoint() => SelectedCalibrationPoint is not null;

    [RelayCommand]
    private void ClearCalibrationPoints()
    {
        CalibrationPoints.Clear();
        CalibrationSummary = "No calibration applied.";
        CalibrationApplied = false;
        _calibrationCoefficients = Array.Empty<double>();
        RecalculateDerivedValues();
    }

    private void OnCalibrationPointsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshCalibrationCommandStates();
    }

    private void RefreshCalibrationCommandStates()
    {
        FitCalibrationCommand.NotifyCanExecuteChanged();
        RemoveCalibrationPointCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanFitCalibration))]
    private void FitCalibration()
    {
        try
        {
            _calibrationCoefficients = CalibrationService.FitPolynomial(CalibrationPoints, CalibrationOrder);
            CalibrationApplied = true;
            CalibrationSummary = $"Fit order {CalibrationOrder}: {FormatCoefficients(_calibrationCoefficients)}";
            SetStatus("Calibration applied.");
            RecalculateDerivedValues();
        }
        catch (Exception ex)
        {
            SetStatus($"Calibration error: {ex.Message}");
        }
    }

    private bool CanFitCalibration() => CalibrationPoints.Count > CalibrationOrder;

    [RelayCommand]
    private void MeasureNoise()
    {
        int windowSamples = Math.Min(_chartBuffer.Count, SamplesPerSecond * 10);
        if (windowSamples == 0)
        {
            SetStatus("Not enough data to measure noise. Collect more samples.");
            return;
        }

        var distances = _chartBuffer
            .TakeLast(windowSamples)
            .Select(raw =>
            {
                double voltage = raw.RawValue * ReferenceVoltage / AdcResolution;
                return EvaluateDistance(raw.RawValue, voltage);
            })
            .ToArray();

        double mean = distances.Average();
        double variance = distances.Sum(d => Math.Pow(d - mean, 2)) / distances.Length;
        double stdDev = Math.Sqrt(variance);

        NoiseStandardDeviation = stdDev;
        NoiseSummary = $"σ = {stdDev:F3} cm over {windowSamples / (double)SamplesPerSecond:F1} s (mean {mean:F2} cm)";
        SetStatus("Noise measurement updated.");
    }

    private void OnSampleReceived(object? sender, RawSample sample)
    {
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            ProcessSample(sample);
        });
    }

    private void ProcessSample(RawSample rawSample)
    {
        TotalSamples = ++_sampleIndex;
        LastRawValue = rawSample.RawValue;

        double voltage = rawSample.RawValue * ReferenceVoltage / AdcResolution;
        CurrentVoltage = voltage;

        double rawDistance = EvaluateDistance(rawSample.RawValue, voltage);
        CurrentDistance = rawDistance;

        double displayDistance = rawDistance;

        if (UseSmoothing)
        {
            _smoothingBuffer.Enqueue(rawDistance);
            while (_smoothingBuffer.Count > SmoothingWindow)
            {
                _smoothingBuffer.Dequeue();
            }

            displayDistance = _smoothingBuffer.Average();
        }

        DisplayValue = MeasurementMode == MeasurementDisplayMode.Voltage ? voltage : displayDistance;

        _chartBuffer.AddLast(rawSample);
        if (_chartBuffer.Count > Math.Max(_chartCapacity, SamplesPerSecond * 180))
        {
            _chartBuffer.RemoveFirst();
        }

        AppendChartValues(voltage, rawDistance, rawSample.RawValue);
        AppendScopeValue(rawDistance);

        UpdateRangeStatus(rawDistance);

        if (IsLogging)
        {
            var logRecord = new SampleRecord(rawSample.Timestamp, rawSample.RawValue, voltage, rawDistance);
            _csvLogger.WriteSample(logRecord);
        }
    }

    private double EvaluateDistance(int rawValue, double voltage)
    {
        if (_calibrationCoefficients.Length > 0)
        {
            return CalibrationService.EvaluatePolynomial(_calibrationCoefficients, voltage);
        }

        return voltage * LinearScale + LinearOffset;
    }

    private void AppendChartValues(double voltage, double distance, double rawCounts)
    {
        _voltageValues.Add(voltage);
        AppendDistanceForPlot(distance);
        _adcValues.Add(rawCounts);

        TrimToCapacity(_voltageValues, _chartCapacity);
        TrimToCapacity(_adcValues, _chartCapacity);
    }

    private void AppendDistanceForPlot(double distance)
    {
        if (UsePlotAverage)
        {
            _plotAverageBuffer.Enqueue(distance);
            _plotAverageSum += distance;

            if (_plotAverageBuffer.Count > PlotAverageWindow)
            {
                _plotAverageSum -= _plotAverageBuffer.Dequeue();
            }

            double averaged = _plotAverageBuffer.Count > 0
                ? _plotAverageSum / _plotAverageBuffer.Count
                : distance;
            _distanceValues.Add(averaged);
        }
        else
        {
            _distanceValues.Add(distance);
        }

        TrimToCapacity(_distanceValues, _chartCapacity);
    }

    private void AppendScopeValue(double distance)
    {
        _scopeDistanceValues.Add(distance);
        TrimToCapacity(_scopeDistanceValues, _scopeCapacity);
    }

    private void TrimToCapacity(Collection<double> collection, int capacity)
    {
        while (collection.Count > capacity)
        {
            collection.RemoveAt(0);
        }
    }

    private void RebuildChartValues()
    {
        _voltageValues.Clear();
        _distanceValues.Clear();
        _adcValues.Clear();
        _scopeDistanceValues.Clear();
        ResetPlotAverageState();

        foreach (var raw in _chartBuffer.TakeLast(_chartCapacity))
        {
            double voltage = raw.RawValue * ReferenceVoltage / AdcResolution;
            double distance = EvaluateDistance(raw.RawValue, voltage);
            _voltageValues.Add(voltage);
            AppendDistanceForPlot(distance);
            _adcValues.Add(raw.RawValue);
        }

        foreach (var raw in _chartBuffer.TakeLast(_scopeCapacity))
        {
            double voltage = raw.RawValue * ReferenceVoltage / AdcResolution;
            double distance = EvaluateDistance(raw.RawValue, voltage);
            _scopeDistanceValues.Add(distance);
        }
    }

    private void UpdateRangeStatus(double distance)
    {
        if (distance < RangeMinimum)
        {
            IsOutOfRange = true;
            OutOfRangeMessage = "Too close!";
        }
        else if (distance > RangeMaximum)
        {
            IsOutOfRange = true;
            OutOfRangeMessage = "Too far!";
        }
        else
        {
            IsOutOfRange = false;
            OutOfRangeMessage = "Within range.";
        }
    }

    private static string FormatCoefficients(IEnumerable<double> coefficients)
    {
        return string.Join(", ", coefficients.Select((c, i) => $"a{i}={c:F5}"));
    }

    private void RecalculateDerivedValues()
    {
        double voltage = LastRawValue * ReferenceVoltage / AdcResolution;
        CurrentVoltage = voltage;
        double distance = EvaluateDistance(LastRawValue, voltage);
        CurrentDistance = distance;
        DisplayValue = MeasurementMode == MeasurementDisplayMode.Voltage ? voltage : distance;
        UpdateRangeStatus(distance);
        RebuildChartValues();
    }

    private void ResetData()
    {
        _chartBuffer.Clear();
        _voltageValues.Clear();
        _distanceValues.Clear();
        _adcValues.Clear();
        _scopeDistanceValues.Clear();
        _smoothingBuffer.Clear();
        ResetPlotAverageState();
        _sampleIndex = 0;
        TotalSamples = 0;
        LastRawValue = 0;
        CurrentVoltage = 0;
        CurrentDistance = 0;
        DisplayValue = 0;
        IsOutOfRange = false;
        OutOfRangeMessage = "Within range.";
        NoiseSummary = "Noise not measured yet.";
        RecalculateDerivedValues();
    }

    private void SetStatus(string message)
    {
        StatusText = $"{DateTime.Now:HH:mm:ss} - {message}";
    }

    public void CleanUp()
    {
        _csvLogger.Dispose();
        _serialDataService.Dispose();
    }
}

public enum MeasurementDisplayMode
{
    Voltage,
    Distance
}

public readonly record struct TimeWindowOption(string Label, double Seconds);

internal static class ObservableCollectionExtensions
{
    public static void SortInPlace<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
    {
        var sortableList = collection.ToList();
        sortableList.Sort(comparison);
        collection.Clear();
        foreach (var item in sortableList)
        {
            collection.Add(item);
        }
    }
}

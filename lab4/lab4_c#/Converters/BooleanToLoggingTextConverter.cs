using System;
using System.Globalization;
using System.Windows.Data;

namespace DistanceMonitor.Converters;

public sealed class BooleanToLoggingTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool flag = value is bool b && b;
        return flag ? "Stop Logging" : "Start Logging";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}

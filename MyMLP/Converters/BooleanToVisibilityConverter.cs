using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyMLP.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (value is bool b) && b;

            if (IsInverted)
            {
                boolValue = !boolValue;
            }

            if (parameter is string paramString &&
                (string.Equals(paramString, "invert", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(paramString, "inverse", StringComparison.OrdinalIgnoreCase)))
            {
                boolValue = !boolValue;
            }

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = (value is Visibility v) && v == Visibility.Visible;

            if (IsInverted)
            {
                isVisible = !isVisible;
            }

            if (parameter is string paramString &&
                (string.Equals(paramString, "invert", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(paramString, "inverse", StringComparison.OrdinalIgnoreCase)))
            {
                isVisible = !isVisible;
            }

            return isVisible;
        }
    }
}
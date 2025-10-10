using System;
using System.Globalization;
using System.Windows.Data;

namespace MyMLP.Converters
{
    public class WeightToOpacityConverter : IValueConverter
    {
        private const double MinOpacity = 0.2;
        private const double MaxOpacity = 1.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double weight)
            {
                double absoluteWeight = Math.Abs(weight);
                double opacity = MinOpacity + (absoluteWeight * (MaxOpacity - MinOpacity));
                return Math.Min(opacity, MaxOpacity);
            }
            return MinOpacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
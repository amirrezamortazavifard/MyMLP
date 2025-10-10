using System;
using System.Globalization;
using System.Windows.Data;

namespace MyMLP.Converters
{
    public class WeightToThicknessConverter : IValueConverter
    {
        private const double MinThickness = 0.5;
        private const double MaxThickness = 3.5;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double weight)
            {
                double absoluteWeight = Math.Abs(weight);
                double thickness = MinThickness + (absoluteWeight * MaxThickness);
                return Math.Min(thickness, MaxThickness + 0.5);
            }
            return MinThickness;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;

namespace MyMLP.Converters
{
    public class CenterTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || !(values[0] is double) || !(values[1] is double))
            {
                return 0;
            }
            double centerX = (double)values[0];
            double actualWidth = (double)values[1];
            return centerX - (actualWidth / 2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
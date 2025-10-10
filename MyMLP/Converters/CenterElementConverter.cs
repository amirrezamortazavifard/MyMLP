using System;
using System.Globalization;
using System.Windows.Data;
namespace MyMLP.Converters
{
    public class CenterElementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double center = (double)value;
            double size = double.Parse(parameter as string, CultureInfo.InvariantCulture);
            return center - (size / 2);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
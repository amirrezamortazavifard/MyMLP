using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MyMLP.Converters
{
    public class WeightToBrushConverter : IValueConverter
    {
        public SolidColorBrush PositiveBrush { get; set; } = new SolidColorBrush(Colors.SteelBlue);
        public SolidColorBrush NegativeBrush { get; set; } = new SolidColorBrush(Colors.OrangeRed);
        public SolidColorBrush NeutralBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double weight)
            {
                if (weight >= 0)
                {
                    return Interpolate(NeutralBrush, PositiveBrush, weight);
                }
                else
                {
                    return Interpolate(NeutralBrush, NegativeBrush, Math.Abs(weight));
                }
            }
            return NeutralBrush;
        }

        private SolidColorBrush Interpolate(SolidColorBrush from, SolidColorBrush to, double ratio)
        {
            if (ratio < 0) ratio = 0;
            if (ratio > 1) ratio = 1;

            byte a = (byte)((to.Color.A - from.Color.A) * ratio + from.Color.A);
            byte r = (byte)((to.Color.R - from.Color.R) * ratio + from.Color.R);
            byte g = (byte)((to.Color.G - from.Color.G) * ratio + from.Color.G);
            byte b = (byte)((to.Color.B - from.Color.B) * ratio + from.Color.B);

            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
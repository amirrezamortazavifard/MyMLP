using System;
using LiveCharts;

namespace MyMLP.ViewModels
{
    public class PerformanceDashboardViewModel : ViewModelBase
    {
        public SeriesCollection ChartSeries { get; set; }
        public Func<double, string> AccuracyAxisFormatter { get; set; }
        public Func<double, string> ErrorAxisFormatter { get; set; }

        public PerformanceDashboardViewModel()
        {
            AccuracyAxisFormatter = value => value.ToString("P0");
            ErrorAxisFormatter = value => value.ToString("F2");

            ChartSeries = new SeriesCollection
            {
                new LiveCharts.Wpf.LineSeries { Title = "Error", Values = new ChartValues<double>(), PointGeometry = null, ScalesYAt = 0 },
                new LiveCharts.Wpf.LineSeries { Title = "Accuracy", Values = new ChartValues<double>(), PointGeometry = null, ScalesYAt = 1 }
            };
        }
    }
}
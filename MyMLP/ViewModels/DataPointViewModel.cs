using System.Windows.Media;

namespace MyMLP.ViewModels
{
    public class DataPointViewModel : ViewModelBase
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Brush Fill { get; set; }
    }
}
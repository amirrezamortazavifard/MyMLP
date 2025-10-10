using System.Windows;
using System.Windows.Controls;
using MyMLP.ViewModels;

namespace MyMLP.Views
{
    public partial class DataView : UserControl
    {
        public DataView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is DataViewModel vm)
            {
                if (ActualWidth > 1 && ActualHeight > 1)
                {
                    vm.UpdateCanvasDimensions(ActualWidth, ActualHeight);
                }
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is DataViewModel vm && e.NewSize.Width > 1 && e.NewSize.Height > 1)
            {
                vm.UpdateCanvasDimensions(e.NewSize.Width, e.NewSize.Height);
            }
        }
    }
}
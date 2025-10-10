using System.Windows;
using MyMLP.ViewModels;

namespace MyMLP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
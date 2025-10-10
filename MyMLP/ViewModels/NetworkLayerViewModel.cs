using System.Collections.ObjectModel;

namespace MyMLP.ViewModels
{
    public class NetworkLayerViewModel
    {
        public string Title { get; set; }
        public double TitleX { get; set; }
        public ObservableCollection<NeuronViewModel> Neurons { get; }

        public NetworkLayerViewModel()
        {
            Neurons = new ObservableCollection<NeuronViewModel>();
        }
    }
}
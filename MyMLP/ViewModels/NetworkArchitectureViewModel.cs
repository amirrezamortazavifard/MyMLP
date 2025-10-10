using System.Collections.ObjectModel;

namespace MyMLP.ViewModels
{
    public class NetworkArchitectureViewModel : ViewModelBase
    {
        public ObservableCollection<NetworkLayerViewModel> VisualLayers { get; }
        public ObservableCollection<ConnectionViewModel> VisualConnections { get; }

        public NetworkArchitectureViewModel()
        {
            VisualLayers = new ObservableCollection<NetworkLayerViewModel>();
            VisualConnections = new ObservableCollection<ConnectionViewModel>();
        }
    }
}
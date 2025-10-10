using System.Collections.ObjectModel;
namespace MyMLP.ViewModels
{
    public class TrainingLogViewModel : ViewModelBase
    {
        public ObservableCollection<string> TrainingLog { get; }
      
    public TrainingLogViewModel()
        {
            TrainingLog = new ObservableCollection<string>();
        }
    }
}
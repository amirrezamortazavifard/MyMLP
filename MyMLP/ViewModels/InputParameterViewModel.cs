namespace MyMLP.ViewModels
{
    public class InputParameterViewModel : ViewModelBase
    {
        private double _value;
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public string Label { get; }

        public InputParameterViewModel(int index)
        {
            Label = $"Input {index}:";
        }
    }
}
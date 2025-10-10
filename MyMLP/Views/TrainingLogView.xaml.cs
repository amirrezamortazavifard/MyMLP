using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using MyMLP.ViewModels;
using System.Collections.ObjectModel;
using System;

namespace MyMLP.Views
{
    public partial class TrainingLogView : UserControl
    {
        public TrainingLogView()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is TrainingLogViewModel oldVm)
            {
                oldVm.TrainingLog.CollectionChanged -= OnTrainingLogCollectionChanged;
            }
            if (e.NewValue is TrainingLogViewModel newVm)
            {
                newVm.TrainingLog.CollectionChanged += OnTrainingLogCollectionChanged;
            }
        }

        private void OnTrainingLogCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    if (sender is ObservableCollection<string> collection && collection.Any())
                    {
                        var lastItem = collection.Last();
                        if (lastItem != null)
                        {
                            LogListBox.ScrollIntoView(lastItem);
                        }
                    }
                }));
            }
        }
    }
}
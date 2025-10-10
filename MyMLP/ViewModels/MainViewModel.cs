using Microsoft.Win32;
using MyMLP.Core;
using MyMLP.Exceptions;
using MyMLP.Models;
using MyMLP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static MyMLP.Services.ThemeService;

namespace MyMLP.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataLoaderService _dataLoaderService;
        private readonly IModelPersistenceService _modelPersistenceService;
        private readonly IDataGeneratorService _dataGeneratorService;
        private readonly IUISnapshotService _uiSnapshotService;
        private readonly object _networkLock = new object();

        private NeuralNetwork _neuralNetwork;
        private List<DataPoint> _dataSet;
        private TrainingEngine _trainingEngine;
        private Task _trainingTask;
        private CancellationTokenSource _cancellationTokenSource;
        private DatasetType? _activeDatasetType;

        private int _currentEpoch;
        private double _maxSeenAbsoluteWeight;
        private List<double> _previousWeights;
        private Theme _currentTheme = Theme.Dark;

        public NetworkArchitectureViewModel ArchitectureVM { get; }
        public PerformanceDashboardViewModel DashboardVM { get; }
        public TrainingLogViewModel LogVM { get; }
        public DataViewModel DataVM { get; }

        private bool _isTrainingRunning;
        public bool IsTrainingRunning
        {
            get => _isTrainingRunning;
            set { _isTrainingRunning = value; OnPropertyChanged(); }
        }

        private bool _isRenderingBoundary;
        public bool IsRenderingBoundary
        {
            get => _isRenderingBoundary;
            set { _isRenderingBoundary = value; OnPropertyChanged(); }
        }

        private bool _isLiveUpdateEnabled = true;
        public bool IsLiveUpdateEnabled
        {
            get => _isLiveUpdateEnabled;
            set { _isLiveUpdateEnabled = value; OnPropertyChanged(); }
        }

        private int _inputSize = 2;
        public int InputSize
        {
            get => _inputSize;
            set
            {
                if (_inputSize == value) return;
                InvalidateDatasetIfArchitectureChanges();
                _inputSize = value;
                OnPropertyChanged();
                UpdateNetworkVisual();
            }
        }

        private string _hiddenLayersDefinition = "4,8";
        public string HiddenLayersDefinition
        {
            get => _hiddenLayersDefinition;
            set
            {
                if (_hiddenLayersDefinition == value) return;
                InvalidateDatasetIfArchitectureChanges();
                _hiddenLayersDefinition = value;
                OnPropertyChanged();
                UpdateNetworkVisual();
            }
        }

        private int _outputSize = 2;
        public int OutputSize
        {
            get => _outputSize;
            set
            {
                if (_outputSize == value) return;
                InvalidateDatasetIfArchitectureChanges();
                _outputSize = value;
                OnPropertyChanged();
                UpdateNetworkVisual();
            }
        }

        private double _learningRate = 0.5;
        public double LearningRate
        {
            get => _learningRate;
            set
            {
                if (Math.Abs(_learningRate - value) < 1e-9) return;
                _learningRate = value;
                OnPropertyChanged();
                if (!_isTrainingRunning && _trainingEngine != null)
                {
                    ResetTrainingEngine();
                }
            }
        }

        private int _epochs = 1000;
        public int Epochs
        {
            get => _epochs;
            set { _epochs = value; OnPropertyChanged(); UpdateStatus(); }
        }

        private int _epochsPerUpdate = 25;
        public int EpochsPerUpdate
        {
            get => _epochsPerUpdate;
            set { _epochsPerUpdate = value; OnPropertyChanged(); }
        }

        private string _selectedFilePath;
        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set { _selectedFilePath = value; OnPropertyChanged(); }
        }

        private int _epochProgressValue;
        public int EpochProgressValue
        {
            get => _epochProgressValue;
            set { _epochProgressValue = value; OnPropertyChanged(); }
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        private int _dataSampleCount = 200;
        public int DataSampleCount
        {
            get => _dataSampleCount;
            set
            {
                if (_dataSampleCount == value) return;
                _dataSampleCount = value;
                OnPropertyChanged();
                RegenerateActiveDataset();
            }
        }

        private double _dataNoiseLevel = 0.2;
        public double DataNoiseLevel
        {
            get => _dataNoiseLevel;
            set
            {
                if (Math.Abs(_dataNoiseLevel - value) < 1e-9) return;
                _dataNoiseLevel = value;
                OnPropertyChanged();
                RegenerateActiveDataset();
            }
        }

        public ObservableCollection<KeyValuePair<string, IActivationFunction>> AvailableActivationFunctions { get; }
        private KeyValuePair<string, IActivationFunction> _selectedActivationFunction;
        public KeyValuePair<string, IActivationFunction> SelectedActivationFunction
        {
            get => _selectedActivationFunction;
            set
            {
                if (Equals(_selectedActivationFunction, value)) return;
                _selectedActivationFunction = value;
                OnPropertyChanged();
            }
        }

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand LoadDataCommand { get; }
        public ICommand GenerateDataCommand { get; }
        public ICommand SaveModelCommand { get; }
        public ICommand LoadModelCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand RenderBoundaryCommand { get; }
        public ICommand ExportSessionCommand { get; }

        public MainViewModel()
        {
            _dataLoaderService = new CsvDataLoaderService();
            _modelPersistenceService = new JsonModelPersistenceService();
            _dataGeneratorService = new ClassicDataGeneratorService();
            _uiSnapshotService = new WpfUISnapshotService();
            _previousWeights = new List<double>();

            ArchitectureVM = new NetworkArchitectureViewModel();
            DashboardVM = new PerformanceDashboardViewModel();
            LogVM = new TrainingLogViewModel();
            DataVM = new DataViewModel();

            AvailableActivationFunctions = new ObservableCollection<KeyValuePair<string, IActivationFunction>>
            {
                new KeyValuePair<string, IActivationFunction>("Sigmoid", new Sigmoid()),
                new KeyValuePair<string, IActivationFunction>("ReLU", new ReLU()),
                new KeyValuePair<string, IActivationFunction>("Tanh", new Tanh())
            };
            SelectedActivationFunction = AvailableActivationFunctions[0];

            PlayCommand = new RelayCommand(PlayTraining, CanPlayTraining);
            PauseCommand = new RelayCommand(PauseTraining, CanPauseTraining);
            ResetCommand = new RelayCommand(ResetTraining, CanResetTraining);
            LoadDataCommand = new RelayCommand(LoadData, CanModifyState);
            GenerateDataCommand = new RelayCommand(GenerateData, CanModifyState);
            SaveModelCommand = new RelayCommand(SaveModel, CanModifyState);
            LoadModelCommand = new RelayCommand(LoadModel, CanModifyState);
            ToggleThemeCommand = new RelayCommand(ToggleTheme);
            RenderBoundaryCommand = new RelayCommand(async _ => await RenderBoundary(), CanRenderBoundary);
            ExportSessionCommand = new RelayCommand(async (p) => await ExportSession(p), CanModifyState);

            ResetTraining(null);
        }

        private async Task ExportSession(object parameter)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Export Session - Choose Base Name",
                Filter = "PNG Image|*.png",
                FileName = $"SessionExport_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            string baseFilePath = Path.ChangeExtension(saveFileDialog.FileName, null);
            string logFilePath = $"{baseFilePath}_log.txt";
            string dataFilePath = $"{baseFilePath}_data.png";
            string networkFilePath = $"{baseFilePath}_network.png";
            string performanceFilePath = $"{baseFilePath}_performance.png";

            if (parameter is not object[] elements || elements.Length < 4)
            {
                Log("Export failed: UI elements not provided correctly.");
                return;
            }

            var dataView = elements[0] as FrameworkElement;
            var performanceView = elements[1] as FrameworkElement;
            var networkView = elements[2] as FrameworkElement;
            var logView = elements[3] as FrameworkElement;

            try
            {
                Log("Exporting session...");

                var logContent = new StringBuilder();
                foreach (var line in LogVM.TrainingLog)
                {
                    logContent.AppendLine(line);
                }
                await File.WriteAllTextAsync(logFilePath, logContent.ToString());

                if (dataView != null) await _uiSnapshotService.SaveAsPngAsync(dataView, dataFilePath);
                if (performanceView != null) await _uiSnapshotService.SaveAsPngAsync(performanceView, performanceFilePath);
                if (networkView != null) await _uiSnapshotService.SaveAsPngAsync(networkView, networkFilePath);

                Log($"Session exported successfully to: {Path.GetDirectoryName(baseFilePath)}");
            }
            catch (Exception ex)
            {
                Log($"Error exporting session: {ex.Message}");
            }
        }

        private async Task RenderBoundary()
        {
            IsRenderingBoundary = true;
            try
            {
                await DataVM.RenderDecisionBoundary(_neuralNetwork, _networkLock);
            }
            catch (Exception ex)
            {
                Log($"Error rendering decision boundary: {ex.Message}");
            }
            finally
            {
                IsRenderingBoundary = false;
            }
        }

        private async void PlayTraining(object parameter)
        {
            if (_trainingEngine == null)
            {
                ResetTrainingEngine();
                if (_trainingEngine == null)
                {
                    StatusText = "Cannot start training. Please load or generate data first.";
                    return;
                }
            }

            IsTrainingRunning = true;
            Log("Training started/resumed...");
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            _trainingTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested && _currentEpoch < Epochs)
                {
                    _currentEpoch++;
                    CaptureCurrentWeights();
                    var (averageError, accuracy) = _trainingEngine.TrainSingleEpoch();

                    if (_currentEpoch % EpochsPerUpdate == 0 || _currentEpoch >= Epochs)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(async () =>
                        {
                            Log($"Epoch: {_currentEpoch}, Error: {averageError:F6}, Accuracy: {accuracy:P2}");
                            UpdateStatus();
                            DashboardVM.ChartSeries[0].Values.Add(averageError);
                            DashboardVM.ChartSeries[1].Values.Add(accuracy);
                            UpdateConnectionWeights();
                            HighlightChangedConnections();
                            if (IsLiveUpdateEnabled)
                            {
                                await RenderBoundary();
                            }
                        });
                    }
                }

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    IsTrainingRunning = false;
                    UpdateStatus();
                    if (_currentEpoch >= Epochs)
                    {
                        Log($"Training completed at epoch {_currentEpoch}.");
                    }
                    else
                    {
                        Log("Training paused.");
                    }
                });
            }, token);
        }

        private void PauseTraining(object parameter)
        {
            _cancellationTokenSource?.Cancel();
        }

        private void ResetTraining(object parameter)
        {
            PauseTraining(null);
            _currentEpoch = 0;
            _maxSeenAbsoluteWeight = 1.0;
            _previousWeights.Clear();
            LogVM.TrainingLog.Clear();
            DashboardVM.ChartSeries[0].Values.Clear();
            DashboardVM.ChartSeries[1].Values.Clear();

            lock (_networkLock)
            {
                _neuralNetwork = BuildNetworkFromSettings();
            }

            if (_neuralNetwork != null)
            {
                ResetTrainingEngine();
                UpdateNetworkVisualWithWeights();
            }
            DataVM.SetData(_dataSet);
            UpdateStatus();

            if (_dataSet == null || !_dataSet.Any())
            {
                Log("System ready. Please load or generate data to begin.");
            }
        }

        private void UpdateStatus()
        {
            EpochProgressValue = Epochs > 0 ? (int)((double)_currentEpoch / Epochs * 100) : 0;
            string status = IsTrainingRunning ? "Training..." : "Paused";
            if (_currentEpoch >= Epochs) status = "Completed";
            if (!IsTrainingRunning && _currentEpoch == 0) status = "Ready";

            StatusText = $"Epoch: {_currentEpoch} / {Epochs} | {status}";
        }

        private void ResetTrainingEngine()
        {
            if (_neuralNetwork != null && _dataSet != null && _dataSet.Any())
            {
                _trainingEngine = new TrainingEngine(_neuralNetwork, _dataSet, LearningRate, _networkLock);
            }
            else
            {
                _trainingEngine = null;
            }
        }

        private void ToggleTheme(object parameter)
        {
            _currentTheme = _currentTheme == Theme.Dark ? Theme.Light : Theme.Dark;
            ThemeService.ApplyTheme(_currentTheme);
        }

        private void UpdateNetworkVisual()
        {
            ArchitectureVM.VisualLayers.Clear();
            ArchitectureVM.VisualConnections.Clear();

            const double layerSpacing = 150;
            const double neuronSpacing = 40;
            const double neuronRadius = 12;
            const double topPadding = 40;
            const double leftPadding = 50;

            var layerDefinitions = new List<Tuple<string, int>>();
            if (InputSize > 0) layerDefinitions.Add(Tuple.Create("Input", InputSize));

            var hiddenLayerSizes = HiddenLayersDefinition
                .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out int n) ? n : 0)
                .Where(n => n > 0).ToList();

            for (int i = 0; i < hiddenLayerSizes.Count; i++)
            {
                layerDefinitions.Add(Tuple.Create($"Hidden {i + 1}", hiddenLayerSizes[i]));
            }

            if (OutputSize > 0) layerDefinitions.Add(Tuple.Create("Output", OutputSize));

            if (!layerDefinitions.Any()) return;

            double maxLayerHeight = layerDefinitions.Max(l => (l.Item2 - 1) * neuronSpacing);
            double verticalOffset = maxLayerHeight / 2;
            NetworkLayerViewModel previousLayerVM = null;

            for (int i = 0; i < layerDefinitions.Count; i++)
            {
                var layerDef = layerDefinitions[i];
                var currentLayerVM = new NetworkLayerViewModel { Title = layerDef.Item1 };
                double layerHeight = (layerDef.Item2 - 1) * neuronSpacing;

                currentLayerVM.TitleX = leftPadding + i * layerSpacing;

                for (int j = 0; j < layerDef.Item2; j++)
                {
                    var neuronVM = new NeuronViewModel
                    {
                        Id = (j + 1).ToString(),
                        CenterX = currentLayerVM.TitleX,
                        CenterY = topPadding + verticalOffset + (j * neuronSpacing) - (layerHeight / 2)
                    };
                    currentLayerVM.Neurons.Add(neuronVM);

                    if (previousLayerVM != null)
                    {
                        foreach (var prevNeuron in previousLayerVM.Neurons)
                        {
                            ArchitectureVM.VisualConnections.Add(new ConnectionViewModel
                            {
                                X1 = prevNeuron.CenterX + neuronRadius,
                                Y1 = prevNeuron.CenterY,
                                X2 = neuronVM.CenterX - neuronRadius,
                                Y2 = neuronVM.CenterY,
                                Weight = 0.0
                            });
                        }
                    }
                }
                ArchitectureVM.VisualLayers.Add(currentLayerVM);
                previousLayerVM = currentLayerVM;
            }
        }

        private void UpdateNetworkVisualWithWeights()
        {
            UpdateNetworkVisual();
            UpdateConnectionWeights();
        }

        private void UpdateConnectionWeights()
        {
            lock (_networkLock)
            {
                if (_neuralNetwork == null || !ArchitectureVM.VisualConnections.Any()) return;

                double currentMaxWeight = GetMaxAbsoluteWeight(_neuralNetwork);
                if (currentMaxWeight > _maxSeenAbsoluteWeight)
                {
                    _maxSeenAbsoluteWeight = currentMaxWeight;
                }

                int connectionIndex = 0;
                foreach (var layer in _neuralNetwork.Layers)
                {
                    foreach (var neuron in layer.Neurons)
                    {
                        foreach (var weight in neuron.Weights)
                        {
                            if (connectionIndex < ArchitectureVM.VisualConnections.Count)
                            {
                                ArchitectureVM.VisualConnections[connectionIndex].Weight = weight / _maxSeenAbsoluteWeight;
                                connectionIndex++;
                            }
                        }
                    }
                }
            }
        }

        private double GetMaxAbsoluteWeight(NeuralNetwork network)
        {
            lock (_networkLock)
            {
                if (!network.Layers.Any() || !network.Layers.SelectMany(l => l.Neurons).SelectMany(n => n.Weights).Any())
                {
                    return 1.0;
                }

                var maxWeight = network.Layers
                    .SelectMany(l => l.Neurons)
                    .SelectMany(n => n.Weights)
                    .Max(w => Math.Abs(w));

                return maxWeight > 1e-9 ? maxWeight : 1.0;
            }
        }

        private void CaptureCurrentWeights()
        {
            lock (_networkLock)
            {
                if (_neuralNetwork == null)
                {
                    _previousWeights.Clear();
                    return;
                }
                _previousWeights = _neuralNetwork.Layers
                    .SelectMany(l => l.Neurons)
                    .SelectMany(n => n.Weights)
                    .ToList();
            }
        }

        private async void HighlightChangedConnections()
        {
            var currentWeights = GetCurrentWeights();
            if (_previousWeights.Count != currentWeights.Count || !currentWeights.Any()) return;

            var highlightedIndices = new HashSet<int>();
            int globalWeightIndex = 0;
            int previousLayerWeightCursor = 0;

            foreach (var layer in _neuralNetwork.Layers)
            {
                var layerWeightsCount = layer.Neurons.Sum(n => n.Weights.Count);
                if (layerWeightsCount == 0) continue;

                var deltas = new List<Tuple<int, double>>();
                for (int i = 0; i < layerWeightsCount; i++)
                {
                    int currentIndex = previousLayerWeightCursor + i;
                    double delta = Math.Abs(currentWeights[currentIndex] - _previousWeights[currentIndex]);
                    deltas.Add(new Tuple<int, double>(currentIndex, delta));
                }

                int highlightsPerLayer = Math.Max(1, layerWeightsCount / 10);
                var topChangesInLayer = deltas.OrderByDescending(d => d.Item2).Take(highlightsPerLayer);

                foreach (var change in topChangesInLayer)
                {
                    highlightedIndices.Add(change.Item1);
                }

                previousLayerWeightCursor += layerWeightsCount;
            }

            foreach (var index in highlightedIndices)
            {
                if (index < ArchitectureVM.VisualConnections.Count)
                {
                    ArchitectureVM.VisualConnections[index].IsHighlighted = true;
                }
            }

            await Task.Delay(200);

            foreach (var index in highlightedIndices)
            {
                if (index < ArchitectureVM.VisualConnections.Count)
                {
                    ArchitectureVM.VisualConnections[index].IsHighlighted = false;
                }
            }
        }

        private List<double> GetCurrentWeights()
        {
            lock (_networkLock)
            {
                if (_neuralNetwork == null) return new List<double>();
                return _neuralNetwork.Layers
                    .SelectMany(l => l.Neurons)
                    .SelectMany(n => n.Weights)
                    .ToList();
            }
        }

        private void GenerateData(object parameter)
        {
            if (parameter is DatasetType type)
            {
                _activeDatasetType = type;
                _dataSet = _dataGeneratorService.GenerateData(type, DataSampleCount, DataNoiseLevel);
                SelectedFilePath = string.Empty;
                Log($"Generated {type} dataset with {DataSampleCount} samples and {DataNoiseLevel:P0} noise.");
                ResetTraining(null);
            }
        }

        private void RegenerateActiveDataset()
        {
            if (!_activeDatasetType.HasValue || IsTrainingRunning) return;

            _dataSet = _dataGeneratorService.GenerateData(_activeDatasetType.Value, DataSampleCount, DataNoiseLevel);
            Log($"Auto-updated dataset: {_activeDatasetType.Value}, Samples: {DataSampleCount}, Noise: {DataNoiseLevel:P0}");
            ResetTraining(null);
        }

        private void LoadData(object parameter)
        {
            var openFileDialog = new OpenFileDialog { Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*" };
            if (openFileDialog.ShowDialog() == true)
            {
                _activeDatasetType = null;
                SelectedFilePath = openFileDialog.FileName;
                try
                {
                    _dataSet = _dataLoaderService.LoadData(SelectedFilePath, InputSize, OutputSize);
                    Log($"Data loaded successfully from {System.IO.Path.GetFileName(SelectedFilePath)}. {_dataSet.Count} records found.");
                    ResetTraining(null);
                }
                catch (DataFormatException ex)
                {
                    Log($"Data Loading Failed: {ex.Message}");
                    _dataSet = null;
                }
                catch (Exception ex)
                {
                    Log($"An unexpected error occurred while loading data: {ex.Message}");
                    _dataSet = null;
                }
            }
        }

        private void SaveModel(object parameter)
        {
            var saveFileDialog = new SaveFileDialog { Filter = "MLP Model (*.mlpjson)|*.mlpjson|All files (*.*)|*.*", Title = "Save Neural Network Model" };
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    lock (_networkLock)
                    {
                        if (_neuralNetwork != null)
                        {
                            _modelPersistenceService.SaveModel(_neuralNetwork, saveFileDialog.FileName);
                        }
                    }
                    Log("Model saved successfully.");
                }
                catch (Exception ex)
                {
                    Log($"Error saving model: {ex.Message}");
                }
            }
        }

        private void LoadModel(object parameter)
        {
            var openFileDialog = new OpenFileDialog { Filter = "MLP Model (*.mlpjson)|*.mlpjson|All files (*.*)|*.*", Title = "Load Neural Network Model" };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var loadedNetwork = _modelPersistenceService.LoadModel(openFileDialog.FileName);
                    lock (_networkLock)
                    {
                        _neuralNetwork = loadedNetwork;
                        InputSize = _neuralNetwork.Layers.First().Neurons.First().Weights.Count;
                        OutputSize = _neuralNetwork.Layers.Last().Neurons.Count;
                        var hiddenLayers = _neuralNetwork.Layers.Take(_neuralNetwork.Layers.Count - 1).Select(l => l.Neurons.Count).ToList();
                        HiddenLayersDefinition = string.Join(",", hiddenLayers);
                        SelectedActivationFunction = AvailableActivationFunctions.FirstOrDefault(kvp => kvp.Value.GetType() == _neuralNetwork.ActivationFunction.GetType());
                    }
                    ResetTraining(null);
                    Log("Model loaded successfully.");
                }
                catch (Exception ex)
                {
                    Log($"Error loading model: {ex.Message}");
                }
            }
        }

        private NeuralNetwork BuildNetworkFromSettings()
        {
            try
            {
                var hiddenLayerSizes = HiddenLayersDefinition
                    .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                return new NeuralNetwork(InputSize, OutputSize, SelectedActivationFunction.Value, hiddenLayerSizes);
            }
            catch
            {
                return null;
            }
        }

        private void InvalidateDatasetIfArchitectureChanges()
        {
            if (_dataSet != null)
            {
                _dataSet = null;
                _activeDatasetType = null;
                SelectedFilePath = string.Empty;
                Log("Network architecture changed. Please reload or regenerate a compatible data file.");
            }
        }

        private bool CanPlayTraining(object parameter) => !IsTrainingRunning && _dataSet != null && _dataSet.Any();
        private bool CanPauseTraining(object parameter) => IsTrainingRunning;
        private bool CanResetTraining(object parameter) => _currentEpoch > 0;
        private bool CanModifyState(object parameter) => !IsTrainingRunning;
        private bool CanRenderBoundary(object parameter) => !IsTrainingRunning && _dataSet != null && _dataSet.Any();
        private void Log(string message)
        {
            Application.Current?.Dispatcher.InvokeAsync(() => LogVM.TrainingLog.Add($"[{DateTime.Now:HH:mm:ss}] {message}"));
        }
    }
}
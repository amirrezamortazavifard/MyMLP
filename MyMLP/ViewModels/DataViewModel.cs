using MyMLP.Core;
using MyMLP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyMLP.ViewModels
{
    public class DataViewModel : ViewModelBase
    {
        private List<DataPoint> _originalDataSet;
        private double _canvasWidth = 1;
        private double _canvasHeight = 1;
        private WriteableBitmap _decisionBoundaryBitmap;

        private readonly Brush _class0Brush = new SolidColorBrush(Colors.SteelBlue);
        private readonly Brush _class1Brush = new SolidColorBrush(Colors.OrangeRed);

        public ObservableCollection<DataPointViewModel> Points { get; }
        public bool HasData => _originalDataSet != null && _originalDataSet.Any();

        public WriteableBitmap DecisionBoundaryBitmap
        {
            get => _decisionBoundaryBitmap;
            set { _decisionBoundaryBitmap = value; OnPropertyChanged(); }
        }

        public DataViewModel()
        {
            Points = new ObservableCollection<DataPointViewModel>();
            _class0Brush.Freeze();
            _class1Brush.Freeze();
        }

        public void SetData(List<DataPoint> dataSet)
        {
            _originalDataSet = dataSet;

            Points.Clear();
            DecisionBoundaryBitmap = null;
            OnPropertyChanged(nameof(HasData));

            if (_canvasWidth > 1 && _canvasHeight > 1)
            {
                RecalculatePointPositions();
            }
        }

        public void UpdateCanvasDimensions(double width, double height)
        {
            if (Math.Abs(_canvasWidth - width) < 1 && Math.Abs(_canvasHeight - height) < 1)
            {
                return;
            }

            _canvasWidth = width;
            _canvasHeight = height;

            if (_originalDataSet != null && _originalDataSet.Any())
            {
                RecalculatePointPositions();
            }
        }

        private void RecalculatePointPositions()
        {
            Points.Clear();

            if (_originalDataSet == null || !_originalDataSet.Any() || _originalDataSet.First().Features.Count < 2)
            {
                return;
            }

            var (scale, offsetX, offsetY, minX, minY) = GetTransformParameters();

            foreach (var dp in _originalDataSet)
            {
                var pointVM = new DataPointViewModel
                {
                    X = (dp.Features[0] - minX) * scale + offsetX,
                    Y = (dp.Features[1] - minY) * scale + offsetY,
                    Fill = dp.ExpectedOutput[0] > dp.ExpectedOutput[1] ? _class0Brush : _class1Brush
                };
                Points.Add(pointVM);
            }
        }

        public async Task RenderDecisionBoundary(NeuralNetwork network, object networkLock)
        {
            if (network == null || _originalDataSet == null || !_originalDataSet.Any()) return;

            int width = (int)_canvasWidth;
            int height = (int)_canvasHeight;

            if (width <= 1 || height <= 1) return;

            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
            var pixels = new uint[width * height];

            var (scale, offsetX, offsetY, minX, minY) = GetTransformParameters();

            Color color0 = ((SolidColorBrush)_class0Brush).Color;
            Color color1 = ((SolidColorBrush)_class1Brush).Color;

            await Task.Run(() =>
            {
                Parallel.For(0, height, y =>
                {
                    for (int x = 0; x < width; x++)
                    {
                        double dataX = minX + (x - offsetX) / scale;
                        double dataY = minY + (y - offsetY) / scale;

                        List<double> prediction;
                        lock (networkLock)
                        {
                            prediction = network.FeedForward(new List<double> { dataX, dataY });
                        }

                        double class0Prob = prediction[0];
                        double class1Prob = prediction[1];

                        byte r = (byte)(color0.R * class0Prob + color1.R * class1Prob);
                        byte g = (byte)(color0.G * class0Prob + color1.G * class1Prob);
                        byte b = (byte)(color0.B * class0Prob + color1.B * class1Prob);
                        byte a = 100;

                        pixels[y * width + x] = (uint)((a << 24) | (r << 16) | (g << 8) | b);
                    }
                });
            });

            bitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixels, width * 4, 0);
            bitmap.Freeze();
            DecisionBoundaryBitmap = bitmap;
        }

        private (double scale, double offsetX, double offsetY, double minX, double minY) GetTransformParameters()
        {
            if (_originalDataSet == null || !_originalDataSet.Any())
            {
                return (1, 0, 0, 0, 0);
            }

            var minX = _originalDataSet.Min(d => d.Features[0]);
            var maxX = _originalDataSet.Max(d => d.Features[0]);
            var minY = _originalDataSet.Min(d => d.Features[1]);
            var maxY = _originalDataSet.Max(d => d.Features[1]);

            double rangeX = maxX - minX;
            double rangeY = maxY - minY;

            if (rangeX < 1e-9) rangeX = 1;
            if (rangeY < 1e-9) rangeY = 1;

            double padding = 40;
            double availableWidth = _canvasWidth - 2 * padding;
            double availableHeight = _canvasHeight - 2 * padding;

            if (availableWidth <= 0) availableWidth = 1;
            if (availableHeight <= 0) availableHeight = 1;

            double scaleX = availableWidth / rangeX;
            double scaleY = availableHeight / rangeY;
            double scale = Math.Min(scaleX, scaleY);

            double offsetX = (_canvasWidth - rangeX * scale) / 2.0;
            double offsetY = (_canvasHeight - rangeY * scale) / 2.0;

            return (scale, offsetX, offsetY, minX, minY);
        }
    }
}
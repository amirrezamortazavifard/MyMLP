using System;
using System.Collections.Generic;
using System.Linq;
using MyMLP.Models;

namespace MyMLP.Core
{
    public class TrainingEngine
    {
        private readonly NeuralNetwork _network;
        private readonly List<DataPoint> _trainingData;
        private readonly double _learningRate;
        private readonly object _networkLock;

        public TrainingEngine(NeuralNetwork network, List<DataPoint> trainingData, double learningRate, object networkLock)
        {
            _network = network ?? throw new ArgumentNullException(nameof(network));
            _trainingData = trainingData ?? throw new ArgumentNullException(nameof(trainingData));
            _learningRate = learningRate;
            _networkLock = networkLock ?? throw new ArgumentNullException(nameof(networkLock));
        }

        public (double AverageError, double Accuracy) TrainSingleEpoch()
        {
            double totalError = 0;

            lock (_networkLock)
            {
                if (!_trainingData.Any())
                {
                    return (0, 0);
                }

                foreach (var dataPoint in _trainingData)
                {
                    var outputs = _network.FeedForward(dataPoint.Features);
                    totalError += CalculateMeanSquaredError(outputs, dataPoint.ExpectedOutput);
                    _network.Backpropagate(dataPoint.ExpectedOutput);
                    _network.UpdateWeights(dataPoint.Features, _learningRate);
                }
            }

            double averageError = totalError / _trainingData.Count;
            double accuracy = CalculateAccuracy();

            return (averageError, accuracy);
        }

        private double CalculateMeanSquaredError(List<double> actualOutputs, List<double> expectedOutputs)
        {
            double error = 0.0;
            for (int i = 0; i < actualOutputs.Count; i++)
            {
                error += Math.Pow(actualOutputs[i] - expectedOutputs[i], 2);
            }
            return error / actualOutputs.Count;
        }

        private double CalculateAccuracy()
        {
            int correctPredictions = 0;
            foreach (var dataPoint in _trainingData)
            {
                List<double> prediction;
                lock (_networkLock)
                {
                    prediction = _network.FeedForward(dataPoint.Features);
                }

                int predictedIndex = prediction.IndexOf(prediction.Max());
                int expectedIndex = dataPoint.ExpectedOutput.IndexOf(dataPoint.ExpectedOutput.Max());

                if (predictedIndex == expectedIndex)
                {
                    correctPredictions++;
                }
            }
            return (double)correctPredictions / _trainingData.Count;
        }
    }
}
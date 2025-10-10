using System;
using System.Collections.Generic;
using System.Linq;

namespace MyMLP.Core
{
    public class NeuralNetwork
    {
        public List<Layer> Layers { get; }
        public IActivationFunction ActivationFunction { get; }

        public NeuralNetwork(int inputSize, int outputSize, IActivationFunction activationFunction, params int[] hiddenLayerSizes)
        {
            ActivationFunction = activationFunction;
            Layers = new List<Layer>();
            var random = new Random();
            int currentInputSize = inputSize;

            if (hiddenLayerSizes != null)
            {
                foreach (var hiddenLayerSize in hiddenLayerSizes)
                {
                    Layers.Add(new Layer(hiddenLayerSize, currentInputSize, random, ActivationFunction));
                    currentInputSize = hiddenLayerSize;
                }
            }

            Layers.Add(new Layer(outputSize, currentInputSize, random, ActivationFunction));
        }

        public List<double> FeedForward(List<double> inputs)
        {
            var currentInputs = inputs;
            foreach (var layer in Layers)
            {
                currentInputs = layer.FeedForward(currentInputs);
            }
            return currentInputs;
        }

        public void Backpropagate(List<double> expectedOutputs)
        {
            for (int i = Layers.Count - 1; i >= 0; i--)
            {
                var currentLayer = Layers[i];
                var errors = new List<double>();

                if (i == Layers.Count - 1)
                {
                    for (int j = 0; j < currentLayer.Neurons.Count; j++)
                    {
                        errors.Add(expectedOutputs[j] - currentLayer.Neurons[j].Output);
                    }
                }
                else
                {
                    for (int j = 0; j < currentLayer.Neurons.Count; j++)
                    {
                        var error = 0.0;
                        var nextLayer = Layers[i + 1];
                        foreach (var neuron in nextLayer.Neurons)
                        {
                            error += neuron.Weights[j] * neuron.Delta;
                        }
                        errors.Add(error);
                    }
                }

                currentLayer.CalculateDeltas(errors);
            }
        }

        public void UpdateWeights(List<double> inputs, double learningRate)
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                var layer = Layers[i];
                var layerInputs = (i == 0) ? inputs : Layers[i - 1].Neurons.Select(n => n.Output).ToList();
                foreach (var neuron in layer.Neurons)
                {
                    for (int j = 0; j < layerInputs.Count; j++)
                    {
                        neuron.Weights[j] += learningRate * neuron.Delta * layerInputs[j];
                    }
                    neuron.Bias += learningRate * neuron.Delta;
                }
            }
        }
    }
}
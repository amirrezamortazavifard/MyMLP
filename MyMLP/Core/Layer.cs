using System;
using System.Collections.Generic;

namespace MyMLP.Core
{
    public class Layer
    {
        public List<Neuron> Neurons { get; set; }
        private readonly IActivationFunction _activationFunction;

        public Layer(int neuronCount, int inputCountPerNeuron, Random random, IActivationFunction activationFunction)
        {
            _activationFunction = activationFunction;
            Neurons = new List<Neuron>(neuronCount);
            for (int i = 0; i < neuronCount; i++)
            {
                Neurons.Add(new Neuron(inputCountPerNeuron, random));
            }
        }

        public List<double> FeedForward(List<double> inputs)
        {
            var outputs = new List<double>(Neurons.Count);
            foreach (var neuron in Neurons)
            {
                double weightedSum = 0.0;
                for (int i = 0; i < inputs.Count; i++)
                {
                    weightedSum += inputs[i] * neuron.Weights[i];
                }
                weightedSum += neuron.Bias;

                neuron.Output = _activationFunction.Activate(weightedSum);
                outputs.Add(neuron.Output);
            }
            return outputs;
        }

        public void CalculateDeltas(List<double> errors)
        {
            for (int i = 0; i < Neurons.Count; i++)
            {
                var neuron = Neurons[i];
                neuron.Delta = errors[i] * _activationFunction.Derivative(neuron.Output);
            }
        }
    }
}
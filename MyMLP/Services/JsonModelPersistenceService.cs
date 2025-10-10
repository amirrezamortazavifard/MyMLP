using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MyMLP.Core;
using MyMLP.DTOs;

namespace MyMLP.Services
{
    public class JsonModelPersistenceService : IModelPersistenceService
    {
        private const string CurrentVersion = "1.1.0";

        public void SaveModel(NeuralNetwork network, string filePath)
        {
            var modelDto = MapToDto(network);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(modelDto, options);
            File.WriteAllText(filePath, jsonString);
        }

        public NeuralNetwork LoadModel(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            var modelDto = JsonSerializer.Deserialize<ModelDto>(jsonString);

            if (modelDto.Version != CurrentVersion)
            {
                throw new InvalidOperationException($"Model version mismatch. Expected '{CurrentVersion}', but file is version '{modelDto.Version}'.");
            }

            return MapFromDto(modelDto);
        }

        private IActivationFunction CreateActivationFunctionFromName(string name)
        {
            switch (name)
            {
                case nameof(Sigmoid):
                    return new Sigmoid();
                case nameof(ReLU):
                    return new ReLU();
                case nameof(Tanh):
                    return new Tanh();
                default:
                    throw new NotSupportedException($"Activation function '{name}' is not supported.");
            }
        }

        private ModelDto MapToDto(NeuralNetwork network)
        {
            var architecture = new ArchitectureDto
            {
                InputSize = network.Layers.First().Neurons.First().Weights.Count,
                HiddenLayerSizes = network.Layers.Take(network.Layers.Count - 1).Select(l => l.Neurons.Count).ToList(),
                OutputSize = network.Layers.Last().Neurons.Count,
                ActivationFunction = network.ActivationFunction.GetType().Name
            };

            var layerDtos = network.Layers.Select(layer => new LayerDto
            {
                Neurons = layer.Neurons.Select(neuron => new NeuronDto
                {
                    Bias = neuron.Bias,
                    Weights = new List<double>(neuron.Weights)
                }).ToList()
            }).ToList();

            return new ModelDto
            {
                Version = CurrentVersion,
                Architecture = architecture,
                Layers = layerDtos
            };
        }

        private NeuralNetwork MapFromDto(ModelDto dto)
        {
            var activationFunction = CreateActivationFunctionFromName(dto.Architecture.ActivationFunction);

            var network = new NeuralNetwork(
                dto.Architecture.InputSize,
                dto.Architecture.OutputSize,
                activationFunction,
                dto.Architecture.HiddenLayerSizes.ToArray());

            for (int i = 0; i < dto.Layers.Count; i++)
            {
                for (int j = 0; j < dto.Layers[i].Neurons.Count; j++)
                {
                    network.Layers[i].Neurons[j].Bias = dto.Layers[i].Neurons[j].Bias;
                    network.Layers[i].Neurons[j].Weights = new List<double>(dto.Layers[i].Neurons[j].Weights);
                }
            }

            return network;
        }
    }
}
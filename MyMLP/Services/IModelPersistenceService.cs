using MyMLP.Core;

namespace MyMLP.Services
{
    public interface IModelPersistenceService
    {
        void SaveModel(NeuralNetwork network, string filePath);
        NeuralNetwork LoadModel(string filePath);
    }
}
using System.Collections.Generic;
using MyMLP.Models;

namespace MyMLP.Services
{
    public interface IDataLoaderService
    {
        List<DataPoint> LoadData(string filePath, int inputSize, int outputSize);
    }
}
using MyMLP.Models;
using System.Collections.Generic;

namespace MyMLP.Services
{
    public enum DatasetType
    {
        Circle,
        XOR,
        Spiral,
        TwoMoons,
        Gaussian,
        FourCorners,
        CenterRing,
        SineWave
    }

    public interface IDataGeneratorService
    {
        List<DataPoint> GenerateData(DatasetType type, int sampleCount, double noise);
    }
}
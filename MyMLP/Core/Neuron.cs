using System;
using System.Collections.Generic;
using System.Linq;

namespace MyMLP.Core
{
    public class Neuron
    {
        public List<double> Weights { get; set; }
        public double Bias { get; set; }
        public double Output { get; set; }
        public double Delta { get; set; }

        public Neuron(int inputCount, Random random)
        {
            Bias = random.NextDouble() * 2 - 1;
            Weights = new List<double>(inputCount);
            for (int i = 0; i < inputCount; i++)
            {
                Weights.Add(random.NextDouble() * 2 - 1);
            }
        }
    }
}
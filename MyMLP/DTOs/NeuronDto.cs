using System.Collections.Generic;

namespace MyMLP.DTOs
{
    public class NeuronDto
    {
        public double Bias { get; set; }
        public List<double> Weights { get; set; }
    }
}
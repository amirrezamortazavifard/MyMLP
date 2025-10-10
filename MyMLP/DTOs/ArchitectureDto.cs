using System.Collections.Generic;

namespace MyMLP.DTOs
{
    public class ArchitectureDto
    {
        public int InputSize { get; set; }
        public List<int> HiddenLayerSizes { get; set; }
        public int OutputSize { get; set; }
        public string ActivationFunction { get; set; }
    }
}
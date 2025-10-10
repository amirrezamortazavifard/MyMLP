using System.Collections.Generic;

namespace MyMLP.DTOs
{
    public class ModelDto
    {
        public string Version { get; set; }
        public ArchitectureDto Architecture { get; set; }
        public List<LayerDto> Layers { get; set; }
    }
}
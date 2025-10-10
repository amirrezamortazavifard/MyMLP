using System.Collections.Generic;

namespace MyMLP.Models
{
    public class DataPoint
    {
        public List<double> Features { get; set; }
        public List<double> ExpectedOutput { get; set; }

        public DataPoint()
        {
            Features = new List<double>();
            ExpectedOutput = new List<double>();
        }
    }
}
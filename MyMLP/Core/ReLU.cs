using System;

namespace MyMLP.Core
{
    public class ReLU : IActivationFunction
    {
        public double Activate(double x)
        {
            return Math.Max(0, x);
        }

        public double Derivative(double output)
        {
            return output > 0 ? 1 : 0;
        }
    }
}
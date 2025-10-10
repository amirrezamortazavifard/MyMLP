using System;

namespace MyMLP.Core
{
    public class Tanh : IActivationFunction
    {
        public double Activate(double x)
        {
            return Math.Tanh(x);
        }

        public double Derivative(double output)
        {
            return 1 - (output * output);
        }
    }
}
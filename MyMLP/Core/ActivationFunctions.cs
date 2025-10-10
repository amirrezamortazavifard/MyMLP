using System;

namespace MyMLP.Core
{
    public static class ActivationFunctions
    {
        public static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        public static double SigmoidDerivative(double x)
        {
            return x * (1.0 - x);
        }
    }
}
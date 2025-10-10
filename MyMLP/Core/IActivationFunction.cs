namespace MyMLP.Core
{
    public interface IActivationFunction
    {
        double Activate(double x);
        double Derivative(double output);
    }
}
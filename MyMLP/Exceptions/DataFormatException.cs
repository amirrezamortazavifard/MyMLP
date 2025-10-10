using System;

namespace MyMLP.Exceptions
{
    public class DataFormatException : Exception
    {
        public DataFormatException()
        {
        }

        public DataFormatException(string message)
            : base(message)
        {
        }

        public DataFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
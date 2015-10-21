using System;

namespace Kaliber3D
{
    public class UnitsNetException : Exception
    {
        public UnitsNetException()
        {
        }

        public UnitsNetException(string message)
            : base(message)
        {
        }

        public UnitsNetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
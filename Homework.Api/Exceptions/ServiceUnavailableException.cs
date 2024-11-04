using System;

namespace Homework.Api.Exceptions
{
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

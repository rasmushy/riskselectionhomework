using System;

namespace Homework.Api.Exceptions
{
    public class JsonParseException : Exception
    {
        public JsonParseException(string message) : base(message) { }

        public JsonParseException(string message, Exception innerException) : base(message, innerException) { }
    }
}

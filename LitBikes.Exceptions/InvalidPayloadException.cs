using System;

namespace LitBikes.Exceptions
{
    public class InvalidPayloadException : Exception
    {
        public InvalidPayloadException(string message) : base(message) { }
    }
}

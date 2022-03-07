using System;


namespace RallyDakar.CustomExceptions
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException()
        {
        }
        public InvalidStateException(string message)
            : base(message)
        {
        }
    }
}

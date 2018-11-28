using System;

namespace Filehook.Abstractions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class FilehookException : Exception
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
    {
        public FilehookException() : base()
        {
        }

        public FilehookException(string message) : base(message)
        {
        }

        public FilehookException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

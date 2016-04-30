using System;

namespace NotSoTotalCommanderApp.Exceptions
{
    internal class InvalidPathException : FileSystemException
    {
        public InvalidPathException(string filePath, string message = "", Exception innerException = null) : base(filePath, message, innerException)
        {
        }
    }
}
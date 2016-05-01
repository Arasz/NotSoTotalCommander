using System;

namespace NotSoTotalCommanderApp.Exceptions
{
    internal class DirectoryCreationException : FileSystemException
    {
        public DirectoryCreationException(string filePath, string message = "", Exception innerException = null) : base(filePath, message, innerException)
        {
        }
    }
}
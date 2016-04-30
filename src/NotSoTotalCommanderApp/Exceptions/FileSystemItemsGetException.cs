using System;

namespace NotSoTotalCommanderApp.Exceptions
{
    internal class FileSystemItemsGetException : FileSystemException
    {
        public FileSystemItemsGetException(string filePath, string message = "", Exception innerException = null) : base(filePath, message, innerException)
        {
        }
    }
}
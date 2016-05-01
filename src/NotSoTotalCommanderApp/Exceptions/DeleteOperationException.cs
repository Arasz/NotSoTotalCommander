using System;

namespace NotSoTotalCommanderApp.Exceptions
{
    internal class DeleteOperationException : FileSystemException
    {
        public DeleteOperationException(string filePath, string message = "", Exception innerException = null) : base(filePath, message, innerException)
        {
        }
    }
}
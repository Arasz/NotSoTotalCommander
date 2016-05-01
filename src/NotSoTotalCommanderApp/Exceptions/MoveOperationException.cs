using System;

namespace NotSoTotalCommanderApp.Exceptions
{
    internal class MoveOperationException : FileSystemException
    {
        public string DestinationPath { get; }

        public MoveOperationException(string sourcePath, string destinationPath, string message = "", Exception innerException = null) : base(sourcePath, message, innerException)
        {
            DestinationPath = destinationPath;
        }
    }
}
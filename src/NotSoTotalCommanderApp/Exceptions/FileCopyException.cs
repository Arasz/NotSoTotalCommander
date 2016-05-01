using System;

namespace NotSoTotalCommanderApp.Exceptions
{
    internal class FileCopyException : FileSystemException
    {
        public string DestinationPath { get; }

        public FileCopyException(string soruceFile, string destinationPath, string message = "", Exception innerException = null) : base(soruceFile, message, innerException)
        {
            DestinationPath = destinationPath;
        }
    }
}
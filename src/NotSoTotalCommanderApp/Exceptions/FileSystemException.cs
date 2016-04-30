using System;

namespace NotSoTotalCommanderApp.Exceptions
{
    internal class FileSystemException : Exception
    {
        public string Path { get; }

        public FileSystemException(string filePath, string message = "", Exception innerException = null) : base(message, innerException)
        {
            Path = filePath;
        }
    }
}
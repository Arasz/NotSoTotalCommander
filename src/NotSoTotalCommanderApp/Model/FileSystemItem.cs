using System;
using System.IO;

namespace NotSoTotalCommanderApp.Model
{
    /// <summary>
    /// FIle system info view model 
    /// </summary>
    public class FileSystemItem
    {
        /// <summary>
        /// Decorated object 
        /// </summary>
        private readonly FileSystemInfo _fileSystemInfo;

        public FileAttributes Attributes => _fileSystemInfo.Attributes;

        public DateTime CreationTime => _fileSystemInfo.CreationTime;

        public bool Exists => _fileSystemInfo.Exists;

        public string Extension => _fileSystemInfo.Extension.TrimStart('.');

        public string FullName => _fileSystemInfo.Name;

        public DateTime LastAccessTime => _fileSystemInfo.LastAccessTime;

        public DateTime LastWriteTime => _fileSystemInfo.LastWriteTime;

        public string Name => _fileSystemInfo.Name;

        /// <summary>
        /// Returns -1 when fileSystemInfo is directory otherwise returns file length or null when
        /// file have no length
        /// </summary>
        public long Size => (_fileSystemInfo as FileInfo)?.Length ?? -1;

        /// <summary>
        /// File system tree traversal direction 
        /// </summary>
        public TraversalDirection TraversalDirection { get; set; }

        public FileSystemItem(FileSystemInfo fileSystemInfo, TraversalDirection traversalDirection = TraversalDirection.Down)
        {
            TraversalDirection = traversalDirection;
            _fileSystemInfo = fileSystemInfo;
        }

        public override bool Equals(object obj)
        {
            return _fileSystemInfo.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _fileSystemInfo.GetHashCode();
        }

        public override string ToString()
        {
            return _fileSystemInfo.ToString();
        }
    }

    /// <summary>
    /// File system tree traversal direction 
    /// </summary>
    public enum TraversalDirection
    {
        Down,
        Up,
    }
}
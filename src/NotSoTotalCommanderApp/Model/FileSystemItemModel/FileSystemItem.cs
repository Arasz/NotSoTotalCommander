using System;
using System.IO;

namespace NotSoTotalCommanderApp.Model.FileSystemItemModel
{
    /// <summary>
    /// FIle system info view model 
    /// </summary>
    public class FileSystemItem : IFileSystemItem
    {
        /// <summary>
        /// Decorated object 
        /// </summary>
        private readonly FileSystemInfo _fileSystemInfo;

        public FileAttributes Attributes => _fileSystemInfo.Attributes;

        public DateTime CreationTime => _fileSystemInfo.CreationTime;

        public bool Exists => _fileSystemInfo.Exists;

        public string Extension => _fileSystemInfo.Extension.TrimStart('.');

        public string FullName => _fileSystemInfo.FullName;

        public bool IsDirectory { get; }

        public DateTime LastAccessTime => _fileSystemInfo.LastAccessTime;

        public DateTime LastWriteTime => _fileSystemInfo.LastWriteTime;

        public string Name => _fileSystemInfo.Name;

        public string Path => ToString();

        public long Size => (_fileSystemInfo as FileInfo)?.Length ?? -1;

        public TraversalDirection TraversalDirection { get; set; }

        public FileSystemItem(FileSystemInfo fileSystemInfo, TraversalDirection traversalDirection = TraversalDirection.Down)
        {
            TraversalDirection = traversalDirection;
            _fileSystemInfo = fileSystemInfo;
            IsDirectory = _fileSystemInfo is DirectoryInfo;
        }

        public override bool Equals(object obj) => _fileSystemInfo.Equals(obj);

        public override int GetHashCode() => _fileSystemInfo.GetHashCode();

        public override string ToString() => _fileSystemInfo.ToString();
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
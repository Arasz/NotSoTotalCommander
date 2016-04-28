using System;
using System.IO;

namespace NotSoTotalCommanderApp.Model
{
    internal class FileSystemBackItemProxy : IFileSystemItem
    {
        private readonly IFileSystemItem _fileSystemItem;

        public FileAttributes Attributes { get; } = 0;

        public DateTime CreationTime { get; } = DateTime.Now;

        public bool Exists => _fileSystemItem.Exists;

        public string Extension { get; } = "";

        public string FullName { get; } = "";

        public bool IsDirectory => _fileSystemItem.IsDirectory;

        public DateTime LastAccessTime { get; } = DateTime.Now;

        public DateTime LastWriteTime { get; } = DateTime.Now;

        public string Name { get; } = "..";

        public string Path => ToString();

        public long Size { get; } = -1;

        public TraversalDirection TraversalDirection { get; set; }

        public FileSystemBackItemProxy(IFileSystemItem fileSystemItem)
        {
            _fileSystemItem = fileSystemItem;
        }

        public override bool Equals(object obj) => _fileSystemItem.Equals(obj);

        public override int GetHashCode() => _fileSystemItem.GetHashCode();

        public override string ToString() => _fileSystemItem.ToString();
    }
}
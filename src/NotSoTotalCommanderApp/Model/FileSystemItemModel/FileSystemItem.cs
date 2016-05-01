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

        public IconType IconType { get; }

        public bool IsDirectory { get; }

        public DateTime LastAccessTime => _fileSystemInfo.LastAccessTime;

        public DateTime LastWriteTime => _fileSystemInfo.LastWriteTime;

        public string Name => _fileSystemInfo.Name;

        public string Path => FullName;

        public long Size => (_fileSystemInfo as FileInfo)?.Length ?? -1;

        public TraversalDirection TraversalDirection { get; }

        public FileSystemItem(FileSystemInfo fileSystemInfo, TraversalDirection traversalDirection = TraversalDirection.Down)
        {
            TraversalDirection = traversalDirection;
            _fileSystemInfo = fileSystemInfo;
            IsDirectory = _fileSystemInfo is DirectoryInfo;

            if (IsDirectory)
            {
                if ((Attributes & FileAttributes.System) != 0)
                    IconType = IconType.RestricedDirectory;
                else
                    IconType = IconType.Directory;
            }
            else
            {
                if ((Attributes & FileAttributes.System) != 0)
                    IconType = IconType.RestrictedFile;
                else
                    IconType = IconType.File;
            }
        }

        public override int GetHashCode() => _fileSystemInfo.GetHashCode();

        public override string ToString() => _fileSystemInfo.ToString();
    }

    /// <summary>
    /// File system tree traversal direction 
    /// </summary>
    public enum TraversalDirection
    {
        Down = 0,
        Up = 1,
    }
}
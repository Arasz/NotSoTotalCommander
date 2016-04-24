using System;
using System.IO;

namespace NotSoTotalCommanderApp.Model
{
    /// <summary>
    /// </summary>
    public class ExtendedFileSystemInfo : FileSystemInfo
    {
        /// <summary>
        /// Decorated object 
        /// </summary>
        private readonly FileSystemInfo _fileSystemInfo;

        public new FileAttributes Attributes
        {
            get { return _fileSystemInfo.Attributes; }
            set { _fileSystemInfo.Attributes = value; }
        }

        public new DateTime CreationTime
        {
            get { return _fileSystemInfo.CreationTime; }
            set { _fileSystemInfo.CreationTime = value; }
        }

        public override bool Exists => _fileSystemInfo.Exists;

        public new string Extension
        {
            get { return _fileSystemInfo.Extension; }
            set { throw new NotImplementedException(); }
        }

        public override string FullName => _fileSystemInfo.Name;

        public new DateTime LastAccessTime
        {
            get { return _fileSystemInfo.LastAccessTime; }
            set { _fileSystemInfo.LastAccessTime = value; }
        }

        public override string Name => _fileSystemInfo.Name;

        public long? Size { get; }

        public ExtendedFileSystemInfo(FileSystemInfo fileSystemInfo)
        {
            _fileSystemInfo = fileSystemInfo;

            var concreateInfo = _fileSystemInfo as FileInfo;

            Size = concreateInfo?.Length;
        }

        public override void Delete() => _fileSystemInfo.Delete();
    }
}
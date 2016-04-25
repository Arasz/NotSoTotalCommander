using System;
using System.IO;
using System.Security;

namespace NotSoTotalCommanderApp.Model
{
    /// <summary>
    /// </summary>
    public class ExtendedFileSystemInfo
    {
        /// <summary>
        /// Decorated object 
        /// </summary>
        private readonly FileSystemInfo _fileSystemInfo;

        public FileAttributes Attributes
        {
            get { return _fileSystemInfo.Attributes; }
            set { _fileSystemInfo.Attributes = value; }
        }

        public DateTime CreationTime
        {
            get { return _fileSystemInfo.CreationTime; }
            set { _fileSystemInfo.CreationTime = value; }
        }

        public DateTime CreationTimeUtc
        {
            get { return _fileSystemInfo.CreationTimeUtc; }
            set { _fileSystemInfo.CreationTimeUtc = value; }
        }

        public bool Exists => _fileSystemInfo.Exists;

        public string Extension
        {
            get { return _fileSystemInfo.Extension; }
            set { throw new NotImplementedException(); }
        }

        public string FullName => _fileSystemInfo.Name;

        public DateTime LastAccessTime
        {
            get { return _fileSystemInfo.LastAccessTime; }
            set { _fileSystemInfo.LastAccessTime = value; }
        }

        public DateTime LastAccessTimeUtc
        {
            get { return _fileSystemInfo.LastAccessTimeUtc; }
            set { _fileSystemInfo.LastAccessTimeUtc = value; }
        }

        public DateTime LastWriteTime
        {
            get { return _fileSystemInfo.LastWriteTime; }
            set { _fileSystemInfo.LastWriteTime = value; }
        }

        public DateTime LastWriteTimeUtc
        {
            get { return _fileSystemInfo.LastWriteTimeUtc; }
            set { _fileSystemInfo.LastWriteTimeUtc = value; }
        }

        public string Name => _fileSystemInfo.Name;

        /// <summary>
        /// Returns -1 when fileSystemInfo is directory otherwise returns file length or null when
        /// file have no length
        /// </summary>
        public long? Size { get; }

        public ExtendedFileSystemInfo(FileSystemInfo fileSystemInfo)
        {
            _fileSystemInfo = fileSystemInfo;

            Size = (_fileSystemInfo as FileInfo)?.Length;
        }

        public void Delete() => _fileSystemInfo.Delete();

        public override bool Equals(object obj)
        {
            return _fileSystemInfo.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _fileSystemInfo.GetHashCode();
        }

        /// <summary>
        /// Refreshes the state of the object. 
        /// </summary>
        [SecuritySafeCritical]
        public void Refresh() => _fileSystemInfo.Refresh();

        public override string ToString()
        {
            return _fileSystemInfo.ToString();
        }
    }
}
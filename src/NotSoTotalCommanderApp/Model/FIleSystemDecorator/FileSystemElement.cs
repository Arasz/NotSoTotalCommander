using System;
using System.IO;
using System.Security;

namespace NotSoTotalCommanderApp.Model.FIleSystemDecorator
{
    /// <summary>
    /// </summary>
    public class FileSystemElement
    {
        /// <summary>
        /// Decorated object 
        /// </summary>
        protected readonly FileSystemInfo _fileSystemInfo;

        public virtual FileAttributes Attributes
        {
            get { return _fileSystemInfo.Attributes; }
            set { _fileSystemInfo.Attributes = value; }
        }

        public virtual DateTime CreationTime
        {
            get { return _fileSystemInfo.CreationTime; }
            set { _fileSystemInfo.CreationTime = value; }
        }

        public virtual DateTime CreationTimeUtc
        {
            get { return _fileSystemInfo.CreationTimeUtc; }
            set { _fileSystemInfo.CreationTimeUtc = value; }
        }

        public virtual bool Exists => _fileSystemInfo.Exists;

        public virtual string Extension
        {
            get { return _fileSystemInfo.Extension; }
            set { throw new NotImplementedException(); }
        }

        public virtual string FullName => _fileSystemInfo.Name;

        public virtual DateTime LastAccessTime
        {
            get { return _fileSystemInfo.LastAccessTime; }
            set { _fileSystemInfo.LastAccessTime = value; }
        }

        public virtual DateTime LastAccessTimeUtc
        {
            get { return _fileSystemInfo.LastAccessTimeUtc; }
            set { _fileSystemInfo.LastAccessTimeUtc = value; }
        }

        public virtual DateTime LastWriteTime
        {
            get { return _fileSystemInfo.LastWriteTime; }
            set { _fileSystemInfo.LastWriteTime = value; }
        }

        public virtual DateTime LastWriteTimeUtc
        {
            get { return _fileSystemInfo.LastWriteTimeUtc; }
            set { _fileSystemInfo.LastWriteTimeUtc = value; }
        }

        public virtual string Name => _fileSystemInfo.Name;

        /// <summary>
        /// Returns -1 when fileSystemInfo is directory otherwise returns file length or null when
        /// file have no length
        /// </summary>
        public virtual long? Size { get; }

        public FileSystemElement(FileSystemInfo fileSystemInfo)
        {
            _fileSystemInfo = fileSystemInfo;

            Size = (_fileSystemInfo as FileInfo)?.Length;
        }

        public virtual void Delete() => _fileSystemInfo.Delete();

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
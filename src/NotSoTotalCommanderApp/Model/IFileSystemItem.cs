using System;
using System.IO;

namespace NotSoTotalCommanderApp.Model
{
    public interface IFileSystemItem
    {
        FileAttributes Attributes { get; }

        DateTime CreationTime { get; }

        bool Exists { get; }

        string Extension { get; }

        string FullName { get; }

        /// <summary>
        /// True if <see cref="IFileSystemItem"/> is directory, false if file 
        /// </summary>
        bool IsDirectory { get; }

        DateTime LastAccessTime { get; }

        DateTime LastWriteTime { get; }

        /// <summary>
        /// File or directory name 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// File system item location path 
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Returns -1 when fileSystemInfo is directory otherwise returns file length or null when
        /// file have no length
        /// </summary>
        long Size { get; }

        /// <summary>
        /// File system tree traversal direction 
        /// </summary>
        TraversalDirection TraversalDirection { get; set; }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NotSoTotalCommanderApp.Model
{
    /// <summary>
    /// Responsible for operations on file system 
    /// </summary>
    public class FileSystemExplorerModel
    {
        private Dictionary<string, IEnumerable<ExtendedFileSystemInfo>> _fileSystemInfoCache = new Dictionary<string, IEnumerable<ExtendedFileSystemInfo>>();

        public string CurrentDirectory { get; set; }

        public string GetCurrentDirectoryParent
        {
            get
            {
                var root = Directory.GetParent(CurrentDirectory)?.ToString();

                return string.IsNullOrEmpty(root) ? null : root;
            }
        }

        public string[] SystemDrives => Directory.GetLogicalDrives();

        public FileSystemExplorerModel()
        {
        }

        /// <summary>
        /// Retrieves informations about all file system items under given <paramref name="path"/> 
        /// </summary>
        /// <param name="path"> Path from which files system items will be retrived </param>
        /// <exception cref="IOException"> Can't read files or directories for given path </exception>
        /// <returns> Collection of informations about file system items (files, dictionaries) </returns>
        public IEnumerable<ExtendedFileSystemInfo> GetAllItemsUnderPath(string path)
        {
            if (!Directory.Exists(path))
                return null;

            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            if (directories == null || files == null)
                throw new IOException("Can't read files or directories for given path");

            var dirInfos = directories.Select(dict => new ExtendedFileSystemInfo(new DirectoryInfo(dict)));
            var filesInfos = files.Select(file => new ExtendedFileSystemInfo(new FileInfo(file)));

            return dirInfos.Concat(filesInfos);
        }
    }
}
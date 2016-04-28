using GalaSoft.MvvmLight.Messaging;
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
        private Dictionary<string, IEnumerable<FileSystemItem>> _fileSystemInfoCache = new Dictionary<string, IEnumerable<FileSystemItem>>();

        private IMessenger _messenger;

        public string CurrentDirectory { get; set; }

        public string GetCurrentDirectoryParent
        {
            get
            {
                var root = Directory.GetParent(CurrentDirectory)?.ToString();

                return string.IsNullOrEmpty(root) ? null : root;
            }
        }

        public IEnumerable<IFileSystemItem> ItemsClipboard { get; set; }

        public string[] SystemDrives => Directory.GetLogicalDrives();

        public FileSystemExplorerModel(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public void Copy(IEnumerable<IFileSystemItem> items)
        {
            ItemsClipboard = items.ToList();
        }

        /// <summary>
        /// Creates directory inside current directory 
        /// </summary>
        public void CreateDirectory(string directoryName)
        {
            Directory.CreateDirectory(ConstructNewPath(CurrentDirectory, directoryName));
        }

        /// <summary>
        /// </summary>
        /// <param name="items"></param>
        public void Delete(IEnumerable<IFileSystemItem> items)
        {
            foreach (var fileSystemItem in items)
            {
                if (fileSystemItem.IsDirectory)
                    Directory.Delete(fileSystemItem.Path, true);
                else
                    File.Delete(fileSystemItem.Path);
            }
        }

        /// <summary>
        /// Retrieves informations about all file system items under given <paramref name="path"/> 
        /// </summary>
        /// <param name="path"> Path from which files system items will be retrived </param>
        /// <exception cref="IOException"> Can't read files or directories for given path </exception>
        /// <returns> Collection of informations about file system items (files, dictionaries) </returns>
        public IEnumerable<IFileSystemItem> GetAllItemsUnderPath(string path)
        {
            if (!Directory.Exists(path))
                return null;

            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            if (directories == null || files == null)
                throw new IOException("Can't read files or directories for given path");

            var dirInfos = directories.Select(dict => new FileSystemItem(new DirectoryInfo(dict)));
            var filesInfos = files.Select(file => new FileSystemItem(new FileInfo(file)));

            return dirInfos.Concat(filesInfos);
        }

        /// <summary>
        /// Copy items from collection inside current directory 
        /// </summary>
        /// <param name="fileSystemItemsToPast"></param>
        /// <returns></returns>
        public void Past(IEnumerable<IFileSystemItem> fileSystemItemsToPast = null, bool canOverwrite = false)
        {
            var itemsToPast = fileSystemItemsToPast ?? ItemsClipboard;

            foreach (var fileSystemItem in itemsToPast)
            {
                if (fileSystemItem.IsDirectory)
                    Directory.CreateDirectory(ConstructNewPath(CurrentDirectory, fileSystemItem.Name));
                else
                    File.Copy(fileSystemItem.Path, ConstructNewPath(CurrentDirectory, fileSystemItem.Name), canOverwrite);
            }
        }

        /// <summary>
        /// Creats new path from given base path and new file system item name 
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string ConstructNewPath(string basePath, string name) => $"{basePath}\\{name}";
    }
}
using log4net;
using NotSoTotalCommanderApp.Exceptions;
using NotSoTotalCommanderApp.Model.FileSystemItemModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NotSoTotalCommanderApp.Model
{
    public class FileSystemExplorerModel
    {
        private ILog _logger = LogManager.GetLogger(typeof(FileSystemExplorerModel));

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

        public FileSystemExplorerModel()
        {
        }

        public void Copy(IEnumerable<IFileSystemItem> items)
        {
            ItemsClipboard = items.ToList();
        }

        /// <summary>
        /// Creates directory inside current directory 
        /// </summary>
        /// <exception cref="InvalidPathException">
        /// Thrown when given directory name is not valid.
        /// </exception>
        public void CreateDirectory(string directoryName)
        {
            try
            {
                var path = Path.Combine(CurrentDirectory, directoryName);
                Directory.CreateDirectory(path);
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                _logger.Info($"Unauthorized access to {CurrentDirectory}", unauthorizedAccessException);
            }
            catch (DirectoryNotFoundException exception)
            {
                _logger.Error($"Given directory name: {directoryName}, is invalid", exception);
                throw new InvalidPathException(Path.Combine(CurrentDirectory, directoryName), innerException: exception);
            }
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
        public void Paste(IEnumerable<IFileSystemItem> fileSystemItemsToPast = null, bool canOverwrite = false, bool inDepth = false)
        {
            var itemsToPast = fileSystemItemsToPast ?? ItemsClipboard;
            var currentDirectoryTmp = CurrentDirectory;

            foreach (var fileSystemItem in itemsToPast)
            {
                if (fileSystemItem.IsDirectory)
                {
                    Directory.CreateDirectory(Path.Combine(CurrentDirectory, fileSystemItem.Name));
                    if (inDepth)
                    {
                        CurrentDirectory = Path.Combine(CurrentDirectory, fileSystemItem.Name);
                        Paste(GetAllItemsUnderPath(fileSystemItem.Path), canOverwrite, inDepth);
                    }
                }
                else
                    File.Copy(fileSystemItem.Path, Path.Combine(CurrentDirectory, fileSystemItem.Name), canOverwrite);
            }

            CurrentDirectory = currentDirectoryTmp;
        }

        /// <summary>
        /// Walks through directories tree and preforms some action on each directory and file 
        /// </summary>
        /// <param name="root"> Root directory </param>
        /// <param name="fileAction"> Action preformed on file </param>
        /// <param name="directoryAction"> Action preformed on directory </param>
        private void WalkDirectoryTree(DirectoryInfo root, Action<FileInfo> fileAction, Action<DirectoryInfo> directoryAction)
        {
            //TODO: MAKE IT PARALLEL
            FileInfo[] files = null;
            DirectoryInfo[] directories = null;

            try
            {
                files = root.GetFiles();
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.Info("Unauthorized access to directory", exception);
            }
            catch (DirectoryNotFoundException exception)
            {
                _logger.Error("Root directory not found", exception);
            }

            if (files == null)
                return;

            foreach (var fileInfo in files)
            {
                try
                {
                    fileAction(fileInfo);
                }
                catch (FileNotFoundException exception)
                {
                    _logger.Error("File was deleted during function call", exception);
                }
            }

            try
            {
                directories = root.GetDirectories();
            }
            catch (DirectoryNotFoundException exception)
            {
                _logger.Error("Root directory not found", exception);
            }

            foreach (var directoryInfo in directories)
            {
                directoryAction(directoryInfo);
                WalkDirectoryTree(directoryInfo, fileAction, directoryAction);
            }
        }
    }
}
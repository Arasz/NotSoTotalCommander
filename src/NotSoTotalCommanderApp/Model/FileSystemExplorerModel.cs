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
        private ILog _logger;

        private FileSystemWatcher _watcher = new FileSystemWatcher();

        public IEnumerable<IFileSystemItem> CachedItems { get; private set; }

        public string CurrentDirectory { get; set; }

        public string GetCurrentDirectoryParent
        {
            get
            {
                var root = Directory.GetParent(CurrentDirectory)?.ToString();

                return string.IsNullOrEmpty(root) ? null : root;
            }
        }

        public IList<IFileSystemItem> SelectedItems { get; } = new List<IFileSystemItem>();

        public string[] SystemDrives => Directory.GetLogicalDrives();

        public FileSystemExplorerModel()
        {
            _logger = LogManager.GetLogger(typeof(FileSystemExplorerModel));
        }

        /// <summary>
        /// Makes copy of selected items informations. This is not copy of files. 
        /// </summary>
        /// <param name="items"> Selected items </param>
        public void CacheSelectedItems(IEnumerable<IFileSystemItem> items)
        {
            CachedItems = items.ToList();
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
            catch (Exception exception)
            {
                _logger.Error($"Given directory name: {directoryName}, is invalid", exception);
                throw new InvalidPathException(Path.Combine(CurrentDirectory, directoryName), innerException: exception);
            }
        }

        /// <summary>
        /// Delete cached items 
        /// </summary>
        /// <exception cref="DeleteOperationException">
        /// Thrown when error other than unauthorized access occurs.
        /// </exception>
        public void Delete()
        {
            foreach (var fileSystemItem in SelectedItems)
            {
                try
                {
                    if (fileSystemItem.IsDirectory)
                        Directory.Delete(fileSystemItem.Path, true);
                    else
                        File.Delete(fileSystemItem.Path);
                }
                catch (UnauthorizedAccessException unauthorizedAccessException)
                {
                    _logger.Info($"Unauthorized access to {fileSystemItem.Path}", unauthorizedAccessException);
                }
                catch (Exception exception)
                {
                    _logger.Error($"Exception thrown when during {fileSystemItem.Path} delete attempt", exception);
                    throw new DeleteOperationException(fileSystemItem.Path, innerException: exception);
                }
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

            IList<FileSystemItem> fileSystemItems = new List<FileSystemItem>();

            WalkDirectoryTree(new DirectoryInfo(path), info => fileSystemItems.Add(new FileSystemItem(info)), info => fileSystemItems.Add(new FileSystemItem(info)));

            return fileSystemItems;
        }

        /// <summary>
        /// Moves cached items to destination folder 
        /// </summary>
        public void MoveCached(IEnumerable<IFileSystemItem> items = null)
        {
            var cachedItems = items ?? CachedItems;

            if (cachedItems == null)
            {
                _logger.Info("Move called but no items where cached");
                return;
            }

            foreach (var fileSystemItem in CachedItems)
                Move(fileSystemItem, CurrentDirectory);
            // after move cached collection is not valid anymore
            CachedItems = null;
        }

        /// <summary>
        /// Makes copy of cached items inside current directory 
        /// </summary>
        /// <param name="fileSystemItemsToPast"></param>
        /// <returns></returns>
        public void Paste(IEnumerable<IFileSystemItem> fileSystemItemsToPast = null, bool canOverwrite = false, bool inDepth = false)
        {
            var itemsToPast = fileSystemItemsToPast ?? CachedItems;
            string currentDirectoryTmp;

            foreach (var fileSystemItem in itemsToPast)
            {
                if (fileSystemItem.IsDirectory)
                {
                    currentDirectoryTmp = CurrentDirectory;
                    var newDirectoryPath = Path.Combine(CurrentDirectory, fileSystemItem.Name);
                    Directory.CreateDirectory(newDirectoryPath);
                    if (inDepth)
                    {
                        CurrentDirectory = newDirectoryPath;
                        Paste(GetAllItemsUnderPath(fileSystemItem.Path), canOverwrite, inDepth);
                    }
                    CurrentDirectory = currentDirectoryTmp;
                }
                else
                    File.Copy(fileSystemItem.Path, Path.Combine(CurrentDirectory, fileSystemItem.Name), canOverwrite);
            }
        }

        /// <summary>
        /// Moves item to current directory under destination name 
        /// </summary>
        /// <param name="itemToMove"> Moved file system item </param>
        /// <param name="destinationPath"> Destination directory name </param>
        private void Move(IFileSystemItem itemToMove, string destinationPath)
        {
            try
            {
                if (itemToMove.IsDirectory)
                    Directory.Move(itemToMove.Path, destinationPath);

                File.Move(itemToMove.Path, destinationPath);
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                _logger.Info($"Unauthorized access to {CurrentDirectory}", unauthorizedAccessException);
            }
            catch (DirectoryNotFoundException exception)
            {
                _logger.Error($"Given soruce path: {itemToMove.Path}, is invalid",
                    exception);
                throw new InvalidPathException(itemToMove.Path, innerException: exception);
            }
            catch (Exception exception)
            {
                _logger.Error("One of the given paths may be invalid, or file system item is used by another process", exception);
                throw new MoveOperationException(itemToMove.Path, destinationPath, innerException: exception);
            }
        }

        /// <summary>
        /// Walks through directories tree and preforms some action on each directory and file 
        /// </summary>
        /// <param name="root"> Root directory </param>
        /// <param name="fileAction"> Action preformed on file </param>
        /// <param name="directoryAction"> Action preformed on directory </param>
        private void WalkDirectoryTree(DirectoryInfo root, Action<FileInfo> fileAction, Action<DirectoryInfo> directoryAction, int maxDepth = 0)
        {
            //TODO: MAKE IT PARALLEL
            FileInfo[] files = null;
            DirectoryInfo[] directories = null;

            try
            {
                directories = root.GetDirectories();
            }
            catch (DirectoryNotFoundException exception)
            {
                _logger.Error("Root directory not found", exception);

                throw new InvalidPathException(root.FullName, "Root directory not found", exception);
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.Info("Unauthorized access to directory", exception);
                throw;
            }

            foreach (var directoryInfo in directories)
            {
                directoryAction(directoryInfo);
                if (maxDepth > 0)
                    WalkDirectoryTree(directoryInfo, fileAction, directoryAction, maxDepth--);
            }

            files = root.GetFiles();

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
        }
    }
}
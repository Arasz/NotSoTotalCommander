using log4net;
using NotSoTotalCommanderApp.Exceptions;
using NotSoTotalCommanderApp.Model.FileSystemItemModel;
using NotSoTotalCommanderApp.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NotSoTotalCommanderApp.Model
{
    public class FileSystemExplorerModel
    {
        /// <summary>
        /// Default logger 
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Specifies if cache should be moved (true) or copied 
        /// </summary>
        private bool _cacheToMove;

        /// <summary>
        /// Currently browsed directory 
        /// </summary>
        public string CurrentDirectory { get; set; }

        /// <summary>
        /// Current directory parrent 
        /// </summary>
        public string GetCurrentDirectoryParent
        {
            get
            {
                var root = Directory.GetParent(CurrentDirectory)?.ToString();

                return string.IsNullOrEmpty(root) ? null : root;
            }
        }

        /// <summary>
        /// Checks if there is directory inside cached file system items collection 
        /// </summary>
        public bool IsAnyDirectoryCached => CachedItems?.Any((item => item.IsDirectory)) ?? false;

        /// <summary>
        /// Selected file system items 
        /// </summary>
        public IList<IFileSystemItem> SelectedItems { get; } = new List<IFileSystemItem>();

        /// <summary>
        /// System drives 
        /// </summary>
        public string[] SystemDrives => Directory.GetLogicalDrives();

        /// <summary>
        /// Cached file system items collection 
        /// </summary>
        private IEnumerable<IFileSystemItem> CachedItems { get; set; }

        public FileSystemExplorerModel()
        {
            _logger = LogManager.GetLogger(typeof(FileSystemExplorerModel));
        }

        /// <summary>
        /// Makes copy of selected items informations. This is not copy of files. 
        /// </summary>
        /// <param name="items"> Selected items </param>
        public void CacheSelectedItems(IEnumerable<IFileSystemItem> items, bool cacheToMove = false)
        {
            _cacheToMove = cacheToMove;
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

        public async Task MoveOrPasteAsync(AsyncOperationResources<int> asyncResources, IEnumerable<IFileSystemItem> items = null, bool inDepth = false)
        {
            if (_cacheToMove)
                await MoveCachedAsync(asyncResources, items).ConfigureAwait(false);
            else
                await PasteAsync(asyncResources, items, inDepth).ConfigureAwait(false);
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
                    Directory.Move(itemToMove.Path, Path.Combine(destinationPath, itemToMove.Name));
                else
                    File.Move(itemToMove.Path, Path.Combine(destinationPath, itemToMove.Name));
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
        /// Moves cached items to new location 
        /// </summary>
        /// <param name="asyncResources"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private async Task MoveCachedAsync(AsyncOperationResources<int> asyncResources, IEnumerable<IFileSystemItem> items = null)
        {
            var cachedItems = items ?? CachedItems;

            if (cachedItems == null)
            {
                _logger.Info("Cut called but no items where cached");
                return;
            }

            int progress = 1;
            foreach (var fileSystemItem in CachedItems)
            {
                if (asyncResources.CancellationTokenSource.IsCancellationRequested)
                    return;
                await Task.Run(() => Move(fileSystemItem, CurrentDirectory)).ConfigureAwait(false);
                asyncResources.Progress.Report(progress++);
            }
            // after move cached collection is not valid anymore
            CachedItems = null;
        }

        /// <summary>
        /// Pasts cached items to new location 
        /// </summary>
        /// <param name="asyncResources"></param>
        /// <param name="fileSystemItemsToPast"></param>
        /// <param name="inDepth"></param>
        /// <returns></returns>
        private async Task PasteAsync(AsyncOperationResources<int> asyncResources, IEnumerable<IFileSystemItem> fileSystemItemsToPast = null, bool inDepth = false)
        {
            int progress = 1;
            var itemsToPast = fileSystemItemsToPast ?? CachedItems;

            var firstItem = itemsToPast.First();
            var root = firstItem.FullName.Replace(firstItem.Name, "");

            Queue<IFileSystemItem> stack = new Queue<IFileSystemItem>(itemsToPast);
            while (stack.Any())
            {
                if (asyncResources.CancellationTokenSource.IsCancellationRequested)
                    return;

                var fileSystemItem = stack.Dequeue();

                var destinationPath = Path.Combine(CurrentDirectory, fileSystemItem.Path.Replace(root, ""));

                if (destinationPath == fileSystemItem.Path)
                {
                    _logger.Info($"{destinationPath} file or directory exist in current directory");
                    //return;
                }

                if (fileSystemItem.IsDirectory)
                {
                    try
                    {
                        await Task.Run(() => Directory.CreateDirectory(destinationPath)).ConfigureAwait(false);
                    }
                    catch (IOException exception)
                    {
                        _logger.Error("Error during directory creation", exception);
                        throw new DirectoryCreationException(destinationPath, innerException: exception);
                    }
                    catch (Exception exception)
                    {
                        _logger.Error($"Given directory name: {destinationPath}, is invalid", exception);
                        throw new InvalidPathException(destinationPath, innerException: exception);
                    }
                    if (inDepth)
                    {
                        foreach (var item in GetAllItemsUnderPath(fileSystemItem.Path))
                            stack.Enqueue(item);
                    }
                }
                else
                {
                    try
                    {
                        await Task.Run(() => File.Copy(fileSystemItem.Path, destinationPath, false)).ConfigureAwait(false);
                    }
                    catch (IOException exception)
                    {
                        _logger.Error("{}", exception);
                        throw new FileCopyException(fileSystemItem.Path, destinationPath, innerException: exception);
                    }
                    catch (Exception exception)
                    {
                        _logger.Error($"Soruce path: {fileSystemItem.Path} or destination path: {destinationPath} is wrong", exception);
                        throw new FileCopyException(fileSystemItem.Path, destinationPath, innerException: exception);
                    }
                }

                asyncResources.Progress.Report(progress++);
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
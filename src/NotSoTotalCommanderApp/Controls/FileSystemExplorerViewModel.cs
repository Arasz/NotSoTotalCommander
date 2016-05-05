using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using Microsoft.Practices.ServiceLocation;
using NotSoTotalCommanderApp.Enums;
using NotSoTotalCommanderApp.Exceptions;
using NotSoTotalCommanderApp.Extensions;
using NotSoTotalCommanderApp.Messages;
using NotSoTotalCommanderApp.Model;
using NotSoTotalCommanderApp.Model.FileSystemItemModel;
using NotSoTotalCommanderApp.Services;
using NotSoTotalCommanderApp.Utility;
using NotSoTotalCommanderApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotSoTotalCommanderApp.Controls
{
    public class FileSystemExplorerViewModel : ViewModelBase
    {
        private readonly FileSystemExplorerModel _explorerModel;

        /// <summary>
        /// Collection of file system items displayed in control 
        /// </summary>
        private readonly ObservableCollection<IFileSystemItem> _fileSystemItemsCollection = new ObservableCollection<IFileSystemItem>();

        /// <summary>
        /// Default logger 
        /// </summary>
        private readonly ILog _logger = LogManager.GetLogger(typeof(MainViewModel));

        /// <summary>
        /// Default messenger 
        /// </summary>
        private readonly IMessenger _messanger;

        /// <summary>
        /// Cancellation token source for asynchronous operations 
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Items reload (refresh) state 
        /// </summary>
        private bool _itemsReloaded;

        /// <summary>
        /// Path selected by user 
        /// </summary>
        private string _selectedPath;

        /// <summary>
        /// File system items observable collection 
        /// </summary>
        public INotifyCollectionChanged FileSystemItemsCollection => _fileSystemItemsCollection;

        public ICommand LoadFileSystemItemsCommand { get; private set; }

        public ICommand RespondForUserActionCommand { get; private set; }

        /// <summary>
        /// Items selected by user 
        /// </summary>
        public IList<IFileSystemItem> SelectedItems { get; }

        /// <summary>
        /// Path selected by user 
        /// </summary>
        public string SelectedPath
        {
            get { return _selectedPath; }
            set
            {
                if (value == null) return;
                _selectedPath = value;
                RaisePropertyChanged(nameof(SelectedPath));
            }
        }

        public ICommand SelectionChangedCommand { get; private set; }

        /// <summary>
        /// System drivers 
        /// </summary>
        public IEnumerable<string> SystemDrives => _explorerModel.SystemDrives;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class. 
        /// </summary>
        public FileSystemExplorerViewModel(FileSystemExplorerModel explorerModel)
        {
            _messanger = Messenger.Default;
            _messanger.Register<CancelAsyncOperationMessage>(this, CancellationRequestedHandler);

            _explorerModel = explorerModel;
            SelectedItems = _explorerModel.SelectedItems;
            SelectedPath = SystemDrives.First();

            LoadFileSystemItems();

            LoadFileSystemItemsCommand = new RelayCommand<bool>(LoadFileSystemItems);
            RespondForUserActionCommand = new RelayCommand<ActionType>(ResponseForUserActionAsync);
            SelectionChangedCommand = new RelayCommand<EventArgs>(HandleSelectionEvent);
        }

        private void CancellationRequestedHandler(CancelAsyncOperationMessage message)
        {
            _cancellationTokenSource.Cancel();
        }

        private void HandleSelectionEvent(EventArgs eventArgs)
        {
            var selectionChanged = (SelectionChangedEventArgs)eventArgs;

            selectionChanged.Handled = true;

            if (_itemsReloaded)
            {
                SelectedItems.Clear();
                _itemsReloaded = false;
            }

            var addedItems = selectionChanged.AddedItems.Cast<IFileSystemItem>();
            var removedItems = selectionChanged.RemovedItems.Cast<IFileSystemItem>();

            foreach (var addedItem in addedItems)
                SelectedItems.Add(addedItem);

            foreach (var removedItem in removedItems)
                SelectedItems.RemoveAll(element => element.Path == removedItem.Path);
        }

        private void LoadFileSystemItems(bool forCurrentDirectory = false)
        {
            try
            {
                var path = forCurrentDirectory
                    ? _explorerModel.CurrentDirectory
                    : SelectedPath;

                _explorerModel.CurrentDirectory = path;

                var items = _explorerModel.GetAllItemsUnderPath(path);

                if (items == null && SystemDrives.Contains(path))
                {
                    _fileSystemItemsCollection.Clear();
                    return;
                }

                if (items == null)
                    return;

                _fileSystemItemsCollection.Clear();
                _fileSystemItemsCollection.Add(
                    new FileSystemBackItemProxy(
                        new FileSystemItem(
                            new DirectoryInfo(_explorerModel.GetCurrentDirectoryParent ??
                                              _explorerModel.CurrentDirectory), TraversalDirection.Up)));

                foreach (var extendedFileSystemInfo in items)
                {
                    _fileSystemItemsCollection.Add(extendedFileSystemInfo);
                }

                SelectedPath = path;
                _itemsReloaded = true;
            }
            catch (UnauthorizedAccessException exception)
            {
            }
            catch (FileSystemException exception)
            {
                _logger.Info($"Exception catched inside {MethodInfo.GetCurrentMethod().Name}.", exception);
            }
        }

        private void ReportProgress(int progress)
        {
            _messanger.Send(new ReportProgressMessage<int>(progress));
        }

        private async void ResponseForUserActionAsync(ActionType action)
        {
            IProgress<int> progress = new Progress<int>(ReportProgress);
            _cancellationTokenSource = new CancellationTokenSource();
            AsyncOperationResources<int> asyncOperationResources = new AsyncOperationResources<int>(progress, _cancellationTokenSource);

            IDialogService dialogSrvice = ServiceLocator.Current.GetInstance<IDialogService>();
            ;

            switch (action)
            {
                case ActionType.OpenFileSystemItem:
                    LoadFileSystemItems();
                    break;

                case ActionType.Copy:
                    _explorerModel.CacheSelectedItems(SelectedItems);
                    break;

                case ActionType.MoveOrPaste:

                    MessageBoxResult pastDecisionResult = MessageBoxResult.None;

                    if (_explorerModel.IsAnyDirectoryCached)
                        pastDecisionResult = dialogSrvice.ShowDecisionMessage("", "", MessageBoxButton.YesNoCancel,
                            DecisionType.DepthPaste).Result;

                    if (pastDecisionResult == MessageBoxResult.Yes)
                    {
                        dialogSrvice.ShowProgressMessage("", "");
                        await _explorerModel.MoveOrPasteAsync(asyncOperationResources, inDepth: true).ConfigureAwait(true);
                        _messanger.Send(new AsyncOperationIndicatorMessage(true));
                    }
                    else
                    {
                        dialogSrvice.ShowProgressMessage("", "");
                        await _explorerModel.MoveOrPasteAsync(asyncOperationResources).ConfigureAwait(true);
                        _messanger.Send(new AsyncOperationIndicatorMessage(true));
                    }

                    LoadFileSystemItems(true);
                    break;

                case ActionType.Cut:
                    _explorerModel.CacheSelectedItems(SelectedItems, true);
                    break;

                case ActionType.Delete:
                    var deleteDecisionResult = dialogSrvice.ShowDecisionMessage("", "", MessageBoxButton.YesNo,
                            DecisionType.Delete).Result;
                    if (deleteDecisionResult == MessageBoxResult.Yes)
                    {
                        _explorerModel.Delete();
                        LoadFileSystemItems(true);
                    }
                    break;

                case ActionType.Create:
                    var cresteResponse = dialogSrvice.ShowDecisionMessage("", "", MessageBoxButton.YesNo,
                            DecisionType.Delete);
                    var newName = cresteResponse.Data;
                    _explorerModel.CreateDirectory(newName);
                    LoadFileSystemItems(true);
                    break;
            }
        }
    }
}
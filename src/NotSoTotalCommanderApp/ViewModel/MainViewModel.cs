using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using NotSoTotalCommanderApp.Culture;
using NotSoTotalCommanderApp.Enums;
using NotSoTotalCommanderApp.Exceptions;
using NotSoTotalCommanderApp.Extensions;
using NotSoTotalCommanderApp.Messages;
using NotSoTotalCommanderApp.Model;
using NotSoTotalCommanderApp.Model.FileSystemItemModel;
using NotSoTotalCommanderApp.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotSoTotalCommanderApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to. 
    /// <para> You can also use Blend to data bind with the tool's support. </para>
    /// <para> See http://www.galasoft.ch/mvvm </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly FileSystemExplorerModel _explorerModel;

        private readonly ILog _logger = LogManager.GetLogger(typeof(MainViewModel));
        private readonly IMessenger _messanger;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _itemsReloaded;
        private UserDecisionResultMessage _lastDecisionResultMessage;
        private ObservableCollection<IFileSystemItem> _leftFieFileSystemInfos = new ObservableCollection<IFileSystemItem>();
        private string _selectedPath;
        public ICommand ChangeLanguageCommand { get; }

        public string CurrentDate { get; private set; } = DateTime.Today.ToString(StaticDateTimeFormat.ShortDate, Properties.Resources.Culture);

        public INotifyCollectionChanged LeftItemsCollection => _leftFieFileSystemInfos;

        public ICommand LoadFileSystemItemsCommand { get; private set; }

        public ICommand RespondForUserActionCommand { get; private set; }

        public IList<IFileSystemItem> SelectedItems { get; }

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

        public IEnumerable<string> SystemDrives => _explorerModel.SystemDrives;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class. 
        /// </summary>
        public MainViewModel(FileSystemExplorerModel explorerModel)
        {
            _messanger = Messenger.Default;
            _messanger.Register<UserDecisionResultMessage>(this, RetrieveUserResponse);
            _messanger.Register<CancelAsyncOperationMessage>(this, CancellationRequestedHandler);

            _explorerModel = explorerModel;
            SelectedItems = _explorerModel.SelectedItems;
            SelectedPath = SystemDrives.First();

            LoadFileSystemItemsCommand = new RelayCommand<bool>(LoadFileSystemItems);
            RespondForUserActionCommand = new RelayCommand<ActionType>(ResponseForUserActionAsync);
            SelectionChangedCommand = new RelayCommand<EventArgs>(HandleSelectionEvent);
            ChangeLanguageCommand = new RelayCommand<CultureInfo>(ChangeLanguage);
        }

        public void ChangeLanguage(CultureInfo cultureInfo)
        {
            CultureResources.ChangeCulture(cultureInfo);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            CurrentDate = DateTime.Today.ToString(StaticDateTimeFormat.ShortDate, Properties.Resources.Culture);
            RaisePropertyChanged(nameof(CurrentDate));
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
                    _leftFieFileSystemInfos.Clear();
                    return;
                }

                if (items == null)
                    return;

                _leftFieFileSystemInfos.Clear();
                _leftFieFileSystemInfos.Add(
                    new FileSystemBackItemProxy(
                        new FileSystemItem(
                            new DirectoryInfo(_explorerModel.GetCurrentDirectoryParent ??
                                              _explorerModel.CurrentDirectory), TraversalDirection.Up)));

                foreach (var extendedFileSystemInfo in items)
                {
                    _leftFieFileSystemInfos.Add(extendedFileSystemInfo);
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
            _cancellationToken = _cancellationTokenSource.Token;
            AsyncOperationResources<int> asyncOperationResources = new AsyncOperationResources<int>(progress, ref _cancellationToken);

            switch (action)
            {
                case ActionType.OpenFileSystemItem:
                    LoadFileSystemItems();
                    break;

                case ActionType.Copy:
                    _explorerModel.CacheSelectedItems(SelectedItems);
                    break;

                case ActionType.MoveOrPaste:

                    if (_explorerModel.IsAnyDirectoryCached)
                        UserDecisionRequest(DecisionType.DepthPaste);

                    MessageBoxResult pastDecisionResult = MessageBoxResult.None;
                    if (_lastDecisionResultMessage?.UserDecisionResult.Any() ?? false)
                        pastDecisionResult = _lastDecisionResultMessage.UserDecisionResult.Dequeue();

                    if (pastDecisionResult == MessageBoxResult.Yes)
                    {
                        _messanger.Send(new AsyncOperationIndicatorMessage());
                        await _explorerModel.MoveOrPasteAsync(asyncOperationResources, inDepth: true).ConfigureAwait(true);
                        _messanger.Send(new AsyncOperationIndicatorMessage(true));
                    }
                    else
                    {
                        _messanger.Send(new AsyncOperationIndicatorMessage());
                        await _explorerModel.MoveOrPasteAsync(asyncOperationResources).ConfigureAwait(true);
                        _messanger.Send(new AsyncOperationIndicatorMessage(true));
                    }

                    LoadFileSystemItems(true);
                    break;

                case ActionType.Cut:
                    _explorerModel.CacheSelectedItems(SelectedItems, true);
                    break;

                case ActionType.Delete:
                    UserDecisionRequest(DecisionType.Delete);
                    var deleteDecisionResult = _lastDecisionResultMessage.UserDecisionResult.Dequeue();
                    if (deleteDecisionResult == MessageBoxResult.Yes)
                    {
                        _explorerModel.Delete();
                        LoadFileSystemItems(true);
                    }
                    break;

                case ActionType.Create:
                    UserDecisionRequest(DecisionType.Create);
                    var cresteResponse = _lastDecisionResultMessage.UserDecisionResult.Dequeue();
                    var newName = _lastDecisionResultMessage.Name;
                    _explorerModel.CreateDirectory(newName);
                    LoadFileSystemItems(true);
                    break;
            }
        }

        private void RetrieveUserResponse(UserDecisionResultMessage userDecisionResult) => _lastDecisionResultMessage = userDecisionResult;

        private void UserDecisionRequest(DecisionType decisionType)
                    => _messanger.Send(new UserDecisionRequestMessage(decisionType));
    }
}
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using NotSoTotalCommanderApp.Enums;
using NotSoTotalCommanderApp.Extensions;
using NotSoTotalCommanderApp.Messages;
using NotSoTotalCommanderApp.Model;
using NotSoTotalCommanderApp.Model.FileSystemItemModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
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

        private readonly IMessenger _messanger;

        private bool _itemsReloaded;
        private UserDecisionResultMessage _lastDecisionResultMessage;
        private ObservableCollection<IFileSystemItem> _leftFieFileSystemInfos = new ObservableCollection<IFileSystemItem>();

        public INotifyCollectionChanged LeftItemsCollection => _leftFieFileSystemInfos;

        public ICommand LoadFileSystemItemsCommand { get; private set; }

        public ICommand RespondForUserActionCommand { get; private set; }

        public IList<IFileSystemItem> SelectedItems { get; }

        public string SelectedPath
        {
            get { return _explorerModel.CurrentDirectory; }
            set
            {
                if (value == null) return;
                _explorerModel.CurrentDirectory = value;
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

            _explorerModel = explorerModel;
            SelectedItems = _explorerModel.SelectedItems;
            SelectedPath = SystemDrives.First();

            LoadFileSystemItemsCommand = new RelayCommand<bool>(LoadFileSystemItems);
            RespondForUserActionCommand = new RelayCommand<ActionType>(ResponseForUserAction);
            SelectionChangedCommand = new RelayCommand<EventArgs>(HandleSelectionEvent);
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

        /// <summary>
        /// Loads file system items informations 
        /// </summary>
        private void LoadFileSystemItems(bool forCurrentDirectory = false)
        {
            try
            {
                var path = forCurrentDirectory ? _explorerModel.GetCurrentDirectoryParent :
                SelectedPath;

                var items = _explorerModel.GetAllItemsUnderPath(path);

                if (items == null && SystemDrives.Contains(path))
                {
                    _leftFieFileSystemInfos.Clear();
                    return;
                }

                if (items == null)
                    return;

                _leftFieFileSystemInfos.Clear();
                _leftFieFileSystemInfos.Add(new FileSystemBackItemProxy(new FileSystemItem(new DirectoryInfo(_explorerModel.GetCurrentDirectoryParent ?? _explorerModel.CurrentDirectory), TraversalDirection.Up)));

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
        }

        private void ResponseForUserAction(ActionType action)
        {
            switch (action)
            {
                case ActionType.OpenFileSystemItem:
                    LoadFileSystemItems();
                    break;

                case ActionType.Copy:
                    _explorerModel.CacheSelectedItems(SelectedItems);
                    break;

                case ActionType.Paste:
                    UserDecisionRequest(DecisionType.DepthPaste);
                    var pastDecisionResult = _lastDecisionResultMessage.UserDecisionResult.Dequeue();

                    if (pastDecisionResult == MessageBoxResult.Yes)
                        _explorerModel.Paste(inDepth: true);
                    else if (pastDecisionResult == MessageBoxResult.No)
                        _explorerModel.Paste();

                    LoadFileSystemItems();
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
                    LoadFileSystemItems();
                    break;
            }
        }

        private void RetrieveUserResponse(UserDecisionResultMessage userDecisionResult) => _lastDecisionResultMessage = userDecisionResult;

        private void UserDecisionRequest(DecisionType decisionType)
                    => _messanger.Send(new UserDecisionRequestMessage(decisionType));
    }
}
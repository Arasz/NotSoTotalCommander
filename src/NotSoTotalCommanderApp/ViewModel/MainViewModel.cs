using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NotSoTotalCommanderApp.Enums;
using NotSoTotalCommanderApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
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

        private bool _itemsReloaded;

        private ObservableCollection<IFileSystemItem> _leftFieFileSystemInfos = new ObservableCollection<IFileSystemItem>();

        public INotifyCollectionChanged LeftItemsCollection => _leftFieFileSystemInfos;

        public ICommand LoadFileSystemItemsCommand { get; private set; }

        public ICommand RespondForUserActionCommand { get; private set; }

        public List<IFileSystemItem> SelectedItems { get; } = new List<IFileSystemItem>();

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
            _explorerModel = explorerModel;
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
                    _explorerModel.Copy(SelectedItems);
                    break;

                case ActionType.Paste:
                    _explorerModel.Past();
                    LoadFileSystemItems();
                    break;

                case ActionType.Delete:
                    _explorerModel.Delete(SelectedItems);
                    LoadFileSystemItems(true);
                    break;

                case ActionType.Create:
                    _explorerModel.CreateDirectory("WOLOLOLO");
                    LoadFileSystemItems();
                    break;
            }
        }
    }
}
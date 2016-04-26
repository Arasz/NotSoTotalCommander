using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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

        private ObservableCollection<FileSystemItem> _leftFieFileSystemInfos = new ObservableCollection<FileSystemItem>();

        public ICommand KeyPressedCommand { get; private set; }

        public INotifyCollectionChanged LeftItemsCollection => _leftFieFileSystemInfos;

        public ICommand LoadFileSystemItemsCommand { get; private set; }

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

        public IList<string> SelectedPaths { get; } = new List<string>();

        public ICommand SelectionChangedCommand { get; private set; }

        public IEnumerable<string> SystemDrives => _explorerModel.SystemDrives;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class. 
        /// </summary>
        public MainViewModel(FileSystemExplorerModel explorerModel)
        {
            _explorerModel = explorerModel;
            SelectedPath = SystemDrives.First();

            LoadFileSystemItemsCommand = new RelayCommand(LoadFileSystemItems);
            KeyPressedCommand = new RelayCommand<EventArgs>(ResponseForPressingKey);
            SelectionChangedCommand = new RelayCommand<EventArgs>(HandleSelectionEvent);
        }

        private void HandleSelectionEvent(EventArgs eventArgs)
        {
            var selectionChanged = (SelectionChangedEventArgs)eventArgs;

            selectionChanged.Handled = true;

            var addedItems = selectionChanged.AddedItems;
            var removedItems = selectionChanged.RemovedItems;

            foreach (var addedItem in addedItems)
                SelectedPaths.Add(((FileSystemItem)addedItem).ToString());

            foreach (var removedItem in removedItems)
                SelectedPaths.Remove(((FileSystemItem)removedItem).ToString());
        }

        /// <summary>
        /// Loads file system items informations 
        /// </summary>
        private void LoadFileSystemItems()
        {
            try
            {
                var tmpSelectedPath = SelectedPath;

                var items = _explorerModel.GetAllItemsUnderPath(SelectedPath);

                if (items == null && SystemDrives.Contains(SelectedPath))
                {
                    _leftFieFileSystemInfos.Clear();
                    return;
                }

                if (items == null)
                    return;

                _leftFieFileSystemInfos.Clear();
                _leftFieFileSystemInfos.Add(new FileSystemItem(new DirectoryInfo(_explorerModel.GetCurrentDirectoryParent ?? _explorerModel.CurrentDirectory), TraversalDirection.Up));

                foreach (var extendedFileSystemInfo in items)
                {
                    _leftFieFileSystemInfos.Add(extendedFileSystemInfo);
                }

                SelectedPath = tmpSelectedPath;
            }
            catch (UnauthorizedAccessException exception)
            {
            }
        }

        private void ResponseForPressingKey(EventArgs eventArgs)
        {
            var keyPressed = eventArgs as KeyEventArgs;

            switch (keyPressed.Key)
            {
                case Key.Enter:
                    LoadFileSystemItems();
                    break;
            }
        }
    }
}
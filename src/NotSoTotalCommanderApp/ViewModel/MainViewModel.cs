using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NotSoTotalCommanderApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
        private ObservableCollection<ExtendedFileSystemInfo> _leftFieFileSystemInfos = new ObservableCollection<ExtendedFileSystemInfo>();
        private string _selectedPath;

        public ICommand KeyPressedCommand { get; private set; }

        public INotifyCollectionChanged LeftItemsCollection => _leftFieFileSystemInfos;

        public ICommand LoadFileSystemItemsCommand { get; private set; }

        public string SelectedDrive { get; set; }

        public string SelectedPath
        {
            get { return _selectedPath; }
            set { Set(ref _selectedPath, value, nameof(SelectedPath)); }
        }

        public IEnumerable<string> SystemDrives => _explorerModel.SystemDrives;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class. 
        /// </summary>
        public MainViewModel(FileSystemExplorerModel explorerModel)
        {
            _explorerModel = explorerModel;
            SelectedPath = SystemDrives.First();
            SelectedDrive = SelectedPath;

            LoadFileSystemItemsCommand = new RelayCommand(LoadFileSystemItems);
            KeyPressedCommand = new RelayCommand<EventArgs>(ResponseForPressingKey);
        }

        /// <summary>
        /// Loads file system items informations 
        /// </summary>
        private void LoadFileSystemItems()
        {
            try
            {
                var tmpSelectedPath = _selectedPath;

                var items = _explorerModel.GetAllItemsUnderPath(SelectedPath);

                if (items == null)
                    return;

                _leftFieFileSystemInfos.Clear();

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
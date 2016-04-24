using GalaSoft.MvvmLight;
using NotSoTotalCommanderApp.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

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
        public IEnumerable<string> DrivesList { get; private set; }

        public INotifyCollectionChanged LeftItemsCollection => _leftFieFileSystemInfos;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class. 
        /// </summary>
        public MainViewModel(FileSystemExplorerModel explorerModel)
        {
            _explorerModel = explorerModel;

            if (IsInDesignMode)
            {
            }

            DrivesList = Directory.GetLogicalDrives();

            var beginingPath = DrivesList.First();
            var directories = Directory.GetDirectories(beginingPath);
            var files = Directory.GetFiles(beginingPath);

            foreach (var dirInfo in directories.Select(dir => new ExtendedFileSystemInfo(new DirectoryInfo(dir))).ToList())
                _leftFieFileSystemInfos.Add(dirInfo);

            foreach (var file in files.Select(file => new ExtendedFileSystemInfo(new FileInfo(file))).ToList())
                _leftFieFileSystemInfos.Add(file);
        }
    }
}
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace NotSoTotalCommanderApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to. 
    /// <para> You can also use Blend to data bind with the tool's support. </para>
    /// <para> See http://www.galasoft.ch/mvvm </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<FileSystemInfo> _leftFieFileSystemInfos = new ObservableCollection<FileSystemInfo>();

        public INotifyCollectionChanged LeftItemsCollection => _leftFieFileSystemInfos;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class. 
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
            }
        }
    }
}
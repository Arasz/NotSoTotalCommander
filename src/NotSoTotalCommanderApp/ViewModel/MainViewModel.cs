using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using NotSoTotalCommanderApp.Culture;
using System;
using System.Globalization;
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
        private readonly ILog _logger = LogManager.GetLogger(typeof(MainViewModel));
        private readonly IMessenger _messanger;
        public ICommand ChangeLanguageCommand { get; }

        public string CurrentDate { get; private set; } = DateTime.Today.ToString(StaticDateTimeFormat.ShortDate, Properties.Resources.Culture);

        public MainViewModel()
        {
            _messanger = Messenger.Default;

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
    }
}
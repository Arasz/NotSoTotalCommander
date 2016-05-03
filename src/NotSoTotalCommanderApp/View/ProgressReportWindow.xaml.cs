using GalaSoft.MvvmLight.Messaging;
using NotSoTotalCommanderApp.Messages;
using System.Windows;

namespace NotSoTotalCommanderApp.View
{
    /// <summary>
    /// Interaction logic for ProgressReportWindow.xaml 
    /// </summary>
    public partial class ProgressReportWindow : Window
    {
        private IMessenger _messenger;

        public ProgressReportWindow()
        {
            _messenger = Messenger.Default;
            InitializeComponent();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            _messenger.Send(new CancelAsyncOperationMessage());
        }
    }
}
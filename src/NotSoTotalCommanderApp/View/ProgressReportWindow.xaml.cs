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
            _messenger.Register<ReportProgressMessage<int>>(this, ProgressReportHandle);
            InitializeComponent();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            _messenger.Send(new CancelAsyncOperationMessage());
        }

        private void ProgressReportHandle(ReportProgressMessage<int> message)
        {
            Run.Text = $" {message.Progress}";
        }
    }
}
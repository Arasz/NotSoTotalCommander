using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using NotSoTotalCommanderApp.Culture;
using NotSoTotalCommanderApp.Messages;
using NotSoTotalCommanderApp.Services;
using NotSoTotalCommanderApp.View;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NotSoTotalCommanderApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml 
    /// </summary>
    public partial class MainWindow : Window, IDialogService
    {
        private IMessenger _messenger;
        private ProgressReportWindow asyncOperationIndicator;

        public MainWindow()
        {
            CultureResources.ChangeCulture(Properties.Settings.Default.DefaultCulture);

            InitializeComponent();

            SimpleIoc.Default.Register<IDialogService>(() => this);

            _messenger = Messenger.Default;

            _messenger.Register<AsyncOperationIndicatorMessage>(this, ResponseForAsyncOperationIndicatorRequest);
        }

        public DecisionResult<string> ShowDecisionMessage(string message, string title, MessageBoxButton messageButton, DecisionType type)
        {
            DecisionResult<string> result = new DecisionResult<string>();
            switch (type)
            {
                case DecisionType.DepthPaste:
                    result.Result = MessageBox.Show(this, Properties.Resources.DepthPasteMessage, Properties.Resources.CopyMenuItem,
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
                    break;

                case DecisionType.Override:
                    result.Result = MessageBox.Show(this, Properties.Resources.DepthPasteMessage, "Interaction",
                        MessageBoxButton.YesNoCancel);

                    break;

                case DecisionType.Delete:
                    result.Result = MessageBox.Show(this, Properties.Resources.DeleteDecisionMessage, Properties.Resources.DeleteMenuItem,
                        MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                    break;

                case DecisionType.Create:
                    string dirName;
                    result.Result = DirectoryNameInputWindow.ShowDialog(out dirName);
                    result.Data = dirName;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }

        public void ShowError(string errorMessage, string errorTitle, MessageBoxButton messageButton = MessageBoxButton.OK)
        {
            MessageBox.Show(errorMessage, errorTitle, messageButton, MessageBoxImage.Error);
        }

        public void ShowMessage(string message, string title, MessageBoxButton messageButton)
        {
            throw new NotImplementedException();
        }

        public void ShowProgressMessage(string message, string title)
        {
            asyncOperationIndicator = new ProgressReportWindow();
            asyncOperationIndicator.Show();
        }

        public Task ShowProgressMessageAsync(string message, string title, CancellationTokenSource cancellationToken)
        {
            throw new NotImplementedException();
        }

        private void AboutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            AboutWindow.Show();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void ResponseForAsyncOperationIndicatorRequest(AsyncOperationIndicatorMessage message)
        {
            if (message.CancelIndicator)
            {
                asyncOperationIndicator.Close();
                asyncOperationIndicator = null;
            }
        }
    }
}
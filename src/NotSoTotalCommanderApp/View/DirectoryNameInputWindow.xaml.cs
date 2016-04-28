using System.Windows;

namespace NotSoTotalCommanderApp.View
{
    /// <summary>
    /// Interaction logic for DirectoryNameInputWindow.xaml 
    /// </summary>
    public partial class DirectoryNameInputWindow : Window
    {
        private static string _directoryName;

        public DirectoryNameInputWindow()
        {
            InitializeComponent();
        }

        public static MessageBoxResult ShowDialog(out string directoryName)
        {
            var instance = new DirectoryNameInputWindow();
            var result = ((Window)instance).ShowDialog();

            directoryName = instance.DirectoryName();

            if (result == null)
                return MessageBoxResult.Cancel;

            return result.Value ? MessageBoxResult.OK : MessageBoxResult.Cancel;
        }

        public string DirectoryName()
        {
            _directoryName = TextBox.Text;
            return _directoryName;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
using System.Windows;

namespace NotSoTotalCommanderApp.View
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        public static void Show()
        {
            var aboutWindow = new AboutWindow();
            ((Window)aboutWindow).Show();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e) => Close();
    }
}
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NotSoTotalCommanderApp.Culture
{
    public class CultureResources
    {
        private static readonly ObjectDataProvider ResourceProvider =
            (ObjectDataProvider)Application.Current.FindResource("Resources");

        public static void ChangeCulture(CultureInfo culture)
        {
            Properties.Resources.Culture = culture;
            ResourceProvider.Refresh();
        }

        public Properties.Resources GetResourceInstance() => new Properties.Resources();
    }
}
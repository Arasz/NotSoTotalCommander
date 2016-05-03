using NotSoTotalCommanderApp.Properties;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NotSoTotalCommanderApp.Culture
{
    public class CultureResources
    {
        /// <summary>
        /// </summary>
        private static readonly ObjectDataProvider ResourceProvider = Application.Current.Resources["Resources"] as ObjectDataProvider;

        public static void ChangeCulture(CultureInfo culture)
        {
            Resources.Culture = culture;
            ResourceProvider.Refresh();
        }

        public Resources GetResourceInstance() => new Resources();
    }
}
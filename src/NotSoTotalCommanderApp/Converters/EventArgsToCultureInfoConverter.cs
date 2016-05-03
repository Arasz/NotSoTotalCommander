using GalaSoft.MvvmLight.Command;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace NotSoTotalCommanderApp.Converters
{
    internal class EventArgsToCultureInfoConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            var eventArgs = value as RoutedEventArgs;
            var clickedItem = eventArgs?.Source as MenuItem;

            if (clickedItem == null) return Properties.Resources.Culture;

            clickedItem.IsChecked = true;

            var name = clickedItem.Header.ToString();

            if (name == Properties.Resources.EnglishMenuItem)
                return new CultureInfo("en-EN");

            return new CultureInfo("pl-PL");
        }
    }
}
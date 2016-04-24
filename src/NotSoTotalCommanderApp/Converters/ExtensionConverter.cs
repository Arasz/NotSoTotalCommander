using System;
using System.Globalization;
using System.Windows.Data;

namespace NotSoTotalCommanderApp.Converters
{
    /// <summary>
    /// Formats extension string 
    /// </summary>
    internal class ExtensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var extension = value as string;
            return extension?.TrimStart('.');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
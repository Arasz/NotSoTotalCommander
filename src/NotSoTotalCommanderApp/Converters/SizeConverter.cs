using System;
using System.Globalization;
using System.Windows.Data;

namespace NotSoTotalCommanderApp.Converters
{
    /// <summary>
    /// Formats size string received from FileSystemInfo 
    /// </summary>
    internal class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "<DIR>";

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using NotSoTotalCommanderApp.Model.FileSystemItemModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace NotSoTotalCommanderApp.Converters
{
    internal class FileSystemInfoToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileSystemInfo = value as IFileSystemItem;

            return fileSystemInfo?.ToString();
        }
    }
}
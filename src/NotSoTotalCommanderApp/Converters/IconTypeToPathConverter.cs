using NotSoTotalCommanderApp.Model.FileSystemItemModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace NotSoTotalCommanderApp.Converters
{
    internal class IconTypeToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var iconType = (IconType)value;

            switch (iconType)
            {
                case IconType.Directory:
                    return @"/Assests/folder.png";

                case IconType.File:
                    return @"/Assests/file.png";

                case IconType.RestricedDirectory:
                    return @"/Assests/folder-red.png";

                case IconType.RestrictedFile:
                    return @"/Assests/file-red.png";

                case IconType.Link:
                    return @"/Assests/up.png";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
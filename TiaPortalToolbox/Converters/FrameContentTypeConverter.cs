using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace TiaPortalToolbox.Converters;

internal class FrameContentTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null) return Visibility.Collapsed;

        return value.GetType() != (Type)parameter ? Visibility.Collapsed : Visibility.Visible;
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
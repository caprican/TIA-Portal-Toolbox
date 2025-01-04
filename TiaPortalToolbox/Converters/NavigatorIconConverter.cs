using System.Globalization;
using System.Windows.Data;

namespace TiaPortalToolbox.Converters;

public class NavigatorIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value switch
        {
            //Core.Models.TiaProject.PlcStruct => "../Assets/DataType.png",
            _ => "../Assets/FolderClosed_16x.png"
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

using System.Globalization;
using System.Windows.Data;

namespace CbcRoastersErp.Helpers.converters
{
    public class NullOrZeroToAddEditTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var moduleName = parameter?.ToString() ?? "Item";
            if (value == null || (int)value == 0)
                return $"Add {moduleName}";
            else
                return $"Edit {moduleName}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

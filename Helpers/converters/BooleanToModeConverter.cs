using System;
using System.Globalization;
using System.Windows.Data;

namespace CbcRoastersErp.Converters
{
    public class BooleanToModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool isEdit && isEdit) ? "Edit Purchase Order" : "Add Purchase Order";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


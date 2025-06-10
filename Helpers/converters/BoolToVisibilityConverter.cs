using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CbcRoastersErp.Helpers.converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; } = false;
        public bool CollapseInsteadOfHidden { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = value is bool b && b;
            if (Invert)
                flag = !flag;

            if (flag)
                return Visibility.Visible;
            else
                return CollapseInsteadOfHidden ? Visibility.Collapsed : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}

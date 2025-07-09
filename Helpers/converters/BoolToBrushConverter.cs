using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CbcRoastersErp.Helpers.converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public Brush PositiveBrush { get; set; } = Brushes.Green;
        public Brush NegativeBrush { get; set; } = Brushes.Red;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? PositiveBrush : NegativeBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}

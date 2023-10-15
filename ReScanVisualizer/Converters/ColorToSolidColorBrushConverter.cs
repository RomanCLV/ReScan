using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ReScanVisualizer.Converters
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is  SolidColorBrush color) 
            {
                return color.Color;
            }
            else if (value is KeyValuePair<string, Color> pair)
            {
                return pair.Value;
            }
            else if (value is null)
            {
                // no selection
                return null;
            }
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReScanVisualizer.Converters
{
    public class MaxPointsToDisplayToStringConvertercs : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = (int)value;
            return i < 0 ? "No limit" : i.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
            {
                return -1;
            }
            return str;
        }
    }
}

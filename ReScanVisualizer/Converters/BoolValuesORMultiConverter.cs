using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReScanVisualizer.Converters
{
    public class BoolValuesORMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;
            foreach (var item in values)
            {
                if (item is bool b)
                {
                    result |= b;
                    if (result)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

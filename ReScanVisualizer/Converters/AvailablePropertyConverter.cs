using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ReScanVisualizer.Converters
{
    public class AvailablePropertyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 0)
            {
                object value = values[0];
                if (value != null && value != DependencyProperty.UnsetValue)
                {
                    return value;
                }
            }
            
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

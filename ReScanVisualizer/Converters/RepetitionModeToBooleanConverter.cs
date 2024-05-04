using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReScanVisualizer.Converters
{
    internal class RepetitionModeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RepetitionMode repetitionModeValue && parameter is RepetitionMode targetValue)
            {
                return repetitionModeValue == targetValue;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && targetType == typeof(RepetitionMode) && parameter is RepetitionMode targetValue)
            {
                return boolValue ? targetValue : RepetitionMode.None;
            }

            return RepetitionMode.None;
        }
    }
}

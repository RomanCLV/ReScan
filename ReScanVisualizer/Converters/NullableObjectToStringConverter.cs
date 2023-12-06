using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReScanVisualizer.Converters
{
    public class NullableObjectToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return value is null ? string.Empty : value.ToString();
            if (value == null || !Tools.IsNumericType(value.GetType()))
            {
                return string.Empty;
            }
            else
            {
                string format = parameter as string ?? "N2"; // Utilise N2 par défaut si aucun paramètre spécifié

                if (value is byte || value is sbyte || value is short || value is ushort ||
                    value is int || value is uint || value is long || value is ulong ||
                    value is float || value is double || value is decimal)
                {
                    return string.Format(culture, "{0:" + format + "}", value);
                }
                else
                {
                    return value.ToString();
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

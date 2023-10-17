using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReScanVisualizer.Converters
{
    public class StringConcaterMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string sep = "\n";
            if (parameter != null)
            {
                sep = parameter.ToString();
            }
            List<string> str = new List<string>(values.Length);
            foreach (object o in values)
            {
                if (o is string s)
                {
                    s = s.Trim();
                    if (string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
                    {
                        continue;
                    }
                    str.Add(s);
                }
            }
            return string.Join(sep, str.ToArray());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

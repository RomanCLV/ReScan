using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MathEvaluatorNetFramework;

namespace ReScanVisualizer.Converters
{
    internal class ExpressionToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value.ToString();
            double result;
            if (!Tools.TryParse(str, out result))
            {
                Expression expression = new Expression();
                try
                {
                    expression.Set(str);
                    result = expression.Evaluate();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }
    }
}

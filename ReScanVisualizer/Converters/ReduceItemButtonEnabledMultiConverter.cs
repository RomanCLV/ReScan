using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders;

namespace ReScanVisualizer.Converters
{
    public class ReduceItemButtonEnabledMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3 &&
                values[0] is ScatterGraphBuilderState state &&
                values[1] is ScatterGraphBuildResult result &&
                values[2] is bool isSuccess)
            {
                return state == ScatterGraphBuilderState.Success && result != null && isSuccess;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

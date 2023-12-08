using ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReScanVisualizer.Converters
{
    public class LoadItemButtonEnabledMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 4 &&
                values[0] is ScatterGraphBuilderState state &&
                values[1] is ScatterGraphBuildResult result &&
                values[2] is bool isSuccess &&
                values[3] is bool isAdded)
            {
                return state == ScatterGraphBuilderState.Success && result != null && isSuccess && !isAdded;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

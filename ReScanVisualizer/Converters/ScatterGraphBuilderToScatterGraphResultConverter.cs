using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ReScanVisualizer.ViewModels.AddScatterGraph;
using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
using ReScanVisualizer.Views.AddScatterGraphViews;

namespace ReScanVisualizer.Converters
{
    public class ScatterGraphBuilderToScatterGraphResultConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3)
            {
                return "3 binding sources required";
            }

            string propertyName = parameter as string;

            if (values[0] is ScatterGraphBuilderBase builder &&
                values[1] is AddScatterGraphView view &&
                view.DataContext is AddScatterGraphViewModel viewModel)
            {
                if (viewModel.Results.TryGetValue(builder, out var result))
                {
                    switch (propertyName)
                    {
                        case "Count":
                            return (result != null && result.IsSuccess) ? result.Count.ToString() : string.Empty;

                        case "IsEnabled":
                            return (result != null && result.IsSuccess && !result.HasToReduceForced);

                        case "HasToReduce":
                            return (result != null && result.HasToReduce);

                        case "ReducedCount":
                            return (result != null && result.IsSuccess) ? result.ReducedCount.ToString() : string.Empty;

                        default:
                            return "Unexpected property: " + propertyName;
                    }
                }
            }

            return "null";
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

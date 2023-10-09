﻿using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
using ReScanVisualizer.ViewModels.AddScatterGraph;
using ReScanVisualizer.Views.AddScatterGraphViews;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReScanVisualizer.Converters
{
    internal class ScatterGraphBuilderToScatterGraphResultCountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
            {
                return "2 binding sources required";
            }

            string propertyName = parameter as string;

            if (values[0] is ScatterGraphBuilderBase builder &&
                values[1] is AddScatterGraphView view &&
                view.DataContext is AddScatterGraphViewModel viewModel)
            {
                if (viewModel.Results.TryGetValue(builder, out var result))
                {
                    
                }
            }

            return "null";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

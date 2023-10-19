﻿using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReScanVisualizer.Converters
{
    public class BuilderStateNotWorkingToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ScatterGraphBuilderState state)
            {
                return state != ScatterGraphBuilderState.Working;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
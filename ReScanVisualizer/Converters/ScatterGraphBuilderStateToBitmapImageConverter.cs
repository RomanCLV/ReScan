using ReScanVisualizer.Properties;
using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ReScanVisualizer.Converters
{
    public class ScatterGraphBuilderStateToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage image = null;
            if (value is ScatterGraphBuilderState state)
            {
                image = new BitmapImage();
                switch (state)
                {
                    case ScatterGraphBuilderState.Ready:
                        image.LoadFromBitmap(Resources.update);
                        break;

                    case ScatterGraphBuilderState.Working:
                        image.LoadFromBitmap(Resources.working);
                        break;
                    
                    case ScatterGraphBuilderState.Success:
                        image.LoadFromBitmap(Resources.available);
                        break;

                    case ScatterGraphBuilderState.Error:
                        image.LoadFromBitmap(Resources.unvailable);
                        break;

                    default:
                        throw new NotImplementedException($"State {value} is not expected!");
                }
            }
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

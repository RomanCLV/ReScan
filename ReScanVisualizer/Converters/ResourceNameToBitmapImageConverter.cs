using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ReScanVisualizer.Properties;

namespace ReScanVisualizer
{
    internal class ResourceNameToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage image = new BitmapImage();
            string resourceName = parameter.ToString();
            switch (resourceName)
            {
                #region ACTION : Add, Close, Delete, Browse, Working
                case "Add":
                    image.LoadFromBitmap(Resources.Add);
                    break;
                case "Close":
                    image.LoadFromBitmap(Resources.Close);
                    break;
                case "Delete":
                    image.LoadFromBitmap(Resources.Delete);
                    break;
                case "Browse":
                    image.LoadFromBitmap(Resources.Browse);
                    break;
                case "Working":
                    image.LoadFromBitmap(Resources.Working);
                    break;
                #endregion

                default:
                    throw new NotImplementedException($"Resource name {resourceName} is not expected!");
            }
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

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
                case "add":
                    image.LoadFromBitmap(Resources.add);
                    break;
                case "available":
                    image.LoadFromBitmap(Resources.available);
                    break;
                case "browse":
                    image.LoadFromBitmap(Resources.browse);
                    break;
                case "close":
                    image.LoadFromBitmap(Resources.close);
                    break;
                case "config":
                    image.LoadFromBitmap(Resources.config);
                    break;
                case "delete":
                    image.LoadFromBitmap(Resources.delete);
                    break;
                case "help":
                    image.LoadFromBitmap(Resources.help);
                    break;
                case "icon":
                    image.LoadFromBitmap(Resources.icon);
                    break;
                case "load":
                    image.LoadFromBitmap(Resources.load);
                    break;
                case "play":
                    image.LoadFromBitmap(Resources.play);
                    break;
                case "save-csv":
                    image.LoadFromBitmap(Resources.save_csv);
                    break;
                case "save-json":
                    image.LoadFromBitmap(Resources.save_json);
                    break;
                case "save-xml":
                    image.LoadFromBitmap(Resources.save_xml);
                    break;
                case "save...":
                    image.LoadFromBitmap(Resources.save___);
                    break;
                case "save":
                    image.LoadFromBitmap(Resources.save);
                    break;
                case "shortcut":
                    image.LoadFromBitmap(Resources.shortcut);
                    break;
                case "stop":
                    image.LoadFromBitmap(Resources.stop);
                    break;
                case "unvailable":
                    image.LoadFromBitmap(Resources.unvailable);
                    break;
                case "update":
                    image.LoadFromBitmap(Resources.update);
                    break;
                case "working":
                    image.LoadFromBitmap(Resources.working);
                    break;

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

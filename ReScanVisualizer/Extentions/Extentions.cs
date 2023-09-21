using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

namespace ReScanVisualizer
{
    internal static class Extentions
    {
        /// <summary>
        /// Set the BitmapImage with a Bitmap using a MemoryStream.
        /// </summary>
        /// <param name="bitmapImage">The instance to load.</param>
        /// <param name="bitmap">The source</param>
        internal static void LoadFromBitmap(this BitmapImage bitmapImage, Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
        }

        internal static bool IsAnInteger(this string numString)
        {
            return int.TryParse(numString, out _);
        }
    }
}

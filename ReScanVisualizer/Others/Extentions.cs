using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Media3D;
using System.Security.AccessControl;

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

        /// <summary>
        /// Round the value to <c>0</c> if <paramref name="d"/> is in ] -<see cref="Const.ZERO_CLAMP"/> ; <see cref="Const.ZERO_CLAMP"/> [
        /// </summary>
        /// <param name="d">The value to clamp.</param>
        /// <returns>0 or <paramref name="d"/>.</returns>
        internal static double Clamp(this double d)
        {
            return (-Const.ZERO_CLAMP < d) && (d < Const.ZERO_CLAMP) ? 0.0: d;
        }

        /// <summary>
        /// Round the value to <c><paramref name="value"/></c> if <paramref name="d"/> is in ] <paramref name="value"/> - <see cref="Const.ZERO_CLAMP"/> ; <paramref name="value"/> + <see cref="Const.ZERO_CLAMP"/> [
        /// </summary>
        /// <param name="d">The value to clamp.</param>
        /// <param name="value">The clamp reference.</param>
        /// <returns><paramref name="value"/> or <paramref name="d"/>.</returns>
        internal static double Clamp(this double d, double value)
        {
            return ((value - Const.ZERO_CLAMP) < d) && (d < (value + Const.ZERO_CLAMP)) ? value : d;
        }

        /// <summary>
        /// Round the value to <c><paramref name="value"/></c> if <paramref name="d"/> is in ] <paramref name="value"/> - <paramref name="clampFactor"/> * <see cref="Const.ZERO_CLAMP"/> ; <paramref name="value"/> + <paramref name="clampFactor"/> * <see cref="Const.ZERO_CLAMP"/> [
        /// </summary>
        /// <param name="d">The value to clamp.</param>
        /// <param name="value">The clamp reference.</param>
        /// <param name="clampFactor">A factor applied to <see cref="Const.ZERO_CLAMP"/></param>
        /// <returns><paramref name="value"/> or <paramref name="d"/>.</returns>
        internal static double Clamp(this double d, double value, double clampFactor)
        {
            return ((value - clampFactor * Const.ZERO_CLAMP) < d) && (d < (value + clampFactor * Const.ZERO_CLAMP)) ? value : d;
        }


        internal static void Clamp(this ref Matrix3D matrix)
        {
            matrix.M11 = matrix.M11.Clamp().Clamp(-1).Clamp(1);
            matrix.M12 = matrix.M12.Clamp().Clamp(-1).Clamp(1);
            matrix.M13 = matrix.M13.Clamp().Clamp(-1).Clamp(1);
            matrix.M14 = matrix.M14.Clamp().Clamp(-1).Clamp(1);
            matrix.M21 = matrix.M21.Clamp().Clamp(-1).Clamp(1);
            matrix.M22 = matrix.M22.Clamp().Clamp(-1).Clamp(1);
            matrix.M23 = matrix.M23.Clamp().Clamp(-1).Clamp(1);
            matrix.M24 = matrix.M24.Clamp().Clamp(-1).Clamp(1);
            matrix.M31 = matrix.M31.Clamp().Clamp(-1).Clamp(1);
            matrix.M32 = matrix.M32.Clamp().Clamp(-1).Clamp(1);
            matrix.M33 = matrix.M33.Clamp().Clamp(-1).Clamp(1);
            matrix.M34 = matrix.M34.Clamp().Clamp(-1).Clamp(1);
            matrix.M44 = matrix.M44.Clamp().Clamp(-1).Clamp(1);
        }

        internal static Point4D ToPoint4D(this Point3D point)
        {
            return new Point4D(point.X, point.Y, point.Z, 1.0);
        }

        internal static Point3D ToPoint3D(this Point4D point)
        {
            return new Point3D(point.X, point.Y, point.Z);
        }
    }
}

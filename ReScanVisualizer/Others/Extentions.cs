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
    public static class Extentions
    {
        /// <summary>
        /// Set the BitmapImage with a Bitmap using a MemoryStream.
        /// </summary>
        /// <param name="bitmapImage">The instance to load.</param>
        /// <param name="bitmap">The source</param>
        public static void LoadFromBitmap(this BitmapImage bitmapImage, Bitmap bitmap)
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

        public static bool IsAnInteger(this string numString)
        {
            return int.TryParse(numString, out _);
        }

        /// <summary>
        /// Round the value to <c>0</c> if <paramref name="d"/> is in ] -<see cref="Const.ZERO_CLAMP"/> ; <see cref="Const.ZERO_CLAMP"/> [
        /// </summary>
        /// <param name="d">The value to clamp.</param>
        /// <returns>0 or <paramref name="d"/>.</returns>
        public static double Clamp(this double d)
        {
            return (-Const.ZERO_CLAMP < d) && (d < Const.ZERO_CLAMP) ? 0.0: d;
        }

        /// <summary>
        /// Round the value to <c><paramref name="value"/></c> if <paramref name="d"/> is in ] <paramref name="value"/> - <see cref="Const.ZERO_CLAMP"/> ; <paramref name="value"/> + <see cref="Const.ZERO_CLAMP"/> [
        /// </summary>
        /// <param name="d">The value to clamp.</param>
        /// <param name="value">The clamp reference.</param>
        /// <returns><paramref name="value"/> or <paramref name="d"/>.</returns>
        public static double Clamp(this double d, double value)
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
        public static double Clamp(this double d, double value, double clampFactor)
        {
            return ((value - clampFactor * Const.ZERO_CLAMP) < d) && (d < (value + clampFactor * Const.ZERO_CLAMP)) ? value : d;
        }

        public static double Clamp3(this double d, double v1, double v2, double v3)
        {
            if ((v1 - Const.ZERO_CLAMP < d) && (d < v1 + Const.ZERO_CLAMP))
            {
                return v1;
            }
            else if ((v2 - Const.ZERO_CLAMP < d) && (d < v2 + Const.ZERO_CLAMP))
            {
                return v2;
            }
            else if ((v3 - Const.ZERO_CLAMP < d) && (d < v3 + Const.ZERO_CLAMP))
            {
                return v3;
            }
            return d;
        }

        public static double Clamp5(this double d, double v1, double v2, double v3, double v4, double v5)
        {
            if ((v1 - Const.ZERO_CLAMP < d) && (d < v1 + Const.ZERO_CLAMP))
            {
                return v1;
            }
            else if ((v2 - Const.ZERO_CLAMP < d) && (d < v2 + Const.ZERO_CLAMP))
            {
                return v2;
            }
            else if ((v3 - Const.ZERO_CLAMP < d) && (d < v3 + Const.ZERO_CLAMP))
            {
                return v3;
            }
            else if ((v4 - Const.ZERO_CLAMP < d) && (d < v4 + Const.ZERO_CLAMP))
            {
                return v4;
            }
            else if ((v5 - Const.ZERO_CLAMP) < d && (d < v5 + Const.ZERO_CLAMP))
            {
                return v5;
            }
            return d;
        }

        public static void Clamp(this ref Matrix3D matrix)
        {
            matrix.M11 = matrix.M11.Clamp3(0.0, -1.0, 1.0);
            matrix.M12 = matrix.M12.Clamp3(0.0, -1.0, 1.0);
            matrix.M13 = matrix.M13.Clamp3(0.0, -1.0, 1.0);
            matrix.M14 = matrix.M14.Clamp3(0.0, -1.0, 1.0);
            matrix.M21 = matrix.M21.Clamp3(0.0, -1.0, 1.0);
            matrix.M22 = matrix.M22.Clamp3(0.0, -1.0, 1.0);
            matrix.M23 = matrix.M23.Clamp3(0.0, -1.0, 1.0);
            matrix.M24 = matrix.M24.Clamp3(0.0, -1.0, 1.0);
            matrix.M31 = matrix.M31.Clamp3(0.0, -1.0, 1.0);
            matrix.M32 = matrix.M32.Clamp3(0.0, -1.0, 1.0);
            matrix.M33 = matrix.M33.Clamp3(0.0, -1.0, 1.0);
            matrix.M34 = matrix.M34.Clamp3(0.0, -1.0, 1.0);
            matrix.M44 = matrix.M44.Clamp3(0.0, -1.0, 1.0);
        }

        public static void Clamp(this ref Vector3D vector)
        {
            vector.X = vector.X.Clamp3(0.0, -1.0, 1.0);
            vector.Y = vector.Y.Clamp3(0.0, -1.0, 1.0);
            vector.Z = vector.Z.Clamp3(0.0, -1.0, 1.0);
        }

        public static Point4D ToPoint4D(this Point3D point)
        {
            return new Point4D(point.X, point.Y, point.Z, 1.0);
        }

        public static Point3D ToPoint3D(this Point4D point)
        {
            return new Point3D(point.X, point.Y, point.Z);
        }
    }
}

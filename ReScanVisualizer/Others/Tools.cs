using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using ReScanVisualizer.Models;

namespace ReScanVisualizer
{
    public static class Tools
    {
        public static double DegreeToRadian(double degree)
        {
            return Math.PI * degree / 180.0;
        }

        public static double RadianToDegree(double radian)
        {
            return 180.0 * radian / Math.PI;
        }

        public static double MixteProduct(Vector3D u, Vector3D v, Vector3D w)
        {
            return Vector3D.DotProduct(u, Vector3D.CrossProduct(v, w));
        }

        public static bool AreVectorsColinear(Vector3D vector1, Vector3D vector2)
        {
            if (vector1.Length == 0.0)
            {
                throw new ArgumentException("Can't compute Tool.AreVectorsColinear(Vector3D, Vector3D) because one them is the zero vector.", nameof(vector1));
            }
            double k;

            if (vector2.X != 0.0)
            {
                k = vector1.X / vector2.X;
            }
            else if (vector2.Y != 0.0)
            {
                k = vector1.Y / vector2.Y;
            }
            else if (vector2.Z != 0.0)
            {
                k = vector1.Z / vector2.Z;
            }
            else
            {
                throw new ArgumentException("Can't compute Tool.AreVectorsColinear(Vector3D, Vector3D) because one them is the zero vector.", nameof(vector2));
            }
            bool areColinear = (vector1.X - k * vector2.X).Clamp() == 0.0;
            areColinear &= (vector1.Y - k * vector2.Y).Clamp() == 0.0;
            areColinear &= (vector1.Z - k * vector2.Z).Clamp() == 0.0;
            return areColinear;
        }

        /// <summary>
        /// Retourne le cosinus de l'angle spécifié.
        /// </summary>
        /// <param name="d">Angle, mesuré en radians.</param>
        /// <returns>Cosinus de d. Si d est égal à <see cref="double.NaN"/>, à <see cref="double.NegativeInfinity"/> ou à <see cref="double.PositiveInfinity"/>, cette méthode retourne <see cref="double.NaN"/>.</returns>
        public static double Cos(double d)
        {
            double c = Math.Cos(d);
            if (!double.IsNaN(c) && !double.IsInfinity(c))
            {
                if (c <= -1.0 + Const.ZERO_CLAMP)
                {
                    c = -1.0;
                }
                else if (c >= 1.0 - Const.ZERO_CLAMP)
                {
                    c = 1.0;
                }
                else if (c <= Const.ZERO_CLAMP && c >= -Const.ZERO_CLAMP)
                {
                    c = 0.0;
                }
            }
            return c;
        }

        /// <summary>
        /// Retourne le sinus de l'angle spécifié.
        /// </summary>
        /// <param name="d">Angle, mesuré en radians.</param>
        /// <returns>Sinus de d. Si d est égal à <see cref="double.NaN"/>, à <see cref="double.NegativeInfinity"/> ou à <see cref="double.PositiveInfinity"/>, cette méthode retourne <see cref="double.NaN"/>.</returns>
        public static double Sin(double d)
        {
            double s = Math.Sin(d);
            if (!double.IsNaN(s) && !double.IsInfinity(s))
            {
                if (s <= -1.0 + Const.ZERO_CLAMP)
                {
                    s = -1.0;
                }
                else if (s >= 1.0 - Const.ZERO_CLAMP)
                {
                    s = 1.0;
                }
                else if (s <= Const.ZERO_CLAMP && s >= -Const.ZERO_CLAMP)
                {
                    s = 0.0;
                }
            }
            return s;
        }

        /// <summary>
        /// Retourne l'angle dont le cosinus est le nombre spécifié.
        /// </summary>
        /// <param name="d">Nombre représentant un cosinus, où d doit être supérieur ou égal à -1, mais inférieur ou égal à 1.</param>
        /// <returns>Angle θ mesuré en radians, tel que 0 ≤ θ ≤ π ou <see cref="double.NaN"/> si d < -1 ou d > 1 ou d est égal à  <see cref="double.NaN"/>.</returns>
        public static double Acos(double d)
        {
            double a = Math.Acos(d);
            if (!double.IsNaN(a))
            {
                if (a <= Const.ZERO_CLAMP)
                {
                    a = 0.0;
                }
                else if (a >= Math.PI - Const.ZERO_CLAMP)
                {
                    a = Math.PI;
                }
            }
            return a;
        }

        /// <summary>
        /// Retourne l'angle dont le sinus est le nombre spécifié.
        /// </summary>
        /// <param name="d">Nombre représentant un sinus, où d doit être supérieur ou égal à -1, mais inférieur ou égal à 1.</param>
        /// <returns>Angle θ mesuré en radians, tel que -π/2 ≤ θ ≤ π/2 ou <see cref="double.NaN"/> si d < -1 ou d > 1 ou d est égal à  <see cref="double.NaN"/>.</returns>
        public static double Asin(double d)
        {
            double a = Math.Asin(d);
            if (!double.IsNaN(a))
            {
                double pi2 = Math.PI / 2.0;
                if (a <= -pi2 + Const.ZERO_CLAMP)
                {
                    a = pi2;
                }
                else if (a >= pi2 - Const.ZERO_CLAMP)
                {
                    a = pi2;
                }
            }
            return a;
        }

        /// <summary>
        /// Retourne l'angle dont la tangente est le nombre spécifié.
        /// </summary>
        /// <param name="d">Nombre représentant une tangente.</param>
        /// <returns>Angle θ mesuré en radians, tel que -π/2 ≤ θ ≤ π/2,
        /// ou <see cref="double.NaN"/> si d est égal à <see cref="double.NaN"/>,
        /// ou -π/2 arrondi à la double précision (-1,5707963267949) si d est égal à <see cref="double.NegativeInfinity"/>,
        /// ou π/2 arrondi à la double précision (1,5707963267949) si d est égal à <see cref="double.PositiveInfinity"/>.</returns>
        public static double Atan(double d)
        {
            double a = Math.Atan(d);
            if (!double.IsNaN(a))
            {
                double pi2 = Math.PI / 2.0;
                if (a <= -pi2 + Const.ZERO_CLAMP)
                {
                    a = pi2;
                }
                else if (a >= pi2 - Const.ZERO_CLAMP)
                {
                    a = pi2;
                }
            }
            return a;
        }

        /// <summary>
        /// Retourne le cosinus de l'angle spécifié en degré.
        /// </summary>
        /// <param name="d">Angle, mesuré en radians.</param>
        /// <returns>Cosinus de d en degrés. Si d est égal à <see cref="double.NaN"/>, à <see cref="double.NegativeInfinity"/> ou à <see cref="double.PositiveInfinity"/>, cette méthode retourne <see cref="double.NaN"/>.</returns>
        public static double CosD(double d)
        {
            return 180.0 * Cos(d) / Math.PI;
        }

        /// <summary>
        /// Retourne le sinus de l'angle spécifié en degrés.
        /// </summary>
        /// <param name="d">Angle, mesuré en radians.</param>
        /// <returns>Sinus de d en degrés. Si d est égal à <see cref="double.NaN"/>, à <see cref="double.NegativeInfinity"/> ou à <see cref="double.PositiveInfinity"/>, cette méthode retourne <see cref="double.NaN"/>.</returns>
        public static double SinD(double d)
        {
            return 180.0 * Sin(d) / Math.PI;
        }

        /// <summary>
        /// Retourne l'angle en degrés dont le cosinus est le nombre spécifié.
        /// </summary>
        /// <param name="d">Nombre représentant un cosinus, où d doit être supérieur ou égal à -1, mais inférieur ou égal à 1.</param>
        /// <returns>Angle θ mesuré en degrés, tel que 0 ≤ θ ≤ 90 ou <see cref="double.NaN"/> si d < -1 ou d > 1 ou d est égal à  <see cref="double.NaN"/>.</returns>
        public static double AcosD(double d)
        {
            return 180.0 * Acos(d) / Math.PI;
        }

        /// <summary>
        /// Retourne l'angle en degrés dont le sinus est le nombre spécifié.
        /// </summary>
        /// <param name="d">Nombre représentant un sinus, où d doit être supérieur ou égal à -1, mais inférieur ou égal à 1.</param>
        /// <returns>Angle θ mesuré en degrés, tel que -90 ≤ θ ≤ 90 ou <see cref="double.NaN"/> si d < -1 ou d > 1 ou d est égal à  <see cref="double.NaN"/>.</returns>
        public static double AsinD(double d)
        {
            return 180.0 * Asin(d) / Math.PI;
        }

        /// <summary>
        /// Retourne l'angle en degrés dont la tangente est le nombre spécifié.
        /// </summary>
        /// <param name="d">Nombre représentant une tangente.</param>
        /// <returns>Angle θ mesuré en degrés, tel que -90 ≤ θ ≤ 90,
        /// ou <see cref="double.NaN"/> si d est égal à <see cref="double.NaN"/>,
        /// ou -90 si d est égal à <see cref="double.NegativeInfinity"/>,
        /// ou 90 si d est égal à <see cref="double.PositiveInfinity"/>.</returns>
        public static double AtanD(double d)
        {
            return 180.0 * Atan(d) / Math.PI;
        }

        /// <param name="axis">Rotation axis</param>
        /// <param name="angle">Angle in radian</param>
        public static Matrix3D CreateRotationMatrix(Axis axis, double angle)
        {
            double cosa = Math.Cos(angle);
            double sina = Math.Sin(angle);
            return axis switch
            {
                Axis.X => new Matrix3D(1, 0, 0, 0,
                                       0, cosa, -sina, 0,
                                       0, sina, cosa, 0,
                                       0, 0, 0, 1),

                Axis.Y => new Matrix3D(cosa, 0, -sina, 0,
                                          0, 1, 0, 0,
                                       sina, 0, cosa, 0,
                                          0, 0, 0, 1),

                Axis.Z => new Matrix3D(cosa, -sina, 0, 0,
                                       sina, cosa, 0, 0,
                                          0, 0, 1, 0,
                                          0, 0, 0, 1),

                _ => throw new NotImplementedException()
            };
        }

        public static Matrix3D CreateRotationMatrix(Vector3D u, double angle)
        {
            double uxx = u.X * u.X;
            double uxy = u.X * u.Y;
            double uxz = u.X * u.Z;
            double uyy = u.Y * u.Y;
            double uyz = u.Y * u.Z;
            double uzz = u.Z * u.Z;

            double cosa = Cos(angle);
            double sina = Sin(angle);

            double _1_cosa = 1.0 - cosa;
            double ux_sina = u.X * sina;
            double uy_sina = u.Y * sina;
            double uz_sina = u.Z * sina;

            return new Matrix3D(
                uxx * _1_cosa + cosa, uxy * _1_cosa - uz_sina, uxz * _1_cosa + uy_sina, 0,
                uxy * _1_cosa + uz_sina, uyy * _1_cosa + cosa, uyz * _1_cosa - ux_sina, 0,
                uxz * _1_cosa - uy_sina, uyz * _1_cosa + ux_sina, uzz * _1_cosa + cosa, 0,
                0, 0, 0, 1);
        }

        public static Base3D ComputeOrientedBase(Vector3D direction, Axis axis)
        {
            Matrix3D rot = Matrix3D.Identity;
            Vector3D rotationAxis;
            Base3D base3D = new Base3D();
            double angle;

            direction.Normalize();

            switch (axis)
            {
                case Axis.X:
                    rotationAxis = Vector3D.CrossProduct(base3D.X, direction);
                    angle = Vector3D.AngleBetween(base3D.X, direction);
                    break;

                case Axis.Y:
                    rotationAxis = Vector3D.CrossProduct(base3D.Y, direction);
                    angle = Vector3D.AngleBetween(base3D.Y, direction);
                    break;

                case Axis.Z:
                    rotationAxis = Vector3D.CrossProduct(base3D.Z, direction);
                    angle = Vector3D.AngleBetween(base3D.Z, direction);
                    break;

                default:
                    throw new NotImplementedException();
            }

            angle = angle.Clamp().Clamp(-180).Clamp(180) % 180.0;

            if (angle != 0.0)
            {
                rotationAxis.Normalize();

                Quaternion q = new Quaternion(rotationAxis, -angle);
                rot.Rotate(q);

                rot.Clamp();
                base3D.X = new Vector3D(rot.M11, rot.M21, rot.M31);
                base3D.Y = new Vector3D(rot.M12, rot.M22, rot.M32);
                base3D.Z = new Vector3D(rot.M13, rot.M23, rot.M33);
            }
            return base3D;
        }

        public static List<RenderQuality> GetRenderQualitiesList()
        {
            return new List<RenderQuality>()
            {
                RenderQuality.VeryLow,
                RenderQuality.Low,
                RenderQuality.Medium,
                RenderQuality.High,
                RenderQuality.VeryHigh,
            };
        }

        public static List<Axis> GetAxisList()
        {
            return new List<Axis> { Axis.X, Axis.Y, Axis.Z };
        }

        public static List<RotationAxis> GetRotationAxesList()
        {
            return new List<RotationAxis> { RotationAxis.X, RotationAxis.Y, RotationAxis.Z, RotationAxis.Personalized };
        }

        public static List<Plan2D> GetPlan2DList()
        {
            return new List<Plan2D> { Plan2D.XY, Plan2D.XZ, Plan2D.YZ };
        }

        public static Color GetRandomLightColor()
        {
            PropertyInfo[] propertyInfos = typeof(Colors).GetProperties();
            List<Color> colors = new List<Color>();
            Random random = new Random();
            string pName;

            foreach (var propertyInfo in propertyInfos)
            {
                pName = propertyInfo.Name;
                if (pName.StartsWith("Dark") || pName == "Black")
                {
                    continue;
                }
                colors.Add((Color)propertyInfo.GetValue(propertyInfo));
            }

            return colors[random.Next(colors.Count)];
        }

        /// <summary>
        /// Get the angle in degrees between the plan XY and the given vector.
        /// </summary>
        /// <param name="vector">The vector</param>
        /// <returns>The relative (or absolute if specified) angle in degrees.</returns>
        public static double AngleZ(Vector3D vector, bool absolute = false)
        {
            double z = vector.Z;
            if (z < 0.0 && absolute)
            {
                z = -z;
            }
            return RadianToDegree(Math.Atan(z / Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y)));
        }

        public static bool IsNumericType(Type type)
        {
            if (type == null)
                return false;

            return type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) ||
                   type == typeof(ushort) || type == typeof(int) || type == typeof(uint) ||
                   type == typeof(long) || type == typeof(ulong) || type == typeof(float) ||
                   type == typeof(double) || type == typeof(decimal);
        }

        /// <summary>
        /// Express the coordinates of the base1 in the base2.
        /// </summary>
        /// <returns>The base1 according the base2.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Base3D GetBase1IntoBase2(Base3D base1, Base3D base2)
        {
            Matrix3D tb10 = base1.ToMatrix3D();        // matrice de la base 1 dans R0
            Matrix3D tb20 = base2.ToMatrix3D();        // matrice de la base 2 dans R0

            // rapporter les bases à l'origine de R0
            tb20.OffsetX = 0.0;
            tb20.OffsetY = 0.0;
            tb20.OffsetZ = 0.0;
            tb10.OffsetX = 0.0;
            tb10.OffsetY = 0.0;
            tb10.OffsetZ = 0.0;

            if (!tb20.HasInverse)
            {
                throw new InvalidOperationException("The matrix can't be inverted.");
            }

            Matrix3D tb02 = tb20.Inverse();                // matrice de passage de base2 vers R0

            Matrix3D tb12 = Matrix3D.Multiply(tb02, tb10); // base 1 dans la base 2

            return new Base3D(tb12);
        }

        public static List<VerticalAlignment> GetVerticalAlignmentList()
        {
            return new List<VerticalAlignment>()
            {
                VerticalAlignment.Top,
                VerticalAlignment.Center,
                VerticalAlignment.Bottom,
                VerticalAlignment.Stretch
            };
        }

        public static List<HorizontalAlignment> GetHorizontalAlignmentList()
        {
            return new List<HorizontalAlignment>()
            {
                HorizontalAlignment.Left,
                HorizontalAlignment.Center,
                HorizontalAlignment.Right,
                HorizontalAlignment.Stretch
            };
        }

        public static Rect3D GetGlobalRect(List<Rect3D> rects)
        {
            if (rects.Count == 0)
            {
                throw new ArgumentException("rects are empty", nameof(rects));
            }
            Rect3D globalRect = rects[0];
            for (int i = 1; i < rects.Count; i++)
            {
                globalRect = Rect3D.Union(globalRect, rects[i]);
            }

            return globalRect;
        }

        public static bool TryParse(string text, out bool result)
        {
            result = false;
            if (text == "0" || text == "false" || text == "False" || text == "f" || text == "F")
            {
                return true;
            }
            if (text == "1" || text == "true" || text == "True" || text == "t" || text == "T")
            {
                result = true;
                return true;
            }
            return false;
        }

        public static bool TryParse(string text, out float result)
        {
            if (text.Contains(','))
            {
                text = text.Replace(',', '.');
            }
            return float.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
        }

        public static bool TryParse(string text, out double result)
        {
            if (text.Contains(','))
            {
                text = text.Replace(',', '.');
            }
            return double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
        }

        public static bool TryParse(string text, out decimal result)
        {
            if (text.Contains(','))
            {
                text = text.Replace(',', '.');
            }
            return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
        }

        public static bool IsPortInUse(int port, ProtocolType protocolType)
        {
            bool isPortInUse = false;
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            switch (protocolType)
            {
                case ProtocolType.Tcp:
                    IPEndPoint[] tcpListeners = ipGlobalProperties.GetActiveTcpListeners();
                    foreach (IPEndPoint endpoint in tcpListeners)
                    {
                        if (endpoint.Port == port)
                        {
                            isPortInUse = true;
                            break;
                        }
                    }
                    break;

                case ProtocolType.Udp:
                    IPEndPoint[] udpListeners = ipGlobalProperties.GetActiveUdpListeners();
                    foreach (IPEndPoint endpoint in udpListeners)
                    {
                        if (endpoint.Port == port)
                        {
                            isPortInUse = true;
                            break;
                        }
                    }
                    break;
            }

            return isPortInUse;
        }
    }
}

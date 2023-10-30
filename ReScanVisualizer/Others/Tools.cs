using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

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
            return vector1.X == k * vector2.X && vector1.Y == k * vector2.Y && vector1.Z == k * vector2.Z;
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

            double cosa = Math.Cos(angle).Clamp().Clamp(-1).Clamp(1);
            double sina = Math.Sin(angle).Clamp().Clamp(-1).Clamp(1);

            double _1_cosa = 1.0 - cosa;
            double ux_sina = u.X * sina;
            double uy_sina = u.Y * sina;
            double uz_sina = u.Z * sina;

            return new Matrix3D(
                uxx * _1_cosa + cosa,    uxy * _1_cosa - uz_sina, uxz * _1_cosa + uy_sina, 0,
                uxy * _1_cosa + uz_sina, uyy * _1_cosa + cosa,    uyz * _1_cosa - ux_sina, 0,
                uxz * _1_cosa - uy_sina, uyz * _1_cosa + ux_sina, uzz * _1_cosa + cosa,    0,
                0, 0, 0, 1);
        }

        public static Base3D ComputeOrientedBase(Vector3D direction, Axis axis)
        {
            Matrix3D rot = Matrix3D.Identity;
            Vector3D rotationAxis;
            Base3D base3D = new Base3D();
            double angle;

            if (direction.Length != 1.0)
            {
                direction.Normalize();
            }

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

            //angle = angle.Clamp().Clamp(-180).Clamp(180) % 360.0;
            angle = angle.Clamp().Clamp(-180).Clamp(180) % 180.0;

            if (angle != 0.0)
            {
                //if (angle == 180.0 || angle == -180.0)
                //{
                //    switch (axis)
                //    {
                //        case Axis.X:
                //            rot.M22 *= -1.0;
                //            rot.M33 *= -1.0;
                //            break;
                //        case Axis.Y:
                //            rot.M11 *= -1.0;
                //            rot.M33 *= -1.0;
                //            break;
                //        case Axis.Z:
                //            rot.M11 *= -1.0;
                //            rot.M22 *= -1.0;
                //            break;
                //    }
                //}
                //else
                //{
                //    rot.Rotate(new Quaternion(rotationAxis, -angle));
                //}
                rot.Rotate(new Quaternion(rotationAxis, -angle));

                rot.Clamp();
                base3D.X = new Vector3D(rot.M11, rot.M21, rot.M31);
                base3D.Y = new Vector3D(rot.M12, rot.M22, rot.M32);
                base3D.Z = new Vector3D(rot.M13, rot.M23, rot.M33);
            }
            return base3D;
        }
    }
}

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
            switch (axis)
            {
                case Axis.X:
                    return new Matrix3D(
                        1,    0,     0, 0,
                        0, cosa, -sina, 0,
                        0, sina,  cosa, 0,
                        0,    0,     0, 1);

                case Axis.Y:
                    return new Matrix3D(
                        cosa, 0, -sina, 0,
                           0, 1,     0, 0,
                        sina, 0,  cosa, 0,
                           0, 0,     0, 1);
                case Axis.Z:
                    return new Matrix3D(
                        cosa, -sina, 0, 0,
                        sina,  cosa, 0, 0,
                           0,     0, 1, 0,
                           0,     0, 0, 1);

                default:
                    throw new NotImplementedException();
            }
        }

        public static Repere3D ComputeOrientedRepere(Vector3D direction, Axis axis)
        {
            return ComputeOrientedRepere2(direction, axis);
            double a;
            double b;
            double dx;
            double dy;
            double dz;

            double cosa;
            double cosb;
            double sina;
            double sinb;

            double cosa_cosb;
            double cosa_sinb;
            double sina_cosb;
            double sina_sinb;

            Repere3D result = new Repere3D();
            Matrix3D rot;


            if (direction.Length != 1.0)
            {
                direction.Normalize();
            }

            switch (axis)
            {
                case Axis.X:
                    dx = direction.X - result.X.X;
                    dy = direction.Y - result.X.Y;
                    dz = direction.Z - result.X.Z;
                    a = Math.Atan2(dy, dx);
                    b = Math.Atan2(dz, dx);
                    break;

                case Axis.Y:
                    dx = direction.X - result.Y.X;
                    dy = direction.Y - result.Y.Y;
                    dz = direction.Z - result.Y.Z;
                    a = Math.Atan2(dx, dy);
                    b = Math.Atan2(dz, dy);
                    break;

                case Axis.Z:
                    dx = direction.X - result.Z.X;
                    dy = direction.Y - result.Z.Y;
                    dz = direction.Z - result.Z.Z;
                    a = Math.Atan2(dx, dz);
                    b = Math.Atan2(dy, dz);
                    break;

                default:
                    throw new NotImplementedException();
            }

            cosa = Math.Cos(a);
            cosb = Math.Cos(b);
            sina = Math.Sin(a);
            sinb = Math.Sin(b);
            cosa_cosb = cosa * cosb;
            cosa_sinb = cosa * sinb;
            sina_cosb = sina * cosb;
            sina_sinb = sina * sinb;

            rot = new Matrix3D(
                        cosa_cosb, -sina, cosa_sinb, 0,
                        sina_cosb, cosa, sina_sinb, 0,
                            -sinb, 0, cosb, 0,
                                0, 0, 0, 1);

            result.X = new Vector3D(rot.M11, rot.M21, rot.M31);
            result.Y = new Vector3D(rot.M12, rot.M22, rot.M32);
            result.Z = new Vector3D(rot.M13, rot.M23, rot.M33);

            return result;
        }

        public static Repere3D ComputeOrientedRepere2(Vector3D direction, Axis axis)
        {
            double a;
            double b;
            Matrix3D rot;
            Repere3D result = new Repere3D();
            
            switch (axis)
            {
                case Axis.X:
                    a = Math.Atan2(direction.Y, direction.X);
                    b = Math.Atan2(direction.Z, direction.X);
                    rot = CreateRotationMatrix(Axis.Z, a) * CreateRotationMatrix(Axis.X, b);
                    break;

                case Axis.Y:
                    a = -Math.Atan2(direction.X, direction.Y);
                    b = Math.Atan2(direction.Z, direction.X);
                    rot = CreateRotationMatrix(Axis.Z, a) * CreateRotationMatrix(Axis.X, b);
                    break;

                case Axis.Z:
                    a = Math.Atan2(direction.Y, direction.X);
                    b = -Math.Atan2(direction.Z, direction.X);
                    rot = CreateRotationMatrix(Axis.Y, b) * CreateRotationMatrix(Axis.X, a);
                    break;

                default:
                    throw new NotImplementedException();
            }

            result.X = new Vector3D(rot.M11, rot.M21, rot.M31);
            result.Y = new Vector3D(rot.M12, rot.M22, rot.M32);
            result.Z = new Vector3D(rot.M13, rot.M23, rot.M33);

            return result;
        }
    }
}

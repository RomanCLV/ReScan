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
    }
}

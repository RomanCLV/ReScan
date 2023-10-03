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
            // TODO : tester la methode AreVectorsColinear
            if (vector1.Length == 0.0)
            {
                throw new ArgumentException("Can't compute Tool.AreVectorsColinear(Vector3D, Vector3D) because one them is the zero vector.", nameof(vector1));
            }
            if (vector2.Length == 0.0)
            {
                throw new ArgumentException("Can't compute Tool.AreVectorsColinear(Vector3D, Vector3D) because one them is the zero vector.", nameof(vector2));
            }

            // verification des cas avec des 0
            if ((vector1.X == 0 && vector2.X != 0) ||
                (vector1.X != 0 && vector2.X == 0) ||
                (vector1.Y == 0 && vector2.Y != 0) ||
                (vector1.Y != 0 && vector2.Y == 0) ||
                (vector1.Z == 0 && vector2.Z != 0) ||
                (vector1.Z != 0 && vector2.Z == 0)
                )
            {
                return false;
            }

            // Calculer les rapports entre les composantes des vecteurs
            double kx = vector1.X == 0 ? double.NaN : vector2.X / vector1.X;
            double ky = vector1.Y == 0 ? double.NaN : vector2.Y / vector1.Y;
            double kz = vector1.Z == 0 ? double.NaN : vector2.Z / vector1.Z;

            if (double.IsNaN(kx) || double.IsNaN(ky) || double.IsNaN(kz))
            {
                return true;
                // TODO : renforcer ici
            }

            return (Math.Abs(kx - ky) < Const.ZERO_CLAMP) && (Math.Abs(ky - kz) < Const.ZERO_CLAMP);
        }
    }
}

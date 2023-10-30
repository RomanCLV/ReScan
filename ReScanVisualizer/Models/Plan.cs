using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.Models
{
    public class Plan
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double D { get; set; }

        public Plan(double a = 1.0, double b = 1.0, double c = 0.0, double d = 0.0)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public Plan(Vector3D normal, double d = 0.0)
        {
            A = normal.X;
            B = normal.Y;
            C = normal.Z;
            D = d;
        }

        public Plan(Plan plan)
        {
            A = plan.A;
            B = plan.B;
            C = plan.C;
            D = plan.D;
        }

        public void SetABCD(double a, double b, double c, double d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public void SetABCD(Plan plan)
        {
            A = plan.A;
            B = plan.B;
            C = plan.C;
            D = plan.D;
        }

        public Vector3D GetNormal()
        {
            return new Vector3D(A, B, C);
        }

        public Point3D GetOrthogonalProjection(Point3D point)
        {
            /*
	        Soit un plan P : ax + by + cz + d = 0
	        Soit un point A. Le projete orthogonale du point A, nomme A', appartient a la droite L passant par A et de direction n, vecteur normal
	        au plan P, de coordonees n = (a, b, c).

	        Si un point M appartient a L, alors il verifie le systeme :

		        Xm = a*t + Xa
	        S1: Ym = b*t + Ya
		        Zm = c*t + Za

	        De plus, si un point M apparient au plan P, alors a*Xm + b*Ym + c*Zm + d = 0

	        A' est un point qui appartient a L et a P donc :

		        a*Xa' + b*Ya' + c*Za' + d = 0

	        <=> a*(a*t + Xa) + b*(b*t + Ya) + c*(c*t + Za) + d = 0
	        <=> t = - (a*Xa + b*Ya + c*Za) / (a^2 + b^2 + c^2)

	        Maintenant qu'on a t, on peut trouver les coordonnees du point A' en le mettant dans S1.
	        */

            double t = -(A * point.X + B * point.Y + C * point.Z + D) / (A * A + B * B + C * C);
            return new Point3D(
                A * t + point.X,
                B * t + point.Y,
                C * t + point.Z
            );
        }

        static double GetDistanceFrom(Plan plan, Point3D point)
        {
            /*
			         | a*Xm + b*Ym + c*Zm + d  |
	        D(P,M) = | ----------------------- |
			         |    sqrt(a^2+b^2+c^2)	   |
	        */
            return Math.Abs((plan.A * point.X + plan.B * point.Y + plan.C * point.Z + plan.D) / Math.Sqrt(plan.A * plan.A + plan.B * plan.B + plan.C * plan.C));
        }

        public string ToStr(string begin = "{ ", string end = " }", string sep = " ")
        {
            return $"{begin}{A}{sep}{B}{sep}{C}{sep}{D}{end}";
        }

        public override string ToString()
        {
            return ToStr();
        }
    }
}

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

        public Plan(double a = 0.0, double b = 0.0, double c = 0.0, double d = 0.0)
        {
            A = a;
            B = b;
            C = c;
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

        public Vector3D GetNormal()
        {
            return new Vector3D(A, B, C);
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

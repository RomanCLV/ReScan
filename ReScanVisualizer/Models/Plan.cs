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
        private double _a;
        private double _b;
        private double _c;
        private double _d;

        public Plan(double a = 0.0, double b = 0.0, double c = 0.0, double d = 0.0)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
        }

        public Plan(Plan plan)
        {
            _a = plan._a;
            _b = plan._b;
            _c = plan._c;
            _d = plan._d;
        }

        public double GetA()
        {
            return _a;
        }

        public void SetA(double a)
        {
            _a = a;
        }

        public double GetB()
        {
            return _b;
        }

        public void SetB(double b)
        {
            _b = b;
        }

        public double GetC()
        {
            return _c;
        }

        public void SetC(double c)
        {
            _c = c;
        }

        public double GetD()
        {
            return _d;
        }

        public void SetD(double d)
        {
            _d = d;
        }

        public void SetABCD(double a, double b, double c, double d)
        {
            SetA(a);
            SetB(b);
            SetC(c);
            SetD(d);
        }

        public Vector3D GetNormal()
        {
            return new Vector3D(_a, _b, _c);
        }

        public string ToStr(string begin = "{ ", string end = " }", string sep = " ")
        {
            return $"{begin}{_a}{sep}{_b}{sep}{_c}{sep}{_d}{end}";
        }

        public override string ToString()
        {
            return ToStr();
        }
    }
}

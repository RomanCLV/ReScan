using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.Models
{
    public class Repere3D
    {
        public Point3D Origin { get; set; }
        public Vector3D X { get; set; }
        public Vector3D Y { get; set; }
        public Vector3D Z { get; set; }

        public Repere3D() : this(new Point3D(), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1))
        { }

        public Repere3D(Point3D origin) : this(origin, new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1))
        { }

        public Repere3D(Vector3D x, Vector3D y, Vector3D z) : this(new Point3D(), x, y, z)
        { }

        public Repere3D(Point3D origin, Vector3D x, Vector3D y, Vector3D z)
        {
            Origin = origin;
            X = x;
            Y = y;
            Z = z;
        }
    }
}

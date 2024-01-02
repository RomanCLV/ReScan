using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.Models
{
    public class CameraConfiguration
    {
        public Point3D Position { get; protected set; }
        public Point3D Target { get; protected set; }
        public Vector3D Direction => Target - Position;
        public double Distance => Direction.Length;

        public CameraConfiguration(Point3D position, Point3D target) 
        {
            Position = position;
            Target = target;
        }
    }
}

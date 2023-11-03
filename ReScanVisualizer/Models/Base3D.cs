using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

#nullable enable

namespace ReScanVisualizer.Models
{
    public class Base3D
    {
        public event EventHandler<Point3D>? OriginChanged;
        public event EventHandler<Vector3D>? XChanged;
        public event EventHandler<Vector3D>? YChanged;
        public event EventHandler<Vector3D>? ZChanged;

        private Point3D _origin;
        public Point3D Origin
        {
            get => _origin;
            set
            {
                if (_origin != value)
                {
                    _origin = value;
                    OnOriginChanged();
                }
            }
        }

        private Vector3D _x;
        public Vector3D X
        {
            get => _x;
            set
            {
                value.Normalize();
                if (_x != value)
                {
                    _x = value;
                    OnXChanged();
                }
            }
        }

        private Vector3D _y;
        public Vector3D Y
        {
            get => _y;
            set
            {
                value.Normalize();
                if (_y != value)
                {
                    _y = value;
                    OnYChanged();
                }
            }
        }

        private Vector3D _z;
        public Vector3D Z
        {
            get => _z;
            set
            {
                value.Normalize();
                if (_z != value)
                {
                    _z = value;
                    OnZChanged();
                }
            }
        }

        public Base3D() : this(new Point3D(), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1))
        { }

        public Base3D(Point3D origin) : this(origin, new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1))
        { }

        public Base3D(Vector3D x, Vector3D y, Vector3D z) : this(new Point3D(), x, y, z)
        { }

        public Base3D(Point3D origin, Vector3D x, Vector3D y, Vector3D z)
        {
            _origin = origin;
            _x = x;
            _y = y;
            _z = z;
        }

        public void Translate(double x = 0.0, double y = 0.0, double z = 0.0)
        {
            if (x != 0.0)
            {
                _origin.X += x;
            }
            if (y != 0.0)
            {
                _origin.Y += y;
            }
            if (z != 0.0)
            {
                _origin.Z += z;
            }
            if (x != 0.0 || y != 0.0 || z != 0.0)
            {
                OnOriginChanged();
            }
        }

        private void OnOriginChanged()
        {
            OriginChanged?.Invoke(this, _origin);
        }

        private void OnXChanged()
        {
            XChanged?.Invoke(this, _x);
        }

        private void OnYChanged()
        {
            YChanged?.Invoke(this, _y);
        }

        private void OnZChanged()
        {
            ZChanged?.Invoke(this, _z);
        }

        public Matrix3D GetRotationMatrix()
        {
            Vector3D xAxis = new Vector3D(1, 0, 0);
            Vector3D rotationAxis = Vector3D.CrossProduct(xAxis, X);
            double angle = Vector3D.AngleBetween(xAxis, X);
            Matrix3D rot;
            if (angle == 0)
            {
                rot = Matrix3D.Identity;
                return rot;
            }
            rot = new Matrix3D();
            rot.Rotate(new Quaternion(rotationAxis, -angle));
            rot.Clamp();
            return rot;
        }

        public Plan GetPlan(Plan2D plan2D)
        {
            var normal = plan2D switch
            {
                Plan2D.XY => _z,
                Plan2D.XZ => _y,
                Plan2D.YZ => _x,
                _ => throw new InvalidOperationException("Unexpected plan."),
            };
            return new Plan(normal, -(normal.X * _origin.X + normal.Y * _origin.Y + normal.Z * _origin.Z));
        }

        public override string ToString()
        {
            return $"{X.X};{X.Y};{X.Z};{Y.X};{Y.Y};{Y.Z};{Z.X};{Z.Y};{Z.Z}";
        }
    }
}

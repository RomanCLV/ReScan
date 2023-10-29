using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.Models
{
    public class Base3D
    {
        public event EventHandler<Point3D> OriginChanged;
        public event EventHandler<Vector3D> XChanged;
        public event EventHandler<Vector3D> YChanged;
        public event EventHandler<Vector3D> ZChanged;
        public event EventHandler<double> ThetaChanged;
        public event EventHandler<double> PhiChanged;

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

        private double _theta;
        public double Theta
        {
            get => _theta;
            set
            {
                if (_theta != value)
                {
                    _theta = value;
                    OnThetaChanged();
                }
            }
        }

        private double _phi;
        public double Phi
        {
            get => _phi;
            set
            {
                if (_phi != value)
                {
                    _phi = value;
                    OnPhiChanged();
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
            Origin = origin;
            X = x;
            Y = y;
            Z = z;
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

        private void OnThetaChanged()
        {
            ThetaChanged?.Invoke(this, _theta);
        }

        private void OnPhiChanged()
        {
            PhiChanged?.Invoke(this, _phi);
        }

        public Matrix3D GetRotationMatrix()
        {
            Vector3D xAxis = new Vector3D(1, 0, 0);
            Vector3D rotationAxis = Vector3D.CrossProduct(xAxis, X);
            double angle = Vector3D.AngleBetween(xAxis, X);
            if (angle == 0)
            {
                return Matrix3D.Identity;
            }
            Matrix3D rot = new Matrix3D();
            rot.Rotate(new Quaternion(rotationAxis, -angle));
            rot.Clamp();
            return rot;
        }

        public override string ToString()
        {
            return $"{X.X};{X.Y};{X.Z};{Y.X};{Y.Y};{Y.Z};{Z.X};{Z.Y};{Z.Z}";
        }
    }
}

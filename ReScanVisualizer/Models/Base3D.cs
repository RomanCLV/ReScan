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

        public bool IsRotating { get; private set; }

        private Vector3D _beginRotateX;
        private Vector3D _beginRotateY;
        private Vector3D _beginRotateZ;

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
            Vector3D rotationAxis = Vector3D.CrossProduct(xAxis, _x);
            double angle = Vector3D.AngleBetween(xAxis, _x);
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
            Vector3D normal = plan2D switch
            {
                Plan2D.XY => _z,
                Plan2D.XZ => _y,
                Plan2D.YZ => _x,
                _ => throw new InvalidOperationException("Unexpected plan."),
            };
            return new Plan(normal, -(normal.X * _origin.X + normal.Y * _origin.Y + normal.Z * _origin.Z));
        }

        /// <summary>
        /// Indicates the start of a base rotation. To call before <see cref="Rotate(Vector3D, double, bool)"/>.<br />
        /// Sets <see cref="IsRotating"/> to true and saves the current positions of the vectors for use during rotation.
        /// </summary>
        public void BeginRotate()
        {
            IsRotating = true;
            _beginRotateX = _x;
            _beginRotateY = _y;
            _beginRotateZ = _z;
        }

        /// <summary>
        /// Indicates the end of a rotation. To be called after <see cref="Rotate(Vector3D, double, bool)"/>.
        /// </summary>
        public void EndRotate()
        {
            IsRotating = false;
        }

        /// <summary>
        /// Rotate the base according to a given direction and an angle.<br />
        /// <see cref="BeginRotate"/> is called automatically if <see cref="IsRotating"/> is false.
        /// </summary>
        /// <param name="rotationAxis">The direction</param>
        /// <param name="rotationAngle">The angle in degrees</param>
        /// <param name="autoCallEndRotate">Call <see cref="EndRotate"/> automatically.<br />
        /// If you have only one rotation, let it to true. Else, set it to false and don't forget to call <see cref="EndRotate"/> when all the rotations have been applied.
        /// </param>
        public void Rotate(Vector3D rotationAxis, double rotationAngle, bool autoCallEndRotate = true)
        {
            if (rotationAngle == 0.0)
            {
                return;
            }
            if (!IsRotating)
            {
                BeginRotate();
            }
            Matrix3D rot = Matrix3D.Identity;
            rot.Rotate(new Quaternion(rotationAxis, rotationAngle));

            X = Vector3D.Multiply(_beginRotateX, rot);
            Y = Vector3D.Multiply(_beginRotateY, rot);
            Z = Vector3D.Multiply(_beginRotateZ, rot);
            if (autoCallEndRotate)
            {
                EndRotate();
            }
        }

        public override string ToString()
        {
            return $"{X.X};{X.Y};{X.Z};{Y.X};{Y.Y};{Y.Z};{Z.X};{Z.Y};{Z.Z}";
        }
    }
}

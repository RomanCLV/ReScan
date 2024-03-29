﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#nullable enable

namespace ReScanVisualizer.Models
{
    public class Base3D
    {
        public event EventHandler<PositionEventArgs>? OriginChanged;
        public event EventHandler? PreviewXYZChanged;
        public event EventHandler<Vector3D>? XChanged;
        public event EventHandler<Vector3D>? YChanged;
        public event EventHandler<Vector3D>? ZChanged;
        public event EventHandler? XYZChanged;

        #region Origin access properties

        private Point3D _origin;
        public Point3D Origin
        {
            get => _origin;
            set
            {
                if (_origin != value)
                {
                    Point3D oldPosition = _origin;
                    _origin = value;
                    Point3D newPosition = _origin;
                    OnOriginChanged(ref oldPosition, ref newPosition);
                }
            }
        }

        public double OX
        {
            get => _origin.X;
            set
            {
                if (_origin.X != value)
                {
                    Point3D oldPosition = _origin;
                    _origin.X = value;
                    Point3D newPosition = _origin;
                    OnOriginChanged(ref oldPosition, ref newPosition);
                }
            }
        }

        public double OY
        {
            get => _origin.Y;
            set
            {
                if (_origin.Y != value)
                {
                    Point3D oldPosition = _origin;
                    _origin.Y = value;
                    Point3D newPosition = _origin;
                    OnOriginChanged(ref oldPosition, ref newPosition);
                }
            }
        }

        public double OZ
        {
            get => _origin.Z;
            set
            {
                if (_origin.Z != value)
                {
                    Point3D oldPosition = _origin;
                    _origin.Z = value;
                    Point3D newPosition = _origin;
                    OnOriginChanged(ref oldPosition, ref newPosition);
                }
            }
        }

        #endregion

        #region X access properties

        private Vector3D _x;
        public Vector3D X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnXChanged();
                }
            }
        }

        public double XX
        {
            get => _x.X;
            set
            {
                if (_x.X != value)
                {
                    _x.X = value;
                    OnXChanged();
                }
            }
        }

        public double XY
        {
            get => _x.Y;
            set
            {
                if (_x.Y != value)
                {
                    _x.Y = value;
                    OnXChanged();
                }
            }
        }

        public double XZ
        {
            get => _x.Z;
            set
            {
                if (_x.Z != value)
                {
                    _x.Z = value;
                    OnXChanged();
                }
            }
        }

        #endregion

        #region Y access properties

        private Vector3D _y;
        public Vector3D Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnYChanged();
                }
            }
        }

        public double YX
        {
            get => _y.X;
            set
            {
                if (_y.X != value)
                {
                    _y.X = value;
                    OnYChanged();
                }
            }
        }

        public double YY
        {
            get => _y.Y;
            set
            {
                if (_y.Y != value)
                {
                    _y.Y = value;
                    OnYChanged();
                }
            }
        }

        public double YZ
        {
            get => _y.Z;
            set
            {
                if (_y.Z != value)
                {
                    _y.Z = value;
                    OnYChanged();
                }
            }
        }

        #endregion

        #region Z access properties

        private Vector3D _z;
        public Vector3D Z
        {
            get => _z;
            set
            {
                if (_z != value)
                {
                    _z = value;
                    OnZChanged();
                }
            }
        }

        public double ZX
        {
            get => _z.X;
            set
            {
                if (_z.X != value)
                {
                    _z.X = value;
                    OnYChanged();
                }
            }
        }

        public double ZY
        {
            get => _z.Y;
            set
            {
                if (_z.Y != value)
                {
                    _z.Y = value;
                    OnYChanged();
                }
            }
        }

        public double ZZ
        {
            get => _z.Z;
            set
            {
                if (_z.Z != value)
                {
                    _z.Z = value;
                    OnYChanged();
                }
            }
        }

        #endregion

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

        public Base3D(Matrix3D matrix3D)
        {
            /*
             | M11  M12  M13  OffsetX |
             | M21  M22  M23  OffsetY |
             | M31  M32  M33  OffsetZ |
             |  0    0    0     1     |
             */
            _origin = new Point3D(matrix3D.OffsetX, matrix3D.OffsetY, matrix3D.OffsetZ);
            _x = new Vector3D(matrix3D.M11, matrix3D.M21, matrix3D.M31);
            _y = new Vector3D(matrix3D.M12, matrix3D.M22, matrix3D.M32);
            _z = new Vector3D(matrix3D.M13, matrix3D.M23, matrix3D.M33);
        }

        public Base3D(Base3D base3D)
        {
            _origin = base3D._origin;
            _x = base3D._x;
            _y = base3D._y;
            _z = base3D._z;
        }

        public static explicit operator Matrix3D(Base3D base3D)
        {
            return base3D.ToMatrix3D();
        }

        public void Translate(double x = 0.0, double y = 0.0, double z = 0.0)
        {
            Point3D oldPosition = _origin;
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
            Point3D newPosition = _origin;
            if (x != 0.0 || y != 0.0 || z != 0.0)
            {
                OnOriginChanged(ref oldPosition, ref newPosition);
            }
        }

        private void OnOriginChanged(ref Point3D oldPosition, ref Point3D newPosition)
        {
            OriginChanged?.Invoke(this, new PositionEventArgs(oldPosition, newPosition));
        }

        private void OnPreviewXYZChanged()
        {
            PreviewXYZChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnXYZChanged()
        {
            XYZChanged?.Invoke(this, EventArgs.Empty);
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

        public void NormalizeX()
        {
            _x.Normalize();
            OnXChanged();
        }

        public void NormalizeY()
        {
            _y.Normalize();
            OnYChanged();
        }

        public void NormalizeZ()
        {
            _z.Normalize();
            OnZChanged();
        }

        public void Normalize()
        {
            OnPreviewXYZChanged();
            _x.Normalize();
            _y.Normalize();
            _z.Normalize();
            OnXChanged();
            OnYChanged();
            OnZChanged();
            OnXYZChanged();
        }

        public Matrix3D ToMatrix3D()
        {
            /*
             |    M11     M12    M13   0 |
             |    M21     M22    M23   0 |
             |    M31     M32    M33   0 |
             | OffsetX OffsetY OffsetZ 1 |
             */
            return new Matrix3D(
                _x.X, _y.X, _z.X, 0,
                _x.Y, _y.Y, _z.Y, 0,
                _x.Z, _y.Z, _z.Z, 0,
                _origin.X, _origin.Y, _origin.Z, 1);
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
            _beginRotateX.Normalize();
            _beginRotateY.Normalize();
            _beginRotateZ.Normalize();
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
            if (!IsRotating)
            {
                BeginRotate();
            }
            Matrix3D rot = Matrix3D.Identity;
            rot.Rotate(new Quaternion(rotationAxis, rotationAngle));
            rot.Clamp();

            Vector3D x = Vector3D.Multiply(_beginRotateX, rot);
            Vector3D y = Vector3D.Multiply(_beginRotateY, rot);
            Vector3D z = Vector3D.Multiply(_beginRotateZ, rot);

            x.Clamp();
            y.Clamp();
            z.Clamp();

            OnPreviewXYZChanged();
            X = x;
            Y = y;
            Z = z;
            OnXYZChanged();

            if (autoCallEndRotate)
            {
                EndRotate();
            }
        }

        public void Flip()
        {
            X *= -1.0;
            Y *= -1.0;
            Z *= -1.0;
        }

        public void ResetAxis(Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    _x.X = 1.0;
                    _x.Y = 0.0;
                    _x.Z = 0.0;
                    OnXChanged();
                    break;
                case Axis.Y:
                    _y.X = 0.0;
                    _y.Y = 1.0;
                    _y.Z = 0.0;
                    OnYChanged();
                    break;
                case Axis.Z:
                    _z.X = 0.0;
                    _z.Y = 0.0;
                    _z.Z = 1.0;
                    OnZChanged();
                    break;
            }
        }

        public void ResetAllAxis()
        {
            _x.X = 1.0;
            _x.Y = 0.0;
            _x.Z = 0.0;
            _y.X = 0.0;
            _y.Y = 1.0;
            _y.Z = 0.0;
            _z.X = 0.0;
            _z.Y = 0.0;
            _z.Z = 1.0;
            OnXChanged();
            OnYChanged();
            OnZChanged();
        }

        public void SetFrom(Base3D base3D, bool setOrigin = true)
        {
            if (setOrigin)
            {
                Point3D oldOrigin = _origin;
                Point3D newOrigin = base3D._origin;
                _origin.X = base3D._origin.X;
                _origin.Y = base3D._origin.Y;
                _origin.Z = base3D._origin.Z;
                OnOriginChanged(ref oldOrigin, ref newOrigin);
            }
            _x.X = base3D._x.X;
            _x.Y = base3D._x.Y;
            _x.Z = base3D._x.Z;
            _y.X = base3D._y.X;
            _y.Y = base3D._y.Y;
            _y.Z = base3D._y.Z;
            _z.X = base3D._z.X;
            _z.Y = base3D._z.Y;
            _z.Z = base3D._z.Z;
            OnXChanged();
            OnYChanged();
            OnZChanged();
        }

        public void SetFrom(Matrix3D matrix3D, bool setOrigin = true)
        {
            if (setOrigin)
            {
                Point3D oldOrigin = _origin;
                _origin.X = matrix3D.OffsetX;
                _origin.Y = matrix3D.OffsetY;
                _origin.Z = matrix3D.OffsetZ;
                Point3D newOrigin = _origin;
                OnOriginChanged(ref oldOrigin, ref newOrigin);
            }
            _x.X = matrix3D.M11;
            _x.Y = matrix3D.M21;
            _x.Z = matrix3D.M31;
            _y.X = matrix3D.M12;
            _y.Y = matrix3D.M22;
            _y.Z = matrix3D.M32;
            _z.X = matrix3D.M13;
            _z.Y = matrix3D.M23;
            _z.Z = matrix3D.M33;
            OnXChanged();
            OnYChanged();
            OnZChanged();
        }

        public void ToEulerAngleZYX(out double a, out double b, out double c)
        {
            Tools.Matrix3DToEulerAnglesZYX(ToMatrix3D(), out a, out b, out c);
        }

        public void SetFromEulerAnglesZYX(double a, double b, double c)
        {
            SetFrom(Tools.EulerAnglesZYXToMatrix3D(a, b, c), false);
        }

        public override string ToString()
        {
            return $"{X.X};{X.Y};{X.Z};{Y.X};{Y.Y};{Y.Z};{Z.X};{Z.Y};{Z.Z}";
        }
    }
}

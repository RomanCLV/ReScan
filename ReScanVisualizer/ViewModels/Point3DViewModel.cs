using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class Point3DViewModel : ViewModelBase
    {
        private Point3D _point;
        public Point3D Point
        {
            get => _point;
            set
            {
                if (CorrectX != null)
                {
                    value.X = CorrectX(value.X);
                }
                if (CorrectY != null)
                {
                    value.Y = CorrectY(value.Y);
                }
                if (CorrectZ != null)
                {
                    value.Z = CorrectZ(value.Z);
                }

                if (SetValue(ref _point, value))
                {
                    OnPropertyChanged(nameof(X));
                    OnPropertyChanged(nameof(Y));
                    OnPropertyChanged(nameof(Z));
                }
            }
        }

        public double X
        {
            get => _point.X;
            set
            {
                if (CorrectX != null) 
                {
                    value = CorrectX(value);
                }
                if (_point.X != value)
                {
                    _point.X = value;
                    OnPropertyChanged(nameof(X));
                    OnPropertyChanged(nameof(Point));
                }
            }
        }

        public double Y
        {
            get => _point.Y;
            set
            {
                if (CorrectY != null)
                {
                    value = CorrectY(value);
                }
                if (_point.Y != value)
                {
                    _point.Y = value;
                    OnPropertyChanged(nameof(Y));
                    OnPropertyChanged(nameof(Point));
                }
            }
        }

        public double Z
        {
            get => _point.Z;
            set
            {
                if (CorrectZ != null)
                {
                    value = CorrectZ(value);
                }
                if (_point.Z != value)
                {
                    _point.Z = value;
                    OnPropertyChanged(nameof(Z));
                    OnPropertyChanged(nameof(Point));
                }
            }
        }

        public Func<double, double>? CorrectX { get; set; }
        public Func<double, double>? CorrectY { get; set; }
        public Func<double, double>? CorrectZ { get; set; }

        public Point3DViewModel() : this(new Point3D())
        { }

        public Point3DViewModel(Point3D point3D)
        {
            _point = point3D;
        }

        public void Set(Point3D point3D)
        {
            Point = point3D;
        }

        public override string ToString()
        {
            return _point.ToString();
        }
    }
}

using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

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
            set => SetValue(_point.X, value);
        }

        public double Y
        {
            get => _point.Y;
            set => SetValue(_point.Y, value);
        }

        public double Z
        {
            get => _point.Z;
            set => SetValue(_point.Z, value);
        }

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

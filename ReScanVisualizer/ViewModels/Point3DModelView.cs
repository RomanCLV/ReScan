using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class Point3DModelView : ViewModelBase
    {
        private readonly Point3D _point;

		public double X
		{
			get => _point.X;
            set
            {
                if (SetValue(_point.X, value))
                {
                    BuildGeometryModel3D();
                }
            }
        }

        public double Y
        {
            get => _point.Y;
            set
            {
                if (SetValue(_point.Y, value))
                {
                    BuildGeometryModel3D();
                }
            }
        }

        public double Z
        {
            get => _point.Z;
            set
            {
                if (SetValue(_point.Z, value))
                {
                    BuildGeometryModel3D();
                }
            }
        }

        private double _radius;
        public double Radius
        {
            get => _radius;
            set
            {
                if (SetValue(ref _radius, value))
                {
                    BuildGeometryModel3D();
                }
            }
        }

        private Brush _brush;
        public Brush Brush
        {
            get => _brush;
            set
            {
                if (SetValue(ref _brush, value))
                {
                    BuildGeometryModel3D();
                }
            }
        }

        private GeometryModel3D? _geometryModel3D;
        public GeometryModel3D? GeometryModel3D
        {
            get => _geometryModel3D;
            set => SetValue(ref _geometryModel3D, value);
        }

        public Point3DModelView() : this(new Point3D())
        {
        }

        public Point3DModelView(double x, double y, double z) : this(new Point3D(x, y, z))
        {
        }

        public Point3DModelView(Point3D point3D) : this(point3D, 1.0, Brushes.White)
        {
        }

        public Point3DModelView(Point3D point3D, double radius, Brush brush)
        {
            _point = point3D;
            _radius = radius;
            _brush = brush;
            _geometryModel3D = Helper3D.Helper3D.BuildSphere(_point, _radius, _brush);
        }

        private void BuildGeometryModel3D()
        {
            GeometryModel3D = Helper3D.Helper3D.BuildSphere(_point, _radius, _brush);
        }
    }
}

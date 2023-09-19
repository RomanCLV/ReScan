using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
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
    public class Point3DViewModel : ViewModelBase, I3DElement
    {
        private readonly Point3D _point;

        public double X
        {
            get => _point.X;
            set
            {
                if (SetValue(_point.X, value))
                {
                    BuildGeometry3D();
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
                    BuildGeometry3D();
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
                    BuildGeometry3D();
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
                    BuildGeometry3D();
                }
            }
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (SetValue(ref _color, value))
                {
                    OnPropertyChanged(nameof(ColorR));
                    OnPropertyChanged(nameof(ColorG));
                    OnPropertyChanged(nameof(ColorB));
                    OnPropertyChanged(nameof(ColorOpacity));
                }
            }
        }

        public byte ColorR
        {
            get => _color.R;
            set
            {
                if (SetValue(_color.R, value))
                {
                    BuildMaterial();
                }
            }
        }

        public byte ColorG
        {
            get => _color.G;
            set
            {
                if (SetValue(_color.G, value))
                {
                    BuildMaterial();
                }
            }
        }

        public byte ColorB
        {
            get => _color.B;
            set
            {
                if (SetValue(_color.B, value))
                {
                    BuildMaterial();
                }
            }
        }

        public byte ColorOpacity
        {
            get => _color.A;
            set
            {
                if (SetValue(_color.A, value))
                {
                    UpdateOldOpacity();
                    BuildMaterial();
                }
            }
        }

        private byte _oldOpacity;

        private bool _isHiden;
        public bool IsHiden
        {
            get => _isHiden;
            set
            {
                if (SetValue(ref _isHiden, value))
                {
                    if (_isHiden)
                    {
                        UpdateOldOpacity();
                        ColorOpacity = 0;
                    }
                    else
                    {
                        ColorOpacity = _oldOpacity;
                    }
                }
            }
        }

        private Model3D _model;
        public Model3D Model
        {
            get => _model;
            set => SetValue(ref _model, value);
        }

        private Geometry3D _modelGeometry;

        public Point3DViewModel() : this(new Point3D())
        {
        }

        public Point3DViewModel(double x, double y, double z) : this(new Point3D(x, y, z))
        {
        }

        public Point3DViewModel(Point3D point3D) : this(point3D, 1.0, Colors.White)
        {
        }

        public Point3DViewModel(Point3D point3D, double radius, Color color)
        {
            _color = color;
            _oldOpacity = color.A;
            _point = point3D;
            _radius = radius;
            _modelGeometry = Helper3D.Helper3D.BuildSphereGeometry(_point, _radius);
            _model = Helper3D.Helper3D.BuildModel(_modelGeometry, new SolidColorBrush(_color));
        }

        private void UpdateOldOpacity()
        {
            _oldOpacity = _color.A;
        }

        private void BuildGeometry3D()
        {
            _modelGeometry = Helper3D.Helper3D.BuildSphereGeometry(_point, _radius);
            ((GeometryModel3D)_model).Geometry = _modelGeometry;
        }

        private void BuildMaterial()
        {
            ((GeometryModel3D)_model).Material = MaterialHelper.CreateMaterial(new SolidColorBrush(_color));
        }

        public void Hide()
        {
            IsHiden = true;
        }

        public void Show()
        {
            IsHiden = false;
        }
    }
}

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
        private Point3D _point;
        public Point3D Point 
        { 
            get => _point;
            set
            {
                if (SetValue(ref _point, value))
                {
                    UpdateModelGeometry();

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
                if (SetValue(_point.X, value))
                {
                    UpdateModelGeometry();
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
                    UpdateModelGeometry();
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
                    UpdateModelGeometry();
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
                    UpdateModelGeometry();
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
                    UpdateModelMaterial();
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
                    UpdateModelMaterial();
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
                    UpdateModelMaterial();
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
                    UpdateModelMaterial();
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
                    OnIsHidenChanged();
                }
            }
        }

        public event EventHandler<bool>? IsHidenChanged;

        private Model3D _model;
        public Model3D Model
        {
            get => _model;
            set => SetValue(ref _model, value);
        }

        public Point3DViewModel() : this(new Point3D())
        {
        }

        public Point3DViewModel(double x, double y, double z) : this(new Point3D(x, y, z))
        {
        }

        public Point3DViewModel(Point3D point3D) : this(point3D, Colors.White, 1.0)
        {
        }

        public Point3DViewModel(Color color) : this(new Point3D(), color, 1.0)
        {
        }

        public Point3DViewModel(Point3D point3D, Color color, double radius = 1.0)
        {
            _color = color;
            _oldOpacity = color.A;
            _isHiden = _oldOpacity == 0;

            _point = point3D;
            _radius = radius;
            _model = Helper3D.Helper3D.BuildSphereModel(_point, _radius, _color);
        }

        private void UpdateOldOpacity()
        {
            _oldOpacity = _color.A;
        }

        public void Hide()
        {
            IsHiden = true;
        }

        public void Show()
        {
            IsHiden = false;
        }

        public void UpdateModelGeometry()
        {
            ((GeometryModel3D)_model).Geometry = Helper3D.Helper3D.BuildSphereGeometry(_point, _radius);
        }

        public void UpdateModelMaterial()
        {
            GeometryModel3D model = (GeometryModel3D)_model;
            model.Material = MaterialHelper.CreateMaterial(new SolidColorBrush(_color));
            model.BackMaterial = model.Material;
        }

        private void OnIsHidenChanged()
        {
            IsHidenChanged?.Invoke(this, _isHiden);
        }

        public override string ToString()
        {
            return _point.ToString();
        }
    }
}

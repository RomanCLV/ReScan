using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class SampleViewModel : ViewModelBase, I3DElement
    {
        private double _scaleFactor;
        public double ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Scale factor must be greater than 0.");
                }
                if (SetValue(ref _scaleFactor, value))
                {
                    // TODO: rebuild all
                }
            }
        }

        public Point3DViewModel Point { get; private set; }

        private Point3D PointScalled => Point.Point.Multiply(_scaleFactor);
        private double RadiusScalled => Math.Max(0.001, _radius * _scaleFactor);

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

        public ColorViewModel Color { get; set; }

        private byte _oldOpacity;

        private bool _isHidenChanging;

        private bool _isHiden;
        public bool IsHiden
        {
            get => _isHiden;
            set
            {
                if (SetValue(ref _isHiden, value))
                {
                    _isHidenChanging = true;
                    if (_isHiden)
                    {
                        UpdateOldOpacity();
                        Color.A = 0;
                    }
                    else
                    {
                        Color.A = _oldOpacity;
                    }
                    _isHidenChanging = false;
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

        public SampleViewModel() : this(new Point3D())
        {
        }

        public SampleViewModel(double x, double y, double z) : this(new Point3D(x, y, z))
        {
        }

        public SampleViewModel(Point3D point3D) : this(point3D, Colors.White)
        {
        }

        public SampleViewModel(Color color) : this(new Point3D(), color)
        {
        }

        public SampleViewModel(Point3D point3D, Color color, double scaleFactor = 1.0, double radius = 0.5)
        {
            if (scaleFactor <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleFactor), "Scale factor must be greater than 0.");
            }
            IsDisposed = false;
            Color = new ColorViewModel(color);
            _scaleFactor = scaleFactor;
            _oldOpacity = color.A;
            _isHidenChanging = false;
            _isHiden = _oldOpacity == 0;

            _radius = radius;
            Point = new Point3DViewModel(point3D)
            {
                CorrectX = CorrectX,
                CorrectY = CorrectY,
                CorrectZ = CorrectZ
            };

            _model = Helper3D.Helper3D.BuildSphereModel(PointScalled, RadiusScalled, Color.Color);

            Color.PropertyChanged += Color_PropertyChanged;
            Point.PropertyChanged += Point_PropertyChanged;
        }

        ~SampleViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Color.PropertyChanged -= Color_PropertyChanged;
                Point.PropertyChanged -= Point_PropertyChanged;
                base.Dispose();
            }
        }

        private void Color_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Color.A))
            {
                if (!_isHidenChanging)
                {
                    UpdateOldOpacity();
                    if (_isHiden && Color.A != 0)
                    {
                        IsHiden = false;
                    }
                }
            }
            if (e.PropertyName == nameof(Color.Color))
            {
                UpdateModelMaterial();
            }
        }

        private void Point_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateModelGeometry();
        }

        private void UpdateOldOpacity()
        {
            _oldOpacity = Color.A;
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
            ((GeometryModel3D)_model).Geometry = Helper3D.Helper3D.BuildSphereGeometry(PointScalled, RadiusScalled);
        }

        public void UpdateModelMaterial()
        {
            GeometryModel3D model = (GeometryModel3D)_model;
            model.Material = MaterialHelper.CreateMaterial(new SolidColorBrush(Color.Color));
            model.BackMaterial = model.Material;
        }

        private void OnIsHidenChanged()
        {
            IsHidenChanged?.Invoke(this, _isHiden);
        }

        private double CorrectX(double x)
        {
            if (x < ScatterGraphBuilderBase.MIN_X)
            {
                x = ScatterGraphBuilderBase.MIN_X;
            }
            else if (x > ScatterGraphBuilderBase.MAX_X)
            {
                x = ScatterGraphBuilderBase.MAX_X;
            }
            return x;
        }

        private double CorrectY(double y)
        {
            if (y < ScatterGraphBuilderBase.MIN_Y)
            {
                y = ScatterGraphBuilderBase.MIN_Y;
            }
            else if (y > ScatterGraphBuilderBase.MAX_Y)
            {
                y = ScatterGraphBuilderBase.MAX_Y;
            }
            return y;
        }

        private double CorrectZ(double z)
        {
            if (z < ScatterGraphBuilderBase.MIN_Z)
            {
                z = ScatterGraphBuilderBase.MIN_Z;
            }
            else if (z > ScatterGraphBuilderBase.MAX_Z)
            {
                z = ScatterGraphBuilderBase.MAX_Z;
            }
            return z;
        }

        public override string ToString()
        {
            return Point.ToString();
        }
    }
}

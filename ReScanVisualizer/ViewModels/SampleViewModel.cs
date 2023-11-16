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
                    UpdateModelGeometry();
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

        private Material? _oldModelMaterial;

        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                if (SetValue(ref _isHidden, value))
                {
                    if (_isHidden)
                    {
                        _oldModelMaterial = _model.Material;
                        _model.Material = null;
                        _model.BackMaterial = null;
                    }
                    else
                    {
                        if (Color.A == 0)
                        {
                            Color.A = 255;
                            //UpdateModelMaterial();
                        }
                        else
                        {
                            _model.Material = _oldModelMaterial;
                            _model.BackMaterial = _oldModelMaterial;
                        }
                    }
                    OnIsHidenChanged();
                }
            }
        }

        private RenderQuality _renderQuality;
        public RenderQuality RenderQuality
        {
            get => _renderQuality;
            set
            {
                if (SetValue(ref _renderQuality, value))
                {
                    UpdateModelGeometry();
                }
            }
        }

        public event EventHandler<bool>? IsHiddenChanged;

        private readonly GeometryModel3D _model;

        public Model3D Model => _model;

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

        public SampleViewModel(Point3D point3D, Color color, double scaleFactor = 1.0, double radius = 0.5, RenderQuality renderQuality = RenderQuality.High)
        {
            if (scaleFactor <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleFactor), "Scale factor must be greater than 0.");
            }
            IsDisposed = false;
            Color = new ColorViewModel(color);
            _scaleFactor = scaleFactor;
            _isHidden = color.A == 0;
            _renderQuality = renderQuality;
            _radius = radius;
            Point = new Point3DViewModel(point3D)
            {
                CorrectX = CorrectX,
                CorrectY = CorrectY,
                CorrectZ = CorrectZ
            };

            _model = Helper3D.Helper3D.BuildSphereModel(PointScalled, RadiusScalled, Color.Color, _renderQuality);

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
                Color.PropertyChanged -= Color_PropertyChanged;
                Point.PropertyChanged -= Point_PropertyChanged;
                base.Dispose();
                IsDisposed = true;
            }
        }

        private void Color_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Color))
            {
                if (_isHidden)
                {
                    IsHidden = false;
                }
                UpdateModelMaterial();
                if (!_isHidden && Color.A == 0)
                {
                    IsHidden = true;
                }
            }
        }

        private void Point_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateModelGeometry();
        }

        public void InverseIsHidden()
        {
            IsHidden = !_isHidden;
        }

        public void Hide()
        {
            IsHidden = true;
        }

        public void Show()
        {
            IsHidden = false;
        }

        public void UpdatePoint(Point3D barycenter)
        {
            Point.Set(barycenter);
        }

        public void UpdateModelGeometry()
        {
            _model.Geometry = Helper3D.Helper3D.BuildSphereGeometry(PointScalled, RadiusScalled, _renderQuality);
        }

        public void UpdateModelMaterial()
        {
            _model.Material = MaterialHelper.CreateMaterial(new SolidColorBrush(Color.Color));
            _model.BackMaterial = _model.Material;
        }

        private void OnIsHidenChanged()
        {
            IsHiddenChanged?.Invoke(this, _isHidden);
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

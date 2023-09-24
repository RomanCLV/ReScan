using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
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
        private const double _scaleFactor = 1;

        public Point3DViewModel Point { get; private set; }

        private Point3D PointScalled => new Point3D(Point.X * _scaleFactor, Point.Y * _scaleFactor, Point.Z * _scaleFactor);

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
                        Color.A = 0;
                    }
                    else
                    {
                        Color.A = _oldOpacity;
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

        public SampleViewModel() : this(new Point3D())
        {
        }

        public SampleViewModel(double x, double y, double z) : this(new Point3D(x, y, z))
        {
        }

        public SampleViewModel(Point3D point3D) : this(point3D, Colors.White, 1.0)
        {
        }

        public SampleViewModel(Color color) : this(new Point3D(), color, 1.0)
        {
        }

        public SampleViewModel(Point3D point3D, Color color, double radius = 1.0)
        {
            IsDisposed = false;
            Color = new ColorViewModel(color);
            _oldOpacity = color.A;
            _isHiden = _oldOpacity == 0;

            _radius = radius;
            Point = new Point3DViewModel(point3D);
            _model = Helper3D.Helper3D.BuildSphereModel(PointScalled, _radius * _scaleFactor, Color.Color);

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
                UpdateOldOpacity();
            }
            UpdateModelMaterial();
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
            ((GeometryModel3D)_model).Geometry = Helper3D.Helper3D.BuildSphereGeometry(PointScalled, _radius * _scaleFactor);
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

        public override string ToString()
        {
            return Point.ToString();
        }
    }
}

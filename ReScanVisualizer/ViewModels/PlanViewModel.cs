using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using HelixToolkit.Wpf;
using System.Windows;
using System.ComponentModel;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class PlanViewModel : ViewModelBase, I3DElement
    {
        private double _scaleFactor;
        public double ScaleFactor
        {
            get => _scaleFactor;
            set => SetValue(ref _scaleFactor, value);
        }

        private Plan _plan;
        public Plan Plan
        {
            get => _plan;
            set
            {
                if (SetValue(ref _plan, value))
                {
                    UpdateModelGeometry();

                    OnPropertyChanged(nameof(A));
                    OnPropertyChanged(nameof(B));
                    OnPropertyChanged(nameof(C));
                    OnPropertyChanged(nameof(D));
                }
            }
        }

        public double A
        {
            get => _plan.A;
            set
            {
                if (_plan.A != value)
                {
                    OnPropertyChanged(nameof(A));
                    OnPropertyChanged(nameof(Plan));
                    UpdateModelGeometry();
                }
            }
        }

        public double B
        {
            get => _plan.B;
            set
            {
                if (_plan.B != value)
                {
                    OnPropertyChanged(nameof(B));
                    OnPropertyChanged(nameof(Plan));
                    UpdateModelGeometry();
                }
            }
        }

        public double C
        {
            get => _plan.C;
            set
            {
                if (_plan.C != value)
                {
                    OnPropertyChanged(nameof(C));
                    OnPropertyChanged(nameof(Plan));
                    UpdateModelGeometry();
                }
            }
        }

        public double D
        {
            get => _plan.D;
            set
            {
                if (_plan.D != value)
                {
                    OnPropertyChanged(nameof(D));
                    OnPropertyChanged(nameof(Plan));
                    UpdateModelGeometry();
                }
            }
        }

        private Point3D _center;
        public Point3D Center
        {
            get => _center;
            set
            {
                _center = value;
                UpdateModelGeometry();
            }
        }

        private Point3D CenterScalled => _center.Multiply(_scaleFactor);


        private Vector3D _up;
        public Vector3D Up
        {
            get => _up;
            set
            {
                _up = value;
                UpdateModelGeometry();
            }
        }

        private double _dist;
        public double Dist
        {
            get => _dist;
            set
            {
                if (SetValue(ref _dist, value))
                {
                    UpdateModelGeometry();
                }
            }
        }

        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                if (SetValue(ref _width, value))
                {
                    UpdateModelGeometry();
                }
            }
        }

        private double _height;
        public double Height
        {
            get => _height;
            set
            {
                if (SetValue(ref _height, value))
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

        public PlanViewModel() : this(new Plan(), new Point3D(), new Vector3D(0, 0, 1), Colors.LightBlue.ChangeAlpha(191))
        { }

        public PlanViewModel(Color color) : this(new Plan(), new Point3D(), new Vector3D(0, 0, 1), color)
        { }

        public PlanViewModel(Plan plan, Point3D center, Vector3D up) : this(plan, center, up, Colors.LightBlue.ChangeAlpha(191))
        { }

        public PlanViewModel(Plan plan, Point3D center, Vector3D up, Color color, double width = 10, double height = 10, double dist = 0.0)
        {
            IsDisposed = false;
            _scaleFactor = 1.0;
            _plan = plan;
            _center = center;
            _up = up;
            _dist = dist;
            _width = width;
            _height = height;
            Color = new ColorViewModel(color);
            _oldOpacity = Color.A;
            _isHidenChanging = false;
            _isHiden = _oldOpacity == 0;

            _model = Helper3D.Helper3D.BuildPlanModel(_center, _plan.GetNormal(), _up, _width, _height, _dist, Color.Color);

            Color.PropertyChanged += Color_PropertyChanged;
        }

        ~PlanViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Color.PropertyChanged -= Color_PropertyChanged;
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
            ((GeometryModel3D)_model).Geometry = Helper3D.Helper3D.BuildPlanGeometry(CenterScalled, _plan.GetNormal(), _up, _width, _height, _dist);
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
            return _plan.ToString();
        }
    }
}

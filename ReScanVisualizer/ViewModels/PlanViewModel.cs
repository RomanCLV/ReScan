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

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class PlanViewModel : ViewModelBase, I3DElement
    {
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
                if (SetValue(_plan.A, value))
                {
                    UpdateModelGeometry();
                }
            }
        }

        public double B
        {
            get => _plan.B;
            set
            {
                if (SetValue(_plan.B, value))
                {
                    UpdateModelGeometry();
                }
            }
        }

        public double C
        {
            get => _plan.C;
            set
            {
                if (SetValue(_plan.C, value))
                {
                    UpdateModelGeometry();
                }
            }
        }

        public double D
        {
            get => _plan.D;
            set
            {
                if (SetValue(_plan.D, value))
                {
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

        public PlanViewModel() : this(new Plan(), new Point3D(), new Vector3D(0, 0, 1), Colors.LightBlue.ChangeAlpha(191))
        { }

        public PlanViewModel(Color color) : this(new Plan(), new Point3D(), new Vector3D(0, 0, 1), color)
        { }

        public PlanViewModel(Plan plan, Point3D center, Vector3D up) : this(plan, center, up, Colors.LightBlue.ChangeAlpha(191))
        { }

        public PlanViewModel(Plan plan, Point3D center, Vector3D up, Color color, double width = 10, double height = 10, double dist = 0.0)
        {
            _plan = plan;
            _center = center;
            _up = up;
            _dist = dist;
            _width = width;
            _height = height;
            _color = color;
            _oldOpacity = color.A;
            _isHiden = _oldOpacity == 0;

            _model = Helper3D.Helper3D.BuildPlanModel(_center, _plan.GetNormal(), _up, _width, _height, _dist, _color);
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
            ((GeometryModel3D)_model).Geometry = Helper3D.Helper3D.BuildPlanGeometry(_center, _plan.GetNormal(), _up, _width, _height, _dist);
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
    }
}

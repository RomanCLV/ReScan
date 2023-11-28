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
using System.Runtime.InteropServices;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class PlanViewModel : ViewModelBase, I3DElement
    {
        public event EventHandler<bool>? IsHiddenChanged;

        private bool _canEdit;
        public bool CanEdit
        {
            get => _canEdit;
            set => SetValue(ref _canEdit, value);
        }

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

        private double _lenght;
        private double LengthScalled => _lenght * _scaleFactor;

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
                            Color.A = 191;
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
            set => SetValue(ref _renderQuality, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            private set => SetValue(ref _isSelected, value);
        }

        private bool _isMouseOver;
        public bool IsMouseOver
        {
            get => _isMouseOver;
            set => SetValue(ref _isMouseOver, value);
        }

        private readonly GeometryModel3D _model;
        public Model3D Model => _model;

        public PlanViewModel() : this(new Plan(), new Point3D(), new Vector3D(0, 0, 1), Colors.LightBlue.ChangeAlpha(191))
        { }

        public PlanViewModel(Color color) : this(new Plan(), new Point3D(), new Vector3D(0, 0, 1), color)
        { }

        public PlanViewModel(Plan plan, Point3D center, Vector3D up) : this(plan, center, up, Colors.LightBlue.ChangeAlpha(191))
        { }

        public PlanViewModel(Plan plan, Point3D center, Vector3D up, Color color, double length = 10.0, double scaleFactor = 1.0, RenderQuality renderQuality = RenderQuality.High)
        {
            if (scaleFactor <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleFactor), "Scale factor must be greater than 0.");
            }
            _canEdit = true;
            _scaleFactor = scaleFactor;
            _plan = plan;
            _center = center;
            _up = up;
            _lenght = length;
            Color = new ColorViewModel(color);
            _isHidden = color.A == 0;
            _renderQuality = renderQuality;
            _isSelected = false;
            _isMouseOver = false;
            _model = Helper3D.Helper3D.BuildPlanModel(CenterScalled, _plan.GetNormal(), _up, LengthScalled, LengthScalled, 0, Color.Color);

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
                Color.PropertyChanged -= Color_PropertyChanged;
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

        public void Select()
        {
            IsSelected = true;
        }

        public void Unselect()
        {
            IsSelected = false;
        }

        public void UpdateModelGeometry()
        {
            _model.Geometry = Helper3D.Helper3D.BuildPlanGeometry(CenterScalled, _plan.GetNormal(), _up, LengthScalled, LengthScalled, 0);
        }

        public void UpdateModelMaterial()
        {
            _model.Material = MaterialHelper.CreateMaterial(new SolidColorBrush(Color.Color));
            _model.BackMaterial = _model.Material;
        }

        public bool IsBelongingToModel(GeometryModel3D geometryModel3D)
        {
            return _model.Geometry.Equals(geometryModel3D.Geometry);
        }

        private void OnIsHidenChanged()
        {
            IsHiddenChanged?.Invoke(this, _isHidden);
        }

        public void UpdateCenter(Point3D center)
        {
            _center = center;
            UpdateModelGeometry();
        }

        public void UpdatePlan(Plan plan, Vector3D up, double length)
        {
            _plan.SetABCD(plan);
            _up = up;
            _lenght = length;
            UpdateModelGeometry();
        }

        public void UpdatePlan(Point3D center, Plan plan, Vector3D up, double length)
        {
            _center = center;
            UpdatePlan(plan, up, length);
        }

        public override string ToString()
        {
            return _plan.ToString();
        }
    }
}

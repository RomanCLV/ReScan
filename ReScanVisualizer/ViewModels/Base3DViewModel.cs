using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ReScanVisualizer.Models;
using HelixToolkit.Wpf;
using ReScanVisualizer.ViewModels.Parts;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class Base3DViewModel : ViewModelBase, I3DElement, IScatterGraphElement, ICameraFocusable
    {
        public event EventHandler<bool>? IsHiddenChanged;

        private readonly Base3D _base3D;
        public Base3D Base3D => _base3D;

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

        private double _axisScaleFactor;
        public double AxisScaleFactor
        {
            get => _axisScaleFactor;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Axis scale factor must be greater than 0.");
                }
                if (SetValue(ref _axisScaleFactor, value))
                {
                    UpdateModelGeometry();
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_canEditName)
                {
                    SetValue(ref _name, value);
                }
            }
        }

        #region Origin access properties

        public Point3D Origin
        {
            get => _base3D.Origin;
            set => _base3D.Origin = value;
        }

        public double OX
        {
            get => _base3D.Origin.X;
            set => _base3D.OX = value;
        }

        public double OY
        {
            get => _base3D.Origin.Y;
            set => _base3D.OY = value;
        }

        public double OZ
        {
            get => _base3D.Origin.Z;
            set => _base3D.OZ = value;
        }

        #endregion

        #region X access properties

        public Vector3D X
        {
            get => _base3D.X;
            set => _base3D.X = value;
        }


        public double XX
        {
            get => _base3D.X.X;
            set => _base3D.XX = value;
        }

        public double XY
        {
            get => _base3D.X.Y;
            set => _base3D.XY = value;
        }

        public double XZ
        {
            get => _base3D.X.Z;
            set => _base3D.XZ = value;
        }

        #endregion

        #region Y access properties

        public Vector3D Y
        {
            get => _base3D.Y;
            set => _base3D.Y = value;
        }

        public double YX
        {
            get => _base3D.Y.X;
            set => _base3D.YX = value;
        }

        public double YY
        {
            get => _base3D.Y.Y;
            set => _base3D.YY = value;
        }

        public double YZ
        {
            get => _base3D.Y.Z;
            set => _base3D.YZ = value;
        }

        #endregion

        #region Z access properties

        public Vector3D Z
        {
            get => _base3D.Z;
            set => _base3D.Z = value;
        }

        public double ZX
        {
            get => _base3D.Z.X;
            set => _base3D.ZX = value;
        }

        public double ZY
        {
            get => _base3D.Z.Y;
            set => _base3D.ZY = value;
        }

        public double ZZ
        {
            get => _base3D.Z.Z;
            set => _base3D.ZZ = value;
        }

        #endregion

        private ScatterGraphViewModel? _scatterGraph;
        public ScatterGraphViewModel? ScatterGraph
        {
            get => _scatterGraph;
            set
            {
                if (SetValue(ref _scatterGraph, value))
                {
                    OnPropertyChanged(nameof(BelongsToAGraph));
                    if (_part != null)
                    {
                        Part = null;
                    }
                }
            }
        }

        private PartViewModelBase? _part;
        public PartViewModelBase? Part
        {
            get => _part;
            set
            {
                if (SetValue(ref _part, value))
                {
                    OnPropertyChanged(nameof(Part));
                    if (_scatterGraph != null)
                    {
                        ScatterGraph = null;
                    }
                }
            }
        }

        public bool BelongsToAGraph => _scatterGraph != null;
        public bool BelongsToAPart => _part != null;

        private readonly Model3DGroup _model;
        public Model3D Model => _model;

        public ColorViewModel Color
        {
            get => throw new InvalidOperationException("Can't access to Color");
            set => throw new InvalidOperationException("Can't access to Color");
        }

        private readonly Material?[] _oldModelMaterials;

        private byte _oldOpacity;
        private byte _opacity;
        public byte Opacity
        {
            get => _opacity;
            set
            {
                if (SetValue(ref _opacity, value))
                {
                    if (_opacity == 0 && !_isHidden)
                    {
                        IsHidden = true;
                    }
                    else if (_opacity != 0 && _isHidden)
                    {
                        IsHidden = false;
                    }
                    else
                    {
                        if (!_isHidden)
                        {
                            UpdateModelMaterial();
                        }
                    }
                }
            }
        }

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
                        for (int i = 0; i < _model.Children.Count; i++)
                        {
                            GeometryModel3D model = (GeometryModel3D)_model.Children[i];
                            _oldModelMaterials[i] = model.Material;
                            model.Material = null;
                            model.BackMaterial = null;
                        }
                        _oldOpacity = _opacity;
                        Opacity = 0;
                    }
                    else
                    {
                        if (_oldOpacity == 0)
                        {
                            Opacity = 255;
                        }
                        else
                        {
                            _opacity = _oldOpacity;
                            for (int i = 0; i < _model.Children.Count; i++)
                            {
                                GeometryModel3D model = (GeometryModel3D)_model.Children[i];
                                model.Material = _oldModelMaterials[i];
                                model.BackMaterial = model.Material;
                            }
                            OnPropertyChanged(nameof(Opacity));
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

        public bool IsRotating => !(_base3D is null) && _base3D.IsRotating;

        public bool IsXNormalized => _base3D != null && _base3D.X.Length.Clamp(1.0) == 1.0;

        public bool IsYNormalized => _base3D != null && _base3D.Y.Length.Clamp(1.0) == 1.0;

        public bool IsZNormalized => _base3D != null && _base3D.Z.Length.Clamp(1.0) == 1.0;

        public double DistFromOrigin => _base3D is null ? double.NaN : Math.Sqrt(_base3D.OX * _base3D.OX + _base3D.OY * _base3D.OY + _base3D.OZ * _base3D.OZ);

        private bool _canEditName;
        public bool CanEditName
        {
            get => _canEditName;
            set => SetValue(ref _canEditName, value);
        }

        private bool _canTranslate;
        public bool CanTranslate
        {
            get => _canTranslate;
            set => SetValue(ref _canTranslate, value);
        }

        private bool _canRotate;
        public bool CanRotate
        {
            get => _canRotate;
            set
            {
                if (SetValue(ref _canRotate, value))
                {
                    EndRotate();
                }
            }
        }

        private bool _canReorient;
        public bool CanReorient
        {
            get => _canReorient;
            set => SetValue(ref _canReorient, value);
        }

        private bool _canFlip;
        public bool CanFlip
        {
            get => _canFlip;
            set => SetValue(ref _canFlip, value);
        }

        private static uint _instanceCreated = 0;

        public Base3DViewModel(Base3D base3D, double scaleFactor = 1.0, double axisScaleFactor = 1.0, RenderQuality renderQuality = RenderQuality.High)
        {
            _base3D = base3D;
            _scaleFactor = scaleFactor;
            _axisScaleFactor = axisScaleFactor;
            _name = $"Base";
            _opacity = 255;
            _isHidden = false;
            _oldModelMaterials = new Material[3];
            _renderQuality = renderQuality;
            _isSelected = false;
            _isMouseOver = false;
            _canEditName = true;
            _canTranslate = true;
            _canRotate = true;
            _canReorient = true;
            _canFlip = true;
            _model = Helper3D.BuildBaseModel(GetBaseScalled(), Brushes.Red, Brushes.Green, Brushes.Blue, 0.1 * _axisScaleFactor, _renderQuality);

            _base3D.OriginChanged += Base3D_OriginChanged;
            _base3D.XChanged += Base3D_XChanged;
            _base3D.YChanged += Base3D_YChanged;
            _base3D.ZChanged += Base3D_ZChanged;
        }

        public static Base3DViewModel CreateCountedInstance(Base3D base3D, double scaleFactor = 1.0, double axisScaleFactor = 1.0, RenderQuality renderQuality = RenderQuality.High)
        {
            _instanceCreated++;
            return new Base3DViewModel(base3D, scaleFactor, axisScaleFactor, renderQuality)
            {
                _name = $"Base {_instanceCreated}"
            };
        }

        ~Base3DViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                _model.Children.Clear();
                _base3D.OriginChanged -= Base3D_OriginChanged;
                _base3D.XChanged -= Base3D_XChanged;
                _base3D.YChanged -= Base3D_YChanged;
                _base3D.ZChanged -= Base3D_ZChanged;
                base.Dispose();
            }
        }

        private void Base3D_OriginChanged(object sender, PositionEventArgs e)
        {
            OnPropertyChanged(nameof(Origin));
            OnPropertyChanged(nameof(OX));
            OnPropertyChanged(nameof(OY));
            OnPropertyChanged(nameof(OZ));
            UpdateModelGeometry();
        }

        private void Base3D_XChanged(object sender, Vector3D e)
        {
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(XX));
            OnPropertyChanged(nameof(XY));
            OnPropertyChanged(nameof(XZ));
            OnPropertyChanged(nameof(IsXNormalized));
            UpdateModelGeometry();
        }

        private void Base3D_YChanged(object sender, Vector3D e)
        {
            OnPropertyChanged(nameof(Y));
            OnPropertyChanged(nameof(YX));
            OnPropertyChanged(nameof(YY));
            OnPropertyChanged(nameof(YZ));
            OnPropertyChanged(nameof(IsYNormalized));
            UpdateModelGeometry();
        }

        private void Base3D_ZChanged(object sender, Vector3D e)
        {
            OnPropertyChanged(nameof(Z));
            OnPropertyChanged(nameof(ZX));
            OnPropertyChanged(nameof(ZY));
            OnPropertyChanged(nameof(ZZ));
            OnPropertyChanged(nameof(IsZNormalized));
            UpdateModelGeometry();
        }

        private void OnIsHidenChanged()
        {
            IsHiddenChanged?.Invoke(this, _isHidden);
        }

        public bool IsDirect()
        {
            Vector3D z = Vector3D.CrossProduct(_base3D.X, _base3D.Y);
            return
                Math.Abs(_base3D.Z.X - z.X) < Const.ZERO_CLAMP &&
                Math.Abs(_base3D.Z.Y - z.Y) < Const.ZERO_CLAMP &&
                Math.Abs(_base3D.Z.Z - z.Z) < Const.ZERO_CLAMP;
        }

        public bool IsOrthogonal()
        {
            double xy = Vector3D.AngleBetween(_base3D.X, _base3D.Y);
            double xz = Vector3D.AngleBetween(_base3D.X, _base3D.Z);
            double yz = Vector3D.AngleBetween(_base3D.Y, _base3D.Z);
            return xy.Clamp(90.0, 10.0) == 90.0 && xz.Clamp(90.0, 10.0) == 90.0 && yz.Clamp(90.0, 10.0) == 90.0;
        }

        private Base3D GetBaseScalled()
        {
            return new Base3D(
                _base3D.Origin.Multiply(_scaleFactor),
                Vector3D.Multiply(_base3D.X, _axisScaleFactor),
                Vector3D.Multiply(_base3D.Y, _axisScaleFactor),
                Vector3D.Multiply(_base3D.Z, _axisScaleFactor));
        }

        public void Translate(Vector3D translation)
        {
            if (_canTranslate)
            {
                Translate(translation.X, translation.Y, translation.Z);
            }
        }

        public void Translate(double x = 0.0, double y = 0.0, double z = 0.0)
        {
            if (_canTranslate && (x != 0.0 || y != 0.0 || z != 0.0))
            {
                _base3D.Translate(x, y, z);
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

        public void Select(bool progateToOwner)
        {
            Select();
            if (progateToOwner && _scatterGraph != null)
            {
                _scatterGraph.Select();
            }
        }

        public void Unselect()
        {
            IsSelected = false;
        }

        /// <summary>
        /// Indicates the start of a base rotation. To call before <see cref="Rotate(Vector3D, double, bool)"/>.<br />
        /// Sets <see cref="IsRotating"/> to true and saves the current positions of the vectors for use during rotation.
        /// </summary>
        public void BeginRotate()
        {
            if (_canRotate)
            {
                _base3D.BeginRotate();
            }
        }

        /// <summary>
        /// Indicates the end of a rotation. To be called after <see cref="Rotate(Vector3D, double, bool)"/>.
        /// </summary>
        public void EndRotate()
        {
            _base3D.EndRotate();
        }

        /// <summary>
        /// Rotate the base according to a given direction and an angle.<br />
        /// <see cref="BeginRotate"/> is called automatically if <see cref="IsRotating"/> is false.
        /// </summary>
        /// <param name="rotationAxis">The direction</param>
        /// <param name="rotationAngle">The angle in degrees</param>
        /// <param name="autoCallEndRotate">Call <see cref="EndRotate"/> automatically.<br />
        /// If you have only one rotation, let it to true. Else, set it to false and don't forget to call <see cref="EndRotate"/> when all the rotations have been applied.
        /// </param>
        public void Rotate(Vector3D rotationAxis, double rotationAngle, bool autoCallEndRotate = true)
        {
            if (_canRotate)
            {
                _base3D.Rotate(rotationAxis, rotationAngle, autoCallEndRotate);
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(Z));
                UpdateModelGeometry();
            }
        }

        public void UpdateBase(Base3D base3D, bool updateOrigin = true)
        {
            if (updateOrigin)
            {
                _base3D.Origin = base3D.Origin;
                OnPropertyChanged(nameof(Origin));
            }
            _base3D.X = base3D.X;
            _base3D.Y = base3D.Y;
            _base3D.Z = base3D.Z;
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
            OnPropertyChanged(nameof(Z));
            UpdateModelGeometry();
        }

        public void UpdateOrigin(Point3D origin)
        {
            if (_canTranslate)
            {
                Origin = origin;
            }
        }

        public void UpdateModelGeometry()
        {
            Base3D base3D = GetBaseScalled();
            ((GeometryModel3D)_model.Children[0]).Geometry = Helper3D.BuildArrowGeometry(base3D.Origin, Point3D.Add(base3D.Origin, base3D.X), 0.1 * _axisScaleFactor, _renderQuality);
            ((GeometryModel3D)_model.Children[1]).Geometry = Helper3D.BuildArrowGeometry(base3D.Origin, Point3D.Add(base3D.Origin, base3D.Y), 0.1 * _axisScaleFactor, _renderQuality);
            ((GeometryModel3D)_model.Children[2]).Geometry = Helper3D.BuildArrowGeometry(base3D.Origin, Point3D.Add(base3D.Origin, base3D.Z), 0.1 * _axisScaleFactor, _renderQuality);
        }

        public void UpdateModelMaterial()
        {
            GeometryModel3D geometryModel3D;
            SolidColorBrush brush;
            // x
            geometryModel3D = (GeometryModel3D)(_model.Children[0]);
            brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_opacity, 255, 0, 0));
            geometryModel3D.Material = MaterialHelper.CreateMaterial(brush);
            geometryModel3D.BackMaterial = geometryModel3D.Material;

            // y
            geometryModel3D = (GeometryModel3D)(_model.Children[1]);
            brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_opacity, 0, 255, 0));
            geometryModel3D.Material = MaterialHelper.CreateMaterial(brush);
            geometryModel3D.BackMaterial = geometryModel3D.Material;

            // z
            geometryModel3D = (GeometryModel3D)(_model.Children[2]);
            brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(_opacity, 0, 0, 255));
            geometryModel3D.Material = MaterialHelper.CreateMaterial(brush);
            geometryModel3D.BackMaterial = geometryModel3D.Material;
        }

        public bool IsBelongingToModel(GeometryModel3D geometryModel3D)
        {
            return _model.Children.AsQueryable().Any(x => ((GeometryModel3D)x).Geometry.Equals(geometryModel3D.Geometry));
        }

        public void Flip()
        {
            if (_canFlip)
            {
                Rotate(_base3D.Y, 180);
            }
        }

        public void NormalizeX()
        {
            _base3D.NormalizeX();
        }

        public void NormalizeY()
        {
            _base3D.NormalizeY();
        }

        public void NormalizeZ()
        {
            _base3D.NormalizeZ();
        }

        public void Normalize()
        {
            _base3D.Normalize();
        }

        public void ResetAxis(Axis axis)
        {
            if (_canReorient)
            {
                _base3D.ResetAxis(axis);
            }
        }

        public void ResetAllAxis()
        {
            if (_canReorient)
            {
                _base3D.ResetAllAxis();
            }
        }

        public CameraConfiguration GetCameraConfigurationToFocus(double fov = 45.0, double distanceScaling = 1.0, double minDistance = 0.0)
        {
            return GetCameraConfigurationToFocus(new Vector3D(-1.0, -1.0, -1.0), fov, distanceScaling, minDistance);
        }

        public CameraConfiguration GetCameraConfigurationToFocus(Vector3D direction, double fov = 45.0, double distanceScaling = 1.0, double minDistance = 0.0)
        {
            Rect3D bounds = _model.Bounds;
            Point3D target = _base3D.Origin.Multiply(_scaleFactor);
            return CameraHelper.GetCameraConfigurationToFocus(bounds, target, direction, fov, distanceScaling, minDistance);
        }
    }
}

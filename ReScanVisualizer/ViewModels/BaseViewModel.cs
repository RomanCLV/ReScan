using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using ReScanVisualizer.Models;

namespace ReScanVisualizer.ViewModels
{
    public class BaseViewModel : ViewModelBase
    {
        private readonly Base3DViewModel _base;

        public Base3DViewModel Base => _base;

        #region Translate

        private double _translateX;
        public double TranslateX
        {
            get => _translateX;
            set => SetValue(ref _translateX, value);
        }

        private double _translateY;
        public double TranslateY
        {
            get => _translateY;
            set => SetValue(ref _translateY, value);
        }

        private double _translateZ;
        public double TranslateZ
        {
            get => _translateZ;
            set => SetValue(ref _translateZ, value);
        }

        #endregion

        #region Reorient

        private bool _isUpdatingAngles;
        private bool _isUpdatingCartesian;

        public List<Axis> AllReorientAxis { get; private set; }

        private Axis _reorientAxis;
        public Axis ReorientAxis
        {
            get => _reorientAxis;
            set
            {
                if (SetValue(ref _reorientAxis, value))
                {
                    ReorientBase();
                }
            }
        }

        private double _reorientX;
        public double ReorientX
        {
            get => _reorientX;
            private set
            {
                if (SetValue(ref _reorientX, value))
                {
                    UpdateAnglesFromCartesian();
                    ReorientBase();
                }
            }
        }

        private double _reorientY;
        public double ReorientY
        {
            get => _reorientY;
            private set
            {
                if (SetValue(ref _reorientY, value))
                {
                    UpdateAnglesFromCartesian();
                    ReorientBase();
                }
            }
        }

        private double _reorientZ;
        public double ReorientZ
        {
            get => _reorientZ;
            private set
            {
                if (SetValue(ref _reorientZ, value))
                {
                    UpdateAnglesFromCartesian();
                    ReorientBase();
                }
            }
        }

        private double _reorientTheta;
        public double ReorientTheta
        {
            get => _reorientTheta;
            set
            {
                if (value > 180.0 || value < -180.0)
                {
                    value = 180.0;
                }
                if (SetValue(ref _reorientTheta, value))
                {
                    UpdateCartesianFromAngles();
                    ReorientBase();
                }
            }
        }

        private double _reorientPhi;
        public double ReorientPhi
        {
            get => _reorientPhi;
            set
            {
                if (value > 180.0 || value < -180.0)
                {
                    value = 180.0;
                }
                if (SetValue(ref _reorientPhi, value))
                {
                    UpdateCartesianFromAngles();
                    ReorientBase();
                }
            }
        }

        #endregion

        #region Rotate

        private bool _rotateXYZEnabled;
        public bool RotateXYZEnabled
        {
            get => _rotateXYZEnabled;
            set => SetValue(ref _rotateXYZEnabled, value);
        }

        public List<RotationAxis> AllRotationAxis { get; private set; }

        private RotationAxis _rotationAxis;
        public RotationAxis RotationAxis
        {
            get => _rotationAxis;
            set
            {
                if (SetValue(ref _rotationAxis, value))
                {
                    EndRotateBase();
                    if (_rotationAxis is RotationAxis.X)
                    {
                        RotationX = _base.X.X;
                        RotationY = _base.X.Y;
                        RotationZ = _base.X.Z;
                    }
                    else if (_rotationAxis is RotationAxis.Y)
                    {
                        RotationX = _base.Y.X;
                        RotationY = _base.Y.Y;
                        RotationZ = _base.Y.Z;
                    }
                    else if (_rotationAxis is RotationAxis.Z)
                    {
                        RotationX = _base.Z.X;
                        RotationY = _base.Z.Y;
                        RotationZ = _base.Z.Z;
                    }
                    _rotationAngle = 0.0;
                    OnPropertyChanged(nameof(RotationAngle));
                    RotateXYZEnabled = _rotationAxis is RotationAxis.Personalized;
                }
            }
        }

        private double _rotationAngle;
        public double RotationAngle
        {
            get => _rotationAngle;
            set
            {
                if (SetValue(ref _rotationAngle, value))
                {
                    RotateBase();
                }
            }
        }

        private double _rotationX;
        public double RotationX
        {
            get => _rotationX;
            set => SetValue(ref _rotationX, value);
        }

        private double _rotationY;
        public double RotationY
        {
            get => _rotationY;
            set => SetValue(ref _rotationY, value);
        }

        private double _rotationZ;
        public double RotationZ
        {
            get => _rotationZ;
            set => SetValue(ref _rotationZ, value);
        }

        #endregion

        public bool CanEditName => Base != null && Base.CanEditName;

        public bool CanTranslate => Base != null && Base.CanTranslate;

        public bool CanRotate => Base != null && Base.CanRotate;

        public bool CanReorient => Base != null && Base.CanReorient;

        public bool CanFlip => Base != null && Base.CanFlip;

        public BaseViewModel(Base3DViewModel base3DViewModel)
        {
            _base = base3DViewModel;
            _base.PropertyChanged += Base_PropertyChanged;

            _isUpdatingAngles = false;
            _isUpdatingCartesian = false;

            // translate
            _translateX = 0.0;
            _translateY = 0.0;
            _translateZ = 0.0;

            // reorient
            AllReorientAxis = Tools.GetAxisList();
            _reorientAxis = Axis.X;
            _reorientX = _base.X.X;
            _reorientY = _base.X.Y;
            _reorientZ = _base.X.Z;
            double r = Math.Sqrt(_reorientX * _reorientX + _reorientY * _reorientY + _reorientZ * _reorientZ);
            _reorientTheta = Tools.RadianToDegree(Math.Acos(_reorientZ / r));
            _reorientPhi = Tools.RadianToDegree(Math.Atan(_reorientY / _reorientX)); // TODO : ajouter verif pour div par 0

            // rotate
            AllRotationAxis = Tools.GetRotationAxesList();
            if (_base.BelongsToAGraph || _base.BelongsToAPart)
            {
                EnableAxisRotationOnlyOn(Axis.Z);
            }
            _rotateXYZEnabled = _rotationAxis is RotationAxis.Personalized;
            _rotationY = 0.0;
        }

        ~BaseViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                _base.PropertyChanged -= Base_PropertyChanged;
                EndRotateBase();
                base.Dispose();
            }
        }

        private void Base_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Base3DViewModel.CanEditName))
            {
                OnPropertyChanged(nameof(CanEditName));
            }
            else if (e.PropertyName == nameof(Base3DViewModel.CanTranslate))
            {
                OnPropertyChanged(nameof(CanTranslate));
            }
            else if (e.PropertyName == nameof(Base3DViewModel.CanRotate))
            {
                OnPropertyChanged(nameof(CanRotate));
            }
            else if (e.PropertyName == nameof(Base3DViewModel.CanReorient))
            {
                OnPropertyChanged(nameof(CanReorient));
            }
            else if (e.PropertyName == nameof(Base3DViewModel.CanFlip))
            {
                OnPropertyChanged(nameof(CanFlip));
            }
        }

        public void EnableAxisRotationOnlyOn(Axis axis)
        {
            AllRotationAxis.Clear();
            switch (axis)
            {
                case Axis.X:
                    AllRotationAxis.Add(RotationAxis.X);
                    break;
                case Axis.Y:
                    AllRotationAxis.Add(RotationAxis.Y);
                    break;
                case Axis.Z:
                    AllRotationAxis.Add(RotationAxis.Z);
                    break;
            }
            if (!AllRotationAxis.Contains(_rotationAxis))
            {
                RotationAxis = AllRotationAxis[0];
            }
            OnPropertyChanged(nameof(AllRotationAxis));
            RotateXYZEnabled = _rotationAxis is RotationAxis.Personalized;
            _base.CanRotate = true;
        }

        public void EnableAllAxisRotation()
        {
            AllRotationAxis = Tools.GetRotationAxesList();
            OnPropertyChanged(nameof(AllRotationAxis));
            _base.CanRotate = true;
        }

        public void UpdateRotationXYZFromBase()
        {
            switch (_rotationAxis)
            {
                case RotationAxis.X:
                    RotationX = _base.X.X;
                    RotationY = _base.X.Y;
                    RotationZ = _base.X.Z;
                    break;

                case RotationAxis.Y:
                    RotationX = _base.Y.X;
                    RotationY = _base.Y.Y;
                    RotationZ = _base.Y.Z;
                    break;

                case RotationAxis.Z:
                    RotationX = _base.Z.X;
                    RotationY = _base.Z.Y;
                    RotationZ = _base.Z.Z;
                    break;
            }
        }

        public void UpdateReorientCartesianFromBase()
        {
            _isUpdatingCartesian = true;

            switch (_reorientAxis)
            {
                case Axis.X:
                    ReorientX = _base.X.X;
                    ReorientY = _base.X.Y;
                    ReorientZ = _base.X.Z;
                    break;

                case Axis.Y:
                    ReorientX = _base.Y.X;
                    ReorientY = _base.Y.Y;
                    ReorientZ = _base.Y.Z;
                    break;

                case Axis.Z:
                    ReorientX = _base.Z.X;
                    ReorientY = _base.Z.Y;
                    ReorientZ = _base.Z.Z;
                    break;
            }

            _isUpdatingCartesian = false;
        }

        public void UpdateAnglesFromCartesian()
        {
            if (!_isUpdatingCartesian)
            {
                _isUpdatingAngles = true;
                double r = Math.Sqrt(_reorientX * _reorientX + _reorientY * _reorientY + _reorientZ * _reorientZ);
                ReorientTheta = Tools.RadianToDegree(Math.Acos(_reorientZ / r));
                //ReorientPhi = _reorientX == 0 ? 0.0 : Tools.RadianToDegree(Math.Atan(_reorientY / _reorientX));
                ReorientPhi = Tools.RadianToDegree(Math.Atan(_reorientY / _reorientX));
                _isUpdatingAngles = false;
            }
        }

        public void UpdateCartesianFromAngles()
        {
            if (!_isUpdatingAngles)
            {
                _isUpdatingCartesian = true;
                double p = Tools.DegreeToRadian(_reorientPhi);
                double t = Tools.DegreeToRadian(_reorientTheta);

                double cosp = Tools.Cos(p);
                double sinp = Tools.Sin(p);
                double cost = Tools.Cos(t);
                double sint = Tools.Sin(t);

                ReorientX = cosp * sint;
                ReorientY = sinp * sint;
                ReorientZ = cost;
                _isUpdatingCartesian = false;
            }
        }

        public void ApplyTranslation()
        {
            if (CanTranslate)
            {
                _base.Translate(_translateX, _translateY, _translateZ);
                TranslateX = 0.0;
                TranslateY = 0.0;
                TranslateZ = 0.0;
            }
        }

        public void ApplyMoveTo()
        {
            if (CanTranslate)
            {
                _translateX -= Base.Origin.X;
                _translateY -= Base.Origin.Y;
                _translateZ -= Base.Origin.Z;
                ApplyTranslation();
            }
        }

        private void RotateBase()
        {
            if (CanRotate)
            {
                _base.Rotate(new Vector3D(_rotationX, _rotationY, _rotationZ), _rotationAngle, false);
            }
        }

        public void EndRotateBase()
        {
            _base.EndRotate();
            _rotationAngle = 0.0;
            OnPropertyChanged(nameof(RotationAngle));
        }

        private void ReorientBase()
        {
            if (!_isUpdatingAngles && !_isUpdatingCartesian && CanReorient)
            {
                Base3D orientedBase = Tools.ComputeOrientedBase(new Vector3D(_reorientX, _reorientY, _reorientZ), _reorientAxis);
                _base.UpdateBase(orientedBase, false);
            }
        }

        public void Flip()
        {
            if (CanFlip)
            {
                _base.Flip();
                UpdateReorientCartesianFromBase();
                UpdateAnglesFromCartesian();
            }
        }

        public void ResetAxis(Axis axis)
        {
            if (CanReorient)
            {
                _base.ResetAxis(axis);
                UpdateReorientCartesianFromBase();
                UpdateAnglesFromCartesian();
            }
        }

        public void ResetAllAxis()
        {
            if (CanReorient)
            {
                _base.ResetAllAxis();
                UpdateReorientCartesianFromBase();
                UpdateAnglesFromCartesian();
            }
        }

        public void RotateNDegree(double degree)
        {
            if (CanRotate)
            {
                _base.Rotate(new Vector3D(_rotationX, _rotationY, _rotationZ), degree);
            }
        }
    }
}

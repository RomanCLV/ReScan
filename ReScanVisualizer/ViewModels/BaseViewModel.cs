using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.ViewModels
{
    public class BaseViewModel : ViewModelBase
    {
        public Base3DViewModel Base { get; private set; }

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

        public List<Axis> AllReorientAxis { get; private set; }

        private Axis _reorientAxis;
        public Axis ReorientAxis
        {
            get => _reorientAxis;
            set
            {
                if (SetValue(ref _reorientAxis, value))
                {
                    RotateBase();
                }
            }
        }

        private double _reorientX;
        public double ReorientX
        {
            get => _reorientX;
            private set => SetValue(ref _reorientX, value);
        }

        private double _reorientY;
        public double ReorientY
        {
            get => _reorientY;
            private set => SetValue(ref _reorientY, value);

        }

        private double _reorientZ;
        public double ReorientZ
        {
            get => _reorientZ;
            private set => SetValue(ref _reorientZ, value);
        }

        private int _reorientTheta;
        public int ReorientTheta
        {
            get => _reorientTheta;
            set
            {
                if (value > 180 || value < -180)
                {
                    value = 180;
                }
                if (SetValue(ref _reorientTheta, value))
                {
                    UpdateCartesianFromAngles();
                }
            }
        }

        private int _reorientPhi;
        public int ReorientPhi
        {
            get => _reorientPhi;
            set
            {
                if (value > 180 || value < -180)
                {
                    value = 180;
                }
                if (SetValue(ref _reorientPhi, value))
                {
                    UpdateCartesianFromAngles();
                }
            }
        }

        #endregion

        #region Rotate

        public List<RotationAxis> AllRotationAxis { get; private set; }

        private RotationAxis _rotationAxis;
        public RotationAxis RotationAxis
        {
            get => _rotationAxis;
            set
            {
                if (SetValue(ref _rotationAxis, value))
                {
                    RotateBase();
                }
            }
        }

        private double _rotationDirectionX;
        public double RotationDirectionX
        {
            get => _rotationDirectionX;
            set
            {
                if (SetValue(ref _rotationDirectionX, value))
                {
                    UpdateRotationAxisSelection();
                }
            }
        }

        private double _rotationDirectionY;
        public double RotationDirectionY
        {
            get => _rotationDirectionY;
            set
            {
                if (SetValue(ref _rotationDirectionY, value))
                {
                    UpdateRotationAxisSelection();
                }
            }
        }

        private double _rotationDirectionZ;
        public double RotationDirectionZ
        {
            get => _rotationDirectionZ;
            set
            {
                if (SetValue(ref _rotationDirectionZ, value))
                {
                    UpdateRotationAxisSelection();
                }
            }
        }

        #endregion

        public bool BelongsToAGraph => !(Base is null) && Base.BelongsToAGraph;

        public BaseViewModel(Base3DViewModel base3DViewModel)
        {
            _translateX = 0.0;
            _translateY = 0.0;
            _translateZ = 0.0;

            // TODO: set next values according to base3DViewModel
            AllReorientAxis = Tools.GetAxisList();
            _reorientAxis = Axis.X;

            _reorientX = 1.0;
            _reorientY = 0.0;
            _reorientZ = 0.0;
            _reorientTheta = 90;
            _reorientPhi = 0;
            // end todo

            AllRotationAxis = Tools.GetRotationAxesList();
            _rotationAxis = RotationAxis.X;
            _rotationDirectionX = 1.0;
            _rotationDirectionY = 0.0;
            _rotationDirectionZ = 0.0;

            Base = base3DViewModel;
        }

        private void UpdateCartesianFromAngles()
        {
            double p = Tools.DegreeToRadian(_reorientPhi);
            double t = Tools.DegreeToRadian(_reorientTheta);

            double cosp = Tools.Cos(p);
            double sinp = Tools.Sin(p);
            double cost = Tools.Cos(t);
            double sint = Tools.Sin(t);

            ReorientX = cosp * sint;
            ReorientY = sinp * sint;
            ReorientZ = cost;

            RotateBase();
        }

        private void UpdateRotationAxisSelection()
        {

        }

        public void ApplyTranslation()
        {
            Base.Translate(_translateX, _translateY, _translateZ);
            TranslateX = 0.0;
            TranslateY = 0.0;
            TranslateZ = 0.0;
        }

        public void ApplyMoveTo()
        {
            _translateX -= Base.Origin.X;
            _translateY -= Base.Origin.Y;
            _translateZ -= Base.Origin.Z;
            ApplyTranslation();
        }

        private void RotateBase()
        {
            Base3D orientedBase = Tools.ComputeOrientedBase(new Vector3D(_reorientX, _reorientY, _reorientZ), _reorientAxis);
            Base.UpdateBase(orientedBase, false);
        }
    }
}

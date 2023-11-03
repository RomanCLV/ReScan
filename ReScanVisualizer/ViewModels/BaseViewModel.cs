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
        public List<Axis> AllAxis { get; private set; }

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

        #region Rotate

        private double _x;
        public double X
        {
            get => _x;
            private set => SetValue(ref _x, value);
        }

        private double _y;
        public double Y
        {
            get => _y;
            private set => SetValue(ref _y, value);

        }

        private double _z;
        public double Z
        {
            get => _z;
            private set => SetValue(ref _z, value);
        }

        private int _theta;
        public int Theta
        {
            get => _theta;
            set
            {
                if (value > 180 || value < -180)
                {
                    value = 180;
                }
                if (SetValue(ref _theta, value))
                {
                    UpdateCartesianFromAngles();
                }
            }
        }

        private int _phi;
        public int Phi
        {
            get => _phi;
            set
            {
                if (value > 180 || value < -180)
                {
                    value = 180;
                }
                if (SetValue(ref _phi, value))
                {
                    UpdateCartesianFromAngles();
                }
            }
        }

        private Axis _axis;
        public Axis Axis
        {
            get => _axis;
            set
            {
                if (SetValue(ref _axis, value))
                {
                    RotateBase();
                }
            }
        }

        #endregion

        public BaseViewModel() : this(new Base3DViewModel())
        {
        }

        public BaseViewModel(Base3DViewModel base3DViewModel)
        {
            _translateX = 0.0;
            _translateY = 0.0;
            _translateZ = 0.0;
            _x = 1.0;
            _y = 0.0;
            _z = 0.0;
            _theta = 90;
            _phi = 0;

            Base = base3DViewModel;
            AllAxis = new List<Axis>();
            LoadAxisList();
            _axis = AllAxis[0];
        }

        private void LoadAxisList()
        {
            AllAxis.Clear();
            foreach (object item in Enum.GetValues(typeof(Axis)))
            {
                AllAxis.Add((Axis)item);
            }
        }

        private void UpdateCartesianFromAngles()
        {
            double p = Tools.DegreeToRadian(_phi);
            double t = Tools.DegreeToRadian(_theta);

            double cosp = Math.Cos(p).Clamp().Clamp(1).Clamp(-1);
            double sinp = Math.Sin(p).Clamp().Clamp(1).Clamp(-1);
            double cost = Math.Cos(t).Clamp().Clamp(1).Clamp(-1);
            double sint = Math.Sin(t).Clamp().Clamp(1).Clamp(-1);

            X = cosp * sint;
            Y = sinp * sint;
            Z = cost;

            RotateBase();
        }

        public void ApplyTranslation()
        {
            Base.Translate(_translateX, _translateY, _translateZ);
            TranslateX = 0.0;
            TranslateY = 0.0;
            TranslateZ = 0.0;
        }

        private void RotateBase()
        {
            Base3D orientedBase = Tools.ComputeOrientedBase(new Vector3D(_x, _y, _z), _axis);
            Base.SetBaseVectorsFrom(orientedBase);
        }
    }
}

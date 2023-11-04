using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class Base3DViewModel : ViewModelBase, I3DElement
    {
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
                    // TODO: rebuild all
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value);
        }

        private Base3D GetBaseScalled()
        {
            Point3D origin = _base3D.Origin.Multiply(_scaleFactor);
            Vector3D x = Vector3D.Multiply(_base3D.X, _scaleFactor);
            Vector3D y = Vector3D.Multiply(_base3D.Y, _scaleFactor);
            Vector3D z = Vector3D.Multiply(_base3D.Z, _scaleFactor);
            return new Base3D(origin, x, y, z);
        }

        public Point3D Origin
        {
            get => _base3D.Origin;
            set
            {
                if (value != _base3D.Origin)
                {
                    _base3D.Origin = value;
                    OnPropertyChanged(nameof(Origin));
                    UpdateModelGeometry();
                }
            }
        }

        public Vector3D X
        {
            get => _base3D.X;
            set
            {
                if (value != _base3D.X)
                {
                    _base3D.X = value;
                    OnPropertyChanged(nameof(X));
                    UpdateModelGeometry();
                }
            }
        }

        public Vector3D Y
        {
            get => _base3D.Y;
            set
            {
                if (value != _base3D.Y)
                {
                    _base3D.Y = value;
                    OnPropertyChanged(nameof(Y));
                    UpdateModelGeometry();
                }
            }
        }

        public Vector3D Z
        {
            get => _base3D.Z;
            set
            {
                if (value != _base3D.Z)
                {
                    _base3D.Z = value;
                    OnPropertyChanged(nameof(Z));
                    UpdateModelGeometry();
                }
            }
        }

        private readonly Model3DGroup _model;
        public Model3D Model => _model;

        public ColorViewModel Color
        {
            get => throw new InvalidOperationException("Can't access to Color"); 
            set => throw new InvalidOperationException("Can't access to Color"); 
        }

        private bool _isHidenChanging;

        private bool _isHiden;
        public bool IsHidden
        {
            get => _isHiden;
            set
            {
                if (SetValue(ref _isHiden, value))
                {
                    _isHidenChanging = true;
                    //if (_isHiden)
                    //{
                    //    UpdateOldOpacity();
                    //    Color.A = 0;
                    //}
                    //else
                    //{
                    //    Color.A = _oldOpacity;
                    //}
                    //_isHidenChanging = false;
                    //OnIsHidenChanged();
                }
            }
        }



        private static uint _instanceCreated = 0;

        public event EventHandler<bool>? IsHidenChanged;

        public Base3DViewModel(double scaleFactor = 1.0) : this(new Base3D(), scaleFactor)
        {
        }

        public Base3DViewModel(Base3D base3D, double scaleFactor)
        {
            _base3D = base3D;
            _scaleFactor = scaleFactor;
            _name = $"Base";
            _isHidenChanging = false;
            _isHiden = false;
            _model = Helper3D.Helper3D.BuildBaseModel(GetBaseScalled(), Brushes.Red, Brushes.Green, Brushes.Blue, 0.1 * _scaleFactor);
        }

        public static Base3DViewModel CreateCountedInstance()
        {
            return CreateCountedInstance(new Base3D());
        }

        public static Base3DViewModel CreateCountedInstance(Base3D base3D, double scaleFactor = 1.0)
        {
            _instanceCreated++;
            return new Base3DViewModel(base3D, scaleFactor)
            {
                Name = $"Base {_instanceCreated}"
            };
        }

        public void Translate(Vector3D translation)
        {
            Translate(translation.X, translation.Y, translation.Z);
        }

        public void Translate(double x = 0.0, double y = 0.0, double z = 0.0)
        {
            if (x != 0.0 || y != 0.0 || z != 0.0)
            {
                _base3D.Translate(x, y, z);
                OnPropertyChanged(nameof(Origin));
                UpdateModelGeometry();
            }
        }

        public void SetBaseVectorsFrom(Base3D base3D)
        {
            _base3D.X = base3D.X;
            _base3D.Y = base3D.Y;
            _base3D.Z = base3D.Z;
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
            OnPropertyChanged(nameof(Z));
            UpdateModelGeometry();
        }

        public void InverseIsHidden()
        {
            IsHidden = !_isHiden;
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public void UpdateModelGeometry()
        {
            Base3D base3D = GetBaseScalled();
            ((GeometryModel3D)_model.Children[0]).Geometry = Helper3D.Helper3D.BuildArrowGeometry(base3D.Origin, Point3D.Add(base3D.Origin, base3D.X), 0.1 * _scaleFactor);
            ((GeometryModel3D)_model.Children[1]).Geometry = Helper3D.Helper3D.BuildArrowGeometry(base3D.Origin, Point3D.Add(base3D.Origin, base3D.Y), 0.1 * _scaleFactor);
            ((GeometryModel3D)_model.Children[2]).Geometry = Helper3D.Helper3D.BuildArrowGeometry(base3D.Origin, Point3D.Add(base3D.Origin, base3D.Z), 0.1 * _scaleFactor);
        }

        public void UpdateModelMaterial()
        {
        }
    }
}

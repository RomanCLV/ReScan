using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.ViewModels
{
    public class Base3DViewModel : ViewModelBase
    {
        private readonly Base3D _base3D;

        //public Base3D Base3D => _base3D;

        private string _name;
        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value);
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
                    Build();
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
                    Build();
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
                    Build();
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
                    Build();
                }
            }
        }

        public Model3DGroup Model { get; private set; }

        private static uint _instanceCreated = 0;

        public Base3DViewModel() : this(new Base3D())
        {
        }

        public Base3DViewModel(Base3D base3D)
        {
            _base3D = base3D;
            Name = $"Base";
            Model = Helper3D.Helper3D.BuildBaseModel(_base3D, Brushes.Red, Brushes.Green, Brushes.Blue);
        }

        public static Base3DViewModel CreateCountedInstance()
        {
            return CreateCountedInstance(new Base3D());
        }

        public static Base3DViewModel CreateCountedInstance(Base3D base3D)
        {
            _instanceCreated++;
            return new Base3DViewModel(base3D)
            {
                Name = $"Base {_instanceCreated}"
            };
        }

        private void Build()
        {
            ((GeometryModel3D)Model.Children[0]).Geometry = Helper3D.Helper3D.BuildArrowGeometry(Origin, Point3D.Add(Origin, X), 0.1);
            ((GeometryModel3D)Model.Children[1]).Geometry = Helper3D.Helper3D.BuildArrowGeometry(Origin, Point3D.Add(Origin, Y), 0.1);
            ((GeometryModel3D)Model.Children[2]).Geometry = Helper3D.Helper3D.BuildArrowGeometry(Origin, Point3D.Add(Origin, Z), 0.1);
        }
    }
}

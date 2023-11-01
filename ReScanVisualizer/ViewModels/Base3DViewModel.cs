﻿using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.ViewModels
{
    public class Base3DViewModel : ViewModelBase
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

        public Base3DViewModel(double scaleFactor = 1.0) : this(new Base3D(), scaleFactor)
        {
        }

        public Base3DViewModel(Base3D base3D, double scaleFactor)
        {
            _base3D = base3D;
            _scaleFactor = scaleFactor;
            Name = $"Base";
            Model = Helper3D.Helper3D.BuildBaseModel(GetBaseScalled(), Brushes.Red, Brushes.Green, Brushes.Blue, 0.1 * _scaleFactor);
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

        public void SetBaseVectorsFrom(Base3D base3D)
        {
            _base3D.X = base3D.X;
            _base3D.Y = base3D.Y;
            _base3D.Z = base3D.Z;
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
            OnPropertyChanged(nameof(Z));
            Build();
        }

        private void Build()
        {
            Base3D base3D = GetBaseScalled();
            ((GeometryModel3D)Model.Children[0]).Geometry = Helper3D.Helper3D.BuildArrowGeometry(base3D.Origin, Point3D.Add(base3D.Origin, base3D.X), 0.1 * _scaleFactor);
            ((GeometryModel3D)Model.Children[1]).Geometry = Helper3D.Helper3D.BuildArrowGeometry(base3D.Origin, Point3D.Add(base3D.Origin, base3D.Y), 0.1 * _scaleFactor);
            ((GeometryModel3D)Model.Children[2]).Geometry = Helper3D.Helper3D.BuildArrowGeometry(base3D.Origin, Point3D.Add(base3D.Origin, base3D.Z), 0.1 * _scaleFactor);
        }
    }
}

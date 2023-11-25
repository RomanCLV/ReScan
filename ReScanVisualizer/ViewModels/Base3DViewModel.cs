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

        public bool BelongsToAGraph { get; }

        private readonly Model3DGroup _model;
        public Model3D Model => _model;

        public ColorViewModel Color
        {
            get => throw new InvalidOperationException("Can't access to Color");
            set => throw new InvalidOperationException("Can't access to Color");
        }

        private readonly Material?[] _oldModelMaterials;

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
                    }
                    else
                    {
                        for (int i = 0; i < _model.Children.Count; i++)
                        {
                            GeometryModel3D model = (GeometryModel3D)_model.Children[i];
                            model.Material = _oldModelMaterials[i];
                            model.BackMaterial = model.Material;
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

        public bool IsRotating => _base3D is null ? false : _base3D.IsRotating;


        private static uint _instanceCreated = 0;

        public event EventHandler<bool>? IsHiddenChanged;

        public Base3DViewModel(Base3D base3D, double scaleFactor = 1.0, double axisScaleFactor = 1.0, bool belongsToAGraph = false, RenderQuality renderQuality = RenderQuality.High)
        {
            _base3D = base3D;
            _scaleFactor = scaleFactor;
            _axisScaleFactor = axisScaleFactor;
            _name = $"Base";
            _isHidden = false;
            _oldModelMaterials = new Material[3];
            BelongsToAGraph = belongsToAGraph;
            _renderQuality = renderQuality;
            _model = Helper3D.Helper3D.BuildBaseModel(GetBaseScalled(), Brushes.Red, Brushes.Green, Brushes.Blue, 0.1 * _axisScaleFactor, _renderQuality);
        }

        public static Base3DViewModel CreateCountedInstance(double scaleFactor = 1.0, double axisScaleFactor = 1.0, RenderQuality renderQuality = RenderQuality.High)
        {
            return CreateCountedInstance(new Base3D(), scaleFactor, axisScaleFactor, renderQuality);
        }

        public static Base3DViewModel CreateCountedInstance(Base3D base3D, double scaleFactor = 1.0, double axisScaleFactor = 1.0, RenderQuality renderQuality = RenderQuality.High)
        {
            _instanceCreated++;
            return new Base3DViewModel(base3D, scaleFactor, axisScaleFactor, false, renderQuality)
            {
                _name = $"Base {_instanceCreated}"
            };
        }

        private void OnIsHidenChanged()
        {
            IsHiddenChanged?.Invoke(this, _isHidden);
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

        /// <summary>
        /// Indicates the start of a base rotation. To call before <see cref="Rotate(Vector3D, double, bool)"/>.<br />
        /// Sets <see cref="IsRotating"/> to true and saves the current positions of the vectors for use during rotation.
        /// </summary>
        public void BeginRotate()
        {
            _base3D.BeginRotate();
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
            if (rotationAngle != 0.0)
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
            }
            _base3D.X = base3D.X;
            _base3D.Y = base3D.Y;
            _base3D.Z = base3D.Z;
            OnPropertyChanged(nameof(Origin));
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
            OnPropertyChanged(nameof(Z));
            UpdateModelGeometry();
        }

        public void UpdateModelGeometry()
        {
            Base3D base3D = GetBaseScalled();
            ((GeometryModel3D)_model.Children[0]).Geometry = Helper3D.Helper3D.BuildArrowGeometry(base3D.Origin, Point3D.Add(base3D.Origin, base3D.X), 0.1 * _axisScaleFactor, _renderQuality);
            ((GeometryModel3D)_model.Children[2]).Geometry = Helper3D.Helper3D.BuildArrowGeometry(base3D.Origin, Point3D.Add(base3D.Origin, base3D.Z), 0.1 * _axisScaleFactor, _renderQuality);
            ((GeometryModel3D)_model.Children[1]).Geometry = Helper3D.Helper3D.BuildArrowGeometry(base3D.Origin, Point3D.Add(base3D.Origin, base3D.Y), 0.1 * _axisScaleFactor, _renderQuality);
        }

        public void UpdateModelMaterial()
        {
            throw new InvalidOperationException("UpdateModelMaterial not allowed for Base3DViewModel.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels.Parts;

namespace ReScanVisualizer.ViewModels.AddPartModelViews.Builders
{
    public class RectanglePartBuilder : PartBuilderBase
    {
        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                if (value <= 0.0)
                {
                    value = 1.0;
                }
                double delta = (value - _width) / 2.0;
                if (SetValue(ref _width, value))
                {
                    UpdateFrontAndBackBase(delta);
                }
            }
        }

        private double _length;
        public double Length
        {
            get => _length;
            set
            {
                if (value <= 0.0)
                {
                    value = 1.0;
                }
                double delta = (value - _length) / 2.0;
                if (SetValue(ref _length, value))
                {
                    UpdateLeftAndRightBase(delta);
                }
            }
        }

        private double _height;
        public double Height
        {
            get => _height;
            set
            {
                if (value <= 0.0)
                {
                    value = 1.0;
                }
                double delta = (value - _height) / 2.0;
                if (SetValue(ref _height, value))
                {
                    UpdateUpAndDownBase(delta);
                }
            }
        }

        public Base3DViewModel FrontBase { get; }

        public Base3DViewModel BackBase { get; }

        public Base3DViewModel LeftBase { get; }

        public Base3DViewModel RightBase { get; }

        public Base3DViewModel UpBase { get; }

        public Base3DViewModel DownBase { get; }

        private double _frontBaseAngle;
        public double FrontBaseAngle
        {
            get => _frontBaseAngle;
            set
            {
                if (value > 180.0)
                {
                    value = 180.0;
                }
                else if (value < -180.0)
                {
                    value = -180.0;
                }
                if (SetValue(ref _frontBaseAngle, value))
                {
                    FrontBase.Rotate(FrontBase.Z, _frontBaseAngle, false);
                }
            }
        }

        private double _backBaseAngle;
        public double BackBaseAngle
        {
            get => _backBaseAngle;
            set
            {
                if (value > 180.0)
                {
                    value = 180.0;
                }
                else if (value < -180.0)
                {
                    value = -180.0;
                }
                if (SetValue(ref _backBaseAngle, value))
                {
                    BackBase.Rotate(BackBase.Z, _backBaseAngle, false);
                }
            }
        }

        private double _leftBaseAngle;
        public double LeftBaseAngle
        {
            get => _leftBaseAngle;
            set
            {
                if (value > 180.0)
                {
                    value = 180.0;
                }
                else if (value < -180.0)
                {
                    value = -180.0;
                }
                if (SetValue(ref _leftBaseAngle, value))
                {
                    LeftBase.Rotate(LeftBase.Z, _leftBaseAngle, false);
                }
            }
        }

        private double _rightBaseAngle;
        public double RightBaseAngle
        {
            get => _rightBaseAngle;
            set
            {
                if (value > 180.0)
                {
                    value = 180.0;
                }
                else if (value < -180.0)
                {
                    value = -180.0;
                }
                if (SetValue(ref _rightBaseAngle, value))
                {
                    RightBase.Rotate(RightBase.Z, _rightBaseAngle, false);
                }
            }
        }

        private double _upBaseAngle;
        public double UpBaseAngle
        {
            get => _upBaseAngle;
            set
            {
                if (value > 180.0)
                {
                    value = 180.0;
                }
                else if (value < -180.0)
                {
                    value = -180.0;
                }
                if (SetValue(ref _upBaseAngle, value))
                {
                    UpBase.Rotate(UpBase.Z, _upBaseAngle, false);
                }
            }
        }

        private double _downBaseAngle;
        public double DownBaseAngle
        {
            get => _downBaseAngle;
            set
            {
                if (value > 180.0)
                {
                    value = 180.0;
                }
                else if (value < -180.0)
                {
                    value = -180.0;
                }
                if (SetValue(ref _downBaseAngle, value))
                {
                    DownBase.Rotate(DownBase.Z, _downBaseAngle, false);
                }
            }
        }

        public RectanglePartBuilder(double width = 1.0, double length = 1.0, double height = 1.0)
        {
            if (width <= 0.0)
            {
                throw new ArgumentException(nameof(width), "width must be positive.");
            }
            if (length <= 0.0)
            {
                throw new ArgumentException(nameof(length), "length must be positive.");
            }
            if (height <= 0.0)
            {
                throw new ArgumentException(nameof(height), "height must be positive.");
            }

            _width = width; // on X
            _length = length; // on Y
            _height = height; // on Z

            FrontBase = new Base3DViewModel(new Base3D(
                new Point3D(OriginBase.OX + _width / 2.0, OriginBase.OY, OriginBase.OZ),
                new Vector3D(0.0, 1.0, 0.0),
                new Vector3D(0.0, 0.0, 1.0),
                new Vector3D(1.0, 0.0, 0.0)));

            BackBase = new Base3DViewModel(new Base3D(
                new Point3D(OriginBase.OX - _width / 2.0, OriginBase.OY, OriginBase.OZ),
                new Vector3D(0.0, -1.0, 0.0),
                new Vector3D(0.0, 0.0, 1.0),
                new Vector3D(-1.0, 0.0, 0.0)));

            LeftBase = new Base3DViewModel(new Base3D(
                new Point3D(OriginBase.OX, OriginBase.OY + _length / 2.0, OriginBase.OZ),
                new Vector3D(-1.0, 0.0, 0.0),
                new Vector3D(0.0, 0.0, 1.0),
                new Vector3D(0.0, 1.0, 0.0)));

            RightBase = new Base3DViewModel(new Base3D(
                new Point3D(OriginBase.OX, OriginBase.OY - _length / 2.0, OriginBase.OZ),
                new Vector3D(1.0, 0.0, 0.0),
                new Vector3D(0.0, 0.0, 1.0),
                new Vector3D(0.0, -1.0, 0.0)));

            UpBase = new Base3DViewModel(new Base3D(
                new Point3D(OriginBase.OX, OriginBase.OY, OriginBase.OZ + _height / 2.0),
                new Vector3D(0.0, 1.0, 0.0),
                new Vector3D(-1.0, 0.0, 0.0),
                new Vector3D(0.0, 0.0, 1.0)));

            DownBase = new Base3DViewModel(new Base3D(
                new Point3D(OriginBase.OX, OriginBase.OY, OriginBase.OZ - _height / 2.0),
                new Vector3D(0.0, -1.0, 0.0),
                new Vector3D(-1.0, 0.0, 0.0),
                new Vector3D(0.0, 0.0, -1.0)));

            FrontBase.Name = "Front base";
            BackBase.Name = "Back base";
            LeftBase.Name = "Left base";
            RightBase.Name = "Right base";
            UpBase.Name = "Up base";
            DownBase.Name = "Down base";

            _frontBaseAngle = 0.0;
            _backBaseAngle = 0.0;
            _leftBaseAngle = 0.0;
            _rightBaseAngle = 0.0;
            _upBaseAngle = 0.0;
            _downBaseAngle = 0.0;

            AddBase(FrontBase);
            AddBase(BackBase);
            AddBase(LeftBase);
            AddBase(RightBase);
            AddBase(UpBase);
            AddBase(DownBase);

            FrontBase.BeginRotate();
            BackBase.BeginRotate();
            LeftBase.BeginRotate();
            RightBase.BeginRotate();
            UpBase.BeginRotate();
            DownBase.BeginRotate();
        }

        ~RectanglePartBuilder()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                FrontBase.EndRotate();
                BackBase.EndRotate();
                LeftBase.EndRotate();
                RightBase.EndRotate();
                UpBase.EndRotate();
                DownBase.EndRotate();

                FrontBase.Dispose();
                BackBase.Dispose();
                LeftBase.Dispose();
                RightBase.Dispose();
                UpBase.Dispose(); 
                DownBase.Dispose();
                base.Dispose();
            }
        }

        private void UpdateFrontAndBackBase(double deltaWidth)
        {
            FrontBase.Translate(deltaWidth, 0, 0);
            BackBase.Translate(-deltaWidth, 0, 0);
        }

        private void UpdateLeftAndRightBase(double deltaLength)
        {
            LeftBase.Translate(0, deltaLength, 0);
            RightBase.Translate(0, -deltaLength, 0);
        }

        private void UpdateUpAndDownBase(double deltaHeight)
        {
            UpBase.Translate(0, 0, deltaHeight);
            DownBase.Translate(0, 0, -deltaHeight);
        }

        public override PartViewModelBase Build()
        {
            RectanglePartViewModel rectanglePartViewModel = new RectanglePartViewModel(OriginBase.Base3D, _width, _length, _height, ScaleFactor);
            SetCommonParameters(rectanglePartViewModel);
            rectanglePartViewModel.FrontBase.UpdateBase(FrontBase.Base3D);
            rectanglePartViewModel.BackBase.UpdateBase(BackBase.Base3D);
            rectanglePartViewModel.LeftBase.UpdateBase(LeftBase.Base3D);
            rectanglePartViewModel.RightBase.UpdateBase(RightBase.Base3D);
            rectanglePartViewModel.UpBase.UpdateBase(UpBase.Base3D);
            rectanglePartViewModel.DownBase.UpdateBase(DownBase.Base3D);

            double minDim = Math.Min(Math.Min(_width, _length), _height);
            double scale = Math.Min(1.0, minDim / 3.0);
            rectanglePartViewModel.OriginBase.AxisScaleFactor = scale;
            rectanglePartViewModel.FrontBase.AxisScaleFactor = scale;
            rectanglePartViewModel.BackBase.AxisScaleFactor = scale;
            rectanglePartViewModel.LeftBase.AxisScaleFactor = scale;
            rectanglePartViewModel.RightBase.AxisScaleFactor = scale;
            rectanglePartViewModel.UpBase.AxisScaleFactor = scale;
            rectanglePartViewModel.DownBase.AxisScaleFactor = scale;

            return rectanglePartViewModel;
        }
    }
}

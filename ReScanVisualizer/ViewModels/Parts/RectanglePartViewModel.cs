﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using ReScanVisualizer.Models;

#nullable enable

namespace ReScanVisualizer.ViewModels.Parts
{
    public class RectanglePartViewModel : PartViewModelBase
    {
        private Point3D OriginScalled => OriginBase.Origin.Multiply(ScaleFactor);

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
                    UpdateModelGeometry();
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
                    UpdateModelGeometry();
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
                    UpdateModelGeometry();
                }
            }
        }

        public Base3DViewModel FrontBase { get; }

        public Base3DViewModel BackBase { get; }

        public Base3DViewModel LeftBase { get; }

        public Base3DViewModel RightBase { get; }

        public Base3DViewModel UpBase { get; }

        public Base3DViewModel DownBase { get; }

        public RectanglePartViewModel(Base3D origin, double width = 1.0, double length = 1.0, double height = 1.0, double scaleFactor = 1.0, RenderQuality renderQuality = RenderQuality.High)
            : base(origin, scaleFactor, renderQuality)
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
            HaveDimensions = true;
            _width = width; // on X
            _length = length; // on Y
            _height = height; // on Z

            FrontBase = new Base3DViewModel(new Base3D(
                new Point3D(origin.OX + _width / 2.0, origin.OY, origin.OZ),
                new Vector3D(0.0, 1.0, 0.0),
                new Vector3D(0.0, 0.0, 1.0),
                new Vector3D(1.0, 0.0, 0.0)), scaleFactor);

            BackBase = new Base3DViewModel(new Base3D(
                new Point3D(origin.OX - _width / 2.0, origin.OY, origin.OZ),
                new Vector3D(0.0, -1.0, 0.0),
                new Vector3D(0.0, 0.0, 1.0),
                new Vector3D(-1.0, 0.0, 0.0)), scaleFactor);

            LeftBase = new Base3DViewModel(new Base3D(
                new Point3D(origin.OX, origin.OY + _length / 2.0, origin.OZ),
                new Vector3D(-1.0, 0.0, 0.0),
                new Vector3D(0.0, 0.0, 1.0),
                new Vector3D(0.0, 1.0, 0.0)), scaleFactor);

            RightBase = new Base3DViewModel(new Base3D(
                new Point3D(origin.OX, origin.OY - _length / 2.0, origin.OZ),
                new Vector3D(1.0, 0.0, 0.0),
                new Vector3D(0.0, 0.0, 1.0),
                new Vector3D(0.0, -1.0, 0.0)), scaleFactor);

            UpBase = new Base3DViewModel(new Base3D(
                new Point3D(origin.OX, origin.OY, origin.OZ + _height / 2.0),
                new Vector3D(0.0, 1.0, 0.0),
                new Vector3D(-1.0, 0.0, 0.0),
                new Vector3D(0.0, 0.0, 1.0)), scaleFactor);

            DownBase = new Base3DViewModel(new Base3D(
                new Point3D(origin.OX, origin.OY, origin.OZ - _height / 2.0),
                new Vector3D(0.0, 1.0, 0.0),
                new Vector3D(1.0, 0.0, 0.0),
                new Vector3D(0.0, 0.0, -1.0)), scaleFactor);

            FrontBase.Name = "Front base";
            BackBase.Name = "Back base";
            LeftBase.Name = "Left base";
            RightBase.Name = "Right base";
            UpBase.Name = "Up base";
            DownBase.Name = "Down base";

            FrontBase.Part = this;
            BackBase.Part = this;
            LeftBase.Part = this;
            RightBase.Part = this;
            UpBase.Part = this;
            DownBase.Part = this;

            double minDim = Math.Min(Math.Min(_width, _length), _height);
            double scale = Math.Min(1.0, minDim / 3.0);
            OriginBase.AxisScaleFactor = scale;
            FrontBase.AxisScaleFactor = scale;
            BackBase.AxisScaleFactor = scale;
            LeftBase.AxisScaleFactor = scale;
            RightBase.AxisScaleFactor = scale;
            UpBase.AxisScaleFactor = scale;
            DownBase.AxisScaleFactor = scale;

            AddBase(FrontBase);
            AddBase(BackBase);
            AddBase(LeftBase);
            AddBase(RightBase);
            AddBase(UpBase);
            AddBase(DownBase);

            PartVisualModel = Helper3D.BuildRectangleModel(OriginScalled, _width * ScaleFactor, _length * ScaleFactor, _height * ScaleFactor, Colors.LightBlue.ChangeAlpha(150));
            AddPartVisualModel();
        }

        private void UpdateFrontAndBackBase(double deltaWidth)
        {
            FrontBase.CanTranslate = true;
            BackBase.CanTranslate = true;
            FrontBase.Translate(deltaWidth, 0, 0);
            BackBase.Translate(-deltaWidth, 0, 0);
            FrontBase.CanTranslate = false;
            BackBase.CanTranslate = false;
        }

        private void UpdateLeftAndRightBase(double deltaLength)
        {
            LeftBase.CanTranslate = true;
            RightBase.CanTranslate = true;
            LeftBase.Translate(0, deltaLength, 0);
            RightBase.Translate(0, -deltaLength, 0);
            LeftBase.CanTranslate = false;
            RightBase.CanTranslate = false;
        }

        private void UpdateUpAndDownBase(double deltaHeight)
        {
            UpBase.CanTranslate = true;
            DownBase.CanTranslate = true;
            UpBase.Translate(0, 0, deltaHeight);
            DownBase.Translate(0, 0, -deltaHeight);
            UpBase.CanTranslate = false;
            DownBase.CanTranslate = false;
        }

        public override Base3D FindNeareatBase(Point3D point)
        {
            Base3D neareastBase = OriginBase.Base3D;
            double minLength = double.MaxValue;
            double currentLength;
            foreach (var item in Bases)
            {
                currentLength = item.DistFromOrigin;
                if (currentLength < minLength)
                {
                    minLength = currentLength;
                    neareastBase = item.Base3D;
                }
            }
            return neareastBase;
        }

        public override void UpdateModelGeometry()
        {
            if (PartVisualModel != null)
            {
                PartVisualModel.Geometry = Helper3D.BuildRectangleGeometry(OriginScalled, _width * ScaleFactor, _length * ScaleFactor, _height * ScaleFactor);
            }
        }
    }
}

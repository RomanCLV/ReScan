using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public class ScatterGraphPopulateRectangle2DBuilder : ScatterGraphPopulateBuilderBase
    {
        public Point3DViewModel Center { get; private set; }

        private Plan2D _plan;
        public Plan2D Plan
        {
            get => _plan;
            set
            {
                if (SetValue(ref _plan, value))
                {
                    OnPropertyChanged(nameof(Details));
                }
            }
        }

        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                if (value < MIN_WIDTH)
                {
                    value = MIN_WIDTH;
                }
                else if (value > MAX_WIDTH)
                {
                    value = MAX_WIDTH;
                }
                if (SetValue(ref _width, value))
                {
                    NumPointsWidth = (uint)_width + 1;
                    OnPropertyChanged(nameof(Details));
                }
            }
        }

        private double _height;
        public double Height
        {
            get => _height;
            set
            {
                if (value < MIN_HEIGTH)
                {
                    value = MIN_HEIGTH;
                }
                else if (value > MAX_HEIGTH)
                {
                    value = MAX_HEIGTH;
                }
                if (SetValue(ref _height, value))
                {
                    NumPointsHeight = (uint)_height + 1;
                    OnPropertyChanged(nameof(Details));
                }
            }
        }

        private uint _numPointsWidth;
        public uint NumPointsWidth
        {
            get => _numPointsWidth;
            set
            {
                if (value < 2)
                {
                    value = 2;
                }
                else if (value > 5 * _width)
                {
                    value = (uint)(5 * _width);
                }
                if (SetValue(ref _numPointsWidth, value))
                {
                    OnPropertyChanged(nameof(Details));
                }
            }
        }

        private uint _numPointsHeight;
        public uint NumPointsHeight
        {
            get => _numPointsHeight;
            set
            {
                if (value < 2)
                {
                    value = 2;
                }
                else if (value > 5 * _height)
                {
                    value = (uint)(5 * _height);
                }
                if (SetValue(ref _numPointsHeight, value))
                {
                    OnPropertyChanged(nameof(Details));
                }
            }
        }

        public override string Name => "Rectange2D builder";

        public override string Details => 
            $"Plan: {_plan}\n" +
            $"Center ({Math.Round(Center.X, 2)}, {Math.Round(Center.Y, 2)}, {Math.Round(Center.Z, 2)})\n" +
            $"Width: {Math.Round(_width, 2)}\n" +
            $"Height: {Math.Round(_height, 2)}\n" +
            $"Num points width: {NumPointsWidth}\n" +
            $"Num points height: {NumPointsHeight}";

        public ScatterGraphPopulateRectangle2DBuilder() : base(Colors.White)
        {
            Center = new Point3DViewModel
            {
                CorrectX = CorrectX,
                CorrectY = CorrectY,
                CorrectZ = CorrectZ
            };
            _plan = Plan2D.XY;
            _width = 10.0;
            _height = 10.0;
            _numPointsWidth = (uint)_width + 1;
            _numPointsHeight = (uint)_height + 1;
        }

        /// <returns>Return a <see cref="ScatterGraphBuildResult"/> using the <see cref="ScatterGraph.PopulateRectangle2D(ScatterGraph, Point3D, Plan2D, double, double, uint, uint)"/> method.</returns>
        public override ScatterGraphBuildResult Build()
        {
            Application.Current.Dispatcher.Invoke(() => State = ScatterGraphBuilderState.Working);
            ScatterGraphBuildResult scatterGraphBuildResult;
            try
            {
                ScatterGraph graph = new ScatterGraph();
                ScatterGraph.PopulateRectangle2D(graph, Center.Point, _plan, _width, _height, _numPointsWidth, _numPointsHeight);
                scatterGraphBuildResult = new ScatterGraphBuildResult(graph);
                State = ScatterGraphBuilderState.Success;
            }
            catch (Exception e)
            {
                scatterGraphBuildResult = new ScatterGraphBuildResult(e);
                State = ScatterGraphBuilderState.Error;
            }
            return scatterGraphBuildResult;
        }

        private double CorrectX(double x)
        {
            if (x < MIN_X)
            {
                x = MIN_X;
            }
            else if (x > MAX_X)
            {
                x = MAX_X;
            }
            return x;
        }

        private double CorrectY(double y)
        {
            if (y < MIN_Y)
            {
                y = MIN_Y;
            }
            else if (y > MAX_Y)
            {
                y = MAX_Y;
            }
            return y;
        }

        private double CorrectZ(double z)
        {
            if (z < MIN_Z)
            {
                z = MIN_Z;
            }
            else if (z > MAX_Z)
            {
                z = MAX_Z;
            }
            return z;
        }
    }
}

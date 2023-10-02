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
            set => SetValue(ref _plan, value);
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
                SetValue(ref _width, value);
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
                SetValue(ref _height, value);
            }
        }

        private double _density;
        public double Density
        {
            get => _density;
            set
            {
                if (value < 1.0)
                {
                    value = 1.0;
                }
                else if (value > 100.0)
                {
                    value = 100.0;
                }
                SetValue(ref _density, value);
            }
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetValue(ref _color, value);
        }

        public List<Plan2D> AllPlans { get; private set; }

        public ScatterGraphPopulateRectangle2DBuilder()
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
            _density = 50.0;
            _color = Colors.White;

            AllPlans = GeneratePlan2DList();
        }

        private List<Plan2D> GeneratePlan2DList()
        {
            List<Plan2D> plans = new List<Plan2D>();
            foreach (Plan2D plan in typeof(Plan2D).GetEnumValues())
            {
                plans.Add(plan);
            }
            return plans;
        }

        /// <summary>
        /// Build an array of ScatterGraphViewModel using the <see cref="ScatterGraph.PopulateRectangle2D(ScatterGraph, Point3D, Plan2D, double, double, double)"/> method.
        /// </summary>
        /// <returns>Return an array of one ScatterGraph</returns>
        public override ScatterGraphViewModel[] Build()
        {
            ScatterGraph graph = new ScatterGraph();
            ScatterGraph.PopulateRectangle2D(graph, Center.Point, _plan, _width, _height, _density);
            return new ScatterGraphViewModel[1] { new ScatterGraphViewModel(graph, _color) };
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

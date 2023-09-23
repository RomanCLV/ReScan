using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public class ScatterGraphPopulateRectangle2DBuilder : ScatterGraphPopulateBuilderBase
    {
        private Point3D _center;
        public Point3D Center
        {
            get => _center;
            set
            {
                if (SetValue(ref _center, value))
                {
                    OnPropertyChanged(nameof(CenterX));
                    OnPropertyChanged(nameof(CenterY));
                    OnPropertyChanged(nameof(CenterZ));
                }
            }
        }

        public double CenterX
        {
            get => _center.X;
            set
            {
                if (value < MIN_X)
                {
                    value = MIN_X;
                }
                else if (value > MAX_X)
                {
                    value = MAX_X;
                }
                SetValue(_center.X, value);
            }
        }

        public double CenterY
        {
            get => _center.Y;
            set
            {
                if (value < MIN_Y)
                {
                    value = MIN_Y;
                }
                else if (value > MAX_Y)
                {
                    value = MAX_Y;
                }
                SetValue(_center.Y, value);
            }
        }

        public double CenterZ
        {
            get => _center.Z;
            set
            {
                if (value < MIN_Z)
                {
                    value = MIN_Z;
                }
                else if (value > MAX_Z)
                {
                    value = MAX_Z;
                }
                SetValue(_center.Z, value);
            }
        }

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
            _center = new Point3D(0, 0, 0);
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
            ScatterGraph.PopulateRectangle2D(graph, _center, _plan, _width, _height, _density);
            return new ScatterGraphViewModel[1] { new ScatterGraphViewModel(graph, _color) };
        }
    }
}

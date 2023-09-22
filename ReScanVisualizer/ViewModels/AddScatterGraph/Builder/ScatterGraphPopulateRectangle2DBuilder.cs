using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public class ScatterGraphPopulateRectangle2DBuilder : ScatterGraphPopulateBuilderBase
    {
        private const double LIMITE = 10000.0;

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

        private double _centerX;
        public double CenterX
        {
            get => _centerX;
            set
            {
                if (value < -LIMITE)
                {
                    value = -LIMITE;
                }
                else if (value > LIMITE)
                {
                    value = LIMITE;
                }
                SetValue(ref _centerX, value);
            }
        }

        private double _centerY;
        public double CenterY
        {
            get => _centerY;
            set
            {
                if (value < -LIMITE)
                {
                    value = -LIMITE;
                }
                else if (value > LIMITE)
                {
                    value = LIMITE;
                }
                SetValue(ref _centerY, value);
            }
        }

        private double _centerZ;
        public double CenterZ
        {
            get => _centerZ;
            set
            {
                if (value < -LIMITE)
                {
                    value = -LIMITE;
                }
                else if (value > LIMITE)
                {
                    value = LIMITE;
                }
                SetValue(ref _centerZ, value);
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
                if (value < -LIMITE)
                {
                    value = -LIMITE;
                }
                else if (value > LIMITE)
                {
                    value = LIMITE;
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
                if (value < -LIMITE)
                {
                    value = -LIMITE;
                }
                else if (value > LIMITE)
                {
                    value = LIMITE;
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

        public ScatterGraphPopulateRectangle2DBuilder()
        {
            _center = new Point3D(0, 0, 0);
            _plan = Plan2D.XY;
            _width = 10.0;
            _height = 10.0;
            _density = 50.0;
            _color = Colors.White;
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

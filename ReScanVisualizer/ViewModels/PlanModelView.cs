using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class PlanModelView : ViewModelBase
    {
        private readonly Plan _plan;

        public double A
        {
            get => _plan.A;
            set
            {
                if (SetValue(_plan.A, value))
                {
                    BuildGeometryModel3D();
                }
            }
        }

        public double B
        {
            get => _plan.B;
            set
            {
                if (SetValue(_plan.B, value))
                {
                    BuildGeometryModel3D();
                }
            }
        }

        public double C
        {
            get => _plan.C;
            set
            {
                if (SetValue(_plan.C, value))
                {
                    BuildGeometryModel3D();
                }
            }
        }

        public double D
        {
            get => _plan.D;
            set
            {
                if (SetValue(_plan.D, value))
                {
                    BuildGeometryModel3D();
                }
            }
        }

        private double _opacity;
        public double Opacity
        {
            get => _opacity;
            set
            {
                if (SetValue(ref _opacity, value))
                {
                    BuildGeometryModel3D();
                }
            }
        }

        private Brush _brush;
        public Brush Brush
        {
            get => _brush;
            set
            {
                if (SetValue(ref _brush, value))
                {
                    BuildGeometryModel3D();
                }
            }
        }

        private GeometryModel3D? _geometryModel3D;
        public GeometryModel3D? GeometryModel3D
        {
            get => _geometryModel3D;
            set => SetValue(ref _geometryModel3D, value);
        }

        public PlanModelView() : this(new Plan(), 0.75, Brushes.White)
        { }

        public PlanModelView(Plan plan) : this(plan, 0.75, Brushes.White)
        { }

        public PlanModelView(Plan plan, double opacity, Brush brush)
        {
            _plan = plan;
            _opacity = opacity;
            _brush = brush;
        }

        private void BuildGeometryModel3D()
        {
            //GeometryModel3D = Helper3D.Helper3D.BuildSphere(_point, _radius, _brush);
        }
    }
}

using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels
{
    public class ScatterGraphViewModel : ViewModelBase
    {
        private readonly ScatterGraph _scatterGraph;

        private double _pointRadius;
        public double PointRadius
        {
            get => _pointRadius;
            set => SetValue(ref _pointRadius, value);
        }

        private Brush _brush;
        public Brush Brush
        {
            get => _brush;
            set => SetValue(ref _brush, value);
        }

        public ObservableCollection<Point3DViewModel> Points { get; private set; }

        private Point3DViewModel _barycenter;
        public Point3DViewModel Barycenter
        {
            get => _barycenter;
            set => SetValue(ref _barycenter, value);
        }

        private bool _isBarycenterVisible;
        public bool IsBarycenterVisible
        {
            get => _isBarycenterVisible;
            set => SetValue(ref _isBarycenterVisible, value);
        }

        private PlanViewModel _averagePlan;
        public PlanViewModel AveragePlan
        {
            get => _averagePlan;
            set => SetValue(ref _averagePlan, value);
        }

        private bool _isAveragePlanVisible;
        public bool IsAveragePlanVisible
        {
            get => _isAveragePlanVisible;
            set => SetValue(ref _isAveragePlanVisible, value);
        }

        public ScatterGraphViewModel() : this(new ScatterGraph(), 1.0, Brushes.White)
        {
        }

        public ScatterGraphViewModel(ScatterGraph scatterGraph) : this(scatterGraph, 1.0, Brushes.White)
        {
        }

        public ScatterGraphViewModel(ScatterGraph scatterGraph, double pointRadius, Brush brush)
        {
            _scatterGraph = scatterGraph;
            _pointRadius = pointRadius;
            _brush = brush;
            Points = new ObservableCollection<Point3DViewModel>();
            for (int i = 0; i < _scatterGraph.Count; i++)
            {
                Points.Add(new Point3DViewModel(_scatterGraph[i], _pointRadius, _brush));
            }
            _isBarycenterVisible = false;
            _isAveragePlanVisible = false;


            // TODO: à utiliser lors de la gestion Hide / Show
            //_barycenter = new Point3DModelView(_scatterGraph.ComputeBarycenter(), 2 * pointRadius, Brushes.Red);
            //Plan plan = _scatterGraph.Count >= 3 ? _scatterGraph.ComputeAveragePlan() : new Plan();
            //_averagePlan = new PlanModelView(plan, 0.75, Brushes.LightBlue);
        }
    }
}

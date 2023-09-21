using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class ScatterGraphViewModel : ViewModelBase
    {
        private readonly ScatterGraph _scatterGraph;

        private double _pointsDiameter;
        public double PointsDiameter
        {
            get => _pointsDiameter;
            set => SetValue(ref _pointsDiameter, value);
        }

        public bool IsBarycenterHiden
        {
            get => Barycenter.IsHiden;
            set
            {
                if (SetValue(Barycenter.IsHiden, value))
                {
                    if (!Barycenter.IsHiden && _hasToComputeBarycenter)
                    {
                        ComputeBarycenter();
                    }
                }
            }
        }

        public bool IsAveragePlanHiden
        {
            get => AveragePlan.IsHiden;
            set
            {
                if (SetValue(AveragePlan.IsHiden, value))
                {
                    if (!AveragePlan.IsHiden && _hasToComputeAveragePlan)
                    {
                        ComputeAveragePlan();
                    }
                }
            }
        }

        private bool _hasToComputeBarycenter;
        private bool _hasToComputeAveragePlan;

        public ObservableCollection<Point3DViewModel> Points { get; private set; }

        private Point3DViewModel _barycenter;
        public Point3DViewModel Barycenter
        {
            get => _barycenter;
            set => SetValue(ref _barycenter, value);
        }

        private PlanViewModel _averagePlan;
        public PlanViewModel AveragePlan
        {
            get => _averagePlan;
            set => SetValue(ref _averagePlan, value);
        }

        public Model3DGroup PointsModel { get; private set; }

        private Color _pointsColor;
        public Color Color
        {
            get => _pointsColor;
            set
            {
                if (SetValue(ref _pointsColor, value))
                {
                    OnPropertyChanged(nameof(PointsColorR));
                    OnPropertyChanged(nameof(PointsColorG));
                    OnPropertyChanged(nameof(PointsColorB));
                    OnPropertyChanged(nameof(PointsColorOpacity));

                    UpdatePointsColor();
                }
            }
        }

        public byte PointsColorR
        {
            get => _pointsColor.R;
            set
            {
                if (SetValue(_pointsColor.R, value))
                {
                    UpdatePointsColor();
                }
            }
        }

        public byte PointsColorG
        {
            get => _pointsColor.G;
            set
            {
                if (SetValue(_pointsColor.G, value))
                {
                    UpdatePointsColor();
                }
            }
        }

        public byte PointsColorB
        {
            get => _pointsColor.B;
            set
            {
                if (SetValue(_pointsColor.B, value))
                {
                    UpdatePointsColor();
                }
            }
        }

        public byte PointsColorOpacity
        {
            get => _pointsColor.A;
            set
            {
                if (SetValue(_pointsColor.A, value))
                {
                    UpdateOldOpacity();
                    UpdatePointsColor();
                }
            }
        }

        private byte _oldPointsOpacity;
        private bool _oldBarycenterIsHiden;
        private bool _oldAveragePlanIsHiden;

        private bool _isHiden;
        public bool IsHiden
        {
            get => _isHiden;
            set
            {
                if (SetValue(ref _isHiden, value))
                {
                    if (_isHiden)
                    {
                        UpdateOldOpacity();
                        PointsColorOpacity = 0;
                        _oldBarycenterIsHiden = _barycenter.IsHiden;
                        _oldAveragePlanIsHiden = _averagePlan.IsHiden;
                    }
                    else
                    {
                        PointsColorOpacity = _oldPointsOpacity;
                        _barycenter.IsHiden = _oldBarycenterIsHiden;
                        _averagePlan.IsHiden = _oldAveragePlanIsHiden;
                    }
                }
            }
        }

        public int ItemsCount
        {
            get => Points.Count + 2; // Points count + barycenter + averplan
        }

        public ScatterGraphViewModel() : this(new ScatterGraph(), Colors.White)
        {
        }

        public ScatterGraphViewModel(ScatterGraph scatterGraph) : this(scatterGraph, Colors.White)
        {
        }

        public ScatterGraphViewModel(ScatterGraph scatterGraph, Color color, double pointDiameter = 1.0)
        {
            _scatterGraph = scatterGraph;
            _pointsDiameter = pointDiameter;
            _pointsColor = color;
            _oldPointsOpacity = _pointsColor.A;
            _isHiden = _oldPointsOpacity == 0;

            PointsModel = new Model3DGroup();
            Points = new ObservableCollection<Point3DViewModel>();

            for (int i = 0; i < _scatterGraph.Count; i++)
            {
                Points.Add(new Point3DViewModel(_scatterGraph[i], _pointsColor, _pointsDiameter));
                PointsModel.Children.Add(Points.Last().Model);
            }
            Points.CollectionChanged += Points_CollectionChanged;

            Point3D barycenter = scatterGraph.ComputeBarycenter();
            Plan averagePlan;
            try
            {
                averagePlan = scatterGraph.ComputeAveragePlan();
            }
            catch (InvalidOperationException)
            {
                averagePlan = new Plan();
            }
            Repere3D repere3D = ScatterGraph.ComputeRepere3D(scatterGraph, barycenter, averagePlan);

            _barycenter = new Point3DViewModel(barycenter, Colors.Red);
            _barycenter.Hide();
            _barycenter.IsHidenChanged += Barycenter_IsHidenChanged;
            _oldBarycenterIsHiden = _barycenter.IsHiden;

            _averagePlan = new PlanViewModel(averagePlan, barycenter, repere3D.X, Colors.LightBlue.ChangeAlpha(191));
            _averagePlan.Hide();
            _averagePlan.IsHidenChanged += AveragePlan_IsHidenChanged;
            _oldAveragePlanIsHiden = _averagePlan.IsHiden;

            _hasToComputeBarycenter = false;
            _hasToComputeAveragePlan = false;
        }

        private void Barycenter_IsHidenChanged(object sender, bool e)
        {
            _oldBarycenterIsHiden = _barycenter.IsHiden;
        }

        private void AveragePlan_IsHidenChanged(object sender, bool e)
        {
            _oldAveragePlanIsHiden = _averagePlan.IsHiden;
        }

        public override void Dispose()
        {
            _barycenter.IsHidenChanged -= Barycenter_IsHidenChanged;
            _averagePlan.IsHidenChanged -= AveragePlan_IsHidenChanged;
            Points.CollectionChanged -= Points_CollectionChanged;
            PointsModel.Children.Clear();
            foreach (Point3DViewModel point in Points)
            {
                point.Dispose();
            }
            Barycenter.Dispose();
            AveragePlan.Dispose();
            base.Dispose();
        }

        private void Points_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _hasToComputeBarycenter = true;
            _hasToComputeAveragePlan = true;

            // TODO : gérer l'ajout / la suppression des models de PointsModel

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (!Barycenter.IsHiden)
            {
                ComputeBarycenter();
            }

            if (!AveragePlan.IsHiden)
            {
                ComputeAveragePlan();
            }

            OnPropertyChanged(nameof(ItemsCount));
        }

        private void UpdateOldOpacity()
        {
            _oldPointsOpacity = _pointsColor.A;
        }

        public void Hide()
        {
            IsHiden = true;
        }

        public void Show()
        {
            IsHiden = false;
        }

        private void ComputeBarycenter()
        {
            if (_hasToComputeBarycenter)
            {
                Barycenter.Point = _scatterGraph.ComputeBarycenter();
                _hasToComputeBarycenter = false;
            }
        }

        private void ComputeAveragePlan()
        {
            if (_hasToComputeAveragePlan)
            {
                AveragePlan.Plan = _scatterGraph.ComputeAveragePlan();
                _hasToComputeAveragePlan = false;
            }
        }

        private void UpdatePointsColor()
        {
            foreach (Point3DViewModel point in Points)
            {
                point.Color = _pointsColor;
            }
        }
    }
}

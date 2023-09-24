using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class ScatterGraphViewModel : ViewModelBase, I3DElement
    {
        public event EventHandler<bool>? IsHidenChanged;

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

        public ObservableCollection<SampleViewModel> Samples { get; private set; }

        private SampleViewModel _barycenter;
        public SampleViewModel Barycenter
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

        public Model3D Model { get; private set; }

        public ColorViewModel Color { get; set; }
        
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
                        Color.A = 0;
                        _oldBarycenterIsHiden = _barycenter.IsHiden;
                        _oldAveragePlanIsHiden = _averagePlan.IsHiden;
                        _barycenter.Hide();
                        _averagePlan.Hide();
                    }
                    else
                    {
                        Color.A = _oldPointsOpacity;
                        _barycenter.IsHiden = _oldBarycenterIsHiden;
                        _averagePlan.IsHiden = _oldAveragePlanIsHiden;
                    }
                }
            }
        }

        public int ItemsCount
        {
            get => Samples.Count + 2; // Points count + barycenter + averplan
        }

        public ScatterGraphViewModel() : this(new ScatterGraph(), Colors.White)
        {
        }

        public ScatterGraphViewModel(ScatterGraph scatterGraph) : this(scatterGraph, Colors.White)
        {
        }

        public ScatterGraphViewModel(ScatterGraph scatterGraph, Color color, double pointDiameter = 1.0)
        {
            IsDisposed = false;
            _scatterGraph = scatterGraph;
            _pointsDiameter = pointDiameter;
            Color = new ColorViewModel(color);
            _oldPointsOpacity = Color.A;
            _isHiden = _oldPointsOpacity == 0;

            Model = new Model3DGroup();
            Samples = new ObservableCollection<SampleViewModel>();

            for (int i = 0; i < _scatterGraph.Count; i++)
            {
                Samples.Add(new SampleViewModel(_scatterGraph[i], Color.Color, _pointsDiameter));
                ((Model3DGroup)Model).Children.Add(Samples.Last().Model);
            }

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

            _barycenter = new SampleViewModel(barycenter, Colors.Red);
            _barycenter.Hide();
            _oldBarycenterIsHiden = _barycenter.IsHiden;

            _averagePlan = new PlanViewModel(averagePlan, barycenter, repere3D.X, Colors.LightBlue.ChangeAlpha(191));
            _averagePlan.Hide();
            _oldAveragePlanIsHiden = _averagePlan.IsHiden;

            _hasToComputeBarycenter = false;
            _hasToComputeAveragePlan = false;

            Color.PropertyChanged += Color_PropertyChanged;
            Samples.CollectionChanged += Points_CollectionChanged;
            _barycenter.IsHidenChanged += Barycenter_IsHidenChanged;
            _averagePlan.IsHidenChanged += AveragePlan_IsHidenChanged;
        }

        ~ScatterGraphViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Color.PropertyChanged -= Color_PropertyChanged;
                _barycenter.IsHidenChanged -= Barycenter_IsHidenChanged;
                _averagePlan.IsHidenChanged -= AveragePlan_IsHidenChanged;
                Samples.CollectionChanged -= Points_CollectionChanged;
                try
                {
                    ((Model3DGroup)Model).Children.Clear();
                }
                catch (InvalidOperationException)
                { 
                }

                foreach (SampleViewModel point in Samples)
                {
                    point.Dispose();
                }
                Barycenter.Dispose();
                AveragePlan.Dispose();
                base.Dispose();
            }
        }

        private void Color_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Color.A))
            {
                UpdateOldOpacity();
            }
            UpdateModelMaterial();
        }

        private void Barycenter_IsHidenChanged(object sender, bool e)
        {
            _oldBarycenterIsHiden = _barycenter.IsHiden;
        }

        private void AveragePlan_IsHidenChanged(object sender, bool e)
        {
            _oldAveragePlanIsHiden = _averagePlan.IsHiden;
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
            _oldPointsOpacity = Color.A;
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
                Barycenter.Point.Set(_scatterGraph.ComputeBarycenter());
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

        public void UpdateModelGeometry()
        { }

        public void UpdateModelMaterial()
        {
            foreach (SampleViewModel point in Samples)
            {
                point.Color.Set(Color.Color);
            }
        }
    }
}

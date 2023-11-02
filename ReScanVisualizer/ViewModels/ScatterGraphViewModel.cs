using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
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
        private double _scaleFactor;
        public double ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Scale factor must be greater than 0.");
                }
                if (SetValue(ref _scaleFactor, value))
                {
                    // TODO: rebuild all
                }
            }
        }

        public event EventHandler<bool>? IsHidenChanged;

        private readonly ScatterGraph _scatterGraph;

        private double _pointsRadius;
        public double PointsRadius
        {
            get => _pointsRadius;
            set => SetValue(ref _pointsRadius, value);
        }

        public bool IsBarycenterHiden
        {
            get => Barycenter.IsHiden;
            set
            {
                if (Barycenter.IsHiden != value)
                {
                    Barycenter.IsHiden = value;
                    OnPropertyChanged(nameof(Barycenter.IsHiden));
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
                if (AveragePlan.IsHiden != value)
                {
                    AveragePlan.IsHiden = value;
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

        private Base3DViewModel _base3D;
        public Base3DViewModel Base3D
        {
            get => _base3D;
            set => SetValue(ref _base3D, value);
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
                    OnIsHiddenChanged();
                }
            }
        }

        public int ItemsCount
        {
            get => Samples.Count + 5; // Points count + barycenter (1) + averplan (1) + base (x, y, z) (3)
        }

        public ScatterGraphViewModel() : this(new ScatterGraph(), Colors.White)
        {
        }

        public ScatterGraphViewModel(ScatterGraph scatterGraph) : this(scatterGraph, Colors.White)
        {
        }

        /// <summary>
        /// ScatterGraphViewModel constuctor.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ScatterGraphViewModel(ScatterGraph scatterGraph, Color color, double scaleFactor = 1.0, double pointRadius = 0.25, bool hideBarycenter = false, bool hideAveragePlan = false)
        {
            if (scaleFactor <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleFactor), "Scale factor must be greater than 0.");
            }
            IsDisposed = false;
            _scaleFactor = scaleFactor;
            _scatterGraph = scatterGraph;
            _pointsRadius = pointRadius;
            Color = new ColorViewModel(color);
            _oldPointsOpacity = Color.A;
            _isHiden = _oldPointsOpacity == 0;

            Model = new Model3DGroup();
            Samples = new ObservableCollection<SampleViewModel>();

            for (int i = 0; i < _scatterGraph.Count; i++)
            {
                Samples.Add(new SampleViewModel(_scatterGraph[i], Color.Color, _scaleFactor, _pointsRadius));
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
                averagePlan = new Plan(0, 0, 1, 0);
            }
            
            if (_scatterGraph.Count < 2)
            {
                _base3D = new Base3DViewModel(new Base3D(barycenter), _scaleFactor);
            }
            else
            {
                if (_scatterGraph.ArePointsColinear())
                {
                    Vector3D x = _scatterGraph[1] - _scatterGraph[0];
                    x.Normalize();
                    Vector3D z = averagePlan.GetNormal();
                    z.Normalize();
                    Vector3D y = Vector3D.CrossProduct(z, x);
                    _base3D = new Base3DViewModel(new Base3D(barycenter, x, y, z), _scaleFactor);
            }
                else
                {
                    _base3D = new Base3DViewModel(ScatterGraph.ComputeRepere3D(barycenter, averagePlan), _scaleFactor);
                }
            }
            
            _base3D.Name = "Plan base";

            _barycenter = new SampleViewModel(barycenter, Colors.Red, _scaleFactor, _pointsRadius);
            _averagePlan = new PlanViewModel(averagePlan, barycenter, _base3D.X, Colors.LightBlue.ChangeAlpha(191), _scaleFactor, ComputeAveragePlanLength());

            _barycenter.IsHiden = hideBarycenter;
            _averagePlan.IsHiden = hideAveragePlan;

            _oldBarycenterIsHiden = _barycenter.IsHiden;
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
                if (Color != null)
                {
                    Color.PropertyChanged -= Color_PropertyChanged;
                }
                if (_barycenter != null)
                {
                    _barycenter.IsHidenChanged -= Barycenter_IsHidenChanged;
                }
                if (_averagePlan != null)
                {
                    _averagePlan.IsHidenChanged -= AveragePlan_IsHidenChanged;
                }
                if (Samples != null)
                {
                    Samples.CollectionChanged -= Points_CollectionChanged;
                }
                try
                {
                    ((Model3DGroup)Model).Children.Clear();
                }
                catch (InvalidOperationException)
                {
                }

                if (Samples != null)
                {
                    foreach (SampleViewModel point in Samples)
                    {
                        point?.Dispose();
                    }
                }
                _barycenter?.Dispose();
                _averagePlan?.Dispose();
                base.Dispose();
                IsDisposed = true;
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

        private void OnIsHiddenChanged()
        {
            IsHidenChanged?.Invoke(this, _isHiden);
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
                AveragePlan.Plan.SetABCD(_scatterGraph.ComputeAveragePlan());
                _hasToComputeAveragePlan = false;
            }
        }

        private double ComputeAveragePlanLength()
        {
            int size = _scatterGraph.Count;
            Base3D base3D = _base3D.Base3D;
            Point3D currentPoint;
            double maxDistance = 0.0;
            double currentDistance;
            Matrix3D matrix = base3D.GetRotationMatrix();
            matrix.Invert();

            for (int i = 0; i < size; i++)
            {
                currentPoint = _scatterGraph[i];

                // cancel translation of origin
                currentPoint.Offset(-base3D.Origin.X, -base3D.Origin.Y, -base3D.Origin.Z);

                // rotate by the invert matrix
                currentPoint = matrix.Transform(currentPoint);

                // projection over the plan
                currentPoint.Z = 0.0;

                currentDistance = Math.Max(Math.Abs(currentPoint.X), Math.Abs(currentPoint.Y));
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                }
            }
            return 2.0 * maxDistance;
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

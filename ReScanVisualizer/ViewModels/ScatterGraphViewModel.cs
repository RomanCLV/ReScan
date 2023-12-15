using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels.Parts;
using ReScanVisualizer.ViewModels.Samples;
using HelixToolkit.Wpf;
using System.Diagnostics;
using System.Text.RegularExpressions;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class ScatterGraphViewModel : ViewModelBase, I3DElement
    {
        public event EventHandler<bool>? IsHiddenChanged;

        private IPartSource? _partsListSource;
        public IPartSource? PartsListSource
        {
            get => _partsListSource;
            set
            {
                if (!Equals(_partsListSource, value))
                {
                    if (_partsListSource != null && _partsListSource.Parts is INotifyCollectionChanged oc)
                    {
                        oc.CollectionChanged -= SourceParts_CollectionChanged;
                    }
                }
                if (SetValue(ref _partsListSource, value))
                {
                    if (_partsListSource != null && _partsListSource.Parts is INotifyCollectionChanged oc)
                    {
                        oc.CollectionChanged += SourceParts_CollectionChanged;
                    }
                }
            }
        }

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
                //if (_part != null && _scaleFactor != value)
                //{
                //    throw new InvalidOperationException("The scale factor of a ScatterGraphViewModel cannot be set when it belongs to a part.");
                //}
                if (SetValue(ref _scaleFactor, value))
                {
                    _barycenter.ScaleFactor = _scaleFactor;
                    _base3D.ScaleFactor = _scaleFactor;
                    _averagePlan.ScaleFactor = _scaleFactor;
                    foreach (SampleViewModel sample in Samples)
                    {
                        sample.ScaleFactor = _scaleFactor;
                    }
                }
            }
        }

        private ScatterGraph _scatterGraph;

        private double _pointsRadius;
        public double PointsRadius
        {
            get => _pointsRadius;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Points radius must be greater than 0.");
                }
                if (SetValue(ref _pointsRadius, value))
                {
                    foreach (SampleViewModel sample in Samples)
                    {
                        sample.Radius = _pointsRadius;
                    }
                }
            }
        }

        public bool IsBarycenterHidden => Barycenter.IsHidden;

        public bool IsAveragePlanHidden => AveragePlan.IsHidden;

        public bool IsBaseHidden => Base3D.IsHidden;

        public ObservableCollection<SampleViewModel> Samples { get; private set; }

        private BarycenterViewModel _barycenter;
        public BarycenterViewModel Barycenter
        {
            get => _barycenter;
            private set => SetValue(ref _barycenter, value);
        }

        private PlanViewModel _averagePlan;
        public PlanViewModel AveragePlan
        {
            get => _averagePlan;
            private set => SetValue(ref _averagePlan, value);
        }

        private Base3DViewModel _base3D;
        public Base3DViewModel Base3D
        {
            get => _base3D;
            private set => SetValue(ref _base3D, value);
        }

        private readonly Model3DGroup _model;
        public Model3D Model => _model;

        public ColorViewModel Color { get; set; }

        private bool _oldBarycenterIsHiden;
        private bool _oldAveragePlanIsHiden;
        private bool _oldBase3DIsHiden;

        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                if (SetValue(ref _isHidden, value))
                {
                    if (_isHidden)
                    {
                        _oldBarycenterIsHiden = _barycenter.IsHidden;
                        _oldAveragePlanIsHiden = _averagePlan.IsHidden;
                        _oldBase3DIsHiden = _base3D.IsHidden;
                        _barycenter.Hide();
                        _averagePlan.Hide();
                        _base3D.Hide();
                        HidePoints();
                    }
                    else
                    {
                        _barycenter.IsHidden = _oldBarycenterIsHiden;
                        _averagePlan.IsHidden = _oldAveragePlanIsHiden;
                        _base3D.IsHidden = _oldBase3DIsHiden;
                        ShowPoints();
                    }
                    OnIsHiddenChanged();
                }
            }
        }

        private bool _arePointsHidden;
        public bool ArePointsHidden
        {
            get => _arePointsHidden;
            private set => SetValue(ref _arePointsHidden, value);
        }

        private RenderQuality _renderQuality;
        public RenderQuality RenderQuality
        {
            get => _renderQuality;
            set
            {
                if (SetValue(ref _renderQuality, value))
                {
                    Base3D.RenderQuality = _renderQuality;
                    AveragePlan.RenderQuality = _renderQuality;
                    foreach (SampleViewModel sample in Samples)
                    {
                        sample.RenderQuality = _renderQuality;
                    }
                }
            }
        }

        public List<RenderQuality> RenderQualities { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            private set => SetValue(ref _isSelected, value);
        }

        private bool _isMouseOver;
        public bool IsMouseOver
        {
            get => _isMouseOver;
            set => SetValue(ref _isMouseOver, value);
        }

        public int ItemsCount
        {
            get => Samples.Count + 5; // Points count + barycenter (1) + averplan (1) + base (x, y, z) (3)
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value);
        }

        private bool _writeHeaders;
        public bool WriteHeaders
        {
            get => _writeHeaders;
            set => SetValue(ref _writeHeaders, value);
        }

        private PartViewModelBase? _part;
        public PartViewModelBase? Part
        {
            get => _part;
            set
            {
                PartViewModelBase? partRemoveLater = _part;
                if (_part != null)
                {
                    _part.OriginChanged -= Part_OriginChanged;
                }
                if (SetValue(ref _part, value))
                {
                    partRemoveLater?.Remove(this);
                    if (_part != null)
                    {
                        _part.OriginChanged += Part_OriginChanged;
                        _part.Add(this);
                        ScaleFactor = _part.ScaleFactor;
                    }
                }
            }
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
        public ScatterGraphViewModel(ScatterGraph scatterGraph, Color color, double scaleFactor = 1.0, double axisScaleFactor = 1.0, double pointRadius = 0.25, RenderQuality renderQuality = RenderQuality.High, bool hideBarycenter = false, bool hideAveragePlan = false, bool hideBase = false)
        {
            if (scaleFactor <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleFactor), "Scale factor must be greater than 0.");
            }
            _name = "Scatter Graph";
            _scaleFactor = scaleFactor;
            _scatterGraph = scatterGraph;
            _pointsRadius = pointRadius;
            _renderQuality = renderQuality;
            _isSelected = false;
            _isMouseOver = false;
            RenderQualities = new List<RenderQuality>(Tools.GetRenderQualitiesList());
            Color = new ColorViewModel(color);
            _isHidden = color.A == 0;
            _arePointsHidden = false;
            _writeHeaders = true;
            _part = null;

            _model = new Model3DGroup();
            Samples = new ObservableCollection<SampleViewModel>();

            for (int i = 0; i < _scatterGraph.Count; i++)
            {
                SampleViewModel sampleViewModel = new SampleViewModel(_scatterGraph[i], Color.Color, _scaleFactor, _pointsRadius, _renderQuality)
                {
                    ScatterGraph = this
                };
                sampleViewModel.IsHiddenChanged += SampleViewModel_IsHiddenChanged;
                sampleViewModel.Point.PropertyChanged += Point_PropertyChanged;
                Samples.Add(sampleViewModel);
                _model.Children.Add(sampleViewModel.Model);
            }

            UpdateArePointsHidden();

            /* Equivalent à RecomputeAll() */
            Point3D barycenter = ComputeBarycenter();
            Plan averagePlan = ComputeAveragePlan();
            Base3D base3D = ComputeBase3D(barycenter, averagePlan);
            double averagePlanLength = ComputeAveragePlanLength(base3D);

            _barycenter = new BarycenterViewModel(barycenter, Colors.Red, _scaleFactor, _pointsRadius, _renderQuality)
            {
                ScatterGraph = this
            };
            _base3D = new Base3DViewModel(base3D, _scaleFactor, axisScaleFactor, _renderQuality)
            {
                Name = "Plan base",
                ScatterGraph = this
            };
            _averagePlan = new PlanViewModel(averagePlan, barycenter, _base3D.X, Colors.LightBlue.ChangeAlpha(191), averagePlanLength, _scaleFactor, _renderQuality)
            {
                ScatterGraph = this,
                CanEdit = false
            };
            /* Fin - Equivalent à RecomputeAll() */

            _barycenter.IsHidden = hideBarycenter;
            _averagePlan.IsHidden = hideAveragePlan;
            _base3D.IsHidden = hideBase;

            Color.PropertyChanged += Color_PropertyChanged;
            Samples.CollectionChanged += Points_CollectionChanged;
        }

        ~ScatterGraphViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                Part = null;
                PartsListSource = null;
                if (Color != null)
                {
                    Color.PropertyChanged -= Color_PropertyChanged;
                    Color.Dispose();
                }
                if (Samples != null)
                {
                    Samples.CollectionChanged -= Points_CollectionChanged;
                    Clear();
                }
                _barycenter?.Dispose();
                _averagePlan?.Dispose();
                _base3D?.Dispose();
                _model.Children.Clear();
                base.Dispose();
            }
        }

        private void Color_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateModelMaterial();
        }

        private void SourceParts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PartsListSource));
        }

        private void Points_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RecomputeAll();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object? o in e.NewItems)
                    {
                        SampleViewModel sampleViewModel = (SampleViewModel)o;
                        if (!Equals(sampleViewModel.ScatterGraph))
                        {
                            sampleViewModel.ScatterGraph = this;
                        }
                        sampleViewModel.IsHidden = ArePointsHidden;
                        sampleViewModel.IsHiddenChanged += SampleViewModel_IsHiddenChanged;
                        sampleViewModel.Point.PropertyChanged += Point_PropertyChanged;
                        _model.Children.Add(sampleViewModel.Model);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object? o in e.OldItems)
                    {
                        SampleViewModel sampleViewModel = (SampleViewModel)o;
                        sampleViewModel.ScatterGraph = null;
                        sampleViewModel.IsHiddenChanged -= SampleViewModel_IsHiddenChanged;
                        sampleViewModel.Point.PropertyChanged -= Point_PropertyChanged;
                        RemoveModelOfSample(sampleViewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    _model.Children.Clear();
                    break;

                default:
                    throw new NotImplementedException();
            }

            OnPropertyChanged(nameof(ItemsCount));
        }

        public void Clear()
        {
            foreach (SampleViewModel sampleViewModel in Samples)
            {
                sampleViewModel.ScatterGraph = null;
                sampleViewModel.IsHiddenChanged -= SampleViewModel_IsHiddenChanged;
                sampleViewModel.Point.PropertyChanged -= Point_PropertyChanged;
                sampleViewModel.Dispose();
            }
            Samples.Clear();
        }

        public void Add(SampleViewModel sampleViewModel)
        {
            Samples.Add(sampleViewModel);
        }

        public void AddRange(IEnumerable<SampleViewModel> sampleViewModels)
        {
            Samples.CollectionChanged -= Points_CollectionChanged;
            foreach (var item in sampleViewModels)
            {
                Samples.Add(item);
            }
            Samples.CollectionChanged += Points_CollectionChanged;
            RecomputeAll();
        }

        public void RemoveSample(SampleViewModel sampleViewModel)
        {
            sampleViewModel.ScatterGraph = null;
            sampleViewModel.IsHiddenChanged -= SampleViewModel_IsHiddenChanged;
            sampleViewModel.Point.PropertyChanged -= Point_PropertyChanged;
            RemoveModelOfSample(sampleViewModel);
            Samples.Remove(sampleViewModel);
        }

        private void RemoveModelOfSample(SampleViewModel sampleViewModel)
        {
            for (int i = 0; i < _model.Children.Count; i++)
            {
                if (sampleViewModel.Model.Equals(_model.Children[i]))
                {
                    _model.Children.RemoveAt(i);
                    break;
                }
            }
        }

        public void SetFrom(ScatterGraph scatterGraph)
        {
            Samples.CollectionChanged -= Points_CollectionChanged;
            Clear();
            _model.Children.Clear();
            _scatterGraph = scatterGraph;
            for (int i = 0; i < _scatterGraph.Count; i++)
            {
                SampleViewModel sampleViewModel = new SampleViewModel(_scatterGraph[i], Color.Color, _scaleFactor, _pointsRadius, _renderQuality)
                {
                    ScatterGraph = this
                };
                sampleViewModel.IsHiddenChanged += SampleViewModel_IsHiddenChanged;
                sampleViewModel.Point.PropertyChanged += Point_PropertyChanged;
                Samples.Add(sampleViewModel);
                _model.Children.Add(sampleViewModel.Model);
            }
            Samples.CollectionChanged += Points_CollectionChanged;
            RecomputeAll();
        }

        private void OnIsHiddenChanged()
        {
            IsHiddenChanged?.Invoke(this, _isHidden);
        }

        public void InverseIsHidden()
        {
            IsHidden = !_isHidden;
        }

        public void Hide()
        {
            IsHidden = true;
        }

        public void Show()
        {
            IsHidden = false;
        }

        public void HidePoints()
        {
            foreach (SampleViewModel sample in Samples)
            {
                sample.Hide();
            }
            ArePointsHidden = true;
        }

        public void ShowPoints()
        {
            foreach (SampleViewModel sample in Samples)
            {
                sample.Show();
            }
            ArePointsHidden = false;
        }

        public void Select()
        {
            IsSelected = true;
        }

        public void Unselect()
        {
            IsSelected = false;
        }

        private void SampleViewModel_IsHiddenChanged(object sender, bool e)
        {
            UpdateArePointsHidden();
        }

        private void UpdateArePointsHidden()
        {
            foreach (SampleViewModel sampleViewModel in Samples)
            {
                if (!sampleViewModel.IsHidden)
                {
                    ArePointsHidden = false;
                    return;
                }
            }
            ArePointsHidden = Samples.Count != 0;
        }

        private void Point_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RecomputeAll();
        }

        private void Part_OriginChanged(object sender, EventArgs e)
        {
            RecomputeAll();
        }

        private void RecomputeAll()
        {
            _scatterGraph.Clear();
            foreach (SampleViewModel item in Samples)
            {
                _scatterGraph.AddPoint(item.Point.Point);
            }

            Point3D barycenter = _scatterGraph.ComputeBarycenter();
            Plan averagePlan = ComputeAveragePlan();
            Base3D base3D = ComputeBase3D(barycenter, averagePlan);
            CorrectBaseWithPart(base3D);
            double averagePlanLength = ComputeAveragePlanLength(base3D);

            _barycenter.UpdatePoint(barycenter);
            _base3D.UpdateBase(base3D);
            _averagePlan.UpdatePlan(barycenter, averagePlan, base3D.X, averagePlanLength);
        }

        private Point3D ComputeBarycenter()
        {
            return _scatterGraph.ComputeBarycenter();
        }

        private Plan ComputeAveragePlan()
        {
            Plan averagePlan;
            try
            {
                averagePlan = _scatterGraph.ComputeAveragePlan();
            }
            catch (InvalidOperationException)
            {
                averagePlan = new Plan(0, 0, 1, 0);
            }
            return averagePlan;
        }

        private double ComputeAveragePlanLength(Base3D base3D)
        {
            int size = _scatterGraph.Count;
            Point3D currentPoint;
            double maxDistance = 0.0;
            double currentDistance;
            Matrix3D matrix = base3D.GetTransformMatrix();
            matrix.Invert();

            for (int i = 0; i < size; i++)
            {
                currentPoint = _scatterGraph[i];

                // cancel translation of origin
                currentPoint.Offset(-base3D.Origin.X, -base3D.Origin.Y, -base3D.Origin.Z);

                // rotate by the invert matrix
                currentPoint = matrix.Transform(currentPoint);

                // projection over the plan
                // currentPoint.Z = 0.0;

                currentDistance = Math.Max(Math.Abs(currentPoint.X), Math.Abs(currentPoint.Y));
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                }
            }
            return 2.0 * maxDistance;
        }

        private Base3D ComputeBase3D(Point3D barycenter, Plan averagePlan)
        {
            Base3D base3D;
            if (_scatterGraph.Count < 2)
            {
                base3D = new Base3D(barycenter);
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
                    base3D = new Base3D(barycenter, x, y, z);
                }
                else
                {
                    base3D = ScatterGraph.ComputeRepere3D(barycenter, averagePlan, false);
                }
            }
            return base3D;
        }

        private void CorrectBaseWithPart(Base3D base3D)
        {
            if (_part != null)
            {
                Base3D nearestBase = _part.FindNeareatBase(base3D.Origin);
                double angleBetween = Vector3D.AngleBetween(base3D.Z, nearestBase.Z);
                if (angleBetween > 90)
                {
                    // Mettre le Z dans la bonne direction
                    base3D.Rotate(base3D.Y, 180.0);
                }

                double angle = GetAnglesBetweenBasesXAxis(nearestBase, base3D);
                base3D.Rotate(base3D.Z, -angle);
                angle = GetAnglesBetweenBasesXAxis(nearestBase, base3D);

                Base3D b = new Base3D(_part.OriginBase.Base3D);
                b.Origin = base3D.Origin;
                Base3DViewModel baseViewModel = new Base3DViewModel(b, _scaleFactor, 1.0, _renderQuality)
                {
                    Name = _name,
                    Opacity = 150
                };

                ((MainViewModel)Application.Current.MainWindow.DataContext).Bases.Add(baseViewModel);

                Trace.WriteLine(_name + ": " + Math.Round(angle, 1));
            }
        }

        private double GetAnglesBetweenBasesXAxis(Base3D base1, Base3D base2)
        {
            Base3D base2in1 = Tools.GetBase1IntoBase2(base2, base1);
            return Tools.RadianToDegree(Math.Atan2(base2in1.X.Y, base2in1.X.X));
        }

        public void UpdateModelGeometry()
        {
            throw new InvalidOperationException("UpdateModelGeometry not allowed for a ScatterGraphViewModel.");
        }

        public void UpdateModelMaterial()
        {
            foreach (SampleViewModel point in Samples)
            {
                point.Color.Set(Color.Color);
            }
        }

        public bool IsBelongingToModel(GeometryModel3D geometryModel3D)
        {
            return
                Barycenter.IsBelongingToModel(geometryModel3D) ||
                AveragePlan.IsBelongingToModel(geometryModel3D) ||
                Base3D.IsBelongingToModel(geometryModel3D) ||
                Samples.Any(x => x.IsBelongingToModel(geometryModel3D));
        }

        public void Export()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Export scatter graph",
                Filter = "Fichiers CSV (*.csv)|*.csv",
                DefaultExt = ".csv"
            };
            saveFileDialog.ShowDialog();

            if (!string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                try
                {
                    ScatterGraph.SaveCSV(saveFileDialog.FileName, _scatterGraph, true, false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Reduce the number of points by skipping points.<br />
        /// <br />
        /// Examples:<br />
	    /// skipped:  3 -> reduce by  3 - if you have 100 points, you will now have 33 and the taken points are index 0,  3,  6, ..., 99<br />
	    /// skipped: 10 -> reduce by 10 - if you have 100 points, you will now have 10 and the taken points are index 0, 10, 20, ..., 90<br />
        /// </summary>
        /// <param name="skipped">between 2 and number of points</param>
        public void Reduce(int skipped)
        {
            SetFrom(_scatterGraph.GetReduced(skipped));
        }

        /// <summary>
        /// Reduce the number of points by the given factor.<br />
        /// <br />
	    /// Examples:<br />
	    /// percent:  10 -> reduced by  10% - if you have 100 points, you will now have 90<br />
        /// percent:  80 -> reduced by  80% - if you have 100 points, you will now have 20<br />
        /// percent:   0 -> reduced by   0% - no changes<br />
        /// percent: 100 -> reduced by 100% - cleared<br />
        /// </summary>
        /// <param name="percent">Percentage of reduction (between 0.0 and 100.0).</param>
        public void ReducePercent(double reductionFactor)
        {
            SetFrom(_scatterGraph.GetReducedPercent(reductionFactor));
        }
    }
}

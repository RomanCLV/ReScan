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
using System.Data.SqlTypes;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class ScatterGraphViewModel : ViewModelBase, I3DElement, ICameraFocusable
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

        private int _maxPointsToDisplay;
        public int MaxPointsToDisplay
        {
            get => _maxPointsToDisplay;
            set
            {
                if (SetValue(ref _maxPointsToDisplay, value))
                {
                    UpdateModelGeometry();
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

        public ObservableCollection<SampleViewModel> Samples { get; }
        private readonly List<SampleViewModel> _displayedSamples;

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
            set
            {
                if (SetValue(ref _isMouseOver, value) && _part != null)
                {
                    _part.IsMouseOver = _isMouseOver;
                }
            }
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
        public ScatterGraphViewModel(ScatterGraph scatterGraph, Color color, double scaleFactor = 1.0, double axisScaleFactor = 1.0, double pointRadius = 0.25, RenderQuality renderQuality = RenderQuality.High, int pointToDisplay = -1, bool hideBarycenter = false, bool hideAveragePlan = false, bool hideBase = false)
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
            _maxPointsToDisplay = pointToDisplay;
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
            _displayedSamples = new List<SampleViewModel>();

            for (int i = 0; i < _scatterGraph.Count; i++)
            {
                SampleViewModel sampleViewModel = new SampleViewModel(_scatterGraph[i], Color.Color, _scaleFactor, _pointsRadius, _renderQuality)
                {
                    ScatterGraph = this
                };
                sampleViewModel.IsHiddenChanged += SampleViewModel_IsHiddenChanged;
                sampleViewModel.Point.PropertyChanged += Point_PropertyChanged;
                Samples.Add(sampleViewModel);
                //_model.Children.Add(sampleViewModel.Model);
            }

            UpdateModelGeometry();

            UpdateArePointsHidden();

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
                CanEditName = false,
                CanTranslate = false,
                CanReorient = false,
                ScatterGraph = this
            };
            _averagePlan = new PlanViewModel(averagePlan, barycenter, _base3D.X, Colors.LightBlue.ChangeAlpha(191), averagePlanLength, _scaleFactor, _renderQuality)
            {
                ScatterGraph = this,
                CanEdit = false
            };

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
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object? o in e.OldItems)
                    {
                        SampleViewModel sampleViewModel = (SampleViewModel)o;
                        sampleViewModel.ScatterGraph = null;
                        sampleViewModel.IsHiddenChanged -= SampleViewModel_IsHiddenChanged;
                        sampleViewModel.Point.PropertyChanged -= Point_PropertyChanged;
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    //_model.Children.Clear();
                    break;

                default:
                    throw new NotImplementedException();
            }

            OnPropertyChanged(nameof(ItemsCount));
            UpdateModelGeometry();
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
            Samples.Remove(sampleViewModel);
        }

        public void SetFrom(ScatterGraph scatterGraph)
        {
            Samples.CollectionChanged -= Points_CollectionChanged;
            Clear();
            //_model.Children.Clear();
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
                //_model.Children.Add(sampleViewModel.Model);
            }
            Samples.CollectionChanged += Points_CollectionChanged;
            RecomputeAll();
            UpdateModelGeometry();
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
            bool arePointsHidden = true;
            foreach (SampleViewModel sample in Samples)
            {
                if (_displayedSamples.Contains(sample))
                {
                    sample.Show();
                    arePointsHidden = false;
                }
                else
                {
                    sample.Hide();
                }
            }
            ArePointsHidden = arePointsHidden;
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
            double distance;
            Matrix3D t0B = base3D.ToMatrix3D();
            Matrix3D tB0 = t0B.Inverse();

            for (int i = 0; i < size; i++)
            {
                currentPoint = _scatterGraph[i];

                // rotate by the invert matrix
                currentPoint = tB0.Transform(currentPoint);

                distance = Math.Max(Math.Abs(currentPoint.X), Math.Abs(currentPoint.Y));
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                }
            }

            if (maxDistance == 0.0)
            {
                maxDistance = 0.5;
            }

            return 2.0 * maxDistance;
        }

        private Base3D ComputeBase3D(Point3D barycenter, Plan averagePlan)
        {
            return _scatterGraph.ComputeBase3D(barycenter, averagePlan);
        }

        private void CorrectBaseWithPart(Base3D base3D)
        {
            if (_part != null)
            {
                Base3D nearestBase = _part.FindNeareatBase(base3D.Origin);
                if (_scatterGraph.Count == 0)
                {
                    base3D.SetFrom(nearestBase, false);
                }
                else
                {
                    double angle = Vector3D.AngleBetween(base3D.Z, nearestBase.Z);
                    if (angle > 90.0)
                    {
                        // Mettre le Z dans la bonne direction
                        base3D.Rotate(base3D.Y, 180.0);
                    }

                    angle = Vector3D.AngleBetween(base3D.X, nearestBase.X);
                    if (angle > 90.0)
                    {
                        // Mettre le X dans la bonne direction
                        base3D.Rotate(base3D.Z, 180.0);

                        // remarque : techniquement on ne devrait pas a avoir le faire
                        // mais cas particulier fait que une base avait son X parfaitement à 180° de X de reference
                        // donc patch
                        // de plus, ca replace les x entre [-90;90] au lieu de [-180;180] donc ça aide (enormement) pour l'algorithme suivant
                        // donc pas plus mal de le faire
                    }

                    angle = GetAnglesBetweenBasesXAxis(base3D, nearestBase);
                    if (!double.IsNaN(angle) && angle != 0.0)
                    {
                        base3D.Rotate(base3D.Z, angle);
                    }
                }
            }
        }

        /// <summary>
        /// Find the angle to apply to match the X axis of the two given bases.
        /// </summary>
        /// <returns>Return the angle to apply in degree or <see cref="double.NaN"/> if the matrix is can't be inverted.</returns>
        private double GetAnglesBetweenBasesXAxis(Base3D base1, Base3D base2)
        {
            Base3D base3DInPartBase;
            try
            {
                base3DInPartBase = Tools.GetBase1IntoBase2(base1, base2);
            }
            catch (InvalidOperationException)
            {
                return double.NaN;
            }

            /*
             * La but est de trouver un angle qui fait que la rotation de la base autour de son Z (par cette angle)
             * fait que le nouveau vecteur X est applatit dans le plan XZ
             * (donc le vecteur engendre par X(1,0,0) ^ X'(xx',xy',xz') appartient au plan XY et on en deduit que xy' = 0)
             * On cherche dont l'angle a tq xy' = 0
             * 
             * Pour le trouver on fait B'=R*B (B' base rotate, B base normal, R rotation)
             * On souhaite faire la rotation autour de l'axe Z de la base donc on construit la matrice de rotation selon un axe U (zx, zy, zz) d'angle a.
             * Comme on cherche que xy', pas besoin de faire tous la rotation, on peut faire les calculs a la main juste pour avoir xy'.
             * On trouve : xy' = k0 + k1 * cos(a) + k2 * sin(a) => 2 "inconnus" pour 1 equation
             * 
             * Algo pas optimise car l'angle est teste par incrementation
             */

            double xx = base3DInPartBase.X.X;
            double xy = base3DInPartBase.X.Y;
            double xz = base3DInPartBase.X.Z;

            double ux = base3DInPartBase.Z.X;
            double uy = base3DInPartBase.Z.Y;
            double uz = base3DInPartBase.Z.Z;

            double uy2 = uy * uy;

            double k0 = xx * ux * uy + xy * uy2 + xz * uy * uz;
            double k1 = xy - xx * ux * uy - xy * uy2 - xz * uy * uz;
            double k2 = xx * uz - xz * ux;

            double epsilon = 0.01;

            // valeur de l'angle teste
            double a = 0.0;

            // nouvelle valeur de la composante Y du vecteur X lorsqu'il a ete tourne par a autour de son Z. On veut cette valeur le plus proche de 0.
            double newXY = k0 + k1;

            double step = Math.PI / 360.0;

            double minNewXY = newXY;
            double minA = 0.0;

            if (newXY > 0.0)
            {
                // algo version angle positif
                while (newXY > epsilon)
                {
                    a -= step;
                    newXY = k0 + Math.Cos(a) * k1 + Math.Sin(a) * k2; // calcul de la nouvelle composante Y du vecteur X apres rotation
                    if (newXY < 0.0)        // 0 atteint
                    {
                        a += step;          // go back to last pos
                        step /= 10.0;       // reduce step to be more precise
                    }

                    /*
                     * patch dans le cas ou il arriverait pas a atteindre le 0 (cas assez rare mais possible..)
                     * ne devrait plus arriver grace au flip du vecteur X (voir un cran au dessus dans la pile d'appel)
                     * mais ca mange pas de pain de le laisser au cas ou...
                     */

                    if (newXY < minNewXY)
                    {
                        minNewXY = newXY;
                        minA = a;           // on enregistre la valeur de a qui permet d'avoir un newXY minimal
                    }
                    if (a <= -360.0)        // patch dans le cas où il arriverait pas a atteindre le 0 et qu'il vient de faire un tour complet
                    {
                        a = minA;
                        break;
                    }
                }
            }
            else
            {
                epsilon = -epsilon;
                // algo version angle negatif
                while (newXY < epsilon)
                {
                    a += step;
                    newXY = k0 + Math.Cos(a) * k1 + Math.Sin(a) * k2;
                    if (newXY > 0.0)
                    {
                        a -= step;      // go back to last pos
                        step /= 10.0;   // reduce step
                    }
                    if (newXY < minNewXY)
                    {
                        minNewXY = newXY;
                        minA = a;
                    }
                    if (a >= 360.0)
                    {
                        a = minA;
                        break;
                    }
                }
            }
            return Tools.RadianToDegree(a);
        }

        public void UpdateModelGeometry()
        {
            int insertIndex = 0;

            if (Samples.Count == 0)
            {
                _displayedSamples.Clear();
                _model.Children.Clear();
            }
            else
            {
                if (_maxPointsToDisplay < 0 || _maxPointsToDisplay > Samples.Count) // display all
                {
                    _displayedSamples.Clear();
                    _model.Children.Clear();
                    for (int i = 0; i < Samples.Count; i++)
                    {
                        Samples[i].Show();
                        _displayedSamples.Add(Samples[i]);
                        _model.Children.Add(Samples[i].Model);
                    }
                }
                else if (_maxPointsToDisplay == 0) // hide all
                {
                    if (_displayedSamples.Count != 0)
                    {
                        _displayedSamples.Clear();
                        _model.Children.Clear();
                    }
                }
                else // take a few
                {
                    int increment = Samples.Count / _maxPointsToDisplay;
                    for (int i = 0; i < Samples.Count; i++)
                    {
                        if (i % increment == 0) // point to display
                        {
                            if (!_displayedSamples.Contains(Samples[i]))
                            {
                                Samples[i].Show();
                                _displayedSamples.Insert(insertIndex, Samples[i]);
                                _model.Children.Insert(insertIndex, Samples[i].Model);
                            }
                            insertIndex++;
                        }
                        else // point to hide
                        {
                            if (_displayedSamples.Contains(Samples[i]))
                            {
                                _displayedSamples.RemoveAt(insertIndex);
                                _model.Children.RemoveAt(insertIndex);
                                Samples[i].IsHidden = true;
                            }
                        }
                    }
                }
            }
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
                    ScatterGraph.SaveCSV(saveFileDialog.FileName, _scatterGraph, true, _writeHeaders, false); // TODO: ajouter l'option . / ,
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
        /// <param name="reductionFactor">Percentage of reduction (between 0.0 and 100.0).</param>
        public void ReducePercent(double reductionFactor)
        {
            SetFrom(_scatterGraph.GetReducedPercent(reductionFactor));
        }

        public CameraConfiguration GetCameraConfigurationToFocus(double fov = 45.0, double distanceScaling = 1.0, double minDistance = 0.0)
        {
            Vector3D direction = Tools.AreVectorsColinear(_base3D.Z, new Vector3D(0, 0, 1)) ? new Vector3D(-1.0, -1.0, -1.0) : -_base3D.Z;
            return GetCameraConfigurationToFocus(direction, fov, distanceScaling, minDistance);
        }

        public CameraConfiguration GetCameraConfigurationToFocus(Vector3D direction, double fov = 45.0, double distanceScaling = 1.0, double minDistance = 0.0)
        {
            Rect3D bounds = Samples.Count == 0 ? Base3D.Model.Bounds : _model.Bounds;
            return CameraHelper.GetCameraConfigurationToFocus(bounds, _base3D.Origin.Multiply(_base3D.ScaleFactor), direction, fov, distanceScaling, minDistance);
        }

        public ScatterGraph ExpressedInItsOwnBase()
        {
            int size = _scatterGraph.Count;
            ScatterGraph scatterGraph = new ScatterGraph(size);
            Matrix3D tB0 = _base3D.Base3D.ToMatrix3D().Inverse();
            for (int i = 0; i < size; i++)
            {
                scatterGraph.AddPoint(tB0.Transform(_scatterGraph[i]));
            }
            return scatterGraph;
        }
    }
}

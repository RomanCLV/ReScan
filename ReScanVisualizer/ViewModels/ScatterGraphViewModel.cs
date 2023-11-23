﻿using HelixToolkit.Wpf;
using Microsoft.Win32;
using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

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

        public event EventHandler<bool>? IsHiddenChanged;

        private readonly ScatterGraph _scatterGraph;

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

        private SampleViewModel _barycenter;
        public SampleViewModel Barycenter
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
        public ScatterGraphViewModel(ScatterGraph scatterGraph, Color color, double scaleFactor = 1.0, double pointRadius = 0.25, RenderQuality renderQuality = RenderQuality.High, bool hideBarycenter = false, bool hideAveragePlan = false, bool hideBase = false)
        {
            if (scaleFactor <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleFactor), "Scale factor must be greater than 0.");
            }
            IsDisposed = false;
            _name = "Scatter Graph";
            _scaleFactor = scaleFactor;
            _scatterGraph = scatterGraph;
            _pointsRadius = pointRadius;
            _renderQuality = renderQuality;
            RenderQualities = new List<RenderQuality>(Tools.GetRenderQualitiesList());
            Color = new ColorViewModel(color);
            _isHidden = color.A == 0;
            _arePointsHidden = false;
            _writeHeaders = true;

            _model = new Model3DGroup();
            Samples = new ObservableCollection<SampleViewModel>();

            for (int i = 0; i < _scatterGraph.Count; i++)
            {
                SampleViewModel sampleViewModel = new SampleViewModel(_scatterGraph[i], Color.Color, _scaleFactor, _pointsRadius, _renderQuality);
                sampleViewModel.IsHiddenChanged += SampleViewModel_IsHiddenChanged;
                sampleViewModel.RemoveItem += SampleViewModel_RemoveItem;
                sampleViewModel.Point.PropertyChanged += Point_PropertyChanged;
                Samples.Add(sampleViewModel);
                _model.Children.Add(sampleViewModel.Model);
            }
            UpdateArePointsHidden();

            Point3D barycenter = ComputeBarycenter();
            Plan averagePlan = ComputeAveragePlan();
            Base3D base3D = ComputeBase3D(barycenter, averagePlan);
            double averagePlanLength = ComputeAveragePlanLength(base3D);

            _barycenter = new SampleViewModel(barycenter, Colors.Red, _scaleFactor, _pointsRadius, _renderQuality);
            _base3D = new Base3DViewModel(base3D, _scaleFactor, true, _renderQuality)
            {
                Name = "Plan base"
            };
            _averagePlan = new PlanViewModel(averagePlan, barycenter, _base3D.X, Colors.LightBlue.ChangeAlpha(191), averagePlanLength, _scaleFactor, _renderQuality);

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
                if (Color != null)
                {
                    Color.PropertyChanged -= Color_PropertyChanged;
                }
                if (Samples != null)
                {
                    Samples.CollectionChanged -= Points_CollectionChanged;
                    foreach (SampleViewModel sample in Samples)
                    {
                        sample.IsHiddenChanged -= SampleViewModel_IsHiddenChanged;
                        sample?.Dispose();
                    }
                    Application.Current?.Dispatcher.Invoke(() => Clear());
                }
                _averagePlan?.Dispose();
                _base3D?.Dispose();
                _barycenter?.Dispose();

                try
                {
                    _model.Children.Clear();
                }
                catch (InvalidOperationException)
                {
                }
                base.Dispose();
                IsDisposed = true;
            }
        }

        private void Color_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateModelMaterial();
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
            double averagePlanLength = ComputeAveragePlanLength(base3D);

            _barycenter.UpdatePoint(barycenter);
            _base3D.UpdateBase(base3D);
            _averagePlan.UpdatePlan(barycenter, averagePlan, base3D.X, averagePlanLength);
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
                        sampleViewModel.IsHidden = ArePointsHidden;
                        sampleViewModel.IsHiddenChanged += SampleViewModel_IsHiddenChanged;
                        sampleViewModel.RemoveItem += SampleViewModel_RemoveItem;
                        sampleViewModel.Point.PropertyChanged += Point_PropertyChanged;
                        _model.Children.Add(sampleViewModel.Model);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object? o in e.OldItems)
                    {
                        SampleViewModel sampleViewModel = (SampleViewModel)o;
                        sampleViewModel.IsHiddenChanged -= SampleViewModel_IsHiddenChanged;
                        sampleViewModel.RemoveItem -= SampleViewModel_RemoveItem;
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
                sampleViewModel.IsHiddenChanged -= SampleViewModel_IsHiddenChanged;
                sampleViewModel.RemoveItem -= SampleViewModel_RemoveItem;
                sampleViewModel.Point.PropertyChanged -= Point_PropertyChanged;
            }
            Samples.Clear();
        }

        private void Point_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RecomputeAll();
        }

        private void SampleViewModel_RemoveItem(object sender, EventArgs e)
        {
            RemoveSample((SampleViewModel)sender);
        }

        public void RemoveSample(SampleViewModel sampleViewModel)
        {
            sampleViewModel.IsHiddenChanged -= SampleViewModel_IsHiddenChanged;
            sampleViewModel.RemoveItem -= SampleViewModel_RemoveItem;
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
                    base3D = ScatterGraph.ComputeRepere3D(barycenter, averagePlan);
                }
            }
            return base3D;
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
    }
}

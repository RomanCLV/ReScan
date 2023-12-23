using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels.Samples;

#nullable enable

namespace ReScanVisualizer.ViewModels.Parts
{
    public abstract class PartViewModelBase : ViewModelBase, I3DElement
    {
        public event EventHandler<bool>? IsHiddenChanged;

        public event EventHandler? OriginChanged;

        private string _name;
        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value);
        }

        private bool _isScatterGraphesHiddenChanging;
        private bool _isBasesHiddenChanging;

        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                if (SetValue(ref _isHidden, value))
                {
                    AreScatterGraphesHidden = _isHidden;
                    AreBasesHidden = _isHidden;
                    OnIsHiddenChanged();
                }
            }
        }

        // TODO : complete visible and fix binding errors

        private bool _areScatterGraphesHidden;
        public bool AreScatterGraphesHidden
        {
            get => _areScatterGraphesHidden;
            set
            {
                if (SetValue(ref _areScatterGraphesHidden, value))
                {
                    _isScatterGraphesHiddenChanging = true;
                    foreach (ScatterGraphViewModel scatterGraphViewModel in _scatterGraphs)
                    {
                        scatterGraphViewModel.IsHidden = _areScatterGraphesHidden;
                    }
                    _isScatterGraphesHiddenChanging = false;
                }
            }
        }

        private bool _areBasesHidden;
        public bool AreBasesHidden
        {
            get => _areBasesHidden;
            set
            {
                if (SetValue(ref _areBasesHidden, value))
                {
                    _isBasesHiddenChanging = true;
                    foreach (Base3DViewModel base3D in _bases)
                    {
                        base3D.IsHidden = _areBasesHidden;
                    }
                    _isBasesHiddenChanging = false;
                }
            }
        }

        private readonly List<Base3DViewModel> _bases;
        public ReadOnlyCollection<Base3DViewModel> Bases { get; }

        private ObservableCollection<ScatterGraphViewModel> _scatterGraphs;
        public ObservableCollection<ScatterGraphViewModel> ScatterGraphs
        {
            get => _scatterGraphs;
            private set => SetValue(ref _scatterGraphs, value);
        }

        public Base3DViewModel OriginBase { get; private set; }

        private BarycenterViewModel _barycenter;
        public BarycenterViewModel Barycenter
        {
            get => _barycenter;
            private set => SetValue(ref _barycenter, value);
        }

        private bool _originAttachedToBarycenter;
        public bool OriginAttachedToBarycenter
        {
            get => _originAttachedToBarycenter;
            set => SetValue(ref _originAttachedToBarycenter, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            private set => SetValue(ref _isSelected, value);
            //private set
            //{
            //    if (SetValue(ref _isSelected, value))
            //    {
            //        if (_isSelected)
            //        {
            //            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            //            {
            //                scatterGraphViewModel.Select();
            //            }
            //        }
            //        else
            //        {
            //            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            //            {
            //                scatterGraphViewModel.Unselect();
            //            }
            //        }
            //    }
            //}
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
                if (SetValue(ref _scaleFactor, value))
                {
                    _barycenter.ScaleFactor = _scaleFactor;
                    OriginBase.ScaleFactor = _scaleFactor;
                    foreach (var item in ScatterGraphs)
                    {
                        item.ScaleFactor = _scaleFactor;
                    }
                }
            }
        }

        private bool _isMouseOver;
        public bool IsMouseOver
        {
            get => _isMouseOver;
            set => SetValue(ref _isMouseOver, value);
        }

        public virtual ColorViewModel Color => throw new InvalidOperationException();

        protected readonly Model3DGroup _modelGroup;
        public Model3D Model => _modelGroup;

        private RenderQuality _renderQuality;
        public RenderQuality RenderQuality
        {
            get => _renderQuality;
            set
            {
                if (SetValue(ref _renderQuality, value))
                {
                    foreach (var item in ScatterGraphs)
                    {
                        item.RenderQuality = _renderQuality;
                    }
                }
            }
        }
        public List<RenderQuality> RenderQualities { get; }

        public static uint InstanceCreated { get; private set; }

        private bool _isRecomputeAllOnScatterGraphsChangedEnalbed;

        public int ItemsCount
        {
            get => _scatterGraphs.Count + _bases.Count + 1; // scatterGraphs count + barycenter (1) + bases count
        }

        public PartViewModelBase(Base3D originBase, double scaleFactor = 1.0, RenderQuality renderQuality = RenderQuality.High)
        {
            InstanceCreated++;
            _isRecomputeAllOnScatterGraphsChangedEnalbed = true;
            _name = "Part " + InstanceCreated;
            _isHidden = false;
            _areBasesHidden = _isHidden;
            _originAttachedToBarycenter = true;
            _scaleFactor = scaleFactor;
            _renderQuality = renderQuality;
            RenderQualities = new List<RenderQuality>(Tools.GetRenderQualitiesList());
            _isMouseOver = false;
            _isSelected = false;
            _bases = new List<Base3DViewModel>(1);
            _modelGroup = new Model3DGroup();
            _scatterGraphs = new ObservableCollection<ScatterGraphViewModel>();
            _barycenter = new BarycenterViewModel(originBase.Origin, Colors.Yellow, _scaleFactor, 0.25, renderQuality);
            OriginBase = new Base3DViewModel(originBase, _scaleFactor, 1.0, renderQuality)
            {
                Name = "Origin base"
            };
            _modelGroup.Children.Add(_barycenter.Model);
            _modelGroup.Children.Add(OriginBase.Model);

            AddBase(OriginBase);
            Bases = new ReadOnlyCollection<Base3DViewModel>(_bases);

            _scatterGraphs.CollectionChanged += ScatterGraphs_CollectionChanged;
            OriginBase.PropertyChanged += OriginBase_PropertyChanged;
        }

        ~PartViewModelBase()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                OriginBase.PropertyChanged -= OriginBase_PropertyChanged;
                _scatterGraphs.CollectionChanged -= ScatterGraphs_CollectionChanged;
                Clear();
                ClearBases();
                _modelGroup.Children.Clear();
                _barycenter.Dispose();
                //OriginBase.Dispose();
                base.Dispose();
            }
        }

        private void OnIsHiddenChanged()
        {
            IsHiddenChanged?.Invoke(this, _isHidden);
        }

        private void OnOriginChanged()
        {
            OriginChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OriginBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Base3DViewModel.Origin) ||
                e.PropertyName == nameof(Base3DViewModel.X) ||
                e.PropertyName == nameof(Base3DViewModel.Y) ||
                e.PropertyName == nameof(Base3DViewModel.Z))
            {
                OnOriginChanged();
            }
        }

        private void ScatterGraphs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ScatterGraphs.Count == 0)
            {
                ResetFromEmpty();
            }
            else if (ScatterGraphs.Count == 1)
            {
                SetFromFirstItem();
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object? item in e.NewItems)
                    {
                        ScatterGraphViewModel scatterGraphViewModel = (ScatterGraphViewModel)item;
                        scatterGraphViewModel.Barycenter.PropertyChanged += ItemBarycenter_PropertyChanged;
                        scatterGraphViewModel.IsHiddenChanged += ScatterGraphViewModel_IsHiddenChanged;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object? item in e.OldItems)
                    {
                        ScatterGraphViewModel scatterGraphViewModel = (ScatterGraphViewModel)item;
                        if (Equals(scatterGraphViewModel.Part))
                        {
                            scatterGraphViewModel.Part = null;
                            scatterGraphViewModel.Barycenter.PropertyChanged -= ItemBarycenter_PropertyChanged;
                            scatterGraphViewModel.IsHiddenChanged -= ScatterGraphViewModel_IsHiddenChanged;
                        }
                        if (_scatterGraphs.Count == 0)
                        {
                            ResetFromEmpty();
                        }
                    }
                    break;
            }
            OnPropertyChanged(nameof(ItemsCount));
            if (_isRecomputeAllOnScatterGraphsChangedEnalbed)
            {
                ComputeAll();
            }
        }

        private void SetFromFirstItem()
        {
            ScatterGraphViewModel scatterGraphViewModel = _scatterGraphs[0];
            ScaleFactor = scatterGraphViewModel.ScaleFactor;
            _barycenter.Radius = scatterGraphViewModel.Barycenter.Radius;
            OriginBase.AxisScaleFactor = scatterGraphViewModel.Base3D.AxisScaleFactor;
            _renderQuality = scatterGraphViewModel.RenderQuality;
        }

        private void ResetFromEmpty()
        {
            ScaleFactor = 1.0;
            _barycenter.Radius = 0.25;
            OriginBase.AxisScaleFactor = 1.0;
            _renderQuality = RenderQuality.High;
        }

        public void EnableRecomputeAllAfterScatterGraphsChanged()
        {
            _isRecomputeAllOnScatterGraphsChangedEnalbed = true;
        }

        public void DisableRecomputeAllAfterScatterGraphsChanged()
        {
            _isRecomputeAllOnScatterGraphsChangedEnalbed = false;
        }

        private void ScatterGraphViewModel_IsHiddenChanged(object sender, bool e)
        {
            if (!_isScatterGraphesHiddenChanging)
            {
                if ((_areScatterGraphesHidden && _scatterGraphs.Any(x => !x.IsHidden)) ||
                    (!_areScatterGraphesHidden && _scatterGraphs.All(x => x.IsHidden)))
                {
                    _areScatterGraphesHidden = !_areScatterGraphesHidden;
                    OnPropertyChanged(nameof(AreScatterGraphesHidden));
                }
            }
        }

        private void Base3DViewModel_IsHiddenChanged(object sender, bool e)
        {
            if (!_isBasesHiddenChanging)
            {
                if ((_areBasesHidden && _bases.Any(x => !x.IsHidden)) ||
                    (!_areBasesHidden && _bases.All(x => x.IsHidden)))
                {
                    _areBasesHidden = !_areBasesHidden;
                    OnPropertyChanged(nameof(AreBasesHidden));
                }
            }
        }

        private void ItemBarycenter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SampleViewModel.Point))
            {
                ComputeAll();
            }
        }

        public void Add(ScatterGraphViewModel scatterGraphViewModel)
        {
            if (!_scatterGraphs.Contains(scatterGraphViewModel))
            {
                _scatterGraphs.Add(scatterGraphViewModel);
            }
        }

        public void Remove(ScatterGraphViewModel scatterGraphViewModel)
        {
            _scatterGraphs.Remove(scatterGraphViewModel);
        }

        public void Clear()
        {
            if (_scatterGraphs.Count > 0)
            {
                foreach (ScatterGraphViewModel item in ScatterGraphs)
                {
                    item.Barycenter.PropertyChanged -= ItemBarycenter_PropertyChanged;
                    item.IsHiddenChanged -= ScatterGraphViewModel_IsHiddenChanged;
                    //item.Part = null; 
                    // TODO: check this functions ... maybe improve clean part gesture
                }
                _scatterGraphs.Clear();
            }
        }

        public void Hide()
        {
            IsHidden = true;
        }

        public void Show()
        {
            IsHidden = false;
        }

        public void InverseIsHidden()
        {
            IsHidden = !_isHidden;
        }

        public void Select()
        {
            IsSelected = true;
        }

        public void Unselect()
        {
            IsSelected = false;
        }

        /// <summary>
        /// Compute all that can be compute that depends on <see cref="ScatterGraphs"/>.
        /// Here, only the barycenter is computed.
        /// </summary>
        /// <param name="setOriginToBarycenter">Set the origin of the part to the new barycenter.</param>
        public virtual void ComputeAll()
        {
            _barycenter.UpdatePoint(ComputeBarycenter());
            if (_originAttachedToBarycenter)
            {
                SetOriginToBarycenter();
            }
        }

        /// <summary>
        /// Compute the barycenter of the part.
        /// </summary>
        /// <returns></returns>
        protected Point3D ComputeBarycenter()
        {
            if (_scatterGraphs.Count == 0)
            {
                return new Point3D(0.0, 0.0, 0.0);
            }
            Point3D point;

            double sumX = 0.0;
            double sumY = 0.0;
            double sumZ = 0.0;

            int skippedGraphsCount = 0;
            foreach (var item in _scatterGraphs)
            {
                if (item.Samples.Count == 0)
                {
                    skippedGraphsCount++;
                    continue;
                }
                point = item.Barycenter.Point.Point;
                sumX += point.X;
                sumY += point.Y;
                sumZ += point.Z;
            }

            if (skippedGraphsCount == _scatterGraphs.Count)
            {
                return new Point3D(0.0, 0.0, 0.0);
            }

            double centerX = sumX / (_scatterGraphs.Count - skippedGraphsCount);
            double centerY = sumY / (_scatterGraphs.Count - skippedGraphsCount);
            double centerZ = sumZ / (_scatterGraphs.Count - skippedGraphsCount);

            return new Point3D(centerX, centerY, centerZ);
        }

        public void SetOriginToBarycenter()
        {
            OriginBase.UpdateOrigin(_barycenter.Point.Point);
        }

        public abstract Base3D FindNeareatBase(Point3D point);

        public virtual bool IsBelongingToModel(GeometryModel3D geometryModel3D)
        {
            return _barycenter.IsBelongingToModel(geometryModel3D);
        }

        public virtual void UpdateModelGeometry()
        {
            throw new InvalidOperationException();
        }

        public virtual void UpdateModelMaterial()
        {
            throw new InvalidOperationException();
        }

        protected void AddBase(Base3DViewModel base3DViewModel)
        {
            // TODO: tester si le wrapper a bien fonctionné
            _bases.Add(base3DViewModel);
            base3DViewModel.IsHiddenChanged += Base3DViewModel_IsHiddenChanged;
            OnPropertyChanged(nameof(ItemsCount));
            OnPropertyChanged(nameof(Bases));
        }

        private void ClearBases()
        {
            foreach (Base3DViewModel base3DViewModel in _bases)
            {
                base3DViewModel.IsHiddenChanged -= Base3DViewModel_IsHiddenChanged;
                base3DViewModel.Dispose();
            }
            OnPropertyChanged(nameof(ItemsCount));
            OnPropertyChanged(nameof(Bases));
        }
    }
}

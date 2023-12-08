using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public abstract class PartViewModelBase : ViewModelBase, I3DElement
    {
        public event EventHandler<bool>? IsHiddenChanged;

        private string _name;
        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value);
        }

        private BarycenterViewModel _barycenter;
        public BarycenterViewModel Barycenter
        {
            get => _barycenter;
            private set => SetValue(ref _barycenter, value);
        }

        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                if (SetValue(ref _isHidden, value))
                {
                    foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
                    {
                        scatterGraphViewModel.IsHidden = _isHidden;
                    }
                    IsHiddenChanged?.Invoke(this, _isHidden);
                }
            }
        }

        public ObservableCollection<ScatterGraphViewModel> ScatterGraphs { get; private set; }

        public Base3DViewModel OriginBase { get; private set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            private set
            {
                if (SetValue(ref _isSelected, value))
                {
                    if (_isSelected)
                    {
                        foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
                        {
                            scatterGraphViewModel.Select();
                        }
                    }
                    else
                    {
                        foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
                        {
                            scatterGraphViewModel.Unselect();
                        }
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
                if (SetValue(ref _scaleFactor, value))
                {
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

        private static uint instanceCreated;

        public PartViewModelBase(double scaleFactor = 1.0, RenderQuality renderQuality = RenderQuality.High)
        {
            instanceCreated++;
            _name = "Part " + instanceCreated;
            _isHidden = false;
            _scaleFactor = scaleFactor;
            _renderQuality = renderQuality;
            _isMouseOver = false;
            _isSelected = false;
            _modelGroup = new Model3DGroup();
            ScatterGraphs = new ObservableCollection<ScatterGraphViewModel>();
            _barycenter = new BarycenterViewModel();
            OriginBase = new Base3DViewModel(new Base3D(_barycenter.Point.Point));

            _modelGroup.Children.Add(_barycenter.Model);
            _modelGroup.Children.Add(OriginBase.Model);

            ScatterGraphs.CollectionChanged += ScatterGraphs_CollectionChanged;
        }

        public PartViewModelBase(IEnumerable<ScatterGraphViewModel> scatterGraphs) : this()
        {
            ScatterGraphs.CollectionChanged -= ScatterGraphs_CollectionChanged;
            foreach (var item in scatterGraphs)
            {
                item.Barycenter.PropertyChanged += ItemBarycenter_PropertyChanged;
                item.IsHiddenChanged += ScatterGraphViewModel_IsHiddenChanged;
                ScatterGraphs.Add(item);
            }
            ComputeAll();
            ScatterGraphs.CollectionChanged += ScatterGraphs_CollectionChanged;
        }

        ~PartViewModelBase()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                Clear();
                ScatterGraphs.CollectionChanged -= ScatterGraphs_CollectionChanged;
                _modelGroup.Children.Clear();
                base.Dispose();
                IsDisposed = true;
            }
        }

        private void ScatterGraphs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object? item in e.NewItems)
                    {
                        ScatterGraphViewModel scatterGraphViewModel = (ScatterGraphViewModel)item;
                        if (!Equals(scatterGraphViewModel.Part))
                        {
                            scatterGraphViewModel.Part = this;
                            scatterGraphViewModel.Barycenter.PropertyChanged += ItemBarycenter_PropertyChanged;
                            scatterGraphViewModel.IsHiddenChanged += ScatterGraphViewModel_IsHiddenChanged;
                        }
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
                    }
                    break;
            }
            ComputeAll();
        }

        private void ScatterGraphViewModel_IsHiddenChanged(object sender, bool e)
        {
            if ((_isHidden && ScatterGraphs.Any(x => !x.IsHidden)) ||
                (!_isHidden && ScatterGraphs.All(x => x.IsHidden)))
            {
                _isHidden = !_isHidden;
                OnPropertyChanged(nameof(IsHidden));
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
            if (!ScatterGraphs.Contains(scatterGraphViewModel))
            {
                ScatterGraphs.Add(scatterGraphViewModel);
            }
        }

        public void Remove(ScatterGraphViewModel scatterGraphViewModel)
        {
            ScatterGraphs.Remove(scatterGraphViewModel);
        }

        public void Clear()
        {
            foreach (ScatterGraphViewModel item in ScatterGraphs)
            {
                item.Barycenter.PropertyChanged -= ItemBarycenter_PropertyChanged;
                item.IsHiddenChanged -= ScatterGraphViewModel_IsHiddenChanged;
            }
            ScatterGraphs.Clear();
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
        protected virtual void ComputeAll(bool setOriginToBarycenter = true)
        {
            ComputeBarycenter();
            _barycenter.UpdatePoint(ComputeBarycenter());
            if (setOriginToBarycenter)
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
            if (ScatterGraphs.Count == 0)
            {
                return new Point3D(0.0, 0.0, 0.0);
            }
            Point3D point;

            double sumX = 0.0;
            double sumY = 0.0;
            double sumZ = 0.0;

            int skippedGraphsCount = 0;
            foreach (var item in ScatterGraphs)
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

            if (skippedGraphsCount == ScatterGraphs.Count)
            {
                return new Point3D(0.0, 0.0, 0.0);
            }

            double centerX = sumX / (ScatterGraphs.Count - skippedGraphsCount);
            double centerY = sumY / (ScatterGraphs.Count - skippedGraphsCount);
            double centerZ = sumZ / (ScatterGraphs.Count - skippedGraphsCount);

            return new Point3D(centerX, centerY, centerZ);
        }

        public void SetOriginToBarycenter()
        {
            OriginBase.UpdateOrigin(_barycenter.Point.Point);
        }

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
    }
}

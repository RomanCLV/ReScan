using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels.Parts;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class ScatterGraphGroupViewModel : ViewModelBase, IEnumerable<ScatterGraphViewModel>
    {
        private readonly static string[] scatterGraphPropertyNames = new string[]
        {
            nameof(ScatterGraphViewModel.ScaleFactor),
            nameof(ScatterGraphViewModel.Part),
            nameof(ScatterGraphViewModel.PointsRadius),
            nameof(ScatterGraphViewModel.IsHidden),
            nameof(ScatterGraphViewModel.ArePointsHidden),
            nameof(ScatterGraphViewModel.Color.Color),
        };

        private readonly static string[] barycenterPropertyNames = new string[]
        {
            nameof(ScatterGraphViewModel.Barycenter.IsHidden),
            nameof(ScatterGraphViewModel.Barycenter.Radius),
            nameof(ScatterGraphViewModel.Barycenter.Color),
        };

        private readonly static string[] averagePlanPropertyNames = new string[]
        {
            nameof(ScatterGraphViewModel.AveragePlan.IsHidden),
            nameof(ScatterGraphViewModel.AveragePlan.Color),
        };

        private readonly static string[] base3DPropertyNames = new string[]
        {
            nameof(ScatterGraphViewModel.Base3D.IsHidden),
            nameof(ScatterGraphViewModel.Base3D.AxisScaleFactor),
            nameof(ScatterGraphViewModel.Base3D.Opacity),
        };

        private bool _inhibitUpdate;

        #region Translate

        private double _translateX;
        public double TranslateX
        {
            get => _translateX;
            set => SetValue(ref _translateX, value);
        }

        private double _translateY;
        public double TranslateY
        {
            get => _translateY;
            set => SetValue(ref _translateY, value);
        }

        private double _translateZ;
        public double TranslateZ
        {
            get => _translateZ;
            set => SetValue(ref _translateZ, value);
        }

        #endregion

        #region Rotate

        public List<RotationAxis> AllRotationAxis { get; private set; }

        private RotationAxis _rotationAxis;
        public RotationAxis RotationAxis
        {
            get => _rotationAxis;
            set => SetValue(ref _rotationAxis, value);
        }

        private double _rotationAngle;
        public double RotationAngle
        {
            get => _rotationAngle;
            set
            {
                if (SetValue(ref _rotationAngle, value))
                {
                    RotateBase();
                }
            }
        }

        #endregion


        public ScatterGraphViewModel this[int index]
        {
            get => _items[index];
        }

        private readonly List<ScatterGraphViewModel> _items;
        public IEnumerable<ScatterGraphViewModel> Items
        {
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value), "Items collection can't be null");
                }
                Clear();
                _items.Capacity = value.Count();
                _items.AddRange(value);
                foreach (var item in _items)
                {
                    AddPropertyChangedCallback(item);
                }
                UpdateFromSource();
            }
        }

        public double Count => _items.Count;

        public double SamplesCount => _items.Sum(x => x.Samples.Count);

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

        private PartViewModelBase? _part;
        public PartViewModelBase? Part
        {
            get => _part;
            set
            {
                if (SetValue(ref _part, value))
                {
                    foreach (var item in _items)
                    {
                        item.Part = value;
                    }
                }
            }
        }

        private double? _scaleFactor;
        public double? ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Scale factor must be greater than 0.");
                }
                if (SetValue(ref _scaleFactor, value) && _scaleFactor != null)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.ScaleFactor = (double)_scaleFactor;
                    }
                    _inhibitUpdate = false;
                }
            }
        }

        private double? _axisScaleFactor;
        public double? AxisScaleFactor
        {
            get => _axisScaleFactor;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Axis scale factor must be greater than 0.");
                }
                if (SetValue(ref _axisScaleFactor, value) && _axisScaleFactor != null)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Base3D.AxisScaleFactor = (double)_axisScaleFactor;
                    }
                    _inhibitUpdate = false;
                }
            }
        }

        private double? _pointsRadius;
        public double? PointsRadius
        {
            get => _pointsRadius;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Points radius must be greater than 0.");
                }
                if (SetValue(ref _pointsRadius, value) && _pointsRadius != null)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.PointsRadius = (double)_pointsRadius;
                    }
                    _inhibitUpdate = false;
                }
            }
        }

        private double? _barycenterRadius;
        public double? BarycenterRadius
        {
            get => _barycenterRadius;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Barycenter radius must be greater than 0.");
                }
                if (SetValue(ref _barycenterRadius, value) && _barycenterRadius != null)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Barycenter.Radius = (double)_barycenterRadius;
                    }
                    _inhibitUpdate = false;
                }
            }
        }

        private bool _isPointsColorDefined;
        private bool _isBarycenterColorDefined;
        private bool _isPlanColorDefined;
        public ColorViewModel PointsColorViewModel { get; set; }

        public ColorViewModel BarycentersColorViewModel { get; set; }

        public ColorViewModel PlansColorViewModel { get; set; }

        private byte? _baseOpacity;
        public byte? BaseOpacity
        {
            get => _baseOpacity;
            set
            {
                if (SetValue(ref _baseOpacity, value) && _baseOpacity != null)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Base3D.Opacity = (byte)_baseOpacity;
                    }
                    _inhibitUpdate = false;
                }
            }
        }

        private RenderQuality? _renderQuality;
        public RenderQuality? RenderQuality
        {
            get => _renderQuality;
            set
            {
                if (SetValue(ref _renderQuality, value) && _renderQuality != null)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.RenderQuality = (RenderQuality)_renderQuality;
                    }
                    _inhibitUpdate = false;
                }
            }
        }

        public List<RenderQuality> RenderQualities { get; }

        private bool? _areHidden;
        public bool? AreHidden
        {
            get => _areHidden;
            set
            {
                if (SetValue(ref _areHidden, value) && _areHidden != null)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.IsHidden = (bool)_areHidden;
                    }
                    _inhibitUpdate = false;
                }
            }
        }

        private bool? _arePointsHidden;
        public bool? ArePointsHidden
        {
            get => _arePointsHidden;
            set
            {
                if (SetValue(ref _arePointsHidden, value) && _arePointsHidden != null)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        if ((bool)_arePointsHidden)
                        {
                            item.HidePoints();
                        }
                        else
                        {
                            item.ShowPoints();
                        }
                    }
                    _inhibitUpdate = false;
                }
            }
        }

        private bool? _areBarycentersHidden;
        public bool? AreBarycentersHidden
        {
            get => _areBarycentersHidden;
            set
            {
                if (SetValue(ref _areBarycentersHidden, value) && _areBarycentersHidden != null)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Barycenter.IsHidden = (bool)_areBarycentersHidden;
                    }
                    _inhibitUpdate = false;
                }
            }
        }

        private bool? _arePlansHidden;
        public bool? ArePlansHidden
        {
            get => _arePlansHidden;
            set
            {
                if (SetValue(ref _arePlansHidden, value) && _arePlansHidden != null)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.AveragePlan.IsHidden = (bool)_arePlansHidden;
                    }
                    _inhibitUpdate = false;
                }
            }
        }

        private bool? _areBasesHidden;
        public bool? AreBasesHidden
        {
            get => _areBasesHidden;
            set
            {
                if (SetValue(ref _areBasesHidden, value) && _areBasesHidden != null)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Base3D.IsHidden = (bool)_areBasesHidden;
                    }
                    _inhibitUpdate = false;
                }
            }
        }

        public ScatterGraphGroupViewModel()
        {
            _inhibitUpdate = false;
            _items = new List<ScatterGraphViewModel>();
            _scaleFactor = null;
            _part = null;
            _axisScaleFactor = null;
            _pointsRadius = null;
            _barycenterRadius = null;
            _isPointsColorDefined = false;
            _isBarycenterColorDefined = false;
            _isPlanColorDefined = false;
            PointsColorViewModel = new ColorViewModel(Colors.Transparent);
            BarycentersColorViewModel = new ColorViewModel(Colors.Transparent);
            PlansColorViewModel = new ColorViewModel(Colors.Transparent);
            _baseOpacity = null;
            _areHidden = null;
            _arePointsHidden = null;
            _areBarycentersHidden = null;
            _arePlansHidden = null;
            _areBasesHidden = null;
            _renderQuality = null;
            RenderQualities = Tools.GetRenderQualitiesList();

            // translate
            _translateX = 0.0;
            _translateY = 0.0;
            _translateZ = 0.0;

            // rotate
            AllRotationAxis = Tools.GetRotationAxesList();
            _rotationAxis = RotationAxis.Z;
            _rotationAngle = 0;

            UpdateFromSource();

            PointsColorViewModel.PropertyChanged += PointsColorViewModel_PropertyChanged;
            BarycentersColorViewModel.PropertyChanged += BarycenterColorViewModel_PropertyChanged;
            PlansColorViewModel.PropertyChanged += PlanColorViewModel_PropertyChanged;
        }

        public ScatterGraphGroupViewModel(IEnumerable<ScatterGraphViewModel> source) : this()
        {
            Items = source;
        }

        ~ScatterGraphGroupViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                PartsListSource = null;
                PointsColorViewModel.PropertyChanged -= PointsColorViewModel_PropertyChanged;
                BarycentersColorViewModel.PropertyChanged -= BarycenterColorViewModel_PropertyChanged;
                PlansColorViewModel.PropertyChanged -= PlanColorViewModel_PropertyChanged;
                PointsColorViewModel.Dispose();
                BarycentersColorViewModel.Dispose();
                PlansColorViewModel.Dispose();
                EndRotateBase();
                _items.Clear();
                base.Dispose();
            }
        }

        private void SourceParts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PartsListSource));
        }

        public void Add(ScatterGraphViewModel item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
                AddPropertyChangedCallback(item);
                OnPropertyChanged(nameof(Count));
                UpdateFromSource();
            }
        }

        public void Remove(ScatterGraphViewModel item)
        {
            if (_items.Contains(item))
            {
                _items.Remove(item);
                RemovePropertyChangedCallback(item);
                OnPropertyChanged(nameof(Count));
                UpdateFromSource();
            }
        }

        public void Clear(bool unselectAll = true)
        {
            if (_items.Count > 0)
            {
                if (unselectAll)
                {
                    UnselectAll();
                }
                foreach (var item in _items)
                {
                    RemovePropertyChangedCallback(item);
                }
                _items.Clear();
                OnPropertyChanged(nameof(Count));
                UpdateFromSource();
            }
        }

        private void PointsColorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ColorViewModel.Color))
            {
                if (_isPointsColorDefined ||
                    PointsColorViewModel.Color != Colors.Transparent)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Color.Set(PointsColorViewModel.Color);
                    }
                    _inhibitUpdate = false;
                    _isPointsColorDefined = true;
                }
            }
        }

        private void BarycenterColorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ColorViewModel.Color))
            {
                if (_isBarycenterColorDefined ||
                    BarycentersColorViewModel.Color != Colors.Transparent)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Barycenter.Color.Set(BarycentersColorViewModel.Color);
                    }
                    _inhibitUpdate = false;
                    _isBarycenterColorDefined = true;
                }
            }
        }

        private void PlanColorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ColorViewModel.Color))
            {
                if (_isPlanColorDefined ||
                    PlansColorViewModel.Color != Colors.Transparent)
                {
                    _inhibitUpdate = true;
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.AveragePlan.Color.Set(PlansColorViewModel.Color);
                    }
                    _inhibitUpdate = false;
                    _isPlanColorDefined = true;
                }
            }
        }

        private void AddPropertyChangedCallback(ScatterGraphViewModel item)
        {
            item.PropertyChanged += Item_PropertyChanged;
            item.Color.PropertyChanged += Item_PropertyChanged;
            item.Barycenter.PropertyChanged += Barycenter_PropertyChanged;
            item.Barycenter.Color.PropertyChanged += Barycenter_PropertyChanged;
            item.AveragePlan.PropertyChanged += AveragePlan_PropertyChanged;
            item.AveragePlan.Color.PropertyChanged += AveragePlan_PropertyChanged;
            item.Base3D.PropertyChanged += Base3D_PropertyChanged;
        }

        private void RemovePropertyChangedCallback(ScatterGraphViewModel item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            item.Color.PropertyChanged -= Item_PropertyChanged;
            item.Barycenter.PropertyChanged -= Barycenter_PropertyChanged;
            item.Barycenter.Color.PropertyChanged -= Barycenter_PropertyChanged;
            item.AveragePlan.PropertyChanged -= AveragePlan_PropertyChanged;
            item.AveragePlan.Color.PropertyChanged -= AveragePlan_PropertyChanged;
            item.Base3D.PropertyChanged -= Base3D_PropertyChanged;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (scatterGraphPropertyNames.Contains(e.PropertyName))
            {
                UpdateFromSource();
            }
        }

        private void Barycenter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (barycenterPropertyNames.Contains(e.PropertyName))
            {
                UpdateFromSource();
            }
        }

        private void AveragePlan_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (averagePlanPropertyNames.Contains(e.PropertyName))
            {
                UpdateFromSource();
            }
        }

        private void Base3D_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (base3DPropertyNames.Contains(e.PropertyName))
            {
                UpdateFromSource();
            }
        }

        public void SelectAll()
        {
            _items.ForEach(x => x.Select());
        }

        public void UnselectAll()
        {
            _items.ForEach(x => x.Unselect());
        }

        private void UpdateFromSource()
        {
            if (_inhibitUpdate)
            {
                return;
            }
            if (_items is null || _items.Count() == 0)
            {
                ScaleFactor = null;
                Part = null;
                AxisScaleFactor = null;
                PointsRadius = null;
                BarycenterRadius = null;
                _isPointsColorDefined = false;
                _isBarycenterColorDefined = false;
                _isPlanColorDefined = false;
                PointsColorViewModel.Set(Colors.Transparent);
                BarycentersColorViewModel.Set(Colors.Transparent);
                PlansColorViewModel.Set(Colors.Transparent);
                BaseOpacity = null;
                AreHidden = null;
                ArePointsHidden = null;
                AreBarycentersHidden = null;
                ArePlansHidden = null;
                AreBasesHidden = null;
                RenderQuality = null;
                return;
            }

            UpdateProperty(x => x.ScaleFactor, ref _scaleFactor, nameof(ScaleFactor));
            UpdatePropertyPart(x => x.Part, ref _part, nameof(Part));
            UpdateProperty(x => x.Base3D.AxisScaleFactor, ref _axisScaleFactor, nameof(AxisScaleFactor));
            UpdateProperty(x => x.PointsRadius, ref _pointsRadius, nameof(PointsRadius));
            UpdateProperty(x => x.Barycenter.Radius, ref _barycenterRadius, nameof(BarycenterRadius));
            UpdatePropertyColor(x => x.Color.Color, PointsColorViewModel, ref _isPointsColorDefined);
            UpdatePropertyColor(x => x.Barycenter.Color.Color, BarycentersColorViewModel, ref _isBarycenterColorDefined);
            UpdatePropertyColor(x => x.AveragePlan.Color.Color, PlansColorViewModel, ref _isPlanColorDefined);
            UpdateProperty(x => x.Base3D.Opacity, ref _baseOpacity, nameof(BaseOpacity));
            UpdateProperty(x => x.IsHidden, ref _areHidden, nameof(AreHidden));
            UpdateProperty(x => x.ArePointsHidden, ref _arePointsHidden, nameof(ArePointsHidden));
            UpdateProperty(x => x.Barycenter.IsHidden, ref _areBarycentersHidden, nameof(AreBarycentersHidden));
            UpdateProperty(x => x.AveragePlan.IsHidden, ref _arePlansHidden, nameof(ArePlansHidden));
            UpdateProperty(x => x.Base3D.IsHidden, ref _areBasesHidden, nameof(AreBasesHidden));
            UpdateProperty(x => x.RenderQuality, ref _renderQuality, nameof(RenderQuality));
        }

        private void UpdateProperty<T>(Func<ScatterGraphViewModel, T> getValue, ref T? backfield, string propertyName) where T : struct, IComparable<T>
        {
            T firstValue = getValue(_items.First());
            if (_items.All(x => firstValue.CompareTo(getValue(x)) == 0))
            {
                backfield = firstValue;
                OnPropertyChanged(propertyName);
            }
            else
            {
                if (backfield != null)
                {
                    backfield = null;
                    OnPropertyChanged(propertyName);
                }
            }
        }

        private void UpdateProperty(Func<ScatterGraphViewModel, RenderQuality> getValue, ref RenderQuality? backfield, string propertyName)
        {
            RenderQuality firstValue = getValue(_items.First());
            if (_items.All(x => firstValue == getValue(x)))
            {
                backfield = firstValue;
                OnPropertyChanged(propertyName);
            }
            else
            {
                if (backfield != null)
                {
                    backfield = null;
                    OnPropertyChanged(propertyName);
                }
            }
        }

        private void UpdatePropertyPart(Func<ScatterGraphViewModel, PartViewModelBase?> getValue, ref PartViewModelBase? backfield, string propertyName)
        {
            PartViewModelBase? firstValue = getValue(_items.First());
            if (_items.All(x => firstValue == getValue(x)))
            {
                backfield = firstValue;
                OnPropertyChanged(propertyName);
            }
            else
            {
                if (backfield != null)
                {
                    backfield = null;
                    OnPropertyChanged(propertyName);
                }
            }
        }

        private void UpdatePropertyColor(Func<ScatterGraphViewModel, Color> getValue, ColorViewModel backfield, ref bool isColorDefined)
        {
            Color firstValue = getValue(_items.First());
            if (_items.All(x => firstValue == getValue(x)))
            {
                isColorDefined = true;
                backfield.Set(firstValue);
            }
            else
            {
                isColorDefined = false;
                backfield.Set(Colors.Transparent);
            }
        }

        public IEnumerator<ScatterGraphViewModel> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void InverseAreHidden()
        {
            AreHidden = _areHidden is null ? true : !_areHidden;
        }

        public void InverseAreBarycentersHidden()
        {
            AreBarycentersHidden = _areBarycentersHidden is null ? true : !_areBarycentersHidden;
        }

        public void InverseArePlansHidden()
        {
            ArePlansHidden = _arePlansHidden is null ? true : !_arePlansHidden;
        }

        public void InverseAreBasesHidden()
        {
            AreBasesHidden = _areBasesHidden is null ? true : !_areBasesHidden;
        }

        public void InverseArePointsHidden()
        {
            ArePointsHidden = _arePointsHidden is null ? true : !_arePointsHidden;
        }

        private void RotateBase()
        {
            foreach (var item in _items)
            {
                item.Base3D.Rotate(item.Base3D.Z, _rotationAngle, false);
            }
        }

        public void EndRotateBase()
        {
            foreach (var item in _items)
            {
                item.Base3D.EndRotate();
            }
            _rotationAngle = 0.0;
            OnPropertyChanged(nameof(RotationAngle));
        }

        public void Flip()
        {
            foreach (var item in _items)
            {
                item.Base3D.Flip();
            }
        }

        public void RotateNDegree(double degree)
        {
            foreach (var item in _items)
            {
                item.Base3D.Rotate(item.Base3D.Z, degree);
            }
        }
    }
}

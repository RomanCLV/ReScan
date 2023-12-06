using ReScanVisualizer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class ScatterGraphGroupViewModel : ViewModelBase, IEnumerable<ScatterGraphViewModel>
    {
        // TODO : renforcer la consistance des données si changement de propriété d'un item

        public ScatterGraphViewModel this[int index]
        {
            get => _items[index];
        }


        private readonly List<ScatterGraphViewModel> _items;
        public IEnumerable<ScatterGraphViewModel> Items
        {
            set
            {
                _items.Clear();
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
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.ScaleFactor = (double)_scaleFactor;
                    }
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
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Base3D.AxisScaleFactor = (double)_axisScaleFactor;
                    }
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
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.PointsRadius = (double)_pointsRadius;
                    }
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
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Base3D.Opacity = (byte)_baseOpacity;
                    }
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
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.RenderQuality = (RenderQuality)_renderQuality;
                    }
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
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.IsHidden = (bool)_areHidden;
                    }
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
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Barycenter.IsHidden = (bool)_areBarycentersHidden;
                    }
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
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.AveragePlan.IsHidden = (bool)_arePlansHidden;
                    }
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
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Base3D.IsHidden = (bool)_areBasesHidden;
                    }
                }
            }
        }

        public ScatterGraphGroupViewModel()
        {
            _items = new List<ScatterGraphViewModel>();
            _scaleFactor = null;
            _axisScaleFactor = null;
            _pointsRadius = null;
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
                PointsColorViewModel.PropertyChanged -= PointsColorViewModel_PropertyChanged;
                BarycentersColorViewModel.PropertyChanged -= BarycenterColorViewModel_PropertyChanged;
                PlansColorViewModel.PropertyChanged -= PlanColorViewModel_PropertyChanged;
                base.Dispose();
                IsDisposed = true;
            }
        }

        private void PointsColorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ColorViewModel.Color))
            {
                if (_isPointsColorDefined ||
                    PointsColorViewModel.Color != Colors.Transparent)
                {
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Color.Set(PointsColorViewModel.Color);
                    }
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
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.Barycenter.Color.Set(BarycentersColorViewModel.Color);
                    }
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
                    foreach (ScatterGraphViewModel item in _items)
                    {
                        item.AveragePlan.Color.Set(PlansColorViewModel.Color);
                    }
                    _isPlanColorDefined = true;
                }
            }
        }

        public void Add(ScatterGraphViewModel item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
                AddPropertyChangedCallback(item);
                UpdateFromSource();
            }
        }

        public void Remove(ScatterGraphViewModel item)
        {
            if (_items.Contains(item))
            {
                _items.Remove(item);
                RemovePropertyChangedCallback(item);
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
                UpdateFromSource();
            }
        }

        private void AddPropertyChangedCallback(ScatterGraphViewModel item)
        {
            item.PropertyChanged += Item_PropertyChanged;
            item.Barycenter.PropertyChanged += Barycenter_PropertyChanged;
            item.Base3D.PropertyChanged += Base3D_PropertyChanged;
            item.AveragePlan.PropertyChanged += AveragePlan_PropertyChanged;
        }

        private void RemovePropertyChangedCallback(ScatterGraphViewModel item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            item.Barycenter.PropertyChanged -= Barycenter_PropertyChanged;
            item.Base3D.PropertyChanged -= Base3D_PropertyChanged;
            item.AveragePlan.PropertyChanged -= AveragePlan_PropertyChanged;
        }

        private void AveragePlan_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string[] propertiesName = new string[]
                {
                    nameof(ScatterGraphViewModel.)
                };
        }

        private void Base3D_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Barycenter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
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
            if (_items is null || _items.Count() == 0)
            {
                ScaleFactor = null;
                AxisScaleFactor = null;
                PointsRadius = null;
                _isPointsColorDefined = false;
                _isBarycenterColorDefined = false;
                _isPlanColorDefined = false;
                PointsColorViewModel.Set(Colors.Transparent);
                BarycentersColorViewModel.Set(Colors.Transparent);
                PlansColorViewModel.Set(Colors.Transparent);
                BaseOpacity = null;
                AreHidden = null;
                ArePointsHidden = null;
                ArePlansHidden = null;
                AreBasesHidden = null;
                RenderQuality = null;
                return;
            }

            UpdateProperty(x => x.ScaleFactor, ref _scaleFactor, nameof(ScaleFactor));
            UpdateProperty(x => x.Base3D.AxisScaleFactor, ref _axisScaleFactor, nameof(AxisScaleFactor));
            UpdateProperty(x => x.PointsRadius, ref _pointsRadius, nameof(PointsRadius));
            UpdatePropertyColor(x => x.Color.Color, PointsColorViewModel, ref _isPointsColorDefined);
            UpdatePropertyColor(x => x.Barycenter.Color.Color, BarycentersColorViewModel, ref _isBarycenterColorDefined);
            UpdatePropertyColor(x => x.AveragePlan.Color.Color, PlansColorViewModel, ref _isPlanColorDefined);
            UpdateProperty(x => x.Base3D.Opacity, ref _baseOpacity, nameof(BaseOpacity));
            UpdateProperty(x => x.IsHidden, ref _areHidden, nameof(AreHidden));
            UpdateProperty(x => x.ArePointsHidden, ref _arePointsHidden, nameof(ArePointsHidden));
            UpdateProperty(x => x.AveragePlan.IsHidden, ref _arePlansHidden, nameof(ArePlansHidden));
            UpdateProperty(x => x.Barycenter.IsHidden, ref _areBarycentersHidden, nameof(AreBarycentersHidden));
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
    }
}

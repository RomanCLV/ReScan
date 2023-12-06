using ReScanVisualizer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class ScatterGraphViewModelGroup : ViewModelBase, IEnumerable<ScatterGraphViewModel>
    {
        private readonly List<ScatterGraphViewModel> _items;
        public IEnumerable<ScatterGraphViewModel> Items
        {
            set
            {
                _items.Clear();
                _items.Capacity = value.Count();
                _items.AddRange(value);
                UpdateFromSource();
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
                    _items.AsParallel().ForAll(x => x.ScaleFactor = (double)_scaleFactor);
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
                    _items.AsParallel().ForAll(x => x.Base3D.AxisScaleFactor = (double)_axisScaleFactor);
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
                    _items.AsParallel().ForAll(x => x.PointsRadius = (double)_pointsRadius);
                }
            }
        }

        private Color? _pointsColor;
        public Color? PointsColor
        {
            get => _pointsColor;
            set
            {
                if (SetValue(ref _pointsColor, value) && _pointsColor != null)
                {
                    _items.AsParallel().ForAll(x => x.Color.Set((Color)_pointsColor));
                    PointsColorViewModel.Set((Color)_pointsColor);
                }
            }
        }

        private Color? _barycenterColor;
        public Color? BarycenterColor
        {
            get => _barycenterColor;
            set
            {
                if (SetValue(ref _barycenterColor, value) && _barycenterColor != null)
                {
                    _items.AsParallel().ForAll(x => x.Barycenter.Color.Set((Color)_barycenterColor));
                    BarycenterColorViewModel.Set((Color)_barycenterColor);
                }
            }
        }

        private Color? _planColor;
        public Color? PlanColor
        {
            get => _planColor;
            set
            {
                if (SetValue(ref _planColor, value) && _planColor != null)
                {
                    _items.AsParallel().ForAll(x => x.AveragePlan.Color.Set((Color)_planColor));
                    PlanColorViewModel.Set((Color)_planColor);
                }
            }
        }

        public ColorViewModel PointsColorViewModel { get; set; }

        public ColorViewModel BarycenterColorViewModel { get; set; }

        public ColorViewModel PlanColorViewModel { get; set; }

        private byte? _baseOpacity;
        public byte? BaseOpacity
        {
            get => _baseOpacity;
            set
            {
                if (SetValue(ref _baseOpacity, value) && _baseOpacity != null)
                {
                    _items.AsParallel().ForAll(x => x.Base3D.Opacity = (byte)_baseOpacity);
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
                    _items.AsParallel().ForAll(x => x.RenderQuality = (RenderQuality)_renderQuality);
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
                    _items.AsParallel().ForAll(x => x.IsHidden = (bool)_areHidden);
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
                    _items.AsParallel().ForAll(x =>
                    {
                        if ((bool)_arePointsHidden)
                        {
                            x.HidePoints();
                        }
                        else
                        {
                            x.ShowPoints();
                        }
                    });
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
                    _items.AsParallel().ForAll(x => x.Barycenter.IsHidden = (bool)_areBarycentersHidden);
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
                    _items.AsParallel().ForAll(x => x.AveragePlan.IsHidden = (bool)_arePlansHidden);
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
                    _items.AsParallel().ForAll(x => x.Base3D.IsHidden = (bool)_areBasesHidden);
                }
            }
        }

        public ScatterGraphViewModelGroup()
        {
            _items = new List<ScatterGraphViewModel>();
            _scaleFactor = null;
            _axisScaleFactor = null;
            _pointsRadius = null;
            _pointsColor = null;
            _barycenterColor = null;
            _planColor = null;
            PointsColorViewModel = new ColorViewModel(Colors.Transparent);
            BarycenterColorViewModel = new ColorViewModel(Colors.Transparent);
            PlanColorViewModel = new ColorViewModel(Colors.Transparent);
            _baseOpacity = null;
            _areHidden = null;
            _arePointsHidden = null;
            _areBarycentersHidden = null;
            _arePlansHidden = null;
            _areBasesHidden = null;
            _renderQuality = null;
            RenderQualities = Tools.GetRenderQualitiesList();
            UpdateFromSource();
        }

        public ScatterGraphViewModelGroup(IEnumerable<ScatterGraphViewModel> source) : this()
        {
            Items = source;
        }

        public void Add(ScatterGraphViewModel item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
            }
        }

        public int Count()
        {
            return _items.Count;
        }

        public void Clear(bool unselectAll = true)
        {
            if (_items.Count > 0)
            {
                if (unselectAll)
                {
                    UnselectAll();
                }
                _items.Clear();
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
            if (_items is null || _items.Count() == 0)
            {
                ScaleFactor = null;
                AxisScaleFactor = null;
                PointsRadius = null;
                PointsColor = null;
                BarycenterColor = null;
                PlanColor = null;
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
            UpdateProperty(x => x.Color.Color, ref _pointsColor, nameof(PointsColor));
            UpdateProperty(x => x.Barycenter.Color.Color, ref _barycenterColor, nameof(BarycenterColor));
            UpdateProperty(x => x.AveragePlan.Color.Color, ref _planColor, nameof(PlanColor));
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

        private void UpdateProperty(Func<ScatterGraphViewModel, Color> getValue, ref Color? backfield, string propertyName)
        {
            Color firstValue = getValue(_items.First());
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
    }
}

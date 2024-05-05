using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels.Parts;
using ReScanVisualizer.Views.ItemTreeViews;

#nullable enable

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders
{
    public enum ScatterGraphBuilderState
    {
        Ready,
        Working,
        Success,
        Error
    }

    public abstract class ScatterGraphBuilderBase : ViewModelBase
    {
        public const uint   MIN_COUNT =    1;
        public const uint   MAX_COUNT = 1000;

        public const double MIN_X = -10000.0;
        public const double MAX_X =  10000.0;

        public const double MIN_Y = -10000.0;
        public const double MAX_Y =  10000.0;

        public const double MIN_Z = -10000.0;
        public const double MAX_Z =  10000.0;

        public const double MIN_WIDTH = 1.0;
        public const double MAX_WIDTH = 10000.0;

        public const double MIN_HEIGTH = 1.0;
        public const double MAX_HEIGTH = 10000.0;

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (SetValue(ref _color, value) && this is IModelisableBuilder modelisableBuilder)
                {
                    modelisableBuilder.ModelHasToUpdate = true;
                }
            }
        }

        private ScatterGraphBuilderState _state;
        public ScatterGraphBuilderState State
        {
            get => _state;
            protected set
            {
                if (SetValue(ref _state, value))
                {
                    OnPropertyChanged(nameof(IsReady));
                }
            }
        }

        public bool IsReady
        {
            get => _state is ScatterGraphBuilderState.Ready;
        }

        public bool CanBuild
        {
            get => _state is ScatterGraphBuilderState.Ready ||
                _state is ScatterGraphBuilderState.Success;
        }

        private string _message;
        public string Message
        {
            get => _message;
            protected set => SetValue(ref _message, value);
        }

        public virtual string Name => GetType().Name;

        public virtual string FullName => Name;

        public virtual string Details => string.Empty;

        private double _pointRadius;
        public double PointRadius
        {
            get => _pointRadius;
            set
            {
                if (value <= 0.0)
                {
                    value = 0.25;
                }
                SetValue(ref _pointRadius, value);
            }
        }

        private RenderQuality _renderQuality;
        public RenderQuality RenderQuality
        {
            get => _renderQuality;
            set => SetValue(ref _renderQuality, value);
        }

        private bool _displayBarycenter;
        public bool DisplayBarycenter
        {
            get => _displayBarycenter;
            set => SetValue(ref _displayBarycenter, value);
        }

        private bool _displayAveragePlan;
        public bool DisplayAveragePlan
        {
            get => _displayAveragePlan;
            set => SetValue(ref _displayAveragePlan, value);
        }

        private bool _displayBase;
        public bool DisplayBase
        {
            get => _displayBase;
            set => SetValue(ref _displayBase, value);
        }

        private IPartSource? _partsListSource;
        public virtual IPartSource? PartsListSource
        {
            get => _partsListSource;
            set
            {
                if (!Equals(_partsListSource, value))
                {
                    if (_partsListSource != null && _partsListSource.Parts is INotifyCollectionChanged collectionChanged)
                    {
                        collectionChanged.CollectionChanged -= SourceParts_CollectionChanged;
                    }
                }
                if (SetValue(ref _partsListSource, value))
                {
                    if (_partsListSource != null && _partsListSource.Parts is INotifyCollectionChanged collectionChanged)
                    {
                        collectionChanged.CollectionChanged += SourceParts_CollectionChanged;
                    }
                }
            }
        }

        private PartViewModelBase? _part;
        public virtual PartViewModelBase? Part
        {
            get => _part;
            set => SetValue(ref _part, value);
        }

        public List<RenderQuality> RenderQualities { get; }

        public ScatterGraphBuilderBase() : this(Colors.White)
        {
        }

        public ScatterGraphBuilderBase(Color color)
        {
            _state = ScatterGraphBuilderState.Ready;
            _message = string.Empty;
            _color = color;
            _pointRadius = 0.25;
            _renderQuality = RenderQuality.High;
            RenderQualities = new List<RenderQuality>(Tools.GetRenderQualitiesList());
            _displayBarycenter = true;
            _displayAveragePlan = true;
            _displayBase = true;
        }

        ~ScatterGraphBuilderBase()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                PartsListSource = null;
                base.Dispose();
            }
        }

        private void SourceParts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PartsListSource));
        }

        /// <summary>
        /// Build a <see cref="ScatterGraphBuildResult"/>.
        /// </summary>
        public abstract ScatterGraphBuildResult Build();

        public virtual async Task<ScatterGraphBuildResult> BuildAsync()
        {
            return await Task.Run(() => Build());
        }
    }
}

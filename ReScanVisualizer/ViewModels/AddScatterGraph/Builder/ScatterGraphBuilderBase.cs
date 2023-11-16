using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ReScanVisualizer.Models;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
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
            set => SetValue(ref _color, value);
        }

        private ScatterGraphBuilderState _state;
        public ScatterGraphBuilderState State
        {
            get => _state;
            protected set
            {
                if (SetValue(ref _state, value))
                {
                    OnPropertyChanged(nameof(CanBuild));
                }
            }
        }

        public bool CanBuild
        {
            get => _state is ScatterGraphBuilderState.Ready;
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
        }

        /// <summary>
        /// Build a <see cref="ScatterGraphBuildResult"/>.
        /// </summary>
        public abstract ScatterGraphBuildResult Build();

        public async Task<ScatterGraphBuildResult> BuildAsync()
        {
            return await Task.Run(() => Build());
        }
    }
}

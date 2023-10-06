using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ReScanVisualizer.Models;

#nullable enable

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public class ScatterGraphBuildResult : ViewModelBase
    {
        private ScatterGraph? _scatterGraph;
        public ScatterGraph? ScatterGraph
        {
            get => _scatterGraph;
            set
            {
                if (SetValue(ref _scatterGraph, value))
                {
                    OnPropertyChanged(nameof(IsSuccess));
                }
                if (IsSuccess)
                {
                    Exception = null;
                }
            }
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetValue(ref _color, value);
        }

        private Exception? _exception;
        public Exception? Exception
        {
            get => _exception;
            set
            {
                if (value != null)
                {
                    ScatterGraph = null;
                }
                if (SetValue(ref _exception, value))
                {
                    OnPropertyChanged(nameof(IsSuccess));
                }

            }
        }

        public bool IsSuccess
        {
            get => _scatterGraph != null;
        }

        public ScatterGraphBuildResult(Color color, ScatterGraph graph) : this(color, graph, null)
        { 
        }

        public ScatterGraphBuildResult(Color color, Exception ex) : this(color, null, ex)
        {
        }

        public ScatterGraphBuildResult(Color color, ScatterGraph? graph, Exception? ex)
        {
            _color = color;
            _scatterGraph = graph;
            _exception = ex;
        }
    }
}

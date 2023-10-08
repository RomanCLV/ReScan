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

        public ScatterGraphBuildResult(ScatterGraph graph) : this(graph, null)
        { 
        }

        public ScatterGraphBuildResult(Exception ex) : this(null, ex)
        {
        }

        public ScatterGraphBuildResult(ScatterGraph? graph, Exception? ex)
        {
            _scatterGraph = graph;
            _exception = ex;
        }
    }
}

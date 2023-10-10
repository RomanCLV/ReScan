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
                    ComputeMinReductionFactor();
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

        private bool _hasToReduce;
        public bool HasToReduce
        {
            get => _hasToReduce || _hasToReduceForced;
            set
            {
                if (_hasToReduceForced)
                {
                    value = true;
                }
                SetValue(ref _hasToReduce, value);
            }
        }

        private bool _hasToReduceForced;
        public bool HasToReduceForced
        {
            get => _hasToReduceForced;
            set
            {
                if (SetValue(ref _hasToReduceForced, value))
                {
                    OnPropertyChanged(nameof(HasToReduce));
                }
            }
        }

        private double _minReductionFactor;
        public double MinReductionFactor
        {
            get => _minReductionFactor;
            set
            {
                if (SetValue(ref _minReductionFactor, value))
                {
                    if (_reductionFactor < _minReductionFactor)
                    {
                        ReductionFactor = _minReductionFactor;
                    }
                }
            }
        }

        private double _reductionFactor;
        public double ReductionFactor
        {
            get => _reductionFactor;
            set
            {
                if (SetValue(ref _reductionFactor, value))
                {
                    OnPropertyChanged(nameof(ReducedCount));
                    HasToReduce = true;
                }
            }
        }

        public int Count => IsSuccess ? _scatterGraph!.Count : 0;

        public int ReducedCount => IsSuccess ? (int)(_scatterGraph!.Count * (_reductionFactor / 100.0)) : 0;

        private bool _isAdded;
        public bool IsAdded
        {
            get => _isAdded;
            private set => SetValue(ref _isAdded, value);
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
            _hasToReduce = false;
            _reductionFactor = 0.0;
            ComputeMinReductionFactor();
        }

        private void ComputeMinReductionFactor()
        {
            if (_scatterGraph != null)
            {
                if (_scatterGraph.Count > ScatterGraphBuilderBase.MAX_COUNT)
                {
                    MinReductionFactor = ScatterGraphBuilderBase.MAX_COUNT / _scatterGraph.Count;
                    HasToReduceForced = true;
                }
            }
        }

        public void SetAddedToTrue()
        {
            IsAdded = true;
        }
    }
}

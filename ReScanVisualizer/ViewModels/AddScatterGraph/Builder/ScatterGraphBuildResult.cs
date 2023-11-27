using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
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
                if (value != null)
                {
                    Exception = null;
                }
                if (SetValue(ref _scatterGraph, value))
                {
                    OnPropertyChanged(nameof(IsSuccess));
                    OnPropertyChanged(nameof(Count));
                    ComputeMinReductionFactor();
                    OnPropertyChanged(nameof(ReducedCount));
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
                    OnPropertyChanged(nameof(Details));
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
                if (value < _minReductionFactor)
                {
                    value = _minReductionFactor;
                }
                else if (value > 100.0)
                {
                    value = 100.0;
                }
                if (SetValue(ref _reductionFactor, value))
                {
                    OnPropertyChanged(nameof(ReducedCount));
                    HasToReduce = _reductionFactor != 0;
                }
            }
        }

        public int Count => IsSuccess ? _scatterGraph!.Count : 0;

        public int ReducedCount => IsSuccess ? (int)(_scatterGraph!.Count * ((100.0 - _reductionFactor) / 100.0)) : 0;

        private bool _isAdded;
        public bool IsAdded
        {
            get => _isAdded;
            private set => SetValue(ref _isAdded, value);
        }

        private double _farthestPointLength;
        private double _suggestedScaleFactor;

        private double _scaleFactor;
        public double ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (value <= 0.0)
                {
                    value = 1.0;
                }
                SetValue(ref _scaleFactor, value);
            }
        }

        private double _axisScaleFactor;
        public double AxisScaleFactor
        {
            get => _axisScaleFactor;
            set
            {
                if (value <= 0.0)
                {
                    value = 1.0;
                }
                SetValue(ref _axisScaleFactor, value);
            }
        }

        public string Details =>
            (_suggestedScaleFactor == 0.0 ? "" : $"Farthest point from origin: {(int)_farthestPointLength}. Suggested scale factor: {Math.Round(_suggestedScaleFactor, 2)}") +
            (_exception is null ? "" : $"\n{_exception.Message}");

        public ScatterGraphBuildResult() : this(null, null)
        {
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
            _hasToReduceForced = false;
            _hasToReduce = _hasToReduceForced;
            _minReductionFactor = 0.0;
            _reductionFactor = _minReductionFactor;
            _scaleFactor = 1.0;
            _axisScaleFactor = 1.0;
            _suggestedScaleFactor = 0.0;
            _farthestPointLength = 0.0;
            _isAdded = false;
            ComputeMinReductionFactor();
        }

        private void ComputeMinReductionFactor()
        {
            if (_scatterGraph != null)
            {
                if (_scatterGraph.Count > ScatterGraphBuilderBase.MAX_COUNT)
                {
                    MinReductionFactor = 100.0 * ( 1.0 - (double)ScatterGraphBuilderBase.MAX_COUNT / _scatterGraph.Count);
                    HasToReduceForced = true;

                    double factor;
                    while (ReducedCount != ScatterGraphBuilderBase.MAX_COUNT)
                    {
                        factor = ReducedCount < ScatterGraphBuilderBase.MAX_COUNT ? -1 : 1;
                        MinReductionFactor += 0.001 * factor;
                        ReductionFactor = _minReductionFactor;
                    }
                }
            }
        }

        public void ComputeScaleFactor()
        {
            if (IsSuccess)
            {
                _farthestPointLength = ((Vector3D)ScatterGraph.GetFarthestPoint(_scatterGraph!)).Length;
                if (_farthestPointLength < 1.0)
                {
                    _farthestPointLength = 1.0;
                }
                _suggestedScaleFactor = Math.Max(0, 1.0 / (50.0 * (Math.Log10(_farthestPointLength) - 1.0)));
                OnPropertyChanged(nameof(Details));
            }
        }

        public void Reduce()
        {
            if (IsSuccess)
            {
                _scatterGraph!.ReducePercent(ReductionFactor);
                HasToReduce = false;
                HasToReduceForced = false;
                MinReductionFactor = 0.0;
                ReductionFactor = 0.0;
                ComputeMinReductionFactor();
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(nameof(ReducedCount));
                ComputeScaleFactor();
            }
        }

        public void SetAddedToTrue()
        {
            IsAdded = true;
        }

        public void SetFrom(ScatterGraphBuildResult result)
        {
            if (!Equals(result))
            {
                if (result is null)
                {
                    Exception = null;
                    ScatterGraph = null;
                }
                else
                {
                    Exception = result.Exception;
                    ScatterGraph = result.ScatterGraph;
                    ComputeMinReductionFactor();
                    ComputeScaleFactor();
                }
            }
        }
    }
}

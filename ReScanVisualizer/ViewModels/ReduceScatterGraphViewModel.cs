using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.ViewModels
{
    public class ReduceScatterGraphViewModel : ViewModelBase
    {
        private double _reductionPercent;
        public double ReductionPercent
        {
            get => _reductionPercent;
            set
            {
                if (value <= 0.0)
                {
                    value = 0.0;
                }
                else if (value > 100.0)
                {
                    value = 100.0;
                }
                if (SetValue(ref _reductionPercent, value))
                {
                    ReducedCount = (int)(_scatterGraphViewModel.Samples.Count * ((100.0 - _reductionPercent) / 100.0));
                }
            }
        }

        private int _reductionSkipped;
        public int ReductionSkipped
        {
            get => _reductionSkipped;
            set
            {
                if (value <= 0)
                {
                    value = 0;
                }
                else if (value > _scatterGraphViewModel.Samples.Count)
                {
                    value = _scatterGraphViewModel.Samples.Count;
                }
                if (SetValue(ref _reductionSkipped, value))
                {
                    ReducedCount = _scatterGraphViewModel.Samples.Count / _reductionSkipped;
                }
            }
        }

        private int _reductionMaxCount;
        public int ReductionMaxCount
        {
            get => _reductionMaxCount;
            set
            {
                if (value <= 0)
                {
                    value = 0;
                }
                else if (value > _scatterGraphViewModel.Samples.Count)
                {
                    value = _scatterGraphViewModel.Samples.Count;
                }
                if (SetValue(ref _reductionMaxCount, value))
                {
                    ReducedCount = _reductionMaxCount;
                }
            }
        }

        public int Count => _scatterGraphViewModel.Samples.Count;

        private int _reducedCount;
        public int ReducedCount
        {
            get => _reducedCount;
            private set => SetValue(ref  _reducedCount, value);
        }

        private int _selectedMethodeIndex;
        public int SelectedMethodeIndex
        {
            get => _selectedMethodeIndex;
            set
            {
                if (SetValue(ref _selectedMethodeIndex, value))
                {
                    UpdateMessageFromSelectMethodeIndex();
                }
            }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetValue(ref _message, value);
        }

        private readonly ScatterGraphViewModel _scatterGraphViewModel;

        public ReduceScatterGraphViewModel(ScatterGraphViewModel scatterGraphViewModel)
        {
            _scatterGraphViewModel = scatterGraphViewModel;
            _message = string.Empty;
            _selectedMethodeIndex = 0;
            _reductionPercent = 0.0;
            _reductionSkipped = 0;
            _reductionMaxCount = 0;
        }

        private void UpdateMessageFromSelectMethodeIndex()
        {
            switch (_selectedMethodeIndex)
            {
                case 0:
                    Message = $"Reduction percentage (0 - 100):";
                    break;
                case 1:
                    Message = $"Skipped (2 - {_scatterGraphViewModel.Samples.Count}):";
                    break;
                case 2:
                    Message = $"Max points (0 - {_scatterGraphViewModel.Samples.Count}):";
                    break;
            }
        }

        public void Reduce()
        {
            switch (_selectedMethodeIndex)
            {
                case 0:
                    _scatterGraphViewModel.ReducePercent(_reductionPercent);
                    break;
                case 1:
                    _scatterGraphViewModel.Reduce(_reductionSkipped);
                    break;
                case 2:
                    double reductionFactor = 0.0; // TODO : externalize find reduction factor with max count
                    _scatterGraphViewModel.ReducePercent(reductionFactor);
                    break;
            }
        }
    }
}

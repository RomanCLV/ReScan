using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.ViewModels
{
    internal class ExpressionVariableRangeViewModel : ViewModelBase
    {
        private double _min;
        public double Min
        {
            get => _min;
            set
            {
                if (value >= _max)
                {
                    value = _max - _step;
                }
                SetValue(ref _min, value);
            }
        }

        private double _max;
        public double Max
        {
            get => _max;
            set
            {
                if (value <= _min)
                {
                    value = _min + _step;
                }
                SetValue(ref _max, value);
            }
        }

        private double _step;
        public double Step
        {
            get => _step;
            set
            {
                if (value <= 0.0)
                {
                    value = 1.0;
                }
                else if (value < 0.001)
                {
                    value = 0.001;
                }
                else if (value > _max - _min)
                {
                    value = _max - _min;
                }
                SetValue(ref _step, value);
            }
        }

        private string _variableName;
        public string VariableName
        {
            get => _variableName;
            set => SetValue(ref _variableName, value);
        }

        public ExpressionVariableRangeViewModel(string  variableName)
        {
            _variableName = variableName;
            _min = -5.0;
            _max = 5.0;
            _step = 1.0;
        }
    }
}

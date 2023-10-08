using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReScanVisualizer.Commands;
using ReScanVisualizer.Views.AddScatterGraphViews;
using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;

#nullable enable

namespace ReScanVisualizer.ViewModels.AddScatterGraph
{
    public class AddScatterGraphBuilderViewModel : ViewModelBase
    {
        private bool _isUnselectingAll;

        private bool _isEmptySelected;
        public bool IsEmptySelected
        {
            get => _isEmptySelected;
            set
            {
                UnselectAll();
                if (SetValue(ref _isEmptySelected, value))
                {
                    Builder = new ScatterGraphEmptyBuilder();
                }
            }
        }

        private bool _isToPopulateSelected;
        public bool IsToPopulateSelected
        {
            get => _isToPopulateSelected;
            set
            {
                UnselectAll();
                if (SetValue(ref _isToPopulateSelected, value))
                {
                    UpdatePopulateBuilder();
                }
            }
        }

        private bool _isToOpenSelected;
        public bool IsToOpenSelected
        {
            get => _isToOpenSelected;
            set
            {
                UnselectAll();
                if (SetValue(ref _isToOpenSelected, value))
                {
                    Builder = new ScatterGraphFilesBuilder();
                }
            }
        }

        private int _populateSelectedIndex;
        public int PopulateSelectedIndex
        {
            get => _populateSelectedIndex;
            set
            {
                if (SetValue(ref _populateSelectedIndex, value))
                {
                    UpdatePopulateBuilder();
                }
            }
        }

        private ScatterGraphBuilderBase? _builder;
        public ScatterGraphBuilderBase? Builder
        {
            get => _builder;
            set
            {
                if (_builder == null)
                {
                    SetValue(ref _builder, value);
                }
                else
                {
                    if (!_builder.Equals(value))
                    {
                        _builder.Dispose();
                        SetValue(ref _builder, value);
                    }
                }
            }
        }

        public CommandKey ValidateCommand { get; }
        public CommandKey CancelCommand { get; }

        public AddScatterGraphBuilderViewModel(AddScatterGraphBuilderView addScatterGraphView, AddScatterGraphViewModel addScatterGraphViewModel)
        {
            _isUnselectingAll = false;
            _isEmptySelected = false;
            _isToPopulateSelected = false;
            _isToOpenSelected = false;
            _populateSelectedIndex = 0;

            ValidateCommand = new CommandKey(new ValidateAddingScatterGraphBuilderCommand(addScatterGraphViewModel, addScatterGraphView, this), Key.Enter, ModifierKeys.None, "Add builder");
            CancelCommand = new CommandKey(new ActionCommand(addScatterGraphView.Close), Key.Escape, ModifierKeys.None, "Cancel");
        }

        private void UnselectAll()
        {
            if (!_isUnselectingAll)
            {
                _isUnselectingAll = true;
                IsEmptySelected = false;
                IsToPopulateSelected = false;
                IsToOpenSelected = false;
                _isUnselectingAll = false;
            }
        }

        private void UpdatePopulateBuilder()
        {
            if (_populateSelectedIndex == 0)
            {
                Builder = new ScatterGraphPopulateRandomBuilder();
            }
            else if (_populateSelectedIndex == 1)
            {
                Builder = new ScatterGraphPopulateRectangle2DBuilder();
            }
            else if (_populateSelectedIndex == 2)
            {
                Builder = new ScatterGraphPopulateLineBuilder();
            }
            else
            {
                Builder = null;
            }
        }
    }
}

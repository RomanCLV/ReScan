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
    public class AddScatterGraphModelView : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;

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
                    Builder = new ScatterGraphPopulateRandomBuilder();
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

        public AddScatterGraphModelView(AddScatterGraphView addScatterGraphView, MainViewModel mainViewModel)
        {
            _isUnselectingAll = false;
            _isEmptySelected = false;
            _isToPopulateSelected = false;
            _isToOpenSelected = false;
            _mainViewModel = mainViewModel;

            ValidateCommand = new CommandKey(new ValidateAddingScatterGraphCommand(addScatterGraphView, this, mainViewModel), Key.Enter, ModifierKeys.None, "Validate");
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
    }
}

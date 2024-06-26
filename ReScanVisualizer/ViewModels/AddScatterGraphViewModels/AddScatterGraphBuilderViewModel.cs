﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ReScanVisualizer.Commands;
using ReScanVisualizer.Views.AddScatterGraphViews;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders;

#nullable enable

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels
{
    public class AddScatterGraphBuilderViewModel : ViewModelBase
    {
        private readonly AddScatterGraphBuilderWindow _addScatterGraphView;

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
                    Builder = new ScatterGraphFilesBuilder(_addScatterGraphView);
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
                if (_builder != null)
                {
                    _builder.PartsListSource = MainViewModel.GetInstance();
                }
            }
        }

        public CommandKey ValidateCommand { get; }
        public CommandKey CancelCommand { get; }

        public AddScatterGraphBuilderViewModel(AddScatterGraphBuilderWindow addScatterGraphView, AddScatterGraphViewModel addScatterGraphViewModel)
        {
            _addScatterGraphView = addScatterGraphView;
            _isUnselectingAll = false;
            _isEmptySelected = false;
            _isToPopulateSelected = false;
            _isToOpenSelected = false;
            _populateSelectedIndex = 0;

            ValidateCommand = new CommandKey(new ValidateAddingScatterGraphBuilderCommand(addScatterGraphViewModel, _addScatterGraphView, this), Key.Enter, ModifierKeys.None, "Add builder");
            CancelCommand = new CommandKey(new ActionCommand(_addScatterGraphView.Close), Key.Escape, ModifierKeys.None, "Cancel");
        }

        ~AddScatterGraphBuilderViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                ValidateCommand.Dispose();
                CancelCommand.Dispose();
                _builder?.Dispose();
                base.Dispose();
            }
        }

        public void DisposeWithoutBuilder()
        {
            if (!IsDisposed)
            {
                ValidateCommand.Dispose();
                CancelCommand.Dispose();
                base.Dispose();
            }
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
            if (_populateSelectedIndex == 0) // Random
            {
                Builder = new ScatterGraphPopulateRandomBuilder();
            }
            else if (_populateSelectedIndex == 1) // Rectangle 2D
            {
                Builder = new ScatterGraphPopulateRectangle2DBuilder();
            }
            else if (_populateSelectedIndex == 2) // Line
            {
                Builder = new ScatterGraphPopulateLineBuilder();
            }
            else if (_populateSelectedIndex == 3) // Function f(x, y) = z
            {
                Builder = new ScatterGraphPopulateFunctionXYBuilder();
            }
            else if (_populateSelectedIndex == 4) // Parametrics functions: t
            {
                Builder = new ScatterGraphPopulateParametricsFunctionsTBuilder();
            }
            else if (_populateSelectedIndex == 5) // Parametrics functions: u, v
            {
                Builder = new ScatterGraphPopulateParametricsFunctionsUVBuilder();
            }
            else
            {
                Builder = null;
            }
        }
    }
}

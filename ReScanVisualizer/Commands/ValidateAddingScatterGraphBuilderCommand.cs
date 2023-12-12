using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders;
using ReScanVisualizer.Views.AddScatterGraphViews;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class ValidateAddingScatterGraphBuilderCommand : CommandBase
    {
        private readonly AddScatterGraphViewModel _addScatterGraphViewModel;
        private readonly AddScatterGraphBuilderWindow _addScatterGraphBuilderView;
        private readonly AddScatterGraphBuilderViewModel _addScatterGraphBuilderModelView;
        private ScatterGraphBuilderBase? _builder;

        public ValidateAddingScatterGraphBuilderCommand(AddScatterGraphViewModel addScatterGraphViewModel, AddScatterGraphBuilderWindow addScatterGraphView, AddScatterGraphBuilderViewModel addScatterGraphModelView)
        {
            _addScatterGraphViewModel = addScatterGraphViewModel;
            _addScatterGraphBuilderView = addScatterGraphView;
            _addScatterGraphBuilderModelView = addScatterGraphModelView;

            _addScatterGraphBuilderModelView.PropertyChanged += AddScatterGraphModelView_PropertyChanged;
            _builder = _addScatterGraphBuilderModelView.Builder;

            if (_builder != null)
            {
                _builder.PropertyChanged += Builder_PropertyChanged;
            }
        }

        ~ValidateAddingScatterGraphBuilderCommand()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                _addScatterGraphBuilderModelView.PropertyChanged -= AddScatterGraphModelView_PropertyChanged;
                if (_builder != null)
                {
                    _builder.PropertyChanged -= Builder_PropertyChanged;
                }
                base.Dispose();
            }
        }

        private void AddScatterGraphModelView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AddScatterGraphBuilderViewModel.Builder))
            {
                if (_builder != null)
                {
                    _builder.PropertyChanged -= Builder_PropertyChanged;
                }
                _builder = _addScatterGraphBuilderModelView.Builder;
                if (_builder != null)
                {
                    _builder.PropertyChanged += Builder_PropertyChanged;
                }
            }
            OnCanExecuteChanged();
        }

        private void Builder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScatterGraphBuilderBase.IsReady))
            {
                OnCanExecuteChanged();
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return (!(_builder is null) && _builder.IsReady) && (
                _addScatterGraphBuilderModelView.IsEmptySelected ||
                _addScatterGraphBuilderModelView.IsToPopulateSelected ||
                _addScatterGraphBuilderModelView.IsToOpenSelected
                );
        }

        public override void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                AddBuilder(_builder);
                _addScatterGraphBuilderView.Close();
            }
            else
            {
                OnCanExecuteChanged(); 
            }
        }

        private void AddBuilder(ScatterGraphBuilderBase? builder)
        {
            if (builder != null)
            {
                if (builder is IScatterGraphBuilderGroup group)
                {
                    foreach (ScatterGraphBuilderBase? b in group.Builders)
                    {
                        AddBuilder(b);
                    }
                }
                else
                {
                    _addScatterGraphViewModel.AddBuilder(builder);
                }
            }
        }
    }
}

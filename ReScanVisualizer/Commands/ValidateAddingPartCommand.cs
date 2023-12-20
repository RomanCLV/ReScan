using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels.AddPartModelViews;
using ReScanVisualizer.ViewModels.AddPartModelViews.Builders;
using ReScanVisualizer.Views.AddPartViews;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class ValidateAddingPartCommand : CommandBase
    {
        private readonly AddPartWindow _addPartView;
        private readonly AddPartViewModel _addPartViewModel;
        private PartBuilderBase? _builder;

        public ValidateAddingPartCommand(AddPartWindow window, AddPartViewModel mainViewModel)
        {
            _addPartView = window;
            _addPartViewModel = mainViewModel;
            _builder = _addPartViewModel.Builder;

            _addPartViewModel.PropertyChanged += AddPartViewModel_PropertyChanged;
            if (_builder != null )
            {
                _builder.PropertyChanged += Builder_PropertyChanged;
            }
        }

        ~ValidateAddingPartCommand()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                _addPartViewModel.PropertyChanged -= AddPartViewModel_PropertyChanged;
                if (_builder != null)
                {
                    _builder.PropertyChanged -= Builder_PropertyChanged;
                }
                base.Dispose();
            }
        }

        private void AddPartViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AddPartViewModel.Builder))
            {
                if (_builder != null)
                {
                    _builder.PropertyChanged -= Builder_PropertyChanged;
                }
                _builder = _addPartViewModel.Builder;
                if (_builder != null)
                {
                    _builder.PropertyChanged += Builder_PropertyChanged;
                }
                OnCanExecuteChanged();
            }
        }

        private void Builder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PartBuilderBase.CanBuild))
            {
                OnCanExecuteChanged();
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter) && _builder != null && _builder.CanBuild;
        }

        public override void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                _addPartViewModel.Build();
                _addPartView.Close();
            }
            else
            {
                OnCanExecuteChanged();
            }
        }
    }
}

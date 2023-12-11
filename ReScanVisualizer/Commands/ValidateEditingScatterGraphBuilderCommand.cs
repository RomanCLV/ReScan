using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders;
using ReScanVisualizer.Views.AddScatterGraphViews;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class ValidateEditingScatterGraphBuilderCommand : CommandBase
    {
        private readonly EditScatterGraphBuilderView _view;
        private readonly ScatterGraphBuilderBase _builder;


        public ValidateEditingScatterGraphBuilderCommand(EditScatterGraphBuilderView view, ScatterGraphBuilderBase builder)
        {
            _view = view;
            _builder = builder;
            _builder.PropertyChanged += Builder_PropertyChanged;
        }

        ~ValidateEditingScatterGraphBuilderCommand()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                _builder.PropertyChanged -= Builder_PropertyChanged;
                base.Dispose();
            }
        }

        private void Builder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScatterGraphBuilderBase.CanBuild))
            {
                OnCanExecuteChanged();
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return _builder.CanBuild;
        }

        public override void Execute(object? parameter)
        {
            _view.Close();
        }
    }
}

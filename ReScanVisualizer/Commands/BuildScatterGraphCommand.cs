using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class BuildScatterGraphCommand : AsyncCommandBase
    {
        private readonly AddScatterGraphViewModel _viewModel;

        public BuildScatterGraphCommand(AddScatterGraphViewModel viewModel)
        {
            _viewModel = viewModel;

            _viewModel.Items.CollectionChanged += Items_CollectionChanged;
        }

        ~BuildScatterGraphCommand()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                _viewModel.Items.CollectionChanged -= Items_CollectionChanged;
                base.Dispose();
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter) && _viewModel.Items.Count > 0;
        }

        public override Task ExecuteAsync(object? parameter)
        {
            return _viewModel.BuildAllAsync();
        }
    }
}

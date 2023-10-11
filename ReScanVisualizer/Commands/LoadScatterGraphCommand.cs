using ReScanVisualizer.ViewModels.AddScatterGraph;
using ReScanVisualizer.Views.AddScatterGraphViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace ReScanVisualizer.Commands
{
    internal class LoadScatterGraphCommand : AsyncCommandBase
    {
        private readonly AddScatterGraphView _view;
        private readonly AddScatterGraphViewModel _viewModel;
        bool _closeAfterExecture;

        public LoadScatterGraphCommand(AddScatterGraphView view, AddScatterGraphViewModel viewModel, bool closeAfterExecture)
        {
            _view = view;
            _viewModel = viewModel;
            _closeAfterExecture = closeAfterExecture;

            _viewModel.Items.CollectionChanged += Items_CollectionChanged;
        }

        ~LoadScatterGraphCommand()
        {
            _viewModel.Items.CollectionChanged -= Items_CollectionChanged;
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
            return _viewModel.LoadAsync();
        }
    }
}

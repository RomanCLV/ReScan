using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.Views.AddScatterGraphViews;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class AddScatterGraphBuilderCommand : CommandBase
    {
        private readonly AddScatterGraphWindow _addScatterGraphView;
        private readonly AddScatterGraphViewModel _addScatterGraphViewModel;

        public AddScatterGraphBuilderCommand(AddScatterGraphWindow addScatterGraphView, AddScatterGraphViewModel addScatterGraphViewModel)
        {
            _addScatterGraphView =  addScatterGraphView;
            _addScatterGraphViewModel = addScatterGraphViewModel;
        }

        public override void Execute(object? parameter)
        {
            AddScatterGraphBuilderWindow view = new AddScatterGraphBuilderWindow()
            {
                Owner = _addScatterGraphView
            };
            AddScatterGraphBuilderViewModel addScatterGraphBuilderViewModel = new AddScatterGraphBuilderViewModel(view, _addScatterGraphViewModel);
            view.DataContext = addScatterGraphBuilderViewModel;
            view.ShowDialog();
            addScatterGraphBuilderViewModel.Dispose();
        }
    }
}

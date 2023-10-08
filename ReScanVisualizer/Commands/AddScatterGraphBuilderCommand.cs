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
    public class AddScatterGraphBuilderCommand : CommandBase
    {
        private readonly AddScatterGraphView _addScatterGraphView;
        private readonly AddScatterGraphViewModel _addScatterGraphViewModel;

        public AddScatterGraphBuilderCommand(AddScatterGraphView addScatterGraphView, AddScatterGraphViewModel addScatterGraphViewModel)
        {
            _addScatterGraphView =  addScatterGraphView;
            _addScatterGraphViewModel = addScatterGraphViewModel;
        }

        public override void Execute(object? parameter)
        {
            AddScatterGraphBuilderView view = new AddScatterGraphBuilderView()
            {
                Owner = _addScatterGraphView
            };
            view.DataContext = new AddScatterGraphBuilderViewModel(view, _addScatterGraphViewModel);
            view.ShowDialog();
        }
    }
}

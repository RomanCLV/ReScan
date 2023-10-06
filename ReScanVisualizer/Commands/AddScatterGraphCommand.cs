using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.ViewModels.AddScatterGraph;
using ReScanVisualizer.Views.AddScatterGraphViews;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class AddScatterGraphCommand : CommandBase
    {
        private readonly MainViewModel _mainViewModel;

        public AddScatterGraphCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public override void Execute(object? parameter)
        {
            AddScatterGraphView view = new AddScatterGraphView()
            {
                Owner = Application.Current.MainWindow,
            };
            view.DataContext = new AddScatterGraphViewModel(view, _mainViewModel);
            view.ShowDialog();
        }
    }
}

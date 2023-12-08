using ReScanVisualizer.ViewModels;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels;
using ReScanVisualizer.Views.AddPartViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReScanVisualizer.Views.AddScatterGraphViews;
using ReScanVisualizer.ViewModels.AddPartModelViews;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class AddPartCommand : CommandBase
    {
        private readonly MainViewModel _mainViewModel;

        public AddPartCommand(MainViewModel mainViewModel) 
        {
            _mainViewModel = mainViewModel;
        }

        public override void Execute(object? parameter)
        {
            AddPartWindow view = new AddPartWindow()
            {
                Owner = Application.Current.MainWindow,
            };
            AddPartViewModel addPartViewModel = new AddPartViewModel(view, _mainViewModel);
            view.DataContext = addPartViewModel;
            view.ShowDialog();
            addPartViewModel.Dispose();
        }
    }
}

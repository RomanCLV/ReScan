﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels;
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
            AddScatterGraphWindow view = new AddScatterGraphWindow()
            {
                Owner = Application.Current.MainWindow,
            };
            AddScatterGraphViewModel addScatterGraphViewModel = new AddScatterGraphViewModel(view, _mainViewModel);
            view.DataContext = addScatterGraphViewModel;
            view.ShowDialog();
            addScatterGraphViewModel.Dispose();
        }
    }
}

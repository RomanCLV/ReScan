using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.ViewModels.AddPartModelViews;
using ReScanVisualizer.Views.AddPartViews;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class AddPartCommand : CommandBase
    {
        private readonly IPartSource _partSource;

        public AddPartCommand(IPartSource partSource) 
        {
            _partSource = partSource;
        }

        public override void Execute(object? parameter)
        {
            AddPartWindow view = new AddPartWindow()
            {
                Owner = Application.Current.MainWindow,
            };
            AddPartViewModel addPartViewModel = new AddPartViewModel(view, _partSource);
            view.DataContext = addPartViewModel;
            view.ShowDialog();
            addPartViewModel.Dispose();
        }
    }
}

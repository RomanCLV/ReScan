using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.ViewModels.AddPartModelViews;
using ReScanVisualizer.Views.AddPartViews;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class ValidateAddingPartCommand : CommandBase
    {
        private readonly AddPartWindow _addPartView;
        private readonly AddPartViewModel _addPartViewModel;

        public ValidateAddingPartCommand(AddPartWindow window, AddPartViewModel mainViewModel)
        {
            _addPartView = window;
            _addPartViewModel = mainViewModel;
        }

        public override void Execute(object? parameter)
        {
            _addPartViewModel.Build();
            _addPartView.Close();
        }
    }
}

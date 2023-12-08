using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.Views.AddPartViews;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class ValidateAddingPartCommand : CommandBase
    {
        private readonly AddPartWindow _addPartView;
        private readonly MainViewModel _mainViewModel;

        public ValidateAddingPartCommand(AddPartWindow window, MainViewModel mainViewModel)
        {
            _addPartView = window;
            _mainViewModel = mainViewModel;
        }

        public override void Execute(object? parameter)
        {
            _addPartView.Close();
        }
    }
}

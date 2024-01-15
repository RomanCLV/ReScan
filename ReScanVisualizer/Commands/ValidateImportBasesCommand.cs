using ReScanVisualizer.ViewModels;
using ReScanVisualizer.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.Commands
{
    public class ValidateImportBasesCommand : CommandBase
    {
        private readonly ImportBasesViewModel _viewModel;

        public ValidateImportBasesCommand(ImportBasesViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public override void Execute(object parameter)
        {
            _viewModel.ImportFile();
        }
    }
}

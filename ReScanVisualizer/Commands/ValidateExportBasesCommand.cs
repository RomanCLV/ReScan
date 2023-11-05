using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.Views;

namespace ReScanVisualizer.Commands
{
    public class ValidateExportBasesCommand : CommandBase
    {
        private readonly ExportBasesViewModel _viewModel;
        private readonly ExportBasesWindow _window;


        public ValidateExportBasesCommand(ExportBasesViewModel exportBasesViewModel, ExportBasesWindow exportBasesWindow) 
        {
            _viewModel = exportBasesViewModel;
            _window = exportBasesWindow;

            foreach (var item in _viewModel.Items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        ~ValidateExportBasesCommand()
        {
            foreach (var item in _viewModel.Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) && _viewModel.Items.Any(x => x.IsSelected);
        }

        public override void Execute(object parameter)
        {
            if (_viewModel.Export())
            {
                _window.Close();
            }
        }
    }
}

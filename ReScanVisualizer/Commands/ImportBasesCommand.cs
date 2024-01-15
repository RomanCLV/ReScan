using ReScanVisualizer.ViewModels;
using ReScanVisualizer.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class ImportBasesCommand : CommandBase
    {

        private readonly MainViewModel _mainViewModel;

        public ImportBasesCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public override void Execute(object? parameter)
        {
            ImportBasesWindow window = new ImportBasesWindow
            {
                Owner = Application.Current.MainWindow
            };
            ImportBasesViewModel importBasesViewModel = new ImportBasesViewModel(_mainViewModel, window);
            window.DataContext = importBasesViewModel;
            window.ShowDialog();
            importBasesViewModel.Dispose();
        }
    }
}

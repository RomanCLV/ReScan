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
    public class ExportBasesCommand : CommandBase
    {
        private readonly MainViewModel _mainViewModel;

        public ExportBasesCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _mainViewModel.Bases.CollectionChanged += Bases_CollectionChanged;
            _mainViewModel.ScatterGraphs.CollectionChanged += ScatterGraphs_CollectionChanged;
        }

        ~ExportBasesCommand()
        {
            _mainViewModel.Bases.CollectionChanged -= Bases_CollectionChanged;
            _mainViewModel.ScatterGraphs.CollectionChanged -= ScatterGraphs_CollectionChanged;
        }

        private void ScatterGraphs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }

        private void Bases_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }

        public override bool CanExecute(object? parameter)
        {
            return 
                base.CanExecute(parameter) && 
                (_mainViewModel.Bases.Count > 0 ||
                _mainViewModel.ScatterGraphs.Count > 0);
        }

        public override void Execute(object? parameter)
        {
            ExportBasesWindow window = new ExportBasesWindow
            {
                Owner = Application.Current.MainWindow
            };
            ExportBasesViewModel exportBasesViewModel = new ExportBasesViewModel(_mainViewModel, window);
            window.DataContext = exportBasesViewModel;
            window.ShowDialog();
            exportBasesViewModel.Dispose();
        }
    }
}

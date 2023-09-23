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
    public class ValidateAddingScatterGraphCommand : CommandBase
    {
        private readonly AddScatterGraphView _addScatterGraphView;
        private readonly AddScatterGraphModelView _addScatterGraphModelView;
        private readonly MainViewModel _mainViewModel;

        public ValidateAddingScatterGraphCommand(AddScatterGraphView addScatterGraphView, AddScatterGraphModelView addScatterGraphModelView, MainViewModel mainViewModel)
        {
            _addScatterGraphView = addScatterGraphView;
            _addScatterGraphModelView = addScatterGraphModelView;
            _mainViewModel = mainViewModel;

            _addScatterGraphModelView.PropertyChanged += AddScatterGraphModelView_PropertyChanged;
        }

        ~ValidateAddingScatterGraphCommand()
        {
            _addScatterGraphModelView.PropertyChanged -= AddScatterGraphModelView_PropertyChanged;
        }

        private void AddScatterGraphModelView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }

        public override bool CanExecute(object? parameter)
        {
            return
                _addScatterGraphModelView.IsEmptySelected ||
                _addScatterGraphModelView.IsToPopulateSelected ||
                _addScatterGraphModelView.IsToOpenSelected;
        }

        public override void Execute(object? parameter)
        {
            if (_addScatterGraphModelView.Builder != null)
            {
                ScatterGraphViewModel[] scatterGraphViewModels;
                try
                {
                    scatterGraphViewModels = _addScatterGraphModelView.Builder.Build();
                }
                catch (Exception ex)
                {
                    scatterGraphViewModels = new ScatterGraphViewModel[0];
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }

                foreach (ScatterGraphViewModel viewModel in scatterGraphViewModels)
                {
                    _mainViewModel.ScatterGraphs.Add(viewModel);
                }
            }
            _addScatterGraphView.Close();
        }
    }
}

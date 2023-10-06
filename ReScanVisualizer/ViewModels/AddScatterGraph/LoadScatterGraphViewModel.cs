using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
using ReScanVisualizer.Views.AddScatterGraphViews;

namespace ReScanVisualizer.ViewModels.AddScatterGraph
{
    internal class LoadScatterGraphViewModel : ViewModelBase
    {
        private readonly LoadScatterGraphView _loadScatterGraphView;
        private readonly MainViewModel _mainViewModel;
        private readonly ScatterGraphBuilderBase _builder;

        public LoadScatterGraphViewModel(LoadScatterGraphView view, MainViewModel mainViewModel, ScatterGraphBuilderBase builder)
        {
            _loadScatterGraphView = view;
            _mainViewModel = mainViewModel;
            _builder = builder;
        }

        // TODO : rendre la méthode async
        public void Load()
        {
            if (_builder != null)
            {
                ScatterGraphViewModel[] scatterGraphViewModels;
                try
                {
                    scatterGraphViewModels = _builder.Build();
                }
                catch (Exception ex)
                {
                    scatterGraphViewModels = new ScatterGraphViewModel[0];
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }

                foreach (ScatterGraphViewModel viewModel in scatterGraphViewModels)
                {
                    // TODO : check if Count > 1000 --> want to reduce ?
                    _mainViewModel.ScatterGraphs.Add(viewModel);
                }
            }
        }
    }
}

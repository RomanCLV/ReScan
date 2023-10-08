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
        private readonly AddScatterGraphView _addScatterGraphView;
        private readonly ScatterGraphBuilderBase _builder;

        public LoadScatterGraphViewModel(LoadScatterGraphView view, MainViewModel mainViewModel, AddScatterGraphView addScatterGraphView, ScatterGraphBuilderBase builder)
        {
            _loadScatterGraphView = view;
            _mainViewModel = mainViewModel;
            _addScatterGraphView = addScatterGraphView;
            _builder = builder;
        }

        // TODO : rendre la méthode async
        public void Load()
        {
            if (_builder != null)
            {
                ScatterGraphBuildResult[] scatterGraphViewModels = _builder.Build();

                foreach (ScatterGraphBuildResult result in scatterGraphViewModels)
                {
                    // TODO : check if Count > 1000 --> want to reduce ?
                    if (result.IsSuccess)
                    {
                        _mainViewModel.ScatterGraphs.Add(new ScatterGraphViewModel(result.ScatterGraph, result.Color));
                    }
                }
                _addScatterGraphView.Close();

            }
        }
    }
}

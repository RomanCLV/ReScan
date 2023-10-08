using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ReScanVisualizer.Commands;
using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
using ReScanVisualizer.Views.AddScatterGraphViews;

#nullable enable

namespace ReScanVisualizer.ViewModels.AddScatterGraph
{
    public class AddScatterGraphViewModel : ViewModelBase
    {
        private readonly AddScatterGraphView _view;
        private readonly MainViewModel _mainViewModel;
        
        public ObservableCollection<ScatterGraphBuilderBase> Builders { get; private set; }

        public Dictionary<ScatterGraphBuilderBase, ScatterGraphBuildResult?> Results { get; private set; }

        public CommandKey AddScatterGraphBuilderCommand { get; private set; }
        public CommandKey LoadScatterGraphCommand { get; private set; }
        public CommandKey CancelCommand { get; private set; }

        public AddScatterGraphViewModel(AddScatterGraphView view, MainViewModel mainViewModel)
        {
            _view = view;
            _mainViewModel = mainViewModel;
            Builders = new ObservableCollection<ScatterGraphBuilderBase>();
            Results = new Dictionary<ScatterGraphBuilderBase, ScatterGraphBuildResult?>();

            Builders.CollectionChanged += Builders_CollectionChanged;

            AddScatterGraphBuilderCommand = new CommandKey(new AddScatterGraphBuilderCommand(_view, this), Key.A, ModifierKeys.Control | ModifierKeys.Alt, "Add a new builder");
            LoadScatterGraphCommand = new CommandKey(new ActionCommand(Build), Key.Enter, ModifierKeys.None, "Load");
            CancelCommand = new CommandKey(new ActionCommand(_view.Close), Key.Escape, ModifierKeys.None, "Cancel");
        }

        ~AddScatterGraphViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                Builders.CollectionChanged -= Builders_CollectionChanged;
                base.Dispose();
                IsDisposed = true;
            }
        }

        private void Builders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Results.Clear();
            foreach (ScatterGraphBuilderBase builder in Builders)
            {
                Results.Add(builder, null);
            }
        }

        // TODO : rendre la méthode async
        private void Build()
        {
            foreach (KeyValuePair<ScatterGraphBuilderBase, ScatterGraphBuildResult?> keyValue in Results)
            {
                if (keyValue.Value is null || !keyValue.Value.IsSuccess)
                {
                    Results[keyValue.Key] = keyValue.Key.Build();
                }
            }
        }

        // TODO : rendre la méthode async
        public void Load()
        {
            foreach (KeyValuePair<ScatterGraphBuilderBase, ScatterGraphBuildResult?> keyValue in Results)
            {
                if (keyValue.Value != null && keyValue.Value.IsSuccess && keyValue.Value.ScatterGraph != null)
                {
                    _mainViewModel.ScatterGraphs.Add(new ScatterGraphViewModel(keyValue.Value.ScatterGraph, keyValue.Key.Color));
                }
            }
            _view.Close();
        }
    }
}

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

namespace ReScanVisualizer.ViewModels.AddScatterGraph
{
    public class AddScatterGraphViewModel : ViewModelBase
    {
        private readonly AddScatterGraphView _view;
        private readonly MainViewModel _mainViewModel;
        
        public ObservableCollection<ScatterGraphBuilderBase> Builders { get; private set; }

        private readonly Dictionary<ScatterGraphBuilderBase, ScatterGraphBuildResult> _results;

        public CommandKey AddScatterGraphBuilderCommand { get; private set; }
        public CommandKey LoadScatterGraphCommand { get; private set; }
        public CommandKey CancelCommand { get; private set; }

        public AddScatterGraphViewModel(AddScatterGraphView view, MainViewModel mainViewModel)
        {
            _view = view;
            _mainViewModel = mainViewModel;
            Builders = new ObservableCollection<ScatterGraphBuilderBase>();
            _results = new Dictionary<ScatterGraphBuilderBase, ScatterGraphBuildResult>();

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
            _results.Clear();
            foreach (ScatterGraphBuilderBase builder in Builders)
            {
                _results.Add(builder, null);
            }
        }

        // TODO : rendre la méthode async
        private void Build()
        {
            foreach (KeyValuePair<ScatterGraphBuilderBase, ScatterGraphBuildResult> keyValue in _results)
            {
                if (keyValue.Value is null || !keyValue.Value.IsSuccess)
                {
                    _results[keyValue.Key] = keyValue.Key.Build();
                }
            }
        }

        // TODO : rendre la méthode async
        public void Load()
        {
            foreach (KeyValuePair<ScatterGraphBuilderBase, ScatterGraphBuildResult> keyValue in _results)
            {
                if (keyValue.Value.IsSuccess)
                {
                    _mainViewModel.ScatterGraphs.Add(new ScatterGraphViewModel(keyValue.Value.ScatterGraph, keyValue.Key.Color));
                }
            }
            _view.Close();
        }
    }
}

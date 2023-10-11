using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        
        public ObservableCollection<KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult?>> Items { get; private set; }

        public CommandKey AddScatterGraphBuilderCommand { get; private set; }
        public CommandKey BuildCommand { get; private set; }
        public CommandKey LoadCommand { get; private set; }
        public CommandKey LoadAndCloseCommand { get; private set; }
        public CommandKey CancelCommand { get; private set; }

        public AddScatterGraphViewModel(AddScatterGraphView view, MainViewModel mainViewModel)
        {
            _view = view;
            _mainViewModel = mainViewModel;
            Items = new ObservableCollection<KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult?>>();

            Items.CollectionChanged += Items_CollectionChanged;

            AddScatterGraphBuilderCommand = new CommandKey(new AddScatterGraphBuilderCommand(_view, this), Key.A, ModifierKeys.Control | ModifierKeys.Alt, "Add a new builder");
            BuildCommand = new CommandKey(new ActionCommand(Build), Key.Enter, ModifierKeys.None, "Build");
            LoadCommand = new CommandKey(new ActionCommand(Load), Key.Enter, ModifierKeys.None, "Load");
            LoadAndCloseCommand = new CommandKey(new ActionCommand(LoadAndClose), Key.Enter, ModifierKeys.None, "Load and close");
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
                Items.CollectionChanged -= Items_CollectionChanged;

                base.Dispose();
                IsDisposed = true;
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                bool firstFound = false;
                List<KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult?>> toRemove = new List<KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult?>>(Items.Count);
                KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult?> v = (KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult?>)e.NewItems[0];
                foreach (var item in Items)
                {
                    // find if item or item key is many times in the list
                    if (item.Equals(v) || item.Key.Equals(v.Key))
                    {
                        if (firstFound)
                        {
                            toRemove.Add(item);
                        }
                        else
                        {
                            firstFound = true;
                        }
                    }
                }
                if (toRemove.Count > 0)
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        foreach (var item in toRemove)
                        {
                            Items.Remove(item);
                        }
                    });
                }
            }
        }

        public void AddBuilder(ScatterGraphBuilderBase builder)
        {
            foreach (var item in Items)
            {
                if (item.Key.Equals(builder))
                {
                    return;
                }
            }
            Items.Add(new KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult?>(builder, new ScatterGraphBuildResult()));
        }

        public void RemoveBuilder(ScatterGraphBuilderBase selectedBuilder)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Key.Equals(selectedBuilder))
                {
                    Items.RemoveAt(i);
                }
            }
        }

        // TODO : rendre la méthode async
        private void Build()
        {
            ScatterGraphBuilderBase scatterGraphBuilderBase;
            ScatterGraphBuildResult? scatterGraphBuildResult;

            for (int i = 0; i < Items.Count; i++)
            {
                scatterGraphBuilderBase = Items[i].Key;
                scatterGraphBuildResult = Items[i].Value;
                if (scatterGraphBuildResult is null || !scatterGraphBuildResult.IsSuccess)
                {
                    ScatterGraphBuildResult result = scatterGraphBuilderBase.Build();
                    if (Items[i].Value is null)
                    {
                        Items[i].Value = result;
                    }
                    else
                    {
                        Items[i].Value.Set(result);
                    }
                }
            }
        }

        // TODO : rendre la méthode async
        private void Load()
        {
            foreach (var item in Items)
            {
                if (item.Value != null && item.Key.State is ScatterGraphBuilderState.Success && !item.Value.IsAdded)
                {
                    _mainViewModel.ScatterGraphs.Add(new ScatterGraphViewModel(item.Value.ScatterGraph!, item.Key.Color));
                    item.Value.SetAddedToTrue();
                }
            }
        }

        private void LoadAndClose()
        {
            Load();
            _view.Close();
        }
    }
}

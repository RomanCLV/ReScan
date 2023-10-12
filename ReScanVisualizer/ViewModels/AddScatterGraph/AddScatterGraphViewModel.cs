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
        
        public ObservableCollection<KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>> Items { get; private set; }

        public CommandKey AddScatterGraphBuilderCommand { get; private set; }
        public CommandKey BuildCommand { get; private set; }
        public CommandKey LoadCommand { get; private set; }
        public CommandKey LoadAndCloseCommand { get; private set; }
        public CommandKey CancelCommand { get; private set; }

        public AddScatterGraphViewModel(AddScatterGraphView view, MainViewModel mainViewModel)
        {
            _view = view;
            _mainViewModel = mainViewModel;
            Items = new ObservableCollection<KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>>();

            Items.CollectionChanged += Items_CollectionChanged;

            AddScatterGraphBuilderCommand = new CommandKey(new AddScatterGraphBuilderCommand(_view, this), Key.A, ModifierKeys.Control | ModifierKeys.Shift, "Add a new builder");
            BuildCommand = new CommandKey(new BuildScatterGraphCommand(this), Key.B, ModifierKeys.Control, "Build");
            LoadCommand = new CommandKey(new LoadScatterGraphCommand(view, this, false), Key.L, ModifierKeys.Control, "Load");
            LoadAndCloseCommand = new CommandKey(new LoadScatterGraphCommand(view, this, true), Key.L, ModifierKeys.Control | ModifierKeys.Shift, "Load and close");
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
                List<KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>> toRemove = new List<KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>>(Items.Count);
                KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> v = (KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>)e.NewItems[0];
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
            Items.Add(new KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>(builder, new ScatterGraphBuildResult()));
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

        public void BuildAll()
        {
            foreach (var item in Items)
            {
                if (item.Value is null || !item.Value.IsSuccess)
                {
                    Build(item);
                }
            }
        }

        public void Build(KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> item)
        {
            ScatterGraphBuilderBase scatterGraphBuilderBase = item.Key;
            ScatterGraphBuildResult result = scatterGraphBuilderBase.Build();
            if (item.Value is null)
            {
                item.Value = result;
            }
            else
            {
                item.Value.Set(result);
            }
        }

        public async Task BuildAllAsync()
        {
            foreach (var item in Items)
            {
                if (item.Value is null || !item.Value.IsSuccess)
                {
                    await BuildAsync(item);
                }
            }
        }

        public async Task BuildAsync(KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> item)
        {
            ScatterGraphBuilderBase scatterGraphBuilderBase = item.Key;
            ScatterGraphBuildResult result = await scatterGraphBuilderBase.BuildAsync();
            if (item.Value is null)
            {
                item.Value = result;
            }
            else
            {
                item.Value.Set(result);
            }
        }

        public void Load(bool closeWindow = false)
        {
            foreach (var item in Items)
            {
                if (item.Value != null && item.Key.State is ScatterGraphBuilderState.Success && !item.Value.IsAdded)
                {
                    _mainViewModel.AddScatterGraph(new ScatterGraphViewModel(item.Value.ScatterGraph!, item.Key.Color));
                    item.Value.SetAddedToTrue();
                }
            }
            if (closeWindow)
            {
                _view.Close();
            }
        }


        public Task LoadAsync(bool closeWindow = false)
        {
            return Task.Run(() => Load(closeWindow));
        }
    }
}

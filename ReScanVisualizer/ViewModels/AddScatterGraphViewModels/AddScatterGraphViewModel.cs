using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ReScanVisualizer.Commands;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders;
using ReScanVisualizer.ViewModels.Parts;
using ReScanVisualizer.Views.AddScatterGraphViews;

#nullable enable

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels
{
    public class AddScatterGraphViewModel : ViewModelBase
    {
        private readonly AddScatterGraphWindow _view;
        private readonly MainViewModel _mainViewModel;

        public ObservableCollection<KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>> Items { get; private set; }

        public List<RenderQuality> RenderQualities { get; }

        private uint _itemsToAddCount;
        public uint ItemsToAddCount
        {
            get => _itemsToAddCount;
            set => SetValue(ref _itemsToAddCount, value);
        }

        private uint _maxPoints;
        public uint MaxPoints
        {
            get => _maxPoints;
            set => SetValue(ref _maxPoints, value);
        }

        private double _commonScaleFactor;
        public double CommonScaleFactor
        {
            get => _commonScaleFactor;
            set => SetValue(ref _commonScaleFactor, value);
        }

        private double _commonAxisScaleFactor;
        public double CommonAxisScaleFactor
        {
            get => _commonAxisScaleFactor;
            set => SetValue(ref _commonAxisScaleFactor, value);
        }

        private double _commonPointRadius;
        public double CommonPointRadius
        {
            get => _commonPointRadius;
            set
            {
                if (_commonPointRadius <= 0.0)
                {
                    _commonPointRadius = 0.25;
                }
                SetValue(ref _commonPointRadius, value);
            }
        }

        private RenderQuality _commonRenderQuality;
        public RenderQuality CommonRenderQuality
        {
            get => _commonRenderQuality;
            set => SetValue(ref _commonRenderQuality, value);
        }

        private bool _commonDisplayBarycenter;
        public bool CommonDisplayBarycenter
        {
            get => _commonDisplayBarycenter;
            set => SetValue(ref _commonDisplayBarycenter, value);
        }

        private bool _commonDisplayAveragePlan;
        public bool CommonDisplayAveragePlan
        {
            get => _commonDisplayAveragePlan;
            set => SetValue(ref _commonDisplayAveragePlan, value);
        }

        private bool _commonDisplayBase;
        public bool CommonDisplayBase
        {
            get => _commonDisplayBase;
            set => SetValue(ref _commonDisplayBase, value);
        }

        private IPartSource _partsListSource;
        public virtual IPartSource PartsListSource
        {
            get => _partsListSource;
            set => SetValue(ref _partsListSource, value);
        }

        private PartViewModelBase? _part;
        public virtual PartViewModelBase? Part
        {
            get => _part;
            set => SetValue(ref _part, value);
        }

        public CommandKey AddScatterGraphBuilderCommand { get; private set; }
        public CommandKey AddPartCommand { get; private set; }
        public CommandKey BuildCommand { get; private set; }
        public CommandKey LoadCommand { get; private set; }
        public CommandKey LoadAndCloseCommand { get; private set; }
        public CommandKey CancelCommand { get; private set; }

        public AddScatterGraphViewModel(AddScatterGraphWindow view, MainViewModel mainViewModel)
        {
            _view = view;
            _mainViewModel = mainViewModel;
            _commonScaleFactor = 1.0;
            _commonAxisScaleFactor = 1.0;
            _maxPoints = 0;
            _commonPointRadius = 0.25;
            _commonRenderQuality = RenderQuality.High;
            RenderQualities = new List<RenderQuality>(Tools.GetRenderQualitiesList());
            _commonDisplayBarycenter = true;
            _commonDisplayAveragePlan = true;
            _commonDisplayBase = true;
            _partsListSource = _mainViewModel;
            _part = null;

            Items = new ObservableCollection<KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>>();

            _mainViewModel.Parts.CollectionChanged += Parts_CollectionChanged;
            Items.CollectionChanged += Items_CollectionChanged;

            AddScatterGraphBuilderCommand = new CommandKey(new AddScatterGraphBuilderCommand(_view, this), Key.A, ModifierKeys.Control | ModifierKeys.Shift, "Add a new builder");
            AddPartCommand = new CommandKey(new AddPartCommand(_partsListSource), Key.P, ModifierKeys.Control | ModifierKeys.Shift, "Add a new part");
            BuildCommand = new CommandKey(new BuildScatterGraphCommand(this), Key.B, ModifierKeys.Control, "Build");
            LoadCommand = new CommandKey(new LoadScatterGraphCommand(this, false), Key.L, ModifierKeys.Control, "Load");
            LoadAndCloseCommand = new CommandKey(new LoadScatterGraphCommand(this, true), Key.L, ModifierKeys.Control | ModifierKeys.Shift, "Load and close");
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
                _mainViewModel.Parts.CollectionChanged -= Parts_CollectionChanged;
                Items.CollectionChanged -= Items_CollectionChanged;
                AddScatterGraphBuilderCommand.Dispose();
                BuildCommand.Dispose();
                LoadCommand.Dispose();
                LoadAndCloseCommand.Dispose();
                CancelCommand.Dispose();
                ClearItems();
                base.Dispose();
            }
        }

        private void Parts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PartsListSource));
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Part = (PartViewModelBase)e.NewItems[e.NewItems.Count - 1];
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    bool firstFound = false;
                    List<KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>> toRemove = new List<KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>>(Items.Count);
                    KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> v = (KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>)e.NewItems[0];

                    // try to find if item is already in the list
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
                    // delete the duplicate
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
                    else
                    {
                        v.Value!.PropertyChanged += Value_PropertyChanged;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object? item in e.OldItems)
                    {
                        KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> oldItem = ((KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>)item);
                        oldItem.Value!.PropertyChanged -= Value_PropertyChanged;
                        oldItem.Key.Dispose();
                        oldItem.Value.Dispose();
                    }
                    break;
            }
            ComputeItemsToAddCount();
        }

        public void ClearItems()
        {
            foreach (KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> item in Items)
            {
                item.Value!.PropertyChanged -= Value_PropertyChanged;
                item.Key.Dispose();
                item.Value.Dispose();
            }
            Items.Clear();
        }

        private void Value_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ComputeItemsToAddCount();
        }

        private void ComputeItemsToAddCount()
        {
            int count = 0;
            foreach (var item in Items)
            {
                count += item.Value!.HasToReduce ? item.Value.ReducedCount : item.Value.Count;
            }
            ItemsToAddCount = (uint)count;
        }

        public void RandomizeColor()
        {
            foreach (var item in Items)
            {
                item.Key.Color = Tools.GetRandomLightColor();
            }
        }

        public void RandomizeColorAsync()
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in Items)
                {
                    Task.Delay(20).Wait();
                    item.Key.Color = Tools.GetRandomLightColor();
                }
            });
        }

        public void ApplyCommonPart()
        {
            foreach (var item in Items)
            {
                if (item.Key != null)
                {
                    item.Key.Part = _part;
                }
            }
        }

        public void ApplyMaxPoints()
        {
            foreach (var item in Items)
            {
                if (item.Value != null && item.Value.Count >= _maxPoints)
                {
                    // on determine le facteur de reduction en fonction du nombre points max qu'on veut
                    item.Value.ReductionFactor = Math.Round(100.0 - ((_maxPoints * 100.0) / item.Value.Count), 3);
                    double factor = item.Value.ReducedCount < _maxPoints ? -1 : 1; // pour la correction, si besoin
                    while (item.Value.ReducedCount != _maxPoints) // parfois erreur de +/- 1
                    {
                        // on modifie légèrement le facteur de reduction pour arriver au nombre de points désiré
                        item.Value.ReductionFactor += 0.001 * factor;
                    }
                }
            }
        }

        public void ApplyCommonScaleFactor()
        {
            foreach (var item in Items)
            {
                if (item.Value != null)
                {
                    item.Value.ScaleFactor = _commonScaleFactor;
                }
            }
        }

        public void ApplyCommonAxisScaleFactor()
        {
            foreach (var item in Items)
            {
                if (item.Value != null)
                {
                    item.Value.AxisScaleFactor = _commonAxisScaleFactor;
                }
            }
        }

        public void ApplyCommonPointRadius()
        {
            foreach (var item in Items)
            {
                if (item.Key != null)
                {
                    item.Key.PointRadius = _commonPointRadius;
                }
            }
        }

        public void ApplyCommonRenderQuality()
        {
            foreach (var item in Items)
            {
                if (item.Key != null)
                {
                    item.Key.RenderQuality = _commonRenderQuality;
                }
            }
        }

        public void ApplyCommonDisplayBarycenter()
        {
            foreach (var item in Items)
            {
                if (item.Key != null)
                {
                    item.Key.DisplayBarycenter = _commonDisplayBarycenter;
                }
            }
        }

        public void ApplyCommonDisplayAveragePlan()
        {
            foreach (var item in Items)
            {
                if (item.Key != null)
                {
                    item.Key.DisplayAveragePlan = _commonDisplayAveragePlan;
                }
            }
        }

        public void ApplyCommonDisplayBase()
        {
            foreach (var item in Items)
            {
                if (item.Key != null)
                {
                    item.Key.DisplayBase = _commonDisplayBase;
                }
            }
        }

        public void AddBuilder(ScatterGraphBuilderBase builder)
        {
            if (!ContainsBuilder(builder))
            {
                Items.Add(new KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult>(builder, new ScatterGraphBuildResult()));
            }
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

        public bool ContainsBuilder(ScatterGraphBuilderBase builder)
        {
            return Items.Any(x => x.Key.Equals(builder));
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
                item.Value.ComputeScaleFactor();
            }
            else
            {
                item.Value.SetFrom(result);
            }
        }

        public async Task BuildAllAsync()
        {
            foreach (var item in Items)
            {
                if (item.Value is null || !item.Value.IsSuccess || item.Key.State is ScatterGraphBuilderState.Ready)
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
                item.Value.SetFrom(result);
            }
        }

        public void Load(KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> item)
        {
            if (item.Key.State is ScatterGraphBuilderState.Success && item.Value != null && item.Value.IsSuccess && !item.Value.IsAdded)
            {
                if (item.Value.HasToReduce && item.Value.ReductionFactor > 0)
                {
                    item.Value.Reduce();
                }
                ScatterGraphViewModel? scatterGraphViewModel = null;
                try
                {
                    scatterGraphViewModel = new ScatterGraphViewModel(item.Value.ScatterGraph!, item.Key.Color, item.Value.ScaleFactor, item.Value.AxisScaleFactor, item.Key.PointRadius, item.Key.RenderQuality, !item.Key.DisplayBarycenter, !item.Key.DisplayAveragePlan, !item.Key.DisplayBase)
                    {
                        Name = item.Key.Name.Replace(" builder", ""),
                        Part = item.Key.Part
                    };
                    if (item.Key is ScatterGraphFileBuilder fileBuilder)
                    {
                        scatterGraphViewModel.Name = fileBuilder.FileName;
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    MessageBox.Show(e.Message, e.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if (scatterGraphViewModel != null)
                {
                    _mainViewModel.AddScatterGraph(scatterGraphViewModel);
                    item.Value.SetAddedToTrue();
                    Items.Remove(item);
                }
            }
        }

        public void LoadAll(bool closeWindow = false)
        {
            List<PartViewModelBase> parts = new List<PartViewModelBase>();
            foreach (var kvo in Items)
            {
                if (kvo.Key != null && kvo.Key.Part != null && !parts.Contains(kvo.Key.Part))
                {
                    parts.Add(kvo.Key.Part);
                    kvo.Key.Part.DisableRecomputeAllAfterScatterGraphsChanged();
                }
            }

            KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> item;

            for (int i = 0; i < Items.Count; i++)
            {
                item = Items[i];
                Load(item);
                if (item.Value != null && item.Value.IsAdded)
                {
                    i--;
                }
            }

            foreach (PartViewModelBase part in parts)
            {
                part.EnableRecomputeAllAfterScatterGraphsChanged();
                part.ComputeAll();
            }

            if (closeWindow && Items.Count == 0)
            {
                _view.Close();
            }
        }

        public Task LoadAsync(KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> item)
        {
            return Task.Run(() => Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    Load(item);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }));
        }

        public Task LoadAllAsync(bool closeWindow = false)
        {
            return Task.Run(() => Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    LoadAll(closeWindow);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }));
        }
    }
}

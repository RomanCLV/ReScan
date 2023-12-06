using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Collections.Specialized;
using ReScanVisualizer.Models;
using ReScanVisualizer.Commands;
using System.Xml.Linq;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public Model3DGroup OriginModel { get; private set; }

        private ViewModelBase? _selectedViewModel;
        public ViewModelBase? SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                // unselect all
                if (_selectedViewModel != null && !_selectedViewModel.Equals(value))
                {
                    ScatterGraphs.AsParallel().ForAll(x => x.Unselect());
                    Bases.AsParallel().ForAll(x => x.Unselect());
                    if (_selectedViewModel is I3DElement element)
                    {
                        element.Unselect();
                    }
                    if (_selectedViewModel is BaseViewModel baseViewModel)
                    {
                        baseViewModel.Base.Unselect();
                        baseViewModel.Dispose();
                    }
                }

                if (SetValue(ref _selectedViewModel, value))
                {
                    if (_selectedViewModel is IScatterGraphElement scatterGraphElement)
                    {
                        scatterGraphElement.Select(true);
                    }
                    else if (_selectedViewModel is I3DElement element)
                    {
                        element.Select();
                    }
                    if (_selectedViewModel is BaseViewModel baseViewModel)
                    {
                        baseViewModel.Base.Select(true);
                        baseViewModel.UpdateReorientCartesianFromBase();
                        baseViewModel.UpdateRotationXYZFromBase();
                    }
                }
            }
        }

        public ObservableCollection<ScatterGraphViewModel> ScatterGraphs { get; private set; }

        public ObservableCollection<Base3DViewModel> Bases { get; private set; }

        public Model3DGroup Models { get; private set; }

        public Model3DGroup BasesModels { get; private set; }

        public ScatterGraphViewModelGroup ScatterGraphViewModelGroup { get; private set; }

        public CommandKey AddScatterGraphCommand { get; }

        public CommandKey AddBaseCommand { get; }

        public CommandKey ExportBaseCommand { get; }

        public MainViewModel()
        {
            IsDisposed = false;
            OriginModel = Helper3D.Helper3D.BuildBaseModel(new Point3D(), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), Brushes.Red, Brushes.Green, Brushes.Blue);

            Models = new Model3DGroup();
            BasesModels = new Model3DGroup();

            SelectedViewModel = null;

            ScatterGraphs = new ObservableCollection<ScatterGraphViewModel>();
            Bases = new ObservableCollection<Base3DViewModel>();
            ScatterGraphViewModelGroup = new ScatterGraphViewModelGroup();

            AddScatterGraphCommand = new CommandKey(new AddScatterGraphCommand(this), Key.A, ModifierKeys.Control | ModifierKeys.Shift, "Add a new scatter graph");
            AddBaseCommand = new CommandKey(new ActionCommand(AddBase), Key.B, ModifierKeys.Control | ModifierKeys.Shift, "Add a new base");
            ExportBaseCommand = new CommandKey(new ExportBasesCommand(this), Key.E, ModifierKeys.Control | ModifierKeys.Shift, "Export bases");

            ScatterGraphs.CollectionChanged += ScatterGraphs_CollectionChanged;
            Bases.CollectionChanged += Bases_CollectionChanged;
        }

        ~MainViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                ScatterGraphs.CollectionChanged -= ScatterGraphs_CollectionChanged;
                Bases.CollectionChanged -= Bases_CollectionChanged;
                ClearScatterGraphs();
                ClearBases();
                base.Dispose();
            }
        }

        private void AddBase()
        {
            Random random = new Random();
            double phi = random.NextDouble() * Math.PI;
            double theta = random.NextDouble() * Math.PI;
            double sint = Tools.Sin(theta);
            double cost = Tools.Cos(theta);
            double sinp = Tools.Sin(phi);
            double cosp = Tools.Cos(phi);

            Bases.Add(Base3DViewModel.CreateCountedInstance(new Base3D(new Point3D(sint * cosp, sint * sinp, cost))));
        }

        public void AddScatterGraph(ScatterGraphViewModel scatterGraphViewModel)
        {
            if (scatterGraphViewModel != null)
            {
                Application.Current.Dispatcher.Invoke(() => ScatterGraphs.Add(scatterGraphViewModel));
            }
        }

        private void ScatterGraphs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object? item in e.NewItems)
                    {
                        ScatterGraphViewModel graphViewModel = (ScatterGraphViewModel)item;
                        graphViewModel.Samples.CollectionChanged += Samples_CollectionChanged;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Model3DGroup group = new Model3DGroup();
                            group.Children.Add(graphViewModel.Model);
                            group.Children.Add(graphViewModel.Base3D.Model);
                            group.Children.Add(graphViewModel.Barycenter.Model);
                            group.Children.Add(graphViewModel.AveragePlan.Model);
                            Models.Children.Add(group);
                        });
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object? item in e.OldItems)
                    {
                        ScatterGraphViewModel graphViewModel = (ScatterGraphViewModel)item;
                        graphViewModel.Samples.CollectionChanged -= Samples_CollectionChanged;
                        for (int i = 0; i < Models.Children.Count; i++)
                        {
                            if (Models.Children[i] is Model3DGroup group)
                            {
                                if (group.Children[0].Equals(graphViewModel.Model))
                                {
                                    if (_selectedViewModel != null)
                                    {
                                        if (graphViewModel.Equals(_selectedViewModel) ||
                                            graphViewModel.AveragePlan.Equals(_selectedViewModel) ||
                                            graphViewModel.Barycenter.Equals(_selectedViewModel) ||
                                            (_selectedViewModel is BaseViewModel bvModel && graphViewModel.Base3D.Equals(bvModel.Base)) ||
                                            graphViewModel.Samples.Any(s => s.Equals(_selectedViewModel)))
                                        {
                                            SelectedViewModel = null;
                                        }
                                    }
                                    Models.Children.RemoveAt(i);
                                    i--;
                                }
                            }
                            else
                            {
                                throw new NotImplementedException("Model3D was expected to be a Model3DGroup.");
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Models.Children.Clear();
                    if (_selectedViewModel is BaseViewModel baseViewModel)
                    {
                        if (Bases.All(b => !b.Equals(baseViewModel.Base)))
                        {
                            SelectedViewModel = null;
                        }
                    }
                    else
                    {
                        SelectedViewModel = null;
                    }
                    break;
            }
        }

        private void Samples_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    bool found = false;
                    if (_selectedViewModel is SampleViewModel)
                    {
                        foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
                        {
                            if (scatterGraphViewModel.Samples.Contains(_selectedViewModel))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            SelectedViewModel = null;
                        }
                    }
                    break;
            }
        }

        private void Bases_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object? item in e.NewItems)
                    {
                        BasesModels.Children.Add(((Base3DViewModel)item).Model);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object? item in e.OldItems)
                    {
                        for (int i = 0; i < BasesModels.Children.Count; i++)
                        {
                            Base3DViewModel base3D = (Base3DViewModel)item;
                            if (BasesModels.Children[i].Equals(base3D.Model))
                            {
                                if (SelectedViewModel is BaseViewModel baseViewModel)
                                {
                                    if (baseViewModel.Base.Equals(item))
                                    {
                                        SelectedViewModel = null;
                                    }
                                }
                                BasesModels.Children.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    BasesModels.Children.Clear();
                    if (SelectedViewModel is BaseViewModel baseViewModel2)
                    {
                        if (ScatterGraphs.All(s => !s.Base3D.Equals(baseViewModel2.Base)))
                        {
                            SelectedViewModel = null;
                        }
                    }
                    break;
            }
        }

        public void ClearScatterGraphs()
        {
            foreach (ScatterGraphViewModel item in ScatterGraphs)
            {
                item.Samples.CollectionChanged -= Samples_CollectionChanged;
                item.Dispose();
            }
            Application.Current?.Dispatcher.Invoke(ScatterGraphs.Clear);
        }

        public void ClearBases()
        {
            Application.Current?.Dispatcher.Invoke(Bases.Clear);
        }

        internal void SetAllRenderQuality(RenderQuality renderQuality)
        {
            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                scatterGraphViewModel.RenderQuality = renderQuality;
            }
        }

        public void RandomizeAllColors()
        {
            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                scatterGraphViewModel.Color.Color = Tools.GetRandomLightColor();
            }
        }

        public void RandomizeAllColorsAsync()
        {
            //return Task.Run(() =>
            //{
            //    foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            //    {
            //        Task.Delay(20).Wait();
            //        scatterGraphViewModel.Color.Color = Tools.GetRandomLightColor();
            //    }
            //});
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
                {
                    Task.Delay(20).Wait();
                    scatterGraphViewModel.Color.Color = Tools.GetRandomLightColor();
                }
            });
        }

        public void ShowAllGraphs()
        {
            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                scatterGraphViewModel.Show();
            }
        }

        public void HideAllGraphs()
        {
            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                scatterGraphViewModel.Hide();
            }
        }

        public void ShowAllPointsGraphs()
        {
            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                scatterGraphViewModel.ShowPoints();
            }
        }

        public void HideAllPointsGraphs()
        {
            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                scatterGraphViewModel.HidePoints();
            }
        }

        public void ShowAllBarycenters()
        {
            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                scatterGraphViewModel.Barycenter.Show();
            }
        }

        public void HideAllBarycenters()
        {
            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                scatterGraphViewModel.Barycenter.Hide();
            }
        }

        public void ShowAllAveragePlans()
        {
            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                scatterGraphViewModel.AveragePlan.Show();
            }
        }

        public void HideAllAveragePlans()
        {
            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                scatterGraphViewModel.AveragePlan.Hide();
            }
        }

        public void ShowAllBasesGraphs()
        {
            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                scatterGraphViewModel.Base3D.Show();
            }
        }

        public void HideAllBasesGraphs()
        {
            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                scatterGraphViewModel.Base3D.Hide();
            }
        }

        public void ShowAllAddedBases()
        {
            foreach (Base3DViewModel base3DViewModel in Bases)
            {
                base3DViewModel.Show();
            }
        }

        public void HideAllAddedBases()
        {
            foreach (Base3DViewModel base3DViewModel in Bases)
            {
                base3DViewModel.Hide();
            }
        }

        public bool IsBelongingToOriginModel(GeometryModel3D hitgeo)
        {
            return OriginModel.Children.Any(x => x.Equals(hitgeo));
        }

        public void SelectHitGeometry(GeometryModel3D hitgeo)
        {
            foreach (Base3DViewModel base3DViewModel in Bases)
            {
                if (base3DViewModel.IsBelongingToModel(hitgeo))
                {
                    SelectedViewModel = new BaseViewModel(base3DViewModel);
                    return;
                }
            }

            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                if (scatterGraphViewModel.IsBelongingToModel(hitgeo))
                {
                    if (scatterGraphViewModel.IsSelected /*scatterGraphViewModel.Equals(SelectedViewModel)*/)
                    {
                        if (scatterGraphViewModel.IsSelected)
                        {
                            SampleViewModel svm;
                            if (scatterGraphViewModel.Barycenter.IsBelongingToModel(hitgeo))
                            {
                                SelectedViewModel = scatterGraphViewModel.Barycenter;
                            }
                            else if (scatterGraphViewModel.AveragePlan.IsBelongingToModel(hitgeo))
                            {
                                SelectedViewModel = scatterGraphViewModel.AveragePlan;
                            }
                            else if (scatterGraphViewModel.Base3D.IsBelongingToModel(hitgeo))
                            {
                                SelectedViewModel = new BaseViewModel(scatterGraphViewModel.Base3D);
                            }
                            else if ((svm = scatterGraphViewModel.Samples.First(x => x.IsBelongingToModel(hitgeo))) != null)
                            {
                                SelectedViewModel = svm;
                            }
                            if (SelectedViewModel is I3DElement element)
                            {
                                element.Select();
                            }
                            scatterGraphViewModel.Select();
                        }
                    }
                    else
                    {
                        SelectedViewModel = scatterGraphViewModel;
                    }
                    return;
                }
            }
        }

        private I3DElement? _viewModelMouseOver;

        public void UnselectMouseOverGeometry()
        {
            if (_viewModelMouseOver != null)
            {
                if (_viewModelMouseOver is ScatterGraphViewModel scatterGraphViewModel)
                {
                    scatterGraphViewModel.Base3D.IsMouseOver = false;
                }
                _viewModelMouseOver.IsMouseOver = false;
            }
        }

        public void SelectMouseOverGeometry(GeometryModel3D geometryModel3DMouseOver)
        {
            foreach (Base3DViewModel base3DViewModel in Bases)
            {
                if (base3DViewModel.IsBelongingToModel(geometryModel3DMouseOver))
                {
                    _viewModelMouseOver = base3DViewModel;
                    _viewModelMouseOver.IsMouseOver = true;
                    return;
                }
            }

            foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
            {
                if (scatterGraphViewModel.IsBelongingToModel(geometryModel3DMouseOver))
                {
                    _viewModelMouseOver = scatterGraphViewModel;
                    _viewModelMouseOver.IsMouseOver = true;
                    scatterGraphViewModel.Base3D.IsMouseOver = true;
                    return;
                }
            }
        }
    }
}

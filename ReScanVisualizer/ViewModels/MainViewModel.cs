using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ReScanVisualizer.Models;
using ReScanVisualizer.Commands;
using ReScanVisualizer.ViewModels.Parts;
using ReScanVisualizer.ViewModels.Samples;
using ReScanVisualizer.Models.Pipes;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

#nullable enable

/*
TODO : upgrades to implement
- Split graph
- Fusion graphs
- Scatter graph -> section Transform
    -> translate
    -> rotate  
 */

namespace ReScanVisualizer.ViewModels
{
    public class MainViewModel : ViewModelBase, IPartSource
    {
        //public Model3DGroup OriginModel { get; private set; }

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
                    foreach (Base3DViewModel base3D in Bases)
                    {
                        base3D.Unselect();
                    }
                    if (_selectedViewModel is ISelectable element)
                    {
                        element.Unselect();
                    }
                    if (_selectedViewModel is BaseViewModel baseViewModel)
                    {
                        baseViewModel.EndRotateBase();
                        baseViewModel.Base.Unselect();
                        baseViewModel.Dispose();
                    }
                    if (_selectedViewModel is ScatterGraphGroupViewModel scatterGraphGroupViewModel)
                    {
                        scatterGraphGroupViewModel.EndRotateBase();
                    }
                }

                if (SetValue(ref _selectedViewModel, value))
                {
                    if (_selectedViewModel is ISelectable element)
                    {
                        element.Select();
                    }
                    if (_selectedViewModel is BaseViewModel baseViewModel)
                    {
                        baseViewModel.Base.Select(true);
                        baseViewModel.UpdateReorientCartesianFromBase();
                        baseViewModel.UpdateRotationXYZFromBase();
                        baseViewModel.UpdateAnglesFromCartesian();
                    }
                }
            }
        }

        public ObservableCollection<ScatterGraphViewModel> ScatterGraphs { get; private set; }

        public ObservableCollection<Base3DViewModel> Bases { get; private set; }

        public ObservableCollection<PartViewModelBase> Parts { get; private set; }

        IList<PartViewModelBase> IPartSource.Parts => Parts;

        public Model3DGroup Models { get; private set; }

        public Model3DGroup BasesModels { get; private set; }

        public Model3DGroup PartsModels { get; private set; }

        public ScatterGraphGroupViewModel ScatterGraphViewModelGroup { get; private set; }

        public CommandKey AddScatterGraphCommand { get; }

        public CommandKey AddBaseCommand { get; }

        public CommandKey AddPartCommand { get; }

        public CommandKey ImportBaseCommand { get; }

        public CommandKey ExportBaseCommand { get; }

        public CommandLinePipe ModifierPipe { get; private set; }

        private readonly List<PipeBase> _pipes;
        public IReadOnlyCollection<UDPPipe> UDPPipes => _pipes.OfType<UDPPipe>().ToList();

        private static readonly Lazy<MainViewModel> instance = new Lazy<MainViewModel>(() => new MainViewModel());

        private MainViewModel()
        {
            IsDisposed = false;

            //OriginModel = Helper3D.BuildBaseModel(new Point3D(), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), Brushes.Red, Brushes.Green, Brushes.Blue);
            ModifierPipe = new CommandLinePipe(this);
            _pipes = new List<PipeBase>() { ModifierPipe };

            Models = new Model3DGroup();
            BasesModels = new Model3DGroup();
            PartsModels = new Model3DGroup();

            SelectedViewModel = null;

            ScatterGraphs = new ObservableCollection<ScatterGraphViewModel>();
            Bases = new ObservableCollection<Base3DViewModel>();
            Parts = new ObservableCollection<PartViewModelBase>();
            ScatterGraphViewModelGroup = new ScatterGraphGroupViewModel
            {
                PartsListSource = this
            };

            AddScatterGraphCommand = new CommandKey(new AddScatterGraphCommand(this), Key.A, ModifierKeys.Control | ModifierKeys.Shift, "Add a new scatter graph");
            AddBaseCommand = new CommandKey(new ActionCommand(AddBase), Key.B, ModifierKeys.Control | ModifierKeys.Shift, "Add a new base");
            AddPartCommand = new CommandKey(new AddPartCommand(this), Key.P, ModifierKeys.Control | ModifierKeys.Shift, "Add a new part");
            ImportBaseCommand = new CommandKey(new ImportBasesCommand(this), Key.R, ModifierKeys.Control | ModifierKeys.Shift, "Import bases");
            ExportBaseCommand = new CommandKey(new ExportBasesCommand(this), Key.E, ModifierKeys.Control | ModifierKeys.Shift, "Export bases");

            ScatterGraphs.CollectionChanged += ScatterGraphs_CollectionChanged;
            Bases.CollectionChanged += Bases_CollectionChanged;
            Parts.CollectionChanged += Parts_CollectionChanged;
        }

        ~MainViewModel()
        {
            Dispose();
        }

        public static MainViewModel GetInstance()
        {
            return instance.Value;
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                foreach (PipeBase pipe in _pipes)
                {
                    pipe.Stop();
                }

                foreach (PartViewModelBase part in Parts)
                {
                    part.DisableRecomputeAllAfterScatterGraphsChanged();
                }

                if (_selectedViewModel is BaseViewModel baseViewModel)
                {
                    baseViewModel.Dispose();
                }
                _selectedViewModel = null;
                AddScatterGraphCommand.Dispose();
                AddBaseCommand.Dispose();
                AddPartCommand.Dispose();
                ExportBaseCommand.Dispose();
                ScatterGraphViewModelGroup.Dispose();
                foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
                {
                    scatterGraphViewModel.Part = null;
                    scatterGraphViewModel.PartsListSource = null;
                }
                Parts.CollectionChanged -= Parts_CollectionChanged;
                ScatterGraphs.CollectionChanged -= ScatterGraphs_CollectionChanged;
                Bases.CollectionChanged -= Bases_CollectionChanged;
                ClearParts();
                ClearScatterGraphs();
                ClearBases();

                Models.Children.Clear();
                BasesModels.Children.Clear();
                PartsModels.Children.Clear();

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

            Base3DViewModel base3DViewModel = Base3DViewModel.CreateCountedInstance(new Base3D(new Point3D(sint * cosp, sint * sinp, cost)));
            Bases.Add(base3DViewModel);
            SelectedViewModel = new BaseViewModel(base3DViewModel);
        }

        public void AddScatterGraph(ScatterGraphViewModel scatterGraphViewModel)
        {
            if (scatterGraphViewModel != null)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    ScatterGraphs.Add(scatterGraphViewModel);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() => ScatterGraphs.Add(scatterGraphViewModel));
                }
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
                        graphViewModel.PartsListSource = this;
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
                            graphViewModel.Dispose();
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Models.Children.Clear();
                    if (_selectedViewModel is ScatterGraphViewModel)
                    {
                        SelectedViewModel = null;
                    }
                    else if (_selectedViewModel is BaseViewModel baseViewModel)
                    {
                        if (Bases.All(b => !b.Equals(baseViewModel.Base)))
                        {
                            SelectedViewModel = null;
                        }
                    }
                    else if (_selectedViewModel is IScatterGraphElement scatterGraphElement)
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
                    foreach (object? item in e.OldItems)
                    {
                        SampleViewModel sampleViewModel = (SampleViewModel)item;
                        if (sampleViewModel.Equals(_selectedViewModel))
                        {
                            SelectedViewModel = null;
                        }
                        sampleViewModel.Dispose();
                    }
                    break;

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
                                if (_selectedViewModel is BaseViewModel baseViewModel)
                                {
                                    if (baseViewModel.Base.Equals(item))
                                    {
                                        SelectedViewModel = null;
                                    }
                                }
                                BasesModels.Children.RemoveAt(i);
                                i--;
                                base3D.Dispose();
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    BasesModels.Children.Clear();
                    if (_selectedViewModel is BaseViewModel baseViewModel2)
                    {
                        if (ScatterGraphs.All(s => !s.Base3D.Equals(baseViewModel2.Base)))
                        {
                            SelectedViewModel = null;
                        }
                    }
                    break;
            }
        }

        private void Parts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object? item in e.NewItems)
                    {
                        PartsModels.Children.Add(((PartViewModelBase)item).Model);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object? item in e.OldItems)
                    {
                        PartViewModelBase partViewModelBase = (PartViewModelBase)item;
                        foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
                        {
                            if (partViewModelBase.Equals(scatterGraphViewModel.Part))
                            {
                                scatterGraphViewModel.Part = null;
                            }
                            PartsModels.Children.Remove(partViewModelBase.Model);
                            partViewModelBase.Dispose();
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (ScatterGraphViewModel scatterGraphViewModel in ScatterGraphs)
                    {
                        scatterGraphViewModel.Part = null;
                    }
                    PartsModels.Children.Clear();
                    if (_selectedViewModel is PartViewModelBase)
                    {
                        SelectedViewModel = null;
                    }
                    break;
            }
        }

        public void ClearScatterGraphs()
        {
            foreach (PartViewModelBase part in Parts)
            {
                part.DisableRecomputeAllAfterScatterGraphsChanged();
            }
            foreach (ScatterGraphViewModel item in ScatterGraphs)
            {
                item.Samples.CollectionChanged -= Samples_CollectionChanged;
                item.Dispose();
            }
            ScatterGraphs.Clear();
            foreach (PartViewModelBase part in Parts)
            {
                part.EnableRecomputeAllAfterScatterGraphsChanged();
            }
        }

        public void ClearBases()
        {
            foreach (Base3DViewModel item in Bases)
            {
                item.Dispose();
            }
            Bases.Clear();
        }

        public void ClearParts()
        {
            foreach (PartViewModelBase item in Parts)
            {
                item.Dispose();
            }
            Parts.Clear();
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

        //public bool IsBelongingToOriginModel(GeometryModel3D hitgeo)
        //{
        //    return OriginModel.Children.Any(x => x.Equals(hitgeo));
        //}

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

            foreach (PartViewModelBase partViewModelBase in Parts)
            {
                if (partViewModelBase.IsBelongingToModel(geometryModel3DMouseOver))
                {
                    _viewModelMouseOver = partViewModelBase;
                    _viewModelMouseOver.IsMouseOver = true;
                }
            }
        }

        public bool StartUDPPipe(ushort port)
        {
            if (Tools.IsPortInUse(port, ProtocolType.Tcp) ||
                Tools.IsPortInUse(port, ProtocolType.Udp))
            {
                MessageBox.Show($"The port {port} is already used.", "UDP error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    UDPPipe udpPipe = new UDPPipe(this, port);
                    udpPipe.ErrorThrowed += Pipe_ErrorThrowed;
                    udpPipe.Start();
                    _pipes.Add(udpPipe);
                    OnPropertyChanged(nameof(UDPPipes));
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return false;
        }

        public void StopUDPPipe(ushort port)
        {
            for (int i = 0; i < _pipes.Count; i++)
            {
                if (_pipes[i] is UDPPipe udpPipe && udpPipe.Port == port)
                {
                    udpPipe.ErrorThrowed -= Pipe_ErrorThrowed;
                    try
                    {
                        udpPipe.Stop();
                        _pipes.RemoveAt(i);
                        i--;
                        OnPropertyChanged(nameof(UDPPipes));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, e.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Pipe_ErrorThrowed(object sender, Exception e)
        {
            // TODO: manage error
#if DEBUG
            Trace.WriteLine($"UDPPipe error ({e.GetType().Name}): {e.Message}");
#endif
        }
    }
}

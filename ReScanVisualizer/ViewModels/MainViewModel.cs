using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Collections.ObjectModel;
using ReScanVisualizer.Views;
using System.Reflection;
using ReScanVisualizer.Commands;
using System.Windows.Input;
using HelixToolkit.Wpf;
using System.Windows;
using ReScanVisualizer.Models;
using System.Security.Cryptography;
using System.Collections.Specialized;

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
                if (_selectedViewModel is BaseViewModel && !_selectedViewModel.Equals(value))
                {
                    _selectedViewModel.Dispose();
                }
                SetValue(ref _selectedViewModel, value);
            }
        }

        public ObservableCollection<ScatterGraphViewModel> ScatterGraphs { get; private set; }

        public ObservableCollection<Base3DViewModel> Bases { get; private set; }

        public Model3DGroup Models { get; private set; }

        public Model3DGroup BasesModels { get; private set; }

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
                base.Dispose();
            }
        }

        private void AddBase()
        {
            Bases.Add(Base3DViewModel.CreateCountedInstance(new Base3D(new Point3D(1, 1, 1))));
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
                        for (int i = 0; i < Models.Children.Count; i++)
                        {
                            if (Models.Children[i] is Model3DGroup group)
                            {
                                if (group.Children[0].Equals(graphViewModel.Model))
                                {
                                    if (SelectedViewModel != null)
                                    {
                                        if (graphViewModel.Equals(SelectedViewModel) ||
                                            graphViewModel.AveragePlan.Equals(SelectedViewModel) ||
                                            graphViewModel.Barycenter.Equals(SelectedViewModel) ||
                                            (SelectedViewModel is BaseViewModel bvModel && graphViewModel.Base3D.Equals(bvModel.Base)) ||
                                            graphViewModel.Samples.Any(s => s.Equals(SelectedViewModel)))
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
                    if (SelectedViewModel is BaseViewModel baseViewModel)
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
    }
}

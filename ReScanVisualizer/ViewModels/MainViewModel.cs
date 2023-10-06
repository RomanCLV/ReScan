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
            set => SetValue(ref _selectedViewModel, value);
        }

        public ObservableCollection<ScatterGraphViewModel> ScatterGraphs { get; private set; }

        public Model3DGroup Models { get; private set; }

        public CommandKey AddScatterGraphCommand { get; }

        public MainViewModel()
        {
            IsDisposed = false;
            AddScatterGraphCommand = new CommandKey(new AddScatterGraphCommand(this), Key.A, ModifierKeys.Control | ModifierKeys.Shift, "Add a new scatter graph");

            OriginModel = new Model3DGroup();
            OriginModel.Children.Add(Helper3D.Helper3D.BuildArrowModel(new Point3D(), new Point3D(1, 0, 0), 0.1, Brushes.Red));
            OriginModel.Children.Add(Helper3D.Helper3D.BuildArrowModel(new Point3D(), new Point3D(0, 1, 0), 0.1, Brushes.Green));
            OriginModel.Children.Add(Helper3D.Helper3D.BuildArrowModel(new Point3D(), new Point3D(0, 0, 1), 0.1, Brushes.Blue));

            Models = new Model3DGroup();
            SelectedViewModel = null;

            ScatterGraphs = new ObservableCollection<ScatterGraphViewModel>();
            ScatterGraphs.CollectionChanged += ScatterGraphs_CollectionChanged;
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
                base.Dispose();
            }
        }

        private void ScatterGraphs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (object? item in e.NewItems)
                    {
                        if (item is ScatterGraphViewModel graphViewModel)
                        {
                            Model3DGroup group = new Model3DGroup();
                            group.Children.Add(graphViewModel.Model);
                            group.Children.Add(graphViewModel.Barycenter.Model);
                            group.Children.Add(graphViewModel.AveragePlan.Model);
                            Models.Children.Add(group);
                        }
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (object? item in e.NewItems)
                    {
                        if (item is ScatterGraphViewModel graphViewModel)
                        {
                            for (int i = 0; i < Models.Children.Count; i++)
                            {
                                if (Models.Children[i] is Model3DGroup group)
                                {
                                    if (group.Children[0].Equals(graphViewModel.Model))
                                    {
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
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    Models.Children.Clear();
                    break;
            }
        }
    }
}

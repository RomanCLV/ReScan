using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ReScanVisualizer.Models;

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders
{
    internal class ScatterGraphPopulateLineBuilder : ScatterGraphPopulateBuilderBase
    {
        public Point3DViewModel Start { get; private set; }

        public Point3DViewModel End { get; private set; }

        public double Width { get; private set; }

        private uint _numPoints;
        public uint NumPoints
        {
            get => _numPoints;
            set
            {
                if (value < 2)
                {
                    value = 2;
                }
                else if (value > 5 * Width)
                {
                    value = (uint)(5 * Width);
                }
                if (SetValue(ref _numPoints, value))
                {
                    OnPropertyChanged(nameof(Details));
                    State = ScatterGraphBuilderState.Ready;
                    ModelHasToUpdate = true;
                }
            }
        }

        public override string Name => "Line builder";

        public override string Details =>
            $"Start: {Start}\n" +
            $"End: {End}\n" +
            $"Num points: {NumPoints}";

        private bool _modelHasToUpdate;
        private bool ModelHasToUpdate
        {
            set
            {
                _modelHasToUpdate = value;
                if (_modelHasToUpdate && _autoUpdateBuilderModel)
                {
                    UpdateBuilderModel();
                }
            }
        }

        private bool _autoUpdateBuilderModel;
        public bool AutoUpdateBuilderModel
        {
            get => _autoUpdateBuilderModel;
            set
            {
                if (SetValue(ref _autoUpdateBuilderModel, value) && _autoUpdateBuilderModel && _modelHasToUpdate)
                {
                    UpdateBuilderModel();
                }
            }
        }

        private readonly ScatterGraphBuilderVisualizerViewModel _scatterGraphBuilderVisualizerViewModel;
        public ScatterGraphBuilderVisualizerViewModel ScatterGraphBuilderVisualizerViewModel
        {
            get => _scatterGraphBuilderVisualizerViewModel;
        }

        public ScatterGraphPopulateLineBuilder()
        {
            Start = new Point3DViewModel();
            End = new Point3DViewModel(new Point3D(1, 0, 0));
            _numPoints = 2;
            _scatterGraphBuilderVisualizerViewModel = new ScatterGraphBuilderVisualizerViewModel();
            _modelHasToUpdate = false;
            _autoUpdateBuilderModel = true;
            UpdateWidth();

            Start.PropertyChanged += StartEnd_PropertyChanged;
            End.PropertyChanged += StartEnd_PropertyChanged;
            UpdateBuilderModel();
        }

        ~ScatterGraphPopulateLineBuilder()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                Start.PropertyChanged -= StartEnd_PropertyChanged;
                End.PropertyChanged -= StartEnd_PropertyChanged;
                _scatterGraphBuilderVisualizerViewModel.Dispose();
                Start.Dispose();
                End.Dispose();
                base.Dispose();
            }
        }

        private void StartEnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Point3DViewModel.Point))
            {
                UpdateWidth();
                State = Start.Point != End.Point ? ScatterGraphBuilderState.Ready : ScatterGraphBuilderState.Error;
                if (IsReady)
                {
                    Message = string.Empty;
                }
                else
                {
                    Message = "Start point and End point must be different!";
                }
                OnPropertyChanged(nameof(Details));
                ModelHasToUpdate = true;
            }
        }

        private void UpdateWidth()
        {
            Width = (End.Point - Start.Point).Length;
            NumPoints = (uint)Math.Ceiling(Width) + 1;
            if (_numPoints == 1)
            {
                NumPoints++;
            }
        }

        private ScatterGraph BuildScatterGraph()
        {
            ScatterGraph graph = new ScatterGraph();
            ScatterGraph.PopulateLine(graph, Start.Point, End.Point, _numPoints);
            return graph;
        }

        private void UpdateBuilderModel()
        {
            try
            {
                ScatterGraph scatterGraph = BuildScatterGraph();
                if (scatterGraph.Count <= 5000 || MessageBox.Show($"Warning: Are you sure to display {scatterGraph.Count} points? It will take some time to display.", "Huge points to display", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _modelHasToUpdate = false;
                    _scatterGraphBuilderVisualizerViewModel.BuildBuilderModel(scatterGraph);
                }
            }
            catch
            {
                _scatterGraphBuilderVisualizerViewModel.ClearBuilderModel();
            }
        }

        /// <returns>Return a <see cref="ScatterGraphBuildResult"/> using the <see cref="ScatterGraph.PopulateLine(ScatterGraph, Point3D, Point3D, uint)"/> method.</returns>
        public override ScatterGraphBuildResult Build()
        {
            Application.Current.Dispatcher.Invoke(() => State = ScatterGraphBuilderState.Working);
            ScatterGraphBuildResult scatterGraphBuildResult;
            try
            {
                scatterGraphBuildResult = new ScatterGraphBuildResult(BuildScatterGraph());
                State = ScatterGraphBuilderState.Success;
            }
            catch (Exception e)
            {
                scatterGraphBuildResult = new ScatterGraphBuildResult(e);
                State = ScatterGraphBuilderState.Error;
            }
            return scatterGraphBuildResult;
        }
    }
}

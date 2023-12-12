using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ReScanVisualizer.Models;

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders
{
    public class ScatterGraphPopulateLineBuilder : ScatterGraphPopulateBuilderBase
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
                }
            }
        }

        public override string Name => "Line builder";

        public override string Details => 
            $"Start: {Start}\n" +
            $"End: {End}\n" +
            $"Num points: {NumPoints}";

        public ScatterGraphPopulateLineBuilder()
        {
            Start = new Point3DViewModel();
            End = new Point3DViewModel(new Point3D(1, 0, 0));
            _numPoints = 2;
            UpdateWidth();

            Start.PropertyChanged += StartEnd_PropertyChanged;
            End.PropertyChanged += StartEnd_PropertyChanged;
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

        /// <returns>Return a <see cref="ScatterGraphBuildResult"/> using the <see cref="ScatterGraph.PopulateLine(ScatterGraph, Point3D, Point3D, uint)"/> method.</returns>
        public override ScatterGraphBuildResult Build()
        {
            Application.Current.Dispatcher.Invoke(() => State = ScatterGraphBuilderState.Working);
            ScatterGraphBuildResult scatterGraphBuildResult;
            try
            {
                ScatterGraph graph = new ScatterGraph();
                ScatterGraph.PopulateLine(graph, Start.Point, End.Point, _numPoints);
                scatterGraphBuildResult = new ScatterGraphBuildResult(graph);
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

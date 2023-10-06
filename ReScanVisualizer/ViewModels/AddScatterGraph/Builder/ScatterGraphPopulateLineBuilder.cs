using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
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
                SetValue(ref _numPoints, value);
            }
        }

        private bool _isDisposed;

        public ScatterGraphPopulateLineBuilder()
        {
            _isDisposed= false;
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

        private void StartEnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Point3DViewModel.Point))
            {
                UpdateWidth();
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

        /// <summary>
        /// Build an array of <see cref="ScatterGraphBuildResult"/> using the <see cref="ScatterGraph.PopulateLine(ScatterGraph, Point3D, Point3D, uint)"/> method.
        /// </summary>
        /// <returns>Return an array of one  <see cref="ScatterGraphBuildResult"/></returns>
        public override ScatterGraphBuildResult[] Build()
        {
            ScatterGraph graph = new ScatterGraph();
            ScatterGraph.PopulateLine(graph, Start.Point, End.Point, _numPoints);
            return new ScatterGraphBuildResult[1] { new ScatterGraphBuildResult(Color, graph) };
        }

        public override void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                Start.PropertyChanged -= StartEnd_PropertyChanged;
                End.PropertyChanged -= StartEnd_PropertyChanged;
                base.Dispose();
            }
        }
    }
}

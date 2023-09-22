using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public class ScatterGraphPopulateRandomBuilder : ScatterGraphPopulateBuilderBase
    {
        private int _count;
        public int Count
        {
            get => _count;
            set => SetValue(ref _count, value);
        }

        private double _minX;
        public double MinX
        {
            get => _minX;
            set => SetValue(ref _minX, value);
        }

        private double _maxX;
        public double MaxX
        {
            get => _maxX;
            set => SetValue(ref _maxX, value);
        }

        private double _minY;
        public double MinY
        {
            get => _minY;
            set => SetValue(ref _minY, value);
        }

        private double _maxY;
        public double MaxY
        {
            get => _maxY;
            set => SetValue(ref _maxY, value);
        }

        private double _minZ;
        public double MinZ
        {
            get => _minZ;
            set => SetValue(ref _minZ, value);
        }

        private double _maxZ;
        public double MazZ
        {
            get => _maxZ;
            set => SetValue(ref _maxZ, value);
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetValue(ref _color, value);
        }

        public ScatterGraphPopulateRandomBuilder()
        {
            _count = 5;
            _minX = -10;
            _maxX =  10;
            _minY = -10;
            _maxY =  10;
            _minZ = -10;
            _maxZ =  10;
            _color = Colors.White;
        }

        /// <summary>
        /// Build an array of ScatterGraphViewModel using the <see cref="ScatterGraph.PopulateRandom(ScatterGraph, int, double, double, double, double, double, double)"/> method.
        /// </summary>
        /// <returns>Return an array of one ScatterGraph</returns>
        public override ScatterGraphViewModel[] Build()
        {
            ScatterGraph graph = new ScatterGraph();
            ScatterGraph.PopulateRandom(graph, _count, _minX, _maxX, _minY, _maxY, _minZ, _maxZ);
            return new ScatterGraphViewModel[1] { new ScatterGraphViewModel(graph, _color) };
        }
    }
}

using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public class ScatterGraphEmptyBuilder : ScatterGraphBuilderBase
    {
        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                SetValue(ref _color, value);
            }
        }

        public ScatterGraphEmptyBuilder()
        {
            _color = Colors.White;
        }

        /// <summary>
        /// Build an array of ScatterGraphViewModel
        /// </summary>
        /// <returns>Return an array of one ScatterGraph</returns>
        public override ScatterGraphViewModel[] Build()
        {
            return new ScatterGraphViewModel[1] { new ScatterGraphViewModel(new ScatterGraph(), _color) };
        }
    }
}

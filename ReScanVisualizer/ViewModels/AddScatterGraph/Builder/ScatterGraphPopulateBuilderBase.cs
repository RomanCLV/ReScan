using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public abstract class ScatterGraphPopulateBuilderBase : ScatterGraphBuilderBase
    {
        public ScatterGraphPopulateBuilderBase() : this(Colors.White)
        {
        }

        public ScatterGraphPopulateBuilderBase(Color color) : base(color)
        {
        }
    }
}

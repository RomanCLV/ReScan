using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public abstract class ScatterGraphBuilderBase : ViewModelBase
    {
        /// <summary>
        /// Build an array of ScatterGraphViewModel
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public abstract ScatterGraphViewModel[] Build();
    }
}

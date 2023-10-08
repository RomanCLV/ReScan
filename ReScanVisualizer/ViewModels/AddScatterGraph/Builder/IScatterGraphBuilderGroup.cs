using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public interface IScatterGraphBuilderGroup
    {
        IEnumerable<ScatterGraphBuilderBase> Builders { get; }
    }
}

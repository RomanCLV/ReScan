using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels.Parts;

#nullable enable

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders
{
    public interface IScatterGraphBuilderGroup
    {
        IEnumerable<ScatterGraphBuilderBase> Builders { get; }
    }
}

using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public class ScatterGraphEmptyBuilder : ScatterGraphBuilderBase
    {
        public override string Name => "Empty";

        public ScatterGraphEmptyBuilder() : base(Colors.White)
        {
        }

        /// <returns>Return a <see cref="ScatterGraphBuildResult"/> with an empty <see cref="ScatterGraph"/></returns>
        public override ScatterGraphBuildResult Build()
        {
            State = ScatterGraphBuilderState.Success;
            return new ScatterGraphBuildResult(new ScatterGraph());
        }
    }
}

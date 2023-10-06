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
        public ScatterGraphEmptyBuilder() : base(Colors.White)
        {
        }

        /// <summary>
        /// Build an array of <see cref="ScatterGraphBuildResult"/>
        /// </summary>
        /// <returns>Return an array of one <see cref="ScatterGraphBuildResult"/></returns>
        public override ScatterGraphBuildResult[] Build()
        {
            return new ScatterGraphBuildResult[1] { new ScatterGraphBuildResult(Color, new ScatterGraph()) };
        }
    }
}

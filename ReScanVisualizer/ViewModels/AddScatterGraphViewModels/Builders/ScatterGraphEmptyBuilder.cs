using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ReScanVisualizer.Models;

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders
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
            Application.Current.Dispatcher.Invoke(() => State = ScatterGraphBuilderState.Working);
            ScatterGraphBuildResult scatterGraphBuildResult = new ScatterGraphBuildResult(new ScatterGraph());
            State = ScatterGraphBuilderState.Success;
            return scatterGraphBuildResult;
        }
    }
}

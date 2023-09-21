using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public class ScatterGraphFilesBuilder : ScatterGraphBuilderBase
    {
        public ObservableCollection<ScatterGraphFileBuilder> Builders { get; private set; }

        public ScatterGraphFilesBuilder()
        {
            Builders = new ObservableCollection<ScatterGraphFileBuilder>();
        }

        /// <summary>
        /// Build an array of ScatterGraphViewModel
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public override ScatterGraphViewModel[] Build()
        {
            ScatterGraphViewModel[] scatterGraphViewModels = new ScatterGraphViewModel[Builders.Count];
            for (int i = 0; i < Builders.Count; i++)
            {
                scatterGraphViewModels[i] = Builders[i].Build()[0];
            }
            return scatterGraphViewModels;
        }
    }
}

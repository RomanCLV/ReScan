using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public class ScatterGraphFileBuilder : ScatterGraphBuilderBase
    {
        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                if (SetValue(ref _path, value))
                {
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        public string FileName => System.IO.Path.GetFileName(_path);

        private bool _containsHeader;
        public bool ContainsHeader
        {
            get => _containsHeader;
            set => SetValue(ref _containsHeader, value);
        }

        public ScatterGraphFileBuilder(string path, Color color, bool containsHeader) : base(color)
        {
            _path = path;
            _containsHeader = containsHeader;
        }

        /// <summary>
        /// Build an array of <see cref="ScatterGraphBuildResult"/> using the <see cref="ScatterGraph.ReadCSV(string, bool)"/> method.
        /// </summary>
        /// <returns>Return an array of one <see cref="ScatterGraphBuildResult"/></returns>
        public override ScatterGraphBuildResult[] Build()
        {
            ScatterGraphBuildResult[] scatterGraphBuildResults = new ScatterGraphBuildResult[1];
            if (!File.Exists(_path))
            {
                scatterGraphBuildResults[1] = new ScatterGraphBuildResult(Color, new FileNotFoundException("File not found!", _path));
            }
            else
            {
                ScatterGraph graph;
                try
                {
                    graph = ScatterGraph.ReadCSV(_path, _containsHeader);
                    scatterGraphBuildResults[1] = new ScatterGraphBuildResult(Color, graph);
                }
                catch (Exception e)
                {
                    scatterGraphBuildResults[1] = new ScatterGraphBuildResult(Color, e);
                }
            }
            return scatterGraphBuildResults;
        }
    }
}

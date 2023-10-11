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

        public override string Name => $"File builder";
        public override string FullName => $"{Name} ({Path})";
        public override string Details => FileName;

        public ScatterGraphFileBuilder(string path, Color color, bool containsHeader) : base(color)
        {
            _path = path;
            _containsHeader = containsHeader;
        }

        /// <returns>Return a <see cref="ScatterGraphBuildResult"/> using the <see cref="ScatterGraph.ReadCSV(string, bool)"/> method.</returns>
        public override ScatterGraphBuildResult Build()
        {
            State = ScatterGraphBuilderState.Working;
            ScatterGraphBuildResult scatterGraphBuildResult;
            try
            {
                ScatterGraph graph = ScatterGraph.ReadCSV(_path, _containsHeader);
                scatterGraphBuildResult = new ScatterGraphBuildResult(graph);
                State = ScatterGraphBuilderState.Success;
            }
            catch (Exception e)
            {
                scatterGraphBuildResult = new ScatterGraphBuildResult(e);
                State = ScatterGraphBuilderState.Error;
            }
            return scatterGraphBuildResult;
        }
    }
}

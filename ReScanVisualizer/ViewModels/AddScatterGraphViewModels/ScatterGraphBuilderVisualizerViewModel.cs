using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders;

#nullable enable

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels
{
    internal class ScatterGraphBuilderVisualizerViewModel : ViewModelBase
    {
        private Model3DGroup _builderModel;
        public Model3D BuilderModel => _builderModel;

        private ScatterGraphBuilderBase? _builder;

        public ScatterGraphBuilderBase? Builder
        {
            get => _builder;
            set => SetValue(ref _builder, value);
        }

        public ScatterGraphBuilderVisualizerViewModel()
        {
            _builder = null;
            _builderModel = new Model3DGroup();
        }

        ~ScatterGraphBuilderVisualizerViewModel()
        {
            Dispose();
        }

        public void BuildBuilderModel(ScatterGraph scatterGraph)
        {
            _builderModel.Children.Clear();
            for (int i = 0; i < scatterGraph.Count; i++)
            {
                _builderModel.Children.Add(Helper3D.BuildSphereModel(scatterGraph[i], 0.25, Colors.White, RenderQuality.Medium));
            }
        }
    }
}

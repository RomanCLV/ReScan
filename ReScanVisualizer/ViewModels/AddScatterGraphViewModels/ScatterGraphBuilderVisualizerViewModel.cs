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

        public void BuildBuilderModel(ScatterGraph scatterGraph, double radius = 0.25)
        {
            _builderModel.Children.Clear();
            RenderQuality renderQuality;
            int count = scatterGraph.Count;
            if (count <= 500)
            {
                renderQuality = RenderQuality.VeryHigh;
            }
            else if (count <= 1000)
            {
                renderQuality = RenderQuality.High;
            }
            else if (count <= 1500)
            {
                renderQuality = RenderQuality.Medium;
            }
            else if (count <= 2000)
            {
                renderQuality = RenderQuality.Low;
            }
            else
            {
                renderQuality = RenderQuality.VeryLow;
            }

            for (int i = 0; i < count; i++)
            {
                _builderModel.Children.Add(Helper3D.BuildSphereModel(scatterGraph[i], radius, Colors.White, renderQuality));
            }
        }
    }
}

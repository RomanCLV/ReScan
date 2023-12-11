using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels.Samples;

namespace ReScanVisualizer.ViewModels
{
    public class AddPointViewModel : ViewModelBase
    {
        private readonly ScatterGraphViewModel _scatterGraphViewModel;

        public Point3DViewModel Point { get; private set; }

        public AddPointViewModel(ScatterGraphViewModel scatterGraphViewModel)
        {
            _scatterGraphViewModel = scatterGraphViewModel;
            Point = new Point3DViewModel();
        }

        public void AddPoint()
        {
            SampleViewModel sampleViewModel = new SampleViewModel(Point.Point, _scatterGraphViewModel.Color.Color, _scatterGraphViewModel.ScaleFactor, _scatterGraphViewModel.PointsRadius, _scatterGraphViewModel.RenderQuality)
            {
                ScatterGraph = _scatterGraphViewModel
            };
            _scatterGraphViewModel.Samples.Add(sampleViewModel);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using ReScanVisualizer.Models;

namespace ReScanVisualizer.ViewModels.Parts
{
    public class PointPartViewModel : PartViewModelBase
    {
        public PointPartViewModel(Base3D origin, double scaleFactor = 1.0, RenderQuality renderQuality = RenderQuality.High) 
            : base(origin, scaleFactor, renderQuality)
        {
        }

        public override Base3D FindNeareatBase(Point3D point)
        {
            return OriginBase.Base3D;
        }
    }
}

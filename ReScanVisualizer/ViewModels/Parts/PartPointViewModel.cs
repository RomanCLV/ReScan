using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.ViewModels.Parts
{
    public class PartPointViewModel : PartViewModelBase
    {
        public PartPointViewModel(Base3D origin) : base(origin)
        {
        }

        public override Base3D FindNeareatBase(Point3D point)
        {
            return OriginBase.Base3D;
        }
    }
}

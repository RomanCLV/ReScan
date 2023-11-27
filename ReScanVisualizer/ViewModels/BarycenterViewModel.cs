using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels
{
    public class BarycenterViewModel : SampleViewModel
    {
        public BarycenterViewModel() : this(new Point3D())
        {
        }

        public BarycenterViewModel(double x, double y, double z) : this(new Point3D(x, y, z))
        {
        }

        public BarycenterViewModel(Point3D point3D) : this(point3D, Colors.White)
        {
        }

        public BarycenterViewModel(Color color) : this(new Point3D(), color)
        {
        }

        public BarycenterViewModel(Point3D point3D, Color color, double scaleFactor = 1.0, double radius = 0.5, RenderQuality renderQuality = RenderQuality.High)
            : base(point3D, color, scaleFactor, radius, renderQuality)
        {
            CanEdit = false; 
        }
    }
}

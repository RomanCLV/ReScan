using ReScanVisualizer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#nullable enable

namespace ReScanVisualizer.Models
{
    public interface I3DElement : IModelisable, IHideable, ISelectable
    {
        public double ScaleFactor { get; set; }

        public bool IsMouseOver { get; set; }
    }
}

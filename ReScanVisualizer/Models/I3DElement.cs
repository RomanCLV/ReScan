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
    public interface I3DElement
    {
        public double ScaleFactor { get; set; }

        public ColorViewModel Color { get; set; }

        public Model3D Model { get; }

        public event EventHandler<bool>? IsHidenChanged;

        public bool IsHiden { get; }

        public void Hide();

        public void Show();

        public void UpdateModelGeometry();

        public void UpdateModelMaterial();
    }
}

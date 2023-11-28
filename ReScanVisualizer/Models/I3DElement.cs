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
    public interface I3DElement : IModelisable
    {
        public double ScaleFactor { get; set; }

        public ColorViewModel Color { get; }

        public event EventHandler<bool>? IsHiddenChanged;

        public bool IsSelected { get; }

        public bool IsMouseOver { get; set; }

        public bool IsHidden { get; }

        public RenderQuality RenderQuality { get; set; }

        public void InverseIsHidden();

        public void Hide();

        public void Show();

        public void UpdateModelGeometry();

        public void UpdateModelMaterial();

        public void Select();

        public void Unselect();
    }
}

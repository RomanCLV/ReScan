using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.Models
{
    public interface I3DElement
    {
        public Color Color { get; set; }

        public Model3D Model { get; }

        public bool IsHiden { get; }

        public void Hide();

        public void Show();
    }
}

using ReScanVisualizer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ReScanVisualizer.Models
{
    public interface IModelisable
    {
        public ColorViewModel Color { get; }

        public Model3D Model { get; }

        public bool IsBelongingToModel(GeometryModel3D geometryModel3D);

        public RenderQuality RenderQuality { get; set; }

        public void UpdateModelGeometry();

        public void UpdateModelMaterial();
    }
}

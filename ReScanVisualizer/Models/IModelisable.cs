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
        public Model3D Model { get; }

        public bool IsBelongingToModel(GeometryModel3D geometryModel3D);
    }
}

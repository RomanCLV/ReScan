using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace ReScanVisualizer.Helper3D
{
    internal static class Helper3D
    {
        internal static GeometryModel3D BuildSphere(Point3D center, double radius, Brush color)
        {
            MeshBuilder builder = new MeshBuilder(false, false);
            builder.AddSphere(center, radius);
            return BuildMesh(builder, color);
        }

        internal static GeometryModel3D BuildArrow(Point3D point1, Point3D point2, double diameter, Brush color)
        {
            return BuildMesh(BuildArrowGeometry(point1, point2, diameter), color);
        }

        internal static Geometry3D BuildArrowGeometry(Point3D point1, Point3D point2, double diameter)
        {
            MeshBuilder builder = new MeshBuilder(false, false);
            builder.AddArrow(point1, point2, diameter);
            return builder.ToMesh(true);
        }

        private static GeometryModel3D BuildMesh(MeshBuilder builder, Brush color)
        {
            // Create a mesh from the builder (and freeze it)
            MeshGeometry3D mesh = builder.ToMesh(true);
            return BuildMesh(mesh, color);
        }

        private static GeometryModel3D BuildMesh(Geometry3D geometry, Brush color)
        {
            // Create some materials
            Material material = MaterialHelper.CreateMaterial(color);

            return new GeometryModel3D
            {
                Geometry = geometry,
                Material = material,
                BackMaterial = material,
                Transform = new TranslateTransform3D()
            };
        }
    }
}

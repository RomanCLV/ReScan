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
    public static class Helper3D
    {
        public static GeometryModel3D BuildSphere(Point3D center, double radius, Brush color)
        {
            return BuildSphere(center, radius, MaterialHelper.CreateMaterial(color));
        }

        public static GeometryModel3D BuildSphere(Point3D center, double radius, Material material)
        {
            MeshBuilder builder = new MeshBuilder(false, false);
            builder.AddSphere(center, radius);
            return BuildGeometry(builder, material);
        }

        public static GeometryModel3D BuildArrow(Point3D point1, Point3D point2, double diameter, Brush color)
        {
            return BuildArrow(point1, point2, diameter, MaterialHelper.CreateMaterial(color));
        }

        public static GeometryModel3D BuildArrow(Point3D point1, Point3D point2, double diameter, Material material)
        {
            return BuildGeometry(BuildArrowGeometry(point1, point2, diameter), material);
        }

        public static Geometry3D BuildArrowGeometry(Point3D point1, Point3D point2, double diameter)
        {
            MeshBuilder builder = new MeshBuilder(false, false);
            builder.AddArrow(point1, point2, diameter);
            return builder.ToMesh(true);
        }

        public static GeometryModel3D BuildGeometry(MeshBuilder builder, Brush color)
        {
            return BuildGeometry(builder, MaterialHelper.CreateMaterial(color));
        }

        public static GeometryModel3D BuildGeometry(MeshBuilder builder, Material material)
        {
            MeshGeometry3D mesh = builder.ToMesh(true);
            return BuildGeometry(mesh, material);
        }

        public static GeometryModel3D BuildGeometry(Geometry3D geometry, Brush color)
        {
            return BuildGeometry(geometry, MaterialHelper.CreateMaterial(color));
        }

        public static GeometryModel3D BuildGeometry(Geometry3D geometry, Material material)
        {
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

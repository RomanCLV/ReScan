using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Data;

namespace ReScanVisualizer.Helper3D
{
    public static class Helper3D
    {
        #region Build Model & Geometry

        public static GeometryModel3D BuildModel(MeshBuilder builder, Brush color)
        {
            return BuildModel(builder, MaterialHelper.CreateMaterial(color));
        }

        public static GeometryModel3D BuildModel(MeshBuilder builder, Material material)
        {
            return BuildModel(builder.ToMesh(true), material);
        }

        public static GeometryModel3D BuildModel(Geometry3D geometry, Brush color)
        {
            return BuildModel(geometry, MaterialHelper.CreateMaterial(color));
        }

        public static GeometryModel3D BuildModel(Geometry3D geometry, Material material)
        {
            return new GeometryModel3D
            {
                Geometry = geometry,
                Material = material,
                BackMaterial = material,
                Transform = new TranslateTransform3D()
            };
        }

        #endregion

        #region Sphere Model & Geometry

        public static MeshGeometry3D BuildSphereGeometry(Point3D center, double radius)
        {
            MeshBuilder builder = new MeshBuilder(false, false);
            builder.AddSphere(center, radius);
            return builder.ToMesh(true);
        }

        public static GeometryModel3D BuildSphereModel(Point3D center, double radius, Brush color)
        {
            return BuildSphereModel(center, radius, MaterialHelper.CreateMaterial(color));
        }

        public static GeometryModel3D BuildSphereModel(Point3D center, double radius, Material material)
        {
            return BuildModel(BuildSphereGeometry(center, radius), material);
        }

        #endregion

        #region Arrow Model & Geometry

        public static MeshGeometry3D BuildArrowGeometry(Point3D point1, Point3D point2, double diameter)
        {
            MeshBuilder builder = new MeshBuilder(false, false);
            builder.AddArrow(point1, point2, diameter);
            return builder.ToMesh(true);
        }

        public static GeometryModel3D BuildArrowModel(Point3D point1, Point3D point2, double diameter, Brush color)
        {
            return BuildArrowModel(point1, point2, diameter, MaterialHelper.CreateMaterial(color));
        }

        public static GeometryModel3D BuildArrowModel(Point3D point1, Point3D point2, double diameter, Material material)
        {
            return BuildModel(BuildArrowGeometry(point1, point2, diameter), material);
        }

        #endregion
    }
}

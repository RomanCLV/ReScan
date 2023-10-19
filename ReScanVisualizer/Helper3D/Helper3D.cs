using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Data;
using ReScanVisualizer.Models;

namespace ReScanVisualizer.Helper3D
{
    public static class Helper3D
    {
        #region Build Model & Geometry

        public static GeometryModel3D BuildModel(MeshBuilder builder, Color color)
        {
            return BuildModel(builder, new SolidColorBrush(color));
        }

        public static GeometryModel3D BuildModel(MeshBuilder builder, Brush brush)
        {
            return BuildModel(builder, MaterialHelper.CreateMaterial(brush));
        }

        public static GeometryModel3D BuildModel(MeshBuilder builder, Material material)
        {
            return BuildModel(builder.ToMesh(true), material);
        }

        public static GeometryModel3D BuildModel(Geometry3D geometry, Color color)
        {
            return BuildModel(geometry, new SolidColorBrush(color));
        }

        public static GeometryModel3D BuildModel(Geometry3D geometry, Brush brush)
        {
            return BuildModel(geometry, MaterialHelper.CreateMaterial(brush));
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

        public static GeometryModel3D BuildSphereModel(Point3D center, double radius, Color color)
        {
            return BuildSphereModel(center, radius, new SolidColorBrush(color));
        }

        public static GeometryModel3D BuildSphereModel(Point3D center, double radius, Brush brush)
        {
            return BuildSphereModel(center, radius, MaterialHelper.CreateMaterial(brush));
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

        public static GeometryModel3D BuildArrowModel(Point3D point1, Point3D point2, double diameter, Color color)
        {
            return BuildArrowModel(point1, point2, diameter, new SolidColorBrush(color));
        }

        public static GeometryModel3D BuildArrowModel(Point3D point1, Point3D point2, double diameter, Brush brush)
        {
            return BuildArrowModel(point1, point2, diameter, MaterialHelper.CreateMaterial(brush));
        }

        public static GeometryModel3D BuildArrowModel(Point3D point1, Point3D point2, double diameter, Material material)
        {
            return BuildModel(BuildArrowGeometry(point1, point2, diameter), material);
        }

        #endregion

        #region Plan Model & Geometry

        public static MeshGeometry3D BuildPlanGeometry(Point3D center, Vector3D normal, Vector3D up, double width, double height, double dist = 0.0)
        {
            MeshBuilder builder = new MeshBuilder(false, false);
            builder.AddCubeFace(center, normal, up, dist, width, height);
            return builder.ToMesh(true);
        }

        public static GeometryModel3D BuildPlanModel(Point3D center, Vector3D normal, Vector3D up, double width, double height, double dist, Color color)
        {
            return BuildPlanModel(center, normal, up, dist, width, height, new SolidColorBrush(color));
        }

        public static GeometryModel3D BuildPlanModel(Point3D center, Vector3D normal, Vector3D up, double width, double height, double dist, Brush brush)
        {
            return BuildPlanModel(center, normal, up, dist, width, height, MaterialHelper.CreateMaterial(brush));
        }

        public static GeometryModel3D BuildPlanModel(Point3D center, Vector3D normal, Vector3D up, double width, double height, double dist, Material material)
        {
            return BuildModel(BuildPlanGeometry(center, normal, up, dist, width, height), material);
        }

        #endregion

        public static Model3DGroup BuildBaseModel(Base3D repere, Brush cx, Brush cy, Brush cz, double diameter = 0.1)
        {
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(BuildArrowModel(repere.Origin, Point3D.Add(repere.Origin, repere.X), diameter, cx));
            group.Children.Add(BuildArrowModel(repere.Origin, Point3D.Add(repere.Origin, repere.Y), diameter, cy));
            group.Children.Add(BuildArrowModel(repere.Origin, Point3D.Add(repere.Origin, repere.Z), diameter, cz));
            return group;
        }

        public static Model3DGroup BuildBaseModel(Point3D origin, Vector3D x, Vector3D y, Vector3D z, Brush cx, Brush cy, Brush cz, double diameter = 0.1)
        {
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(BuildArrowModel(origin, Point3D.Add(origin, x), diameter, cx));
            group.Children.Add(BuildArrowModel(origin, Point3D.Add(origin, y), diameter, cy));
            group.Children.Add(BuildArrowModel(origin, Point3D.Add(origin, z), diameter, cz));
            return group;
        }
    }
}

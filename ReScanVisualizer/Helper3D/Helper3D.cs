using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Data;
using ReScanVisualizer.Models;
using HelixToolkit.Wpf;

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

        public static MeshGeometry3D BuildSphereGeometry(Point3D center, double radius, RenderQuality renderQuality = RenderQuality.High)
        {
            MeshBuilder builder = new MeshBuilder(false, false);
            int thetaDiv = renderQuality switch
            {
                RenderQuality.VeryLow => 3,
                RenderQuality.Low => 8,
                RenderQuality.Medium => 16,
                RenderQuality.High => 22,
                RenderQuality.VeryHigh => 36,
                _ => throw new NotImplementedException(),
            };
            builder.AddSphere(center, radius, thetaDiv, thetaDiv);
            return builder.ToMesh(true);
        }

        public static GeometryModel3D BuildSphereModel(Point3D center, double radius, Color color, RenderQuality renderQuality = RenderQuality.High)
        {
            return BuildSphereModel(center, radius, new SolidColorBrush(color), renderQuality);
        }

        public static GeometryModel3D BuildSphereModel(Point3D center, double radius, Brush brush, RenderQuality renderQuality = RenderQuality.High)
        {
            return BuildSphereModel(center, radius, MaterialHelper.CreateMaterial(brush), renderQuality);
        }

        public static GeometryModel3D BuildSphereModel(Point3D center, double radius, Material material, RenderQuality renderQuality = RenderQuality.High)
        {
            return BuildModel(BuildSphereGeometry(center, radius, renderQuality), material);
        }

        #endregion

        #region Arrow Model & Geometry

        public static MeshGeometry3D BuildArrowGeometry(Point3D point1, Point3D point2, double diameter, RenderQuality renderQuality=RenderQuality.High)
        {
            MeshBuilder builder = new MeshBuilder(false, false);
            int thetaDiv = renderQuality switch
            {
                RenderQuality.VeryLow => 5,
                RenderQuality.Low => 8,
                RenderQuality.Medium => 16,
                RenderQuality.High => 22,
                RenderQuality.VeryHigh => 36,
                _ => throw new NotImplementedException(),
            };
            builder.AddArrow(point1, point2, diameter, 3, thetaDiv);
            return builder.ToMesh(true);
        }

        public static GeometryModel3D BuildArrowModel(Point3D point1, Point3D point2, double diameter, Color color, RenderQuality renderQuality = RenderQuality.High)
        {
            return BuildArrowModel(point1, point2, diameter, new SolidColorBrush(color), renderQuality);
        }

        public static GeometryModel3D BuildArrowModel(Point3D point1, Point3D point2, double diameter, Brush brush, RenderQuality renderQuality = RenderQuality.High)
        {
            return BuildArrowModel(point1, point2, diameter, MaterialHelper.CreateMaterial(brush), renderQuality);
        }

        public static GeometryModel3D BuildArrowModel(Point3D point1, Point3D point2, double diameter, Material material, RenderQuality renderQuality = RenderQuality.High)
        {
            return BuildModel(BuildArrowGeometry(point1, point2, diameter, renderQuality), material);
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

        public static Model3DGroup BuildBaseModel(Base3D repere, Brush cx, Brush cy, Brush cz, double diameter = 0.1, RenderQuality renderQuality = RenderQuality.High)
        {
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(BuildArrowModel(repere.Origin, Point3D.Add(repere.Origin, repere.X), diameter, cx, renderQuality));
            group.Children.Add(BuildArrowModel(repere.Origin, Point3D.Add(repere.Origin, repere.Y), diameter, cy, renderQuality));
            group.Children.Add(BuildArrowModel(repere.Origin, Point3D.Add(repere.Origin, repere.Z), diameter, cz, renderQuality));
            return group;
        }

        public static Model3DGroup BuildBaseModel(Point3D origin, Vector3D x, Vector3D y, Vector3D z, Brush cx, Brush cy, Brush cz, double diameter = 0.1, RenderQuality renderQuality = RenderQuality.High)
        {
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(BuildArrowModel(origin, Point3D.Add(origin, x), diameter, cx, renderQuality));
            group.Children.Add(BuildArrowModel(origin, Point3D.Add(origin, y), diameter, cy, renderQuality));
            group.Children.Add(BuildArrowModel(origin, Point3D.Add(origin, z), diameter, cz, renderQuality));
            return group;
        }
    }
}

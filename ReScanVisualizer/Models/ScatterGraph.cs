using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace ReScanVisualizer.Models
{
    public class ScatterGraph
    {
        private readonly List<Point3D> _points;

        public int Count => _points.Count;

        public ScatterGraph()
        {
            _points = new List<Point3D>();
        }

        public ScatterGraph(ScatterGraph scatterGraph)
        {
            _points = new List<Point3D>(scatterGraph._points);
        }

        public ScatterGraph(int capacity)
        {
            _points = new List<Point3D>(capacity);
        }

        public ScatterGraph(List<Point3D> pointsList)
        {
            _points = new List<Point3D>(pointsList);
        }

        public ScatterGraph(Point3D[] pointsArray)
        {
            _points = new List<Point3D>(pointsArray);
        }

        public void AddPoint(Point3D point)
        {
            _points.Add(point);
        }

        public Point3D AddPoint(double x, double y, double z)
        {
            Point3D point = new Point3D(x, y, z);
            _points.Add(point);
            return point;
        }

        public Point3D this[int index]
        {
            get
            {
                if (index < 0 || index >= _points.Count)
                {
                    throw new IndexOutOfRangeException("Index is out of range.");
                }

                return _points[index];
            }
        }

        public Point3D At(int pos)
        {
            return _points[pos];
        }

        public void Clear()
        {
            _points.Clear();
        }

        /// <summary>
        /// Reduce the number of points by the given factor.<br />
	    /// If percent isn't valid, the process is canceled, return false, else return true<br />
        /// <br />
	    /// Examples:<br />
	    /// percent:  10 -> reduced by  10% - if you have 100 points, you will now have 90<br />
        /// percent:  80 -> reduced by  80% - if you have 100 points, you will now have 20<br />
        /// percent:   0 -> reduced by   0% - no changes<br />
        /// percent: 100 -> reduced by 100% - cleared<br />
        /// </summary>
        /// <param name="percent">Percentage of reduction (between 0.0 and 100.0).</param>
        public void ReducePercent(double percent)
        {
            if (percent > 0.0 && percent <= 100.0)
            {
                if (percent == 100.0)
                {
                    _points.Clear();
                }
                else
                {
                    int size = _points.Count;
                    int newSize = (int)(size * (1.0 - (percent / 100.0)));
                    double inc = (double)size / (double)newSize;
                    List<Point3D> taken = new List<Point3D>(newSize);
                    int i;
                    double j = 0.0;
                    for (i = 0; i < newSize; i++)
                    {
                        taken.Add(_points[(int)j]);
                        j += inc;
                    }
                    _points.Clear();
                    _points.AddRange(taken);
                }
            }
        }

        /// <summary>
        /// Reduce the number of points by skipping points.<br />
        /// <br />
        /// Examples:<br />
	    /// skipped:  3 -> reduce by  3 - if you have 100 points, you will now have 33 and the taken points are index 0,  3,  6, ..., 99<br />
	    /// skipped: 10 -> reduce by 10 - if you have 100 points, you will now have 10 and the taken points are index 0, 10, 20, ..., 90<br />
        /// </summary>
        /// <param name="skipped">between 2 and number of points</param>
        public void Reduce(int skipped)
        {
            int size = _points.Count;

            if (skipped >= 2 && skipped <= size)
            {
                int newSize = size / skipped;
                List<Point3D> taken = new List<Point3D>(newSize);
                int i;
                for (i = 0; i < size; i += skipped)
                {
                    taken.Add(_points[i]);
                }
                _points.Clear();
                _points.AddRange(taken);
            }
        }

        /// <summary>
        /// Get a new <see cref="ScatterGraph"/> to which the <see cref="ReducePercent(double)"/> method has been applied.
        /// </summary>
        /// <param name="percent">Percentage of reduction (between 0.0 and 100.0).</param>
        public ScatterGraph GetReducedPercent(double percent)
        {
            ScatterGraph graph = new ScatterGraph(this);
            graph.ReducePercent(percent);
            return graph;
        }

        /// <summary>
        /// Get a new <see cref="ScatterGraph"/> to which the <see cref="Reduce(int)"/> method has been applied.
        /// </summary>
        /// <param name="skipped">between 2 and number of points</param>
        public ScatterGraph GetReduced(int skipped)
        {
            ScatterGraph graph = new ScatterGraph(this);
            graph.Reduce(skipped);
            return graph;
        }

        public int FindMin(Func<Point3D, double> getter)
        {
            if (_points.Count == 0)
            {
                throw new InvalidOperationException("The list is empty.");
            }

            int minIndex = _points
                .Select((point, index) => new ItemIndexed<Point3D>(point, index))
                .Aggregate((min, next) => getter(next.Item) < getter(min.Item) ? next : min)
                .Index;

            return minIndex;
        }

        public int FindMax(Func<Point3D, double> getter)
        {
            if (_points.Count == 0)
            {
                throw new InvalidOperationException("The list is empty.");
            }

            int maxIndex = _points
                .Select((point, index) => new ItemIndexed<Point3D>(point, index))
                .Aggregate((max, next) => getter(next.Item) > getter(max.Item) ? next : max)
                .Index;

            return maxIndex;
        }

        public void FindExtrema(Plan2D plan, Func<Point3D, double>[] getters, out Point3D minPoint, out Point3D maxPoint)
        {
            if (_points.Count == 0)
            {
                throw new InvalidOperationException("The list is empty.");
            }

            if (getters.Length != 2)
            {
                throw new InvalidOperationException($"Excatly 2 getters must be given. Given: {getters.Length}");
            }

            int[] extremas = new int[4];

            extremas[0] = FindMin(getters[0]);
            extremas[1] = FindMin(getters[1]);
            extremas[2] = FindMax(getters[0]);
            extremas[3] = FindMax(getters[1]);

            minPoint = new Point3D();
            maxPoint = new Point3D();

            switch (plan)
            {
                case Plan2D.XY:
                    minPoint.X = _points[extremas[0]].X;
                    minPoint.Y = _points[extremas[1]].Y;
                    minPoint.Z = 0.0;
                    maxPoint.X = _points[extremas[2]].X;
                    maxPoint.Y = _points[extremas[3]].Y;
                    maxPoint.Z = 0.0;
                    break;

                case Plan2D.XZ:
                    minPoint.X = _points[extremas[0]].X;
                    minPoint.Y = 0.0;
                    minPoint.Z = _points[extremas[1]].Z;
                    maxPoint.X = _points[extremas[2]].X;
                    maxPoint.Y = 0.0;
                    maxPoint.Z = _points[extremas[3]].Z;
                    break;

                case Plan2D.YZ:
                    minPoint.X = 0.0;
                    minPoint.Y = _points[extremas[0]].Y;
                    minPoint.Z = _points[extremas[1]].Z;
                    maxPoint.X = 0.0;
                    maxPoint.Y = _points[extremas[2]].Y;
                    maxPoint.Z = _points[extremas[3]].Z;
                    break;

                default:
                    throw new InvalidOperationException("Unexpected plan.");
            }
        }

        /// <summary>
        /// Find the closest point from the origin.
        /// </summary>
        public static Point3D GetClosestPoint(ScatterGraph scatterGraph)
        {
            return GetClosestPoint(scatterGraph, new Point3D());
        }

        /// <summary>
        /// Find the closest point from another point.
        /// </summary>
        public static Point3D GetClosestPoint(ScatterGraph scatterGraph, Point3D point)
        {
            int size = scatterGraph.Count;
            Point3D closestPoint = point;
            Point3D currentPoint;
            double minDistance = double.MaxValue;
            double currentDistance;
            for (int i = 0; i < size; i++)
            {
                currentPoint = scatterGraph[i];
                currentDistance = (point - currentPoint).Length;
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    closestPoint = currentPoint;
                }
            }
            return closestPoint;
        }

        /// <summary>
        /// Find the closest point in a specified 3D base using the projection of each point over a plan of this base.
        /// </summary>
        public static Point3D GetClosestPoint(ScatterGraph scatterGraph, Base3D base3D, Plan2D plan2D)
        {
            int size = scatterGraph.Count;
            Point3D closestPoint = base3D.Origin;
            Point3D currentPoint;
            double minDistance = double.MaxValue;
            double currentDistance;
            Plan plan = base3D.GetPlan(plan2D);
            for (int i = 0; i < size; i++)
            {
                currentPoint = plan.GetOrthogonalProjection(scatterGraph[i]);
                currentDistance = (base3D.Origin - currentPoint).Length;
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    closestPoint = currentPoint;
                }
            }
            return closestPoint;
        }

        /// <summary>
        /// Find the farthest point from the origin.
        /// </summary>
        public static Point3D GetFarthestPoint(ScatterGraph scatterGraph)
        {
            return GetFarthestPoint(scatterGraph, new Point3D());
        }

        /// <summary>
        /// Find the farthest point from another point.
        /// </summary>
        public static Point3D GetFarthestPoint(ScatterGraph scatterGraph, Point3D point)
        {
            int size = scatterGraph.Count;
            Point3D farthestPoint = point;
            Point3D currentPoint;
            double maxDistance = double.MinValue;
            double currentDistance;
            for (int i = 0; i < size; i++)
            {
                currentPoint = scatterGraph[i];
                currentDistance = (point - currentPoint).Length;
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    farthestPoint = currentPoint;
                }
            }
            return farthestPoint;
        }

        public Point3D ComputeBarycenter()
        {
            if (_points.Count == 0)
            {
                return new Point3D(0.0, 0.0, 0.0);
            }

            double sumX = 0.0;
            double sumY = 0.0;
            double sumZ = 0.0;

            foreach (Point3D point in _points)
            {
                sumX += point.X;
                sumY += point.Y;
                sumZ += point.Z;
            }

            double centerX = sumX / _points.Count;
            double centerY = sumY / _points.Count;
            double centerZ = sumZ / _points.Count;

            return new Point3D(centerX, centerY, centerZ);
        }

        public bool ArePointsColinear()
        {
            if (_points.Count < 2)
            {
                return true;
                //throw new InvalidOperationException("Need at least 2 points to compute the linearity of a graph.");
            }

            Point3D p0 = _points[0];
            Vector3D vector1 = _points[1] - p0;
            for (int i = 2; i < _points.Count; i++)
            {
                if (!Tools.AreVectorsColinear(vector1, _points[i] - p0))
                {
                    return false;
                }
            }
            return true;
        }

        public bool ArePointsCoplanar()
        {
            if (_points.Count <= 3)
            {
                return true;
            }

            Point3D p0 = _points[0];
            Vector3D vector1 = _points[1] - p0;
            Vector3D vector2 = new Vector3D();
            Vector3D vector3;
            bool hasFoundVector2 = false;
            bool isCoplanar = true;
            for (int i = 2; i < _points.Count; i++)
            {
                if (!hasFoundVector2)
                {
                    vector2 = _points[i] - p0;
                    hasFoundVector2 = !Tools.AreVectorsColinear(vector1, vector2);
                }
                else
                {
                    vector3 = _points[i] - p0;
                    isCoplanar = Math.Abs(Tools.MixteProduct(vector1, vector2, vector3)) < Const.ZERO_CLAMP;
                    if (!isCoplanar)
                    {
                        break;
                    }
                }
            }
            return isCoplanar;
        }

        public Plan ComputeAveragePlan()
        {
            // based on: https://www.claudeparisel.com/monwiki/data/Karnak/K2/PLAN%20MOYEN.pdf

            int size = _points.Count;

            Point3D barycenter = ComputeBarycenter();
            Vector3D z;
            if (size < 2)
            {
                z = new Vector3D(0, 0, 1);
                return size == 1 ? new Plan(z, -z.Z * barycenter.Z) : new Plan(z);
            }
            else if (ArePointsCoplanar())
            {
                Vector3D x = _points[1] - _points[0];
                if (ArePointsColinear())
                {
                    Base3D repere = Tools.ComputeOrientedBase(x, Axis.X);
                    z = repere.Z;
                }
                else
                {
                    Vector3D y = new Vector3D(0, 1, 0);
                    for (int i = 2; i < size; i++)
                    {
                        y = _points[i] - _points[0];
                        if (!Tools.AreVectorsColinear(x, y))
                        {
                            break;
                        }
                    }
                    z = Vector3D.CrossProduct(x, y);
                }
                if (z.Z < 0)
                {
                    z *= -1;
                }
                z.Normalize();
                return new Plan(z, -(z.X * barycenter.X + z.Y * barycenter.Y + z.Z * barycenter.Z));
            }

            double sX = 0.0;
            double sXX = 0.0;
            double sXY = 0.0;
            double sXZ = 0.0;

            double sY = 0.0;
            //double sYX = 0.0; // meme chose que sXY
            double sYY = 0.0;
            double sYZ = 0.0;

            double sZ = 0.0;
            //double sZX = 0.0; // meme chose que sXZ
            //double sZY = 0.0;   // meme chose que sYZ
            double sZZ = 0.0;

            double D;
            double k = 1.0;

            Point3D point;
            double pX;
            double pY;
            double pZ;

            double a;
            double b;
            double c;

            // calcul des sommmes
            for (int i = 0; i < size; i++)
            {
                point = _points[i];
                pX = point.X;
                pY = point.Y;
                pZ = point.Z;

                sX += pX;
                sXX += pX * pX;
                sXY += pX * pY;
                sXZ += pX * pZ;

                sY += pY;
                sYY += pY * pY;
                sYZ += pY * pZ;

                sZ += pZ;
                sZZ += pZ * pZ;
            }

            D = (sXX * sYY * sZZ) - (sXX * sYZ * sYZ) - (sYY * sXZ * sXZ) + (2.0 * sXY * sXZ * sYZ);

            a = (sX * (sYY * sZZ - sYZ * sYZ)) - (sY * (sXY * sZZ - sXZ * sYZ)) + (sZ * (sXY * sYZ - sYY * sXZ));
            b = (sX * (sXY * sZZ - sXZ * sYZ)) - (sY * (sXX * sZZ - sXZ * sXZ)) + (sZ * (sYZ * sXX - sXY * sXZ));
            c = (sX * (sXY * sYZ - sYY * sXZ)) - (sY * (sXX * sYZ - sXY * sXZ)) + (sZ * (sXX * sYY - sXY * sXY));

            a *= (-k / D);
            b *= (k / D);
            c *= (-k / D);

            if (c < 0)
            {
                a *= -1;
                b *= -1;
                c *= -1;
            }

            z = new Vector3D(a, b, c);

            if (z.Length == 0)
            {
                z.Z = 1.0;
            }
            else if (z.Length != 1.0)
            {
                z.Normalize();
            }

            return new Plan(z.X, z.Y, z.Z, -(z.X * barycenter.X + z.Y * barycenter.Y + z.Z * barycenter.Z));
        }

        //public static Base3D ComputeRepere3D(ScatterGraph scatterGraph)
        //{
        //    Point3D barycenter = scatterGraph.ComputeBarycenter();
        //    Plan averagePlan = scatterGraph.ComputeAveragePlan();
        //    return ComputeRepere3D(barycenter, averagePlan, true);
        //}

        public static Base3D ComputeRepere3D(Point3D origin, Plan averagePlan, bool putXOnXY)
        {
            Base3D repere = Tools.ComputeOrientedBase(averagePlan.GetNormal(), Axis.Z);
            repere.Origin = origin;

            if (putXOnXY)
            {
                Vector3D xProjected;
                double angle;
                double angleAfter;
                do
                {
                    xProjected = repere.X;
                    xProjected.Z = 0;
                    angle = Vector3D.AngleBetween(xProjected, repere.X);
                    repere.Rotate(repere.Z, angle);
                    angleAfter = Vector3D.AngleBetween(xProjected, repere.X);
                } while (angleAfter > 0.1);
            }

            return repere;
        }

        #region static functions

        #region Populate

        public static void PopulateRandom(ScatterGraph scatterGraph, uint count, double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
        {
            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                double randomX = minX + (maxX - minX) * random.NextDouble();
                double randomY = minY + (maxY - minY) * random.NextDouble();
                double randomZ = minZ + (maxZ - minZ) * random.NextDouble();

                scatterGraph.AddPoint(new Point3D(randomX, randomY, randomZ));
            }
        }

        public static void PopulateRectangle2D(ScatterGraph scatterGraph, Point3D Center, Plan2D plan, double width, double height, uint numPointsWidth, uint numPointsHeight)
        {
            if (numPointsWidth < 2)
            {
                throw new ArgumentException("2 points minimum are required.", nameof(numPointsWidth));
            }
            if (numPointsHeight < 2)
            {
                throw new ArgumentException("2 points minimum are required.", nameof(numPointsHeight));
            }

            double stepWidth = width / (numPointsWidth - 1);
            double stepHeight = height / (numPointsHeight - 1);

            double halfWidth = width / 2.0;
            double halfHeight = height / 2.0;

            for (int i = 0; i < numPointsWidth; i++)
            {
                for (int j = 0; j < numPointsHeight; j++)
                {
                    Point3D p = new Point3D();

                    switch (plan)
                    {
                        case Plan2D.XY:
                            p.X = Center.X - halfWidth + i * stepWidth;
                            p.Y = Center.Y - halfHeight + j * stepHeight;
                            p.Z = Center.Z;
                            break;

                        case Plan2D.XZ:
                            p.X = Center.X - halfWidth + i * stepWidth;
                            p.Y = Center.Y;
                            p.Z = Center.Z - halfHeight + j * stepHeight;
                            break;

                        case Plan2D.YZ:
                            p.X = Center.X;
                            p.Y = Center.Y - halfWidth + i * stepWidth;
                            p.Z = Center.Z - halfHeight + j * stepHeight;
                            break;
                    }

                    scatterGraph.AddPoint(p);
                }
            }
        }

        public static void PopulateLine(ScatterGraph scatterGraph, Point3D start, Point3D end, uint numPoints)
        {
            if (numPoints < 2)
            {
                throw new ArgumentException("At least 2 points are required.", nameof(numPoints));
            }

            if (start == end)
            {
                throw new ArgumentException("Start and end points must be different.", nameof(numPoints));
            }

            double step = 1.0 / (numPoints - 1);

            for (int i = 0; i < numPoints; i++)
            {
                double t = i * step;
                Point3D p = new Point3D
                {
                    X = start.X + (end.X - start.X) * t,
                    Y = start.Y + (end.Y - start.Y) * t,
                    Z = start.Z + (end.Z - start.Z) * t
                };

                scatterGraph.AddPoint(p);
            }
        }

        #endregion

        #region Read & Save

        /// <summary>
        /// Save the current graph. Columns are X, Y and Z.
        /// </summary>
        /// <param name="filename">The path</param>
        /// <param name="scatterGraph">THe graph to save</param>
        /// <param name="replaceIfFileExists">Rewrite the existing file</param>
        /// <param name="writeHeaders">If the file contains headers</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void SaveCSV(string filename, ScatterGraph scatterGraph, bool replaceIfFileExists, bool writeHeaders)
        {
            if (scatterGraph == null)
            {
                throw new ArgumentNullException(nameof(scatterGraph));
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("Invalid filename.", nameof(filename));
            }

            // Vérifier si l'extension du fichier est bien ".csv"
            if (!filename.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The file must have the '.csv' extension.");
            }

            // Vérifier si le fichier existe déjà et si on doit le remplacer
            if (File.Exists(filename))
            {
                if (!replaceIfFileExists)
                {
                    throw new InvalidOperationException($"The file {filename} already exists.");
                }
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    if (writeHeaders)
                    {
                        // Écrire les en-têtes CSV
                        writer.WriteLine("X;Y;Z");
                    }

                    foreach (Point3D point in scatterGraph._points)
                    {
                        // Écrire chaque point CSV
                        writer.WriteLine($"{point.X};{point.Y};{point.Z}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Read the given file to create a <see cref="ScatterGraph"/>. Columns must be x, y and z.
        /// </summary>
        /// <param name="filename">The path</param>
        /// <param name="containsHeader">If the file contains headers</param>
        /// <returns>A new scatter graph</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static ScatterGraph ReadCSV(string filename, bool containsHeader)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("Invalid filename.", nameof(filename));
            }

            // Vérifier si l'extension du fichier est bien ".csv"
            if (!filename.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The file must have the '.csv' extension.");
            }

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"The file {filename} does not exist.");
            }

            List<Point3D> points = new List<Point3D>();

            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string line;
                    if (containsHeader)
                    {
                        // Lire la première ligne pour ignorer les en-têtes
                        reader.ReadLine();
                    }

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = (line.Replace('.', ',')).Split(';');
                        if (values.Length >= 3 &&
                            double.TryParse(values[0], out double x) &&
                            double.TryParse(values[1], out double y) &&
                            double.TryParse(values[2], out double z))
                        {
                            Point3D point = new Point3D(x, y, z);
                            points.Add(point);
                        }
                        else
                        {
                            throw new FormatException("Invalid CSV format.");
                        }
                    }
                }

                return new ScatterGraph(points);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion
    }
}

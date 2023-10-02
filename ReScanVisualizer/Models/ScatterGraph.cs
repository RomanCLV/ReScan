using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

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

        public ScatterGraph(int size)
        {
            _points = new List<Point3D>(size);
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

        public ScatterGraph GetReducedPercent(double percent)
        {
            ScatterGraph graph = new ScatterGraph(this);
            graph.ReducePercent(percent);
            return graph;
        }

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

        static Point3D GetClosestPoint(ScatterGraph scatterGraph, Point3D point)
        {
            int size = scatterGraph.Count;
            Point3D closestPoint = new Point3D();
            Point3D currentPoint;
            double minDistance = double.MaxValue;
            double currentDistance;
            for (int i = 0; i < size; i++)
            {
                currentPoint = scatterGraph[i];
                
                currentDistance = (point - currentPoint).Length;
                if (currentDistance < minDistance)
                {
                    closestPoint = currentPoint;
                }
            }
            return closestPoint;
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

        public Plan ComputeAveragePlan()
        {
            // based on: https://www.claudeparisel.com/monwiki/data/Karnak/K2/PLAN%20MOYEN.pdf

            int size = _points.Count;

            if (size < 3)
            {
                throw new InvalidOperationException("Need at least 3 points to compute the average plan.");
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

            Point3D barycenter;
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

            barycenter = ComputeBarycenter();

            return new Plan(a, b, c, -(a * barycenter.X + b * barycenter.Y + c * barycenter.Z));
        }

        public static Repere3D ComputeRepere3D(ScatterGraph scatterGraph)
        {
            Point3D barycenter = scatterGraph.ComputeBarycenter();
            Plan averagePlan = scatterGraph.ComputeAveragePlan();
            return ComputeRepere3D(scatterGraph, barycenter, averagePlan);
        }

        public static Repere3D ComputeRepere3D(ScatterGraph scatterGraph, Point3D origin, Plan averagePlan)
        {
            Repere3D repere = new Repere3D(origin, new Vector3D(), new Vector3D(), averagePlan.GetNormal());
            repere.Z.Normalize();

            // on trouve le point le plus proche de l'origine du repere, on en fait son projet� orthogonal
            Point3D closestPointFromOrigin = GetClosestPoint(scatterGraph, origin);
            Point3D projetedPoint = averagePlan.GetOrthogonalProjection(closestPointFromOrigin);

            // On trouve le vecteur entre l'origne et le projet� et on le normalise - On a X
            repere.X = projetedPoint - repere.Origin;
            repere.X.Normalize();

            // On fait le produit vectoriel Z*X pour avoir Y, et on normalise
            repere.Y = Vector3D.CrossProduct(repere.Z, repere.X);
            repere.Y.Normalize();

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

        #endregion

        #region Read & Save

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
                        writer.WriteLine("X,Y,Z");
                    }

                    foreach (Point3D point in scatterGraph._points)
                    {
                        // Écrire chaque point CSV
                        writer.WriteLine($"{point.X},{point.Y},{point.Z}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Read the given file to create a <see cref="ScatterGraph"/>.
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
                        string[] values = line.Split(',');
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

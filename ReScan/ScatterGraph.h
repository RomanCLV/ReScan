#ifndef RESCAN_SCATTERGRAPH_H
#define RESCAN_SCATTERGRAPH_H

#include "Point3D.h"
#include "Plan.h"
#include "Plan2D.h"
#include "Base3D.h"
#include "macros.h"

#include <vector>
#include <string>

namespace ReScan
{
	class ScatterGraph
	{
	private:
		std::vector<const Point3D*> m_points;

	public:
		// constructor / destructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		ScatterGraph();

		/// <summary>
		/// Create a new ScatterGraph by copying another scatterGraph. 
		/// The points are new: modifying a point in the original graph will not affect the new graph.
		/// Think to clear a graph at the end of the usage. Destructor will not call clear method automatically.
		/// </summary>
		/// <param name="scatterGraph">The graph to copy</param>
		ScatterGraph(const ScatterGraph& scatterGraph);

		/// <summary>
		/// Create a graph with preallocate size.
		/// </summary>
		/// <param name="size">size to allocate.</param>
		ScatterGraph(const size_t size);

		/// <summary>
		/// Create a new ScatterGraph by copying points. 
		/// The points are new: modifying a point in the original list will not affect the new graph.
		/// Think to clear a graph at the end of the usage. Destructor will not call clear method automatically.
		/// </summary>
		/// <param name="scatterGraph">The points to copy</param>
		ScatterGraph(const std::vector<Point3D>* points);

		/// <summary>
		/// Create a new ScatterGraph by copying points. 
		/// The points are new: modifying a point in the original list will not affect the new graph.
		/// Think to clear a graph at the end of the usage. Destructor will not call clear method automatically.
		/// </summary>
		/// <param name="scatterGraph">The points to copy</param>
		ScatterGraph(const std::vector<const Point3D*>* pointsList);

		/// <summary>
		/// Create a new ScatterGraph by copying points. 
		/// The points are new: modifying a point in the original array will not affect the new graph.
		/// Think to clear a graph at the end of the usage. Destructor will not call clear method automatically.
		/// </summary>
		/// <param name="scatterGraph">The points to copy</param>
		/// <param name="size">size of the array.</param>
		ScatterGraph(const Point3D pointsArray[], const size_t size);

		/// <summary>
		/// Create a new ScatterGraph by copying points. 
		/// The points are new: modifying a point in the original array will not affect the new graph.
		/// Think to clear a graph at the end of the usage. Destructor will not call clear method automatically.
		/// </summary>
		/// <param name="scatterGraph">The points to copy</param>
		/// <param name="size">size of the array.</param>
		ScatterGraph(const Point3D* pointsArray[], const size_t size);

		/// <summary>
		/// Destructor of a ScatterGraph. Think to call clear method before.
		/// </summary>
		~ScatterGraph();

		void addPoint(const Point3D* point);
		Point3D* addPoint(const double x, const double y, const double z);
		size_t size() const;
		const Point3D* at(const size_t) const;
		void clear(const bool freeMemory = true);

		/*
		Reduce the number of points by the given factor.

		Params:
		percent: between 0.0 and 100.0
		If percent isn't valid, the process is canceled.

		Examples:
		percent:  10 -> reduced by  10% - if you have 100 points, you will now have 90
		percent:  80 -> reduced by  80% - if you have 100 points, you will now have 20
		percent:   0 -> reduced by   0% - no changes
		percent: 100 -> reduced by 100% - cleared
		*/
		void reducePercent(const double percent);

		/*
		Reduce the number of points by skipping points.

		Params:
		int skipped: between 2 and number of points (if not, the process is canceled)

		Examples:
		skipped:  3 -> reduce by  3 - if you have 100 points, you will now have 33 and the taken points are index 0,  3,  6, ..., 99<br />
		skipped: 10 -> reduce by 10 - if you have 100 points, you will now have 10 and the taken points are index 0, 10, 20, ..., 90<br />
		*/
		void reduce(const size_t skipped);

		/*
		Create a new graph based on the current one, and reduce it using reducePercent methode
		*/
		ScatterGraph* getReducedPercent(const double percent);

		/*
		Create a new graph based on the current one, and reduce it using reduce methode
		*/
		ScatterGraph* getReduced(const unsigned int skipped);

		// static functions

		/// <summary>
		/// Copy the source graph into the destination graph. The copied point are new ones.<br />
		/// WARNING: think to clear the destination graph before to call this method.
		/// </summary>
		/// <param name="source">Source graph</param>
		/// <param name="dest">Destination graph</param>
		static void copy(const ScatterGraph& source, ScatterGraph& dest);

		// static functions to populate

		static void populateFromVectorXYZ(const std::vector<float>* vect, ScatterGraph& scatterGraph);

		static void populateWithRandomPoints(ScatterGraph& scatterGraph, int count, double minX, double maxX, double minY, double maxY, double minZ, double maxZ);
		static void populateRectangle2D(ScatterGraph& scatterGraph, Plan2D plane, const Point3D& center, double width, double height, int numPoints);
		static void populateRectangle3D(ScatterGraph& scatterGraph, Plan2D plane, const Point3D& center, double width, double height, double depth, int numPoints);
		static void populateDisk(ScatterGraph& scatterGraph, Plan2D plane, const Point3D& center, double radius, int numPoints);

		// static functions to save / read

		static bool saveCSV(const std::string& filename, const ScatterGraph& scatterGraph, const bool replaceIfFileExists = false, const bool writeHeaders = true, const bool decimalCharIsDot = true);
		static bool readCSV(const std::string& filename, ScatterGraph* scatterGraph, bool containsHeader = true);

		// static functions to compute things

		static int findMin(const ScatterGraph& scatterGraph, double (Point3D::* getter)() const);
		static int findMax(const ScatterGraph& scatterGraph, double (Point3D::* getter)() const);
		static void findExtrema(const ScatterGraph& scatterGraph, const Plan2D& plan, double (Point3D::* getters[2])() const, Point3D* minPoint, Point3D* maxPoint);

		static const Point3D* getClosestPoint(const ScatterGraph& scatterGraph, const Point3D& point);
		static const Point3D* getFarthestPoint(const ScatterGraph& scatterGraph, const Point3D& point);

		static void computeBarycenter(const ScatterGraph& scatterGraph, Point3D* barycenter);
		static void computeAveragePlan(const ScatterGraph& scatterGraph, Plan* averagePlan);

		static bool arePointsCoplanar(const ScatterGraph& scatterGraph);
		static bool arePointsColinear(const ScatterGraph& scatterGraph);

		static void computeBase3D(const ScatterGraph& scatterGraph, Base3D* repere);
		static void computeBase3D(const ScatterGraph& scatterGraph, const Point3D& origin, const Plan& averagePlan, Base3D* repere);
	};
}

#endif // RESCAN_SCATTERGRAPH_H
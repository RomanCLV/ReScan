#ifndef SCATTERGRAPH_H
#define SCATTERGRAPH_H

#include "Point3D.h"
#include "Plan.h"
#include "Plan2D.h"
#include <vector>
#include <string>

class ScatterGraph
{
public:
	std::vector<const Point3D*> points;

public:
	// constructor / destructor

	ScatterGraph();
	ScatterGraph(const ScatterGraph& scatterGraph);
	ScatterGraph(const size_t size);
	ScatterGraph(const std::vector<Point3D>* points);
	ScatterGraph(const std::vector<const Point3D*>* pointsList);
	ScatterGraph(const Point3D pointsArray[], const size_t size);
	ScatterGraph(const Point3D* pointsArray[], const size_t size);
	~ScatterGraph();

	void addPoint(const Point3D* point);
	Point3D* addPoint(const double x, const double y, const double z);
	size_t size() const;
	const Point3D* at(const size_t) const;
	void clear();

	/*
	Reduce the number of points by the given factor.
	percent: between 0.0 and 100.0 
	If percent isn't valid, the process is canceled, return false, else return true
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
	percent: 10 -> reduce by 10 - if you have 100 points, you will now have 10
	*/
	void reduce(const unsigned int skipped);

	/*
	Create a new graph based on the current one, and reduce it using reducePercent methode
	*/
	ScatterGraph* getReducedPercent(const double percent);

	/*
	Create a new graph based on the current one, and reduce it using reduce methode
	*/
	ScatterGraph* getReduced(const unsigned int skipped);

	// static functions to populate

	static void populateFromVectorXYZ(const std::vector<float>* vect, ScatterGraph& scatterGraph);

	static void populateWithRandomPoints(ScatterGraph& scatterGraph, int count, double minX, double maxX, double minY, double maxY, double minZ, double maxZ);
	static void populateRectangle2D(ScatterGraph& scatterGraph, Plan2D plane, const Point3D& center, double width, double height, int numPoints);
	static void populateRectangle3D(ScatterGraph& scatterGraph, Plan2D plane, const Point3D& center, double width, double height, double depth, int numPoints);
	static void populateDisk(ScatterGraph& scatterGraph, Plan2D plane, const Point3D& center, double radius, int numPoints);

	// static functions to save / read

	static bool saveCSV(const std::string& filename, const ScatterGraph& scatterGraph, bool replaceIfFileExists = false, bool writeHeaders = true);
	static bool readCSV(const std::string& filename, ScatterGraph* scatterGraph, bool containsHeader = true);
	
	// static functions to compute things

	static int findMin(const ScatterGraph& scatterGraph, double (Point3D::* getter)() const);
	static int findMax(const ScatterGraph& scatterGraph, double (Point3D::* getter)() const);
	static void findExtrema(const ScatterGraph& scatterGraph, const Plan2D& plan, double (Point3D::* getters[2])() const, Point3D* minPoint, Point3D* maxPoint);

	static void computeBarycenter(const ScatterGraph& scatterGraph, Point3D& barycenter);
	static void computeAveragePlan(const ScatterGraph& scatterGraph, Plan& averagePlan);
};

#endif // SCATTERGRAPH_H
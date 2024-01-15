#ifndef RESCAN_RESCAN_H
#define RESCAN_RESCAN_H

#include "ObjFileIO.h"
#include "ScatterGraph.h"
#include "Plan2D.h"
#include "Point3D.h"
#include "macros.h"

#include <string>
#include <Eigen/Dense>

namespace ReScan
{
	class ReScan
	{

	private:
		std::string m_fileName;

		Plan2D* m_plan2D;
		unsigned int* m_stepAxis1;
		unsigned int* m_stepAxis2;

	public:
		ReScan(const std::string& fileName);
		ReScan(const ReScan& reScan);
		~ReScan();

		int process(const bool exportSubDivisions = false);
		int process(const Plan2D plan2D, const unsigned int stepAxis1, const unsigned int stepAxis2, const bool exportSubDivisions = false);

	private:
		bool isFileValid() const;
		Plan2D selectPlan2D() const;
		double getDistance1D(const Point3D& point1, const Point3D& point2, double (Point3D::* getter)() const) const;
		void getDistances(Point3D const& minPoint, Point3D const& maxPoint, double (Point3D::* getters[2])() const, double& distance1, double& distance2) const;
		unsigned int selectStep(char axisName, unsigned int min, unsigned int max) const;
		unsigned int getSubDivision(double distance, int step) const;
		void fillSubDivisions(const Point3D& minPoint, const ScatterGraph& graph, std::vector<ScatterGraph>* subGraph,
			double (Point3D::* getters[2])() const,
			const unsigned int step1, const unsigned int step2, const unsigned int subDivision1, const unsigned int subDivision2, const bool deleteIfEmpty = true) const;

		int internalProcess(const bool exportSubDivisions);
		bool exportBasesToCSV(const std::string& basePath, const std::vector<Base3D>& bases, const bool writeHeaders = true, const bool decimalCharIsDot = true) const;
		void exportSubDivisionsToCSV(const std::string& basePath, const std::vector<ScatterGraph>& subDivisions) const;
	};
}

std::string getDate();
std::string removeFileExtension(const std::string& fileName);

#endif // RESCAN_RESCAN_H
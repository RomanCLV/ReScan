#ifndef RESCAN_RESCAN_H
#define RESCAN_RESCAN_H

#include "ObjFileIO.h"
#include "ReScanProcessData.h"
#include "ReScanConfig.h"
#include "ScatterGraph.h"
#include "Plan2D.h"
#include "Point3D.h"
#include "macros.h"

#include <Eigen/Dense>
#include <string>

namespace ReScan
{
	class ReScan
	{

	private:
		std::string m_configFile;
		ReScanProcessData m_processData;

	public:
		ReScan(const std::string& m_configFileName);
		ReScan(const ReScan& reScan);
		~ReScan();

		int process();

		int process(
			const bool exportSubDivisions = false,
			const bool exportBasesCartesian = true, 
			const bool exportBasesEulerAngles = true, 
			const bool exportDetailsFile = true,
			const bool writeHeaders = true,
			const bool decimalCharIsDot = true);

		int process(
			const Plan2D plan2D, 
			const unsigned int stepAxis1, 
			const unsigned int stepAxis2, 
			const bool exportSubDivisions = false, 
			const bool exportBasesCartesian = true, 
			const bool exportBasesEulerAngles = true, 
			const bool exportDetailsFile = true, 
			const bool writeHeaders = true,
			const bool decimalCharIsDot = true);

	private:
		void resetProcessData();
		bool isFileValid() const;
		Plan2D selectPlan2D() const;
		double getDistance1D(const Point3D& point1, const Point3D& point2, double (Point3D::* getter)() const) const;
		void getDistances(Point3D const& minPoint, Point3D const& maxPoint, double (Point3D::* getters[2])() const, double& distance1, double& distance2) const;
		unsigned int selectStep(char axisName, unsigned int min, unsigned int max) const;
		unsigned int getSubDivision(double distance, int step) const;
		void fillSubDivisions(const Point3D& minPoint, const ScatterGraph& graph, std::vector<ScatterGraph>* subGraph,
			double (Point3D::* getters[2])() const,
			const unsigned int step1, const unsigned int step2, const unsigned int subDivision1, const unsigned int subDivision2, const bool deleteIfEmpty = true) const;

		int internalProcess();

		void exportSubDivisionsToCSV(const std::string& basePath, const std::vector<ScatterGraph>& subDivisions) const;
		bool exportBasesCartesianToCSV(const std::string& basePath, const std::vector<Base3D*>& bases, const std::string& nullText = "", const bool writeHeaders = true, const bool decimalCharIsDot = true) const;
		bool exportBasesEulerAnglesToCSV(const std::string& basePath, const std::vector<Base3D*>& bases, const std::string& nullText = "", const bool writeHeaders = true, const bool decimalCharIsDot = true) const;
		bool exportTrajectoryDetailsFile(const std::string& filename,
			const double distance1,
			const double distance2,
			const unsigned int stepAxis1,
			const unsigned int stepAxis2,
			const unsigned int subDivision1,
			const unsigned int subDivision2,
			const bool decimalCharIsDot) const;
	};
}

std::string getDate();
std::string removeFileExtension(const std::string& fileName);

#endif // RESCAN_RESCAN_H
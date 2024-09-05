#ifndef RESCAN_RESCAN_H
#define RESCAN_RESCAN_H

#include "MultiOStream.h"
#include "ReScanProcessData.h"
#include "ReScanConfig.h"
#include "ObjFileIO.h"
#include "ScatterGraph.h"
#include "Plan2D.h"
#include "Point3D.h"
#include "macros.h"
#include "ReScanFileType.h"

#include <Eigen/Dense>
#include <string>
#include <vector>

namespace ReScan
{
	class ReScan
	{
	private:
		ReScanProcessData m_processData;
		std::vector<std::function<void(const FileType, const std::string&)>> m_subscribers;

	public:
		using EventCallback = std::function<void(const FileType, const std::string&)>;

		ReScan();
		ReScan(const ReScan& reScan);
		~ReScan();

		void subscribe(EventCallback callback);
		void unsubscribe(EventCallback callback);

		int process(std::string& configFile);
		int process(const ReScanConfig& config);

		int process(
			const std::string& objFile,
			const bool exportSubDivisions,
			const bool exportBasesCartesian = true,
			const bool exportBasesEulerAngles = true,
			const bool exportDetailsFile = true,
			const bool writeHeaders = true,
			const bool decimalCharIsDot = true);

		int process(
			const std::string& objFile,
			const Plan2D plan2D, 
			const unsigned int stepAxis1, 
			const unsigned int stepAxis2, 
			const bool exportSubDivisions, 
			const bool exportBasesCartesian = true, 
			const bool exportBasesEulerAngles = true, 
			const bool exportDetailsFile = true, 
			const bool writeHeaders = true,
			const bool decimalCharIsDot = true);

		static bool isValidNameFile(const std::string& filename, const std::string& extention);

	private:
		void notifyObservers(const FileType fileType, const std::string& path) const;

		void resetProcessData();
		bool fileExists(const std::string& filename) const;

		int selectPlan2D(Plan2D* plan2D) const;
		double getDistance1D(const Point3D& point1, const Point3D& point2, double (Point3D::* getter)() const) const;
		void getDistances(Point3D const& minPoint, Point3D const& maxPoint, double (Point3D::* getters[2])() const);
		unsigned int selectStep(char axisName, unsigned int min, unsigned int max) const;
		unsigned int getSubDivision(double distance, int step) const;
		void fillSubDivisions(const Point3D& minPoint, const ScatterGraph& graph, std::vector<ScatterGraph>* subGraph,
			double (Point3D::* getters[2])() const, const bool deleteIfEmpty = true) const;

		int internalProcess();

		void exportSubDivisionsToCSV(const std::string& basePath, const std::vector<ScatterGraph>& subDivisions) const;
		bool exportBasesCartesianToCSV(const std::string& basePath, const std::vector<Base3D*>& bases, const std::string& nullText = "") const;
		bool exportBasesEulerAnglesToCSV(const std::string& basePath, const std::vector<Base3D*>& bases, const std::string& nullText = "") const;
		bool exportTrajectoryDetailsFile(const std::string& filename) const;
	};
}

std::string getDate();
std::string removeFileExtension(const std::string& fileName);

#endif // RESCAN_RESCAN_H
#ifndef RESCAN_PRESCAN_PRESCAN_H
#define RESCAN_PRESCAN_PRESCAN_H

#include "MultiOStream.h"
#include "ReScan.h"
#include "PreScanProcessData.h"
#include "PreScanConfig.h"
#include "PreScanMode.h"
#include "ScatterGraph.h"
#include "Point3D.h"
#include "macros.h"
#include "ReScanFileType.h"
#include "PreScanResultDetails.h"

#include <Eigen/Dense>
#include <string>
#include <vector>

namespace ReScan::PreScan
{
	class PreScan
	{
	private:
		PreScanProcessData m_processData;
		std::vector<std::function<void(const FileType, const std::string&)>> m_subscribers;
		std::vector<Base3D*>* m_bases;
		PreScanResultDetails* m_details;

	public:
		PreScan();
		PreScan(const PreScan& preScan);
		~PreScan();
		
		void subscribe(ReScan::EventCallback callback);
		void unsubscribe(ReScan::EventCallback callback);

		int process(const std::string& configFile);
		int process(const PreScanConfig& config);

		int process(
			const bool exportBasesCartesian = true,
			const bool exportBasesEulerAngles = true,
			const bool exportDetailsFile = true,
			const bool writeHeaders = true,
			const bool decimalCharIsDot = true);

		int process(
			const Point3D& point1,
			const Point3D& point2,
			const unsigned int stepAxis1,
			const unsigned int stepAxis2,
			const int planOffset,
			const bool exportBasesCartesian = true,
			const bool exportBasesEulerAngles = true,
			const bool exportDetailsFile = true,
			const bool writeHeaders = true,
			const bool decimalCharIsDot = true);

		std::vector<Base3D*>* getResutlt();
		PreScanResultDetails* getResutltDetails();

	private:
		void clearResults();
		void clearBases();

		void notifyObservers(const FileType fileType, const std::string& path) const;

		void resetProcessData();
		bool fileExists(const std::string& filename) const;

		void selectPoint(std::string& name, Point3D* point) const;
		int selectPointValue(char axisName) const;
		double selectPeakRatio() const;
		int selectPreScanMode(PreScanMode* mode) const;

		unsigned int selectStep(std::string& axisName, unsigned int min, unsigned int max) const;
		unsigned int getPointsNumber(double distance, int step) const;
		void fillBases(std::vector<Base3D*>* bases) const;
		void fillBasesDefault(std::vector<Base3D*>* bases, const double p1rx, const double p1ry, const double p1rz, const double p2rx, const double p2ry, const double p2rz, const Eigen::Matrix4Xd& rotationMatrix) const;
		void fillBasesHorizontal(std::vector<Base3D*>* bases, const double p1rx, const double p1ry, const double p1rz, const double p2rx, const double p2ry, const double p2rz, const Eigen::Matrix4Xd& rotationMatrix) const;
		void fillBasesVertical(std::vector<Base3D*>* bases, const double p1rx, const double p1ry, const double p1rz, const double p2rx, const double p2ry, const double p2rz, const Eigen::Matrix4Xd& rotationMatrix) const;

		int internalProcess();

		bool exportBasesCartesianToCSV(const std::string& basePath, const std::vector<Base3D*>& bases, const std::string& nullText = "") const;
		bool exportBasesEulerAnglesToCSV(const std::string& basePath, const std::vector<Base3D*>& bases, const std::string& nullText = "") const;
		bool exportTrajectoryDetailsFile(const std::string& filename) const;
	};
}

#endif // !RESCAN_PRESCAN_PRESCAN_H

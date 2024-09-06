#ifndef RESCAN_PRESCAN_PRESCANPROCESSDATA_H
#define RESCAN_PRESCAN_PRESCANPROCESSDATA_H

#include "PreScanConfig.h"
#include "Point3D.h"

#include <string>

namespace ReScan::PreScan
{
	class PreScanProcessData
	{
	private:
		bool m_enableUserInput;
		unsigned int* m_stepAxisXY;
		unsigned int* m_stepAxisZ;
		Point3D* m_point1;
		Point3D* m_point2;
		double* m_planOffset;
		double m_distanceXY;
		double m_distanceZ;
		unsigned int m_pointsNumberXY;
		unsigned int m_pointsNumberZ;
		bool m_exportBasesCartesian;
		bool m_exportBasesEulerAngles;
		bool m_exportDetailsFile;
		bool m_writeHeaders;
		bool m_decimalCharIsDot;
		std::string m_basesCartesianDefaultFileName;
		std::string m_basesEulerAnglesDefaultFileName;
		std::string m_detailsDefaultFileName;

	public:
		PreScanProcessData();
		PreScanProcessData(const PreScanProcessData& preScanProcessData);
		~PreScanProcessData();

		/* Setters */

		void setFromConfig(const PreScanConfig& config);
		void reset();

		void resetPoint1();
		void setPoint1(const Point3D* point);

		void resetPoint2();
		void setPoint2(const Point3D* point);

		void resetStepAxisXY();
		void setStepAxisXY(const unsigned int value);

		void resetStepAxisZ();
		void setStepAxisZ(const unsigned int value);

		void resetPlanOffset();
		void setPlanOffset(const double value);

		void setDistanceXY(const double value);
		void setDistanceZ(const double value);

		void setPointsNumberXY(const unsigned int value);
		void setPointsNumberZ(const unsigned int value);

		void setExportBasesCartesian(const bool value);
		void setExportBasesEulerAngles(const bool value);
		void setExportDetailsFile(const bool value);
		void setWriteHeaders(const bool value);
		void setDecimalCharIsDot(const bool value);

		void setBasesCartesianDefaultFileName(const std::string& basesCartesianDefaultFileName);
		void setBasesEulerAnglesDefaultFileName(const std::string& basesEulerAnglesDefaultFileName);
		void setDetailsDefaultFileName(const std::string& detailsDefaultFileName);

		/* End - Setters */

		/* Getters */

		bool getEnableUserInput() const;
		const Point3D* getPoint1() const;
		const Point3D* getPoint2() const;
		const unsigned int* getStepAxisXY() const;
		const unsigned int* getStepAxisZ() const;
		const double* getPlanOffset() const;
		double getDistanceXY() const;
		double getDistanceZ() const;
		unsigned int getPointsNumberXY() const;
		unsigned int getPointsNumberZ() const;
		unsigned int getTotalPointsNumber() const;
		bool getExportBasesCartesian() const;
		bool getExportBasesEulerAngles() const;
		bool getExportDetailsFile() const;
		bool getWriteHeaders() const;
		bool getDecimalCharIsDot() const;
		std::string getBasesCartesianDefaultFileName() const;
		std::string getBasesEulerAnglesDefaultFileName() const;
		std::string getDetailsDefaultFileName() const;

		/* End - Getters */

		void findPlanOffset(const Point3D& point);

		bool isStepXYValid(unsigned int min) const;
		bool isStepZValid(unsigned int min) const;
	};
}

#endif // !RESCAN_PRESCAN_PRESCANPROCESSDATA_H

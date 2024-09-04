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
		unsigned int* m_stepAxis1;
		unsigned int* m_stepAxis2;
		Point3D* m_point1;
		Point3D* m_point2;
		int* m_planOffset;
		double m_distance1;
		double m_distance2;
		unsigned int m_subDivision1;
		unsigned int m_subDivision2;
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

		void resetStepAxis1();
		void setStepAxis1(const unsigned int value);

		void resetStepAxis2();
		void setStepAxis2(const unsigned int value);

		void resetPlanOffset();
		void setPlanOffset(const int value);

		void setDistance1(const double value);
		void setDistance2(const double value);

		void setSubDivision1(const unsigned int value);
		void setSubDivision2(const unsigned int value);

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
		const unsigned int* getStepAxis1() const;
		const unsigned int* getStepAxis2() const;
		const int* getPlanOffset() const;
		double getDistance1() const;
		double getDistance2() const;
		unsigned int getSubDivisions1() const;
		unsigned int getSubDivisions2() const;
		unsigned int getTotalSubDivisions() const;
		bool getExportBasesCartesian() const;
		bool getExportBasesEulerAngles() const;
		bool getExportDetailsFile() const;
		bool getWriteHeaders() const;
		bool getDecimalCharIsDot() const;
		std::string getBasesCartesianDefaultFileName() const;
		std::string getBasesEulerAnglesDefaultFileName() const;
		std::string getDetailsDefaultFileName() const;

		/* End - Getters */

		bool isStep1Valid(unsigned int min) const;
		bool isStep2Valid(unsigned int min) const;
	};
}

#endif // !RESCAN_PRESCAN_PRESCANPROCESSDATA_H

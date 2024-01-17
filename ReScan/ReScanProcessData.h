#ifndef RESCAN_RESCANPROCESSDATA_H
#define RESCAN_RESCANPROCESSDATA_H

#include "Plan2D.h"
#include "ReScanConfig.h"

#include <string>

namespace ReScan
{
	class ReScanProcessData
	{
	private:
		Plan2D* m_plan2D;
		unsigned int* m_stepAxis1;
		unsigned int* m_stepAxis2;
		char m_axis1Name;
		char m_axis2Name;
		double m_distance1;
		double m_distance2;
		unsigned int m_subDivision1;
		unsigned int m_subDivision2;
		std::string m_objFile;
		bool m_exportSubDivisions;
		bool m_exportBasesCartesian;
		bool m_exportBasesEulerAngles;
		bool m_exportDetailsFile;
		bool m_writeHeaders;
		bool m_decimalCharIsDot;

		public:
			ReScanProcessData();
			ReScanProcessData(const ReScanProcessData& reScanProcessData);
			~ReScanProcessData();

			/* Setters */

			void setFromConfig(const ReScanConfig& config);

			void reset();

			void resetPlan2D();
			void setPlan2D(const Plan2D plan2D);

			void resetStepAxis1();
			void setStepAxis1(const unsigned int value);

			void resetStepAxis2();
			void setStepAxis2(const unsigned int value);
			
			void setAxis1Name(const char c);
			void setAxis2Name(const char c);

			void setDistance1(const double value);
			void setDistance2(const double value);

			void setSubDivision1(const unsigned int value);
			void setSubDivision2(const unsigned int value);

			void setObjFile(const std::string& filename);

			void setExportSubDivisions(const bool value);
			void setExportBasesCartesian(const bool value);
			void setExportBasesEulerAngles(const bool value);
			void setExportDetailsFile(const bool value);
			void setWriteHeaders(const bool value);
			void setDecimalCharIsDot(const bool value);

			/* End - Setters */

			/* Getters */

			const Plan2D* getPlan2D() const;
			const unsigned int* getStepAxis1() const;
			const unsigned int* getStepAxis2() const;
			char getAxis1Name() const;
			char getAxis2Name() const;
			double getDistance1() const;
			double getDistance2() const;
			unsigned int getSubDivisions1() const;
			unsigned int getSubDivisions2() const;
			unsigned int getTotalSubDivisions() const;
			std::string getObjFile() const;
			bool getExportSubDivisions() const;
			bool getExportBasesCartesian() const;
			bool getExportBasesEulerAngles() const;
			bool getExportDetailsFile() const;
			bool getWriteHeaders() const;
			bool getDecimalCharIsDot() const;

			/* End - Getters */

			bool isStep1Valid(unsigned int min) const;
			bool isStep2Valid(unsigned int min) const;
	};
}

#endif // !RESCAN_RESCANPROCESSDATA_H



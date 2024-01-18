#ifndef RESCAN_RESCANCONFIG_H
#define RESCAN_RESCANCONFIG_H

#include "Plan2D.h"
#include "macros.h"

#include <string>

namespace ReScan
{
	class ReScanConfig
	{
	private:
		std::string m_objFile;
		Plan2D m_plan2D;
		unsigned int m_xAxisStep;
		unsigned int m_yAxisStep;
		bool m_exportSubDivisions;
		bool m_exportBasesCartesian;
		bool m_exportBasesEulerAngles;
		bool m_exportDetailsFile;
		bool m_writeHeaders;
		bool m_decimalCharIsDot;

	private:
		ReScanConfig(const ReScanConfig& config);

	public:
		ReScanConfig();
		~ReScanConfig();

		bool getEnableCout() const;
		std::string getObjFile() const;
		Plan2D getPlan2D() const;
		unsigned int getStepAxis1() const;
		unsigned int getStepAxis2() const;
		bool getExportSubDivisions() const;
		bool getExportBasesCartesian() const;
		bool getExportBasesEulerAngles() const;
		bool getExportDetailsFile() const;
		bool getWriteHeaders() const;
		bool getDecimalCharIsDot() const;

		static bool isFileValid(const std::string& filename);
		static int loadConfigFromFile(const std::string& filePath, ReScanConfig* config);
		static int saveConfigToFile(const ReScanConfig& config, const std::string& filePath);

		static ReScanConfig createDassaultConfig();
	};
}

#endif // RESCAN_RESCANCONFIG_H

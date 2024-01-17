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

		unsigned int getXAxisStep();
		unsigned int getYAxisStep();
		bool getExportSubDivisions();
		bool getExportBasesCartesian();
		bool getExportBasesEulerAngles();
		bool getExportDetailsFile();
		bool getWriteHeaders();
		bool getDecimalCharIsDot();

		static int loadConfigFromFile(const std::string& filePath, ReScanConfig* config);
		static int saveConfigToFile(const ReScanConfig& config, const std::string& filePath);
	};
}

#endif // RESCAN_RESCANCONFIG_H

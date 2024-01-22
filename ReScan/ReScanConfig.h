#ifndef RESCAN_RESCANCONFIG_H
#define RESCAN_RESCANCONFIG_H

#include "Plan2D.h"
#include "macros.h"
#include "Base3D.h"

#include <vector>
#include <string>
#include <iostream>
#include <sstream>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/ini_parser.hpp>
#include <stdexcept>
#include <type_traits> // for is_same
#include <algorithm>   // for std::transform

namespace ReScan
{
	class ReScanConfig
	{
	private:
		bool m_enableUserInput;
		std::string m_objFile;
		Plan2D m_plan2D;
		unsigned int m_xAxisStep;
		unsigned int m_yAxisStep;
		Base3D m_referenceBase;
		bool m_exportSubDivisions;
		bool m_exportBasesCartesian;
		bool m_exportBasesEulerAngles;
		bool m_exportDetailsFile;
		bool m_writeHeaders;
		bool m_decimalCharIsDot;

	private:
		ReScanConfig(const ReScanConfig& config);

		template<typename T>
		static const T getConfigNode(const boost::property_tree::ptree& pt, const std::string& path)
		{
			std::string valueStr = pt.get<std::string>(path);

			T value{};
			std::stringstream ss(valueStr);

			if (std::is_same<T, std::string>::value)
			{
				ss >> value;
				return value;
			}
			else
			{
				if (valueStr.empty())
				{
					throw std::invalid_argument("Value is empty - Path: " + path);
				}
			}

			if (std::is_unsigned<T>::value && valueStr[0] == '-')
			{
				throw std::invalid_argument("Failed to convert value");
			}

			try
			{
				if (std::is_same<T, bool>::value)
				{
					std::transform(valueStr.begin(), valueStr.end(), valueStr.begin(), ::tolower);

					if (valueStr == "true")
						value = true;
					else if (valueStr == "false")
						value = false;
					else
						throw std::invalid_argument("Invalid boolean value");
				}
				else if (!(ss >> value))
				{
					throw std::invalid_argument("Failed to convert value");
				}
				else if (!ss.eof())
				{
					throw std::invalid_argument("Failed to convert value");
				}
			}
			catch (const std::exception& e)
			{
				std::string message(e.what());
				message += " - Path: " + path + " - Value: " + valueStr;
				throw std::runtime_error(message);
			}

			return value;
		}

		static std::vector<double> splitAndConvert(const std::string& inputString);

	public:
		ReScanConfig();
		~ReScanConfig();

		bool getEnableUserInput() const;
		std::string getObjFile() const;
		Plan2D getPlan2D() const;
		unsigned int getStepAxis1() const;
		unsigned int getStepAxis2() const;
		const Base3D* getReferenceBase() const;
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

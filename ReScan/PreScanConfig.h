#ifndef RESCAN_PRESCAN_PRESCANCONFIG_H
#define RESCAN_PRESCAN_PRESCANCONFIG_H

#include "macros.h"
#include "Point3D.h"

#include <vector>
#include <string>
#include <iostream>
#include <sstream>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/ini_parser.hpp>
#include <stdexcept>
#include <type_traits> // for is_same
#include <algorithm>   // for std::transform

namespace ReScan::PreScan
{
	class PreScanConfig
	{
	private:
		bool m_enableUserInput;
		unsigned int m_xyAxisStep;
		unsigned int m_zAxisStep;
		Point3D m_point1;
		Point3D m_point2;
		double m_planOffset;
		bool m_exportBasesCartesian;
		bool m_exportBasesEulerAngles;
		bool m_exportDetailsFile;
		bool m_writeHeaders;
		bool m_decimalCharIsDot;
		std::string m_basesCartesianDefaultFileName;
		std::string m_basesEulerAnglesDefaultFileName;
		std::string m_detailsDefaultFileName;

	private:
		PreScanConfig(const PreScanConfig& config);

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
					{
						ss = std::stringstream("1");
						ss >> value;
					}
					else if (valueStr == "false")
					{
						ss = std::stringstream("0");
						ss >> value;
					}
					else
					{
						throw std::invalid_argument("Invalid boolean value");
					}
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
		PreScanConfig();
		~PreScanConfig();

		/* Getters */

		bool getEnableUserInput() const;
		unsigned int getStepAxisXY() const;
		unsigned int getStepAxisZ() const;
		const Point3D* getPoint1() const;
		const Point3D* getPoint2() const;
		double getPlanOffset() const;
		bool getExportBasesCartesian() const;
		bool getExportBasesEulerAngles() const;
		bool getExportDetailsFile() const;
		bool getWriteHeaders() const;
		bool getDecimalCharIsDot() const;
		std::string getBasesCartesianDefaultFileName() const;
		std::string getBasesEulerAnglesDefaultFileName() const;
		std::string getDetailsDefaultFileName() const;

		/* Setters */

		void setEnableUserInput(const bool enableUserInput);
		void setStepAxisXY(const unsigned int xyAxisStep);
		void setStepAxisZ(const unsigned int zAxisStep);
		void setPoint1(const Point3D& point1);
		void setPoint2(const Point3D& point2);
		void setPlanOffset(const double distance);
		void setExportBasesCartesian(const bool exportBasesCartesian);
		void setExportBasesEulerAngles(const bool exportBasesEulerAngles);
		void setExportDetailsFile(const bool exportDetailsFile);
		void setWriteHeaders(const bool writeHeaders);
		void setDecimalCharIsDot(const bool decimalCharIsDot);
		void setBasesCartesianDefaultFileName(const std::string& basesCartesianDefaultFileName);
		void setBasesEulerAnglesDefaultFileName(const std::string& basesEulerAnglesDefaultFileName);
		void setDetailsDefaultFileName(const std::string& detailsDefaultFileName);

		void findPlanOffset(const Point3D& point);

		static bool isFileValid(const std::string& filename);
		static int loadConfigFromFile(const std::string& filePath, PreScanConfig* config);
		static int saveConfigToFile(const PreScanConfig& config, const std::string& filePath);
		
		static PreScanConfig createConfig();
	};
}

#endif // RESCAN_PRESCAN_PRESCANCONFIG_H

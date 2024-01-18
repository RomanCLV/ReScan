#include "ReScanConfig.h"
#include "tools.h"
#include "StreamHelper.h"

#include <iostream>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/ini_parser.hpp>

namespace ReScan
{
	ReScanConfig::ReScanConfig() :
		m_objFile(""),
		m_plan2D(Plan2D::XY),
		m_xAxisStep(100),
		m_yAxisStep(100),
		m_exportSubDivisions(false),
		m_exportBasesCartesian(true),
		m_exportBasesEulerAngles(true),
		m_exportDetailsFile(true),
		m_writeHeaders(true),
		m_decimalCharIsDot(true)
	{
	}

	ReScanConfig::ReScanConfig(const ReScanConfig& config) :
		m_objFile(config.m_objFile),
		m_plan2D(config.m_plan2D),
		m_xAxisStep(config.m_xAxisStep),
		m_yAxisStep(config.m_yAxisStep),
		m_exportSubDivisions(config.m_exportSubDivisions),
		m_exportBasesCartesian(config.m_exportBasesCartesian),
		m_exportBasesEulerAngles(config.m_exportBasesEulerAngles),
		m_exportDetailsFile(config.m_exportDetailsFile),
		m_writeHeaders(config.m_writeHeaders),
		m_decimalCharIsDot(config.m_decimalCharIsDot)
	{
	}

	ReScanConfig::~ReScanConfig()
	{
	}

	std::string ReScanConfig::getObjFile() const
	{
		return m_objFile;
	}

	Plan2D ReScanConfig::getPlan2D() const
	{
		return m_plan2D;
	}

	unsigned int ReScanConfig::getStepAxis1() const
	{
		return m_xAxisStep;
	}

	unsigned int ReScanConfig::getStepAxis2() const
	{
		return m_yAxisStep;
	}

	bool ReScanConfig::getExportSubDivisions() const
	{
		return m_exportSubDivisions;
	}

	bool ReScanConfig::getExportBasesCartesian() const
	{
		return m_exportBasesCartesian;
	}

	bool ReScanConfig::getExportBasesEulerAngles() const
	{
		return m_exportBasesEulerAngles;
	}

	bool ReScanConfig::getExportDetailsFile() const
	{
		return m_exportDetailsFile;
	}

	bool ReScanConfig::getWriteHeaders() const
	{
		return m_writeHeaders;
	}

	bool ReScanConfig::getDecimalCharIsDot() const
	{
		return m_decimalCharIsDot;
	}

	bool ReScanConfig::isFileValid(const std::string& filename)
	{
		std::ifstream fileExists(filename);
		if (!fileExists)
		{
			ReScan::StreamHelper::out << "File: " << filename << " not found." << std::endl;
			return false;
		}
		if (filename.length() < 4 || filename.substr(filename.length() - 4) != ".ini")
		{
			ReScan::StreamHelper::out << "File is not .ini" << std::endl;
			return false;
		}
		return true;
	}

	int ReScanConfig::loadConfigFromFile(const std::string& filePath, ReScanConfig* config)
	{
		int result = SUCCESS_CODE;

		if (isFileValid(filePath))
		{
			boost::property_tree::ptree pt;
			try
			{
				boost::property_tree::read_ini(filePath, pt);
			}
			catch (const std::exception& e)
			{
				result = READ_CONFIG_ERROR_CODE;
				ReScan::StreamHelper::out << "error occured when reading config file:" << filePath << std::endl << e.what() << std::endl;
			}

			if (result == SUCCESS_CODE)
			{
				try
				{
					config->m_objFile = pt.get<std::string>("General.objFile", "");
					std::string planStr = pt.get<std::string>("General.plan2D");
					Plan2D plan2D;
					if (Tools::stringToPlan2D(planStr, plan2D) != SUCCESS_CODE)
					{
						throw std::exception("Invalid value for Plan2D");
					}
					config->m_plan2D = plan2D;
					config->m_xAxisStep = pt.get<unsigned int>("General.xAxisStep", 0);
					config->m_yAxisStep = pt.get<unsigned int>("General.yAxisStep", 0);
					config->m_decimalCharIsDot = pt.get<bool>("General.decimalCharIsDot", false);
					config->m_exportSubDivisions = pt.get<bool>("Export.exportSubDivisions", false);
					config->m_exportBasesCartesian = pt.get<bool>("Export.exportBasesCartesian", false);
					config->m_exportBasesEulerAngles = pt.get<bool>("Export.exportBasesEulerAngles", false);
					config->m_exportDetailsFile = pt.get<bool>("Export.exportDetailsFile", false);
					config->m_writeHeaders = pt.get<bool>("Export.writeHeaders", false);
				}
				catch (const std::exception& e)
				{
					result = SET_CONFIG_ERROR_CODE;
					ReScan::StreamHelper::out << "error occured when setting read config file:" << std::endl << e.what() << std::endl;
				}
			}
		}
		else
		{
			result = FILE_NOT_FOUND_ERROR_CODE;
		}
		return result;
	}

	int ReScanConfig::saveConfigToFile(const ReScanConfig& config, const std::string& filePath)
	{
		int result = SUCCESS_CODE;
		boost::property_tree::ptree pt;

		std::string planStr;
		Tools::plan2DToString(config.m_plan2D, planStr);

		pt.put("General.objFile", config.m_objFile);
		pt.put("General.plan2D", planStr);
		pt.put("General.xAxisStep", config.m_xAxisStep);
		pt.put("General.yAxisStep", config.m_yAxisStep);
		pt.put("General.decimalCharIsDot", config.m_decimalCharIsDot);

		pt.put("Export.exportSubDivisions", config.m_exportSubDivisions);
		pt.put("Export.exportBasesCartesian", config.m_exportBasesCartesian);
		pt.put("Export.exportBasesEulerAngles", config.m_exportBasesEulerAngles);
		pt.put("Export.exportDetailsFile", config.m_exportDetailsFile);
		pt.put("Export.writeHeaders", config.m_writeHeaders);

		try
		{
			boost::property_tree::write_ini(filePath, pt);
			ReScan::StreamHelper::out << "Config saved: " << filePath << std::endl << std::endl;
		}
		catch (const std::exception& e)
		{
			result = SAVE_CONFIG_ERROR_CODE;
			ReScan::StreamHelper::out << "error occured when writing config file:" << std::endl << e.what() << std::endl;
		}
		return result;
	}

	ReScanConfig ReScanConfig::createDassaultConfig()
	{
		ReScanConfig config;
		config.m_objFile = "Resulting-Mesh-smoothed.obj";
		config.m_plan2D = Plan2D::YZ;
		config.m_exportBasesCartesian = false;
		config.m_writeHeaders = false;
		return config;
	}
}
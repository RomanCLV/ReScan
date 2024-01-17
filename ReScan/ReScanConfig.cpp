#include "ReScanConfig.h"

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

	unsigned int ReScanConfig::getXAxisStep()
	{
		return m_xAxisStep;
	}

	unsigned int ReScanConfig::getYAxisStep()
	{
		return m_yAxisStep;
	}

	bool ReScanConfig::getExportSubDivisions()
	{
		return m_exportSubDivisions;
	}

	bool ReScanConfig::getExportBasesCartesian()
	{
		return m_exportBasesCartesian;
	}

	bool ReScanConfig::getExportBasesEulerAngles()
	{
		return m_exportBasesEulerAngles;
	}

	bool ReScanConfig::getExportDetailsFile()
	{
		return m_exportDetailsFile;
	}

	bool ReScanConfig::getWriteHeaders()
	{
		return m_writeHeaders;
	}

	bool ReScanConfig::getDecimalCharIsDot()
	{
		return m_decimalCharIsDot;
	}


	int ReScanConfig::loadConfigFromFile(const std::string& filePath, ReScanConfig* config)
	{
		int result = SUCCESS_CODE;

		boost::property_tree::ptree pt;
		try
		{
			boost::property_tree::read_ini(filePath, pt);		
		}
		catch (const std::exception& e)
		{
			result = READ_CONFIG_ERROR_CODE;
			std::cerr << "error occured when reading config file:" << filePath << std::endl << e.what() << std::endl;
		}

		if (result == SUCCESS_CODE)
		{
			try
			{
				config->m_objFile = pt.get<std::string>("General.objFile", "");
				config->m_plan2D = static_cast<Plan2D>(pt.get<int>("General.plan2D", 0));
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
				std::cerr << "error occured when setting read config file:" << std::endl << e.what() << std::endl;
			}
		}

		return result;
	}

	int ReScanConfig::saveConfigToFile(const ReScanConfig& config, const std::string& filePath)
	{
		int result = SUCCESS_CODE;
		boost::property_tree::ptree pt;

		pt.put("General.objFile", config.m_objFile);
		pt.put("General.plan2D", static_cast<int>(config.m_plan2D));
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
		}
		catch (const std::exception& e)
		{
			result = SAVE_CONFIG_ERROR_CODE;
			std::cerr << "error occured when writing config file:" << std::endl << e.what() << std::endl;
		}
		return result;
	}
}
#include "PreScanConfig.h"
#include "tools.h"
#include "MultiOStream.h"

namespace ReScan::PreScan
{
	PreScanConfig::PreScanConfig() :
		m_enableUserInput(true),
		m_xAxisStep(100),
		m_yAxisStep(100),
		m_point1(Point3D(0., 0., 0.)),
		m_point2(Point3D(100., 0., 0.)),
		m_planOffset(0),
		m_exportBasesCartesian(true),
		m_exportBasesEulerAngles(true),
		m_exportDetailsFile(true),
		m_writeHeaders(true),
		m_decimalCharIsDot(true),
		m_basesCartesianDefaultFileName(""),
		m_basesEulerAnglesDefaultFileName(""),
		m_detailsDefaultFileName("")
	{
	}

	PreScanConfig::PreScanConfig(const PreScanConfig& config) :
		m_enableUserInput(config.m_enableUserInput),
		m_xAxisStep(config.m_xAxisStep),
		m_yAxisStep(config.m_yAxisStep),
		m_point1(config.m_point1),
		m_point2(config.m_point2),
		m_planOffset(config.m_planOffset),
		m_exportBasesCartesian(config.m_exportBasesCartesian),
		m_exportBasesEulerAngles(config.m_exportBasesEulerAngles),
		m_exportDetailsFile(config.m_exportDetailsFile),
		m_writeHeaders(config.m_writeHeaders),
		m_decimalCharIsDot(config.m_decimalCharIsDot),
		m_basesCartesianDefaultFileName(config.m_basesCartesianDefaultFileName),
		m_basesEulerAnglesDefaultFileName(config.m_basesEulerAnglesDefaultFileName),
		m_detailsDefaultFileName(config.m_detailsDefaultFileName)
	{
	}

	PreScanConfig::~PreScanConfig()
	{
	}

	/* Getters */

	bool PreScanConfig::getEnableUserInput() const
	{
		return m_enableUserInput;
	}

	unsigned int PreScanConfig::getStepAxis1() const
	{
		return m_xAxisStep;
	}

	unsigned int PreScanConfig::getStepAxis2() const
	{
		return m_yAxisStep;
	}

	const Point3D* PreScanConfig::getPoint1() const
	{
		return &m_point1;
	}

	const Point3D* PreScanConfig::getPoint2() const
	{
		return &m_point2;
	}

	int PreScanConfig::getPlanOffset() const
	{
		return m_planOffset;
	}

	bool PreScanConfig::getExportBasesCartesian() const
	{
		return m_exportBasesCartesian;
	}

	bool PreScanConfig::getExportBasesEulerAngles() const
	{
		return m_exportBasesEulerAngles;
	}

	bool PreScanConfig::getExportDetailsFile() const
	{
		return m_exportDetailsFile;
	}

	bool PreScanConfig::getWriteHeaders() const
	{
		return m_writeHeaders;
	}

	bool PreScanConfig::getDecimalCharIsDot() const
	{
		return m_decimalCharIsDot;
	}

	std::string PreScanConfig::getBasesCartesianDefaultFileName() const
	{
		return m_basesCartesianDefaultFileName;
	}

	std::string PreScanConfig::getBasesEulerAnglesDefaultFileName() const
	{
		return m_basesEulerAnglesDefaultFileName;
	}

	std::string PreScanConfig::getDetailsDefaultFileName() const
	{
		return m_detailsDefaultFileName;
	}

	/* Setters */

	void PreScanConfig::setEnableUserInput(const bool enableUserInput)
	{
		m_enableUserInput = enableUserInput;
	}

	void PreScanConfig::setStepAxis1(const unsigned int xAxisStep)
	{
		m_xAxisStep = xAxisStep;
	}

	void PreScanConfig::setStepAxis2(const unsigned int yAxisStep)
	{
		m_yAxisStep = yAxisStep;
	}

	void PreScanConfig::setPoint1(const Point3D& point1)
	{
		m_point1 = point1;
	}

	void PreScanConfig::setPoint2(const Point3D& point2)
	{
		m_point2 = point2;
	}

	void PreScanConfig::setPlanOffset(const int distance)
	{
		m_planOffset = distance;
	}

	void PreScanConfig::setExportBasesCartesian(const bool exportBasesCartesian)
	{
		m_exportBasesCartesian = exportBasesCartesian;
	}

	void PreScanConfig::setExportBasesEulerAngles(const bool exportBasesEulerAngles)
	{
		m_exportBasesEulerAngles = exportBasesEulerAngles;
	}

	void PreScanConfig::setExportDetailsFile(const bool exportDetailsFile)
	{
		m_exportDetailsFile = exportDetailsFile;
	}

	void PreScanConfig::setWriteHeaders(const bool writeHeaders)
	{
		m_writeHeaders = writeHeaders;
	}

	void PreScanConfig::setDecimalCharIsDot(const bool decimalCharIsDot)
	{
		m_decimalCharIsDot = decimalCharIsDot;
	}

	void PreScanConfig::setBasesCartesianDefaultFileName(const std::string& basesCartesianDefaultFileName)
	{
		m_basesCartesianDefaultFileName = basesCartesianDefaultFileName;
	}

	void PreScanConfig::setBasesEulerAnglesDefaultFileName(const std::string& basesEulerAnglesDefaultFileName)
	{
		m_basesEulerAnglesDefaultFileName = basesEulerAnglesDefaultFileName;
	}

	void PreScanConfig::setDetailsDefaultFileName(const std::string& detailsDefaultFileName)
	{
		m_detailsDefaultFileName = detailsDefaultFileName;
	}

	/* Static */

	bool PreScanConfig::isFileValid(const std::string& filename)
	{
		std::ifstream fileExists(filename);
		if (!fileExists)
		{
			ReScan::mout << "File: " << filename << " not found." << std::endl;
			return false;
		}
		if (filename.length() < 4 || filename.substr(filename.length() - 4) != ".ini")
		{
			ReScan::mout << "File is not .ini" << std::endl;
			return false;
		}
		return true;
	}

	int PreScanConfig::loadConfigFromFile(const std::string& filePath, PreScanConfig* config)
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
				ReScan::mout << "error occured when reading config file:" << filePath << std::endl << e.what() << std::endl;
			}

			if (result == SUCCESS_CODE)
			{
				try
				{
					config->m_xAxisStep = getConfigNode<unsigned int>(pt, "General.xAxisStep");
					config->m_yAxisStep = getConfigNode<unsigned int>(pt, "General.yAxisStep");
					config->m_decimalCharIsDot = getConfigNode<bool>(pt, "General.decimalCharIsDot");
					config->m_enableUserInput = getConfigNode<bool>(pt, "General.enableUserInput");
					std::string point1Str = getConfigNode<std::string>(pt, "General.point1");
					try
					{
						std::vector<double> pointValues = splitAndConvert(point1Str);
						if (pointValues.size() != 3)
						{
							throw std::runtime_error("Invalid point 1. Number of element not equal to 3.");
						}
						config->m_point1.setFrom(Point3D(pointValues[0], pointValues[1], pointValues[2]));
					}
					catch (const std::exception& e)
					{
						std::string message(e.what());
						throw std::runtime_error(message + " - Value: " + point1Str);
					}
					
					std::string point2Str = getConfigNode<std::string>(pt, "General.point2");
					try
					{
						std::vector<double> pointValues = splitAndConvert(point2Str);
						if (pointValues.size() != 3)
						{
							throw std::runtime_error("Invalid point 2. Number of element not equal to 3.");
						}
						config->m_point2.setFrom(Point3D(pointValues[0], pointValues[1], pointValues[2]));
					}
					catch (const std::exception& e)
					{
						std::string message(e.what());
						throw std::runtime_error(message + " - Value: " + point2Str);
					}

					config->m_exportBasesCartesian = getConfigNode<bool>(pt, "Export.exportBasesCartesian");
					config->m_exportBasesEulerAngles = getConfigNode<bool>(pt, "Export.exportBasesEulerAngles");
					config->m_exportDetailsFile = getConfigNode<bool>(pt, "Export.exportDetailsFile");
					config->m_writeHeaders = getConfigNode<bool>(pt, "Export.writeHeaders");
					config->m_basesCartesianDefaultFileName = getConfigNode<std::string>(pt, "Export.basesCartesianDefaultFileName");
					config->m_basesEulerAnglesDefaultFileName = getConfigNode<std::string>(pt, "Export.basesEulerAnglesDefaultFileName");
					config->m_detailsDefaultFileName = getConfigNode<std::string>(pt, "Export.detailsDefaultFileName");
				}
				catch (const std::exception& e)
				{
					result = SET_CONFIG_ERROR_CODE;
					ReScan::mout << "error occured when setting config from file:" << std::endl << e.what() << std::endl;
				}
			}
		}
		else
		{
			result = FILE_NOT_FOUND_ERROR_CODE;
		}
		return result;
	}

	std::vector<double> PreScanConfig::splitAndConvert(const std::string& inputString)
	{
		std::vector<double> doubleValues;
		std::istringstream ss(inputString);
		std::string token;

		while (std::getline(ss, token, ';'))
		{
			doubleValues.push_back(std::stod(token));
		}

		return doubleValues;
	}

	int PreScanConfig::saveConfigToFile(const PreScanConfig& config, const std::string& filePath)
	{
		int result = SUCCESS_CODE;
		boost::property_tree::ptree pt;


		pt.put("General.xAxisStep", config.m_xAxisStep);
		pt.put("General.yAxisStep", config.m_yAxisStep);
		pt.put("General.decimalCharIsDot", config.m_decimalCharIsDot);
		pt.put("General.enableUserInput", config.m_enableUserInput);
		pt.put("General.point1", config.m_point1.toStr());
		pt.put("General.point2", config.m_point2.toStr());
		pt.put("General.planOffset", config.m_planOffset);

		pt.put("Export.exportBasesCartesian", config.m_exportBasesCartesian);
		pt.put("Export.exportBasesEulerAngles", config.m_exportBasesEulerAngles);
		pt.put("Export.exportDetailsFile", config.m_exportDetailsFile);
		pt.put("Export.writeHeaders", config.m_writeHeaders);
		pt.put("Export.basesCartesianDefaultFileName", config.m_basesCartesianDefaultFileName);
		pt.put("Export.basesEulerAnglesDefaultFileName", config.m_basesEulerAnglesDefaultFileName);
		pt.put("Export.detailsDefaultFileName", config.m_detailsDefaultFileName);

		try
		{
			boost::property_tree::write_ini(filePath, pt);
			ReScan::mout << "Config saved: " << filePath << std::endl << std::endl;
		}
		catch (const std::exception& e)
		{
			result = SAVE_CONFIG_ERROR_CODE;
			ReScan::mout << "error occured when writing config file:" << std::endl << e.what() << std::endl;
		}
		return result;
	}

	PreScanConfig PreScanConfig::createICNDEConfig()
	{
		PreScanConfig config;
		config.m_enableUserInput = false;
		config.m_exportBasesCartesian = false;
		config.m_writeHeaders = false;
		return config;
	}
}
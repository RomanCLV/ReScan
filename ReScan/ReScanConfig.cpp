#include "ReScanConfig.h"
#include "tools.h"
#include "MultiOStream.h"

namespace ReScan
{
	ReScanConfig::ReScanConfig() :
		m_enableUserInput(true),
		m_objFile(""),
		m_plan2D(Plan2D::XY),
		m_xAxisStep(100),
		m_yAxisStep(100),
		m_referenceBase(),
		m_exportSubDivisions(false),
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

	ReScanConfig::ReScanConfig(const ReScanConfig& config) :
		m_enableUserInput(config.m_enableUserInput),
		m_objFile(config.m_objFile),
		m_plan2D(config.m_plan2D),
		m_xAxisStep(config.m_xAxisStep),
		m_yAxisStep(config.m_yAxisStep),
		m_referenceBase(config.m_referenceBase),
		m_exportSubDivisions(config.m_exportSubDivisions),
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

	ReScanConfig::~ReScanConfig()
	{
	}

	/* Getters */

	bool ReScanConfig::getEnableUserInput() const
	{
		return m_enableUserInput;
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

	const Base3D* ReScanConfig::getReferenceBase() const
	{
		return &m_referenceBase;
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

	std::string ReScanConfig::getBasesCartesianDefaultFileName() const
	{
		return m_basesCartesianDefaultFileName;
	}

	std::string ReScanConfig::getBasesEulerAnglesDefaultFileName() const
	{
		return m_basesEulerAnglesDefaultFileName;
	}

	std::string ReScanConfig::getDetailsDefaultFileName() const
	{
		return m_detailsDefaultFileName;
	}

	/* Setters */

	void ReScanConfig::setEnableUserInput(const bool enableUserInput)
	{
		m_enableUserInput = enableUserInput;
	}

	void ReScanConfig::setObjFile(const std::string& objFile)
	{
		m_objFile = objFile;
	}

	void ReScanConfig::setPlan2D(const Plan2D& plan2D)
	{
		m_plan2D = plan2D;
	}

	void ReScanConfig::setStepAxis1(const unsigned int xAxisStep)
	{
		m_xAxisStep = xAxisStep;
	}

	void ReScanConfig::setStepAxis2(const unsigned int yAxisStep)
	{
		m_yAxisStep = yAxisStep;
	}

	void ReScanConfig::setReferenceBase(const Base3D& referenceBase)
	{
		m_referenceBase = referenceBase;
	}

	void ReScanConfig::setExportSubDivisions(const bool exportSubDivisions)
	{
		m_exportSubDivisions = exportSubDivisions;
	}

	void ReScanConfig::setExportBasesCartesian(const bool exportBasesCartesian)
	{
		m_exportBasesCartesian = exportBasesCartesian;
	}

	void ReScanConfig::setExportBasesEulerAngles(const bool exportBasesEulerAngles)
	{
		m_exportBasesEulerAngles = exportBasesEulerAngles;
	}

	void ReScanConfig::setExportDetailsFile(const bool exportDetailsFile)
	{
		m_exportDetailsFile = exportDetailsFile;
	}

	void ReScanConfig::setWriteHeaders(const bool writeHeaders)
	{
		m_writeHeaders = writeHeaders;
	}

	void ReScanConfig::setDecimalCharIsDot(const bool decimalCharIsDot)
	{
		m_decimalCharIsDot = decimalCharIsDot;
	}

	void ReScanConfig::setBasesCartesianDefaultFileName(const std::string& basesCartesianDefaultFileName)
	{
		m_basesCartesianDefaultFileName = basesCartesianDefaultFileName;
	}

	void ReScanConfig::setBasesEulerAnglesDefaultFileName(const std::string& basesEulerAnglesDefaultFileName)
	{
		m_basesEulerAnglesDefaultFileName = basesEulerAnglesDefaultFileName;
	}

	void ReScanConfig::setDetailsDefaultFileName(const std::string& detailsDefaultFileName)
	{
		m_detailsDefaultFileName = detailsDefaultFileName;
	}

	/* Static */

	bool ReScanConfig::isFileValid(const std::string& filename)
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
				ReScan::mout << "error occured when reading config file:" << filePath << std::endl << e.what() << std::endl;
			}

			if (result == SUCCESS_CODE)
			{
				try
				{
					config->m_objFile = getConfigNode<std::string>(pt, "General.objFile");
					std::string planStr = getConfigNode<std::string>(pt, "General.plan2D");
					Plan2D plan2D;
					if (Tools::stringToPlan2D(planStr, plan2D) != SUCCESS_CODE)
					{
						throw std::runtime_error("Invalid value for Plan2D - Value " + planStr);
					}
					config->m_plan2D = plan2D;
					config->m_xAxisStep = getConfigNode<unsigned int>(pt, "General.xAxisStep");
					config->m_yAxisStep = getConfigNode<unsigned int>(pt, "General.yAxisStep");
					config->m_decimalCharIsDot = getConfigNode<bool>(pt, "General.decimalCharIsDot");
					config->m_enableUserInput = getConfigNode<bool>(pt, "General.enableUserInput");
					std::string fixBaseStr = getConfigNode<std::string>(pt, "General.referenceBase");

					try
					{
						std::vector<double> baseValues = splitAndConvert(fixBaseStr);
						if (baseValues.size() != 9)
						{
							throw std::runtime_error("Invalid reference base. Number of element not equal to 9.");
						}
						config->m_referenceBase.setFrom(Base3D(baseValues[0], baseValues[1], baseValues[2], baseValues[3], baseValues[4], baseValues[5], baseValues[6], baseValues[7], baseValues[8]));
					}
					catch (const std::exception& e)
					{
						std::string message(e.what());
						throw std::runtime_error(message + " - Value: " + fixBaseStr);
					}

					config->m_exportSubDivisions = getConfigNode<bool>(pt, "Export.exportSubDivisions");
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

	std::vector<double> ReScanConfig::splitAndConvert(const std::string& inputString) 
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
		pt.put("General.enableUserInput", config.m_enableUserInput);
		pt.put("General.referenceBase", config.m_referenceBase.toStr());

		pt.put("Export.exportSubDivisions", config.m_exportSubDivisions);
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

	ReScanConfig ReScanConfig::createFrontalICNDEConfig()
	{
		ReScanConfig config;
		config.m_enableUserInput = false;
		config.m_objFile = "Resulting-Mesh-smoothed.obj";
		config.m_plan2D = Plan2D::YZ;
		config.m_exportBasesCartesian = false;
		config.m_writeHeaders = false;
		config.m_referenceBase = Base3D(0.0, -1.0, 0.0, 0.0, 0.0, 1.0, -1.0, 0.0, 0.0);
		return config;
	}

	ReScanConfig ReScanConfig::createLateralICNDEConfig()
	{
		ReScanConfig config;
		config.m_enableUserInput = false;
		config.m_objFile = "Resulting-Mesh-smoothed.obj";
		config.m_plan2D = Plan2D::XZ;
		config.m_exportBasesCartesian = false;
		config.m_writeHeaders = false;
		config.m_referenceBase = Base3D(-1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 1.0, 0.0);
		return config;
	}
}
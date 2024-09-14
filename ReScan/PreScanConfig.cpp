#include "PreScanConfig.h"
#include "tools.h"
#include "MultiOStream.h"

namespace ReScan::PreScan
{
	PreScanConfig::PreScanConfig() :
		m_enableUserInput(true),
		m_xyAxisStep(100),
		m_zAxisStep(100),
		m_point1(Point3D(0., 0., 0.)),
		m_point2(Point3D(100., 0., 0.)),
		m_planOffset(0),
		m_peakRatio(0.5),
		m_preScanMode(PreScanMode::Default),
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
		m_xyAxisStep(config.m_xyAxisStep),
		m_zAxisStep(config.m_zAxisStep),
		m_point1(config.m_point1),
		m_point2(config.m_point2),
		m_planOffset(config.m_planOffset),
		m_peakRatio(config.m_peakRatio),
		m_preScanMode(config.m_preScanMode),
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

	unsigned int PreScanConfig::getStepAxisXY() const
	{
		return m_xyAxisStep;
	}

	unsigned int PreScanConfig::getStepAxisZ() const
	{
		return m_zAxisStep;
	}

	const Point3D* PreScanConfig::getPoint1() const
	{
		return &m_point1;
	}

	const Point3D* PreScanConfig::getPoint2() const
	{
		return &m_point2;
	}

	double PreScanConfig::getPlanOffset() const
	{
		return m_planOffset;
	}

	double PreScanConfig::getPeakRatio() const
	{
		return m_peakRatio;
	}

	PreScanMode PreScanConfig::getPreScanMode() const
	{
		return m_preScanMode;
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

	void PreScanConfig::setStepAxisXY(const unsigned int xyAxisStep)
	{
		m_xyAxisStep = xyAxisStep;
	}

	void PreScanConfig::setStepAxisZ(const unsigned int zAxisStep)
	{
		m_zAxisStep = zAxisStep;
	}

	void PreScanConfig::setPoint1(const Point3D& point1)
	{
		m_point1 = point1;
	}

	void PreScanConfig::setPoint2(const Point3D& point2)
	{
		m_point2 = point2;
	}

	void PreScanConfig::setPlanOffset(const double distance)
	{
		m_planOffset = distance;
	}

	void PreScanConfig::setPeakRatio(const double peakRatio)
	{
		m_peakRatio = (peakRatio < 0.0 || peakRatio > 1.0) ? 0.5 : peakRatio;
	}

	void PreScanConfig::setPreScanMode(const PreScanMode mode)
	{
		m_preScanMode = mode;
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

	int PreScanConfig::findPlanOffsetAndPeakRatio(const Point3D& point, double& planOffset, double& peakRatio) const
	{
		planOffset = 0.0;
		peakRatio = 0.0;

		double p1x = m_point1.getX();
		double p1y = m_point1.getY();
		double p2x = m_point2.getX();
		double p2y = m_point2.getY();

		if (p1x == p2x && p1y == p2y)
		{
			return IDENTICAL_POINTS_ERROR_CODE;
		}

		// direction
		//double ux = p2x - p1x;
		//double uy = p2y - p1y;

		// normal
		//double nx = -uy;
		//double ny = ux;

		// angle
		//double angle = atan2(ny, nx);
		double angle = atan2(p2x - p1x, p1y - p2y);

		double cosa = cos(angle);
		double sina = sin(angle);

		// Find plan offset

		// p1 rotated = Rot(-angle) * P1
		// p1rx = x*cos(-a) - y*sin(-a) = x*cos(a)+y*sin(a)

		//double p1rx = p1x * cosa + p1y * sina;
		//double p1ry = p1y * cosa - p1x * sina;

		// point rotated
		//double p3rx = point.getX() * cosa + point.getY() * sina;

		//double distance = p3rx - p1rx;
		//double distance = (point.getX() - p1x) * cosa + (point.getY() - p1y) * sina;
		//setPlanOffset(distance);

		planOffset = (point.getX() - p1x) * cosa + (point.getY() - p1y) * sina;

		// Find peak ratio
		//p1ry = x*sin(-a) + y*cos(-a) = y*cos(a)-x*sin(a)
		double p1ry = p1y * cosa - p1x * sina;
		double p2ry = p2y * cosa - p2x * sina;
		double p3ry = point.getY() * cosa - point.getX() * sina;

		if (p3ry < p1ry || p3ry > p2ry)
		{
			double ratio = abs(p3ry - p1ry) / abs(p2ry - p1ry);
			return INVALID_PEAK_RATIO_ERROR_CODE;
		}
		else
		{
			peakRatio = abs(p3ry - p1ry) / abs(p2ry - p1ry);
		}

		return SUCCESS_CODE;
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
			mout << std::endl << "Reading " << filePath << std::endl;
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
					config->m_enableUserInput = getConfigNode<bool>(pt, "General.enableUserInput");
					config->m_xyAxisStep = getConfigNode<unsigned int>(pt, "General.xyAxisStep");
					config->m_zAxisStep = getConfigNode<unsigned int>(pt, "General.zAxisStep");
					config->m_decimalCharIsDot = getConfigNode<bool>(pt, "General.decimalCharIsDot");
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

					config->m_planOffset = getConfigNode<int>(pt, "General.planOffset");
					double peakRatio = getConfigNode<double>(pt, "General.peakRatio");
					if (peakRatio < 0.0 || peakRatio > 1.0)
					{
						throw std::runtime_error("Invalid value for peak ratio - Value " + std::to_string(peakRatio));
					}
					config->m_peakRatio = peakRatio;

					std::string modeStr = getConfigNode<std::string>(pt, "General.mode");
					PreScan::PreScanMode preScanMode;
					if (Tools::stringToPreScanMode(modeStr, preScanMode) != SUCCESS_CODE)
					{
						throw std::runtime_error("Invalid value for Mode - Value " + modeStr);
					}
					config->m_preScanMode = preScanMode;

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
					ReScan::mout << std::endl << "error occured when setting config from file:" << std::endl << e.what() << std::endl;
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

		std::string modeStr;
		Tools::preScanModeToString(config.m_preScanMode, modeStr);

		pt.put("General.enableUserInput", config.m_enableUserInput);
		pt.put("General.xyAxisStep", config.m_xyAxisStep);
		pt.put("General.zAxisStep", config.m_zAxisStep);
		pt.put("General.decimalCharIsDot", config.m_decimalCharIsDot);
		pt.put("General.point1", config.m_point1.toStr("", "", ";"));
		pt.put("General.point2", config.m_point2.toStr("", "", ";"));
		pt.put("General.planOffset", config.m_planOffset);
		pt.put("General.peakRatio", config.m_peakRatio);
		pt.put("General.mode", modeStr);

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
			ReScan::mout << std::endl << "error occured when writing config file:" << std::endl << e.what() << std::endl;
		}
		return result;
	}

	PreScanConfig PreScanConfig::createICNDEConfig()
	{
		PreScanConfig config;
		config.m_enableUserInput = false;
		config.m_exportBasesCartesian = false;
		config.m_exportBasesEulerAngles = false;
		config.m_exportDetailsFile = false;
		config.m_writeHeaders = false;
		return config;
	}
}

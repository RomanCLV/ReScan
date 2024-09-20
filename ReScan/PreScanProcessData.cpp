#include "PreScanProcessData.h"

namespace ReScan::PreScan
{
	PreScanProcessData::PreScanProcessData() :
		m_enableUserInput(true),
		m_stepAxisXY(nullptr),
		m_stepAxisZ(nullptr),
		m_point1(nullptr),
		m_point2(nullptr),
		m_planOffset(nullptr),
		m_peakRatio(nullptr),
		m_preScanMode(nullptr),
		m_distanceXY(0.0),
		m_distanceZ(0.0),
		m_pointsNumberXY(0),
		m_pointsNumberZ(0),
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

	PreScanProcessData::PreScanProcessData(const PreScanProcessData& preScanProcessData) :
		m_enableUserInput(preScanProcessData.m_enableUserInput),
		m_stepAxisXY(nullptr),
		m_stepAxisZ(nullptr),
		m_point1(nullptr),
		m_point2(nullptr),
		m_planOffset(nullptr),
		m_peakRatio(nullptr),
		m_preScanMode(nullptr),
		m_distanceXY(preScanProcessData.m_distanceXY),
		m_distanceZ(preScanProcessData.m_distanceZ),
		m_pointsNumberXY(preScanProcessData.m_pointsNumberXY),
		m_pointsNumberZ(preScanProcessData.m_pointsNumberZ),
		m_exportBasesCartesian(preScanProcessData.m_exportBasesCartesian),
		m_exportBasesEulerAngles(preScanProcessData.m_exportBasesEulerAngles),
		m_exportDetailsFile(preScanProcessData.m_exportDetailsFile),
		m_writeHeaders(preScanProcessData.m_writeHeaders),
		m_decimalCharIsDot(preScanProcessData.m_decimalCharIsDot),
		m_basesCartesianDefaultFileName(preScanProcessData.m_basesCartesianDefaultFileName),
		m_basesEulerAnglesDefaultFileName(preScanProcessData.m_basesEulerAnglesDefaultFileName),
		m_detailsDefaultFileName(preScanProcessData.m_detailsDefaultFileName)
	{
		if (preScanProcessData.m_point1)
		{
			m_point1 = new Point3D(*preScanProcessData.m_point1);
		}
		if (preScanProcessData.m_point2)
		{
			m_point2 = new Point3D(*preScanProcessData.m_point2);
		}
		if (preScanProcessData.m_stepAxisXY)
		{
			m_stepAxisXY = new unsigned int(*preScanProcessData.m_stepAxisXY);
		}
		if (preScanProcessData.m_stepAxisZ)
		{
			m_stepAxisZ = new unsigned int(*preScanProcessData.m_stepAxisZ);
		}
		if (preScanProcessData.m_planOffset)
		{
			m_planOffset = new double(*preScanProcessData.m_planOffset);
		}
		if (preScanProcessData.m_peakRatio)
		{
			m_peakRatio = new double(*preScanProcessData.m_peakRatio);
		}
		if (preScanProcessData.m_preScanMode)
		{
			m_preScanMode = new PreScanMode(*preScanProcessData.m_preScanMode);
		}
	}

	PreScanProcessData::~PreScanProcessData()
	{
		reset();
	}

#pragma region Setters

	void PreScanProcessData::setFromConfig(const PreScanConfig& config)
	{
		reset();
		m_enableUserInput = config.getEnableUserInput();
		setStepAxisXY(config.getStepAxisXY());
		setStepAxisZ(config.getStepAxisZ());
		setPoint1(config.getPoint1());
		setPoint2(config.getPoint2());
		setPlanOffset(config.getPlanOffset());
		setPeakRatio(config.getPeakRatio());
		setPreScanMode(config.getPreScanMode());
		setExportBasesCartesian(config.getExportBasesCartesian());
		setExportBasesEulerAngles(config.getExportBasesEulerAngles());
		setExportDetailsFile(config.getExportDetailsFile());
		setWriteHeaders(config.getWriteHeaders());
		setDecimalCharIsDot(config.getDecimalCharIsDot());
		setBasesCartesianDefaultFileName(config.getBasesCartesianDefaultFileName());
		setBasesEulerAnglesDefaultFileName(config.getBasesEulerAnglesDefaultFileName());
		setDetailsDefaultFileName(config.getDetailsDefaultFileName());
	}

	void PreScanProcessData::reset()
	{
		m_enableUserInput = true;
		resetPoint1();
		resetPoint2();
		resetStepAxisXY();
		resetStepAxisZ();
		resetPlanOffset();
		resetPeakRatio();
		resetPreScanMode();
		m_distanceXY = 0.0;
		m_distanceZ = 0.0;
		m_pointsNumberXY = 0;
		m_pointsNumberZ = 0;
		m_exportBasesCartesian = true;
		m_exportBasesEulerAngles = true;
		m_exportDetailsFile = true;
		m_writeHeaders = true;
		m_decimalCharIsDot = true;
		m_basesCartesianDefaultFileName = "";
		m_basesEulerAnglesDefaultFileName = "";
		m_detailsDefaultFileName = "";
	}

	void PreScanProcessData::resetPoint1()
	{
		if (m_point1)
		{
			delete m_point1;
			m_point1 = nullptr;
		}
	}

	void PreScanProcessData::setPoint1(const Point3D* point)
	{
		if (!m_point1)
		{
			m_point1 = new Point3D();
		}
		*m_point1 = *point;
	}

	void PreScanProcessData::resetPoint2()
	{
		if (m_point2)
		{
			delete m_point2;
			m_point2 = nullptr;
		}
	}

	void PreScanProcessData::setPoint2(const Point3D* point)
	{
		if (!m_point2)
		{
			m_point2 = new Point3D();
		}
		*m_point2 = *point;
	}

	void PreScanProcessData::resetStepAxisXY()
	{
		if (m_stepAxisXY)
		{
			delete m_stepAxisXY;
			m_stepAxisXY = nullptr;
		}
	}

	void PreScanProcessData::setStepAxisXY(const unsigned int value)
	{
		if (!m_stepAxisXY)
		{
			m_stepAxisXY = new unsigned int(0);
		}
		*m_stepAxisXY = value;
	}

	void PreScanProcessData::resetStepAxisZ()
	{
		if (m_stepAxisZ)
		{
			delete m_stepAxisZ;
			m_stepAxisZ = nullptr;
		}
	}

	void PreScanProcessData::setStepAxisZ(const unsigned int value)
	{
		if (!m_stepAxisZ)
		{
			m_stepAxisZ = new unsigned int(0);
		}
		*m_stepAxisZ = value;
	}


	void PreScanProcessData::resetPlanOffset()
	{
		if (m_planOffset)
		{
			delete m_planOffset;
			m_planOffset = nullptr;
		}
	}

	void PreScanProcessData::setPlanOffset(const double value)
	{
		if (!m_planOffset)
		{
			m_planOffset = new double(0.);
		}
		*m_planOffset = value;
	}

	void PreScanProcessData::resetPeakRatio()
	{
		if (m_peakRatio)
		{
			delete m_peakRatio;
			m_peakRatio = nullptr;
		}
	}

	void PreScanProcessData::setPeakRatio(const double value)
	{
		if (!m_peakRatio)
		{
			m_peakRatio = new double(0.);
		}
		*m_peakRatio = (value < 0.0 || value > 1.0) ? 0.5 : value;
	}

	void PreScanProcessData::resetPreScanMode()
	{
		if (m_preScanMode)
		{
			delete m_preScanMode;
			m_preScanMode = nullptr;
		}
	}

	void PreScanProcessData::setPreScanMode(const PreScanMode value)
	{
		if (!m_preScanMode)
		{
			m_preScanMode = new PreScanMode();
		}
		*m_preScanMode = value;
	}

	void PreScanProcessData::setDistanceXY(const double value)
	{
		m_distanceXY = value;
	}

	void PreScanProcessData::setDistanceZ(const double value)
	{
		m_distanceZ = value;
	}

	void PreScanProcessData::setPointsNumberXY(const unsigned int value)
	{
		m_pointsNumberXY = value;
	}

	void PreScanProcessData::setPointsNumberZ(const unsigned int value)
	{
		m_pointsNumberZ = value;
	}

	void PreScanProcessData::setExportBasesCartesian(const bool value)
	{
		m_exportBasesCartesian = value;
	}

	void PreScanProcessData::setExportBasesEulerAngles(const bool value)
	{
		m_exportBasesEulerAngles = value;
	}

	void PreScanProcessData::setExportDetailsFile(const bool value)
	{
		m_exportDetailsFile = value;
	}

	void PreScanProcessData::setWriteHeaders(const bool value)
	{
		m_writeHeaders = value;
	}

	void PreScanProcessData::setDecimalCharIsDot(const bool value)
	{
		m_decimalCharIsDot = value;
	}

	void PreScanProcessData::setBasesCartesianDefaultFileName(const std::string& basesCartesianDefaultFileName)
	{
		m_basesCartesianDefaultFileName = basesCartesianDefaultFileName;
	}

	void PreScanProcessData::setBasesEulerAnglesDefaultFileName(const std::string& basesEulerAnglesDefaultFileName)
	{
		m_basesEulerAnglesDefaultFileName = basesEulerAnglesDefaultFileName;
	}

	void PreScanProcessData::setDetailsDefaultFileName(const std::string& detailsDefaultFileName)
	{
		m_detailsDefaultFileName = detailsDefaultFileName;
	}


#pragma endregion

#pragma region Getters

	bool PreScanProcessData::getEnableUserInput() const
	{
		return m_enableUserInput;
	}

	const Point3D* PreScanProcessData::getPoint1() const
	{
		return m_point1;
	}

	const Point3D* PreScanProcessData::getPoint2() const
	{
		return m_point2;
	}

	const unsigned int* PreScanProcessData::getStepAxisXY() const
	{
		return m_stepAxisXY;
	}

	const unsigned int* PreScanProcessData::getStepAxisZ() const
	{
		return m_stepAxisZ;
	}

	const double* PreScanProcessData::getPlanOffset() const
	{
		return m_planOffset;
	}

	const double* PreScanProcessData::getPeakRatio() const
	{
		return m_peakRatio;
	}

	const PreScanMode* PreScanProcessData::getPreScanMode() const
	{
		return m_preScanMode;
	}

	double PreScanProcessData::getDistanceXY() const
	{
		return m_distanceXY;
	}

	double PreScanProcessData::getDistanceZ() const
	{
		return m_distanceZ;
	}

	unsigned int PreScanProcessData::getPointsNumberXY() const
	{
		return m_pointsNumberXY;
	}

	unsigned int PreScanProcessData::getPointsNumberZ() const
	{
		return m_pointsNumberZ;
	}

	unsigned int PreScanProcessData::getTotalPointsNumber() const
	{
		return m_pointsNumberXY * m_pointsNumberZ;
	}

	bool PreScanProcessData::getExportBasesCartesian() const
	{
		return m_exportBasesCartesian;
	}

	bool PreScanProcessData::getExportBasesEulerAngles() const
	{
		return m_exportBasesEulerAngles;
	}

	bool PreScanProcessData::getExportDetailsFile() const
	{
		return m_exportDetailsFile;
	}

	bool PreScanProcessData::getWriteHeaders() const
	{
		return m_writeHeaders;
	}

	bool PreScanProcessData::getDecimalCharIsDot() const
	{
		return m_decimalCharIsDot;
	}

	std::string PreScanProcessData::getBasesCartesianDefaultFileName() const
	{
		return m_basesCartesianDefaultFileName;
	}

	std::string PreScanProcessData::getBasesEulerAnglesDefaultFileName() const
	{
		return m_basesEulerAnglesDefaultFileName;
	}

	std::string PreScanProcessData::getDetailsDefaultFileName() const
	{
		return m_detailsDefaultFileName;
	}


#pragma endregion

	int PreScanProcessData::findPlanOffsetAndPeakRatio(const Point3D& point1, const Point3D& point2, const Point3D& point3, const PreScan::PreScanMode& mode, double& planOffset, double& peakRatio)
	{
		int code = SUCCESS_CODE;
		planOffset = 0.0;
		peakRatio = 0.0;

		double p1x = point1.getX();
		double p1y = point1.getY();
		double p2x = point2.getX();
		double p2y = point2.getY();

		if (p1x == p2x && p1y == p2y)
		{
			code = IDENTICAL_POINTS_XY_ERROR_CODE;
		}
		else
		{
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

			if (p1x * cosa + p1y * sina < 0)
			{
				angle += EIGEN_PI;
				cosa = cos(angle);
				sina = sin(angle);
			}

			planOffset = (point3.getX() - p1x) * cosa + (point3.getY() - p1y) * sina;

			// Find peak ratio
			if (mode == PreScanMode::Horizontal)
			{
				//p1ry = x*sin(-a) + y*cos(-a) = y*cos(a)-x*sin(a)
				double p1ry = p1y * cosa - p1x * sina;
				double p2ry = p2y * cosa - p2x * sina;
				double p3ry = point3.getY() * cosa - point3.getX() * sina;

				peakRatio = abs(p3ry - p1ry) / abs(p2ry - p1ry);
				if (p3ry < p1ry || p3ry > p2ry)
				{
					code = INVALID_PEAK_RATIO_ERROR_CODE;
				}
			}
			else if (mode == PreScanMode::Vertical)
			{
				double p1z = (point1.getZ() <= point2.getZ()) ? point1.getZ() : point2.getZ();
				double p2z = (point1.getZ() <= point2.getZ()) ? point2.getZ() : point1.getZ();
				if (p1z == p2z)
				{
					code = IDENTICAL_POINTS_Z_ERROR_CODE;
				}
				else
				{
					double p3z = point3.getZ();
					peakRatio = (p3z - p1z) / (p2z - p1z);
					if (p3z < p1z || p3z > p2z)
					{
						code = INVALID_PEAK_RATIO_ERROR_CODE;
					}
				}
			}
			else // PreScanMode::Default
			{
				peakRatio = 0.5;
			}
		}

		return code;
	}

	bool PreScanProcessData::isStepXYValid(unsigned int min) const
	{
		return min > m_distanceXY ? false : ((min <= *m_stepAxisXY) && (*m_stepAxisXY <= m_distanceXY));
	}

	bool PreScanProcessData::isStepZValid(unsigned int min) const
	{
		return min > m_distanceZ ? false : ((min <= *m_stepAxisZ) && (*m_stepAxisZ <= m_distanceZ));
	}
}

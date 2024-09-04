#include "PreScanProcessData.h"

namespace ReScan::PreScan
{
	PreScanProcessData::PreScanProcessData() :
		m_enableUserInput(true),
		m_stepAxis1(nullptr),
		m_stepAxis2(nullptr),
		m_point1(nullptr),
		m_point2(nullptr),
		m_planOffset(nullptr),
		m_distance1(0.0),
		m_distance2(0.0),
		m_subDivision1(0),
		m_subDivision2(0),
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
		m_stepAxis1(nullptr),
		m_stepAxis2(nullptr),
		m_point1(nullptr),
		m_point2(nullptr),
		m_planOffset(nullptr),
		m_distance1(preScanProcessData.m_distance1),
		m_distance2(preScanProcessData.m_distance2),
		m_subDivision1(preScanProcessData.m_subDivision1),
		m_subDivision2(preScanProcessData.m_subDivision2),
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
		if (preScanProcessData.m_stepAxis1)
		{
			m_stepAxis1 = new unsigned int(*preScanProcessData.m_stepAxis1);
		}
		if (preScanProcessData.m_stepAxis2)
		{
			m_stepAxis2 = new unsigned int(*preScanProcessData.m_stepAxis2);
		}
		if (preScanProcessData.m_planOffset)
		{
			m_planOffset = new int(*preScanProcessData.m_planOffset);
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
		setStepAxis1(config.getStepAxis1());
		setStepAxis2(config.getStepAxis2());
		setPoint1(config.getPoint1());
		setPoint2(config.getPoint2());
		setPlanOffset(config.getPlanOffset());
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
		resetStepAxis1();
		resetStepAxis2();
		resetPlanOffset();
		m_distance1 = 0.0;
		m_distance2 = 0.0;
		m_subDivision1 = 0;
		m_subDivision2 = 0;
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

	void PreScanProcessData::resetStepAxis1()
	{
		if (m_stepAxis1)
		{
			delete m_stepAxis1;
			m_stepAxis1 = nullptr;
		}
	}

	void PreScanProcessData::setStepAxis1(const unsigned int value)
	{
		if (!m_stepAxis1)
		{
			m_stepAxis1 = new unsigned int(0);
		}
		*m_stepAxis1 = value;
	}

	void PreScanProcessData::resetStepAxis2()
	{
		if (m_stepAxis2)
		{
			delete m_stepAxis2;
			m_stepAxis2 = nullptr;
		}
	}

	void PreScanProcessData::setStepAxis2(const unsigned int value)
	{
		if (!m_stepAxis2)
		{
			m_stepAxis2 = new unsigned int(0);
		}
		*m_stepAxis2 = value;
	}


	void PreScanProcessData::resetPlanOffset()
	{
		if (m_planOffset)
		{
			delete m_planOffset;
			m_planOffset = nullptr;
		}
	}

	void PreScanProcessData::setPlanOffset(const int value)
	{
		if (!m_planOffset)
		{
			m_planOffset = new int(0);
		}
		*m_planOffset = value;
	}

	void PreScanProcessData::setDistance1(const double value)
	{
		m_distance1 = value;
	}

	void PreScanProcessData::setDistance2(const double value)
	{
		m_distance2 = value;
	}

	void PreScanProcessData::setSubDivision1(const unsigned int value)
	{
		m_subDivision1 = value;
	}

	void PreScanProcessData::setSubDivision2(const unsigned int value)
	{
		m_subDivision2 = value;
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

	const unsigned int* PreScanProcessData::getStepAxis1() const
	{
		return m_stepAxis1;
	}

	const unsigned int* PreScanProcessData::getStepAxis2() const
	{
		return m_stepAxis2;
	}

	const int* PreScanProcessData::getPlanOffset() const
	{
		return m_planOffset;
	}

	double PreScanProcessData::getDistance1() const
	{
		return m_distance1;
	}

	double PreScanProcessData::getDistance2() const
	{
		return m_distance2;
	}

	unsigned int PreScanProcessData::getSubDivisions1() const
	{
		return m_subDivision1;
	}

	unsigned int PreScanProcessData::getSubDivisions2() const
	{
		return m_subDivision2;
	}

	unsigned int PreScanProcessData::getTotalSubDivisions() const
	{
		return m_subDivision1 * m_subDivision2;
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

	bool PreScanProcessData::isStep1Valid(unsigned int min) const
	{
		return min > m_distance1 ? false : ((min <= *m_stepAxis1) && (*m_stepAxis1 <= m_distance1));
	}

	bool PreScanProcessData::isStep2Valid(unsigned int min) const
	{
		return min > m_distance2 ? false : ((min <= *m_stepAxis2) && (*m_stepAxis2 <= m_distance2));
	}
}

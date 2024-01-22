#include "ReScanProcessData.h"

namespace ReScan
{
	ReScanProcessData::ReScanProcessData() :
		m_enableUserInput(true),
		m_plan2D(nullptr),
		m_stepAxis1(nullptr),
		m_stepAxis2(nullptr),
		m_referenceBase(),
		m_axis1Name('?'),
		m_axis2Name('?'),
		m_distance1(0.0),
		m_distance2(0.0),
		m_subDivision1(0),
		m_subDivision2(0),
		m_objFile(""),
		m_exportSubDivisions(false),
		m_exportBasesCartesian(true),
		m_exportBasesEulerAngles(true),
		m_exportDetailsFile(true),
		m_writeHeaders(true),
		m_decimalCharIsDot(true)
	{
	}

	ReScanProcessData::ReScanProcessData(const ReScanProcessData& reScanProcessData) :
		m_enableUserInput(reScanProcessData.m_enableUserInput),
		m_plan2D(nullptr),
		m_stepAxis1(nullptr),
		m_stepAxis2(nullptr),
		m_referenceBase(reScanProcessData.m_referenceBase),
		m_axis1Name(reScanProcessData.m_axis1Name),
		m_axis2Name(reScanProcessData.m_axis2Name),
		m_distance1(reScanProcessData.m_distance1),
		m_distance2(reScanProcessData.m_distance2),
		m_subDivision1(reScanProcessData.m_subDivision1),
		m_subDivision2(reScanProcessData.m_subDivision2),
		m_objFile(reScanProcessData.m_objFile),
		m_exportSubDivisions(reScanProcessData.m_exportSubDivisions),
		m_exportBasesCartesian(reScanProcessData.m_exportBasesCartesian),
		m_exportBasesEulerAngles(reScanProcessData.m_exportBasesEulerAngles),
		m_exportDetailsFile(reScanProcessData.m_exportDetailsFile),
		m_writeHeaders(reScanProcessData.m_writeHeaders),
		m_decimalCharIsDot(reScanProcessData.m_decimalCharIsDot)
	{
		if (reScanProcessData.m_plan2D)
		{
			m_plan2D = new Plan2D(*reScanProcessData.m_plan2D);
		}
		if (reScanProcessData.m_stepAxis1)
		{
			m_stepAxis1 = new unsigned int(*reScanProcessData.m_stepAxis1);
		}
		if (reScanProcessData.m_stepAxis2)
		{
			m_stepAxis2 = new unsigned int(*reScanProcessData.m_stepAxis2);
		}
	}

	ReScanProcessData::~ReScanProcessData()
	{
		reset();
	}

#pragma region Setters

	void ReScanProcessData::setFromConfig(const ReScanConfig& config) 
	{
		reset();
		m_enableUserInput = config.getEnableUserInput();
		setObjFile(config.getObjFile());
		setPlan2D(config.getPlan2D());
		setStepAxis1(config.getStepAxis1());
		setStepAxis2(config.getStepAxis2());
		setReferenceBase(config.getReferenceBase());
		setExportSubDivisions(config.getExportSubDivisions());
		setExportBasesCartesian(config.getExportBasesCartesian());
		setExportBasesEulerAngles(config.getExportBasesEulerAngles());
		setExportDetailsFile(config.getExportDetailsFile());
		setWriteHeaders(config.getWriteHeaders());
		setDecimalCharIsDot(config.getDecimalCharIsDot());
	}

	void ReScanProcessData::reset()
	{
		m_enableUserInput = true;
		resetPlan2D();
		resetStepAxis1();
		resetStepAxis2();
		resetReferenceBase();
		m_distance1 = 0.0;
		m_distance2 = 0.0;
		m_axis1Name = '?';
		m_axis2Name = '?';
		m_distance1 = 0.0;
		m_distance2 = 0.0;
		m_subDivision1 =0;
		m_subDivision2 =0;
		m_objFile = "";
		m_exportSubDivisions = false;
		m_exportBasesCartesian = true;
		m_exportBasesEulerAngles = true;
		m_exportDetailsFile = true;
		m_writeHeaders = true;
		m_decimalCharIsDot = true;
	}

	void ReScanProcessData::resetPlan2D()
	{
		if (m_plan2D)
		{
			delete m_plan2D;
			m_plan2D = nullptr;
		}
	}

	void ReScanProcessData::setPlan2D(Plan2D plan2D)
	{
		if (!m_plan2D)
		{
			m_plan2D = new Plan2D();
		}
		*m_plan2D = plan2D;
	}

	void ReScanProcessData::resetStepAxis1()
	{
		if (m_stepAxis1)
		{
			delete m_stepAxis1;
			m_stepAxis1 = nullptr;
		}
	}

	void ReScanProcessData::setStepAxis1(const unsigned int value)
	{
		if (!m_stepAxis1)
		{
			m_stepAxis1 = new unsigned int(0);
		}
		*m_stepAxis1 = value;
	}

	void ReScanProcessData::resetStepAxis2()
	{
		if (m_stepAxis2)
		{
			delete m_stepAxis2;
			m_stepAxis2 = nullptr;
		}
	}

	void ReScanProcessData::setStepAxis2(const unsigned int value)
	{
		if (!m_stepAxis2)
		{
			m_stepAxis2 = new unsigned int(0);
		}
		*m_stepAxis2 = value;
	}

	void ReScanProcessData::resetReferenceBase()
	{
		m_referenceBase.reset();
	}

	void ReScanProcessData::setReferenceBase(const Base3D* base)
	{
		m_referenceBase.setFrom(*base, true);
	}

	void ReScanProcessData::setAxis1Name(const char c)
	{
		m_axis1Name = c;
	}

	void ReScanProcessData::setAxis2Name(const char c)
	{
		m_axis2Name = c;
	}

	void ReScanProcessData::setDistance1(const double value)
	{
		m_distance1 = value;
	}

	void ReScanProcessData::setDistance2(const double value)
	{
		m_distance2 = value;
	}

	void ReScanProcessData::setSubDivision1(const unsigned int value)
	{
		m_subDivision1 = value;
	}

	void ReScanProcessData::setSubDivision2(const unsigned int value)
	{
		m_subDivision2 = value;
	}

	void ReScanProcessData::setObjFile(const std::string& filename)
	{
		m_objFile = filename;
	}

	void ReScanProcessData::setExportSubDivisions(const bool value)
	{
		m_exportSubDivisions = value;
	}

	void ReScanProcessData::setExportBasesCartesian(const bool value)
	{
		m_exportBasesCartesian = value;
	}

	void ReScanProcessData::setExportBasesEulerAngles(const bool value)
	{
		m_exportBasesEulerAngles = value;
	}

	void ReScanProcessData::setExportDetailsFile(const bool value)
	{
		m_exportDetailsFile = value;
	}

	void ReScanProcessData::setWriteHeaders(const bool value)
	{
		m_writeHeaders = value;
	}

	void ReScanProcessData::setDecimalCharIsDot(const bool value)
	{
		m_decimalCharIsDot = value;
	}

#pragma endregion

#pragma region Getters

	bool ReScanProcessData::getEnableUserInput() const
	{
		return m_enableUserInput;
	}

	const Plan2D* ReScanProcessData::getPlan2D() const
	{
		return m_plan2D;
	}

	const unsigned int* ReScanProcessData::getStepAxis1() const
	{
		return m_stepAxis1;
	}

	const unsigned int* ReScanProcessData::getStepAxis2() const
	{
		return m_stepAxis2;
	}

	const Base3D* ReScanProcessData::getReferenceBase() const
	{
		return &m_referenceBase;
	}

	char ReScanProcessData::getAxis1Name() const
	{
		return m_axis1Name;
	}

	char ReScanProcessData::getAxis2Name() const
	{
		return m_axis2Name;
	}

	double ReScanProcessData::getDistance1() const
	{
		return m_distance1;
	}

	double ReScanProcessData::getDistance2() const
	{
		return m_distance2;
	}

	unsigned int ReScanProcessData::getSubDivisions1() const
	{
		return m_subDivision1;
	}

	unsigned int ReScanProcessData::getSubDivisions2() const
	{
		return m_subDivision2;
	}

	unsigned int ReScanProcessData::getTotalSubDivisions() const
	{
		return m_subDivision1 * m_subDivision2;
	}

	std::string ReScanProcessData::getObjFile() const
	{
		return m_objFile;
	}

	bool ReScanProcessData::getExportSubDivisions() const
	{
		return m_exportSubDivisions;
	}

	bool ReScanProcessData::getExportBasesCartesian() const
	{
		return m_exportBasesCartesian;
	}

	bool ReScanProcessData::getExportBasesEulerAngles() const
	{
		return m_exportBasesEulerAngles;
	}

	bool ReScanProcessData::getExportDetailsFile() const
	{
		return m_exportDetailsFile;
	}

	bool ReScanProcessData::getWriteHeaders() const
	{
		return m_writeHeaders;
	}

	bool ReScanProcessData::getDecimalCharIsDot() const
	{
		return m_decimalCharIsDot;
	}

#pragma endregion

	bool ReScanProcessData::isStep1Valid(unsigned int min) const
	{
		return min > m_distance1 ? false : ((min <= *m_stepAxis1) && (*m_stepAxis1 <= m_distance1));
	}

	bool ReScanProcessData::isStep2Valid(unsigned int min) const
	{
		return min > m_distance2 ? false : ((min <= *m_stepAxis2) && (*m_stepAxis2 <= m_distance2));
	}
}
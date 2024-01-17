#include "ReScanProcessData.h"

namespace ReScan
{
	ReScanProcessData::ReScanProcessData() :
		m_plan2D(nullptr),
		m_stepAxis1(nullptr),
		m_stepAxis2(nullptr),
		m_distance1(0.0),
		m_distance2(0.0)
	{
	}

	ReScanProcessData::ReScanProcessData(const ReScanProcessData& reScanProcessData) :
		m_plan2D(nullptr),
		m_stepAxis1(nullptr),
		m_stepAxis2(nullptr),
		m_distance1(reScanProcessData.m_distance1),
		m_distance2(reScanProcessData.m_distance2)
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

	void ReScanProcessData::reset()
	{
		resetPlan2D();
		resetStepAxis1();
		resetStepAxis2();
		m_distance1 = 0.0;
		m_distance2 = 0.0;
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

#pragma endregion

#pragma region Getters

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

	double ReScanProcessData::getDistance1() const
	{
		return m_distance1;
	}

	double ReScanProcessData::getDistance2() const
	{
		return m_distance2;
	}

#pragma endregion
}
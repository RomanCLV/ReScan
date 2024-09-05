#include "ReScanResultDetails.h"

namespace ReScan
{
	ReScanResultDetails::ReScanResultDetails() :
		m_xDistance(0.),
		m_yDistance(0.),
		m_xStep(0),
		m_yStep(0),
		m_xDivisions(0),
		m_yDivisions(0),
		m_totalDivisions(0)
	{
	}

	ReScanResultDetails::ReScanResultDetails(const ReScanResultDetails& reScanResultDetails) :
		m_xDistance(reScanResultDetails.m_xDistance),
		m_yDistance(reScanResultDetails.m_yDistance),
		m_xStep(reScanResultDetails.m_xStep),
		m_yStep(reScanResultDetails.m_yStep),
		m_xDivisions(reScanResultDetails.m_xDivisions),
		m_yDivisions(reScanResultDetails.m_yDivisions),
		m_totalDivisions(reScanResultDetails.m_totalDivisions)
	{
	}

	ReScanResultDetails::ReScanResultDetails(const ReScanProcessData& data) :
		m_xDistance(0.),
		m_yDistance(0.),
		m_xStep(0),
		m_yStep(0),
		m_xDivisions(0),
		m_yDivisions(0),
		m_totalDivisions(0)
	{
		setFromProcessData(data);
	}

	ReScanResultDetails::~ReScanResultDetails()
	{
	}

	void ReScanResultDetails::setFromProcessData(const ReScanProcessData& data)
	{
		m_xDistance = data.getDistance1();
		m_yDistance = data.getDistance2();
		m_xStep = *data.getStepAxis1();
		m_yStep = *data.getStepAxis2();
		m_xDivisions = data.getSubDivisions1();
		m_yDivisions = data.getSubDivisions2();
		m_totalDivisions = data.getTotalSubDivisions();
	}

	double ReScanResultDetails::getXDistance() const
	{
		return m_xDistance;
	}

	double ReScanResultDetails::getYDistance() const
	{
		return m_yDistance;
	}

	unsigned int ReScanResultDetails::getXStep() const
	{
		return m_xStep;
	}

	unsigned int ReScanResultDetails::getYStep() const
	{
		return m_yStep;
	}

	unsigned int ReScanResultDetails::getXDivisions() const
	{
		return m_xDivisions;
	}

	unsigned int ReScanResultDetails::getYDivisions() const
	{
		return m_yDivisions;
	}

	unsigned int ReScanResultDetails::getTotalDivisions() const
	{
		return m_totalDivisions;
	}
}

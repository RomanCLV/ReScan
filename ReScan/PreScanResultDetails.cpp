#include "PreScanResultDetails.h"

namespace ReScan::PreScan
{
	PreScanResultDetails::PreScanResultDetails() :
		m_xyDistance(0.),
		m_zDistance(0.),
		m_xyStep(0),
		m_zStep(0),
		m_xyPointsNumber(0),
		m_zPointsNumber(0),
		m_totalPointsNumber(0)
	{
	}

	PreScanResultDetails::PreScanResultDetails(const PreScanResultDetails& reScanResultDetails) :
		m_xyDistance(reScanResultDetails.m_xyDistance),
		m_zDistance(reScanResultDetails.m_zDistance),
		m_xyStep(reScanResultDetails.m_xyStep),
		m_zStep(reScanResultDetails.m_zStep),
		m_xyPointsNumber(reScanResultDetails.m_xyPointsNumber),
		m_zPointsNumber(reScanResultDetails.m_zPointsNumber),
		m_totalPointsNumber(reScanResultDetails.m_totalPointsNumber)
	{
	}

	PreScanResultDetails::PreScanResultDetails(const PreScanProcessData& data) :
		m_xyDistance(0.),
		m_zDistance(0.),
		m_xyStep(0),
		m_zStep(0),
		m_xyPointsNumber(0),
		m_zPointsNumber(0),
		m_totalPointsNumber(0)
	{
		setFromProcessData(data);
	}

	PreScanResultDetails::~PreScanResultDetails()
	{
	}

	void PreScanResultDetails::setFromProcessData(const PreScanProcessData& data)
	{
		m_xyDistance = data.getDistanceXY();
		m_zDistance = data.getDistanceZ();
		m_xyStep = *data.getStepAxisXY();
		m_zStep = *data.getStepAxisZ();
		m_xyPointsNumber = data.getPointsNumberXY();
		m_zPointsNumber = data.getPointsNumberZ();
		m_totalPointsNumber = data.getTotalPointsNumber();
	}

	double PreScanResultDetails::getXYDistance() const
	{
		return m_xyDistance;
	}

	double PreScanResultDetails::getZDistance() const
	{
		return m_zDistance;
	}

	unsigned int PreScanResultDetails::getXYStep() const
	{
		return m_xyStep;
	}

	unsigned int PreScanResultDetails::getZStep() const
	{
		return m_zStep;
	}

	unsigned int PreScanResultDetails::getXYPointsNumber() const
	{
		return m_xyPointsNumber;
	}

	unsigned int PreScanResultDetails::getZPointsNumber() const
	{
		return m_zPointsNumber;
	}

	unsigned int PreScanResultDetails::getTotalPointsNumber() const
	{
		return m_totalPointsNumber;
	}
}

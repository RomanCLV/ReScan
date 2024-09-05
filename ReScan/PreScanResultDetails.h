#ifndef RESCAN_PRESCAN_PRESCANRESULTDETAILS_H
#define RESCAN_PRESCAN_PRESCANRESULTDETAILS_H

#include "PreScanProcessData.h"

namespace ReScan::PreScan
{
	class PreScanResultDetails
	{
	private:
		double m_xyDistance;
		double m_zDistance;
		unsigned int m_xyStep;
		unsigned int m_zStep;
		unsigned int m_xyPointsNumber;
		unsigned int m_zPointsNumber;
		unsigned int m_totalPointsNumber;

	public:
		PreScanResultDetails();
		PreScanResultDetails(const PreScanResultDetails& preScanResultDetails);
		PreScanResultDetails(const PreScanProcessData& data);

		~PreScanResultDetails();

		void setFromProcessData(const PreScanProcessData& data);

		double       getXYDistance() const;
		double       getZDistance() const;
		unsigned int getXYStep() const;
		unsigned int getZStep() const;
		unsigned int getXYPointsNumber() const;
		unsigned int getZPointsNumber() const;
		unsigned int getTotalPointsNumber() const;
	};
}

#endif // !RESCAN_PRESCAN_PRESCANRESULTDETAILS_H

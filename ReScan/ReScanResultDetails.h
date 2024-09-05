#ifndef RESCAN_RESCANRESULTDETAILS_H
#define RESCAN_RESCANRESULTDETAILS_H

#include "ReScanProcessData.h"

namespace ReScan
{
	class ReScanResultDetails
	{
	private:
		double m_xDistance;
		double m_yDistance;
		unsigned int m_xStep;
		unsigned int m_yStep;
		unsigned int m_xDivisions;
		unsigned int m_yDivisions;
		unsigned int m_totalDivisions;

	public:
		ReScanResultDetails();
		ReScanResultDetails(const ReScanResultDetails & reScanResultDetails);
		ReScanResultDetails(const ReScanProcessData& data);

		~ReScanResultDetails();

		void setFromProcessData(const ReScanProcessData& data);

		double       getXDistance() const;
		double       getYDistance() const;
		unsigned int getXStep() const;
		unsigned int getYStep() const;
		unsigned int getXDivisions() const;
		unsigned int getYDivisions() const;
		unsigned int getTotalDivisions() const;
	};
}
#endif // RESCAN_RESCANRESULTDETAILS_H

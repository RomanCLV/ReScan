#ifndef RESCAN_TOOLS_H
#define RESCAN_TOOLS_H

#include <Eigen/Dense>

namespace ReScan
{
	namespace Tools
	{
		double clamp(double d);
		double mixtProduct(const Eigen::Vector3d& u, const Eigen::Vector3d& v, const Eigen::Vector3d& w);
		bool areVectorsColinear(const Eigen::Vector3d& u, const Eigen::Vector3d& v);
	}
}

#endif // RESCAN_TOOLS_H
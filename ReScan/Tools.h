#ifndef TOOLS_H
#define TOOLS_H

#include <Eigen/Dense>

namespace ReScan
{
	static class Tools
	{
	public:
		inline static double Clamp(double d);
		inline static double mixtProduct(const Eigen::Vector3d& u, const Eigen::Vector3d& v, const Eigen::Vector3d& w);
		inline static bool areVectorsColinear(const Eigen::Vector3d& u, const Eigen::Vector3d& v);
	};
}

#endif
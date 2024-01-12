#include "tools.h"
#include "macros.h"

#include <cmath>

namespace ReScan
{
	namespace Tools
	{
		double clamp(double d)
		{
			return -ZERO_CLAMP < d && d < ZERO_CLAMP ? 0.0 : d;
		}

		double mixtProduct(const Eigen::Vector3d& u, const Eigen::Vector3d& v, const Eigen::Vector3d& w)
		{
			return u.dot(v.cross(w));
		}

		bool areVectorsColinear(const Eigen::Vector3d& u, const Eigen::Vector3d& v)
		{
			if (u.norm() < ZERO_CLAMP || v.norm() < ZERO_CLAMP)
			{
				return true;
			}
			double k;
			if (v.x() != 0.0)
			{
				k = u.x() / v.x();
			}
			else if (v.y() != 0.0)
			{
				k = u.y() / v.y();
			}
			else
			{
				k = u.z() / v.z();
			}

			double d = u.x() - k * v.x();
			bool areColinear = clamp(d) == 0.0;
			if (areColinear)
			{
				d = u.y() - k * v.y();
				areColinear = clamp(d) == 0.0;
				if (areColinear)
				{
					d = u.z() - k * v.z();
					areColinear = clamp(d) == 0.0;
				}
			}
			return areColinear;
		}
	}
}
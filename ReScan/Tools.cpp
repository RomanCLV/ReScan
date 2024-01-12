#include "tools.h"
#include "macros.h"

#include <cmath>

namespace ReScan
{
	namespace Tools
	{
		void strReplace(std::string& s, char oldChar, char newChar)
		{
			for (int i = 0; i < s.length(); i++)
			{
				if (s[i] == oldChar)
				{
					s[i] = newChar;
				}
			}
		}

		double d2r(double degree)
		{
			return EIGEN_PI * degree / 180.0;
		}

		double r2d(double radian)
		{
			return 180.0 * radian / EIGEN_PI;
		}

		double clamp(double d)
		{
			return (-ZERO_CLAMP < d && d < ZERO_CLAMP) ? 0.0 : d;
		}

		double clampv(double d, double v)
		{
			return (v - ZERO_CLAMP < d && d < v + ZERO_CLAMP) ? v : d;
		}

		double clampv3(double d, double v1, double v2, double v3)
		{
			if (v1 - ZERO_CLAMP < d && d < v1 + ZERO_CLAMP)
			{
				return v1;
			}
			else if (v2 - ZERO_CLAMP < d && d < v2 + ZERO_CLAMP)
			{
				return v2;
			}
			else if (v3 - ZERO_CLAMP < d && d < v3 + ZERO_CLAMP)
			{
				return v3;
			}
			return d;
		}

		double mixteProduct(const Eigen::Vector3d& u, const Eigen::Vector3d& v, const Eigen::Vector3d& w)
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

			double d = clamp(u.x() - k * v.x());
			bool areColinear = d == 0.0;
			if (areColinear)
			{
				d = clamp(u.y() - k * v.y());
				areColinear = d == 0.0;
				if (areColinear)
				{
					d = clamp(u.z() - k * v.z());
					areColinear = d == 0.0;
				}
			}
			return areColinear;
		}

		double angleBetween(Eigen::Vector3d vector1, Eigen::Vector3d vector2)
		{
			vector1.normalize();
			vector2.normalize();
			double num = vector1.dot(vector2);

			double radians = ((!(num < 0.0)) ? (2.0 * asin((vector1 - vector2).norm() / 2.0)) : (EIGEN_PI - 2.0 * asin((-vector1 - vector2).norm() / 2.0)));
			return r2d(radians);
		}
	}
}
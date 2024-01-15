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

		void clampMatrix(Eigen::Matrix3d& matrix)
		{
			matrix(0, 0) = clampv3(matrix(0, 0), 0.0, -1.0, 1.0);
			matrix(0, 1) = clampv3(matrix(0, 1), 0.0, -1.0, 1.0);
			matrix(0, 2) = clampv3(matrix(0, 2), 0.0, -1.0, 1.0);
			matrix(1, 0) = clampv3(matrix(1, 0), 0.0, -1.0, 1.0);
			matrix(1, 1) = clampv3(matrix(1, 1), 0.0, -1.0, 1.0);
			matrix(1, 2) = clampv3(matrix(1, 2), 0.0, -1.0, 1.0);
			matrix(2, 0) = clampv3(matrix(2, 0), 0.0, -1.0, 1.0);
			matrix(2, 1) = clampv3(matrix(2, 1), 0.0, -1.0, 1.0);
			matrix(2, 2) = clampv3(matrix(2, 2), 0.0, -1.0, 1.0);
		}

		void clampVector(Eigen::Vector3d& vector)
		{
			vector[0] = clampv3(vector[0], 0, -1.0, 1.0);
			vector[1] = clampv3(vector[1], 0, -1.0, 1.0);
			vector[2] = clampv3(vector[2], 0, -1.0, 1.0);
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

		bool getBase1IntoBase2(const Base3D& base1, const Base3D& base2, Base3D* base1InBase2)
		{
			Eigen::Matrix3d tb10 = base1.toMatrix3d();  // matrice de la base 1 dans R0
			Eigen::Matrix3d tb20 = base2.toMatrix3d();  // matrice de la base 2 dans R0

			if (tb20.determinant() == 0.0)
			{
				// La matrice n'est pas inversible
				return false;
			}

			Eigen::Matrix3d tb02 = tb20.inverse();            // matrice de passage de base2 vers R0
			Eigen::Matrix3d tb12 = tb02 * tb10;               // base 1 dans la base 2

			// Modifier la base1InBase2 avec la nouvelle base
			base1InBase2->setFromMatrix3d(tb12);

			return true;
		}
	}
}
#include "tools.h"
#include "macros.h"

#include <cmath>

namespace ReScan
{
	namespace Tools
	{
		int plan2DToString(const Plan2D& plan2D, std::string& result)
		{
			int r = SUCCESS_CODE;

			switch (plan2D)
			{
			case Plan2D::XY:
				result = "XY";
				break;
			case Plan2D::XZ:
				result = "XZ";
				break;
			case Plan2D::YZ:
				result = "YZ";
				break;
			default:
				result = "Unknow";
				r = INVALID_PLAN_ERROR_CODE;
				break;
			}
			return r;
		}

		int stringToPlan2D(const std::string planStr, Plan2D& plan2D)
		{
			int r = SUCCESS_CODE;

			if (planStr == "XY")
			{
				plan2D = Plan2D::XY;
			}
			else if (planStr == "XZ")
			{
				plan2D = Plan2D::XZ;
			}
			else if (planStr == "YZ")
			{
				plan2D = Plan2D::YZ;
			}
			else
			{
				r = INVALID_PLAN_ERROR_CODE;
			}

			return r;
		}

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
			return ((-ZERO_CLAMP < d) && (d < ZERO_CLAMP)) ? 0.0 : d;
		}

		double clampv(double d, double v)
		{
			return ((v - ZERO_CLAMP < d) && (d < v + ZERO_CLAMP)) ? v : d;
		}

		double clampv3(double d, double v1, double v2, double v3)
		{
			if ((v1 - ZERO_CLAMP < d) && (d < v1 + ZERO_CLAMP))
			{
				return v1;
			}
			else if ((v2 - ZERO_CLAMP < d) && (d < v2 + ZERO_CLAMP))
			{
				return v2;
			}
			else if ((v3 - ZERO_CLAMP < d) && (d < v3 + ZERO_CLAMP))
			{
				return v3;
			}
			return d;
		}

		double clampv5(double d, double v1, double v2, double v3, double v4, double v5)
		{
			if ((v1 - ZERO_CLAMP < d) && (d < v1 + ZERO_CLAMP))
			{
				return v1;
			}
			else if ((v2 - ZERO_CLAMP < d) && (d < v2 + ZERO_CLAMP))
			{
				return v2;
			}
			else if ((v3 - ZERO_CLAMP < d) && (d < v3 + ZERO_CLAMP))
			{
				return v3;
			}
			else if ((v4 - ZERO_CLAMP < d) && (d < v4 + ZERO_CLAMP))
			{
				return v4;
			}
			else if ((v5 - ZERO_CLAMP) < d && (d < v5 + ZERO_CLAMP))
			{
				return v5;
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

		int getBase1IntoBase2(const Base3D& base1, const Base3D& base2, Base3D* base1InBase2)
		{
			Eigen::Matrix3d tb10 = base1.toMatrix3d();  // matrice de la base 1 dans R0
			Eigen::Matrix3d tb20 = base2.toMatrix3d();  // matrice de la base 2 dans R0

			double detTb20 = tb20.determinant();
			int result = SUCCESS_CODE;

			if (detTb20 == 0.0)
			{
				// La matrice n'est pas inversible
				result = NO_MATRIX_INVERSE_ERROR_CODE;
			}
			else
			{
				Eigen::Matrix3d tb02 = tb20.inverse();            // matrice de passage de base2 vers R0
				Eigen::Matrix3d tb12 = tb02 * tb10;               // base 1 dans la base 2

				// Modifier la base1InBase2 avec la nouvelle base
				base1InBase2->setFromMatrix3d(tb12);
			}

			return result;
		}

		void computeABC(const Eigen::Matrix3d& matrix, double* a, double* b, double* c)
		{
			double r11, r12, r13, r21, r22, r23, r31, r32, r33;
			double x, y, z;

			r11 = matrix(0, 0);
			r12 = matrix(0, 1);
			r13 = matrix(0, 2);
			r21 = matrix(1, 0);
			r22 = matrix(1, 1);
			r23 = matrix(1, 2);
			r31 = matrix(2, 0);
			r32 = matrix(2, 1);
			r33 = matrix(2, 2);

			// Calcul des paramètres selon cours
			y = 180.0 * (atan2(-r31, sqrt(r11 * r11 + r21 * r21))) / EIGEN_PI;

			if (y == 90.0)          // singularité
			{
				x = 0.0;
				z = 180.0 * (atan2(r12, r22)) / EIGEN_PI;
			}
			else if (y == -90.0)     // singularité
			{
				x = 0.0;
				z = 180.0 * (-atan2(r12, r22)) / EIGEN_PI;
			}
			else
			{
				x = 180.0 * (atan2(r21, r11)) / EIGEN_PI;
				z = 180.0 * (atan2(r32, r33)) / EIGEN_PI;
			}

			x = clampv5(x, 0.0, 180.0, 360.0, -180.0, -360.0);
			y = clampv5(y, 0.0, 180.0, 360.0, -180.0, -360.0);
			z = clampv5(z, 0.0, 180.0, 360.0, -180.0, -360.0);

			*a = x;
			*b = y;
			*c = z;
		}

		void computeABC(const Eigen::Matrix4d& matrix, double* a, double* b, double* c)
		{
			double r11, r12, r13, r21, r22, r23, r31, r32, r33;
			double x, y, z;

			r11 = matrix(0, 0);
			r12 = matrix(0, 1);
			r13 = matrix(0, 2);
			r21 = matrix(1, 0);
			r22 = matrix(1, 1);
			r23 = matrix(1, 2);
			r31 = matrix(2, 0);
			r32 = matrix(2, 1);
			r33 = matrix(2, 2);

			// Calcul des paramètres selon cours
			y = 180.0 * (atan2(-r31, sqrt(r11 * r11 + r21 * r21))) / EIGEN_PI;

			if (y == 90.0)          // singularité
			{
				x = 0.0;
				z = 180.0 * (atan2(r12, r22)) / EIGEN_PI;
			}
			else if (y == -90.0)     // singularité
			{
				x = 0.0;
				z = 180.0 * (-atan2(r12, r22)) / EIGEN_PI;
			}
			else
			{
				x = 180.0 * (atan2(r21, r11)) / EIGEN_PI;
				z = 180.0 * (atan2(r32, r33)) / EIGEN_PI;
			}

			x = clampv5(x, 0.0, 180.0, 360.0, -180.0, -360.0);
			y = clampv5(y, 0.0, 180.0, 360.0, -180.0, -360.0);
			z = clampv5(z, 0.0, 180.0, 360.0, -180.0, -360.0);

			*a = x;
			*b = y;
			*c = z;
		}

		std::vector<std::string> splitString(const std::string& input, const std::string& delimiter) 
		{
			std::vector<std::string> result;
			size_t start = 0;
			size_t pos = input.find(delimiter);

			while (pos != std::string::npos)
			{
				result.push_back(input.substr(start, pos - start));
				start = pos + delimiter.length();
				pos = input.find(delimiter, start);
			}

			result.push_back(input.substr(start));

			return result;
		}
	}
}
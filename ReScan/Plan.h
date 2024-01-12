#ifndef RESCAN_PLAN_H
#define RESCAN_PLAN_H

#include "Point3D.h"
#include "Plan.h"

#include <string>
#include <iostream>
#include <Eigen/Dense>

namespace ReScan
{
	class Plan
	{
	private:
		double m_a;
		double m_b;
		double m_c;
		double m_d;

	public:
		Plan(double a = 0.0, double b = 0.0, double c = 0.0, double d = 0.0);
		Plan(const Plan& plan);

		double getA() const;
		void setA(const double a);

		double getB() const;
		void setB(const double b);

		double getC() const;
		void setC(const double c);

		double getD() const;
		void setD(const double d);

		void setABCD(double a, double b, double c, double d);

		void getNormal(Eigen::Vector3d& vector3d) const;

		static void getOrthogonalProjection(const Plan& plan, const Point3D&, Point3D* projection);

		static double getDistanceFrom(const Plan& plan, const Point3D& point);

		std::string toStr(const char* begin = "{ ", const char* end = " }", const char* sep = " ") const;

		friend std::ostream& operator<<(std::ostream& os, const Plan& plan);
	};
}

#endif // RESCAN_PLAN_H
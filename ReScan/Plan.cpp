#include "Plan.h"

namespace ReScan
{
	Plan::Plan(double a, double b, double c, double d) :
		m_a(a),
		m_b(b),
		m_c(c),
		m_d(d)
	{
	}

	Plan::Plan(const Plan& plan) :
		m_a(plan.m_a),
		m_b(plan.m_b),
		m_c(plan.m_c),
		m_d(plan.m_d)
	{
	}

	double Plan::getA() const
	{
		return m_a;
	}

	void Plan::setA(const double a)
	{
		m_a = a;
	}

	double Plan::getB() const
	{
		return m_b;
	}

	void Plan::setB(const double b)
	{
		m_b = b;
	}

	double Plan::getC() const
	{
		return m_c;
	}

	void Plan::setC(const double c)
	{
		m_c = c;
	}

	double Plan::getD() const
	{
		return m_d;
	}

	void Plan::setD(const double d)
	{
		m_d = d;
	}

	void Plan::setABCD(double a, double b, double c, double d)
	{
		setA(a);
		setB(b);
		setC(c);
		setD(d);
	}

	void Plan::getNormal(Eigen::Vector3d& vector3d) const
	{
		vector3d[0] = m_a;
		vector3d[1] = m_b;
		vector3d[2] = m_c;
	}

	void Plan::getOrthogonalProjection(const Plan& plan, const Point3D& point, Point3D* projection)
	{
		/*
		Soit un plan P : ax + by + cz + d = 0
		Soit un point A. Le projete orthogonale du point A, nomme A', appartient a la droite L passant par A et de direction n, vecteur normal
		au plan P, de coordonees n = (a, b, c).

		Si un point M appartient a L, alors il verifie le systeme :

			Xm = a*t + Xa
		S1: Ym = b*t + Ya
			Zm = c*t + Za

		De plus, si un point M apparient au plan P, alors a*Xm + b*Ym + c*Zm + d = 0

		A' est un point qui appartient a L et a P donc :

			a*Xa' + b*Ya' + c*Za' + d = 0

		<=> a*(a*t + Xa) + b*(b*t + Ya) + c*(c*t + Za) + d = 0
		<=> t = - (a*Xa + b*Ya + c*Za) / (a^2 + b^2 + c^2)

		Maintenant qu'on a t, on peut trouver les coordonnees du point A' en le mettant dans S1.
		*/

		double t = -(plan.m_a * point.getX() + plan.m_b * point.getY() + plan.m_c * point.getZ() + plan.m_d) / (plan.m_a * plan.m_a + plan.m_b * plan.m_b + plan.m_c * plan.m_c);
		projection->setXYZ(
			plan.m_a * t + point.getX(),
			plan.m_b * t + point.getY(),
			plan.m_c * t + point.getZ()
		);
	}

	double Plan::getDistanceFrom(const Plan& plan, const Point3D& point)
	{
		/*
				 | a*Xm + b*Ym + c*Zm + d  |
		D(P,M) = | ----------------------- |
				 |    sqrt(a^2+b^2+c^2)	   |
		*/
		return abs((plan.m_a * point.getX() + plan.m_b * point.getY() + plan.m_c * point.getZ() + plan.m_d) / sqrt(plan.m_a * plan.m_a + plan.m_b * plan.m_b + plan.m_c * plan.m_c));
	}

	std::string Plan::toStr(const char* begin, const char* end, const char* sep) const
	{
		std::ostringstream oss;
		oss << begin << m_a << sep << m_b << sep << m_c << sep << m_d << end;
		return oss.str();
	}

	std::ostream& operator<<(std::ostream& os, const Plan& plan)
	{
		os << plan.toStr();
		return os;
	}
}

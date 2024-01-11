#include "Plan.h"

namespace ReScan
{
	Plan::Plan(double a, double b, double c, double d) :
		a(a),
		b(b),
		c(c),
		d(d)
	{
	}

	Plan::Plan(const Plan& plan) :
		a(plan.a),
		b(plan.b),
		c(plan.c),
		d(plan.d)
	{
	}

	double Plan::getA() const
	{
		return a;
	}

	void Plan::setA(const double a)
	{
		this->a = a;
	}

	double Plan::getB() const
	{
		return b;
	}

	void Plan::setB(const double b)
	{
		this->b = b;
	}

	double Plan::getC() const
	{
		return c;
	}

	void Plan::setC(const double c)
	{
		this->c = c;
	}

	double Plan::getD() const
	{
		return d;
	}

	void Plan::setD(const double d)
	{
		this->d = d;
	}

	void Plan::setABCD(double a, double b, double c, double d)
	{
		setA(a);
		setB(b);
		setC(c);
		setD(d);
	}

	void Plan::getNormal(Vector3D& vector3D) const
	{
		vector3D.setXYZ(a, b, c);
	}


	void Plan::getOrthogonalProjection(const Plan* plan, const Point3D* point, Point3D* projection)
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

		double t = -(plan->a * point->getX() + plan->b * point->getY() + plan->c * point->getZ() + plan->d) / (plan->a * plan->a + plan->b * plan->b + plan->c * plan->c);
		projection->setXYZ(
			plan->a * t + point->getX(),
			plan->b * t + point->getY(),
			plan->c * t + point->getZ()
		);
	}

	double Plan::getDistanceFrom(const Plan* plan, const Point3D* point)
	{
		/*
				 | a*Xm + b*Ym + c*Zm + d  |
		D(P,M) = | ----------------------- |
				 |    sqrt(a^2+b^2+c^2)	   |
		*/
		return abs((plan->a * point->getX() + plan->b * point->getY() + plan->c * point->getZ() + plan->d) / sqrt(plan->a * plan->a + plan->b * plan->b + plan->c * plan->c));
	}

	std::string Plan::toStr(const char* begin, const char* end, const char* sep) const
	{
		std::ostringstream oss;
		oss << begin << a << sep << b << sep << c << sep << d << end;
		return oss.str();
	}

	std::ostream& operator<<(std::ostream& os, const Plan& plan)
	{
		os << plan.toStr();
		return os;
	}
}

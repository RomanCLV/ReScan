#include "Plan.h"

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

void Plan::getNormal(Vector3D& vector3D) 
{
	vector3D.setXYZ(a, b, c);
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

#ifndef PLAN_H
#define PLAN_H

#include "Vector3D.h"
#include "Point3D.h"
#include "Plan.h"

#include <string>
#include <iostream>

class Plan
{
private:
	double a;
	double b;
	double c;
	double d;

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

	void getNormal(Vector3D& vector3D) const;

	static void getOrthogonalProjection(const Plan* plan, const Point3D*, Point3D* projection);

	static double getDistanceFrom(const Plan* plan, const Point3D* point);

	std::string toStr(const char* begin = "{ ", const char* end = " }", const char* sep = " ") const;

	friend std::ostream& operator<<(std::ostream& os, const Plan& plan);
};

#endif // PLAN_H
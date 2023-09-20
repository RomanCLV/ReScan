#ifndef REPERE3D_H
#define REPERE3D_H

#include "Vector3D.h"
#include "Point3D.h"

class Repere3D
{
public:
	Point3D origin;
	Vector3D x;
	Vector3D y;
	Vector3D z;

public:
	Repere3D();
	Repere3D(const Repere3D& repere3D);
	Repere3D(const Point3D& origin);
	Repere3D(const Vector3D& x, const Vector3D& y, const Vector3D& z);
	Repere3D(const Point3D& origin, const Vector3D& x, const Vector3D& y, const Vector3D& z);
	~Repere3D();
};

#endif // REPERE3D_H
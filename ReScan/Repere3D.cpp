#include "Repere3D.h"

Repere3D::Repere3D() :
	origin(0, 0, 0),
	x(1, 0, 0),
	y(0, 1, 0),
	z(0, 0, 1)
{
}

Repere3D::Repere3D(const Repere3D& repere3D) :
	origin(repere3D.origin),
	x(repere3D.x),
	y(repere3D.y),
	z(repere3D.z)
{
}

Repere3D::Repere3D(const Point3D& origin) :
	origin(origin),
	x(1, 0, 0),
	y(0, 1, 0),
	z(0, 0, 1)
{
}

Repere3D::Repere3D(const Vector3D& x, const Vector3D& y, const Vector3D& z) :
	origin(0, 0, 0),
	x(x),
	y(y),
	z(z)
{
}

Repere3D::Repere3D(const Point3D& origin, const Vector3D& x, const Vector3D& y, const Vector3D& z) :
	origin(origin),
	x(x),
	y(y),
	z(z)
{
}

Repere3D::~Repere3D()
{
}

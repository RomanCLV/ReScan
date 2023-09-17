#include "Point3D.h"

Point3D::Point3D(double x, double y, double z) :
	x(x),
	y(y),
	z(z) 
{}

Point3D::Point3D(const Point3D& point3D) : 
	x(point3D.x),
	y(point3D.y),
	z(point3D.z)
{}

double Point3D::getX() const 
{
	return x;
}

void Point3D::setX(const double x)
{
	this->x = x;
}

double Point3D::getY() const 
{
	return y;
}

void Point3D::setY(const double y)
{
	this->y = y;
}

double Point3D::getZ() const
{
	return z;
}

void Point3D::setZ(const double z) 
{
	this->z = z;
}

void Point3D::setXYZ(const double x, const double y, const double z)
{
	setX(x);
	setY(y);
	setZ(z);
}

void Point3D::getDiff(const Point3D& point, Vector3D& result) const
{
	result.setXYZ(point.x - x, point.y - y, point.z - z);
}

std::string Point3D::toStr(const char* begin, const char* end, const char* sep) const
{
	std::ostringstream oss;
	oss << begin << x << sep << y << sep << z << end;
	return oss.str();
}

Vector3D operator-(const Point3D& p1, const Point3D& p2)
{
	Vector3D vector;
	p1.getDiff(p2, vector);
	return vector;
}

Point3D operator-(const Point3D& p)
{
	return Point3D(-p.getX(), -p.getY(), -p.getZ());
}

std::ostream& operator<<(std::ostream& os, const Point3D& point3D) 
{
	os << point3D.toStr();
	return os;
}

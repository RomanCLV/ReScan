#include "Vector3D.h"

Vector3D::Vector3D(double x, double y, double z) :
	x(x),
	y(y),
	z(z)
{}

Vector3D::Vector3D(const Vector3D& vector3D) :
	x(vector3D.x),
	y(vector3D.y),
	z(vector3D.z)
{}

double Vector3D::getX() const
{
	return x;
}

void Vector3D::setX(const double x)
{
	this->x = x;
}

double Vector3D::getY() const
{
	return y;
}

void Vector3D::setY(const double y)
{
	this->y = y;
}

double Vector3D::getZ() const
{
	return z;
}

void Vector3D::setZ(const double z)
{
	this->z = z;
}

void Vector3D::setXYZ(const double x, const double y, const double z)
{
	setX(x);
	setY(y);
	setZ(z);
}

double Vector3D::norm() const
{
    return sqrt(x * x + y * y + z * z);
}

double Vector3D::dot(const Vector3D& other) const
{
    return x * other.x + y * other.y + z * other.z;
}

void Vector3D::cross(const Vector3D& other, Vector3D& result) const
{
    result.setXYZ(y * other.z - z * other.y, z * other.x - x * other.z, x * other.y - y * other.x);
}

Vector3D Vector3D::normalize() const
{
    double magnitude = norm();
    if (magnitude != 0.0) {
        return Vector3D(x / magnitude, y / magnitude, z / magnitude);
    }
    // Gérer le cas où le vecteur est nul (division par zéro)
    // Ici, nous renvoyons simplement le vecteur lui-même.
    return *this;
}

std::string Vector3D::toStr(const char* begin, const char* end, const char* sep) const
{
	std::ostringstream oss;
	oss << begin << x << sep << y << sep << z << end;
	return oss.str();
}

std::ostream& operator<<(std::ostream& os, const Vector3D& vector3D) {
	os << vector3D.toStr();
	return os;
}

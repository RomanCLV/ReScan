#include "Point3D.h"

namespace ReScan
{
	Point3D::Point3D(double x, double y, double z) :
		m_x(x),
		m_y(y),
		m_z(z)
	{}

	Point3D::Point3D(const Point3D& point3D) :
		m_x(point3D.m_x),
		m_y(point3D.m_y),
		m_z(point3D.m_z)
	{}

	double Point3D::getX() const
	{
		return m_x;
	}

	void Point3D::setX(const double x)
	{
		m_x = x;
	}

	double Point3D::getY() const
	{
		return m_y;
	}

	void Point3D::setY(const double y)
	{
		m_y = y;
	}

	double Point3D::getZ() const
	{
		return m_z;
	}

	void Point3D::setZ(const double z)
	{
		m_z = z;
	}

	void Point3D::setXYZ(const double x, const double y, const double z)
	{
		m_x = x;
		m_y = y;
		m_z = z;
	}

	void Point3D::setFrom(const Point3D& point)
	{
		m_x = point.m_x;
		m_y = point.m_y;
		m_z = point.m_z;
	}

	void Point3D::getDiff(const Point3D& point, Eigen::Vector3d* result) const
	{
		(*result)[0] = m_x - point.m_x;
		(*result)[1] = m_y - point.m_y;
		(*result)[2] = m_z - point.m_z;
	}

	double Point3D::distanceBetween(const Point3D& p1, const Point3D& p2)
	{
		return sqrt((p1.m_x - p2.m_x) * (p1.m_x - p2.m_x) + (p1.m_y - p2.m_y) * (p1.m_y - p2.m_y) + (p1.m_z - p2.m_z) * (p1.m_z - p2.m_z));
	}

	Eigen::Vector3d Point3D::toVector3d() const
	{
		return Eigen::Vector3d(m_x, m_y, m_z);
	}

	std::string Point3D::toStr(const char* begin, const char* end, const char* sep) const
	{
		std::ostringstream oss;
		oss << begin << m_x << sep << m_y << sep << m_z << end;
		return oss.str();
	}

	Eigen::Vector3d operator-(const Point3D& p1, const Point3D& p2)
	{
		Eigen::Vector3d vector;
		p1.getDiff(p2, &vector);
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
}

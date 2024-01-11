#include "Repere3D.h"

namespace ReScan
{
	Repere3D::Repere3D() :
		m_origin(0.0, 0.0, 0.0),
		m_x(1.0, 0.0, 0.0),
		m_y(0.0, 1.0, 0.0),
		m_z(0.0, 0.0, 1.0)
	{
	}

	Repere3D::Repere3D(const Repere3D& repere3D) :
		m_origin(repere3D.m_origin),
		m_x(repere3D.m_x),
		m_y(repere3D.m_y),
		m_z(repere3D.m_z)
	{
	}

	Repere3D::Repere3D(const Point3D& origin) :
		m_origin(origin),
		m_x(1.0, 0.0, 0.0),
		m_y(0.0, 1.0, 0.0),
		m_z(0.0, 0.0, 1.0)
	{
	}

	Repere3D::Repere3D(const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z) :
		m_origin(0.0, 0.0, 0.0),
		m_x(x),
		m_y(y),
		m_z(z)
	{
	}

	Repere3D::Repere3D(const Point3D& origin, const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z) :
		m_origin(origin),
		m_x(x),
		m_y(y),
		m_z(z)
	{
	}

	Repere3D::~Repere3D()
	{
	}

	const Point3D* Repere3D::getOrigin() const
	{
		return &m_origin;
	}

	void Repere3D::setOrigin(const Point3D& origin) 
	{
		m_origin.setFrom(origin);
	}

	void Repere3D::setX(const Eigen::Vector3d& x)
	{
		m_x[0] = x[0];
		m_x[1] = x[1];
		m_x[2] = x[2];
	}

	const Eigen::Vector3d* Repere3D::getX() const
	{
		return &m_x;
	}

	void Repere3D::setY(const Eigen::Vector3d& y)
	{
		m_y[0] = y[0];
		m_y[1] = y[1];
		m_y[2] = y[2];
	}

	const Eigen::Vector3d* Repere3D::getY() const
	{
		return &m_y;
	}

	void Repere3D::setZ(const Eigen::Vector3d& z)
	{
		m_z[0] = z[0];
		m_z[1] = z[1];
		m_z[2] = z[2];
	}

	const Eigen::Vector3d* Repere3D::getZ() const
	{
		return &m_z;
	}

	void Repere3D::normalize()
	{
		m_x.normalize();
		m_y.normalize();
		m_z.normalize();
	}

	void Repere3D::normalizeX()
	{
		m_x.normalize();
	}

	void Repere3D::normalizeY()
	{
		m_y.normalize();
	}

	void Repere3D::normalizeZ()
	{
		m_z.normalize();
	}
}

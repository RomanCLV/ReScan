#include "Base3D.h"
#include "tools.h"

#include <cmath> // for fmod

using namespace Eigen;

namespace ReScan
{
	Base3D::Base3D() :
		m_origin(0.0, 0.0, 0.0),
		m_x(1.0, 0.0, 0.0),
		m_y(0.0, 1.0, 0.0),
		m_z(0.0, 0.0, 1.0)
	{
	}

	Base3D::Base3D(const Base3D& repere3D) :
		m_origin(repere3D.m_origin),
		m_x(repere3D.m_x),
		m_y(repere3D.m_y),
		m_z(repere3D.m_z)
	{
	}

	Base3D::Base3D(const Point3D& origin) :
		m_origin(origin),
		m_x(1.0, 0.0, 0.0),
		m_y(0.0, 1.0, 0.0),
		m_z(0.0, 0.0, 1.0)
	{
	}

	Base3D::Base3D(const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z) :
		m_origin(0.0, 0.0, 0.0),
		m_x(x),
		m_y(y),
		m_z(z)
	{
	}

	Base3D::Base3D(const Point3D& origin, const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z) :
		m_origin(origin),
		m_x(x),
		m_y(y),
		m_z(z)
	{
	}

	Base3D::~Base3D()
	{
	}

	const Point3D* Base3D::getOrigin() const
	{
		return &m_origin;
	}

	void Base3D::setOrigin(const Point3D& origin)
	{
		m_origin.setFrom(origin);
	}

	void Base3D::setX(const Eigen::Vector3d& x)
	{
		m_x[0] = x[0];
		m_x[1] = x[1];
		m_x[2] = x[2];
	}

	const Eigen::Vector3d* Base3D::getX() const
	{
		return &m_x;
	}

	void Base3D::setY(const Eigen::Vector3d& y)
	{
		m_y[0] = y[0];
		m_y[1] = y[1];
		m_y[2] = y[2];
	}

	const Eigen::Vector3d* Base3D::getY() const
	{
		return &m_y;
	}

	void Base3D::setZ(const Eigen::Vector3d& z)
	{
		m_z[0] = z[0];
		m_z[1] = z[1];
		m_z[2] = z[2];
	}

	const Eigen::Vector3d* Base3D::getZ() const
	{
		return &m_z;
	}

	void Base3D::normalize()
	{
		m_x.normalize();
		m_y.normalize();
		m_z.normalize();
	}

	void Base3D::normalizeX()
	{
		m_x.normalize();
	}

	void Base3D::normalizeY()
	{
		m_y.normalize();
	}

	void Base3D::normalizeZ()
	{
		m_z.normalize();
	}

	Base3D Base3D::computeOrientedBase(Eigen::Vector3d direction, Axis axis)
	{
		Vector3d rotationAxis;
		Base3D base3D;
		double angle = 0.0;

		direction.normalize();

		switch (axis)
		{
		case ReScan::Axis::X:
			rotationAxis = base3D.getX()->cross(direction);
			angle = Tools::angleBetween(*base3D.getX(), direction);
			break;
		case ReScan::Axis::Y:
			rotationAxis = base3D.getY()->cross(direction);
			angle = Tools::angleBetween(*base3D.getY(), direction);
			break;
		case ReScan::Axis::Z:
			rotationAxis = base3D.getZ()->cross(direction);
			angle = Tools::angleBetween(*base3D.getZ(), direction);
			break;
		default:
			throw std::invalid_argument("Invalid axis given: " + std::to_string(int(axis)));
			break;
		}

		// clamp to 0, or -/+ 180 and then, modulo it by 180
		angle = fmod(Tools::clampv3(angle, 0.0, -180.0, 180.0), 180.0); // fmod : modulo for float/double

		if (angle != 0.0)
		{
			// Convert angle (that is in ]-180;180[ domain to radians
			double angleRad = Tools::d2r(angle);

			rotationAxis.normalize();

			Eigen::Quaterniond quaternion;
			quaternion = Eigen::AngleAxisd(-angleRad, rotationAxis);

			// Convert the quaternion into a rotation matrix
			Eigen::Matrix3d rotationMatrix = quaternion.toRotationMatrix();

			base3D.setX(rotationMatrix.row(0));
			base3D.setY(rotationMatrix.row(1));
			base3D.setZ(rotationMatrix.row(2));
		}

		return base3D;
	}

}

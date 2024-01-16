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
		m_z(0.0, 0.0, 1.0),
		m_isRotating(false),
		m_beginRotateX(0.0, 0.0, 0.0),
		m_beginRotateY(0.0, 0.0, 0.0),
		m_beginRotateZ(0.0, 0.0, 0.0)
	{
	}

	Base3D::Base3D(const Base3D& base3D) :
		m_origin(base3D.m_origin),
		m_x(base3D.m_x),
		m_y(base3D.m_y),
		m_z(base3D.m_z),
		m_isRotating(false),
		m_beginRotateX(0.0, 0.0, 0.0),
		m_beginRotateY(0.0, 0.0, 0.0),
		m_beginRotateZ(0.0, 0.0, 0.0)
	{
	}

	Base3D::Base3D(const Point3D& origin) :
		m_origin(origin),
		m_x(1.0, 0.0, 0.0),
		m_y(0.0, 1.0, 0.0),
		m_z(0.0, 0.0, 1.0),
		m_isRotating(false),
		m_beginRotateX(0.0, 0.0, 0.0),
		m_beginRotateY(0.0, 0.0, 0.0),
		m_beginRotateZ(0.0, 0.0, 0.0)
	{
	}

	Base3D::Base3D(const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z) :
		m_origin(0.0, 0.0, 0.0),
		m_x(x),
		m_y(y),
		m_z(z),
		m_isRotating(false),
		m_beginRotateX(0.0, 0.0, 0.0),
		m_beginRotateY(0.0, 0.0, 0.0),
		m_beginRotateZ(0.0, 0.0, 0.0)
	{
	}

	Base3D::Base3D(const Point3D& origin, const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z) :
		m_origin(origin),
		m_x(x),
		m_y(y),
		m_z(z),
		m_isRotating(false),
		m_beginRotateX(0.0, 0.0, 0.0),
		m_beginRotateY(0.0, 0.0, 0.0),
		m_beginRotateZ(0.0, 0.0, 0.0)
	{
	}

	Base3D::~Base3D()
	{
	}

	void Base3D::reset()
	{
		m_origin.setXYZ(0.0, 0.0, 0.0);

		m_x[0] = 1.0;
		m_x[1] = 0.0;
		m_x[2] = 0.0;

		m_y[0] = 0.0;
		m_y[1] = 1.0;
		m_y[2] = 0.0;

		m_z[0] = 0.0;
		m_z[1] = 0.0;
		m_z[2] = 1.0;
	}

	void Base3D::setFrom(const Base3D& base3D, const bool setOrigin)
	{
		if (setOrigin)
		{
			m_origin.setFrom(*base3D.getOrigin());
		}
		setX(*base3D.getX());
		setY(*base3D.getY());
		setZ(*base3D.getZ());
	}

	void Base3D::setFromMatrix3d(const Eigen::Matrix3d& matrix)
	{
		m_x = matrix.col(0);
		m_y = matrix.col(1);
		m_z = matrix.col(2);
	}

	void Base3D::setFromMatrix4d(const Eigen::Matrix4d& matrix, const bool setOrigin)
	{
		m_x = matrix.block<3, 1>(0, 0);
		m_y = matrix.block<3, 1>(0, 1);
		m_z = matrix.block<3, 1>(0, 2);

		if (setOrigin)
		{
			auto translation = matrix.block<3, 1>(0, 3);
			m_origin.setXYZ(translation[0], translation[1], translation[2]);
		}
	}

	void Base3D::setXYZ(const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z)
	{
		m_x[0] = x[0];
		m_x[1] = x[1];
		m_x[2] = x[2];

		m_y[0] = y[0];
		m_y[1] = y[1];
		m_y[2] = y[2];

		m_z[0] = z[0];
		m_z[1] = z[1];
		m_z[2] = z[2];
	}

	Eigen::Matrix3d Base3D::toMatrix3d() const
	{
		Eigen::Matrix3d matrix;
		matrix.col(0) = m_x;
		matrix.col(1) = m_y;
		matrix.col(2) = m_z;
		return matrix;
	}

	Eigen::Matrix4d Base3D::toMatrix4d() const
	{
		Eigen::Matrix4d matrix;
		matrix.block<3, 3>(0, 0) = toMatrix3d();
		matrix.block<3, 1>(0, 3) = m_origin.toVector3d();
		matrix.row(3).setZero();
		matrix(3, 3) = 1.0;
		return matrix;
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

	void Base3D::setX(const double xx, const double xy, const double xz)
	{
		m_x[0] = xx;
		m_x[1] = xy;
		m_x[2] = xz;
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

	void Base3D::setY(const double yx, const double yy, const double yz)
	{
		m_y[0] = yx;
		m_y[1] = yy;
		m_y[2] = yz;
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

	void Base3D::setZ(const double zx, const double zy, const double zz)
	{
		m_z[0] = zx;
		m_z[1] = zy;
		m_z[2] = zz;
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

	bool Base3D::getIsRotating()
	{
		return m_isRotating;
	}

	void Base3D::beginRotate() 
	{
		m_isRotating = true;

		m_beginRotateX[0] = m_x[0];
		m_beginRotateX[1] = m_x[1];
		m_beginRotateX[2] = m_x[2];

		m_beginRotateY[0] = m_y[0];
		m_beginRotateY[1] = m_y[1];
		m_beginRotateY[2] = m_y[2];

		m_beginRotateZ[0] = m_z[0];
		m_beginRotateZ[1] = m_z[1];
		m_beginRotateZ[2] = m_z[2];
	}

	void Base3D::endRotate()
	{
		m_isRotating = false;
	}

	void Base3D::rotate(const Eigen::Vector3d& rotationAxis, const double rotationAngle, const bool autoCallEndRotate)
	{
		if (!m_isRotating)
		{
			beginRotate();
		}

		Eigen::Quaterniond quaternion;
		quaternion = Eigen::AngleAxisd(Tools::d2r(rotationAngle), rotationAxis);
		Eigen::Matrix3d rotationMatrix = quaternion.toRotationMatrix();

		Eigen::Vector3d x = rotationMatrix * m_beginRotateX;
		Eigen::Vector3d y = rotationMatrix * m_beginRotateY;
		Eigen::Vector3d z = rotationMatrix * m_beginRotateZ;

		Tools::clampVector(x);
		Tools::clampVector(y);
		Tools::clampVector(z);

		setX(x);
		setY(y);
		setZ(z);

		if (autoCallEndRotate)
		{
			endRotate();
		}
	}

	void Base3D::toEulerAnglesZYX(double* a, double* b, double* c) const
	{
		Tools::computeABC(toMatrix3d(), a, b, c);
	}


	Base3D Base3D::computeOrientedBase(Eigen::Vector3d direction, const Axis axis)
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
			// Convert angle (that is in ]-180;180[ domain) to radians
			double angleRad = Tools::d2r(angle);

			rotationAxis.normalize();

			Eigen::Quaterniond quaternion;
			quaternion = Eigen::AngleAxisd(-angleRad, rotationAxis);

			// Convert the quaternion into a rotation matrix
			Eigen::Matrix3d rotationMatrix = quaternion.toRotationMatrix();
			Tools::clampMatrix(rotationMatrix);

			// etrange d'utiliser row à la place de colonne mais ça marche
			base3D.setX(rotationMatrix.row(0));
			base3D.setY(rotationMatrix.row(1));
			base3D.setZ(rotationMatrix.row(2));

			// normally, x y and z are already normalized
		}

		return base3D;
	}

}

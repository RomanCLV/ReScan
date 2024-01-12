#ifndef RESCAN_REPERE3D_H
#define RESCAN_REPERE3D_H

#include "Point3D.h"
#include "Axis.h"
#include <Eigen/Dense>

namespace ReScan
{
	class Base3D
	{
	private:
		Point3D m_origin;
		Eigen::Vector3d m_x;
		Eigen::Vector3d m_y;
		Eigen::Vector3d m_z;

	public:
		Base3D();
		Base3D(const Base3D& repere3D);
		Base3D(const Point3D& origin);
		Base3D(const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z);
		Base3D(const Point3D& origin, const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z);
		~Base3D();

		const Point3D* getOrigin() const;
		void setOrigin(const Point3D& origin);

		const Eigen::Vector3d* getX() const;
		void setX(const Eigen::Vector3d& x);

		const Eigen::Vector3d* getY() const;
		void setY(const Eigen::Vector3d& y);

		const Eigen::Vector3d* getZ() const;
		void setZ(const Eigen::Vector3d& z);

		void normalize();
		void normalizeX();
		void normalizeY();
		void normalizeZ();

		static Base3D computeOrientedBase(const Eigen::Vector3d& direction, Axis axis);
	};
}

#endif // RESCAN_REPERE3D_H
#ifndef REPERE3D_H
#define REPERE3D_H

#include "Point3D.h"
#include <Eigen/Dense>

namespace ReScan
{
	class Repere3D
	{
	private:
		Point3D m_origin;
		Eigen::Vector3d m_x;
		Eigen::Vector3d m_y;
		Eigen::Vector3d m_z;

	public:
		Repere3D();
		Repere3D(const Repere3D& repere3D);
		Repere3D(const Point3D& origin);
		Repere3D(const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z);
		Repere3D(const Point3D& origin, const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z);
		~Repere3D();

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
	};
}

#endif // REPERE3D_H
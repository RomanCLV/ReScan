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

		bool m_isRotating;
		Eigen::Vector3d m_beginRotateX;
		Eigen::Vector3d m_beginRotateY;
		Eigen::Vector3d m_beginRotateZ;

	public:
		Base3D();
		Base3D(const Base3D& base3D);
		Base3D(const Point3D& origin);
		Base3D(const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z);
		Base3D(const Point3D& origin, const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z);
		~Base3D();

		void reset();
		void setFrom(const Base3D& base3D, const bool setOrigin = true);
		void setFromMatrix3d(const Eigen::Matrix3d& matrix);
		void setFromMatrix4d(const Eigen::Matrix4d& matrix, const bool setOrigin = true);
		void setXYZ(const Eigen::Vector3d& x, const Eigen::Vector3d& y, const Eigen::Vector3d& z);

		Eigen::Matrix3d toMatrix3d() const;
		Eigen::Matrix4d toMatrix4d() const;

		const Point3D* getOrigin() const;
		void setOrigin(const Point3D& origin);

		const Eigen::Vector3d* getX() const;
		void setX(const Eigen::Vector3d& x);
		void setX(const double xx, const double xy, const double xz);

		const Eigen::Vector3d* getY() const;
		void setY(const Eigen::Vector3d& y);
		void setY(const double yx, const double yy, const double yz);

		const Eigen::Vector3d* getZ() const;
		void setZ(const Eigen::Vector3d& z);
		void setZ(const double zx, const double zy, const double zz);

		void normalize();
		void normalizeX();
		void normalizeY();
		void normalizeZ();

		bool getIsRotating();

		/// <summary>
		/// Indicates the start of a base rotation. To call before rotate(Vector3D, double, bool).
		/// Stores the current x, y and z axes as reference axes for calculating the rotation.
		/// </summary>
		void beginRotate();

		/// <summary>
		/// Indicates the end of a rotation. Call after rotate(Vector3D, double, bool). 
		/// Releases reference axes.
		/// </summary>
		void endRotate();

		/// <summary>
		/// Rotates the base (using the reference axes saved during beginRotate()) in a given direction and angle. 
		/// beginRotate() is called automatically if the base does not rotate.
		/// </summary>
		/// <param name="rotationAxis">The direction</param>
		/// <param name="rotationAngle">The angle in degrees</param>
		/// <param name="autoCallEndRotate">Call endRotate() automatically.
		/// If you only have one rotation, leave it set to true. 
		/// Otherwise, if you have several rotations (with always the same reference, for example to produce a continuous rotation) 
		/// set it to false and don't forget to call endRotate() when all rotations have been applied.
		/// </param>
		void rotate(const Eigen::Vector3d& rotationAxis, const double rotationAngle, const bool autoCallEndRotate = true);

		static Base3D computeOrientedBase(Eigen::Vector3d direction, const Axis axis);
	};
}

#endif // RESCAN_REPERE3D_H
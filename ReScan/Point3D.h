#ifndef POINT3D_H
#define POINT3D_H

#include <iostream>
#include <sstream>
#include <string>
#include <vector>

#include <Eigen/Dense>

namespace ReScan
{
	class Point3D 
	{
	private:
		double m_x;
		double m_y;
		double m_z;

	public:
		Point3D(double x = 0.0, double y = 0.0, double z = 0.0);
		Point3D(const Point3D& point3D);

		double getX() const;
		void setX(const double x);

		double getY() const;
		void setY(const double y);

		double getZ() const;
		void setZ(const double z);

		void setXYZ(const double x, const double y, const double z);
		void setFrom(const Point3D& point);

		void getDiff(const Point3D& point, Eigen::Vector3d* result) const;

		static double distanceBetween(const Point3D& p1, const Point3D& p2);

		std::string toStr(const char* begin = "{ ", const char* end = " }", const char* sep = " ") const;

		friend Eigen::Vector3d operator-(const Point3D& p1, const Point3D& p2);
		friend Point3D operator-(const Point3D& p);
		friend std::ostream& operator<<(std::ostream& os, const Point3D& point3D);
	};
}

#endif // POINT3D_H
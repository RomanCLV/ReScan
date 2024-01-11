#ifndef VECTOR3D_H
#define VECTOR3D_H

#include <cmath>
#include <iostream>
#include <sstream>
#include <string>

namespace ReScan
{
	class Vector3D
	{
	private:
		double x;
		double y;
		double z;

	public:
		Vector3D(double x = 0.0, double y = 0.0, double z = 0.0);
		Vector3D(const Vector3D& vector3D);

		double getX() const;
		void setX(const double x);

		double getY() const;
		void setY(const double y);

		double getZ() const;
		void setZ(const double z);

		void setXYZ(const double x, const double y, const double z);

		// Méthode pour calculer la norme du vecteur
		double norm() const;

		// Méthode pour calculer le produit scalaire avec un autre vecteur
		double dot(const Vector3D& other) const;

		// Méthode pour calculer le produit vectoriel avec un autre vecteur
		void cross(const Vector3D& other, Vector3D& result) const;

		// Méthode pour normaliser le vecteur (le rendre unitaire)
		Vector3D normalize() const;

		std::string toStr(const char* begin = "{ ", const char* end = " }", const char* sep = " ") const;

		friend std::ostream& operator<<(std::ostream& os, const Vector3D& vector3D);
	};
}

#endif // VECTOR3D_H
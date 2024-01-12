#ifndef RESCAN_TOOLS_H
#define RESCAN_TOOLS_H

#include <Eigen/Dense>

namespace ReScan
{
	namespace Tools
	{
		void strReplace(std::string& s, char oldChar, char newChar);

		/// <summary>
		/// Radians to degrees.
		/// </summary>
		double r2d(double radian);

		/// <summary>
		/// Degrees to radians.
		/// </summary>
		double d2r(double degree);
		
		/// <summary>
		/// Try to clamp the value d to 0.
		/// </summary>
		/// <param name="d">The value to clamp.</param>
		/// <returns>Return 0 if d is in ]-0.001 ; 0.001[ else return d.</returns>
		double clamp(double d);

		/// <summary>
		/// Try to clamp the value d to v.
		/// </summary>
		/// <param name="d">The value to clamp.</param>
		/// <param name="v">The clamped value.</param>
		/// <returns>Return v if d is in ]v-0.001 ; v+0.001[ else return d.</returns>
		double clampv(double d, double v);

		/// <summary>
		/// Try to clamp the value d to v1, or v2 or v3.
		/// </summary>
		/// <param name="d">The value to clamp.</param>
		/// <param name="v1">First clamped value.</param>
		/// <param name="v2">Second clamped value.</param>
		/// <param name="v3">Third clamped value.</param>
		/// <returns>Return vi if d is in ]vi-0.001 ; vi+0.001[ else return d.</returns>
		double clampv3(double d, double v1, double v2, double v3);

		/// <summary>
		/// Return the mixte product u.dot(v.cross(w))
		/// </summary>
		/// <param name="u">First vector</param>
		/// <param name="v">Second vector</param>
		/// <param name="w">Third vector</param>
		/// <returns>Return the mixte product u.dot(v.cross(w))</returns>
		double mixteProduct(const Eigen::Vector3d& u, const Eigen::Vector3d& v, const Eigen::Vector3d& w);

		/// <summary>
		/// Express if we can express u = k*v with k a real number.
		/// </summary>
		/// <param name="u">First vector</param>
		/// <param name="v">Second vector</param>
		/// <returns>Return true if it exists a real number as u = k*v is true. If u = 0 or v = 0 (or both), it returns true.</returns>
		bool areVectorsColinear(const Eigen::Vector3d& u, const Eigen::Vector3d& v);

		/// <summary>
		/// Retrieves the angle required to rotate the first Vector3d structure into the specified second Vector3d structure.
		/// </summary>
		/// <param name="vector1">First structure to evaluate.</param>
		/// <param name="vector2">Second structure to evaluate.</param>
		/// <returns>Angle in degrees required to rotate vector1 in vector2.</returns>
		double angleBetween(Eigen::Vector3d vector1, Eigen::Vector3d vector2);
	}
}

#endif // RESCAN_TOOLS_H
#ifndef RESCAN_TOOLS_H
#define RESCAN_TOOLS_H

#include "Base3D.h"
#include "Plan2D.h"
#include "PreScanMode.h"

#include <string>
#include <iostream>
#include <vector>
#include <Eigen/Dense>

namespace ReScan
{
	namespace Tools
	{
		int plan2DToString(const Plan2D& plan2D, std::string& result);
		int stringToPlan2D(const std::string& planStr, Plan2D& plan2D);

		int preScanModeToString(const PreScan::PreScanMode& mode, std::string& result, bool shortName=false);
		int stringToPreScanMode(const std::string& modeStr, PreScan::PreScanMode& mode);

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
		/// Try to clamp the value d to v1, or v2, or v3, or v4 or v5.
		/// </summary>
		/// <param name="d">The value to clamp.</param>
		/// <param name="v1">First clamped value.</param>
		/// <param name="v2">Second clamped value.</param>
		/// <param name="v3">Third clamped value.</param>
		/// <param name="v4">Fourth clamped value.</param>
		/// <param name="v5">Fiveth clamped value.</param>
		/// <returns>Return vi if d is in ]vi-0.001 ; vi+0.001[ else return d.</returns>
		double clampv5(double d, double v1, double v2, double v3, double v4, double v5);

		/// <summary>
		/// Try to clamp each values to 0, -1 or 1 if they are equal to one of them more or less 0.001
		/// </summary>
		/// <param name="matrix">The matrix to clamp</param>
		void clampMatrix(Eigen::Matrix3d& matrix);

		/// <summary>
		/// Try to clamp each values to 0, -1 or 1 if they are equal to one of them more or less 0.001
		/// </summary>
		/// <param name="vector">The vector to clamp</param>
		void clampVector(Eigen::Vector3d& vector);


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

		/// <summary>
		/// Express the coordinates of base 1 in base 2.
		/// </summary>
		/// <param name="base1">Base 1</param>
		/// <param name="base2">Base 2</param>
		/// <param name="base1InBase2">Base 1 expressed in base 2</param>
		/// <returns>Returns SUCCESS_CODE (0) or NO_MATRIX_INVERSE_ERROR_CODE if the invert matrix can't be calculated.</returns>
		int getBase1IntoBase2(const Base3D& base1, const Base3D& base2, Base3D* base1InBase2);

		/// <summary>
		/// Calculates ABC operational coordinates according to the Euler ZYX convention, adapted for KUKA robots.
		/// </summary>
		/// <param name="matrix">Rotation matrix</param>
		/// <param name="a">Result of angle A in degrees.</param>
		/// <param name="b">Result of angle B in degrees.</param>
		/// <param name="c">Result of angle C in degrees.</param>
		void computeABC(const Eigen::Matrix3d& matrix, double* a, double* b, double* c);

		/// <summary>
		/// Calculates ABC operational coordinates according to the Euler ZYX convention, adapted for KUKA robots.
		/// </summary>
		/// <param name="matrix">Rotation matrix</param>
		/// <param name="a">Result of angle A in degrees.</param>
		/// <param name="b">Result of angle B in degrees.</param>
		/// <param name="c">Result of angle C in degrees.</param>
		void computeABC(const Eigen::Matrix4d& matrix, double* a, double* b, double* c);

		std::vector<std::string> splitString(const std::string& input, const std::string& delimiter);
	}
}

#endif // RESCAN_TOOLS_H
#include "ScatterGraph.h"
#include "tools.h"
#include "Axis.h"
#include "MultiOStream.h"

#include <iostream>   // for reading / writing files
#include <fstream>    // for reading / writing files
#include <sstream>    // for reading / writing files
#include <stdexcept>
#include <numbers>    // std::numbers::pi
#include <cstdlib>    // for rand()
#include <ctime>      // for srand()
#include <cmath>      // for trigo functions
#include <functional> // for findMin & findMax
#include <algorithm>  // for findMin & findMax
#include <limits>	  // for DBL_MAX, NaN
#include <Eigen/Dense>

using namespace std;
using namespace Eigen;

namespace ReScan
{
#pragma region Constructor / Destructor

	ScatterGraph::ScatterGraph() :
		m_points()
	{
	}

	ScatterGraph::ScatterGraph(const ScatterGraph& scatterGraph) :
		m_points()
	{
		copy(scatterGraph, *this);
	}

	ScatterGraph::ScatterGraph(const size_t size) :
		m_points(size)
	{
	}

	ScatterGraph::ScatterGraph(const vector<Point3D>* pointsList) :
		m_points(pointsList->size())
	{
		for (size_t i = 0; i < pointsList->size(); i++)
		{
			m_points[i] = &pointsList->at(i);
		}
	}

	ScatterGraph::ScatterGraph(const vector<const Point3D*>* pointsList) :
		m_points(pointsList->size())
	{
		for (size_t i = 0; i < pointsList->size(); i++)
		{
			m_points[i] = pointsList->at(i);
		}
	}

	ScatterGraph::ScatterGraph(const Point3D pointsArray[], const size_t size) :
		m_points(size)
	{
		for (size_t i = 0; i < size; i++)
		{
			m_points[i] = &pointsArray[i];
		}
	}

	ScatterGraph::ScatterGraph(const Point3D* pointsArray[], const size_t size) :
		m_points(size)
	{
		for (size_t i = 0; i < size; i++)
		{
			m_points[i] = pointsArray[i];
		}
	}

	ScatterGraph::~ScatterGraph()
	{
	}

#pragma endregion

	void ScatterGraph::addPoint(const Point3D* point)
	{
		m_points.push_back(point);
	}

	Point3D* ScatterGraph::addPoint(const double x, const double y, const double z)
	{
		Point3D* point = new Point3D(x, y, z);
		if (point)
		{
			m_points.push_back(point);
			return point;
		}
		else
		{
			ReScan::mout << "Memory allocation error" << endl;
			return nullptr;
		}
	}

	size_t ScatterGraph::size() const
	{
		return m_points.size();
	}

	const Point3D* ScatterGraph::at(const size_t pos) const
	{
		return m_points.at(pos);
	}

	void ScatterGraph::clear(const bool freeMemory)
	{
		if (freeMemory)
		{
			size_t size = m_points.size();
			for (size_t i = 0; i < size; i++)
			{
				delete m_points[i];
				m_points[i] = nullptr;
			}
		}
		m_points.clear();
	}

	void ScatterGraph::reducePercent(const double percent)
	{
		if (percent > 0.0 && percent <= 100.0)
		{
			if (percent == 100.0)
			{
				m_points.clear();
			}
			else
			{
				int size = (int)m_points.size();
				int newSize = (int)(size * (1.0 - (percent / 100.0)));
				double inc = (double)size / (double)newSize;
				vector<const Point3D*> taken(newSize);
				int i;
				double j = 0.0;
				for (i = 0; i < newSize; i++)
				{
					taken.at(i) = m_points.at((int)j);
					j += inc;
				}
				m_points.clear();
				m_points.assign(newSize, nullptr);
				for (i = 0; i < newSize; i++)
				{
					m_points.at(i) = taken.at(i);
				}
			}
		}
		else
		{
			mout << "reducePercent aborted: invalid percent value given: " << percent << ". Shoud be in ]0.0;100.0]" << endl;
		}
	}

	void ScatterGraph::reduce(const size_t skipped)
	{
		size_t size = m_points.size();

		if (skipped >= 2 && skipped <= size)
		{
			size_t newSize = size / skipped;
			if (newSize * skipped != size)
			{
				newSize++;
			}
			vector<const Point3D*> taken = vector<const Point3D*>(newSize);

			size_t i;
			size_t ti = 0;
			size_t j = skipped;
			for (i = 0; i < size; i++)
			{
				if (j == skipped)
				{
					taken[ti] = m_points[i];
					ti++;
					j = 1;
				}
				else
				{
					delete m_points[i];
					m_points[i] = nullptr;
					j++;
				}
			}
			m_points.clear();
			m_points.assign(newSize, nullptr);
			for (i = 0; i < ti; i++)
			{
				m_points[i] = taken[i];
			}
		}
		else
		{
			mout << "reduce aborted: invalid skipped value given: " << skipped << ". Shoud be in [2;" << size << "]" << endl;
		}
	}

	ScatterGraph* ScatterGraph::getReducedPercent(const double percent)
	{
		ScatterGraph* graph = new ScatterGraph(*this);
		if (!graph)
		{
			ReScan::mout << "Memory allocation error" << endl;
			return nullptr;
		}
		graph->reducePercent(percent);
		return graph;
	}

	ScatterGraph* ScatterGraph::getReduced(const unsigned int skipped)
	{
		ScatterGraph* graph = new ScatterGraph(*this);
		if (!graph)
		{
			ReScan::mout << "Memory allocation error" << endl;
			return nullptr;
		}
		graph->reduce(skipped);
		return graph;
	}

#pragma region Static functions

	void ScatterGraph::copy(const ScatterGraph& source, ScatterGraph& dest)
	{
		size_t size = source.size();
		for (size_t i = 0; i < size; i++)
		{
			const Point3D* p = source.at(i);
			dest.addPoint(p->getX(), p->getY(), p->getZ());
		}
	}

#pragma region Populate

	void ScatterGraph::populateFromVectorXYZ(const std::vector<float>* vect, ScatterGraph& scatterGraph)
	{
		int size = (int)vect->size();
		for (int i = 0; i < size; i += 3)
		{
			scatterGraph.addPoint(vect->at(i), vect->at(i + 1), vect->at(i + 2));
		}
	}

	void ScatterGraph::populateWithRandomPoints(ScatterGraph& scatterGraph, int count, double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
	{
		srand(static_cast<unsigned int>(time(nullptr))); // Initialisation de la graine du générateur de nombres aléatoires
		for (int i = 0; i < count; i++)
		{
			double randomX = minX + static_cast<double>(rand()) / static_cast<double>(RAND_MAX / (maxX - minX));
			double randomY = minY + static_cast<double>(rand()) / static_cast<double>(RAND_MAX / (maxY - minY));
			double randomZ = minZ + static_cast<double>(rand()) / static_cast<double>(RAND_MAX / (maxZ - minZ));
			if (!scatterGraph.addPoint(randomX, randomY, randomZ))
			{
				return;
			}
		}
	}

	void ScatterGraph::populateRectangle2D(ScatterGraph& scatterGraph, Plan2D plane, const Point3D& center, double width, double height, int numPoints)
	{
		// Vérification du nombre de points minimum
		if (numPoints < 4) {
			cerr << "Le nombre de points doit être au moins 4." << std::endl;
			return;
		}

		// Calcul des coins du rectangle
		double halfWidth = width / 2.0f;
		double halfHeight = height / 2.0f;

		switch (plane) {
		case Plan2D::XY:
			// Ajout des points sur le plan XY

			for (int i = 0; i < numPoints; ++i)
			{
				double x = center.getX() - halfWidth + (i % 2) * width;
				double y = center.getY() - halfHeight + (i / 2) * height;
				double z = center.getZ();
				if (!scatterGraph.addPoint(x, y, z))
				{
					return;
				}
			}
			break;

		case Plan2D::XZ:
			// Ajout des points sur le plan XZ
			for (int i = 0; i < numPoints; ++i) {
				double x = center.getX() - halfWidth + (i % 2) * width;
				double y = center.getY();
				double z = center.getZ() - halfHeight + (i / 2) * height;
				if (!scatterGraph.addPoint(x, y, z))
				{
					return;
				}
			}
			break;

		case Plan2D::YZ:
			// Ajout des points sur le plan YZ
			for (int i = 0; i < numPoints; ++i) {
				double x = center.getX();
				double y = center.getY() - halfWidth + (i % 2) * width;
				double z = center.getZ() - halfHeight + (i / 2) * height;
				if (!scatterGraph.addPoint(x, y, z))
				{
					return;
				}
			}
			break;

		default:
			ReScan::mout << "Plan non reconnu." << std::endl;
			break;
		}
	}

	void ScatterGraph::populateRectangle3D(ScatterGraph& scatterGraph, Plan2D plane, const Point3D& center, double width, double height, double depth, int numPoints)
	{
		// Vérification du nombre de points minimum
		if (numPoints < 8) {
			ReScan::mout << "Le nombre de points doit être au moins 8." << std::endl;
			return;
		}

		// Calcul des coins du rectangle
		double halfWidth = width / 2.0f;
		double halfHeight = height / 2.0f;
		double halfDepth = depth / 2.0f;

		switch (plane) {
		case Plan2D::XY:
			// Ajout des points sur le plan XY
			for (int i = 0; i < numPoints; ++i) {
				double x = center.getX() - halfWidth + (i % 2) * width;
				double y = center.getY() - halfHeight + (i / 2) * height;
				double z = center.getZ() + halfDepth;
				if (!scatterGraph.addPoint(x, y, z))
				{
					return;
				}
			}
			break;

		case Plan2D::XZ:
			// Ajout des points sur le plan XZ
			for (int i = 0; i < numPoints; ++i) {
				double x = center.getX() - halfWidth + (i % 2) * width;
				double y = center.getY() + halfDepth;
				double z = center.getZ() - halfHeight + (i / 2) * height;
				if (!scatterGraph.addPoint(x, y, z))
				{
					return;
				}
			}
			break;

		case Plan2D::YZ:
			// Ajout des points sur le plan YZ
			for (int i = 0; i < numPoints; ++i) {
				double x = center.getX() + halfDepth;
				double y = center.getY() - halfWidth + (i % 2) * width;
				double z = center.getZ() - halfHeight + (i / 2) * height;
				if (!scatterGraph.addPoint(x, y, z))
				{
					return;
				}
			}
			break;

		default:
			ReScan::mout << "Plan non reconnu." << std::endl;
			break;
		}
	}

	void ScatterGraph::populateDisk(ScatterGraph& scatterGraph, Plan2D plane, const Point3D& center, double radius, int numPoints)
	{
		// Vérification du nombre de points minimum
		if (numPoints < 4) {
			ReScan::mout << "Le nombre de points doit être au moins 4." << std::endl;
			return;
		}

		const double pi = EIGEN_PI;

		switch (plane) {
		case Plan2D::XY:
			// Ajout des points sur le plan XY
			for (int i = 0; i < numPoints; ++i) {
				double angle = static_cast<double>(i) * 2.0f * pi / static_cast<double>(numPoints);
				double x = center.getX() + radius * std::cos(angle);
				double y = center.getY() + radius * std::sin(angle);
				double z = center.getZ();
				if (!scatterGraph.addPoint(x, y, z))
				{
					return;
				}
			}
			break;

		case Plan2D::XZ:
			// Ajout des points sur le plan XZ
			for (int i = 0; i < numPoints; ++i) {
				double angle = static_cast<double>(i) * 2.0f * pi / static_cast<double>(numPoints);
				double x = center.getX() + radius * std::cos(angle);
				double y = center.getY();
				double z = center.getZ() + radius * std::sin(angle);
				if (!scatterGraph.addPoint(x, y, z))
				{
					return;
				}
			}
			break;

		case Plan2D::YZ:
			// Ajout des points sur le plan YZ
			for (int i = 0; i < numPoints; ++i) {
				double angle = static_cast<double>(i) * 2.0f * pi / static_cast<double>(numPoints);
				double x = center.getX();
				double y = center.getY() + radius * std::cos(angle);
				double z = center.getZ() + radius * std::sin(angle);
				if (!scatterGraph.addPoint(x, y, z))
				{
					return;
				}
			}
			break;

		default:
			ReScan::mout << "Plan non reconnu." << std::endl;
			break;
		}
	}

#pragma endregion

#pragma region Save & Read

	bool ScatterGraph::saveCSV(const std::string& filename, const ScatterGraph& scatterGraph, const bool replaceIfFileExists, const bool writeHeaders, const bool decimalCharIsDot)
	{
		// Vérifier si le nom de fichier se termine par ".csv"
		if (filename.length() < 4 || filename.substr(filename.length() - 4) != ".csv")
		{
			ReScan::mout << "File is not a .csv" << std::endl;
			return false;
		}

		std::ifstream fileExists(filename);
		if (fileExists && !replaceIfFileExists)
		{
			ReScan::mout << "The file " + filename + " already exists." << std::endl;
			fileExists.close();
			return false;
		}
		fileExists.close();

		std::ofstream outputFile(filename);

		if (!outputFile.is_open())
		{
			ReScan::mout << "Cannot open : " + filename << std::endl;
			return false;
		}

		if (writeHeaders)
		{
			outputFile << "x;y;z" << std::endl;
		}

		std::string xStr;
		std::string yStr;
		std::string zStr;

		for (const Point3D* point : scatterGraph.m_points)
		{
			xStr = std::to_string(point->getX());
			yStr = std::to_string(point->getY());
			zStr = std::to_string(point->getZ());

			if (decimalCharIsDot)
			{
				Tools::strReplace(xStr, ',', '.');
				Tools::strReplace(yStr, ',', '.');
				Tools::strReplace(zStr, ',', '.');
			}
			else
			{
				Tools::strReplace(xStr, '.', ',');
				Tools::strReplace(yStr, '.', ',');
				Tools::strReplace(zStr, '.', ',');
			}

			outputFile << xStr << ";" << yStr << ";" << zStr << std::endl;
		}

		outputFile.close();

		ReScan::mout << "Data saved into : " << filename << std::endl;

		return true;
	}

	bool ScatterGraph::readCSV(const std::string& filename, ScatterGraph* scatterGraph, bool containsHeader)
	{
		// Vérifier si le nom de fichier se termine par ".csv"
		if (filename.length() < 4 || filename.substr(filename.length() - 4) != ".csv")
		{
			ReScan::mout << "File is not a .csv" << std::endl;
			return false;
		}

		int lineN = 0;
		std::ifstream inputFile(filename);

		if (!inputFile.is_open())
		{
			ReScan::mout << "Cannot open : " + filename << std::endl;
			return false;
		}

		std::string line;
		if (containsHeader)
		{
			// read first line to ignore it
			std::getline(inputFile, line);
			lineN++;
		}

		while (std::getline(inputFile, line))
		{
			lineN++;
			std::stringstream lineStream(line);
			std::string cell;
			std::vector<double> rowData;

			// on lit chaque cellule (qui sont séparés par des virugles) afin de récupérer x, y et z dans le tableau rowData
			while (std::getline(lineStream, cell, ';'))
			{
				Tools::strReplace(cell, ',', '.');
				try
				{
					// Convertir la cellule en double
					double cellValue = std::stod(cell);
					rowData.push_back(cellValue);
				}
				catch (const std::exception& e)
				{
					// Gérer les erreurs de conversion
					ReScan::mout << "Cannot parse the cell in double - Line : " << lineN << " - Cell : " << cell << " - " << e.what() << std::endl;
					return false;
				}
			}

			// Vérifier si le nombre de colonnes est valide (3 pour Point3D)
			if (rowData.size() != 3)
			{
				ReScan::mout << "File doesn't contain the expected number of columns (3). Line : " + lineN << std::endl;
				return false;
			}

			if (!scatterGraph->addPoint(rowData[0], rowData[1], rowData[2]))
			{
				return false;
			}
		}

		// Fermer le fichier
		inputFile.close();

		return true;
	}

#pragma endregion

#pragma region Compute

	int ScatterGraph::findMax(const ScatterGraph& scatterGraph, double (Point3D::* getter)() const)
	{
		auto maxValueElement = std::max_element(scatterGraph.m_points.begin(), scatterGraph.m_points.end(), [getter](const Point3D* p1, const Point3D* p2) {
			return (p1->*getter)() < (p2->*getter)();
			});
		return (int)std::distance(scatterGraph.m_points.begin(), maxValueElement);
	}

	int ScatterGraph::findMin(const ScatterGraph& scatterGraph, double (Point3D::* getter)() const)
	{
		auto maxValueElement = std::max_element(scatterGraph.m_points.begin(), scatterGraph.m_points.end(), [getter](const Point3D* p1, const Point3D* p2) {
			return (p1->*getter)() > (p2->*getter)();
			});
		return (int)std::distance(scatterGraph.m_points.begin(), maxValueElement);
	}

	void ScatterGraph::findExtrema(const ScatterGraph& scatterGraph, const Plan2D& plan, double (Point3D::* getters[2])() const, Point3D* minPoint, Point3D* maxPoint)
	{
		// Tableau pour les 4 index des points extremas (min, min, max, max)
		int extremas[4] = { 0 };

		// Trouver les index des extremas avec les getters séléctionnés : [min1, max1, min2, max2]
		extremas[0] = findMin(scatterGraph, getters[0]);
		extremas[1] = findMin(scatterGraph, getters[1]);
		extremas[2] = findMax(scatterGraph, getters[0]);
		extremas[3] = findMax(scatterGraph, getters[1]);

		// Fill min and max points
		switch (plan)
		{
		case Plan2D::XY:
			minPoint->setX(scatterGraph.at(extremas[0])->getX());
			minPoint->setY(scatterGraph.at(extremas[1])->getY());
			minPoint->setZ(0.0);
			maxPoint->setX(scatterGraph.m_points.at(extremas[2])->getX());
			maxPoint->setY(scatterGraph.m_points.at(extremas[3])->getY());
			maxPoint->setZ(0.0);
			break;

		case Plan2D::XZ:
			minPoint->setX(scatterGraph.at(extremas[0])->getX());
			minPoint->setY(0.0);
			minPoint->setZ(scatterGraph.at(extremas[1])->getZ());
			maxPoint->setX(scatterGraph.at(extremas[2])->getX());
			maxPoint->setY(0.0);
			maxPoint->setZ(scatterGraph.at(extremas[3])->getZ());
			break;

		case Plan2D::YZ:
			minPoint->setX(0.0);
			minPoint->setY(scatterGraph.at(extremas[0])->getY());
			minPoint->setZ(scatterGraph.at(extremas[1])->getZ());
			maxPoint->setX(0.0);
			maxPoint->setY(scatterGraph.at(extremas[2])->getY());
			maxPoint->setZ(scatterGraph.at(extremas[3])->getZ());
			break;

		default:
			ReScan::mout << "Unexpected plan." << std::endl;
			break;
		}
	}

	const Point3D* ScatterGraph::getClosestPoint(const ScatterGraph& scatterGraph, const Point3D& point)
	{
		int size = (int)scatterGraph.size();
		const Point3D* closestPoint = &point;
		const Point3D* currentPoint;
		double minDistance = DBL_MAX;
		double currentDistance;
		for (int i = 0; i < size; i++)
		{
			currentPoint = scatterGraph.m_points[i];
			currentDistance = Point3D::distanceBetween(point, *currentPoint);
			if (currentDistance < minDistance)
			{
				minDistance = currentDistance;
				closestPoint = currentPoint;
			}
		}
		return closestPoint;
	}

	const Point3D* ScatterGraph::getFarthestPoint(const ScatterGraph& scatterGraph, const Point3D& point)
	{
		int size = (int)scatterGraph.size();
		const Point3D* farthestPoint = &point;
		const Point3D* currentPoint;
		double maxDistance = DBL_MIN;
		double currentDistance;
		for (int i = 0; i < size; i++)
		{
			currentPoint = scatterGraph.m_points[i];
			currentDistance = Point3D::distanceBetween(point, *currentPoint);
			if (currentDistance > maxDistance)
			{
				maxDistance = currentDistance;
				farthestPoint = currentPoint;
			}
		}
		return farthestPoint;
	}

	void ScatterGraph::computeBarycenter(const ScatterGraph& scatterGraph, Point3D* barycenter)
	{
		size_t size = scatterGraph.size();
		double x = 0.0;
		double y = 0.0;
		double z = 0.0;

		double sumX = 0.0;
		double sumY = 0.0;
		double sumZ = 0.0;

		if (size != 0)
		{

			size_t i = 0;
			double dsize = (double)size;
			const Point3D* point;

			while (i < size)
			{
				point = scatterGraph.at(i);

				sumX += point->getX();
				sumY += point->getY();
				sumZ += point->getZ();
				i++;
			}

			x = sumX / dsize;
			y = sumY / dsize;
			z = sumZ / dsize;
		}
		barycenter->setXYZ(x, y, z);
	}

	void ScatterGraph::computeAveragePlan(const ScatterGraph& scatterGraph, Plan* averagePlan)
	{
		// based on: https://www.claudeparisel.com/monwiki/data/Karnak/K2/PLAN%20MOYEN.pdf

		size_t size = scatterGraph.size();
		Point3D barycenter;
		computeBarycenter(scatterGraph, &barycenter);

		if (size < 2)
		{
			averagePlan->setABCD(0.0, 0.0, 1.0, -barycenter.getZ());
			if (averagePlan->getD() == -0.0)
			{
				averagePlan->setD(0.0);
			}
		}
		else if (ScatterGraph::arePointsCoplanar(scatterGraph))
		{
			const Point3D* p0 = scatterGraph.at(0);
			const Point3D* p1 = scatterGraph.at(1);
			Vector3d x = *p1 - *p0;
			Vector3d x2;
			p1->getDiff(*p0, &x2);
			Vector3d z;
			if (ScatterGraph::arePointsColinear(scatterGraph))
			{
				Base3D repere = Base3D::computeOrientedBase(x, Axis::X);
				const Vector3d* rZ = repere.getZ();
				z[0] = (*rZ)[0];
				z[1] = (*rZ)[1];
				z[2] = (*rZ)[2];
			}
			else
			{
				Vector3d y;
				size_t indexBestAngle = 2;
				double bestAngle = 0.0;
				double currentAngle;

				for (size_t i = 2; i < size; i++)
				{
					y = *scatterGraph.at(i) - *p0;
					currentAngle = ReScan::Tools::angleBetween(x, y);
					if (abs(90.0 - currentAngle) < abs(90.0 - bestAngle))
					{
						indexBestAngle = i;
					}
					//if (!ReScan::Tools::areVectorsColinear(x, y))
					//{
					//	break;
					//}
				}
				y = *scatterGraph.at(indexBestAngle) - *p0;
				z = x.cross(y);
			}
			if (z.z() == -0.0)
			{
				z.z() = 0.0;
			}
			if (z.z() < 0.0)
			{
				z *= -1.0;
			}
			if (z.x() == -0.0)
			{
				z.x() = 0.0;
			}
			if (z.y() == -0.0)
			{
				z.y() = 0.0;
			}
			z.normalize();
			averagePlan->setABCD(z.x(), z.y(), z.z(), -(z.x() * barycenter.getX() + z.y() * barycenter.getY() + z.z() * barycenter.getZ()));
			if (averagePlan->getD() == -0.0)
			{
				averagePlan->setD(0.0);
			}
		}
		else
		{
			double sX = 0.0;
			double sXX = 0.0;
			double sXY = 0.0;
			double sXZ = 0.0;

			double sY = 0.0;
			//double sYX = 0.0; // meme chose que sXY
			double sYY = 0.0;
			double sYZ = 0.0;

			double sZ = 0.0;
			//double sZX = 0.0; // meme chose que sXZ
			//double sZY = 0.0;   // meme chose que sYZ
			double sZZ = 0.0;

			double D;
			double k = 1.0;

			const Point3D* point;
			double pX;
			double pY;
			double pZ;

			double a = 0.0;
			double b = 0.0;
			double c = 0.0;

			// calcul des sommmes
			for (size_t i = 0; i < size; i++)
			{
				point = scatterGraph.at(i);
				pX = point->getX() / 1000.0;
				pY = point->getY() / 1000.0;
				pZ = point->getZ() / 1000.0;

				sX += pX;
				sXX += pX * pX;
				sXY += pX * pY;
				sXZ += pX * pZ;

				sY += pY;
				sYY += pY * pY;
				sYZ += pY * pZ;

				sZ += pZ;
				sZZ += pZ * pZ;
			}

			D = (sXX * sYY * sZZ) - (sXX * sYZ * sYZ) - (sYY * sXZ * sXZ) + (2.0 * sXY * sXZ * sYZ);

			a = (sX * (sYY * sZZ - sYZ * sYZ)) - (sY * (sXY * sZZ - sXZ * sYZ)) + (sZ * (sXY * sYZ - sYY * sXZ));
			b = (sX * (sXY * sZZ - sXZ * sYZ)) - (sY * (sXX * sZZ - sXZ * sXZ)) + (sZ * (sYZ * sXX - sXY * sXZ));
			c = (sX * (sXY * sYZ - sYY * sXZ)) - (sY * (sXX * sYZ - sXY * sXZ)) + (sZ * (sXX * sYY - sXY * sXY));

			a *= (-k / D);
			b *= (k / D);
			c *= (-k / D);

			if (c == -0.0)
			{
				c = 0.0;
			}
			if (c < 0)
			{
				a *= -1.0;
				b *= -1.0;
				c *= -1.0;
			}
			if (a == -0.0)
			{
				a = 0.0;
			}
			if (b == -0.0)
			{
				b = 0.0;
			}
			Vector3d z(a, b, c);
			if (z.norm() == 0.0)
			{
				z[2] = 1.0;
			}
			else
			{
				z.normalize();
			}

			averagePlan->setABCD(z.x(), z.y(), z.z(), -(z.x() * barycenter.getX() + z.y() * barycenter.getY() + z.z() * barycenter.getZ()));
			if (averagePlan->getD() == -0.0)
			{
				averagePlan->setD(0.0);
			}
		}
	}

	bool ScatterGraph::arePointsCoplanar(const ScatterGraph& scatterGraph)
	{
		size_t size = scatterGraph.size();
		if (size <= 3)
		{
			return true;
		}
		const Point3D* p0 = scatterGraph.at(0);
		Vector3d vector1 = *scatterGraph.at(1) - *p0;
		Vector3d vector2;
		Vector3d vector3;
		bool hasFoundVector2 = false;
		bool isCoplanar = true;

		for (size_t i = 2; i < size; i++)
		{
			if (!hasFoundVector2)
			{
				vector2 = *scatterGraph.at(i) - *p0;
				hasFoundVector2 = !(ReScan::Tools::areVectorsColinear(vector1, vector2));
			}
			else
			{
				vector3 = *scatterGraph.at(i) - *p0;
				isCoplanar = abs(ReScan::Tools::mixteProduct(vector1, vector2, vector3)) < ZERO_CLAMP;
				if (!isCoplanar)
				{
					break;
				}
			}
		}

		return isCoplanar;
	}

	bool ScatterGraph::arePointsColinear(const ScatterGraph& scatterGraph)
	{
		size_t size = scatterGraph.size();
		if (size <= 2)
		{
			return true;
		}

		const Point3D* p0 = scatterGraph.at(0);
		Vector3d vector1 = *scatterGraph.at(1) - *p0;
		for (int i = 2; i < size; i++)
		{
			if (!ReScan::Tools::areVectorsColinear(vector1, *scatterGraph.at(i) - *p0))
			{
				return false;
			}
		}
		return true;
	}

	void ScatterGraph::computeBase3D(const ScatterGraph& scatterGraph, Base3D* base3D)
	{
		Point3D barycenter;
		Plan averagePlan;
		computeBarycenter(scatterGraph, &barycenter);
		computeAveragePlan(scatterGraph, &averagePlan);
		computeBase3D(scatterGraph, barycenter, averagePlan, base3D);
	}

	void ScatterGraph::computeBase3D(const ScatterGraph& scatterGraph, const Point3D& origin, const Plan& averagePlan, Base3D* base3D)
	{
		base3D->reset();
		base3D->setOrigin(origin);

		if (scatterGraph.m_points.size() > 1)
		{
			Vector3d z;
			averagePlan.getNormal(z);

			if (ScatterGraph::arePointsColinear(scatterGraph))
			{
				Vector3d x = *scatterGraph.m_points[1] - *scatterGraph.m_points[0];
				x.normalize();
				Vector3d y = z.cross(x);
				y.normalize();

				base3D->setXYZ(x, y, z);
			}
			else
			{
				base3D->setFrom(Base3D::computeOrientedBase(z, Axis::Z), false);
			}
		}
	}

	int ScatterGraph::fixBase3D(const Base3D& baseReference, Base3D* base3D)
	{
		double angle = Tools::angleBetween(*base3D->getZ(), *baseReference.getZ());
		if (angle > 90.0)
		{
			// Mettre le Z dans la bonne direction
			base3D->rotate(*base3D->getY(), 180.0);
		}

		angle = Tools::angleBetween(*base3D->getX(), *baseReference.getX());
		if (angle > 90.0)
		{
			// Mettre le X dans la bonne direction
			base3D->rotate(*base3D->getZ(), 180.0);

			// remarque : techniquement on ne devrait pas a avoir le faire
			// mais cas particulier fait que une base avait son X parfaitement à 180° de X de reference
			// donc patch
			// de plus, ca replace les x entre [-90;90] au lieu de [-180;180] donc ça aide (enormement) pour l'algorithme suivant
			// donc pas plus mal de le faire
		}

		int result = getAngleToAlignXAxis(*base3D, baseReference, &angle);
		if (result == SUCCESS_CODE)
		{
			base3D->rotate(*base3D->getZ(), angle);
		}

		return result;
	}

	int ScatterGraph::getAngleToAlignXAxis(const Base3D& base1, const Base3D& base2, double* angle)
	{
		Base3D base3DInPartBase;
		int result = Tools::getBase1IntoBase2(base1, base2, &base3DInPartBase);
		if (result == SUCCESS_CODE)
		{
			/*
			 * La but est de trouver un angle qui fait que la rotation de la base autour de son Z (par cette angle)
			 * fait que le nouveau vecteur X est applatit dans le plan XZ
			 * (donc le vecteur engendre par X(1,0,0) ^ X'(xx',xy',xz') appartient au plan XY et on en deduit que xy' = 0)
			 * On cherche dont l'angle a tq xy' = 0
			 *
			 * Pour le trouver on fait B'=R*B (B' base rotate, B base normal, R rotation)
			 * On souhaite faire la rotation autour de l'axe Z de la base donc on construit la matrice de rotation selon un axe U (zx, zy, zz) d'angle a.
			 * Comme on cherche que xy', pas besoin de faire tous la rotation, on peut faire les calculs a la main juste pour avoir xy'.
			 * On trouve : xy' = k + k1 * cos(a) + k2 * sin(a) => 2 "inconnus" pour 1 equation
			 *
			 * Algo pas optimise car l'angle est teste par incrementation
			 */

			Eigen::Vector3d x = *base3DInPartBase.getX();
			Eigen::Vector3d z = *base3DInPartBase.getZ();

			double xx = x[0];
			double xy = x[1];
			double xz = x[2];

			double ux = z[0];
			double uy = z[1];
			double uz = z[2];

			double uy2 = uy * uy;

			double k0 = xx * ux * uy + xy * uy2 + xz * uy * uz;
			double k1 = xy - xx * ux * uy - xy * uy2 - xz * uy * uz;
			double k2 = xx * uz - xz * ux;

			double epsilon = 0.01;

			// valeur de l'angle teste
			double a = 0.0;

			// nouvelle valeur de la composante Y du vecteur X lorsqu'il a ete tourne par a autour de son Z. On veut cette valeur le plus proche de 0.
			double newXY = k0 + k1;

			double step = EIGEN_PI / 360.0;

			double minNewXY = newXY;
			double minA = 0.0;

			if (newXY > 0.0)
			{
				// algo version angle positif
				while (newXY > epsilon)
				{
					a -= step;
					newXY = k0 + cos(a) * k1 + sin(a) * k2;  // calcul de la nouvelle composante Y du vecteur X apres rotation
					if (newXY < 0.0)
					{
						a += step;      // go back to last pos
						step /= 10.0;   // reduce step to be more precise
					}

					/*
					* patch dans le cas ou il arriverait pas a atteindre le 0 (cas assez rare mais possible..)
					* ne devrait plus arriver grace au flip du vecteur X (voir un cran au dessus dans la pile d'appel)
					* mais ca mange pas de pain de le laisser au cas ou...
					*/

					if (newXY < minNewXY)
					{
						minNewXY = newXY;
						minA = a;           // on enregistre la valeur de a qui permet d'avoir un newXY minimal
					}
					if (a <= -360.0)        // patch dans le cas où il arriverait pas a atteindre le 0 et qu'il vient de faire un tour complet
					{
						a = minA;
						break;
					}
				}
			}
			else
			{
				epsilon = -epsilon;
				// algo version angle negatif
				while (newXY < epsilon)
				{
					a += step;
					newXY = k0 + cos(a) * k1 + sin(a) * k2;
					if (newXY > 0.0)
					{
						a -= step;      // go back to last pos
						step /= 10.0;   // reduce step
					}
					if (newXY < minNewXY)
					{
						minNewXY = newXY;
						minA = a;
					}
					if (a >= 360.0)		// patch dans le cas ou il arriverait pas a atteindre le 0 et qu'il vient de faire un tour complet
					{
						a = minA;
						break;
					}
				}
			}
			*angle = Tools::r2d(a);
		}

		return result;
	}

#pragma endregion

#pragma endregion
}

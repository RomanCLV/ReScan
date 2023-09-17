#include "ScatterGraph.h"
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

using namespace std;

#pragma region Constructor / Destructor

ScatterGraph::ScatterGraph() :
	points()
{
}

ScatterGraph::ScatterGraph(const ScatterGraph& scatterGraph) :
	points(scatterGraph.points)
{
}

ScatterGraph::ScatterGraph(const size_t size) :
	points(size)
{
}

ScatterGraph::ScatterGraph(const vector<Point3D>* pointsList) :
	points(pointsList->size())
{
	for (int i = 0; i < pointsList->size(); i++)
	{
		points[i] = &pointsList->at(i);
	}
}

ScatterGraph::ScatterGraph(const vector<const Point3D*>* pointsList) :
	points(pointsList->size())
{
	for (int i = 0; i < pointsList->size(); i++)
	{
		points[i] = pointsList->at(i);
	}
}

ScatterGraph::ScatterGraph(const Point3D pointsArray[], const size_t size) :
	points(size)
{
	for (int i = 0; i < size; i++)
	{
		points[i] = &pointsArray[i];
	}
}

ScatterGraph::ScatterGraph(const Point3D* pointsArray[], const size_t size) :
	points(size)
{
	for (int i = 0; i < size; i++)
	{
		points[i] = pointsArray[i];
	}
}

ScatterGraph::~ScatterGraph()
{
}

#pragma endregion

void ScatterGraph::addPoint(const Point3D* point)
{
	points.push_back(point);
}

Point3D* ScatterGraph::addPoint(const double x, const double y, const double z)
{
	Point3D* point = new Point3D(x, y, z);
	if (point)
	{
		points.push_back(point);
		return point;
	}
	else
	{
		std::cerr << "Memory allocation error" << endl;
		return nullptr;
	}
}

size_t ScatterGraph::size() const
{
	return points.size();
}

const Point3D* ScatterGraph::at(const size_t pos) const
{
	return points.at(pos);
}

void ScatterGraph::clear()
{
	points.clear();
}

void ScatterGraph::reducePercent(const double percent)
{
	if (percent > 0.0 && percent <= 100.0)
	{
		if (percent == 100.0)
		{
			points.clear();
		}
		else
		{
			int size = (int)points.size();
			int newSize = (int)(size * (1.0 - (percent / 100.0)));
			double inc = (double)size / (double)newSize;
			vector<const Point3D*> taken(newSize);
			int i;
			double j = 0.0;
			for (i = 0; i < newSize; i++)
			{
				taken.at(i) = points.at((int)j);
				j += inc;
			}
			points.clear();
			points.assign(newSize, nullptr);
			for (i = 0; i < newSize; i++)
			{
				points.at(i) = taken.at(i);
			}
		}
	}
}

void ScatterGraph::reduce(const unsigned int skipped)
{
	int size = (int)points.size();

	if (skipped >= (unsigned int)size)
	{
		points.clear();
	}
	else
	{
		int newSize = size / skipped;
		vector<const Point3D*> taken(newSize);
		int i;
		int j = 0;
		for (i = 0; i < size; i += skipped)
		{
			taken.at(j++) = points.at(i);
		}
		points.clear();
		points.assign(newSize, nullptr);
		for (i = 0; i < newSize; i++)
		{
			points.at(i) = taken.at(i);
		}
	}
}

ScatterGraph* ScatterGraph::getReducedPercent(const double percent)
{
	ScatterGraph* graph = new ScatterGraph(*this);
	if (!graph)
	{
		std::cerr << "Memory allocation error" << endl;
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
		std::cerr << "Memory allocation error" << endl;
		return nullptr;
	}
	graph->reduce(skipped);
	return graph;
}

#pragma region Static functions

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
		std::cerr << "Plan non reconnu." << std::endl;
		break;
	}
}

void ScatterGraph::populateRectangle3D(ScatterGraph& scatterGraph, Plan2D plane, const Point3D& center, double width, double height, double depth, int numPoints)
{
	// Vérification du nombre de points minimum
	if (numPoints < 8) {
		std::cerr << "Le nombre de points doit être au moins 8." << std::endl;
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
		std::cerr << "Plan non reconnu." << std::endl;
		break;
	}
}

void ScatterGraph::populateDisk(ScatterGraph& scatterGraph, Plan2D plane, const Point3D& center, double radius, int numPoints)
{
	// Vérification du nombre de points minimum
	if (numPoints < 4) {
		std::cerr << "Le nombre de points doit être au moins 4." << std::endl;
		return;
	}

	const double pi = std::numbers::pi; // Utilisation de std::numbers::pi

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
		std::cerr << "Plan non reconnu." << std::endl;
		break;
	}
}

#pragma endregion

#pragma region Save & Read

bool ScatterGraph::saveCSV(const std::string& filename, const ScatterGraph& scatterGraph, bool replaceIfFileExists, bool writeHeaders)
{
	// Vérifier si le nom de fichier se termine par ".csv"
	if (filename.length() < 4 || filename.substr(filename.length() - 4) != ".csv")
	{
		std::cerr << "File is not a .csv" << std::endl;
		return false;
	}

	std::ifstream fileExists(filename);
	if (fileExists && !replaceIfFileExists)
	{
		std::cerr << "The file " + filename + " already exists." << std::endl;
		fileExists.close();
		return false;
	}
	fileExists.close();

	std::ofstream outputFile(filename);

	if (!outputFile.is_open())
	{
		std::cerr << "Cannot open : " + filename << std::endl;
		return false;
	}

	if (writeHeaders)
	{
		outputFile << "x;y;z" << std::endl;
	}

	for (const Point3D* point : scatterGraph.points)
	{
		outputFile << point->getX() << ";" << point->getY() << ";" << point->getZ() << std::endl;
	}

	outputFile.close();

	std::cout << "Data saved into : " << filename << std::endl;

	return true;
}

bool ScatterGraph::readCSV(const std::string& filename, ScatterGraph* scatterGraph, bool containsHeader)
{
	// Vérifier si le nom de fichier se termine par ".csv"
	if (filename.length() < 4 || filename.substr(filename.length() - 4) != ".csv")
	{
		std::cerr << "File is not a .csv" << std::endl;
		return false;
	}

	int lineN = 0;
	std::ifstream inputFile(filename);

	if (!inputFile.is_open())
	{
		std::cerr << "Cannot open : " + filename << std::endl;
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
			try
			{
				// Convertir la cellule en double
				double cellValue = std::stod(cell);
				rowData.push_back(cellValue);
			}
			catch (const std::exception& e)
			{
				// Gérer les erreurs de conversion
				std::cerr << "Cannot parse the cell in double - Line : " << lineN << " - Cell : " << cell << " - " << e.what() << std::endl;
				return false;
			}
		}

		// Vérifier si le nombre de colonnes est valide (3 pour Point3D)
		if (rowData.size() != 3)
		{
			std::cerr << "File doesn't contain the expected number of columns (3). Line : " + lineN << std::endl;
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
	auto maxValueElement = std::max_element(scatterGraph.points.begin(), scatterGraph.points.end(), [getter](const Point3D* p1, const Point3D* p2) {
		return (p1->*getter)() < (p2->*getter)();
		});
	return (int)std::distance(scatterGraph.points.begin(), maxValueElement);
}

int ScatterGraph::findMin(const ScatterGraph& scatterGraph, double (Point3D::* getter)() const)
{
	auto maxValueElement = std::max_element(scatterGraph.points.begin(), scatterGraph.points.end(), [getter](const Point3D* p1, const Point3D* p2) {
		return (p1->*getter)() > (p2->*getter)();
		});
	return (int)std::distance(scatterGraph.points.begin(), maxValueElement);
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
		maxPoint->setX(scatterGraph.points.at(extremas[2])->getX());
		maxPoint->setY(scatterGraph.points.at(extremas[3])->getY());
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
		std::cerr << "Unexpected plan." << std::endl;
		break;
	}
}

void ScatterGraph::computeBarycenter(const ScatterGraph& scatterGraph, Point3D& barycenter)
{
	double x = 0.0f;
	double y = 0.0f;
	double z = 0.0f;
	int size = static_cast<int>(scatterGraph.size());
	double sized = (double)size;
	const Point3D* point;

	// methode 1: sum all then divide

	for (int i = 0; i < size; i++)
	{
		point = scatterGraph.at(i);
		x += point->getX();
		y += point->getY();
		z += point->getZ();
	}
	barycenter.setXYZ(x / sized, y / sized, z / sized);

	// methode 2 :
	// calcul des moyennes en "continu" pour éviter d'atteindre les valeurs hors mémoire
	// si on fait d'abord la somme de tous les x, y et z

	//for (int i = 0; i < size; i++)
	//{
	//	point = scatterGraph.at(i);
	//	x = (i * x + point->getX()) / (i + 1);
	//	y = (i * y + point->getY()) / (i + 1);
	//	z = (i * z + point->getZ()) / (i + 1);
	//}
	//barycenter.setXYZ(x, y, z);
}

void ScatterGraph::computeAveragePlan(const ScatterGraph& scatterGraph, Plan& averagePlan)
{
	// based on: https://www.claudeparisel.com/monwiki/data/Karnak/K2/PLAN%20MOYEN.pdf

	int size = (int)scatterGraph.size();

	if (size < 3)
	{
		std::cerr << "Need at least 3 points to compute the average plan." << std::endl;
		return;
	}

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

	Point3D barycenter;
	double a;
	double b;
	double c;

	// calcul des sommmes
	for (int i = 0; i < size; i++)
	{
		point = scatterGraph.at(i);
		pX = point->getX();
		pY = point->getY();
		pZ = point->getZ();

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

	D = sXX * sYY * sZZ - sXX * sYZ * sYZ - sYY * sXZ * sXZ + 2 * sXY * sXZ * sYZ;
	
	a = sX * (sYY * sZZ - sYZ * sYZ);
	a -= sY * (sXY * sZZ - sXZ * sYZ);
	a += sZ * (sXY * sYZ - sYY * sXZ);
	a *= (-k / D);

	b = sX * (sXY * sZZ - sXZ * sYZ);
	b -= sY * (sXX * sZZ - sXZ * sXZ);
	b += sZ * (sYZ * sXX - sXY * sXZ);
	b *= (k / D);

	c = sX * (sXY * sYZ - sYY * sXZ);
	c -= sY * (sXX * sZZ - sXZ * sXZ);
	c += sZ * (sXX * sYY - sXY * sXY);
	c *= (-k / D);

	computeBarycenter(scatterGraph, barycenter);

	averagePlan.setABCD(a, b, c, -(a * barycenter.getX() + b * barycenter.getY() + c * barycenter.getZ()));
}

#pragma endregion

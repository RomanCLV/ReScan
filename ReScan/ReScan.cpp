#include "ReScan.h"
#include "Tools.h"

#include <iostream>   // for reading / writing files
#include <fstream>    // for reading / writing files
#include <sstream>    // for reading / writing files
#include <ctime>	  // for getDate()
#include <iomanip>	  // for getDate()

using namespace std;

namespace ReScan
{
#pragma region Constructors & Desctructor

	ReScan::ReScan(const std::string& fileName) :
		m_fileName(fileName),
		m_plan2D(nullptr),
		m_stepAxis1(nullptr),
		m_stepAxis2(nullptr)
	{
	}

	ReScan::ReScan(const ReScan& reScan) :
		m_fileName(reScan.m_fileName),
		m_plan2D(nullptr),
		m_stepAxis1(nullptr),
		m_stepAxis2(nullptr)

	{
		if (reScan.m_plan2D)
		{
			m_plan2D = new Plan2D(*reScan.m_plan2D);
		}
		if (reScan.m_stepAxis1)
		{
			m_stepAxis1 = new unsigned int(*reScan.m_stepAxis1);
		}
		if (reScan.m_stepAxis2)
		{
			m_stepAxis2 = new unsigned int(*reScan.m_stepAxis2);
		}
	}

	// desconstructor

	ReScan::~ReScan()
	{
		delete m_plan2D;
		m_plan2D = nullptr;

		delete m_stepAxis1;
		m_stepAxis1 = nullptr;

		delete m_stepAxis2;
		m_stepAxis2 = nullptr;
	}

#pragma endregion

#pragma region private functions

	Plan2D ReScan::selectPlan2D() const
	{
		int choice;

		// Choix des axes
		do
		{
			cout << endl << "Find extrema:" << endl;
			cout << "1) XY" << endl;
			cout << "2) XZ" << endl;
			cout << "3) YZ" << endl << endl;

			cin >> choice;

			if (choice < 1 || choice > 3) {
				cout << "Invalide choice." << endl;
			}

		} while (choice < 1 || choice > 3);

		switch (choice)
		{
		case 1:
			cout << "XY axes selected" << endl << endl;
			return Plan2D::XY;

		case 2:
			cout << "XZ axes selected" << endl << endl;
			return Plan2D::XZ;

		case 3:
			cout << "YZ axes selected" << endl << endl;
			return Plan2D::YZ;

		default:
			std::cerr << "Unexpected plan choice." << std::endl;
			exit(-1);
			break;
		}
	}

	double ReScan::getDistance1D(const Point3D& point1, const Point3D& point2, double (Point3D::* getter)() const) const
	{
		double x1 = (point1.*getter)();
		double x2 = (point2.*getter)();
		return abs(x2 - x1);
	}

	void ReScan::getDistances(Point3D const& minPoint, Point3D const& maxPoint, double (Point3D::* getters[2])() const, double& distance1, double& distance2) const
	{
		distance1 = getDistance1D(minPoint, maxPoint, getters[0]);
		distance2 = getDistance1D(minPoint, maxPoint, getters[1]);
	}

	unsigned int ReScan::selectStep(char axisName, unsigned int min, unsigned int max) const
	{
		unsigned int step;
		do
		{
			cout << endl << "Select the " << axisName << " axis step between " << to_string(min) << " mm and " << to_string(max) << " mm" << endl << endl;
			cin >> step;
		} while (step < min || step > max);
		return step;
	}

	unsigned int ReScan::getSubDivision(double distance, int step) const
	{
		unsigned int div = (unsigned int)(abs(distance) / step);
		if ((double)(div * step) < distance)
		{
			div++;
		}
		return div;
	}

	void ReScan::fillSubDivisions(const Point3D& minPoint, const ScatterGraph& graph, std::vector<ScatterGraph>* subGraph,
		double (Point3D::* getters[2])() const,
		const unsigned int step1, const unsigned int step2, const unsigned int subDivision1, const unsigned int subDivision2, const bool deleteIfEmpty) const
	{
		const Point3D* currentPoint;
		double minX;
		double minY;
		double x;
		double y;
		double dx;
		double dy;
		unsigned int divX;
		unsigned int divY;
		int divIndex;
		int pointsSize = (int)graph.size();

		minX = (minPoint.*getters[0])();
		minY = (minPoint.*getters[1])();

		double totalSubDivision = subDivision1 * subDivision2;

		subGraph->clear();
		for (int i = 0; i < totalSubDivision; i++)
		{
			subGraph->push_back(ScatterGraph());
		}

		for (int i = 0; i < pointsSize; i++)
		{
			currentPoint = graph.at(i);
			x = (currentPoint->*getters[0])();
			y = (currentPoint->*getters[1])();

			dx = abs(x - minX);
			dy = abs(y - minY);

			divX = (unsigned int)(dx / step1);
			divY = (unsigned int)(dy / step2);

			divIndex = divY * subDivision1 + divX;
			(subGraph->at(divIndex)).addPoint(currentPoint);
		}

		if (deleteIfEmpty)
		{
			int removed = 0;
			for (int i = (int)subGraph->size() - 1; i >= 0; i--)
			{
				if (subGraph->at(i).size() == 0)
				{
					subGraph->erase(subGraph->begin() + i);
					removed++;
				}
			}
			cout << removed << " removed empty graph(s)" << std::endl;
		}
	}

	bool ReScan::isFileValid() const
	{
		std::ifstream fileExists(m_fileName);
		if (!fileExists)
		{
			std::cerr << "File: " << m_fileName << " not found." << std::endl;
			return false;
		}
		if (m_fileName.length() < 4 || m_fileName.substr(m_fileName.length() - 4) != ".obj")
		{
			std::cerr << "File is not .obj" << std::endl;
			return false;
		}
		return true;
	}

	int ReScan::internalProcess(const bool exportSubDivisions, const bool exportBasesCartesian, const bool exportBasesEulerAngles, const bool exportDetailsFile, const bool writeHeaders, const bool decimalCharIsDot)
	{
		// declarations
		vector<float> vertices;

		char axis1Name;
		char axis2Name;

		// Tableau de pointeurs de membres pour les getters désirés
		double (Point3D:: * getters[2])() const; {}

		ScatterGraph graph;

		Point3D minPoint;
		Point3D maxPoint;

		double distance1;
		double distance2;

		unsigned int subDivision1;
		unsigned int subDivision2;

		vector<ScatterGraph> subDivisions;

		// END - declarations

		if (!isFileValid())
		{
			return INVALID_FILE_ERROR_CODE;
		}
		string filenameWithoutExtention = removeFileExtension(m_fileName);

		// Read obj file
		//objio::readObjFile(m_fileName, &vertices, &triangles, &uvs, &uvtriangles);
		objio::readObjFileVertices(m_fileName, &vertices);

		// populate graph with vertex read in the file
		ScatterGraph::populateFromVectorXYZ(&vertices, graph);

		// Select plan if needed
		if (!m_plan2D)
		{
			m_plan2D = new Plan2D(selectPlan2D());
		}

		// Select getters
		switch (*m_plan2D)
		{
		case Plan2D::XY:
			axis1Name = 'X';
			axis2Name = 'Y';
			getters[0] = &Point3D::getX;
			getters[1] = &Point3D::getY;
			break;
		case Plan2D::XZ:
			axis1Name = 'X';
			axis2Name = 'Z';
			getters[0] = &Point3D::getX;
			getters[1] = &Point3D::getZ;
			break;
		case Plan2D::YZ:
			axis1Name = 'Y';
			axis2Name = 'Z';
			getters[0] = &Point3D::getY;
			getters[1] = &Point3D::getZ;
			break;
		default:
			std::cerr << "Unexpected plan." << std::endl;
			return INVALID_PLAN_ERROR_CODE;
		}

		cout << "\nSelected plan: " << axis1Name << axis2Name << std::endl;

		// Find extremas points (2 corners of the ROI)
		ScatterGraph::findExtrema(graph, *m_plan2D, getters, &minPoint, &maxPoint);

		cout << std::endl;
		cout << "min point: " << minPoint << endl;
		cout << "max point: " << maxPoint << endl;

		// Compute dimensions of the ROI
		getDistances(minPoint, maxPoint, getters, distance1, distance2);

		// display infos about distances
		cout << std::endl;
		cout << "distance " << axis1Name << ": " << distance1 << " mm" << endl;
		cout << "distance " << axis2Name << ": " << distance2 << " mm" << endl;

		// Select step of axis 1 if needed
		if (!m_stepAxis1)
		{
			m_stepAxis1 = new unsigned int(selectStep(axis1Name, 50, int(distance1)));
		}
		// Select step of axis 2 if needed
		if (!m_stepAxis2)
		{
			m_stepAxis2 = new unsigned int(selectStep(axis2Name, 50, int(distance2)));
		}

		cout << std::endl;
		cout << axis1Name << " axis step selected: " << *m_stepAxis1 << " mm" << std::endl;
		cout << axis2Name << " axis step selected: " << *m_stepAxis2 << " mm" << std::endl;

		// compute number of subdivsions on both axis
		subDivision1 = getSubDivision(distance1, *m_stepAxis1);
		subDivision2 = getSubDivision(distance2, *m_stepAxis2);

		// display infos about subdivisions
		cout << endl;
		cout << "Number of subdivisions on " << axis1Name << ": " << subDivision1 << endl;
		cout << "Number of subdivisions on " << axis2Name << ": " << subDivision2 << endl;
		cout << "Total of subdivisions: " << (subDivision1 * subDivision2) << endl << endl;

		// fill all sub graphs
		fillSubDivisions(minPoint, graph, &subDivisions, getters, *m_stepAxis1, *m_stepAxis2, subDivision1, subDivision2, false);

		// display infos about points
		cout << "Number of points in the main graph: " << graph.size() << endl << endl;
		cout << "Number of points in the sub graphes" << endl;
		unsigned int sum = 0;
		for (int i = 0; i < subDivisions.size(); i++)
		{
			cout << "Graph " << i + 1 << ": " << subDivisions[i].size() << endl;
			sum += (unsigned int)subDivisions[i].size();
		}
		cout << "Total of points: " << sum << endl;

		if (exportSubDivisions)
		{
			exportSubDivisionsToCSV(filenameWithoutExtention, subDivisions);
		}

		cout << endl << "Computing base..." << endl;

		vector<Base3D*> bases(subDivisions.size(), nullptr);

		Base3D reference = Base3D(Point3D(), Eigen::Vector3d(0.0, -1.0, 0.0), Eigen::Vector3d(0.0, 0.0, 1.0), Eigen::Vector3d(-1.0, 0.0, 0.0));
		int fixResult;
		for (int i = 0; i < subDivisions.size(); i++)
		{
			if (subDivisions[i].size() != 0)
			{
				Base3D* base = new Base3D();
				if (base == nullptr)
				{
					return MEMORY_ALLOCATION_ERROR_CODE;
				}
				ScatterGraph::computeBase3D(subDivisions[i], base);
				fixResult = ScatterGraph::fixBase3D(reference, base);
				if (fixResult == NO_MATRIX_INVERSE_ERROR_CODE)
				{
					cout << "cannot fix base " << (i + 1) << ": matrix can't be inverted" << endl;
				}
				bases[i] = base;
			}
		}

		std::string basePath = filenameWithoutExtention + "_bases_" + getDate();

		if (exportBasesCartesian)
		{
			cout << endl << "Exporting bases in cartesian..." << endl;
			string defaultNullText = "0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0";
			exportBasesCartesianToCSV(basePath + "_cartesian.csv", bases, defaultNullText, writeHeaders, decimalCharIsDot);
		}
		if (exportBasesEulerAngles)
		{
			cout << endl << "Exporting Euler angles..." << endl;
			string defaultNullText = "0.0;0.0;0.0;0.0;0.0;0.0";
			exportBasesEulerAnglesToCSV(basePath + "_euler-angles-ZYX.csv", bases, defaultNullText, writeHeaders, decimalCharIsDot);
		}
		if (exportDetailsFile)
		{
			cout << endl << "Exporting trajectory file details..." << endl;
			exportTrajectoryDetailsFile(basePath + "_details.csv", distance1, distance2, *m_stepAxis1, *m_stepAxis2, subDivision1, subDivision2, decimalCharIsDot);
		}

		for (int i = 0; i < bases.size(); i++)
		{
			if (bases[i] != nullptr)
			{
				delete bases[i];
				bases[i] = nullptr;
			}
		}

		return SUCCESS_CODE;
	}

	void ReScan::exportSubDivisionsToCSV(const std::string& basePath, const std::vector<ScatterGraph>& subDivisions) const
	{
		for (int i = 0; i < subDivisions.size(); i++)
		{
			ScatterGraph::saveCSV(basePath + to_string(i + 1) + ".csv", subDivisions[i], true, true);
		}
	}

	bool ReScan::exportBasesCartesianToCSV(const std::string& filename, const std::vector<Base3D*>& bases, const std::string& nullText, const bool writeHeaders, const bool decimalCharIsDot) const
	{
		if (filename.length() < 4 || filename.substr(filename.length() - 4) != ".csv")
		{
			std::cerr << "File is not a .csv" << std::endl;
			return false;
		}

		std::ofstream outputFile(filename);

		if (!outputFile.is_open())
		{
			std::cerr << "Cannot open: " + filename << std::endl;
			return false;
		}

		if (writeHeaders)
		{
			outputFile << "o_x;o_y;o_z;x_x;x_y;x_z;y_x;y_y;y_z;z_x;z_y;z_z" << std::endl;
		}

		std::string oxStr;
		std::string oyStr;
		std::string ozStr;
		std::string xxStr;
		std::string xyStr;
		std::string xzStr;
		std::string yxStr;
		std::string yyStr;
		std::string yzStr;
		std::string zxStr;
		std::string zyStr;
		std::string zzStr;

		const Point3D* origin;
		const Eigen::Vector3d* x;
		const Eigen::Vector3d* y;
		const Eigen::Vector3d* z;

		for (const Base3D* base3D : bases)
		{
			if (base3D == nullptr)
			{
				outputFile << nullText << std::endl;
			}
			else
			{
				origin = base3D->getOrigin();
				x = base3D->getX();
				y = base3D->getY();
				z = base3D->getZ();

				oxStr = std::to_string(origin->getX());
				oyStr = std::to_string(origin->getY());
				ozStr = std::to_string(origin->getZ());
				xxStr = std::to_string((*x)[0]);
				xyStr = std::to_string((*x)[1]);
				xzStr = std::to_string((*x)[2]);
				yxStr = std::to_string((*y)[0]);
				yyStr = std::to_string((*y)[1]);
				yzStr = std::to_string((*y)[2]);
				zxStr = std::to_string((*z)[0]);
				zyStr = std::to_string((*z)[1]);
				zzStr = std::to_string((*z)[2]);

				if (decimalCharIsDot)
				{
					Tools::strReplace(oxStr, ',', '.');
					Tools::strReplace(oyStr, ',', '.');
					Tools::strReplace(ozStr, ',', '.');
					Tools::strReplace(xxStr, ',', '.');
					Tools::strReplace(xyStr, ',', '.');
					Tools::strReplace(xzStr, ',', '.');
					Tools::strReplace(yxStr, ',', '.');
					Tools::strReplace(yyStr, ',', '.');
					Tools::strReplace(yzStr, ',', '.');
					Tools::strReplace(zxStr, ',', '.');
					Tools::strReplace(zyStr, ',', '.');
					Tools::strReplace(zzStr, ',', '.');
				}
				else
				{
					Tools::strReplace(oxStr, '.', ',');
					Tools::strReplace(oyStr, '.', ',');
					Tools::strReplace(ozStr, '.', ',');
					Tools::strReplace(xxStr, '.', ',');
					Tools::strReplace(xyStr, '.', ',');
					Tools::strReplace(xzStr, '.', ',');
					Tools::strReplace(yxStr, '.', ',');
					Tools::strReplace(yyStr, '.', ',');
					Tools::strReplace(yzStr, '.', ',');
					Tools::strReplace(zxStr, '.', ',');
					Tools::strReplace(zyStr, '.', ',');
					Tools::strReplace(zzStr, '.', ',');
				}

				outputFile \
					<< oxStr << ";" << oyStr << ";" << ozStr << ";" \
					<< xxStr << ";" << xyStr << ";" << xzStr << ";" \
					<< yxStr << ";" << yyStr << ";" << yzStr << ";" \
					<< zxStr << ";" << zyStr << ";" << zzStr << std::endl;
			}
		}

		outputFile.close();

		std::cout << "Bases saved into:" << std::endl << filename << std::endl;

		return true;
	}

	bool ReScan::exportBasesEulerAnglesToCSV(const std::string& filename, const std::vector<Base3D*>& bases, const std::string& nullText, const bool writeHeaders, const bool decimalCharIsDot) const
	{
		if (filename.length() < 4 || filename.substr(filename.length() - 4) != ".csv")
		{
			std::cerr << "File is not a .csv" << std::endl;
			return false;
		}

		std::ofstream outputFile(filename);

		if (!outputFile.is_open())
		{
			std::cerr << "Cannot open: " + filename << std::endl;
			return false;
		}

		if (writeHeaders)
		{
			outputFile << "o_x;o_y;o_z;a;b;c" << std::endl;
		}

		std::string oxStr;
		std::string oyStr;
		std::string ozStr;
		std::string rxStr;
		std::string ryStr;
		std::string rzStr;

		const Point3D* origin;

		double a;
		double b;
		double c;

		for (const Base3D* base3D : bases)
		{
			if (base3D == nullptr)
			{
				outputFile << nullText << std::endl;
			}
			else
			{
				origin = base3D->getOrigin();
				base3D->toEulerAnglesZYX(&a, &b, &c);

				oxStr = std::to_string(origin->getX());
				oyStr = std::to_string(origin->getY());
				ozStr = std::to_string(origin->getZ());
				rxStr = std::to_string(a);
				ryStr = std::to_string(b);
				rzStr = std::to_string(c);

				if (decimalCharIsDot)
				{
					Tools::strReplace(oxStr, ',', '.');
					Tools::strReplace(oyStr, ',', '.');
					Tools::strReplace(ozStr, ',', '.');
					Tools::strReplace(rxStr, ',', '.');
					Tools::strReplace(ryStr, ',', '.');
					Tools::strReplace(rzStr, ',', '.');
				}
				else
				{
					Tools::strReplace(oxStr, '.', ',');
					Tools::strReplace(oyStr, '.', ',');
					Tools::strReplace(ozStr, '.', ',');
					Tools::strReplace(rxStr, '.', ',');
					Tools::strReplace(ryStr, '.', ',');
					Tools::strReplace(rzStr, '.', ',');
				}

				outputFile \
					<< oxStr << ";" << oyStr << ";" << ozStr << ";" \
					<< rxStr << ";" << ryStr << ";" << rzStr << ";" << std::endl;
			}
		}

		outputFile.close();

		std::cout << "Euler angles saved into:" << std::endl << filename << std::endl;

		return true;
	}

	bool ReScan::exportTrajectoryDetailsFile(const std::string& filename,
		const double distance1,
		const double distance2,
		const unsigned int stepAxis1,
		const unsigned int stepAxis2,
		const unsigned int subDivision1,
		const unsigned int subDivision2,
		const bool decimalCharIsDot) const
	{
		if (filename.length() < 4 || filename.substr(filename.length() - 4) != ".csv")
		{
			std::cerr << "File is not a .csv" << std::endl;
			return false;
		}

		std::ofstream outputFile(filename);

		if (!outputFile.is_open())
		{
			std::cerr << "Cannot open: " + filename << std::endl;
			return false;
		}

		string distance1Str = to_string(distance1);
		string distance2Str = to_string(distance2);
		if (decimalCharIsDot)
		{
			Tools::strReplace(distance1Str, ',', '.');
			Tools::strReplace(distance2Str, ',', '.');
		}
		else
		{
			Tools::strReplace(distance1Str, '.', ',');
			Tools::strReplace(distance2Str, '.', ',');
		}

		outputFile << "X dimensions (mm);" << distance1Str << std::endl;
		outputFile << "Y dimensions (mm);" << distance2Str << std::endl;
		outputFile << "X step (mm);" << to_string(stepAxis1) << std::endl;
		outputFile << "Y step (mm);" << to_string(stepAxis2) << std::endl;
		outputFile << "X divisions;" << to_string(subDivision1) << std::endl;
		outputFile << "Y divisions;" << to_string(subDivision2) << std::endl;
		outputFile << "total divisions;" << to_string(subDivision1 * subDivision2) << std::endl;

		outputFile.close();

		return true;
	}


#pragma endregion

#pragma region public functions

	int ReScan::process(const bool exportSubDivisions, const bool exportBasesCartesian, const bool exportBasesEulerAngles, const bool exportDetailsFile, const bool writeHeaders, const bool decimalCharIsDot)
	{
		delete m_plan2D;
		m_plan2D = nullptr;

		delete m_stepAxis1;
		m_stepAxis1 = nullptr;

		delete m_stepAxis2;
		m_stepAxis2 = nullptr;

		return internalProcess(exportSubDivisions, exportBasesCartesian, exportBasesEulerAngles, exportDetailsFile, writeHeaders, decimalCharIsDot);
	}

	int ReScan::process(const Plan2D plan2D, const unsigned int stepAxis1, const unsigned int stepAxis2, const bool exportSubDivisions, const bool exportBasesCartesian, const bool exportBasesEulerAngles, const bool exportDetailsFile, const bool writeHeaders, const bool decimalCharIsDot)
	{
		if (!m_plan2D)
		{
			m_plan2D = new Plan2D();
		}
		if (!m_stepAxis1)
		{
			m_stepAxis1 = new unsigned int(0);
		}
		if (!m_stepAxis2)
		{
			m_stepAxis2 = new unsigned int(0);
		}
		*m_plan2D = plan2D;
		*m_stepAxis1 = stepAxis1;
		*m_stepAxis2 = stepAxis2;
		return internalProcess(exportSubDivisions, exportBasesCartesian, exportBasesEulerAngles, exportDetailsFile, writeHeaders, decimalCharIsDot);
	}

#pragma endregion
}

std::string getDate()
{
	std::time_t t = std::time(nullptr);
	std::tm now;
	//std::tm* now = std::localtime(&t);

#ifdef _WIN32
	localtime_s(&now, &t);
#else
	localtime_r(&t, &now); // Utilisation de localtime_r pour les systèmes non-Windows
#endif

	std::ostringstream oss;
	oss << std::put_time(&now, "%Y-%m-%d_%H-%M-%S");

	return oss.str();
}

std::string removeFileExtension(const std::string& fileName)
{
	// Recherchez la dernière occurrence du caractère '.' dans le nom de fichier
	size_t dotPosition = fileName.find_last_of('.');

	// Si le caractère '.' est trouvé et n'est pas le premier caractère,
	// retournez la sous-chaîne du début jusqu'au caractère '.'
	if (dotPosition != std::string::npos && dotPosition != 0)
	{
		return fileName.substr(0, dotPosition);
	}
	else
	{
		// Si aucun caractère '.' n'est trouvé ou s'il est au début du nom de fichier,
		// retournez simplement le nom de fichier original
		return fileName;
	}
}
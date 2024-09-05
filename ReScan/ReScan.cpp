#include "ReScan.h"
#include "Tools.h"

#include <iostream>   // for reading / writing files
#include <fstream>    // for reading / writing files
#include <sstream>    // for reading / writing files
#include <ctime>	  // for getDate()
#include <iomanip>	  // for getDate()
#include <algorithm>  // for transform
#include <functional>
#include <filesystem> // for create_directory

using namespace std;

namespace ReScan
{
#pragma region Constructors & Desctructor

	ReScan::ReScan() :
		m_processData(),
		m_subscribers()
	{
	}

	ReScan::ReScan(const ReScan& reScan) :
		m_processData(reScan.m_processData),
		m_subscribers()
	{
	}

	// destructor

	ReScan::~ReScan()
	{
		m_subscribers.clear();
	}

#pragma endregion

#pragma region private functions

	void ReScan::notifyObservers(const FileType fileType, const std::string& path) const
	{
		for (const auto& subscriber : m_subscribers)
		{
			subscriber(fileType, path);
		}
	}

	void ReScan::resetProcessData()
	{
		m_processData.reset();
	}

	int ReScan::selectPlan2D(Plan2D* plan2D) const
	{
		int choice;
		int result = SUCCESS_CODE;

		// Choix des axes
		do
		{
			mout << endl << "Find extrema:" << endl;
			mout << "1) XY" << endl;
			mout << "2) XZ" << endl;
			mout << "3) YZ" << endl << endl;

			std::cin >> choice;

			if (choice < 1 || choice > 3) {
				mout << "Invalide choice." << endl;
			}

		} while (choice < 1 || choice > 3);

		switch (choice)
		{
		case 1:
			mout << "XY axes selected" << endl << endl;
			*plan2D = Plan2D::XY;
			break;
		case 2:
			mout << "XZ axes selected" << endl << endl;
			*plan2D = Plan2D::XZ;
			break;

		case 3:
			mout << "YZ axes selected" << endl << endl;
			*plan2D = Plan2D::YZ;
			break;

		default:
			mout << "Unexpected plan choice." << std::endl;
			result = INVALID_PLAN_ERROR_CODE;
			break;
		}

		return result;
	}

	double ReScan::getDistance1D(const Point3D& point1, const Point3D& point2, double (Point3D::* getter)() const) const
	{
		double x1 = (point1.*getter)();
		double x2 = (point2.*getter)();
		return abs(x2 - x1);
	}

	void ReScan::getDistances(Point3D const& minPoint, Point3D const& maxPoint, double (Point3D::* getters[2])() const)
	{
		m_processData.setDistance1(getDistance1D(minPoint, maxPoint, getters[0]));
		m_processData.setDistance2(getDistance1D(minPoint, maxPoint, getters[1]));
	}

	unsigned int ReScan::selectStep(char axisName, unsigned int min, unsigned int max) const
	{
		unsigned int step;
		do
		{
			mout << endl << "Select the " << axisName << " axis step between " << to_string(min) << " mm and " << to_string(max) << " mm" << endl << endl;
			std::cin >> step;
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
		double (Point3D::* getters[2])() const, const bool deleteIfEmpty) const
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

		double totalSubDivision = m_processData.getTotalSubDivisions();

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

			divX = (unsigned int)(dx / *m_processData.getStepAxis1());
			divY = (unsigned int)(dy / *m_processData.getStepAxis2());

			divIndex = divY * m_processData.getSubDivisions1() + divX;
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
			mout << removed << " removed empty graph(s)" << std::endl;
		}
	}

	bool ReScan::fileExists(const std::string& filename) const
	{
		std::ifstream ifs(filename);
		return ifs ? true : false;
	}

	int ReScan::internalProcess()
	{
		// declarations
		const unsigned int MIN_DISTANCE = 1;

		vector<float> vertices;

		// Tableau de pointeurs de membres pour les getters désirés
		double (Point3D:: * getters[2])() const; {}

		ScatterGraph graph;

		Point3D minPoint;
		Point3D maxPoint;

		vector<ScatterGraph> subDivisions;

		// END - declarations
		string filename = m_processData.getObjFile();

		if (filename.length() == 0)
		{
			mout << "No obj file specified." << std::endl;
			return INVALID_FILE_ERROR_CODE;
		}

		if (!fileExists(filename))
		{
			mout << "File " << filename << " not found." << std::endl;
			return FILE_NOT_FOUND_ERROR_CODE;
		}

		string filenameWithoutExtention = removeFileExtension(filename);

		if (isValidNameFile(filename, "obj"))
		{
			// Read obj file
			objio::readObjFileVertices(filename, &vertices);

			// populate graph with vertex read in the file
			ScatterGraph::populateFromVectorXYZ(&vertices, graph);

			// Select plan if needed
			if (!m_processData.getPlan2D())
			{
				if (m_processData.getEnableUserInput())
				{
					Plan2D plan2D;
					int result = selectPlan2D(&plan2D);
					if (result == SUCCESS_CODE)
					{
						m_processData.setPlan2D(plan2D);
					}
					else
					{
						return result;
					}
				}
				else
				{
					mout << "No selected plan." << std::endl;
					return NO_PLAN_SELECTED_ERROR_CODE;
				}
			}

			// Select getters
			switch (*m_processData.getPlan2D())
			{
			case Plan2D::XY:
				m_processData.setAxis1Name('X');
				m_processData.setAxis2Name('Y');
				getters[0] = &Point3D::getX;
				getters[1] = &Point3D::getY;
				break;
			case Plan2D::XZ:
				m_processData.setAxis1Name('X');
				m_processData.setAxis2Name('Z');
				getters[0] = &Point3D::getX;
				getters[1] = &Point3D::getZ;
				break;
			case Plan2D::YZ:
				m_processData.setAxis1Name('Y');
				m_processData.setAxis2Name('Z');
				getters[0] = &Point3D::getY;
				getters[1] = &Point3D::getZ;
				break;
			default:
				mout << "Unexpected selected plan." << std::endl;
				return INVALID_PLAN_ERROR_CODE;
			}

			mout << "\nSelected plan: " << m_processData.getAxis1Name() << m_processData.getAxis2Name() << std::endl;

			// Find extremas points (2 corners of the ROI)
			ScatterGraph::findExtrema(graph, *m_processData.getPlan2D(), getters, &minPoint, &maxPoint);

			mout << std::endl;
			mout << "min point: " << minPoint << endl;
			mout << "max point: " << maxPoint << endl;

			// Compute dimensions of the ROI
			getDistances(minPoint, maxPoint, getters);

			// display infos about distances
			mout << std::endl;
			mout << "distance " << m_processData.getAxis1Name() << ": " << m_processData.getDistance1() << " mm" << endl;
			mout << "distance " << m_processData.getAxis2Name() << ": " << m_processData.getDistance2() << " mm" << endl;

			// Select step of axis 1 if needed
			if (!m_processData.getStepAxis1())
			{
				if (m_processData.getEnableUserInput())
				{
					m_processData.setStepAxis1(selectStep(m_processData.getAxis1Name(), MIN_DISTANCE, int(m_processData.getDistance1())));
				}
				else
				{
					mout << "No (or invalid) " << m_processData.getAxis1Name() << " step axis selected." << std::endl;
					return NO_STEP_AXIS_1_SELECTED_ERROR_CODE;
				}
			}
			else if (!m_processData.isStep1Valid(MIN_DISTANCE))
			{
				mout << m_processData.getAxis1Name() << " is not between " << MIN_DISTANCE << " and " << m_processData.getDistance1() << '.' << endl;
				if (m_processData.getEnableUserInput())
				{
					m_processData.setStepAxis1(selectStep(m_processData.getAxis1Name(), MIN_DISTANCE, int(m_processData.getDistance1())));
				}
				else
				{
					mout << "No (or invalid) " << m_processData.getAxis1Name() << " step axis selected." << std::endl;
					return NO_STEP_AXIS_1_SELECTED_ERROR_CODE;
				}
			}

			// Select step of axis 2 if needed
			if (!m_processData.getStepAxis2())
			{
				if (m_processData.getEnableUserInput())
				{
					m_processData.setStepAxis2(selectStep(m_processData.getAxis2Name(), MIN_DISTANCE, int(m_processData.getDistance2())));
				}
				else
				{
					mout << "No (or invalid) " << m_processData.getAxis2Name() << " step axis selected." << std::endl;
					return NO_STEP_AXIS_2_SELECTED_ERROR_CODE;
				}
			}
			else if (!m_processData.isStep2Valid(MIN_DISTANCE))
			{
				mout << m_processData.getAxis2Name() << " is not between " << MIN_DISTANCE << " and " << m_processData.getDistance2() << '.' << endl;
				if (m_processData.getEnableUserInput())
				{
					m_processData.setStepAxis2(selectStep(m_processData.getAxis2Name(), MIN_DISTANCE, int(m_processData.getDistance2())));
				}
				else
				{
					mout << "No (or invalid) " << m_processData.getAxis2Name() << " step axis selected." << std::endl;
					return NO_STEP_AXIS_2_SELECTED_ERROR_CODE;
				}
			}

			mout << std::endl;
			mout << m_processData.getAxis1Name() << " axis step selected: " << *m_processData.getStepAxis1() << " mm" << std::endl;
			mout << m_processData.getAxis2Name() << " axis step selected: " << *m_processData.getStepAxis2() << " mm" << std::endl;

			// compute number of subdivsions on both axis
			m_processData.setSubDivision1(getSubDivision(m_processData.getDistance1(), *m_processData.getStepAxis1()));
			m_processData.setSubDivision2(getSubDivision(m_processData.getDistance2(), *m_processData.getStepAxis2()));

			// display infos about subdivisions
			mout << endl;
			mout << "Number of subdivisions on " << m_processData.getAxis1Name() << ": " << m_processData.getSubDivisions1() << endl;
			mout << "Number of subdivisions on " << m_processData.getAxis2Name() << ": " << m_processData.getSubDivisions2() << endl;
			mout << "Total of subdivisions: " << (m_processData.getTotalSubDivisions()) << endl << endl;

			// fill all sub graphs
			fillSubDivisions(minPoint, graph, &subDivisions, getters, false);

			// display infos about points
			mout << "Number of points in the main graph: " << graph.size() << endl << endl;
			mout << "Number of points in the sub graphes" << endl;
			unsigned int sum = 0;
			for (int i = 0; i < subDivisions.size(); i++)
			{
				mout << "Graph " << i + 1 << ": " << subDivisions[i].size() << endl;
				sum += (unsigned int)subDivisions[i].size();
			}
			mout << "Total of points: " << sum << endl << endl;

			if (m_processData.getExportSubDivisions())
			{
				std::string subdivisionsDir = filenameWithoutExtention + "_subdivisions";
				std::filesystem::create_directory(subdivisionsDir);

				auto splitPath = Tools::splitString(filenameWithoutExtention, "\\");
				std::string subdivionBasePath = subdivisionsDir + "\\" + splitPath[splitPath.size() - 1];

				exportSubDivisionsToCSV(subdivionBasePath, subDivisions);
				mout << std::endl;
			}
		}
		else if (isValidNameFile(filename, "csv"))
		{
			ScatterGraph g;
			if (ScatterGraph::readCSV(filename, &g))
			{
				subDivisions.push_back(g);
			}
			else
			{
				mout << "Error while reading " << filename << std::endl;
				return READ_CSV_ERROR_CODE;
			}
		}
		else
		{
			return INVALID_FILE_ERROR_CODE;
		}

		mout << "Computing base..." << endl;

		vector<Base3D*> bases(subDivisions.size(), nullptr);

		Base3D refBase = Base3D(*m_processData.getReferenceBase());
		const Eigen::Vector3d refZ = *(refBase.getZ());
		const Eigen::Vector3d refX = *(refBase.getX());

		bool isNotBaseIdentity = !refBase.isIdentity();

		int fixResult;
		for (int i = 0; i < subDivisions.size(); i++)
		{
			if (subDivisions[i].size() != 0)
			{
				Base3D* base = new Base3D();
				if (base == nullptr)
				{
					mout << "Memory allocation error." << endl;
					return MEMORY_ALLOCATION_ERROR_CODE;
				}
				ScatterGraph::computeBase3D(subDivisions[i], base);
				if (isNotBaseIdentity)
				{
					fixResult = ScatterGraph::fixBase3D(refBase, base);
					Eigen::Vector3d baseZ = *(base->getZ());
					Eigen::Vector3d baseX = *(base->getX());

					double angleZ = Tools::angleBetween(refZ, baseZ);
					double angleX = Tools::angleBetween(refX, baseX);

					if (fixResult == NO_MATRIX_INVERSE_ERROR_CODE)
					{
						mout << "cannot fix base " << (i + 1) << ": matrix can't be inverted" << endl;
					}
					if (angleZ >= 90.0 || angleX >= 90.0)
					{
						mout << "WARNING: base " << (i + 1) << " has a wrong correction: z angle diff.: " << angleZ << "°   |   x angle diff.: " << angleX << "°" << std::endl;
					}
				}
				bases[i] = base;
			}
		}

		std::string basePath = filenameWithoutExtention + "_bases_" + getDate();

		if (m_processData.getExportBasesCartesian())
		{
			std::string path = basePath + "_cartesian.csv";
			std::string defaultFileName = m_processData.getBasesCartesianDefaultFileName();
			if (defaultFileName.size() != 0)
			{
				if (isValidNameFile(defaultFileName, "csv"))
				{
					path = defaultFileName;
				}
				else
				{
					mout << "Default file name for cartesian bases invalid." << std::endl;
				}
			}
			mout << endl << "Exporting bases (cartesian)..." << endl;
			if (exportBasesCartesianToCSV(path, bases, "0;0;0;0;0;0;0;0;0;0;0;0"))
			{
				mout << "Bases (cartesian) saved into:" << std::endl << path << std::endl;
				notifyObservers(FileType::BasesCartesian, path);
			}
			else
			{
				mout << "Bases (cartesian) not saved." << std::endl;
			}
		}

		if (m_processData.getExportBasesEulerAngles())
		{
			std::string path = basePath + "_euler-angles-ZYX.csv";
			std::string defaultFileName = m_processData.getBasesEulerAnglesDefaultFileName();
			if (defaultFileName.size() != 0)
			{
				if (isValidNameFile(defaultFileName, "csv"))
				{
					path = defaultFileName;
				}
				else
				{
					mout << "Default file name for Euler angles bases invalid." << std::endl;
				}
			}
			mout << endl << "Exporting bases (Euler angles)..." << endl;
			if (exportBasesEulerAnglesToCSV(path, bases, "0;0;0;0;0;0"))
			{
				mout << "Bases (Euler angles) saved into:" << std::endl << path << std::endl;
				notifyObservers(FileType::BasesEulerAngles, path);
			}
			else
			{
				mout << "Bases (Euler angles) not saved." << std::endl;
			}
		}

		if (m_processData.getExportDetailsFile())
		{
			std::string path = basePath + "_details.csv";
			std::string defaultFileName = m_processData.getDetailsDefaultFileName();
			if (defaultFileName.size() != 0)
			{
				if (isValidNameFile(defaultFileName, "csv"))
				{
					path = defaultFileName;
				}
				else
				{
					mout << "Default file name for details invalid." << std::endl;
				}
			}
			mout << endl << "Exporting trajectory details file..." << endl;
			if (exportTrajectoryDetailsFile(path))
			{
				mout << "Details saved into:" << std::endl << path << std::endl;
				notifyObservers(FileType::BasesDetails, path);
			}
			else
			{
				mout << "Details not saved." << std::endl;
			}
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
			const std::string path = basePath + "-" + to_string(i + 1) + ".csv";
			if (ScatterGraph::saveCSV(path, subDivisions[i], true, m_processData.getWriteHeaders(), m_processData.getDecimalCharIsDot()))
			{
				notifyObservers(FileType::Subdivision, path);
			}
		}
	}

	bool ReScan::exportBasesCartesianToCSV(const std::string& filename, const std::vector<Base3D*>& bases, const std::string& nullText) const
	{
		if (!isValidNameFile(filename, "csv"))
		{
			return false;
		}

		std::ofstream outputFile(filename);

		if (!outputFile.is_open())
		{
			mout << "Cannot open: " + filename << std::endl;
			return false;
		}

		if (m_processData.getWriteHeaders())
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

				if (m_processData.getDecimalCharIsDot())
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
		return true;
	}

	bool ReScan::exportBasesEulerAnglesToCSV(const std::string& filename, const std::vector<Base3D*>& bases, const std::string& nullText) const
	{
		if (!isValidNameFile(filename, "csv"))
		{
			return false;
		}

		std::ofstream outputFile(filename);

		if (!outputFile.is_open())
		{
			mout << "Cannot open: " + filename << std::endl;
			return false;
		}

		if (m_processData.getWriteHeaders())
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

				if (m_processData.getDecimalCharIsDot())
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
					<< rxStr << ";" << ryStr << ";" << rzStr << std::endl;
			}
		}

		outputFile.close();
		return true;
	}

	bool ReScan::exportTrajectoryDetailsFile(const std::string& filename) const
	{
		if (!isValidNameFile(filename, "csv"))
		{
			return false;
		}

		std::ofstream outputFile(filename);

		if (!outputFile.is_open())
		{
			mout << "Cannot open: " + filename << std::endl;
			return false;
		}

		string distance1Str = to_string(m_processData.getDistance1());
		string distance2Str = to_string(m_processData.getDistance2());
		if (m_processData.getDecimalCharIsDot())
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
		outputFile << "X step (mm);" << to_string(*m_processData.getStepAxis1()) << std::endl;
		outputFile << "Y step (mm);" << to_string(*m_processData.getStepAxis2()) << std::endl;
		outputFile << "X divisions;" << to_string(m_processData.getSubDivisions1()) << std::endl;
		outputFile << "Y divisions;" << to_string(m_processData.getSubDivisions2()) << std::endl;
		outputFile << "total divisions;" << to_string(m_processData.getTotalSubDivisions()) << std::endl;

		outputFile.close();
		return true;
	}

#pragma endregion

#pragma region public functions

	void ReScan::subscribe(EventCallback callback)
	{
		m_subscribers.push_back(callback);
	}

	void ReScan::unsubscribe(EventCallback callback)
	{
		auto it = std::find_if(m_subscribers.begin(), m_subscribers.end(),
			[callback](const EventCallback& cb) { return cb.target<void(const std::string&)>() == callback.target<void(const std::string&)>(); });

		if (it != m_subscribers.end())
		{
			m_subscribers.erase(it);
		}
	}

	int ReScan::process(std::string& configFile)
	{
		int result;

		resetProcessData();

		ReScanConfig config;
		result = ReScanConfig::loadConfigFromFile(configFile, &config);

		if (result == SUCCESS_CODE)
		{
			m_processData.setFromConfig(config);
			result = internalProcess();
		}
		else if (result == FILE_NOT_FOUND_ERROR_CODE || result == READ_CONFIG_ERROR_CODE || result == SET_CONFIG_ERROR_CODE)
		{
			if (!m_processData.getEnableUserInput())
			{
				return result;
			}
			if (configFile.size() == 0)
			{
				configFile = "config.ini";
			}
			if (!configFile.ends_with(".ini"))
			{
				configFile += ".ini";
			}

			mout << "Would you like to create a new config file (" << configFile << ") ? " << std::endl;
			mout << "0: No" << std::endl;
			mout << "1: Create a new config file" << std::endl;
			mout << "2: Create a new config file adapated for ICNDE (frontal) and use it" << std::endl;
			mout << "3: Create a new config file adapated for ICNDE (lateral) and use it" << std::endl;

			int choice;
			do
			{
				std::cin >> choice;
				if (choice < 0 || choice > 3)
				{
					mout << "Invalide choice." << endl;
				}
			} while (choice < 0 || choice > 3);

			if (choice == 1)
			{
				ReScanConfig::saveConfigToFile(ReScanConfig(), configFile);
				mout << "You now have to edit this new file to set the .obj file." << std::endl;
			}
			else if (choice == 2 || choice == 3)
			{
				config = choice == 2 ?
					ReScanConfig::createFrontalICNDEConfig() :
					ReScanConfig::createLateralICNDEConfig();

				ReScanConfig::saveConfigToFile(config, configFile);
				m_processData.setFromConfig(config);
				result = internalProcess();
			}
		}
		return result;
	}

	int ReScan::process(const ReScanConfig& config)
	{
		m_processData.setFromConfig(config);
		return internalProcess();
	}

	int ReScan::process(const std::string& objFile, const bool exportSubDivisions, const bool exportBasesCartesian, const bool exportBasesEulerAngles, const bool exportDetailsFile, const bool writeHeaders, const bool decimalCharIsDot)
	{
		resetProcessData();
		m_processData.setObjFile(objFile);
		m_processData.setExportSubDivisions(exportSubDivisions);
		m_processData.setExportBasesCartesian(exportBasesCartesian);
		m_processData.setExportBasesEulerAngles(exportBasesEulerAngles);
		m_processData.setExportDetailsFile(exportDetailsFile);
		m_processData.setWriteHeaders(writeHeaders);
		m_processData.setDecimalCharIsDot(decimalCharIsDot);
		return internalProcess();
	}

	int ReScan::process(const std::string& objFile, const Plan2D plan2D, const unsigned int stepAxis1, const unsigned int stepAxis2, const bool exportSubDivisions, const bool exportBasesCartesian, const bool exportBasesEulerAngles, const bool exportDetailsFile, const bool writeHeaders, const bool decimalCharIsDot)
	{
		resetProcessData();
		m_processData.setObjFile(objFile);
		m_processData.setPlan2D(plan2D);
		m_processData.setStepAxis1(stepAxis1);
		m_processData.setStepAxis2(stepAxis2);
		m_processData.setExportSubDivisions(exportSubDivisions);
		m_processData.setExportBasesCartesian(exportBasesCartesian);
		m_processData.setExportBasesEulerAngles(exportBasesEulerAngles);
		m_processData.setExportDetailsFile(exportDetailsFile);
		m_processData.setWriteHeaders(writeHeaders);
		m_processData.setDecimalCharIsDot(decimalCharIsDot);
		return internalProcess();
	}

	bool ReScan::isValidNameFile(const std::string& filename, const std::string& extention)
	{
		size_t extentionLength = extention.length();
		std::string extentionLower = extention;

		std::transform(extentionLower.begin(), extentionLower.end(), extentionLower.begin(), ::tolower);

		if (filename.length() - extentionLength - 1 <= 0)
		{
			mout << "Filename is empty." << std::endl;
			return false;
		}
		std::string filenameExtentionLower = filename.substr(filename.length() - extentionLength);
		std::transform(filenameExtentionLower.begin(), filenameExtentionLower.end(), filenameExtentionLower.begin(), ::tolower);

		if (filenameExtentionLower != extentionLower)
		{
			mout << "File " << filename << " is not ." << extentionLower << std::endl;
			return false;
		}
		return true;
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

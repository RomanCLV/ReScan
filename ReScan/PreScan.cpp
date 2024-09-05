#include "PreScan.h"
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

namespace ReScan::PreScan
{
#pragma region Constructors & Desctructor

	PreScan::PreScan() :
		m_processData(),
		m_subscribers()
	{
	}

	PreScan::PreScan(const PreScan& preScan) :
		m_processData(preScan.m_processData),
		m_subscribers()
	{
	}

	// destructor

	PreScan::~PreScan()
	{
		m_subscribers.clear();
	}

#pragma endregion

#pragma region private functions
	void PreScan::notifyObservers(const FileType fileType, const std::string& path) const
	{
		for (const auto& subscriber : m_subscribers)
		{
			subscriber(fileType, path);
		}
	}

	void PreScan::resetProcessData()
	{
		m_processData.reset();
	}

	void PreScan::selectPoint(std::string name, Point3D* point) const
	{
		mout << endl << "Input coordinates of " << name << " in mm:" << endl << endl;
		point->setX((double)selectPointValue('X'));
		point->setY((double)selectPointValue('Y'));
		point->setZ((double)selectPointValue('Z'));
		mout << endl;
	}

	int PreScan::selectPointValue(char axisName) const
	{
		int step;
		mout << endl << axisName << ": ";
		std::cin >> step;
		return step;
	}

	unsigned int PreScan::selectStep(std::string axisName, unsigned int min, unsigned int max) const
	{
		unsigned int step;
		do
		{
			mout << endl << "Select the " << axisName << " axis step between " << to_string(min) << " mm and " << to_string(max) << " mm" << endl << endl;
			std::cin >> step;
		} while (step < min || step > max);
		return step;
	}

	unsigned int PreScan::getPointsNumber(double distance, int step) const
	{
		unsigned int div = (unsigned int)(abs(distance) / step);
		if ((double)(div * step) < distance)
		{
			div++;
		}
		return div;
	}

	bool PreScan::fileExists(const std::string& filename) const
	{
		std::ifstream ifs(filename);
		return ifs ? true : false;
	}

	void PreScan::fillBases(std::vector<Base3D*>* bases) const
	{

	}

	int PreScan::internalProcess() 
	{
		const unsigned int MIN_DISTANCE = 1;

		// Select point1 if needed
		if (!m_processData.getPoint1())
		{
			if (m_processData.getEnableUserInput())
			{
				Point3D point1;
				selectPoint("Point 1", &point1);
				m_processData.setPoint1(&point1);
			}
			else
			{
				return NO_POINT_1_SELECTED_ERROR_CODE;
			}
		}

		// Select point2 if needed
		if (!m_processData.getPoint2())
		{
			if (m_processData.getEnableUserInput())
			{
				Point3D point2;
				selectPoint("Point 2", &point2);
				m_processData.setPoint2(&point2);
			}
			else
			{
				return NO_POINT_2_SELECTED_ERROR_CODE;
			}
		}

		// Select planOffset if needed
		if (m_processData.getPlanOffset() == nullptr)
		{
			if (m_processData.getEnableUserInput())
			{
				int planOffset;
				mout << "Input plan offset in mm: ";
				std::cin >> planOffset;
				m_processData.setPlanOffset(planOffset);
			}
			else
			{
				return NO_PLAN_OFFSET_SELECTED_ERROR_CODE;
			}
		}

		const Point3D* processP1 = m_processData.getPoint1();
		const Point3D* processP2 = m_processData.getPoint2();

		mout << std::endl;
		mout << "point1: " << *processP1 << endl;
		mout << "point2: " << *processP2 << endl;

		// if points are on the same height
		if (processP1->getZ() == processP2->getZ())
		{
			mout << endl << "Points musts have different height." << endl;
			return IDENTICAL_POINTS_HEIGHT;
		}

		// swap point if needed
		if (processP1->getZ() > processP2->getZ())
		{
			mout << "Points swaped because point 1 was higher than point 2." << endl;
			Point3D tmp = Point3D(*processP2);
			m_processData.setPoint2(processP1);
			m_processData.setPoint1(&tmp);
			processP1 = m_processData.getPoint1();
			processP2 = m_processData.getPoint2();
			mout << std::endl;
			mout << "point 1: " << *processP1 << endl;
			mout << "point 2: " << *processP2 << endl;
		}

		mout << "plan offset: " << m_processData.getPlanOffset() << endl;

		m_processData.setDistanceXY(sqrt(pow((double)processP2->getX() - processP1->getX(), 2) + pow((double)processP2->getY() - processP1->getY(), 2)));
		m_processData.setDistanceZ(m_processData.getPoint2()->getZ() - m_processData.getPoint1()->getZ());

		// display infos about distances
		mout << std::endl;
		mout << "distance XY: " << m_processData.getDistanceXY() << " mm" << endl;
		mout << "distance Z : " << m_processData.getDistanceZ() << " mm" << endl;

		// Select step of axis 1 if needed
		if (!m_processData.getStepAxisXY())
		{
			if (m_processData.getEnableUserInput())
			{
				m_processData.setStepAxisXY(selectStep("XY", MIN_DISTANCE, int(m_processData.getDistanceXY())));
			}
			else
			{
				mout << "No (or invalid) XY step axis selected" << std::endl;
				return NO_STEP_AXIS_1_SELECTED_ERROR_CODE;
			}
		}
		else if (!m_processData.isStepXYValid(MIN_DISTANCE))
		{
			mout << "XY step is not between " << MIN_DISTANCE << " and " << m_processData.getDistanceXY() << endl;
			if (m_processData.getEnableUserInput())
			{
				m_processData.setStepAxisXY(selectStep("XY", MIN_DISTANCE, int(m_processData.getDistanceXY())));
			}
			else
			{
				mout << "No (or invalid) XY step axis selected" << std::endl;
				return NO_STEP_AXIS_1_SELECTED_ERROR_CODE;
			}
		}

		// Select step of axis 2 if needed
		if (!m_processData.getStepAxisZ())
		{
			if (m_processData.getEnableUserInput())
			{
				m_processData.setStepAxisZ(selectStep("Z", MIN_DISTANCE, int(m_processData.getDistanceZ())));
			}
			else
			{
				mout << "No (or invalid) Z step axis selected" << std::endl;
				return NO_STEP_AXIS_2_SELECTED_ERROR_CODE;
			}
		}
		else if (!m_processData.isStepZValid(MIN_DISTANCE))
		{
			mout << "Z is not between " << MIN_DISTANCE << " and " << m_processData.getDistanceZ() << endl;
			if (m_processData.getEnableUserInput())
			{
				m_processData.setStepAxisZ(selectStep("Z", MIN_DISTANCE, int(m_processData.getDistanceZ())));
			}
			else
			{
				mout << "No (or invalid) Z step axis selected" << std::endl;
				return NO_STEP_AXIS_2_SELECTED_ERROR_CODE;
			}
		}

		mout << std::endl;
		mout << "XY axis step selected: " << *m_processData.getStepAxisXY() << " mm" << std::endl;
		mout << "Z  axis step selected: " << *m_processData.getStepAxisZ() << " mm" << std::endl;

		// compute number of subdivsions on both axis
		m_processData.setPointsNumberXY(getPointsNumber(m_processData.getDistanceXY(), *m_processData.getStepAxisXY()));
		m_processData.setPointsNumberZ(getPointsNumber(m_processData.getDistanceZ(), *m_processData.getStepAxisZ()));

		// display infos about subdivisions
		mout << endl;
		mout << "Number of points on XY: " << m_processData.getSubDivisionsXY() << endl;
		mout << "Number of points on Z : " << m_processData.getSubDivisionsZ() << endl;
		mout << "Total of points: " << (m_processData.getTotalPointsNumber()) << endl << endl;

		// fill bases
		vector<Base3D*> bases = vector<Base3D*>(m_processData.getTotalPointsNumber(), nullptr);
		fillBases(&bases);

		std::string basePath = "prescan_bases_" + getDate();

		if (m_processData.getExportBasesCartesian())
		{
			std::string path = basePath + "_cartesian.csv";
			std::string defaultFileName = m_processData.getBasesCartesianDefaultFileName();
			if (defaultFileName.size() != 0)
			{
				if (ReScan::isValidNameFile(defaultFileName, "csv"))
				{
					path = defaultFileName;
				}
				else
				{
					mout << "Default file name for bases cartesian invalid." << std::endl;
				}
			}
			mout << endl << "Exporting bases (cartesian)..." << endl;
			if (exportBasesCartesianToCSV(path, bases, "0;0;0;0;0;0;0;0;0;0;0;0"))
			{
				mout << "Bases (cartesians) saved into:" << std::endl << path << std::endl;
				notifyObservers(FileType::BasesCartesian, path);
			}
			else
			{
				mout << "Bases (cartesians) not saved" << std::endl;
			}
		}

		if (m_processData.getExportBasesEulerAngles())
		{
			std::string path = basePath + "_euler-angles-ZYX.csv";
			std::string defaultFileName = m_processData.getBasesEulerAnglesDefaultFileName();
			if (defaultFileName.size() != 0)
			{
				if (ReScan::isValidNameFile(defaultFileName, "csv"))
				{
					path = defaultFileName;
				}
				else
				{
					mout << "Default file name for bases Euler angles invalid." << std::endl;
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
				mout << "Bases (Euler angles) not saved" << std::endl;
			}
		}

		if (m_processData.getExportDetailsFile())
		{
			std::string path = basePath + "_details.csv";
			std::string defaultFileName = m_processData.getDetailsDefaultFileName();
			if (defaultFileName.size() != 0)
			{
				if (ReScan::isValidNameFile(defaultFileName, "csv"))
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
				mout << "Details not saved" << std::endl;
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

	bool PreScan::exportBasesCartesianToCSV(const std::string& filename, const std::vector<Base3D*>& bases, const std::string& nullText) const
	{
		if (!ReScan::isValidNameFile(filename, "csv"))
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

	bool PreScan::exportBasesEulerAnglesToCSV(const std::string& filename, const std::vector<Base3D*>& bases, const std::string& nullText) const
	{
		if (!ReScan::isValidNameFile(filename, "csv"))
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

	bool PreScan::exportTrajectoryDetailsFile(const std::string& filename) const
	{
		if (!ReScan::isValidNameFile(filename, "csv"))
		{
			return false;
		}

		std::ofstream outputFile(filename);

		if (!outputFile.is_open())
		{
			mout << "Cannot open: " + filename << std::endl;
			return false;
		}

		string distance1Str = to_string(m_processData.getDistanceXY());
		string distance2Str = to_string(m_processData.getDistanceZ());
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
		outputFile << "X step (mm);" << to_string(*m_processData.getStepAxisXY()) << std::endl;
		outputFile << "Y step (mm);" << to_string(*m_processData.getStepAxisZ()) << std::endl;
		outputFile << "X divisions;" << to_string(m_processData.getSubDivisionsXY()) << std::endl;
		outputFile << "Y divisions;" << to_string(m_processData.getSubDivisionsZ()) << std::endl;
		outputFile << "total divisions;" << to_string(m_processData.getTotalPointsNumber()) << std::endl;

		outputFile.close();
		return true;
	}

#pragma endregion

#pragma region public functions

	void PreScan::subscribe(ReScan::EventCallback callback)
	{
		m_subscribers.push_back(callback);
	}

	void PreScan::unsubscribe(ReScan::EventCallback callback)
	{
		auto it = std::find_if(m_subscribers.begin(), m_subscribers.end(),
			[callback](const ReScan::EventCallback& cb) { return cb.target<void(const std::string&)>() == callback.target<void(const std::string&)>(); });

		if (it != m_subscribers.end())
		{
			m_subscribers.erase(it);
		}
	}

	int PreScan::process(std::string& configFile)
	{
		int result;

		resetProcessData();

		PreScanConfig config;
		result = PreScanConfig::loadConfigFromFile(configFile, &config);

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
				configFile = "prescan-config.ini";
			}
			if (!configFile.ends_with(".ini"))
			{
				configFile += ".ini";
			}

			mout << "Would you like to create a new config file (" << configFile << ") ? " << std::endl;
			mout << "0: No" << std::endl;
			mout << "1: Create a new config file" << std::endl;

			int choice;
			do
			{
				std::cin >> choice;
				if (choice < 0 || choice > 1)
				{
					mout << "Invalide choice." << endl;
				}
			} while (choice < 0 || choice > 1);

			if (choice == 1)
			{
				ReScanConfig::saveConfigToFile(ReScanConfig(), configFile);
				mout << "You now have to edit this new file." << std::endl;
			}
		}
		return result;
	}

	int PreScan::process(const PreScanConfig& config)
	{
		m_processData.setFromConfig(config);
		return internalProcess();
	}

	int PreScan::process(
		const bool exportBasesCartesian,
		const bool exportBasesEulerAngles,
		const bool exportDetailsFile,
		const bool writeHeader,
		const bool decimalCharIsDot)
	{
		resetProcessData();
		m_processData.setExportBasesCartesian(exportBasesCartesian);
		m_processData.setExportBasesEulerAngles(exportBasesEulerAngles);
		m_processData.setExportDetailsFile(exportDetailsFile);
		m_processData.setWriteHeaders(writeHeader);
		m_processData.setDecimalCharIsDot(decimalCharIsDot);
		return internalProcess();
	}

	int PreScan::process(
		const Point3D& point1,
		const Point3D& point2,
		const unsigned int stepAxis1,
		const unsigned int stepAxis2,
		const int planOffset,
		const bool exportBasesCartesian,
		const bool exportBasesEulerAngles,
		const bool exportDetailsFile,
		const bool writeHeaders,
		const bool decimalCharIsDot)
	{
		resetProcessData();
		m_processData.setPoint1(&point1);
		m_processData.setPoint2(&point2);
		m_processData.setStepAxisXY(stepAxis1);
		m_processData.setStepAxisZ(stepAxis2);
		m_processData.setPlanOffset(planOffset);
		m_processData.setExportBasesCartesian(exportBasesCartesian);
		m_processData.setExportBasesEulerAngles(exportBasesEulerAngles);
		m_processData.setExportDetailsFile(exportDetailsFile);
		m_processData.setWriteHeaders(writeHeaders);
		m_processData.setDecimalCharIsDot(decimalCharIsDot);
		return internalProcess();
	}

#pragma endregion
}

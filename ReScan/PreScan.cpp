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

#include <Eigen/Dense>

using namespace std;

namespace ReScan::PreScan
{
#pragma region Constructors & Desctructor

	PreScan::PreScan() :
		m_processData(),
		m_subscribers(),
		m_bases(nullptr),
		m_details(nullptr)
	{
	}

	PreScan::PreScan(const PreScan& preScan) :
		m_processData(preScan.m_processData),
		m_subscribers(),
		m_bases(nullptr),
		m_details(nullptr)
	{
		if (preScan.m_bases)
		{
			m_bases = new vector<Base3D*>(preScan.m_bases->size(), nullptr);
			for (size_t i = 0; i < m_bases->size(); i++)
			{
				(*m_bases)[i] = new Base3D(*((*preScan.m_bases)[i]));
			}
		}
		if (preScan.m_details)
		{
			m_details = new PreScanResultDetails(*preScan.m_details);
		}
	}

	// destructor

	PreScan::~PreScan()
	{
		m_subscribers.clear();
		clearResults();
	}

#pragma endregion

#pragma region private functions

	void PreScan::clearResults()
	{
		if (m_details)
		{
			delete m_details;
			m_details = nullptr;
		}
		clearBases();
	}

	void PreScan::clearBases()
	{
		if (m_bases)
		{
			for (int i = 0; i < m_bases->size(); i++)
			{
				if ((*m_bases)[i] != nullptr)
				{
					delete (*m_bases)[i];
					(*m_bases)[i] = nullptr;
				}
			}
			m_bases->clear();
			delete m_bases;
			m_bases = nullptr;
		}
	}

	void PreScan::notifyObservers(const FileType fileType, const std::string& path) const
	{
		for (const auto& subscriber : m_subscribers)
		{
			subscriber(fileType, path);
		}
	}

	void PreScan::resetProcessData()
	{
		clearResults();
		m_processData.reset();
	}

	void PreScan::selectPoint(std::string& name, Point3D* point) const
	{
		mout << "Input coordinates of " << name << " in mm:" << endl;
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

	double PreScan::selectPeakRatio() const
	{
		int peakRatio;
		do
		{
			mout << endl << "Input peak ratio (0-100): ";
			std::cin >> peakRatio;

			if (peakRatio < 0 || peakRatio > 100) {
				mout << "Invalide peakRatio." << endl;
			}

		} while (peakRatio < 0 || peakRatio > 100);
		return ((double)peakRatio) / 100.0;
	}

	int PreScan::selectPreScanMode(PreScanMode* mode) const
	{
		int choice;
		int result = SUCCESS_CODE;

		// Choix des axes
		do
		{
			mout << endl << "Select the PreScan mode:" << endl;
			mout << "1) Default" << endl;
			mout << "2) Horizontal" << endl;
			mout << "3) Vertical" << endl << endl;

			std::cin >> choice;

			if (choice < 1 || choice > 3) {
				mout << "Invalide choice." << endl;
			}

		} while (choice < 1 || choice > 3);

		switch (choice)
		{
		case 1:
			mout << "Default mode selected" << endl << endl;
			*mode = PreScanMode::Default;
			break;
		case 2:
			mout << "Horizontal mode selected" << endl << endl;
			*mode = PreScanMode::Horizontal;
			break;

		case 3:
			mout << "Vertical mode selected" << endl << endl;
			*mode = PreScanMode::Vertical;
			break;

		default:
			mout << "Unexpected prescan mode choice." << std::endl;
			result = INVALID_PRESCAN_MODE_ERROR_CODE;
			break;
		}

		return result;
	}
	
	unsigned int PreScan::selectStep(std::string& axisName, unsigned int min, unsigned int max) const
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
		return div + 1;
	}

	bool PreScan::fileExists(const std::string& filename) const
	{
		std::ifstream ifs(filename);
		return ifs ? true : false;
	}

	void PreScan::fillBases(std::vector<Base3D*>* bases) const
	{
		// détails de l'algo :
		// on souhaite créer un plan / une surface incurvée entre p1 et p2
		// on trouve l'angle du vecteur normal à la droite P1P2 projeté sur XY
		// on applique la rotation de p1 et p2 pour les recentrer sur l'axe X du W0
		// on fait notre surface dans ce repere la
		// on applique la rotation inverse sur chaque base pour les remettre correctement dans le W0

		const Point3D* p1 = m_processData.getPoint1();
		const Point3D* p2 = m_processData.getPoint2();

		double p1x = p1->getX();
		double p1y = p1->getY();
		double p2x = p2->getX();
		double p2y = p2->getY();

		if (p1x > p2x)
		{
			// p1 takes (x,y) from p2 and p2 takes (x,y) from p1
			double tmp = p1x;
			p1x = p2x;
			p2x = tmp;

			tmp = p1y;
			p1y = p2y;
			p2y = tmp;
		}

		double angle = atan2(p2x - p1x, p1y - p2y); // angle of the normal
		double cosa = cos(angle);
		double sina = sin(angle);

		// p1 rotated = Rot(-angle) * p1
		// p1rx = x*cos(-a) - y*sin(-a) = x*cos(a)+y*sin(a)
		// p1ry = x*sin(-a) + y*cos(-a) = y*cos(a)-x*sin(a)

		double p1rx = p1x * cosa + p1y * sina;
		if (p1rx < 0)
		{
			angle += EIGEN_PI;
			cosa = cos(angle);
			sina = sin(angle);

			// swap x
			p2x += p1x;
			p1x = p2x - p1x;
			p2x -= p1x;
			
			// swap y
			p2y += p1y;
			p1y = p2y - p1y;
			p2y -= p1y;

			//p1rx = p1x * cosa + p1y * sina;
			p1rx = -p1rx;
		}

		double p1ry = p1y * cosa - p1x * sina;

		//double p2rx = p2x * cosa + p2y * sina;
		double p2ry = p2y * cosa - p2x * sina;

		if (p2ry < p1ry)
		{
			// swap y
			p2ry += p1ry;
			p1ry = p2ry - p1ry;
			p2ry -= p1ry;
		}

		Eigen::Matrix4d rotationMatrix = Eigen::Matrix4d::Identity();
		rotationMatrix(0, 0) = cosa;
		rotationMatrix(0, 1) = -sina;
		rotationMatrix(1, 0) = sina;
		rotationMatrix(1, 1) = cosa;
		
		switch (*m_processData.getPreScanMode())
		{
		case PreScanMode::Horizontal:
			fillBasesHorizontal(bases, p1rx, p1ry, p1->getZ(), p2ry, p2->getZ(), rotationMatrix);
			break;
		case PreScanMode::Vertical:
			fillBasesVertical(bases, p1rx, p1ry, p1->getZ(), p2ry, p2->getZ(), rotationMatrix);
			break;
		//case PreScanMode::Default:
		default:
			fillBasesDefault(bases, p1rx, p1ry, p1->getZ(), p2ry, p2->getZ(), rotationMatrix);
			break;
		}
	}

	void PreScan::fillBasesDefault(std::vector<Base3D*>* bases, const double p1rx, const double p1ry, const double p1rz, const double p2ry, const double p2rz, const Eigen::Matrix4Xd& rotationMatrix) const
	{
		double prx = p1rx + *m_processData.getPlanOffset();
		double pry;
		double prz = p1rz;

		double nextPry;
		double nextPrz = prz;

		double stepY = (double)*m_processData.getStepAxisXY();
		double stepZ = (double)*m_processData.getStepAxisZ();
		int baseIndex = 0;

		Eigen::Matrix4d br;
		Eigen::Matrix4d b0;

		while (prz <= p2rz)
		{
			pry = p1ry;
			nextPry = pry;
			while (pry <= p2ry)
			{
				br = Base3D(prx, pry, prz, 0., -1., 0., 0., 0., 1., -1., 0., 0.).toMatrix4d();
				b0 = rotationMatrix * br;
				Base3D* b = new Base3D();
				b->setFromMatrix4d(b0);
				(*bases)[baseIndex++] = b;

				if (pry == p2ry)
				{
					break;
				}
				nextPry += stepY;
				pry = nextPry > p2ry ? p2ry : nextPry;
			}

			if (prz == p2rz)
			{
				break;
			}
			nextPrz += stepZ;
			prz = nextPrz > p2rz ? p2rz : nextPrz;
		}
	}

	void PreScan::fillBasesHorizontal(std::vector<Base3D*>* bases, const double p1rx, const double p1ry, const double p1rz, const double p2ry, const double p2rz, const Eigen::Matrix4Xd& rotationMatrix) const
	{
		double planOffset = *m_processData.getPlanOffset();
		double peakRatio = *m_processData.getPeakRatio();
		double distance = p2ry - p1ry;
		double peakDistance = peakRatio * distance;
		double peakDistance2 = (1. - peakRatio) * distance;

		double prx;
		double pry = p1ry;
		double prz;

		double nextPry = pry;
		double nextPrz;

		double stepY = (double)*m_processData.getStepAxisXY();
		double stepZ = (double)*m_processData.getStepAxisZ();
		int baseIndex = 0;

		double alpha = -atan2(planOffset, peakDistance);
		double beta = atan2(planOffset, peakDistance2);

		double cosa = cos(alpha);
		double sina = sin(alpha);
		double cosb = cos(beta);
		double sinb = sin(beta);

		Eigen::Matrix4d br;
		Eigen::Matrix4d b0;
		bool isBeforePeak = false;
		double currentDistance;

		while (pry <= p2ry)
		{
			prz = p1rz;
			nextPrz = prz;

			currentDistance = pry - p1ry;
			isBeforePeak = currentDistance <= peakDistance && peakRatio;

			prx = isBeforePeak ?
				p1rx + planOffset * (1. + (currentDistance / peakDistance)) :
				p1rx + planOffset * (1. + ((distance - currentDistance) / peakDistance2));

			while (prz <= p2rz)
			{
				br = isBeforePeak ?
					Base3D(prx, pry, prz, sina, -cosa, 0., 0., 0., 1., -cosa, -sina, 0.).toMatrix4d() :
					Base3D(prx, pry, prz, sinb, -cosb, 0., 0., 0., 1., -cosb, -sinb, 0.).toMatrix4d();

				b0 = rotationMatrix * br;
				Base3D* b = new Base3D();
				b->setFromMatrix4d(b0);
				(*bases)[baseIndex++] = b;

				if (prz == p2rz)
				{
					break;
				}
				nextPrz += stepZ;
				prz = nextPrz > p2rz ? p2rz : nextPrz;
			}

			if (pry == p2ry)
			{
				break;
			}
			nextPry += stepY;
			pry = nextPry > p2ry ? p2ry : nextPry;
		}
	}

	void PreScan::fillBasesVertical(std::vector<Base3D*>* bases, const double p1rx, const double p1ry, const double p1rz, const double p2ry, const double p2rz, const Eigen::Matrix4Xd& rotationMatrix) const
	{
		//double prx = p1rx + *m_processData.getPlanOffset();
		//double pry;
		//double prz = p1rz;

		//double nextPry;
		//double nextPrz = prz;

		//double stepY = (double)*m_processData.getStepAxisXY();
		//double stepZ = (double)*m_processData.getStepAxisZ();
		//int baseIndex = 0;

		//Eigen::Matrix4d br;
		//Eigen::Matrix4d b0;
	}

	int PreScan::internalProcess()
	{
		const unsigned int MIN_DISTANCE = 1;

		//m_processData.setExportBasesEulerAngles(false);
		//m_processData.setExportDetailsFile(false);
		//Point3D p1 = Point3D(500, -200, 200); // apres rotation: (536, -045, 200)
		//Point3D p2 = Point3D(400, -400, 500); // apres rotation: (536, -179, 500)
		//m_processData.setPoint1(&p1);
		//m_processData.setPoint2(&p2);

		// Select point1 if needed
		if (!m_processData.getPoint1())
		{
			if (m_processData.getEnableUserInput())
			{
				Point3D point1;
				std::string pName = "Point 1";
				selectPoint(pName, &point1);
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
				std::string pName = "Point 2";
				selectPoint(pName, &point2);
				m_processData.setPoint2(&point2);
			}
			else
			{
				return NO_POINT_2_SELECTED_ERROR_CODE;
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

		//double planOffset;
		//double peakRatio;
		//m_processData.findPlanOffsetAndPeakRatio(Point3D(285, -405, 350), planOffset, peakRatio); // planOffset: -100 | peakRatio: -0.25 -> INVALIDE 
		//m_processData.findPlanOffsetAndPeakRatio(Point3D(310, -355, 350), planOffset, peakRatio); // planOffset: -100 | peakRatio:  0.00 -> VALIDE 
		//m_processData.findPlanOffsetAndPeakRatio(Point3D(335, -305, 350), planOffset, peakRatio); // planOffset: -100 | peakRatio:  0.25 -> VALIDE 
		//m_processData.findPlanOffsetAndPeakRatio(Point3D(360, -255, 350), planOffset, peakRatio); // planOffset: -100 | peakRatio:  0.50 -> VALIDE 
		//m_processData.findPlanOffsetAndPeakRatio(Point3D(385, -205, 350), planOffset, peakRatio); // planOffset: -100 | peakRatio:  0.75 -> VALIDE 
		//m_processData.findPlanOffsetAndPeakRatio(Point3D(410, -157, 350), planOffset, peakRatio); // planOffset: -100 | peakRatio:  1.00 -> VALIDE 
		//m_processData.findPlanOffsetAndPeakRatio(Point3D(445, -105, 350), planOffset, peakRatio); // planOffset: -100 | peakRatio: 1.25 -> INVALIDE 
		//m_processData.setPlanOffset(planOffset);
		//m_processData.setPeakRatio(peakRatio);

		// Select planOffset if needed
		if (m_processData.getPlanOffset() == nullptr)
		{
			if (m_processData.getEnableUserInput())
			{
				int planOffset;
				mout << endl << "Input plan offset in mm: ";
				std::cin >> planOffset;
				m_processData.setPlanOffset(planOffset);
			}
			else
			{
				mout << "No plan offset selected" << endl;
				return NO_PLAN_OFFSET_SELECTED_ERROR_CODE;
			}
		}
		mout << "plan offset: " << *m_processData.getPlanOffset() << " mm" << endl;

		m_processData.setDistanceXY(sqrt(pow(processP2->getX() - processP1->getX(), 2) + pow(processP2->getY() - processP1->getY(), 2)));
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
				std::string aName = "XY";
				m_processData.setStepAxisXY(selectStep(aName, MIN_DISTANCE, int(m_processData.getDistanceXY())));
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
				std::string aName = "XY";
				m_processData.setStepAxisXY(selectStep(aName, MIN_DISTANCE, int(m_processData.getDistanceXY())));
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
				std::string aName = "Z";
				m_processData.setStepAxisZ(selectStep(aName, MIN_DISTANCE, int(m_processData.getDistanceZ())));
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
				std::string aName = "Z";
				m_processData.setStepAxisZ(selectStep(aName, MIN_DISTANCE, int(m_processData.getDistanceZ())));
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
		mout << "Number of points on XY: " << m_processData.getPointsNumberXY() << endl;
		mout << "Number of points on Z : " << m_processData.getPointsNumberZ() << endl;
		mout << "Total of points: " << (m_processData.getTotalPointsNumber()) << endl << endl;

		// select preScan mode if needed
		if (m_processData.getPreScanMode() == nullptr)
		{
			if (m_processData.getEnableUserInput())
			{
				PreScanMode prescanMode;
				int result = selectPreScanMode(&prescanMode);
				if (result == SUCCESS_CODE)
				{
					m_processData.setPreScanMode(prescanMode);
				}
				else
				{
					return result;
				}
			}
			else
			{
				mout << "No mode selected." << std::endl;
				return NO_PRESCAN_MODE_SELECTED_ERROR_CODE;
			}
		}

		std::string modeStr;
		Tools::preScanModeToString(*m_processData.getPreScanMode(), modeStr);
		mout << "Mode: " << modeStr << std::endl;

		// Select peakRatio if needed
		if (*m_processData.getPreScanMode() != PreScanMode::Default && m_processData.getPeakRatio() == nullptr)
		{
			if (m_processData.getEnableUserInput())
			{
				m_processData.setPeakRatio(selectPeakRatio());
			}
			else
			{
				mout << "No plan offset selected" << endl;
				return NO_PEAK_RATIO_SELECTED_ERROR_CODE;
			}
			mout << "Peak ratio: " << *m_processData.getPeakRatio() << endl;
		}

		// fill bases
		if (m_bases || m_details)
		{
			clearResults();
		}

		m_bases = new vector<Base3D*>(m_processData.getTotalPointsNumber(), nullptr);
		m_details = new PreScanResultDetails();
		fillBases(m_bases);
		m_details->setFromProcessData(m_processData);

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
			if (exportBasesCartesianToCSV(path, *m_bases, "0;0;0;0;0;0;0;0;0;0;0;0"))
			{
				mout << "Bases (cartesian) saved into:" << std::endl << path << std::endl;
				notifyObservers(FileType::BasesCartesian, path);
			}
			else
			{
				mout << "Bases (cartesian) not saved" << std::endl;
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
			if (exportBasesEulerAnglesToCSV(path, *m_bases, "0;0;0;0;0;0"))
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

		outputFile << "XY dimensions (mm);" << distance1Str << std::endl;
		outputFile << "Z dimensions (mm);" << distance2Str << std::endl;
		outputFile << "XY step (mm);" << to_string(*m_processData.getStepAxisXY()) << std::endl;
		outputFile << "Z step (mm);" << to_string(*m_processData.getStepAxisZ()) << std::endl;
		outputFile << "XY points;" << to_string(m_processData.getPointsNumberXY()) << std::endl;
		outputFile << "Z points;" << to_string(m_processData.getPointsNumberZ()) << std::endl;
		outputFile << "total points;" << to_string(m_processData.getTotalPointsNumber()) << std::endl;

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

	int PreScan::process(const std::string& configFile)
	{
		int result;
		std::string configFileName = configFile;

		resetProcessData();

		PreScanConfig config;
		result = PreScanConfig::loadConfigFromFile(configFileName, &config);
		m_processData.setFromConfig(config);

		if (result == SUCCESS_CODE)
		{
			result = internalProcess();
		}
		else if (result == FILE_NOT_FOUND_ERROR_CODE || result == READ_CONFIG_ERROR_CODE || result == SET_CONFIG_ERROR_CODE)
		{
			if (!m_processData.getEnableUserInput())
			{
				return result;
			}
			if (configFileName.size() == 0)
			{
				configFileName = "prescan-config.ini";
			}
			if (!configFileName.ends_with(".ini"))
			{
				configFileName += ".ini";
			}

			mout << std::endl << "Would you like to create a new config file (" << configFileName << ") ? " << std::endl;
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
				mout << std::endl << "You now have to edit this new file." << std::endl;
			}
		}
		return result;
	}

	int PreScan::process(const PreScanConfig& config)
	{
		resetProcessData();
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

	std::vector<Base3D*>* PreScan::getResutlt()
	{
		return m_bases;
	}

	PreScanResultDetails* PreScan::getResutltDetails()
	{
		return m_details;
	}

#pragma endregion
}

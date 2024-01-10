#include "ReScan.h"
#include <fstream>    // for file exist

using namespace std;

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

int ReScan::internalProcess(const bool exportFiles)
{
    // declarations

    vector<float> vertices;
    vector<int>   triangles;
    vector<float> uvs;
    vector<int>   uvtriangles;

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
        return INVALID_FILE_CODE;
    }
    string filenameWithoutExtention = removeFileExtension(m_fileName);

    // Read obj file
    objio::readObjFile(m_fileName, &vertices, &triangles, &uvs, &uvtriangles);

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
        return INVALID_PLAN_CODE;
    }

    // Find extremas points (2 corners of the ROI)
    ScatterGraph::findExtrema(graph, *m_plan2D, getters, &minPoint, &maxPoint);

    cout << "min point: " << minPoint << endl;
    cout << "max point: " << maxPoint << endl << endl;

    // Compute dimensions of the ROI
    getDistances(minPoint, maxPoint, getters, distance1, distance2);

    // display infos about distances
    cout << "distance " << axis1Name << ": " << distance1 << " mm" << endl;
    cout << "distance " << axis2Name << ": " << distance2 << " mm" << endl << endl;

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

    // compute number of subdivsions on both axis
    subDivision1 = getSubDivision(distance1, *m_stepAxis1);
    subDivision2 = getSubDivision(distance2, *m_stepAxis2);

    // display infos about subdivisions
    cout << endl;
    cout << "Number of subdivisions on " << axis1Name << ": " << subDivision1 << endl;
    cout << "Number of subdivisions on " << axis2Name << ": " << subDivision2 << endl;
    cout << "Total of subdivisions: " << (subDivision1 * subDivision2) << endl << endl;

    // fill all sub graphs
    fillSubDivisions(minPoint, graph, &subDivisions, getters, *m_stepAxis1, *m_stepAxis2, subDivision1, subDivision2, true);

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

    if (exportFiles)
    {
        exportFilesCSV(filenameWithoutExtention, subDivisions);
    }

    return SUCCESS_CODE;
}

void ReScan::exportFilesCSV(const std::string& basePath, const std::vector<ScatterGraph>& subDivisions) const
{
    for (int i = 0; i < subDivisions.size(); i++)
    {
        ScatterGraph::saveCSV(basePath + to_string(i + 1) + ".csv", subDivisions[i], true, true);
    }
}

#pragma endregion

#pragma region public functions

int ReScan::process(const bool exportFiles)
{
    delete m_plan2D;
    m_plan2D = nullptr;

    delete m_stepAxis1;
    m_stepAxis1 = nullptr;

    delete m_stepAxis2;
    m_stepAxis2 = nullptr;

    return internalProcess(exportFiles);
}

int ReScan::process(const Plan2D plan2D, const unsigned int stepAxis1, const unsigned int stepAxis2, const bool exportFiles)
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
    return internalProcess(exportFiles);
}

#pragma endregion

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
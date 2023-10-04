#include "ObjFileIO.h"
#include "ScatterGraph.h"
#include "Plan2D.h"
#include "Point3D.h"

#include <iostream>
#include <fstream>    // for file exist
#include <string>

using namespace std;

std::string removeFileExtension(const std::string& fileName);
Plan2D selectPlan2D();
double getDistance1D(const Point3D& point1, const Point3D& point2, double (Point3D::* getter)() const);
void getDistances(Point3D const& minPoint, Point3D const& maxPoint, double (Point3D::* getters[2])() const, double& distance1, double& distance2);
unsigned int selectStep(char axisName, unsigned int min, unsigned int max);
unsigned int getSubDivision(double distance, int step);
void fillSubDivisions(const Point3D& minPoint, const ScatterGraph& graph, std::vector<ScatterGraph>* subGraph,
    double (Point3D::* getters[2])() const,
    const unsigned int step1, const unsigned int step2, const unsigned int subDivision1, const unsigned int subDivision2, const bool deleteIfEmpty=true);

int main(int argc, char* argv[])
{
    vector<float> vertices;
    vector<int> triangles;
    vector<float> uvs;
    vector<int> uvtriangles;

    ScatterGraph graph;
    Plan2D plan;

    // Tableau de pointeurs de membres pour les getters désirés
    double (Point3D:: * getters[2])() const; {}

    Point3D minPoint;
    Point3D maxPoint;

    char axis1Name = '?';
    char axis2Name = '?';

    double distance1;
    double distance2;

    unsigned int step1;
    unsigned int step2;

    unsigned int subDivision1;
    unsigned int subDivision2;

    vector<ScatterGraph> subDivisions;

    #pragma region Check filename, exist and extention

    if (argc != 2)
    {
        std::cerr << "Usage: ReScan.exe filename.obj" << std::endl;
        exit(-1);
    }

    std::string filename = argv[1];
    std::string filenameWithoutExtention;

    std::ifstream fileExists(filename);
    if (!fileExists)
    {
        std::cerr << "File: " << filename << " not found." << std::endl;
        exit(-1);
    }

    if (filename.length() < 4 || filename.substr(filename.length() - 4) != ".obj")
    {
        std::cerr << "File is not .obj" << std::endl;
        return false;
    }

    filenameWithoutExtention = removeFileExtension(filename);
    
    #pragma endregion

    // Read file
    objio::readObjFile(filename, &vertices, &triangles, &uvs, &uvtriangles);

    ScatterGraph::populateFromVectorXYZ(&vertices, graph);

    plan = selectPlan2D();

    // Sélection des getters
    switch (plan)
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
        exit(-1);
        break;
    }

    ScatterGraph::findExtrema(graph, plan, getters, &minPoint, &maxPoint);

    cout << "min point: " << minPoint << endl;
    cout << "max point: " << maxPoint << endl << endl;

    getDistances(minPoint, maxPoint, getters, distance1, distance2);

    cout << "distance " << axis1Name << ": " << distance1 << " mm" << endl;
    cout << "distance " << axis2Name << ": " << distance2 << " mm" << endl << endl;

    step1 = selectStep(axis1Name, 50, int(distance1));
    step2 = selectStep(axis2Name, 50, int(distance2));

    subDivision1 = getSubDivision(distance1, step1);
    subDivision2 = getSubDivision(distance2, step2);

    cout << endl;
    cout << "Number of subdivisions on " << axis1Name << ": " << subDivision1 << endl;
    cout << "Number of subdivisions on " << axis2Name << ": " << subDivision2 << endl;
    cout << "Total of subdivisions: " << (subDivision1 * subDivision2) << endl << endl;

    fillSubDivisions(minPoint, graph, &subDivisions, getters, step1, step2, subDivision1, subDivision2, true);

    cout << "Number of points in the main graph: " << graph.size() << endl << endl;
    cout << "Number of points in the sub graphes" << endl;
    unsigned int sum = 0;
    for (int i = 0; i < subDivisions.size(); i++)
    {
        cout << "Graph " << i + 1 << ": " << subDivisions[i].size() << endl;
        sum += (unsigned int)subDivisions[i].size();
    }
    cout << "Total of points: " << sum << endl;

    for (int i = 0; i < subDivisions.size(); i++)
    {
        ScatterGraph::saveCSV(filenameWithoutExtention + to_string(i + 1) + ".csv", subDivisions[i], true, true);
    }
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

Plan2D selectPlan2D()
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

double getDistance1D(const Point3D& point1, const Point3D& point2, double (Point3D::* getter)() const)
{
    double x1 = (point1.*getter)();
    double x2 = (point2.*getter)();
    return abs(x2 - x1);
}

void getDistances(Point3D const& minPoint, Point3D const& maxPoint, double (Point3D::* getters[2])() const, double& distance1, double& distance2)
{
    distance1 = getDistance1D(minPoint, maxPoint, getters[0]);
    distance2 = getDistance1D(minPoint, maxPoint, getters[1]);
}

unsigned int selectStep(char axisName, unsigned int min, unsigned int max)
{
    unsigned int step;
    do
    {
        cout << endl << "Select the " << axisName << " axis step between " << to_string(min) << " mm and " << to_string(max) << " mm" << endl << endl;
        cin >> step;
    } while (step < min || step > max);
    return step;
}

unsigned int getSubDivision(double distance, int step)
{
    unsigned int div = (unsigned int)(abs(distance) / step);
    if ((double)(div * step) < distance)
    {
        div++;
    }
    return div;
}

void fillSubDivisions(const Point3D& minPoint, const ScatterGraph& graph, std::vector<ScatterGraph>* subGraph,
    double (Point3D::* getters[2])() const,
    const unsigned int step1, const unsigned int step2, const unsigned int subDivision1, const unsigned int subDivision2, const bool deleteIfEmpty)
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

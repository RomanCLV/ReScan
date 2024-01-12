#include "ReScan.h"

#include <iostream>
#include <string>

using namespace std;

int main(int argc, char* argv[])
{
    if (argc != 2)
    {
        std::cerr << "Usage: ReScan.exe filename.obj" << std::endl;
        exit(-1);
    }

    ReScan::ScatterGraph graph;
    ReScan::ScatterGraph::readCSV("p2.csv", &graph, true);

    ReScan::Point3D point;
    ReScan::Plan plan;
    
    ReScan::ScatterGraph::computeBarycenter(graph, &point);
    ReScan::ScatterGraph::computeAveragePlan(graph, &plan);

    cout << "Barycenter:" << endl;
    cout << point << endl << endl;

    cout << "Plan:" << endl;
    cout << plan << endl;

    graph.clear();

    //ReScan::ReScan reScan(argv[1]);
    //reScan.process(true);
    //reScan.process(ReScan::Plan2D::YZ, 100, 100, false);

    return 0;
}

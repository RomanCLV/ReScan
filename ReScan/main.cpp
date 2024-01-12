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
    for (int i = 0; i < 1000; i++)
    {
        graph.addPoint(i, i, i);
    }

    size_t numbers[] = { 0, 10, 50, 33, 48, 95, 1005 };

    for (int i = 0; i < 7; i++)
    {
        ReScan::ScatterGraph g(graph);
        g.reduce(numbers[i]);
        ReScan::ScatterGraph::saveCSV("test " + std::to_string(i) + " ( " + std::to_string(numbers[i]) + ".csv", g, true, true);
        g.clear();
    }

    graph.clear();

    //ReScan::ReScan reScan(argv[1]);
    //reScan.process(true);
    //reScan.process(ReScan::Plan2D::YZ, 100, 100, false);

    return 0;
}

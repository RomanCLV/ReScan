#include "ReScan.h"

#include <iostream>
#include <string>
#include <Eigen/Dense>

using namespace std;

int main(int argc, char* argv[])
{
    if (argc != 2)
    {
        std::cerr << "Usage: ReScan.exe filename.obj" << std::endl;
        exit(-1);
    }

    ReScan::ReScan reScan(argv[1]);
    //reScan.process(false, true, true);
    reScan.process(ReScan::Plan2D::YZ, 100, 100, false, true, true);

    return 0;
}

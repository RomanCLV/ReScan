#include "ReScan.h"

#include <iostream>
#include <string>
#include <Eigen/Dense>

void createConfig();
void createConfigICNDE();

void help()
{
	std::cout << std::endl;
	std::cout << "Usage:" << std::endl;
	std::cout << std::endl;
	std::cout << "ReScan.exe [-h]                        Help" << std::endl;
	std::cout << "ReScan.exe [--help]                    " << std::endl;
	std::cout << std::endl;
	std::cout << "ReScan.exe [-c config.ini]             Config file is specified" << std::endl;
	std::cout << "ReScan.exe [--config config.ini]       " << std::endl;
	std::cout << std::endl;
	std::cout << "ReScan.exe [-f objFile.obj]            Obj file is specified" << std::endl;
	std::cout << "ReScan.exe [--file objFile.obj]        " << std::endl;
	std::cout << std::endl;
	std::cout << "ReScan.exe [-cc]                       Create a new default config file: config.ini" << std::endl;
	std::cout << "ReScan.exe [--create-config]           " << std::endl;
	std::cout << std::endl;
	std::cout << "ReScan.exe [-cci]                       Create a new default config file adapted for ICNDE: config.ini" << std::endl;
	std::cout << "ReScan.exe [--create-config-icnde]      " << std::endl;
	std::cout << std::endl;
}

int main(int argc, char* argv[])
{
	if (argc == 2)
	{
		std::string arg1(argv[1]);
		if (arg1 == "-h" || arg1 == "--help")
		{
			help();
		}
		else if (arg1 == "-cc" || arg1 == "--create-config")
		{
			createConfig();
		}
		else if (arg1 == "-cci" || arg1 == "--create-config-icnde")
		{
			createConfigICNDE();
		}
		else
		{
			std::cerr << "Unknow option: " << arg1 << std::endl;
			help();
		}
	}
	else if (argc == 3)
	{
		std::string arg1(argv[1]);
		std::string arg2(argv[2]);
		ReScan::ReScan reScan;
		if (arg1 == "-c" || arg1 == "--config")
		{
			reScan.process(arg2);
		}
		else if (arg1 == "-f" || arg1 == "--file")
		{
			reScan.process(arg2, true, true, true, true, false, true);
		}
		else
		{
			std::cerr << "Unknow option: " << arg1 << std::endl;
			help();
		}
	}
	else
	{
		help();
	}

	std::cout << std::endl << "Press enter to exit..." << std::endl;
	std::cin.get();
	return 0;
}

void createConfig()
{
	ReScan::ReScanConfig::saveConfigToFile(ReScan::ReScanConfig(), "config.ini");
}

void createConfigICNDE()
{
	ReScan::ReScanConfig::saveConfigToFile(ReScan::ReScanConfig::createDassaultConfig(), "config.ini");
}
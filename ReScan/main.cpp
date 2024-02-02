#include "ReScan.h"
#include "ReScanConfig.h"
#include <iostream>
#include <string>
#include "Tools.h"

static void help()
{
	std::cout << std::endl;
	std::cout << "Usage:" << std::endl;
	std::cout << "ReScan.exe	                                     Basic usage" << std::endl << std::endl;
	std::cout << "ReScan.exe [-h|--help]                             Help" << std::endl << std::endl;
	std::cout << "ReScan.exe [-c|--config] config.ini                Config file is specified" << std::endl << std::endl;
	std::cout << "ReScan.exe [-f|--file] objFile.obj                 Obj file is specified" << std::endl << std::endl;
	std::cout << "ReScan.exe [-cc|--create-config]                   Create a new default config file: config.ini" << std::endl;
	std::cout << "ReScan.exe [-ccif|--create-config-icnde-frontal]   Create a new config file adapted for ICNDE (frontal): configFrontal.ini" << std::endl << std::endl;
	std::cout << "ReScan.exe [-ccil|--create-config-icnde-lateral]   Create a new config file adapted for ICNDE (lateral): configLateral.ini" << std::endl << std::endl;
	std::cout << std::endl;
}

int main(int argc, char* argv[])
{
	int result = SUCCESS_CODE;

	if (argc == 1)
	{
		ReScan::ReScan rescan;
		std::string filename;
		std::cout << "filename: ";
		std::getline(std::cin, filename);

		rescan.process(filename, true);
	}
	else if (argc == 2)
	{
		std::string arg1(argv[1]);
		if (arg1 == "-h" || arg1 == "--help")
		{
			help();
		}
		else if (arg1 == "-cc" || arg1 == "--create-config")
		{
			ReScan::ReScanConfig::saveConfigToFile(ReScan::ReScanConfig(), "config.ini");
		}
		else if (arg1 == "-ccif" || arg1 == "--create-config-icnde-frontal")
		{
			ReScan::ReScanConfig::saveConfigToFile(ReScan::ReScanConfig::createFrontalICNDEConfig(), "configFrontal.ini");
		}
		else if (arg1 == "-ccil" || arg1 == "--create-config-icnde-lateral")
		{
			ReScan::ReScanConfig::saveConfigToFile(ReScan::ReScanConfig::createLateralICNDEConfig(), "configLateral.ini");
		}
		else
		{
			ReScan::mout << "Unknow option: " << arg1 << std::endl;
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
			result = reScan.process(arg2);
		}
		else if (arg1 == "-f" || arg1 == "--file")
		{
			result = reScan.process(arg2, true);
		}
		else
		{
			ReScan::mout << "Unknow option: " << arg1 << std::endl;
			help();
		}
	}
	else
	{
		help();
	}

	if (result != SUCCESS_CODE)
	{
		std::cout << std::endl << "Press enter to exit..." << std::endl;
		std::cin.get();
	}
	return 0;
}

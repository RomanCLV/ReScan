#include "ReScan.h"
#include "ReScanConfig.h"

static void help()
{
	std::cout << std::endl;
	std::cout << "Usage:" << std::endl;
	std::cout << std::endl;
	std::cout << "ReScan.exe [-h]                                 Help" << std::endl;
	std::cout << "ReScan.exe [--help]                             " << std::endl;
	std::cout << std::endl;								          
	std::cout << "ReScan.exe [-c config.ini]                      Config file is specified" << std::endl;
	std::cout << "ReScan.exe [--config config.ini]                " << std::endl;
	std::cout << std::endl;								          
	std::cout << "ReScan.exe [-f objFile.obj]                     Obj file is specified" << std::endl;
	std::cout << "ReScan.exe [--file objFile.obj]                 " << std::endl;
	std::cout << std::endl;								          
	std::cout << "ReScan.exe [-cc]                                Create a new default config file: config.ini" << std::endl;
	std::cout << "ReScan.exe [--create-config]                    " << std::endl;
	std::cout << std::endl;								          
	std::cout << "ReScan.exe [-ccif]                              Create a new default config file adapted for ICNDE (frontal): configFrontal.ini" << std::endl;
	std::cout << "ReScan.exe [--create-config-icnde-frontal] "    << std::endl;
	std::cout << std::endl;
	std::cout << "ReScan.exe [-ccil]                              Create a new default config file adapted for ICNDE (lateral): configLateral.ini" << std::endl;
	std::cout << "ReScan.exe [--create-config-icnde-lateral] "    << std::endl;
	std::cout << std::endl;
}

int main(int argc, char* argv[])
{
	int result = SUCCESS_CODE;

	if (argc == 2)
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

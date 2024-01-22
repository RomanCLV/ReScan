#include "ReScan.h"
#include "ReScanConfig.h"
#include "MultiOStream.h"
#include "ObservableOStream.h"
#include <ostream>
#include <fstream>

static void help()
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
	std::cout << "ReScan.exe [-cci]                      Create a new default config file adapted for ICNDE: config.ini" << std::endl;
	std::cout << "ReScan.exe [--create-config-icnde]     " << std::endl;
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
		else if (arg1 == "-cci" || arg1 == "--create-config-icnde")
		{
			ReScan::ReScanConfig::saveConfigToFile(ReScan::ReScanConfig::createDassaultConfig(), "config.ini");
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

static void example()
{
	std::ofstream log("a.log");

	auto customCallback = [&log](const std::string& message)
		{
			log << message;
		};

	ReScan::StreamHelper::ObservableOStream ost(std::cout);
	ost.subscribe(customCallback);
	ReScan::mout.add(&ost);

	ReScan::mout << "Test qui affiche un message dans std::cout et qui le sauvegarde dans un fichier grace a l'ObservableOStream." << std::endl;

	ReScan::mout.remove(&ost);
	ost.unsubscribe(customCallback);
	log.close();
}
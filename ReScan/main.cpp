#include "ReScan.h"
#include "ReScanConfig.h"
#include "PreScan.h"
#include "PreScanConfig.h"
#include <iostream>
#include <string>
#include "Tools.h"

#include <boost/program_options.hpp>

namespace po = boost::program_options;

int main(int argc, char* argv[])
{
	int result = SUCCESS_CODE;
	try {
		// Declare the supported options
		// Catégorie: Options générales
		po::options_description generalOptions("General Options");
		generalOptions.add_options()
			("help,h", "Produce help message");

		// Catégorie: Exécution du processus
		po::options_description runProcessOptions("Run Process Options");
		runProcessOptions.add_options()
			("config,c", po::value<std::string>()->value_name("config.ini"), "Specify a config file")
			("file,f", po::value<std::string>()->value_name("file.obj"), "Specify an obj file")
			("prescan,p", "Indicates to run the PreScan process");

		// Catégorie: Options de configuration
		po::options_description configOptions("Configuration Options");
		configOptions.add_options()
			("ccd", "Create a new default config file: config.ini")
			("ccif", "Create a new config file adapted for ICNDE (frontal): configFrontal.ini")
			("ccil", "Create a new config file adapted for ICNDE (lateral): configLateral.ini")
			("ccp", "Create a new default config file for PreScan process: prescan-config.ini");

		// Groupement des catégories
		po::options_description allOptions("*** ReScan Command Line Parser ***");
		allOptions.add(generalOptions).add(runProcessOptions).add(configOptions);

		// Parsing des arguments
		po::variables_map vm;
		po::store(po::parse_command_line(argc, argv, allOptions), vm);
		po::notify(vm);

		if (vm.count("help"))
		{
			std::cout << allOptions << std::endl;
		}
		else
		{
			bool cancelProcess = false;
			bool isConfigSpecified = vm.count("config") ? true : false;
			bool isFileSpecified = vm.count("file") ? true : false;

			if (vm.count("ccd"))
			{
				ReScan::ReScanConfig::saveConfigToFile(ReScan::ReScanConfig(), "config.ini");
				cancelProcess = true;
			}

			if (vm.count("ccif"))
			{
				ReScan::ReScanConfig::saveConfigToFile(ReScan::ReScanConfig::createFrontalICNDEConfig(), "configFrontal.ini");
				cancelProcess = true;
			}

			if (vm.count("ccil"))
			{
				ReScan::ReScanConfig::saveConfigToFile(ReScan::ReScanConfig::createLateralICNDEConfig(), "configLateral.ini");
				cancelProcess = true;
			}

			if (vm.count("ccp"))
			{
				ReScan::PreScan::PreScanConfig::saveConfigToFile(ReScan::PreScan::PreScanConfig::createConfig(), "prescan-config.ini");
				cancelProcess = true;
			}

			if (!cancelProcess)
			{
				if (vm.count("prescan"))
				{
					ReScan::PreScan::PreScan preScan;
					if (isConfigSpecified)
					{
						std::string config = vm["config"].as<std::string>();
					}
					else
					{
						result = preScan.process();
					}
				}
				else
				{
					ReScan::ReScan reScan;
					if (isConfigSpecified)
					{
						std::string config = vm["config"].as<std::string>();
						result = reScan.process(config);
					}
					else if (isFileSpecified)
					{
						std::string file = vm["file"].as<std::string>();
						result = reScan.process(file, true);
					}
					else
					{
						std::string filename;
						std::cout << ".obj filename: ";
						std::getline(std::cin, filename);

						result = reScan.process(filename, true);
					}
				}

				if (result != SUCCESS_CODE)
				{
					std::cout << std::endl << "Press enter to exit..." << std::endl;
					std::cin.get();
				}
			}
		}
	}
	catch (std::exception& e)
	{
		std::cout << e.what() << "\n";
		result = 1;
	}

	return result;
}

#include "ReScan.h"
#include "ReScanConfig.h"
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
		po::options_description desc("*** ReScan Command Line Parser ***");
		desc.add_options()
			("help,h", "produce help message")
			("create-config-default,d", "create a new default config file: config.ini")
			("create-config-icnde-frontal,k", "create a new config file adapted for ICNDE (frontal): configFrontal.ini")
			("create-config-icnde-lateral,l", "create a new config file adapted for ICNDE (lateral): configLateral.ini")
			("config,c", po::value<std::string>()->value_name("config.ini"), "specify config file")
			("file,f", po::value<std::string>()->value_name("file.obj"), "specify obj file")
			;

		po::variables_map vm;
		po::store(po::parse_command_line(argc, argv, desc), vm);
		po::notify(vm);

		if (vm.count("help"))
		{
			std::cout << desc << std::endl;
			return 0;
		}

		bool cancelProcess = false;
		bool isConfigSpecified = vm.count("config") ? true : false;
		bool isFileSpecified = vm.count("file") ? true : false;

		if (vm.count("create-config"))
		{
			ReScan::ReScanConfig::saveConfigToFile(ReScan::ReScanConfig(), "config.ini");
			cancelProcess = true;
		}

		if (vm.count("create-config-icnde-frontal"))
		{
			ReScan::ReScanConfig::saveConfigToFile(ReScan::ReScanConfig::createFrontalICNDEConfig(), "configFrontal.ini");
			cancelProcess = true;
		}

		if (vm.count("create-config-icnde-lateral"))
		{
			ReScan::ReScanConfig::saveConfigToFile(ReScan::ReScanConfig::createLateralICNDEConfig(), "configLateral.ini");
			cancelProcess = true;
		}

		if (!cancelProcess)
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
				std::cout << "obj filename: ";
				std::getline(std::cin, filename);

				result = reScan.process(filename, true);
			}

			if (result != SUCCESS_CODE)
			{
				std::cout << std::endl << "Press enter to exit..." << std::endl;
				std::cin.get();
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

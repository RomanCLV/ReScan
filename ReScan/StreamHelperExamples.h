#pragma once

#include "ObservableOStream.h"
#include "MultiOStream.h"

#include <ostream>
#include <fstream>

static void example1();
static void example2();
static void example3();
static void example4();
static void example5();


static void example1()
{
	// utilisation d'un ObservableOStream

	std::ofstream log("example1.log");

	auto customCallback = [&log](const std::string& message)
		{
			log << message;
		};

	ReScan::StreamHelper::ObservableOStream ost; // creation d'un observable stream
	ost.subscribe(customCallback);

	// envoie d'un message dans ce stream (qui sera recupere dans la callback)
	ost << "Test qui sauvegarde dans un fichier grace a l'ObservableOStream." << std::endl;

	ost.unsubscribe(customCallback);
	log.close();
}

static void example2()
{
	// utilisation d'un ObservableOStream sur un stream sp�cifique

	std::ofstream log("example2.log");

	auto customCallback = [&log](const std::string& message)
		{
			log << message;
		};

	ReScan::StreamHelper::ObservableOStream ost(&std::cout); // creation d'un observable stream
	ost.subscribe(customCallback);

	// envoie d'un message dans ce stream (qui sera recuper� dans la callback et envoy� au stream observ�)
	ost << "Test qui affiche un message (dans std::cout) et le sauvegarde dans un fichier grace a l'ObservableOStream." << std::endl;

	ost.unsubscribe(customCallback);
	log.close();
}

static void example3()
{
	// utilisation de ReScan::mout

	// aucun stream ajout� au multi out, donc std::cout utilis� par d�faut
	ReScan::mout << "Test message envoye a std::cout par defaut" << std::endl;

	std::ofstream log("example3.log");
	ReScan::mout.add(&log);	// ajout d'un stream sp�cifique

	ReScan::mout << "Test message envoye au fichier log grace a son ajout (mais plus a std::cout)" << std::endl;

	ReScan::mout.add(&std::cout); // ajout du flux standard

	ReScan::mout << "Test message envoye au fichier log et a std::cout" << std::endl;

	ReScan::mout.remove(&std::cout);	// suppression du flux
	ReScan::mout.remove(&log);			// suppression du flux

	ReScan::mout << "Test message envoye a std::cout par defaut si aucun flux n'a ete ajoute" << std::endl;
}

static void example4()
{
	// utilisation d'un ObservableOStream sur un stream sp�cifique et de mout
	std::ofstream log("example4.log");

	auto customCallback = [&log](const std::string& message)
		{
			log << message;
		};

	ReScan::StreamHelper::ObservableOStream ost; // creation d'un observable stream
	ost.subscribe(customCallback);

	ReScan::mout.add(&ost);				// ajout du stream observe dans le multi out
	ReScan::mout.add(&std::cout);		// ajout de la sortie standard dans le multi out

	// envoie d'un message dans le multi out stream (qui  enverra � chaque stream ajout�)
	ReScan::mout << "Test qui affiche un message dans std::cout et qui le sauvegarde dans un fichier grace a l'ObservableOStream." << std::endl;

	ReScan::mout.remove(&ost);
	ReScan::mout.remove(&std::cout);

	ost.unsubscribe(customCallback);
	log.close();
}

static void example5()
{
	// meme chose que 4 mais mieux "optimis�"
	std::ofstream log("example5.log");

	auto customCallback = [&log](const std::string& message)
		{
			log << message;
		};

	ReScan::StreamHelper::ObservableOStream ost(&std::cout); // creation d'un observable stream sur le flux de sortie standard
	ost.subscribe(customCallback);

	ReScan::mout.add(&ost);				// ajout du stream observe dans le multi out

	// envoie d'un message dans le multi out stream (qui  enverra � chaque stream ajout�)
	// comme le seul stream ajout� est l'observable stream, il n'est envoy� qu'� lui
	// cependant ce stream redirige �galement le flux vers std::cout
	ReScan::mout << "Test qui affiche un message dans std::cout et qui le sauvegarde dans un fichier grace a l'ObservableOStream." << std::endl;

	ReScan::mout.remove(&ost);

	ost.unsubscribe(customCallback);
	log.close();
}

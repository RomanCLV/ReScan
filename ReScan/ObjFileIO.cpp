#include "ObjFileIO.h"
#include "MultiOStream.h"

#include <iostream>
#include <sstream>
#include <thread>
#include <chrono>
#include <fstream>

void objio::VerticesParser(std::string buffer, std::vector<float>* vertices)
{
	ReScan::mout << "In Vertices Parser\n";
	size_t found, foundendline, foundPrec = 0;
	std::string elementType("\nv ");
	std::string line, word;
	std::stringstream ss;
	int tmp = 0;
	do
	{
		found = buffer.find(elementType, foundPrec + elementType.size());
		if (found != std::string::npos)
		{
			foundendline = buffer.find("\n", found + 1);
			line = buffer.substr(found + 1, foundendline - found - 1);
			foundPrec = found;
			tmp++;
			ss << line;
			ss >> word >> word;
			vertices->push_back(stof(word));
			ss >> word;
			vertices->push_back(stof(word));
			ss >> word;
			vertices->push_back(stof(word));
		}
	} while (found != std::string::npos);
	ReScan::mout << "Vertices parser ended\n";
}


void objio::FacesParser(std::string buffer, std::vector<int>* triangles, std::vector<int>* uvtriangles)
{
	ReScan::mout << "In Faces Parser\n";
	size_t found, foundSeparator, foundendline, foundPrec = 0, foundSeparatorPrec;
	std::string elementType("\nf ");
	std::string line, word, value;
	std::stringstream ss;
	do
	{
		found = buffer.find(elementType, foundPrec + elementType.size()); //Find next 'f'
		if (found != std::string::npos)
		{
			foundendline = buffer.find("\n", found + 1); //Find end of the line
			line = buffer.substr(found + 1, foundendline - found - 1); //Extract line from string buffer

			//foundSeparator = word.find("/");

			foundPrec = found; //Saving f position so that the new search will start from there
			ss << line; //Putting string line in a stringstream in order to simplify the extraction
			ss >> word; //first word is 'f' so we skip that
			for (int i = 0; i < 3; i++)
			{
				ss >> word;
				value = word;
				foundSeparator = word.find("/");
				if (foundSeparator == std::string::npos)
				{
					triangles->push_back(stoi(value));
				}
				else
				{
					value.resize(foundSeparator);
					triangles->push_back(stoi(value));
					value = word;
					foundSeparatorPrec = foundSeparator;
					foundSeparator = word.find("/", foundSeparatorPrec + 1);
					if (foundSeparator == std::string::npos)
					{
						value = value.substr(foundSeparatorPrec + 1);
						uvtriangles->push_back(stoi(value));
					}
					else if (foundSeparator - foundSeparatorPrec > 1)
					{
						value = word;
						value = value.substr(foundSeparatorPrec, foundSeparator - foundSeparatorPrec);
						uvtriangles->push_back(stoi(value));
					}
				}
			}
			ss.str(std::string());
			ss.clear();
		}
	} while (found != std::string::npos);
	ReScan::mout << "Faces parser ended\n";
}


void objio::UVsParser(std::string buffer, std::vector<float>* uvs)
{
	ReScan::mout << "In UVs Parser\n";
	size_t found, foundendline, foundPrec = 0;
	std::string elementType("\nvt ");
	std::string line, word;
	std::stringstream ss;
	do
	{
		found = buffer.find(elementType, foundPrec + elementType.size()); //Find next 'vt'
		if (found != std::string::npos)
		{
			foundendline = buffer.find("\n", found + 1); //Find end of the line
			line = buffer.substr(found + 1, foundendline - found - 1); //Extract line from string buffer
			foundPrec = found; //Saving vt position so that the new search will start from there
			ss << line; //Putting string line in a stringstream in order to simplify the extraction
			ss >> word >> word; //first word is 'vt' so we skip that, second is 'u' value
			uvs->push_back(stof(word)); //putting 'u' value in vector
			ss >> word; //Extracting 'v' value
			uvs->push_back(stof(word)); //putting 'v' value in vector
		}
	} while (found != std::string::npos);
	ReScan::mout << "UVs parser ended\n";
}


void objio::readObjFileVertices(std::string& path, std::vector<float>* vertices)
{
	objio::_internalReadObjFile(path, vertices, nullptr, nullptr, nullptr);
}


void objio::readObjFileFaces(std::string& path, std::vector<int>* triangles, std::vector<float>* uvs)
{
	objio::_internalReadObjFile(path, nullptr, triangles, uvs, nullptr);
}


void objio::readObjFileUVs(std::string& path, std::vector<int>* uvtriangles)
{
	objio::_internalReadObjFile(path, nullptr, nullptr, nullptr, uvtriangles);
}

void objio::readObjFile(std::string& path,
    std::vector<float>* vertices,   \
    std::vector<int>* triangles,    \
    std::vector<float>* uvs,        \
    std::vector<int>* uvtriangles)
{
	objio::_internalReadObjFile(path, vertices, triangles, uvs, uvtriangles);
}

void objio::_internalReadObjFile(std::string& path,
	std::vector<float>* vertices, \
	std::vector<int>* triangles, \
	std::vector<float>* uvs, \
	std::vector<int>* uvtriangles)
{
	//Initialising timers and data structures
	auto _start = std::chrono::system_clock::now();
	auto _end = _start;
	std::chrono::duration<double> elapsed_seconds = _end - _start;

	std::stringstream ss;

	//The parser is using C implementation for perfomance purposes
	FILE* pFile;
	long lSize;
	char* buffer;
	size_t result;

	ReScan::mout << "File Path is : " << path << std::endl;

	fopen_s(&pFile, path.c_str(), "rb"); //Strange way to convert, there must be another way
	if (pFile == NULL)
	{
		fputs("File error\n", stderr);
		return;
	}

	// obtain file size:
	fseek(pFile, 0, SEEK_END);
	lSize = ftell(pFile);
	rewind(pFile);

	// allocate memory to contain the whole file:
	buffer = (char*)malloc(sizeof(char) * lSize);
	if (buffer == NULL)
	{
		fputs("Memory error", stderr);
		return;
	}

	ReScan::mout << "Reading...\n";

	// copy the file into the buffer:
	result = fread(buffer, 1, lSize, pFile);
	if (result != lSize)
	{
		fputs("Reading error", stderr);
		return;
	}

	ReScan::mout << "Read done\n";

	/* the whole file is now loaded in the memory buffer. */

	// terminate
	fclose(pFile);
	std::string bufferString = buffer;
	free(buffer);

	// By JC - before

	//Calling threads to fill vertices and triangles vectors
	//std::thread myThread1(objio::VerticesParser, bufferString, vertices);
	//std::thread myThread2(objio::FacesParser, bufferString, triangles, uvtriangles);
	//std::thread myThread3(objio::UVsParser, bufferString, uvs);

	//myThread1.join();
	//myThread2.join();
	//myThread3.join();

	// RC - 12/01/2024

	std::vector<std::thread*> threads;

	// vertices
	if (vertices != nullptr)
	{
		std::thread* t = new std::thread(objio::VerticesParser, bufferString, vertices);
		if (t == nullptr)
		{
			fputs("Memory error", stderr);
			return;
		}
		threads.push_back(t);
	}

	// faces
	if (triangles != nullptr && uvtriangles != nullptr)
	{
		std::thread* t = new std::thread(objio::FacesParser, bufferString, triangles, uvtriangles);
		if (t == nullptr)
		{
			fputs("Memory error", stderr);
			return;
		}
		threads.push_back(t);
	}

	// uvs
	if (uvs != nullptr)
	{
		std::thread* t = new std::thread(objio::UVsParser, bufferString, uvs);
		if (t == nullptr)
		{
			fputs("Memory error", stderr);
			return;
		}
		threads.push_back(t);
	}

	for (int i = 0; i < threads.size(); i++)
	{
		threads[i]->join();
	}

	_end = std::chrono::system_clock::now();
	elapsed_seconds = _end - _start;
	ReScan::mout << "File read in : " << elapsed_seconds.count() << " seconds\n";
	for (int i = 0; i < threads.size(); i++)
	{
		delete threads[i];
	}
}

void objio::writeObjFile(std::string& path,							\
	const std::vector<float>* vertices,								\
	const std::vector<int>* triangles)
{
	std::ofstream file(path);
	file << "# File exported by Bayab Industries\n\n";
	int i;

	//Write vertices
	for(i = 0; i<vertices->size(); i+=3)
		file << "v " << vertices->at(i) << " " << vertices->at(i+1) << " " << vertices->at(i+2) << "\n";

	//Write faces
	for (i = 0; i < triangles->size(); i += 3)
		file << "f " << triangles->at(i) << " " << triangles->at(i + 1) << " " << triangles->at(i + 2) << "\n";
}
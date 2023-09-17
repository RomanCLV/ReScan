#pragma once
#include <vector>
#include <string>

namespace objio
{
    void VerticesParser(std::string buffer, std::vector<float>* vertices);
    void FacesParser(std::string buffer, std::vector<int>* triangles, std::vector<int>* uvstriangles);
    void UVsParser(std::string buffer, std::vector<float>* uvs);
    void readObjFile( std::string path, \
        std::vector<float>* vertices,   \
        std::vector<int>* triangles,    \
        std::vector<float>* uvs,        \
        std::vector<int>* uvtriangles);
    void writeObjFile(std::string path,         \
        const std::vector<float>* vertices,     \
        const std::vector<int>* triangles);
}
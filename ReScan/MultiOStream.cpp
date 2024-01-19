#include "MultiOStream.h"

namespace ReScan
{
    StreamHelper::MultiOStream mout;
}

namespace ReScan::StreamHelper
{
    MultiOStream::MultiOStream() : 
        std::ostream(nullptr),
        m_useCoutIfNoStream(true)
    {
    }

    MultiOStream::MultiOStream(const MultiOStream& other) :
        std::ostream(nullptr),
        m_useCoutIfNoStream(other.m_useCoutIfNoStream)
    {
        m_outs = other.m_outs;
    }

    MultiOStream::~MultiOStream()
    {
    }

    MultiOStream& MultiOStream::operator=(const MultiOStream& other)
    {
        if (this != &other)
        {
            m_outs = other.m_outs;
        }
        return *this;
    }

    void MultiOStream::setUseCoutIfNoStream(const bool value)
    {
        m_useCoutIfNoStream = value;
    }

    void MultiOStream::add(std::ostream* stream)
    {
        m_outs.push_back(stream);
    }

    void MultiOStream::remove(std::ostream* stream)
    {
        auto it = std::find(m_outs.begin(), m_outs.end(), stream);
        if (it != m_outs.end())
        {
            m_outs.erase(it);
        }
    }

    void MultiOStream::clear()
    {
        m_outs.clear();
    }
}
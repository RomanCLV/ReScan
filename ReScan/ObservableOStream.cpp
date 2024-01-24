#include "ObservableOStream.h"

namespace ReScan::StreamHelper
{
    ObservableOStream::ObservableOStream() :
        m_buffer(this),
        std::ostream(&m_buffer)
    {
    }

    ObservableOStream::ObservableOStream(std::ostream* wrappedStream) :
        m_buffer(wrappedStream, this),
        std::ostream(&m_buffer)
    {
    }

    ObservableOStream::~ObservableOStream()
    {
        m_subscribers.clear();
    }

    void ObservableOStream::subscribe(EventCallback callback)
    {
        m_subscribers.push_back(callback);
    }

    void ObservableOStream::unsubscribe(EventCallback callback)
    {
        auto it = std::find_if(m_subscribers.begin(), m_subscribers.end(),
            [callback](const EventCallback& cb) { return cb.target<void(const std::string&)>() == callback.target<void(const std::string&)>(); });

        if (it != m_subscribers.end())
        {
            m_subscribers.erase(it);
        }
    }
}
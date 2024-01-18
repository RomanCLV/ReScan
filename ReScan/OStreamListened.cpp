#include "OStreamListened.h"

OStreamListened::OStreamListened(std::ostream& listenedStream) : 
    std::ostream(listenedStream.rdbuf()), 
    m_listenedStream(listenedStream)
{
}

void OStreamListened::addListener(EventCallback listener) 
{
    std::lock_guard<std::mutex> lock(m_mutex);
    m_listeners.push_back(listener);
}

void OStreamListened::removeListener(EventCallback listener) 
{
    std::lock_guard<std::mutex> lock(m_mutex);
    auto it = std::find_if(m_listeners.begin(), m_listeners.end(),
        [listener](const EventCallback& cb) { return cb.target<void(const std::string&)>() == listener.target<void(const std::string&)>(); });

    if (it != m_listeners.end())
    {
        m_listeners.erase(it);
    }
}

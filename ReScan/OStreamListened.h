#pragma once

#include <iostream>
#include <fstream>
#include <sstream>
#include <functional>
#include <vector>
#include <string>
#include <mutex>

class OStreamListened : public std::ostream 
{
public:
    OStreamListened(std::ostream& listenedStream);

    using EventCallback = std::function<void(const std::string&)>;

private:
    std::ostream& m_listenedStream;
    std::vector<EventCallback> m_listeners;
    std::mutex m_mutex;

    template <typename T>
    void notifyListeners(const T& value)
    {
        std::lock_guard<std::mutex> lock(m_mutex);
        for (const auto& listener : m_listeners) 
        {
            std::ostringstream oss;
            oss << value;
            listener(oss.str());
        }
    }

public:
    template <typename T>
    OStreamListened& operator<<(const T& value) 
    {
        // Intercept the output operation
        notifyListeners(value);

        // Forward the operation to the base class
        m_listenedStream << value;

        return *this;
    }

    void addListener(EventCallback listener);
    void removeListener(EventCallback listener);
};


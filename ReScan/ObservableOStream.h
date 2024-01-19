#ifndef RESCAN_OBSERVABLE_OSTREAM_H
#define RESCAN_OBSERVABLE_OSTREAM_H

#include <iostream>
#include <sstream>
#include <vector>
#include <functional>
#include <algorithm>

namespace ReScan::StreamHelper
{
    class ObservableOStream : public std::ostream 
    {
    public:
        using EventCallback = std::function<void(const std::string&)>;

        ObservableOStream(std::ostream& wrappedStream) : 
            std::ostream(&buffer), 
            buffer(wrappedStream, this)
        {
        }

        void subscribe(EventCallback callback)
        {
            subscribers.push_back(callback);
        }

        void unsubscribe(EventCallback callback)
        {
            auto it = std::find_if(subscribers.begin(), subscribers.end(),
                [callback](const EventCallback& cb) { return cb.target<void(const std::string&)>() == callback.target<void(const std::string&)>(); });

            if (it != subscribers.end())
            {
                subscribers.erase(it);
            }
        }

    private:
        class ObservableBuffer : public std::stringbuf 
        {
        public:
            explicit ObservableBuffer(std::ostream& wrappedStream, ObservableOStream* observableStream) :
                wrappedStream(wrappedStream),
                m_observableStream(observableStream)
            {
            }

            int sync() override
            {
                int result = wrappedStream.rdbuf()->pubsync();
                notifyObservers(str());
                str(""); // Réinitialiser le tampon interne
                return result;
            }

            // Méthode pour notifier les abonnés
            void notifyObservers(const std::string& message) 
            {
                auto subscribers = m_observableStream->subscribers;
                for (const auto& subscriber : subscribers)
                {
                    subscriber(message);
                }
            }

        private:
            ObservableOStream* m_observableStream;
            std::ostream& wrappedStream;
        };

        ObservableBuffer buffer;
        std::vector<std::function<void(const std::string&)>> subscribers;
    };
}

#endif // RESCAN_OBSERVABLE_OSTREAM_H

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

        ObservableOStream();
        ObservableOStream(std::ostream* wrappedStream);
        ~ObservableOStream();

        void subscribe(EventCallback callback);
        void unsubscribe(EventCallback callback);

    private:
        class ObservableBuffer : public std::stringbuf 
        {
        public:
            explicit ObservableBuffer(ObservableOStream* observableStream) :
                m_observableStream(observableStream),
                m_wrappedStream(nullptr)
            {
            }

            explicit ObservableBuffer(std::ostream* wrappedStream, ObservableOStream* observableStream) :
                m_observableStream(observableStream),
                m_wrappedStream(wrappedStream)
            {
            }

            int sync() override
            {
                int result = 0;
                if (m_wrappedStream) 
                {
                    result = m_wrappedStream->rdbuf()->pubsync();
                }
                notifyObservers(str());
                str(""); // Réinitialiser le tampon interne
                return result;
            }

            // Méthode pour notifier les abonnés
            void notifyObservers(const std::string& message) 
            {
                auto subscribers = m_observableStream->m_subscribers;
                for (const auto& subscriber : subscribers)
                {
                    subscriber(message);
                }
            }

        private:
            ObservableOStream* m_observableStream;
            std::ostream* m_wrappedStream;
        };

        ObservableBuffer m_buffer;
        std::vector<std::function<void(const std::string&)>> m_subscribers;
    };
}

#endif // RESCAN_OBSERVABLE_OSTREAM_H

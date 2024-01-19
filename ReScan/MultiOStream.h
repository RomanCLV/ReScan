#ifndef RESCAN_STREAMHELPER_H
#define RESCAN_STREAMHELPER_H

#include <vector>
#include <iostream>
#include <ostream>
#include <istream>

namespace ReScan::StreamHelper
{
	class MultiOStream : public std::ostream
	{
	private:
		std::vector<std::ostream*> m_outs;
		bool m_useCoutIfNoStream;

	public:
        MultiOStream();
        MultiOStream(const MultiOStream& other);
		~MultiOStream();

		void setUseCoutIfNoStream(const bool value);

		void add(std::ostream* stream);
		void remove(std::ostream* stream);
		void clear();

		MultiOStream& operator=(const MultiOStream& other);

        template <typename T>
        MultiOStream& operator<<(const T& value)
        {
            if (m_outs.size() == 0)
            {
                if (m_useCoutIfNoStream)
                {
                    std::cout << value;
                }
            }
            else
            {
                for (std::ostream* stream : m_outs)
                {
                    (*stream) << value;
                }
            }
            return *this;
        }

        MultiOStream& operator<<(std::ostream& (*manipulator)(std::ostream&))
        {
            if (m_outs.size() == 0)
            {
                if (m_useCoutIfNoStream)
                {
                    manipulator(std::cout);
                }
            }
            else
            {
                for (std::ostream* stream : m_outs)
                {
                    manipulator(*stream);
                }
            }
            return *this;
        }
	};
}

namespace ReScan
{
    extern StreamHelper::MultiOStream mout;
}

#endif // !RESCAN_STREAMHELPER_H

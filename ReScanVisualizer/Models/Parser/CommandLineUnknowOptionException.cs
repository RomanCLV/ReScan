using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.Models.Parser
{
    internal class CommandLineUnknowOptionException : ArgumentException
    {
        public CommandLineUnknowOptionException() : base("Unknow parameter given.")
        {
        }

        public CommandLineUnknowOptionException(string paramName) : base("Unknow parameter given.", paramName)
        {
        }

        public CommandLineUnknowOptionException(string paramName, Exception innerException) : base("Unknow parameter given.", paramName, innerException)
        {
        }

        protected CommandLineUnknowOptionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

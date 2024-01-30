using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.Models.CommandLineParser.Options
{
    public abstract class CommandLineOptionBase
    {
        public static string Key => "-?";
        public static string FullKey => "--?";
        public static string Keys => Key + " | " + FullKey;
        public static string Description => "Description";
        public static uint MinimumParameters => 0;
        public static uint MaximumParameters => 0;
        public static List<CommandLineParameterBase> Parameters { get; } = new List<CommandLineParameterBase>();

        public CommandLineOptionBase()
        {
        }

        public static string ParametersNames()
        {
            return string.Empty;
        }

        protected static string ParametersNames(List<CommandLineParameterBase> parameters)
        {
            string str = string.Empty;

            for (int i = 0; i < parameters.Count; i++)
            {
                if (i != 0)
                {
                    str += ' ';
                }
                if (!parameters[i].IsRequired)
                {
                    str += '[';
                }
                str += parameters[i].Name;
                if (!parameters[i].IsRequired)
                {
                    str += ']';
                }
            }

            return str;
        }
    }
}

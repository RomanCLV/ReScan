using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.Models.Parser.Options
{
    public class CommandLineOptionHelp : CommandLineOptionBase
    {
        public const string KEY = "-h";
        public const string FULL_KEY = "--help";

        public new static string Key => KEY;
        public new static string FullKey => FULL_KEY;
        public new static string Keys => Key + " | " + FullKey;
        public new static string Description => "Display general help or help of the specified command. Example: -h abs";
        public new static uint MinimumParameters => 0;
        public new static uint MaximumParameters => 1;
        public static CommandLineParameter<string> CommandParameter { get; } = new CommandLineParameter<string>("filePath", "Path of the CSV file that contains bases (in cartesians) to load.");

        public new static List<CommandLineParameterBase> Parameters { get; } = new List<CommandLineParameterBase>()
        {
            CommandParameter
        };

        public string Command { get; }

        public CommandLineOptionHelp() : this(new List<string>())
        {
        }

        public CommandLineOptionHelp(List<string> args)
        {
            if (MaximumParameters == 0 && args.Count != 0)
            {
                throw new ArgumentException($"{GetType().Name}: Too many parameters - No parameter expected");
            }
            if (args.Count < MinimumParameters)
            {
                throw new ArgumentException($"{GetType().Name}: Too few parameters - minimum: {MinimumParameters} - maximum: {MaximumParameters}");
            }
            if (args.Count > MaximumParameters)
            {
                throw new ArgumentException($"{GetType().Name}: Too many parameters - minimum: {MinimumParameters} - maximum: {MaximumParameters}");
            }

            Command = string.Empty;
            if (args.Count >= 1)
            {
                Command = args[0].ToLower();
            }
        }

        public new static string ParametersNames()
        {
            return ParametersNames(Parameters);
        }
    }
}

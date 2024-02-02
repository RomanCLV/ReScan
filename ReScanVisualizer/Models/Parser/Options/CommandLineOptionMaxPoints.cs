using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.Models.Parser.Options
{
    public class CommandLineOptionMaxPoints : CommandLineOptionBase
    {
        public const string KEY = "-mp";
        public const string FULL_KEY = "--max-points";

        public new static string Key => KEY;
        public new static string FullKey => FULL_KEY;
        public new static string Keys => Key + " | " + FullKey;
        public new static string Description => "Set the max points of graph added by command line";
        public new static uint MinimumParameters => 1;
        public new static uint MaximumParameters => 1;
        public static CommandLineParameter<string> OptionParameter { get; } = new CommandLineParameter<string>("option", "option: reset (or a 0-negative number) or a positive integer. Examples: -mp reset | -mp 50");

        public new static List<CommandLineParameterBase> Parameters { get; } = new List<CommandLineParameterBase>()
        {
            OptionParameter
        };

        public bool ToReset { get; }
        public uint MaxPoints { get; }

        public CommandLineOptionMaxPoints(List<string> args)
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

            string option = args[0].ToLower();
            if (option == "reset")
            {
                ToReset = true;
            }
            else
            {
                if (int.TryParse(option, out int maxPoints))
                {
                    if (maxPoints <= 0)
                    {
                        ToReset = true;
                    }
                    else
                    {
                        MaxPoints = (uint)maxPoints;
                    }
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Wrong max points option given. Must be reset (or a 0-negative number) or a positive integer. Given: " + args[0], OptionParameter.Name);
                }
            }
        }

        public new static string ParametersNames()
        {
            return ParametersNames(Parameters);
        }
    }
}

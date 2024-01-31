using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.Models.Parser.Options
{
    public class CommandLineOptionUDP : CommandLineOptionBase
    {
        public const string KEY = "-udp";
        public const string FULL_KEY = "--udp";

        public new static string Key => KEY;
        public new static string FullKey => FULL_KEY;
        public new static string Keys => Key + " | " + FullKey;
        public new static string Description => "Start an UDP listener on specified port";
        public new static uint MinimumParameters => 1;
        public new static uint MaximumParameters => 1;
        public static CommandLineParameter<ushort> PortParameter { get; } = new CommandLineParameter<ushort>("port", "Receiver port", true);

        public new static List<CommandLineParameterBase> Parameters { get; } = new List<CommandLineParameterBase>()
        {
            PortParameter
        };

        public ushort Port { get; }

        public CommandLineOptionUDP() : this (new List<string>(0))
        { 
        }

        public CommandLineOptionUDP(List<string> args)
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

            if (ushort.TryParse(args[0], out ushort port))
            {
                Port = port;
            }
            else
            {
                throw new ArgumentException(GetType().Name + ": Cannot parse " + args[0] + " into a ushort", PortParameter.Name);
            }
        }

        public new static string ParametersNames()
        {
            return ParametersNames(Parameters);
        }
    }
}

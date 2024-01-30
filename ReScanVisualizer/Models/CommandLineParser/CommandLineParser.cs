using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReScanVisualizer.Models.CommandLineParser.Options;

#nullable enable

namespace ReScanVisualizer.Models.CommandLineParser
{
    public class CommandLineParser
    {
        private readonly List<CommandLineOptionBase> _items;
        public IReadOnlyList<CommandLineOptionBase> Items => _items;
        
        private CommandLineParser() 
        {
            _items = new List<CommandLineOptionBase>();
        }

        public static CommandLineParser Parse(string[] args)
        {
            CommandLineParser commandLineParser = new CommandLineParser();

            List<string>? argParams = null;
            Type? paramType = null;
            Type? type;
            foreach (string arg in args)
            {
                type = GetTypeOfKey(arg);
                if (type == null)
                {
                    if (argParams == null)
                    {
                        throw new CommandLineUnknowOptionException(arg);
                    }
                    else
                    {
                        argParams.Add(arg);
                    }
                }
                else
                {
                    if (argParams == null)
                    {
                        paramType = type;
                        argParams = new List<string>();
                    }
                    else
                    {
                        if (!commandLineParser.HasValue<CommandLineOptionHelp>() || paramType != typeof(CommandLineOptionHelp))
                        {
                            commandLineParser._items.Add((CommandLineOptionBase)Activator.CreateInstance(paramType!, argParams!));
                        }
                        paramType = null;
                        argParams = null;
                    }
                }
            }
            if (paramType != null)
            {
                if (!commandLineParser.HasValue<CommandLineOptionHelp>() || paramType != typeof(CommandLineOptionHelp))
                {
                    commandLineParser._items.Add((CommandLineOptionBase)Activator.CreateInstance(paramType!, argParams!));
                }
            }
            return commandLineParser;
        }

        private static Type? GetTypeOfKey(string key)
        {
            if (key == CommandLineOptionHelp.KEY || key == CommandLineOptionHelp.FULL_KEY)
            {
                return typeof(CommandLineOptionHelp);
            }
            else if (key == CommandLineOptionAddBases.KEY || key == CommandLineOptionAddBases.FULL_KEY)
            {
                return typeof(CommandLineOptionAddBases);
            }
            else
            {
                return null;
            }
        }

        public bool HasValue<T>() 
            where T : CommandLineOptionBase
        {
            return _items.Any(x => x.GetType()  == typeof(T));
        }

        public T Get<T>()
            where T : CommandLineOptionBase
        {
            return (T)_items.Find(x => x.GetType() == typeof(T));
        }

        internal static string Help()
        {
            string help = "Usage:\n";

            help += CommandLineOptionHelp.Keys + '\n';
            help += CommandLineOptionHelp.ParametersNames() + '\n';
            help += CommandLineOptionHelp.Description + "\n\n";

            help += CommandLineOptionAddBases.Keys + '\n';
            help += CommandLineOptionAddBases.ParametersNames() + '\n';
            help += CommandLineOptionAddBases.Description + "\n\n";

            return help;
        }

        public static List<string> HelpOption(string option)
        {
            Type? type = GetTypeOfKey(option);
            if (type == null)
            {
                return new List<string>() { "Unknowed option: " + option };
            }
            return HelpOption(type);
        }

        public static List<string> HelpOption(Type type)
        {
            List<string> help;
            if (type == typeof(CommandLineOptionHelp))
            {
                help = HelpOption(
                    CommandLineOptionHelp.Keys,
                    CommandLineOptionHelp.ParametersNames(),
                    CommandLineOptionHelp.Description,
                    CommandLineOptionHelp.Parameters);

            }
            else if (type == typeof(CommandLineOptionAddBases))
            {
                help = HelpOption(
                    CommandLineOptionAddBases.Keys,
                    CommandLineOptionAddBases.ParametersNames(),
                    CommandLineOptionAddBases.Description,
                    CommandLineOptionAddBases.Parameters);
            }
            else
            {
                throw new ArgumentException("Unexpected type given: " + type.Name);
            }
            return help;
        }

        private static List<string> HelpOption(string keys, string parametersNames, string description, List<CommandLineParameterBase> parameters)
        {
            List<string> help = new List<string>(4)
            {
                keys,
                parametersNames,
                description,
                "\n"
            };
            foreach (CommandLineParameterBase parameter in parameters)
            {
                help.Add($"{parameter.Name}: {parameter.Type.Name} - {parameter.Description} - Required: {parameter.IsRequired}");
            }
            return help;
        }
    }
}

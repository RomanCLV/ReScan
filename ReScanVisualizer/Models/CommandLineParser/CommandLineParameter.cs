using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ReScanVisualizer.Models.CommandLineParser
{
    public abstract class CommandLineParameterBase
    {
        private readonly string _name;
        public string Name => _name;

        private readonly string _description;
        public string Description => _description;

        public readonly bool _isRequired;
        public bool IsRequired => _isRequired;

        private readonly Type _type;
        public Type Type => _type;

        public CommandLineParameterBase(Type type, string name, string description, bool isRequired)
        {
            _type = type;
            _name = name;
            _description = description;
            _isRequired = isRequired;
        }
    }

    public class CommandLineParameter<T> : CommandLineParameterBase
    {
        public CommandLineParameter(string name, string description, bool isRequired = false) : base(typeof(T), name, description, isRequired)
        {
        }
    }
}

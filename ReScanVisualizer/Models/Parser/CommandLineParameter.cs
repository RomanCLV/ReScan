using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

#nullable enable

namespace ReScanVisualizer.Models.Parser
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

        private readonly object? _defaultValue;
        public object? DefaultValue => _defaultValue;

        /// <summary>
        /// Construct a command line parameter that is REQUIRED. Its default value is NULL.
        /// </summary>
        /// <param name="type">Type of data</param>
        /// <param name="name">Name of the parameter</param>
        /// <param name="description">Description of the parameter</param>
        public CommandLineParameterBase(Type type, string name, string description)
        {
            _type = type;
            _name = name;
            _description = description;
            _isRequired = true;
            _defaultValue = null;
        }

        /// <summary>
        /// Construct a command line parameter that is NOT REQUIRED. Its default value is given.
        /// </summary>
        /// <param name="type">Type of data</param>
        /// <param name="name">Name of the parameter</param>
        /// <param name="description">Description of the parameter</param>
        /// <param name="defaultValue">Default value</param>
        public CommandLineParameterBase(Type type, string name, string description, object defaultValue)
        {
            _type = type;
            _name = name;
            _description = description;
            _defaultValue = defaultValue;
        }
    }

    public class CommandLineParameter<T> : CommandLineParameterBase
    {
        public new T DefaultValue => (T)base.DefaultValue!;

        public CommandLineParameter(string name, string description) : base(typeof(T), name, description)
        {
        }

        public CommandLineParameter(string name, string description, T defaultValue) : base(typeof(T), name, description, defaultValue!)
        {
        }
    }
}

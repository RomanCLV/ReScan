using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.Models.Parser.Options
{
    public class CommandLineOptionAddGraph : CommandLineOptionBase
    {
        public const string KEY = "-ag";
        public const string FULL_KEY = "--add-graph";

        public new static string Key => KEY;
        public new static string FullKey => FULL_KEY;
        public new static string Keys => Key + " | " + FullKey;
        public new static string Description => "Add a graph";
        public new static uint MinimumParameters => 2;
        public new static uint MaximumParameters => 10;

        private static readonly string[] _extentions = new string[]
        {
            "csv"
        };

        private static readonly string _joinedExtentions = string.Join(";", _extentions);

        public static IReadOnlyList<string> Extentions => _extentions;

        public static CommandLineParameter<string> TypeParameter { get; } = new CommandLineParameter<string>("type", "file type. Accecpted: " + _joinedExtentions, true);
        public static CommandLineParameter<string> FilePathParameter { get; } = new CommandLineParameter<string>("filePath", "file path", true);
        public static CommandLineParameter<bool> ContainsHeaderParameter { get; } = new CommandLineParameter<bool>("containsHeader", "If file contains headers: true|t|1 - else: false|f|0");
        public static CommandLineParameter<double> ScaleFactorParameter { get; } = new CommandLineParameter<double>("scaleFactor", "");
        public static CommandLineParameter<double> AxisScaleFactorParameter { get; } = new CommandLineParameter<double>("axisScaleFactor", "");
        public static CommandLineParameter<double> PointRadiusParameter { get; } = new CommandLineParameter<double>("pointRadius", "");
        public static CommandLineParameter<bool> DisplayBarycenterParameter { get; } = new CommandLineParameter<bool>("displayBarycenter", "");
        public static CommandLineParameter<bool> DisplayAveragePlanParameter { get; } = new CommandLineParameter<bool>("displayAverageplan", "");
        public static CommandLineParameter<bool> DisplayBaseParameter { get; } = new CommandLineParameter<bool>("displayBase", "");
        public static CommandLineParameter<RenderQuality> RenderQualityParameter { get; } = new CommandLineParameter<RenderQuality>("renderQuality", "");

        public new static List<CommandLineParameterBase> Parameters { get; } = new List<CommandLineParameterBase>()
        {
            TypeParameter,
            FilePathParameter,
            ContainsHeaderParameter,
            ScaleFactorParameter,
            AxisScaleFactorParameter,
            PointRadiusParameter,
            DisplayBarycenterParameter,
            DisplayAveragePlanParameter,
            DisplayBaseParameter,
            RenderQualityParameter
        };

        public string Type { get; }
        public string FilePath { get; }
        public bool ContainsHeader { get; }
        public double ScaleFactor { get; }
        public double AxisScaleFactor { get; }
        public double PointRadius { get; }
        public bool DisplayBarycenter { get; }
        public bool DisplayAveragePlan { get; }
        public bool DisplayBase { get; }
        public RenderQuality RenderQuality { get; }

        public CommandLineOptionAddGraph(List<string> args)
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

            Type = args[0].ToLower();
            if (!_extentions.Contains(Type))
            {
                throw new ArgumentException(GetType().Name + ": wrong type: " + Type + ". Must be : " + _joinedExtentions, TypeParameter.Name);
            }

            FilePath = args[1];

            if (args.Count >= 3)
            {
                if (Tools.TryParse(args[2], out bool tmp))
                {
                    ContainsHeader = tmp;
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[2] + " into a boolean", ContainsHeaderParameter.Name);
                }
            }
            else
            {
                ContainsHeader = true;
            }

            if (args.Count >= 4)
            {
                if (Tools.TryParse(args[3], out double tmp))
                {
                    ScaleFactor = tmp;
                    if (ScaleFactor <= 0.0)
                    {
                        throw new ArgumentException(GetType().Name + ": Scale factor must be positive. Given: " + ScaleFactor, ScaleFactorParameter.Name);
                    }
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[3] + " into a double", ScaleFactorParameter.Name);
                }
            }
            else
            {
                ScaleFactor = 1.0;
            }

            if (args.Count >= 5)
            {
                if (Tools.TryParse(args[4], out double tmp))
                {
                    AxisScaleFactor = tmp;
                    if (AxisScaleFactor <= 0.0)
                    {
                        throw new ArgumentException(GetType().Name + ": Axis scale factor must be positive. Given: " + AxisScaleFactor, AxisScaleFactorParameter.Name);
                    }
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[4] + " into a double", AxisScaleFactorParameter.Name);
                }
            }
            else
            {
                AxisScaleFactor = 1.0;
            }

            if (args.Count >= 6)
            {
                if (Tools.TryParse(args[5], out double tmp))
                {
                    PointRadius = tmp;
                    if (PointRadius <= 0.0)
                    {
                        throw new ArgumentException(GetType().Name + ": Point radius must be positive. Given: " + PointRadius, PointRadiusParameter.Name);
                    }
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[5] + " into a double", PointRadiusParameter.Name);
                }
            }
            else
            {
                PointRadius = 0.25;
            }

            if (args.Count >= 7)
            {
                if (Tools.TryParse(args[6], out bool tmp))
                {
                    DisplayBarycenter = tmp;
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[6] + " into a boolean", DisplayBarycenterParameter.Name);
                }
            }
            else
            {
                DisplayBarycenter = true;
            }

            if (args.Count >= 8)
            {
                if (Tools.TryParse(args[7], out bool tmp))
                {
                    DisplayAveragePlan = tmp;
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[7] + " into a boolean", DisplayAveragePlanParameter.Name);
                }
            }
            else
            {
                DisplayAveragePlan = true;
            }

            if (args.Count >= 9)
            {
                if (Tools.TryParse(args[8], out bool tmp))
                {
                    DisplayBase = tmp;
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[8] + " into a boolean", DisplayBaseParameter.Name);
                }
            }
            else
            {
                DisplayBase = true;
            }

            if (args.Count >= 10)
            {
                if (Tools.TryParse(args[9], out RenderQuality tmp))
                {
                    RenderQuality = tmp;
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[9] + " into a RenderQuality", RenderQualityParameter.Name);
                }
            }
            else
            {
                RenderQuality = RenderQuality.High;
            }
        }

        public new static string ParametersNames()
        {
            return ParametersNames(Parameters);
        }
    }
}

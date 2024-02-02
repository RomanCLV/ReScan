using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.Models.Parser.Options
{
    public class CommandLineOptionAddBases : CommandLineOptionBase
    {
        public const string KEY = "-abs";
        public const string FULL_KEY = "--add-bases";

        public new static string Key => KEY;
        public new static string FullKey => FULL_KEY;
        public new static string Keys => Key + " | " + FullKey;
        public new static string Description => "Read a CSV file and add the contained bases";
        public new static uint MinimumParameters => 1;
        public new static uint MaximumParameters => 5;
        public static CommandLineParameter<string> FilePathParameter { get; } = new CommandLineParameter<string>("filePath", "Path of the CSV file that contains bases (in cartesians) to load.");
        public static CommandLineParameter<bool> ContainsHeaderParameter { get; } = new CommandLineParameter<bool>("containsHeader", "If the file contains headers: true|t|1 - else: false|f|0", true);
        public static CommandLineParameter<double> ScaleFactorParameter { get; } = new CommandLineParameter<double>("scaleFactor", "Scale factor to apply. Must be greater than 0. Example: 0.02 | 1.5", 1.0);
        public static CommandLineParameter<double> AxisScaleFactorParameter { get; } = new CommandLineParameter<double>("axisScaleFactor", "Axis scale factor to apply. Must be greater than 0. Example: 0.5 | 2", 1.0);
        public static CommandLineParameter<RenderQuality> RenderQualityParameter { get; } = new CommandLineParameter<RenderQuality>("renderQuality", "Render quality: " + string.Join("|", Tools.GetRenderQualitiesList().ToArray()), RenderQuality.High);

        public new static List<CommandLineParameterBase> Parameters { get; } = new List<CommandLineParameterBase>()
        {
            FilePathParameter, ContainsHeaderParameter, ScaleFactorParameter, AxisScaleFactorParameter, RenderQualityParameter
        };

        public string FilePath { get; }
        public bool ContainsHeader { get; }
        public double ScaleFactor { get; }
        public double AxisScaleFactor { get; }
        public RenderQuality RenderQuality { get; }

        public CommandLineOptionAddBases(List<string> args)
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

            FilePath = args[0];

            if (args.Count >= 2)
            {
                if (Tools.TryParse(args[1], out bool tmp))
                {
                    ContainsHeader = tmp;
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[1] + " into a boolean", ContainsHeaderParameter.Name);
                }
            }
            else
            {
                ContainsHeader = ContainsHeaderParameter.DefaultValue;
            }

            if (args.Count >= 3)
            {
                if (Tools.TryParse(args[2], out double tmp))
                {
                    ScaleFactor = tmp;
                    if (ScaleFactor <= 0.0)
                    {
                        throw new ArgumentException(GetType().Name + ": Scale factor must be positive. Given: " + ScaleFactor, ScaleFactorParameter.Name);
                    }
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[2] + " into a double", ScaleFactorParameter.Name);
                }
            }
            else
            {
                ScaleFactor = ScaleFactorParameter.DefaultValue;
            }

            if (args.Count >= 4)
            {
                if (Tools.TryParse(args[3], out double tmp))
                {
                    AxisScaleFactor = tmp;
                    if (AxisScaleFactor <= 0.0)
                    {
                        throw new ArgumentException(GetType().Name + ": Scale factor must be positive. Given: " + AxisScaleFactor, AxisScaleFactorParameter.Name);
                    }
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[3] + " into a double", AxisScaleFactorParameter.Name);
                }
            }
            else
            {
                AxisScaleFactor = AxisScaleFactorParameter.DefaultValue;
            }

            if (args.Count >= 5)
            {
                if (Tools.TryParse(args[4], out RenderQuality tmp))
                {
                    RenderQuality = tmp;
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[4] + " into a RenderQuality", RenderQualityParameter.Name);
                }
            }
            else
            {
                RenderQuality = RenderQualityParameter.DefaultValue;
            }
        }

        public new static string ParametersNames()
        {
            return ParametersNames(Parameters);
        }
    }
}

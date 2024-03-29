﻿using System;
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
        public new static uint MaximumParameters => 11;

        private static readonly string[] _extentions = new string[]
        {
            "csv"
        };

        private static readonly string _joinedExtentions = string.Join(";", _extentions);

        public static IReadOnlyList<string> Extentions => _extentions;

        public static CommandLineParameter<string> TypeParameter { get; } = new CommandLineParameter<string>("type", "file type. Supported files: " + _joinedExtentions);
        public static CommandLineParameter<string> FilePathParameter { get; } = new CommandLineParameter<string>("filePath", "file path");
        public static CommandLineParameter<bool> ContainsHeaderParameter { get; } = new CommandLineParameter<bool>("containsHeader", "If the file contains headers: true|t|1 - else: false|f|0", true);
        public static CommandLineParameter<double> ScaleFactorParameter { get; } = new CommandLineParameter<double>("scaleFactor", "scale factor to apply for the visual representation of the graph", 1.0);
        public static CommandLineParameter<double> AxisScaleFactorParameter { get; } = new CommandLineParameter<double>("axisScaleFactor", "scale factor to apply for the visual representation of the graph's base", 1.0);
        public static CommandLineParameter<double> PointRadiusParameter { get; } = new CommandLineParameter<double>("pointRadius", "the point's radius to apply for the visual representation of each point", 0.25);
        public static CommandLineParameter<int> MaxPointsDisplayedParameter { get; } = new CommandLineParameter<int>("maxPointsDisplayed", "the number of maximum points that will be displayed. Negative value to disable it or a 0-positive number to set a limit.", -1);
        public static CommandLineParameter<bool> DisplayBarycenterParameter { get; } = new CommandLineParameter<bool>("displayBarycenter", "show or hide the barycenter of a graph. true|t|1 or false|f|0", true);
        public static CommandLineParameter<bool> DisplayAveragePlanParameter { get; } = new CommandLineParameter<bool>("displayAverageplan", "show or hide the average plan of a graph. true|t|1 or false|f|0", true);
        public static CommandLineParameter<bool> DisplayBaseParameter { get; } = new CommandLineParameter<bool>("displayBase", "show or hide the base of a graph. true|t|1 or false|f|0", true);
        public static CommandLineParameter<RenderQuality> RenderQualityParameter { get; } = new CommandLineParameter<RenderQuality>("renderQuality", "Render quality: " + string.Join("|", Tools.GetRenderQualitiesList().ToArray()), RenderQuality.High);

        public new static List<CommandLineParameterBase> Parameters { get; } = new List<CommandLineParameterBase>()
        {
            TypeParameter,
            FilePathParameter,
            ContainsHeaderParameter,
            ScaleFactorParameter,
            AxisScaleFactorParameter,
            PointRadiusParameter,
            MaxPointsDisplayedParameter,
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
        public int MaxPointsDisplayed { get; }
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
                ContainsHeader = ContainsHeaderParameter.DefaultValue;
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
                ScaleFactor = ScaleFactorParameter.DefaultValue;
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
                AxisScaleFactor = AxisScaleFactorParameter.DefaultValue;
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
                PointRadius = PointRadiusParameter.DefaultValue;
            }

            if (args.Count >= 7)
            {
                if (int.TryParse(args[6], out int tmp))
                {
                    MaxPointsDisplayed = tmp < -1 ? -1 : tmp;
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[6] + " into an integer", MaxPointsDisplayedParameter.Name);
                }
            }
            else
            {
                MaxPointsDisplayed = MaxPointsDisplayedParameter.DefaultValue;
            }

            if (args.Count >= 8)
            {
                if (Tools.TryParse(args[7], out bool tmp))
                {
                    DisplayBarycenter = tmp;
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[7] + " into a boolean", DisplayBarycenterParameter.Name);
                }
            }
            else
            {
                DisplayBarycenter = DisplayBarycenterParameter.DefaultValue;
            }

            if (args.Count >= 9)
            {
                if (Tools.TryParse(args[8], out bool tmp))
                {
                    DisplayAveragePlan = tmp;
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[8] + " into a boolean", DisplayAveragePlanParameter.Name);
                }
            }
            else
            {
                DisplayAveragePlan = DisplayAveragePlanParameter.DefaultValue;
            }

            if (args.Count >= 10)
            {
                if (Tools.TryParse(args[9], out bool tmp))
                {
                    DisplayBase = tmp;
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[9] + " into a boolean", DisplayBaseParameter.Name);
                }
            }
            else
            {
                DisplayBase = DisplayBaseParameter.DefaultValue;
            }

            if (args.Count >= 11)
            {
                if (Tools.TryParse(args[10], out RenderQuality tmp))
                {
                    RenderQuality = tmp;
                }
                else
                {
                    throw new ArgumentException(GetType().Name + ": Cannot parse " + args[10] + " into a RenderQuality", RenderQualityParameter.Name);
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

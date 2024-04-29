﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ReScanVisualizer.Models;
using MathEvaluatorNetFramework;

#nullable enable

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders
{
    internal class ScatterGraphPopulateFunctionXYBuilder : ScatterGraphPopulateBuilderBase
    {
        private uint _numPoints;
        public uint NumPoints
        {
            get => _numPoints;
        }

        public ExpressionVariableRangeViewModel XVariableRange { get; }
        public ExpressionVariableRangeViewModel YVariableRange { get; }

        private readonly MathEvaluatorNetFramework.Expression _expression;
        public MathEvaluatorNetFramework.Expression Expression
        {
            get => _expression;
        }

        private string _expressionString;
        public string ExpressionString
        {
            get => _expressionString;
            set
            {
                if (SetValue(ref _expressionString, value))
                {
                    _modelHasToUpdate = true;
                    OnPropertyChanged(nameof(Details));
                    SetExpression();
                }
            }
        }

        private string _expressionErrorMessage;
        public string ExpressionErrorMessage
        {
            get => _expressionErrorMessage;
        }

        public override string Name => "Function f(x, y) = z builder";

        public override string Details =>
            $"{_expression.GetNameWithVariables()} = {_expression}\n" +
            $"{XVariableRange.Min} <= {XVariableRange.VariableName} <= {XVariableRange.Max} | step: {XVariableRange.Step}\n" +
            $"{YVariableRange.Min} <= {YVariableRange.VariableName} <= {YVariableRange.Max} | step: {YVariableRange.Step}\n" +
            $"Num points: {_numPoints}";

        private bool _modelHasToUpdate;

        private bool _autoUpdateBuilderModel;
        public bool AutoUpdateBuilderModel
        {
            get => _autoUpdateBuilderModel;
            set
            {
                if (SetValue(ref _autoUpdateBuilderModel, value))
                {
                    if (_autoUpdateBuilderModel && _modelHasToUpdate)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private readonly ScatterGraphBuilderVisualizerViewModel _scatterGraphBuilderVisualizerViewModel;
        public ScatterGraphBuilderVisualizerViewModel ScatterGraphBuilderVisualizerViewModel
        {
            get => _scatterGraphBuilderVisualizerViewModel;
        }

        public ScatterGraphPopulateFunctionXYBuilder() : base(Colors.White)
        {
            _expressionString = string.Empty;
            _expressionErrorMessage = string.Empty;
            _modelHasToUpdate = false;
            _autoUpdateBuilderModel = true;
            XVariableRange = new ExpressionVariableRangeViewModel("x");
            YVariableRange = new ExpressionVariableRangeViewModel("y");
            _expression = new MathEvaluatorNetFramework.Expression("f");
            _scatterGraphBuilderVisualizerViewModel = new ScatterGraphBuilderVisualizerViewModel();

            XVariableRange.PropertyChanged += VariableRange_PropertyChanged;
            YVariableRange.PropertyChanged += VariableRange_PropertyChanged;

            ComputeNumPoints();
            SetExpression();
        }

        ~ScatterGraphPopulateFunctionXYBuilder()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                XVariableRange.PropertyChanged -= VariableRange_PropertyChanged;
                YVariableRange.PropertyChanged -= VariableRange_PropertyChanged;
                XVariableRange.Dispose();
                YVariableRange.Dispose();
                _scatterGraphBuilderVisualizerViewModel.Dispose();
                base.Dispose();
            }
        }

        private void VariableRange_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _modelHasToUpdate = true;
            ComputeNumPoints();
            OnPropertyChanged(nameof(Details));

            if (_autoUpdateBuilderModel)
            {
                UpdateBuilderModel();
            }
        }

        private void ComputeNumPoints()
        {
            _numPoints = (uint)((((XVariableRange.Max - XVariableRange.Min) / XVariableRange.Step) + 1) * (((YVariableRange.Max - YVariableRange.Min) / YVariableRange.Step) + 1));
            OnPropertyChanged(nameof(NumPoints));
        }

        private void SetExpression()
        {
            _expressionErrorMessage = string.Empty;
            State = ScatterGraphBuilderState.Ready;
            try
            {
                _expression.Set(_expressionString);

                if (_expression.DependsOnVariables(out List<string> variables))
                {
                    variables.Sort();
                    if (variables.Count == 1)
                    {
                        if (variables[0] != "x" && variables[0] != "y")
                        {
                            throw new ArgumentException("Expression must depend on x and/or y. Expression depends on: " + variables[0]);
                        }
                    }
                    else if (variables.Count == 2)
                    {
                        if (variables[0] != "x" || variables[1] != "y")
                        {
                            throw new ArgumentException("Expression must depend on x and/or y. Expression depends on: " + string.Join(", ", variables.ToArray()));
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Expression must depend on x and/or y. Expression depends on: " + string.Join(", ", variables.ToArray()));
                    }
                }

                if (_autoUpdateBuilderModel)
                {
                    UpdateBuilderModel();
                }
            }
            catch (Exception ex)
            {
                _expressionErrorMessage = ex.GetType().Name + ": " + ex.Message;
                State = ScatterGraphBuilderState.Error;
            }
            OnPropertyChanged(nameof(ExpressionErrorMessage));
        }

        private void UpdateBuilderModel()
        {
            ScatterGraph? scatterGraph = null;
            try
            {
                if (_expression.IsSet)
                {
                    scatterGraph = BuildScatterGraph();
                }
            }
            finally
            {
                if (scatterGraph == null)
                {
                    _scatterGraphBuilderVisualizerViewModel.BuildBuilderModel(new ScatterGraph(), 1.0);
                }
                else
                {
                    double radius = Math.Max(0.25, Math.Min(0.05, 10 * (XVariableRange.Step + YVariableRange.Step) / 2.0));

                    if (scatterGraph.Count > 5000)
                    {
                        if (MessageBox.Show($"Warning: Are you sure to display {scatterGraph.Count} points? It will take some time to display.", "Huge points to display", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            _modelHasToUpdate = false;
                            _scatterGraphBuilderVisualizerViewModel.BuildBuilderModel(scatterGraph, radius);
                        }
                    }
                    else
                    {
                        _modelHasToUpdate = false;
                        _scatterGraphBuilderVisualizerViewModel.BuildBuilderModel(scatterGraph, radius);
                    }
                }
            }
        }

        private ScatterGraph BuildScatterGraph()
        {
            ScatterGraph scatterGraph = new ScatterGraph();
            Variable x = new Variable("x", XVariableRange.Min);
            Variable y = new Variable("y", YVariableRange.Min);
            Variable[] vars = new Variable[2] { x, y };
            double z;

            while (y.Value <= YVariableRange.Max)
            {
                x.Value = XVariableRange.Min;
                while (x.Value <= XVariableRange.Max)
                {
                    z = _expression.Evaluate(vars);
                    scatterGraph.AddPoint((double)x.Value, (double)y.Value, z);
                    x.Value = Math.Round((double)x.Value + XVariableRange.Step, 4);
                }
                y.Value = Math.Round((double)y.Value + YVariableRange.Step, 4);
            }

            return scatterGraph;
        }

        public override ScatterGraphBuildResult Build()
        {
            Application.Current.Dispatcher.Invoke(() => State = ScatterGraphBuilderState.Working);
            ScatterGraphBuildResult result;
            try
            {
                result = new ScatterGraphBuildResult(BuildScatterGraph());
                State = ScatterGraphBuilderState.Success;
            }
            catch (Exception e)
            {
                result = new ScatterGraphBuildResult(e);
                State = ScatterGraphBuilderState.Error;
            }

            return result;
        }
    }
}

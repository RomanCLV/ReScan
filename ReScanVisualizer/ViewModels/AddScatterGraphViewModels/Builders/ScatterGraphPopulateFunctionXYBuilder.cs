using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using MathEvaluatorNetFramework;
using ReScanVisualizer.Models;

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

        private readonly Expression _expression;
        public Expression Expression
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

        public ScatterGraphPopulateFunctionXYBuilder() : base(Colors.White)
        {
            _expressionString = string.Empty;
            _expressionErrorMessage = string.Empty;
            XVariableRange = new ExpressionVariableRangeViewModel("x");
            YVariableRange = new ExpressionVariableRangeViewModel("y");
            _expression = new Expression("f");

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
                base.Dispose();
            }
        }

        private void VariableRange_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ComputeNumPoints();
            OnPropertyChanged(nameof(Details));
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
            }
            catch (Exception ex)
            {
                _expressionErrorMessage = ex.GetType().Name + ": " + ex.Message;
                State = ScatterGraphBuilderState.Error;
            }
            OnPropertyChanged(nameof(ExpressionErrorMessage));
        }

        public override ScatterGraphBuildResult Build()
        {
            State = ScatterGraphBuilderState.Working;
            ScatterGraphBuildResult result = null;
            ScatterGraph scatterGraph = new ScatterGraph();
            Variable x = new Variable("x", XVariableRange.Min);
            Variable y = new Variable("y", YVariableRange.Min);
            Variable[] vars = new Variable[2] { x, y };
            double z;

            while (State != ScatterGraphBuilderState.Error && y.Value <= YVariableRange.Max)
            {
                x.Value = XVariableRange.Min;
                while (State != ScatterGraphBuilderState.Error && x.Value <= XVariableRange.Max)
                {
                    try
                    {
                        z = _expression.Evaluate(vars);
                        scatterGraph.AddPoint((double)x.Value, (double)y.Value, z);
                    }
                    catch (Exception e)
                    {
                        State = ScatterGraphBuilderState.Error;
                        result = new ScatterGraphBuildResult(e);
                    }
                    x.Value = Math.Round((double)x.Value + XVariableRange.Step, 4);
                }
                y.Value = Math.Round((double)y.Value + YVariableRange.Step, 4);
            }

            if (result == null)
            {
                result = new ScatterGraphBuildResult(scatterGraph);
                State = ScatterGraphBuilderState.Success;
            }
            return result;
        }
    }
}

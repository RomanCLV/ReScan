using System;
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
    internal class ScatterGraphPopulateParametrics2FunctionsBuilder : ScatterGraphPopulateBuilderBase
    {
        private uint _numPoints;
        public uint NumPoints
        {
            get => _numPoints;
        }

        public ExpressionVariableRangeViewModel T1VariableRange { get; }
        public ExpressionVariableRangeViewModel T2VariableRange { get; }

        private readonly MathEvaluatorNetFramework.Expression _expressionX;
        private readonly MathEvaluatorNetFramework.Expression _expressionY;
        private readonly MathEvaluatorNetFramework.Expression _expressionZ;

        public MathEvaluatorNetFramework.Expression ExpressionX
        {
            get => _expressionX;
        }
        public MathEvaluatorNetFramework.Expression ExpressionY
        {
            get => _expressionX;
        }
        public MathEvaluatorNetFramework.Expression ExpressionZ
        {
            get => _expressionX;
        }

        private string _expressionStringX;
        public string ExpressionStringX
        {
            get => _expressionStringX;
            set
            {
                if (SetValue(ref _expressionStringX, value))
                {
                    _modelHasToUpdate = true;
                    OnPropertyChanged(nameof(Details));
                    SetExpression(_expressionX, _expressionStringX);
                }
            }
        }

        private string _expressionStringY;
        public string ExpressionStringY
        {
            get => _expressionStringY;
            set
            {
                if (SetValue(ref _expressionStringY, value))
                {
                    _modelHasToUpdate = true;
                    OnPropertyChanged(nameof(Details));
                    SetExpression(_expressionY, _expressionStringY);
                }
            }
        }

        private string _expressionStringZ;
        public string ExpressionStringZ
        {
            get => _expressionStringZ;
            set
            {
                if (SetValue(ref _expressionStringZ, value))
                {
                    _modelHasToUpdate = true;
                    OnPropertyChanged(nameof(Details));
                    SetExpression(_expressionZ, _expressionStringZ);
                }
            }
        }

        private string _expressionXErrorMessage;
        private string _expressionYErrorMessage;
        private string _expressionZErrorMessage;
        private string _expressionErrorMessage;
        public string ExpressionErrorMessage
        {
            get => _expressionErrorMessage;
        }

        public override string Name => "Functions x(t1, t2), y(t1, t2), z(t1, t2) builder";

        public override string Details =>
            $"{_expressionX.GetNameWithVariables()} = {_expressionX}\n" +
            $"{_expressionY.GetNameWithVariables()} = {_expressionY}\n" +
            $"{_expressionZ.GetNameWithVariables()} = {_expressionZ}\n" +
            $"{T1VariableRange.Min} <= {T1VariableRange.VariableName} <= {T1VariableRange.Max} | step: {T1VariableRange.Step}\n" +
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

        private bool _expressionXIsOnError;
        private bool _expressionYIsOnError;
        private bool _expressionZIsOnError;

        public ScatterGraphPopulateParametrics2FunctionsBuilder() : base(Colors.White)
        {
            _expressionXIsOnError = false;
            _expressionYIsOnError = false;
            _expressionZIsOnError = false;
            _expressionStringX = string.Empty;
            _expressionStringY = string.Empty;
            _expressionStringZ = string.Empty;
            _expressionXErrorMessage = string.Empty;
            _expressionYErrorMessage = string.Empty;
            _expressionZErrorMessage = string.Empty;
            _expressionErrorMessage = string.Empty;
            _modelHasToUpdate = false;
            _autoUpdateBuilderModel = true;
            T1VariableRange = new ExpressionVariableRangeViewModel("t1");
            T2VariableRange = new ExpressionVariableRangeViewModel("t2");
            _expressionX = new MathEvaluatorNetFramework.Expression("x");
            _expressionY = new MathEvaluatorNetFramework.Expression("y");
            _expressionZ = new MathEvaluatorNetFramework.Expression("z");
            _scatterGraphBuilderVisualizerViewModel = new ScatterGraphBuilderVisualizerViewModel();

            T1VariableRange.PropertyChanged += VariableRange_PropertyChanged;
            T2VariableRange.PropertyChanged += VariableRange_PropertyChanged;

            ComputeNumPoints();
            SetExpression(_expressionZ, _expressionStringZ);
            SetExpression(_expressionY, _expressionStringY);
            SetExpression(_expressionX, _expressionStringX);
        }

        ~ScatterGraphPopulateParametrics2FunctionsBuilder()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                T1VariableRange.PropertyChanged -= VariableRange_PropertyChanged;
                T2VariableRange.PropertyChanged -= VariableRange_PropertyChanged;
                T1VariableRange.Dispose();
                T2VariableRange.Dispose();
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
            _numPoints = (uint)(((T1VariableRange.Max - T1VariableRange.Min) / T1VariableRange.Step) + 1);
            OnPropertyChanged(nameof(NumPoints));
        }

        private void SetExpression(MathEvaluatorNetFramework.Expression expression, string expressionString)
        {
            _expressionErrorMessage = string.Empty;
            if (expression.Equals(_expressionX))
            {
                _expressionXErrorMessage = string.Empty;
                _expressionXIsOnError = false;
            }
            else if (expression.Equals(_expressionY))
            {
                _expressionYErrorMessage = string.Empty;
                _expressionYIsOnError = false;
            }
            else if (expression.Equals(_expressionZ))
            {
                _expressionZErrorMessage = string.Empty;
                _expressionZIsOnError = false;
            }

            try
            {
                expression.Set(expressionString);

                if (expression.DependsOnVariables(out List<string> variables))
                {
                    variables.Sort();
                    if (variables.Count == 1)
                    {
                        if (variables[0] != "t1" && variables[0] != "t2")
                        {
                            throw new ArgumentException("Expression " + expression.Name + "() must depend on t1 and/or t2. Expression depends on: " + variables[0]);
                        }
                    }
                    else if (variables.Count == 2)
                    {
                        if (variables[0] != "t1" || variables[1] != "t2")
                        {
                            throw new ArgumentException("Expression must depend on t1 and/or t2. Expression depends on: " + string.Join(", ", variables.ToArray()));
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Expression " + expression.Name + "() must depend on t1 and/or t2. Expression depends on: " + string.Join(", ", variables.ToArray()));
                    }
                }

                if (_autoUpdateBuilderModel)
                {
                    UpdateBuilderModel();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = expression.Name + " expression - " + ex.GetType().Name + ": " + ex.Message;
                if (expression.Equals(_expressionX))
                {
                    _expressionXErrorMessage = errorMessage;
                    _expressionErrorMessage = _expressionXErrorMessage;
                    _expressionXIsOnError = true;
                }
                else if (expression.Equals(_expressionY))
                {
                    _expressionYErrorMessage = errorMessage;
                    _expressionErrorMessage = _expressionYErrorMessage;
                    _expressionYIsOnError = true;
                }
                else if (expression.Equals(_expressionZ))
                {
                    _expressionZErrorMessage = errorMessage;
                    _expressionErrorMessage = _expressionZErrorMessage;
                    _expressionZIsOnError = true;
                }
            }
            if (string.IsNullOrEmpty(_expressionErrorMessage))
            {
                if (!string.IsNullOrEmpty(_expressionXErrorMessage))
                {
                    _expressionErrorMessage = _expressionXErrorMessage;
                }
                else if (!string.IsNullOrEmpty(_expressionYErrorMessage))
                {
                    _expressionErrorMessage = _expressionYErrorMessage;
                }
                else if (!string.IsNullOrEmpty(_expressionZErrorMessage))
                {
                    _expressionErrorMessage = _expressionZErrorMessage;
                }
            }
            State = _expressionXIsOnError || _expressionYIsOnError || _expressionZIsOnError ? ScatterGraphBuilderState.Error : ScatterGraphBuilderState.Ready;
            OnPropertyChanged(nameof(ExpressionErrorMessage));
        }

        private void UpdateBuilderModel()
        {
            ScatterGraph? scatterGraph = null;
            try
            {
                if (_expressionX.IsSet && _expressionY.IsSet && _expressionZ.IsSet)
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
                    double radius = (T1VariableRange.Step + T2VariableRange.Step) / 10.0;
                    radius = Math.Max(0.25, Math.Min(0.05, radius));

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
            Variable t1 = new Variable("t1", T1VariableRange.Min);
            Variable t2 = new Variable("t2", T2VariableRange.Min);
            Variable[] vars = new Variable[2] { t1, t2 };
            double x;
            double y;
            double z;

            while (t1.Value < T1VariableRange.Max)
            {
                t2.Value = T2VariableRange.Min;
                while (t2.Value < T2VariableRange.Max)
                {
                    x = _expressionX.Evaluate(vars);
                    y = _expressionY.Evaluate(vars);
                    z = _expressionZ.Evaluate(vars);
                    scatterGraph.AddPoint(x, y, z);
                    t2.Value = Math.Round((double)t2.Value + T2VariableRange.Step, 4);
                }
                t1.Value = Math.Round((double)t1.Value + T1VariableRange.Step, 4);
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

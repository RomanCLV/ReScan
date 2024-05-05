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
    internal class ScatterGraphPopulateParametricsFunctionsUVBuilder : ScatterGraphPopulateBuilderBase, IModelisableBuilder
    {
        private uint _numPoints;
        public uint NumPoints
        {
            get => _numPoints;
        }

        public bool AngleAreInDegrees
        {
            get => MathEvaluator.Parameters.AngleAreInDegrees;
            set
            {
                if (MathEvaluator.Parameters.AngleAreInDegrees != value)
                {
                    MathEvaluator.Parameters.AngleAreInDegrees = value;
                    OnPropertyChanged(nameof(AngleAreInDegrees));
                    ModelHasToUpdate = true;
                }
            }
        }
        public ExpressionVariableRangeViewModel UVariableRange { get; }
        public ExpressionVariableRangeViewModel VVariableRange { get; }

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

        public override string Name => "Functions x(u, v), y(u, v), z(u, v) builder";

        public override string Details =>
            $"{_expressionX.GetNameWithVariables()} = {_expressionX}\n" +
            $"{_expressionY.GetNameWithVariables()} = {_expressionY}\n" +
            $"{_expressionZ.GetNameWithVariables()} = {_expressionZ}\n" +
            $"{Math.Round(UVariableRange.Min, 4)} <= {UVariableRange.VariableName} <= {Math.Round(UVariableRange.Max, 4)} | step: {Math.Round(UVariableRange.Step, 4)}\n" +
            $"{Math.Round(VVariableRange.Min, 4)} <= {VVariableRange.VariableName} <= {Math.Round(VVariableRange.Max, 4)} | step: {Math.Round(VVariableRange.Step, 4)}\n" +
            $"Num points: {_numPoints}";

        private bool _modelHasToUpdate;
        public bool ModelHasToUpdate
        {
            set
            {
                _modelHasToUpdate = value;
                if (_modelHasToUpdate && _autoUpdateBuilderModel)
                {
                    UpdateBuilderModel();
                }
            }
        }

        private bool _autoUpdateBuilderModel;
        public bool AutoUpdateBuilderModel
        {
            get => _autoUpdateBuilderModel;
            set
            {
                if (SetValue(ref _autoUpdateBuilderModel, value) && _autoUpdateBuilderModel && _modelHasToUpdate)
                {
                    UpdateBuilderModel();
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

        public ScatterGraphPopulateParametricsFunctionsUVBuilder() : base(Colors.White)
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
            UVariableRange = new ExpressionVariableRangeViewModel("u");
            VVariableRange = new ExpressionVariableRangeViewModel("v");
            _expressionX = new MathEvaluatorNetFramework.Expression("x");
            _expressionY = new MathEvaluatorNetFramework.Expression("y");
            _expressionZ = new MathEvaluatorNetFramework.Expression("z");
            _scatterGraphBuilderVisualizerViewModel = new ScatterGraphBuilderVisualizerViewModel();

            UVariableRange.PropertyChanged += VariableRange_PropertyChanged;
            VVariableRange.PropertyChanged += VariableRange_PropertyChanged;

            ComputeNumPoints();
            SetExpression(_expressionZ, _expressionStringZ);
            SetExpression(_expressionY, _expressionStringY);
            SetExpression(_expressionX, _expressionStringX);
        }

        ~ScatterGraphPopulateParametricsFunctionsUVBuilder()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                UVariableRange.PropertyChanged -= VariableRange_PropertyChanged;
                VVariableRange.PropertyChanged -= VariableRange_PropertyChanged;
                UVariableRange.Dispose();
                VVariableRange.Dispose();
                _scatterGraphBuilderVisualizerViewModel.Dispose();
                base.Dispose();
            }
        }

        private void VariableRange_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ComputeNumPoints();
            OnPropertyChanged(nameof(Details));
            ModelHasToUpdate = true;
        }

        private void ComputeNumPoints()
        {
            _numPoints = (uint)(
                (((UVariableRange.Max - UVariableRange.Min) / UVariableRange.Step) + 1) *
                (((VVariableRange.Max - VVariableRange.Min) / VVariableRange.Step) + 1)
                );
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
                        if (variables[0] != "u" && variables[0] != "v")
                        {
                            throw new ArgumentException("Expression " + expression.Name + "() must depend on u and/or v. Expression depends on: " + variables[0]);
                        }
                    }
                    else if (variables.Count == 2)
                    {
                        if (variables[0] != "u" || variables[1] != "v")
                        {
                            throw new ArgumentException("Expression must depend on u and/or v. Expression depends on: " + string.Join(", ", variables.ToArray()));
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Expression " + expression.Name + "() must depend on u and/or v. Expression depends on: " + string.Join(", ", variables.ToArray()));
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
            if (_expressionX.IsSet && _expressionY.IsSet && _expressionZ.IsSet)
            {
                try
                {
                    ScatterGraph scatterGraph = BuildScatterGraph();
                    if (scatterGraph.Count <= 5000 || MessageBox.Show($"Warning: Are you sure to display {scatterGraph.Count} points? It will take some time to display.", "Huge points to display", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        _modelHasToUpdate = false;
                        _scatterGraphBuilderVisualizerViewModel.BuildBuilderModel(scatterGraph, Color, BuildRadius());
                    }
                }
                catch
                {
                    _scatterGraphBuilderVisualizerViewModel.ClearBuilderModel();
                }
            }
            else
            {
                _scatterGraphBuilderVisualizerViewModel.ClearBuilderModel();
            }
        }

        private double BuildRadius()
        {
            return Math.Min(0.25, Math.Max(0.01, Math.Min(UVariableRange.Step, VVariableRange.Step) / 3.0));
        }

        private ScatterGraph BuildScatterGraph()
        {
            ScatterGraph scatterGraph = new ScatterGraph();
            Variable u = new Variable("u", UVariableRange.Min);
            Variable v = new Variable("v", VVariableRange.Min);
            double x;
            double y;
            double z;

            while (u.Value < UVariableRange.Max)
            {
                v.Value = VVariableRange.Min;
                while (v.Value < VVariableRange.Max)
                {
                    x = _expressionX.Evaluate(u, v);
                    y = _expressionY.Evaluate(u, v);
                    z = _expressionZ.Evaluate(u, v);
                    scatterGraph.AddPoint(x, y, z);
                    v.Value = Math.Round((double)v.Value + VVariableRange.Step, 9);
                }
                u.Value = Math.Round((double)u.Value + UVariableRange.Step, 9);
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
                PointRadius = BuildRadius();
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

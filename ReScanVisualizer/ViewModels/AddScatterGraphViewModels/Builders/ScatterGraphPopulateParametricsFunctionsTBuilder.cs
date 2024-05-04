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
using System.Diagnostics;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;

#nullable enable

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders
{
    internal class ScatterGraphPopulateParametricsFunctionsTBuilder : ScatterGraphPopulateBuilderBase
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
                    UpdateBuilderModel();
                }
            }
        }

        public ExpressionVariableRangeViewModel TVariableRange { get; }

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

        private bool _expressionXIsOnError;
        private bool _expressionYIsOnError;
        private bool _expressionZIsOnError;

        private string _expressionXErrorMessage;
        private string _expressionYErrorMessage;
        private string _expressionZErrorMessage;

        private string _expressionErrorMessage;
        public string ExpressionErrorMessage
        {
            get => _expressionErrorMessage;
        }

        public override string Name => "Functions x(t), y(t), z(t) builder";

        public override string Details =>
            $"{_expressionX.GetNameWithVariables()} = {_expressionX}\n" +
            $"{_expressionY.GetNameWithVariables()} = {_expressionY}\n" +
            $"{_expressionZ.GetNameWithVariables()} = {_expressionZ}\n" +
            $"{Math.Round(TVariableRange.Min, 4)} <= {TVariableRange.VariableName} <= {Math.Round(TVariableRange.Max, 4)} | step: {Math.Round(TVariableRange.Step, 4)}\n" +
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

        private bool _isRepeated;
        public bool IsRepeated
        {
            get => _isRepeated;
            set
            {
                if (SetValue(ref _isRepeated, value))
                {
                    if (!_isRepeated && _repetitionMode != RepetitionMode.None)
                    {
                        _repetitionMode = RepetitionMode.None;
                        OnPropertyChanged(nameof(RepetitionMode));
                    }
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private RepetitionMode _repetitionMode;
        public RepetitionMode RepetitionMode
        {
            get => _repetitionMode;
            set
            {
                if (SetValue(ref _repetitionMode, value))
                {
                    //if (_repetitionMode == RepetitionMode.Rotation)
                    //{
                    //    if (_negativeRepetitionLength < -180.0)
                    //    {
                    //        _negativeRepetitionLength = -180.0;
                    //        OnPropertyChanged(nameof(NegativeRepetitionLength));
                    //    }
                    //    if (_positiveTranslationLength > 180.0)
                    //    {
                    //        _positiveTranslationLength = 180.0;
                    //        OnPropertyChanged(nameof(PositiveRepetitionLength));
                    //    }
                    //    if (_repetitionStep > _positiveTranslationLength - _negativeRepetitionLength)
                    //    {
                    //        _repetitionStep = _positiveTranslationLength - _negativeRepetitionLength;
                    //        OnPropertyChanged(nameof(RepetitionStep));
                    //    }
                    //}
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        #region Translation repetition

        private double _translationX;
        public double TranslationX
        {
            get => _translationX;
            set
            {
                if (SetValue(ref _translationX, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _translationY;
        public double TranslationY
        {
            get => _translationY;
            set
            {
                if (SetValue(ref _translationY, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _translationZ;
        public double TranslationZ
        {
            get => _translationZ;
            set
            {
                if (SetValue(ref _translationZ, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _positiveTranslationLength;
        public double PositiveTranslationLength
        {
            get => _positiveTranslationLength;
            set
            {
                if (value < 0.0)
                {
                    value = 0.0;
                }
                if (SetValue(ref _positiveTranslationLength, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _negativeTranslationLength;
        public double NegativeTranslationLength
        {
            get => _negativeTranslationLength;
            set
            {
                if (value > 0.0)
                {
                    value = 0.0;
                }
                if (SetValue(ref _negativeTranslationLength, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _translationStep;
        public double TranslationStep
        {
            get => _translationStep;
            set
            {
                if (value <= 0.0)
                {
                    value = 1.0;
                }
                else if (value < 0.001)
                {
                    value = 0.001;
                }
                else if (value > _positiveTranslationLength - _negativeTranslationLength)
                {
                    value = _positiveTranslationLength - _negativeTranslationLength;
                }
                if (SetValue(ref _translationStep, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        #endregion

        #region Rotation repetition

        private double _rotationX;
        public double RotationX
        {
            get => _rotationX;
            set
            {
                if (SetValue(ref _rotationX, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _rotationY;
        public double RotationY
        {
            get => _rotationY;
            set
            {
                if (SetValue(ref _rotationY, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _rotationZ;
        public double RotationZ
        {
            get => _rotationZ;
            set
            {
                if (SetValue(ref _rotationZ, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _rotationPointX;
        public double RotationPointX
        {
            get => _rotationPointX;
            set
            {
                if (SetValue(ref _rotationPointX, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _rotationPointY;
        public double RotationPointY
        {
            get => _rotationPointY;
            set
            {
                if (SetValue(ref _rotationPointY, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _rotationPointZ;
        public double RotationPointZ
        {
            get => _rotationPointZ;
            set
            {
                if (SetValue(ref _rotationPointZ, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _positiveRotationLength;
        public double PositiveRotationLength
        {
            get => _positiveRotationLength;
            set
            {
                if (value < 0.0)
                {
                    value = 0.0;
                }
                else if (value > 180.0)
                {
                    value = 180.0;
                }
                if (SetValue(ref _positiveRotationLength, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _negativeRotationLength;
        public double NegativeRotationLength
        {
            get => _negativeRotationLength;
            set
            {
                if (value > 0.0)
                {
                    value = 0.0;
                }
                else if (value < -180.0)
                {
                    value = -180.0;
                }
                if (SetValue(ref _negativeRotationLength, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        private double _rotationStep;
        public double RotationStep
        {
            get => _rotationStep;
            set
            {
                if (value <= 0.0)
                {
                    value = 1.0;
                }
                else if (value < 0.001)
                {
                    value = 0.001;
                }
                else if (value > _positiveRotationLength - _negativeRotationLength)
                {
                    value = _positiveRotationLength - _negativeRotationLength;
                }
                if (SetValue(ref _rotationStep, value))
                {
                    if (_autoUpdateBuilderModel)
                    {
                        UpdateBuilderModel();
                    }
                }
            }
        }

        #endregion

        public ScatterGraphPopulateParametricsFunctionsTBuilder() : base(Colors.White)
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
            TVariableRange = new ExpressionVariableRangeViewModel("t");
            _expressionX = new MathEvaluatorNetFramework.Expression("x");
            _expressionY = new MathEvaluatorNetFramework.Expression("y");
            _expressionZ = new MathEvaluatorNetFramework.Expression("z");
            _scatterGraphBuilderVisualizerViewModel = new ScatterGraphBuilderVisualizerViewModel();

            // visualizer
            _modelHasToUpdate = false;
            _autoUpdateBuilderModel = true;

            // repetition
            _isRepeated = false;
            _repetitionMode = RepetitionMode.None;

            // translation repetition
            _translationX = 0.0;
            _translationY = 0.0;
            _translationZ = 1.0;
            _positiveTranslationLength = 1.0;
            _negativeTranslationLength = -1.0;
            _translationStep = 1.0;

            // rotation repetition
            _rotationX = 0.0;
            _rotationY = 0.0;
            _rotationZ = 1.0;
            _rotationPointX = 0.0;
            _rotationPointY = 0.0;
            _rotationPointZ = 0.0;
            _positiveRotationLength = 90.0;
            _negativeRotationLength = -90.0;
            _rotationStep = 30.0;

            TVariableRange.PropertyChanged += VariableRange_PropertyChanged;

            ComputeNumPoints();
            SetExpression(_expressionZ, _expressionStringZ);
            SetExpression(_expressionY, _expressionStringY);
            SetExpression(_expressionX, _expressionStringX);
        }

        ~ScatterGraphPopulateParametricsFunctionsTBuilder()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                TVariableRange.PropertyChanged -= VariableRange_PropertyChanged;
                TVariableRange.Dispose();
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
            _numPoints = (uint)(((TVariableRange.Max - TVariableRange.Min) / TVariableRange.Step) + 1);
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
                    if (variables.Count == 1)
                    {
                        if (variables[0] != "t")
                        {
                            throw new ArgumentException("Expression " + expression.Name + "() must depend on t. Expression depends on: " + variables[0]);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Expression " + expression.Name + "() must depend on t. Expression depends on: " + string.Join(", ", variables.ToArray()));
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
                    if (scatterGraph.Count > 5000)
                    {
                        if (MessageBox.Show($"Warning: Are you sure to display {scatterGraph.Count} points? It will take some time to display.", "Huge points to display", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            _modelHasToUpdate = false;
                            _scatterGraphBuilderVisualizerViewModel.BuildBuilderModel(scatterGraph, BuildRadius());
                        }
                    }
                    else
                    {
                        _modelHasToUpdate = false;
                        _scatterGraphBuilderVisualizerViewModel.BuildBuilderModel(scatterGraph, BuildRadius());
                    }
                }
            }
        }

        private double BuildRadius()
        {
            double radius;
            if (_repetitionMode is RepetitionMode.None)
            {
                radius = TVariableRange.Step;
            }
            else if (_repetitionMode is RepetitionMode.Translation)
            {
                radius = Math.Min(TVariableRange.Step, _translationStep);
            }
            else // RepetitionMode.Rotation
            {
                radius = Math.Min(TVariableRange.Step, _rotationStep);
            }
            return Math.Min(0.25, Math.Max(0.01, radius / 3.0));
        }

        private ScatterGraph BuildScatterGraph()
        {
            ScatterGraph scatterGraph = new ScatterGraph();
            Variable t = new Variable("t", TVariableRange.Min);
            double x;
            double y;
            double z;

            while (t.Value < TVariableRange.Max)
            {
                x = _expressionX.Evaluate(t);
                y = _expressionY.Evaluate(t);
                z = _expressionZ.Evaluate(t);
                scatterGraph.AddPoint(x, y, z);
                t.Value = Math.Round((double)t.Value + TVariableRange.Step, 9);
            }

            if (_repetitionMode == RepetitionMode.Translation)
            {
                scatterGraph = ApplyTranslationRepetition(scatterGraph);
            }
            else if (_repetitionMode == RepetitionMode.Rotation)
            {
                scatterGraph = ApplyRotationRepetition(scatterGraph);
            }

            return scatterGraph;
        }

        public ScatterGraph ApplyTranslationRepetition(ScatterGraph scatterGraph)
        {
            ScatterGraph translationGraph = new ScatterGraph();
            int count = scatterGraph.Count;
            Vector3D direction = new Vector3D(_translationX, _translationY, _translationZ);
            direction.Normalize();

            for (int i = 0; i < count; i++)
            {
                Point3D currentPoint = scatterGraph[i];
                double t = _negativeTranslationLength;
                while (t <= _positiveTranslationLength)
                {
                    translationGraph.AddPoint(
                        direction.X * t + currentPoint.X,
                        direction.Y * t + currentPoint.Y,
                        direction.Z * t + currentPoint.Z
                        );
                    t = Math.Round(t + _translationStep, 9);
                }
            }
            return translationGraph;
        }

        public ScatterGraph ApplyRotationRepetition(ScatterGraph scatterGraph)
        {
            ScatterGraph rotationGraph = new ScatterGraph();
            int count = scatterGraph.Count;
            Vector3D axisRotation = new Vector3D(_rotationX, _rotationY, _rotationZ);
            axisRotation.Normalize();

            for (int i = 0; i < count; i++)
            {
                Point3D currentPoint = scatterGraph[i];
                double t = _negativeRotationLength;
                while (t <= _positiveRotationLength)
                {
                    Quaternion quaternion = new Quaternion(axisRotation, t);
                    RotateTransform3D rotateTransform = new RotateTransform3D(new QuaternionRotation3D(quaternion));
                    rotationGraph.AddPoint(rotateTransform.Transform(currentPoint), true);
                    t = Math.Round(t + _rotationStep, 9);
                }
            }
            return rotationGraph;
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

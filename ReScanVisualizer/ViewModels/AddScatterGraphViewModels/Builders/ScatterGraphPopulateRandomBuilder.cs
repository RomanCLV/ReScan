﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ReScanVisualizer.Models;

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders
{
    public class ScatterGraphPopulateRandomBuilder : ScatterGraphPopulateBuilderBase
    {
        private uint _numPoints;
        public uint NumPoints
        {
            get => _numPoints;
            set
            {
                if (value > MAX_COUNT)
                {
                    value = MAX_COUNT;
                }
                else if (value < 1)
                {
                    value = 1;
                }
                if (SetValue(ref _numPoints, value))
                {
                    OnPropertyChanged(nameof(Details));
                    State = ScatterGraphBuilderState.Ready;
                }
            }
        }

        private double _minX;
        public double MinX
        {
            get => _minX;
            set
            {
                if (value < MIN_X)
                {
                    value = MIN_X;
                }
                else if (value >= _maxX)
                {
                    value = _maxX - 1;
                }
                if (SetValue(ref _minX, value))
                {
                    OnPropertyChanged(nameof(Details));
                    State = ScatterGraphBuilderState.Ready;
                }
            }
        }

        private double _maxX;
        public double MaxX
        {
            get => _maxX;
            set
            {
                if (value > MAX_X)
                {
                    value = MAX_X;
                }
                else if (value <= _minX)
                {
                    value = _minX + 1;
                }
                if (SetValue(ref _maxX, value))
                {
                    OnPropertyChanged(nameof(Details));
                    State = ScatterGraphBuilderState.Ready;
                }
            }
        }

        private double _minY;
        public double MinY
        {
            get => _minY;
            set
            {
                if (value < MIN_Y)
                {
                    value = MIN_Y;
                }
                else if (value >= _maxY)
                {
                    value = _maxY - 1;
                }
                if (SetValue(ref _minY, value))
                {
                    OnPropertyChanged(nameof(Details));
                    State = ScatterGraphBuilderState.Ready;
                }
            }
        }

        private double _maxY;
        public double MaxY
        {
            get => _maxY;
            set
            {
                if (value > MAX_Y)
                {
                    value = MAX_Y;
                }
                else if (value <= _minY)
                {
                    value = _minY + 1;
                }
                if (SetValue(ref _maxY, value))
                {
                    OnPropertyChanged(nameof(Details));
                    State = ScatterGraphBuilderState.Ready;
                }
            }
        }

        private double _minZ;
        public double MinZ
        {
            get => _minZ;
            set
            {
                if (value < MIN_Z)
                {
                    value = MIN_Z;
                }
                else if (value >= _maxZ)
                {
                    value = _maxZ - 1;
                }
                if (SetValue(ref _minZ, value))
                {
                    OnPropertyChanged(nameof(Details));
                    State = ScatterGraphBuilderState.Ready;
                }
            }
        }

        private double _maxZ;
        public double MaxZ
        {
            get => _maxZ;
            set
            {
                if (value > MAX_Z)
                {
                    value = MAX_Z;
                }
                else if (value <= _minZ)
                {
                    value = _minZ + 1;
                }
                if (SetValue(ref _maxZ, value))
                {
                    OnPropertyChanged(nameof(Details));
                    State = ScatterGraphBuilderState.Ready;
                }
            }
        }

        public override string Name => "Random builder";

        public override string Details => $"Limits: " +
            $"{Math.Round(_minX, 2)}, {Math.Round(_maxX, 2)}\n" +
            $"{Math.Round(_minY, 2)}, {Math.Round(_maxY, 2)}\n" +
            $"{Math.Round(_minZ, 2)}, {Math.Round(_maxZ, 2)}\n" +
            $"Num points: {NumPoints}";

        public ScatterGraphPopulateRandomBuilder() : base(Colors.White)
        {
            _numPoints = 5;
            _minX = -10;
            _maxX = 10;
            _minY = -10;
            _maxY = 10;
            _minZ = -10;
            _maxZ = 10;
        }

        /// <returns>Return a <see cref="ScatterGraphBuildResult"/> using the <see cref="ScatterGraph.PopulateRandom(ScatterGraph, uint, double, double, double, double, double, double)"/> method.</returns>
        public override ScatterGraphBuildResult Build()
        {
            Application.Current.Dispatcher.Invoke(() => State = ScatterGraphBuilderState.Working);
            ScatterGraphBuildResult scatterGraphBuildResult;
            try
            {
                ScatterGraph graph = new ScatterGraph();
                ScatterGraph.PopulateRandom(graph, _numPoints, _minX, _maxX, _minY, _maxY, _minZ, _maxZ);
                scatterGraphBuildResult = new ScatterGraphBuildResult(graph);
                State = ScatterGraphBuilderState.Success;
            }
            catch (Exception e)
            {
                scatterGraphBuildResult = new ScatterGraphBuildResult(e);
                State = ScatterGraphBuilderState.Error;
            }
            return scatterGraphBuildResult;
        }

        public override Task<ScatterGraphBuildResult> BuildAsync()
        {
            Task.Delay(20).Wait();
            return base.BuildAsync();
        }
    }
}

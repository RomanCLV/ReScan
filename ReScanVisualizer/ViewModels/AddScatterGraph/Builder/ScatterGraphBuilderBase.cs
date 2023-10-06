﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ReScanVisualizer.Models;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public abstract class ScatterGraphBuilderBase : ViewModelBase
    {
        public const uint   MAX_COUNT = 10000;
        public const double MIN_X = -10000.0;
        public const double MAX_X =  10000.0;
        public const double MIN_Y = -10000.0;
        public const double MAX_Y =  10000.0;
        public const double MIN_Z = -10000.0;
        public const double MAX_Z =  10000.0;
        public const double MIN_WIDTH = 1.0;
        public const double MAX_WIDTH = 10000.0;
        public const double MIN_HEIGTH = 1.0;
        public const double MAX_HEIGTH = 10000.0;

        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetValue(ref _color, value);
        }

        private bool _canBuild;
        public bool CanBuild
        {
            get => _canBuild;
            set => SetValue(ref _canBuild, value);
        }

        // TODO : implémenter tous les can buid

        public ScatterGraphBuilderBase() : this(Colors.White)
        {
        }

        public ScatterGraphBuilderBase(Color color)
        {
            _color = color;
            _canBuild = true;
        }

        /// <summary>
        /// Build an array of <see cref="ScatterGraphBuildResult"/>
        /// </summary>
        public abstract ScatterGraphBuildResult[] Build();
    }
}

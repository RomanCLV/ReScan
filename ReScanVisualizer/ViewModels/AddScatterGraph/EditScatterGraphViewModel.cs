﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReScanVisualizer.Commands;
using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
using ReScanVisualizer.Views;
using ReScanVisualizer.Views.AddScatterGraphViews;

namespace ReScanVisualizer.ViewModels.AddScatterGraph
{
    public class EditScatterGraphViewModel : ViewModelBase
    {
        private ScatterGraphBuilderBase _builder;
        public ScatterGraphBuilderBase Builder
        {
            get => _builder;
            private set => SetValue(ref _builder, value);
        }

        public CommandKey ValidateCommand { get; }

        public EditScatterGraphViewModel(EditScatterGraphBuilderView view, ScatterGraphBuilderBase builder)
        {
            _builder = builder;

            ValidateCommand = new CommandKey(new ActionCommand(view.Close), Key.Enter, ModifierKeys.None, "OK");
        }
    }
}

﻿using ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReScanVisualizer.Views.AddScatterGraphViews
{
    /// <summary>
    /// Logique d'interaction pour ScatterGraphPopulateFunctionsXYZBuilderView.xaml
    /// </summary>
    public partial class ScatterGraphPopulateFunctionsXYZBuilderView : UserControl
    {
        public ScatterGraphPopulateFunctionsXYZBuilderView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateColorSelectorColor();
        }

        private void UpdateColorSelectorColor()
        {
            if (DataContext is ScatterGraphPopulateFunctionsXYZBuilder builder)
            {
                ColorSelector.Color = builder.Color;
            }
        }

        public void ColorSelector_ColorChanged(object sender, Color c)
        {
            if (DataContext is ScatterGraphPopulateFunctionsXYZBuilder builder)
            {
                builder.Color = c;
            }
        }
    }
}
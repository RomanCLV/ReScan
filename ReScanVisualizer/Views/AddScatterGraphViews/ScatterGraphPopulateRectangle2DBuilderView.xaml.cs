﻿using System;
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
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders;

namespace ReScanVisualizer.Views.AddScatterGraphViews
{
    /// <summary>
    /// Logique d'interaction pour ScatterGraphPopulateRectangle2DBuilderView.xaml
    /// </summary>
    public partial class ScatterGraphPopulateRectangle2DBuilderView : UserControl
    {
        public ScatterGraphPopulateRectangle2DBuilderView()
        {
            InitializeComponent();
            UpdateColorSelectorColor();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateColorSelectorColor();
        }

        private void UpdateColorSelectorColor()
        {
            if (DataContext is ScatterGraphPopulateRectangle2DBuilder builder)
            {
                ColorSelector.Color = builder.Color;
            }
        }

        public void ColorSelector_ColorChanged(object sender, Color c)
        {
            if (DataContext is ScatterGraphPopulateRectangle2DBuilder builder)
            {
                builder.Color = c;
            }
        }
    }
}

﻿using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
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
    /// Logique d'interaction pour ScatterGraphPopulateLineBuilderView.xaml
    /// </summary>
    public partial class ScatterGraphPopulateLineBuilderView : UserControl
    {
        public ScatterGraphPopulateLineBuilderView()
        {
            InitializeComponent();
        }

        public void ColorSelector_ColorChanged(object sender, Color c)
        {
            if (DataContext is ScatterGraphPopulateLineBuilder builder)
            {
                builder.Color = c;
            }
        }
    }
}

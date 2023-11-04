﻿using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.Views.ItemTreeViews;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

#nullable enable

namespace ReScanVisualizer.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BaseItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is Base3DViewModel viewModel)
            {
                ((MainViewModel)DataContext).SelectedViewModel = new BaseViewModel(viewModel);
            }
        }

        private void ScatterGraphTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedViewModel = ((ScatterGraphTreeViewItemHeader)sender).DataContext as ViewModelBase;
            }
        }

        private void BarycenterTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedViewModel = ((SampleTreeViewItemHeader)sender).DataContext as ViewModelBase;
            }
        }

        private void AveragePlanTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedViewModel = ((AveragePlanTreeViewItemHeader)sender).DataContext as ViewModelBase;
            }
        }

        private void BaseTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedViewModel = ((BaseTreeViewItemHeader)sender).DataContext as ViewModelBase;
            }
        }
    }
}

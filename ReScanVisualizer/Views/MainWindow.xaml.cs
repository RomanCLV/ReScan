﻿using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.Views.ItemTreeViews;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
using static HelixToolkit.Wpf.Viewport3DHelper;

#nullable enable

namespace ReScanVisualizer.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GeometryModel3D? _geometryModel3DMouseOver;

        public MainWindow()
        {
            InitializeComponent();
            _geometryModel3DMouseOver = null;
        }

        private void BaseClearButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).ClearBases();
        }

        private void BaseTreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem tb && tb.DataContext is Base3DViewModel viewModel)
            {
                ((MainViewModel)DataContext).SelectedViewModel = new BaseViewModel(viewModel);
            }
        }

        private void BaseTreeViewItem_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is TreeViewItem tb && tb.DataContext is Base3DViewModel viewModel)
            {
                switch (e.Key)
                {
                    case Key.Delete:
                    case Key.Back:
                        ((MainViewModel)DataContext).Bases.Remove(viewModel);
                        break;
                }
            }
        }

        private void GraphClearButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).ClearScatterGraphs();
        }

        private void ScatterGraphTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedViewModel = ((ScatterGraphTreeViewItemHeader)sender).DataContext as ViewModelBase;
            }
        }

        private void ScatterGraphTreeViewItem_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is TreeViewItem tb && tb.DataContext is ScatterGraphViewModel viewModel)
            {
                switch (e.Key)
                {
                    case Key.Delete:
                    case Key.Back:
                        ((MainViewModel)DataContext).ScatterGraphs.Remove(viewModel);
                        break;
                }
            }
        }

        private void BarycenterTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedViewModel = (SampleViewModel)((BarycenterTreeViewItemHeader)sender).DataContext;

            }
        }

        private void AveragePlanTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedViewModel = (PlanViewModel)((AveragePlanTreeViewItemHeader)sender).DataContext;
            }
        }

        private void BaseTreeViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                Base3DViewModel base3DviewModel = (Base3DViewModel)((BaseTreeViewItem)sender).DataContext;
                viewModel.SelectedViewModel = new BaseViewModel(base3DviewModel);
            }
        }

        private void SampleTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedViewModel = (SampleViewModel)((SampleTreeViewItemHeader)sender).DataContext;
            }
        }

        private void VeryLowRenderQualityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SetAllRenderQuality(RenderQuality.VeryLow);
            }
        }

        private void LowRenderQualityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SetAllRenderQuality(RenderQuality.Low);
            }
        }

        private void MediumRenderQualityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SetAllRenderQuality(RenderQuality.Medium);
            }
        }

        private void HighRenderQualityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SetAllRenderQuality(RenderQuality.High);
            }
        }

        private void VeryHighRenderQualityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SetAllRenderQuality(RenderQuality.VeryHigh);
            }
        }

        private void RandomizeColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.RandomizeAllColors();
            }
        }

        private void ShowAllGraphsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ShowAllGraphs();
            }
        }

        private void HideAllGraphsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.HideAllGraphs();
            }
        }

        private void ShowAllPointsGraphsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ShowAllPointsGraphs();
            }
        }

        private void HideAllPointsGraphsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.HideAllPointsGraphs();
            }
        }

        private void ShowAllAveragePlansMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ShowAllAveragePlans();
            }
        }

        private void HideAllAveragePlansMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.HideAllAveragePlans();
            }
        }

        private void ShowAllBarycentersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ShowAllBarycenters();
            }
        }

        private void HideAllBarycentersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.HideAllBarycenters();
            }
        }

        private void ShowAllBasesGraphsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ShowAllBasesGraphs();
            }
        }

        private void HideAllBasesGraphsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.HideAllBasesGraphs();
            }
        }

        private void ShowAllAddedBasesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ShowAllAddedBases();
            }
        }

        private void HideAllAddedBasesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.HideAllAddedBases();
            }
        }

        private bool IsGridModel(GeometryModel3D hitgeo)
        {
            GridLinesVisual3D gridLines = _viewPort.Children
                .OfType<GridLinesVisual3D>()
                .FirstOrDefault();
            return !(gridLines == null) && gridLines.Model.Equals(hitgeo);
        }

        private void HelixViewport3D_KeyUp(object sender, KeyEventArgs e)
        {
            MainViewModel mainViewModel = (MainViewModel)DataContext;
            switch (e.Key)
            {
                case Key.Escape:
                    mainViewModel.SelectedViewModel = null;
                    break;

                case Key.Back:
                case Key.Delete:
                    ViewModelBase? selectedModel = mainViewModel.SelectedViewModel;
                    if (selectedModel != null)
                    {
                        if (selectedModel is ScatterGraphViewModel scatterGraphViewModel)
                        {
                            mainViewModel.ScatterGraphs.Remove(scatterGraphViewModel);
                        }
                        else if (selectedModel is SampleViewModel sampleViewModel && !(selectedModel is BarycenterViewModel))
                        {
                            sampleViewModel.InvokeRemoveItem();
                        }
                    }
                    break;
            }
        }

        private void HelixViewport3D_MouseMove(object sender, MouseEventArgs e)
        {
            HelixViewport3D viewport3D = (HelixViewport3D)sender;

            Point mouseposition = e.GetPosition(viewport3D);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);
            PointHitTestParameters pointparams = new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);

            //test for a result in the Viewport3D
            VisualTreeHelper.HitTest(viewport3D, null, HTResult, pointparams);
        }

        public HitTestResultBehavior HTResult(HitTestResult rawresult)
        {
            MainViewModel viewModel = (MainViewModel)DataContext;
            if (rawresult is RayHitTestResult rayResult &&
                rayResult is RayMeshGeometry3DHitTestResult rayMeshResult &&
                rayMeshResult.ModelHit is GeometryModel3D hitgeo &&
                !viewModel.IsBelongingToOriginModel(hitgeo) &&
                !IsGridModel(hitgeo))
            {
                if (Cursor != Cursors.Hand)
                {
                    Cursor = Cursors.Hand;
                }
                if (!hitgeo.Equals(_geometryModel3DMouseOver))
                {
                    if (_geometryModel3DMouseOver != null)
                    {
                        viewModel.UnselectMouseOverGeometry();
                    }
                    _geometryModel3DMouseOver = hitgeo;
                    viewModel.SelectMouseOverGeometry(_geometryModel3DMouseOver);
                }
            }
            else
            {
                if (Cursor != Cursors.Arrow)
                {
                    Cursor = Cursors.Arrow;
                }
                _geometryModel3DMouseOver = null;
                viewModel.UnselectMouseOverGeometry();
            }
            return HitTestResultBehavior.Stop;
        }

        private void HelixViewport3D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_geometryModel3DMouseOver != null && DataContext is MainViewModel viewModel)
            {
                viewModel.SelectHitGeometry(_geometryModel3DMouseOver);
            }
        }

        private void ViewPort_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Cursor != Cursors.Arrow)
            {
                Cursor = Cursors.Arrow;
            }
        }
    }
}

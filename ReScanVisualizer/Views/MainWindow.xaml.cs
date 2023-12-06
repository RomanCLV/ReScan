using HelixToolkit.Wpf;
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
        private int shiftStartIndex;
        private int shiftEndIndex;

        public MainWindow()
        {
            InitializeComponent();
            _geometryModel3DMouseOver = null;
            shiftStartIndex = 0;
            shiftEndIndex = 0;
        }

        #region Bases tree view

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
                    case Key.Escape:
                        ((MainViewModel)DataContext).SelectedViewModel = null;
                        break;
                }
            }
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

        #endregion

        #region Graphs tree view

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
                    case Key.Escape:
                        ((MainViewModel)DataContext).SelectedViewModel = null;
                        shiftStartIndex = 0;
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
            ScatterGraphClicked((ScatterGraphViewModel)((ScatterGraphTreeViewItemHeader)sender).DataContext);
        }

        private void BarycenterTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BarycenterClicked((BarycenterViewModel)((BarycenterTreeViewItemHeader)sender).DataContext);
        }

        private void AveragePlanTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AveragePlanClicked((PlanViewModel)((AveragePlanTreeViewItemHeader)sender).DataContext);
        }

        private void BaseTreeViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BaseClicked((Base3DViewModel)((BaseTreeViewItem)sender).DataContext);
        }

        private void SamplesTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ScatterGraphClicked((ScatterGraphViewModel)((SamplesTreeViewItemHeader)sender).DataContext);
        }

        private void SampleTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SampleClicked((SampleViewModel)((SampleTreeViewItemHeader)sender).DataContext);
        }

        private void BarycenterClicked(BarycenterViewModel barycenterViewModel)
        {
            if (DataContext is MainViewModel viewModel)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift))
                {
                    ScatterGraphClicked(barycenterViewModel.ScatterGraph!);
                }
                else
                {
                    viewModel.SelectedViewModel = barycenterViewModel;
                    SelectShiftSartItem(barycenterViewModel.ScatterGraph!);
                }
            }
        }

        private void AveragePlanClicked(PlanViewModel planViewModel)
        {
            if (DataContext is MainViewModel viewModel)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift))
                {
                    ScatterGraphClicked(planViewModel.ScatterGraph!);
                }
                else
                {
                    viewModel.SelectedViewModel = planViewModel;
                    SelectShiftSartItem(planViewModel.ScatterGraph!);
                }
            }
        }

        private void BaseClicked(Base3DViewModel base3DviewModel)
        {
            if (DataContext is MainViewModel viewModel)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift))
                {
                    ScatterGraphClicked(base3DviewModel.ScatterGraph!);
                }
                else
                {
                    viewModel.SelectedViewModel = new BaseViewModel(base3DviewModel);
                    SelectShiftSartItem(base3DviewModel.ScatterGraph!);
                }
            }
        }

        private void SampleClicked(SampleViewModel sampleViewModel)
        {
            if (DataContext is MainViewModel viewModel)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift))
                {
                    ScatterGraphClicked(sampleViewModel.ScatterGraph!);
                }
                else
                {
                    viewModel.SelectedViewModel = sampleViewModel;
                    SelectShiftSartItem(sampleViewModel.ScatterGraph!);
                }
            }
        }

        private void ScatterGraphClicked(ScatterGraphViewModel scatterGraphViewModel)
        {
            if (DataContext is MainViewModel viewModel)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    if (scatterGraphViewModel.IsSelected)
                    {
                        viewModel.ScatterGraphViewModelGroup.Remove(scatterGraphViewModel);
                        scatterGraphViewModel.Unselect();
                        if (viewModel.ScatterGraphViewModelGroup.Count == 0)
                        {
                            viewModel.SelectedViewModel = null;
                        }
                        else if (viewModel.ScatterGraphViewModelGroup.Count == 1)
                        {
                            viewModel.SelectedViewModel = viewModel.ScatterGraphViewModelGroup[0];
                        }
                        else
                        {
                            viewModel.SelectedViewModel = viewModel.ScatterGraphViewModelGroup;
                            viewModel.ScatterGraphViewModelGroup.SelectAll();
                        }
                    }
                    else
                    {
                        viewModel.ScatterGraphViewModelGroup.Add(scatterGraphViewModel);
                        if (viewModel.ScatterGraphViewModelGroup.Count > 1)
                        {
                            viewModel.SelectedViewModel = viewModel.ScatterGraphViewModelGroup;
                            viewModel.ScatterGraphViewModelGroup.SelectAll();
                        }
                        else
                        {
                            viewModel.SelectedViewModel = scatterGraphViewModel;
                            scatterGraphViewModel.Select();
                        }
                    }
                }
                else if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    SelectShiftEndItem(scatterGraphViewModel);
                    ApplyShiftSelection();
                }
                else
                {
                    SelectShiftSartItem(scatterGraphViewModel);
                    viewModel.ScatterGraphViewModelGroup.Clear();
                    viewModel.ScatterGraphViewModelGroup.Add(scatterGraphViewModel);
                    viewModel.SelectedViewModel = scatterGraphViewModel;
                    scatterGraphViewModel.Select();
                }
            }
        }

        private void SelectShiftSartItem(ScatterGraphViewModel scatterGraphViewModel)
        {
            shiftStartIndex = GraphsTreeView.Items.IndexOf(scatterGraphViewModel);
        }

        private void SelectShiftEndItem(ScatterGraphViewModel scatterGraphViewModel)
        {
            shiftEndIndex = GraphsTreeView.Items.IndexOf(scatterGraphViewModel);
        }

        private void ApplyShiftSelection()
        {
            MainViewModel viewModel = (MainViewModel)DataContext;
            viewModel.ScatterGraphViewModelGroup.Clear();
            for (int i = Math.Min(shiftStartIndex, shiftEndIndex); i <= Math.Max(shiftStartIndex, shiftEndIndex); i++)
            {
                viewModel.ScatterGraphViewModelGroup.Add((ScatterGraphViewModel)GraphsTreeView.Items[i]);
            }
            viewModel.SelectedViewModel = viewModel.ScatterGraphViewModelGroup;
            viewModel.ScatterGraphViewModelGroup.SelectAll();
        }

        #endregion

        #region Menu

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
                viewModel.RandomizeAllColorsAsync();
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

        #endregion

        #region Viewport

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
                        if (selectedModel is ScatterGraphGroupViewModel scatterGraphViewModelGroup)
                        {
                            foreach (ScatterGraphViewModel item in scatterGraphViewModelGroup)
                            {
                                mainViewModel.ScatterGraphs.Remove(item);
                            }
                            scatterGraphViewModelGroup.Clear();
                            mainViewModel.SelectedViewModel = null;
                        }
                        else if (selectedModel is ScatterGraphViewModel scatterGraphViewModel)
                        {
                            mainViewModel.ScatterGraphs.Remove(scatterGraphViewModel);
                        }
                        else if (selectedModel is SampleViewModel sampleViewModel && !(selectedModel is BarycenterViewModel))
                        {
                            sampleViewModel.ScatterGraph!.Samples.Remove(sampleViewModel);
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
                //ViewModelBase? oldSelectedModel = viewModel.SelectedViewModel;
                viewModel.SelectHitGeometry(_geometryModel3DMouseOver);
                //HandleMultiSelectionViewportClicked(oldSelectedModel, viewModel.SelectedViewModel);
                ViewModelBase? viewModelBase = viewModel.SelectedViewModel;
                if (viewModelBase is ScatterGraphViewModel scatterGraphViewModel)
                {
                    ScatterGraphClicked(scatterGraphViewModel);
                }
                else if (viewModelBase is BarycenterViewModel barycenterViewModel)
                {
                    BarycenterClicked(barycenterViewModel);
                }
                else if (viewModelBase is PlanViewModel planViewModel)
                {
                    AveragePlanClicked(planViewModel);
                }
                else if (viewModelBase is BaseViewModel baseViewModel)
                {
                    BaseClicked(baseViewModel.Base);
                }
                else if (viewModelBase is SampleViewModel sampleViewModel)
                {
                    SampleClicked(sampleViewModel);
                }
            }
        }

        private void HandleMultiSelectionViewportClicked(ViewModelBase? oldSelectedModel, ViewModelBase? currentSelectedModel)
        {
            if (currentSelectedModel != null)
            {
                MainViewModel viewModel = (MainViewModel)DataContext;
                if (oldSelectedModel is null)
                {
                    FindStartOrEndShiftElement(currentSelectedModel, SelectShiftSartItem);
                }
                else
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {

                    }
                    else if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        FindStartOrEndShiftElement(currentSelectedModel, SelectShiftEndItem);
                        ApplyShiftSelection();
                    }
                    else
                    {
                        FindStartOrEndShiftElement(currentSelectedModel, SelectShiftSartItem);
                    }
                }
            }
        }

        private void FindStartOrEndShiftElement(ViewModelBase viewModelBase, Action<ScatterGraphViewModel> action)
        {
            if (viewModelBase is ScatterGraphViewModel scatterGraphViewModel)
            {
                action(scatterGraphViewModel);
            }
            else if (viewModelBase is BarycenterViewModel barycenterViewModel)
            {
                action(barycenterViewModel.ScatterGraph!);
            }
            else if (viewModelBase is PlanViewModel planViewModel)
            {
                action(planViewModel.ScatterGraph!);
            }
            else if (viewModelBase is BaseViewModel baseViewModel)
            {
                action(baseViewModel.Base.ScatterGraph!);
            }
            else if (viewModelBase is SampleViewModel sampleViewModel)
            {
                action(sampleViewModel.ScatterGraph!);
            }
        }

        private void ViewPort_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Cursor != Cursors.Arrow)
            {
                Cursor = Cursors.Arrow;
            }
        }

        #endregion
    }
}

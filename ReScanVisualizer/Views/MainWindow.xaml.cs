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
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.Views.ItemTreeViews;
using ReScanVisualizer.ViewModels.Parts;
using ReScanVisualizer.ViewModels.Samples;
using HelixToolkit.Wpf;

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

        private static HelixViewport3D? s_viewport;

        public MainWindow()
        {
            InitializeComponent();
            _geometryModel3DMouseOver = null;
            shiftStartIndex = 0;
            shiftEndIndex = 0;

            s_viewport = _viewPort;
        }

        ~MainWindow()
        {
            if (s_viewport != null && s_viewport.Equals(_viewPort))
            {
                s_viewport = null;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).ModifierPipe.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((MainViewModel)DataContext).Dispose();
        }

        #region static

        public static HelixViewport3D? GetViewPort()
        {
            return s_viewport;
        }

        /// <summary>
        /// Get the viewport view ration calculated as ActualWidth / ActualHeight.
        /// </summary>
        /// <returns>Returns -1 if the viewport is null, else the ration ActualWidth / ActualHeight.</returns>
        public static double GetViewPortRatio()
        {
            return s_viewport is null ? -1.0 : s_viewport.ActualWidth / s_viewport.ActualHeight;
        }

        public static PerspectiveCamera? GetCamera()
        {
            return s_viewport?.Camera is PerspectiveCamera camera ? camera : null;
        }

        public static void SetCamera(CameraConfiguration cameraConfiguration, double animationTime = 0.3)
        {
            if (s_viewport != null)
            {
                Vector3D up = new Vector3D(0.0, 0.0, 1.0);
                if (Tools.AreVectorsColinear(cameraConfiguration.Direction, up))
                {
                    MessageBox.Show("Camera direction and camera up can't be colinear!\nGiven direction: " +  cameraConfiguration.Direction.ToString(), "Camera direction error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    s_viewport.Camera.LookAt(cameraConfiguration.Target, cameraConfiguration.Direction, up, animationTime);
                }
            }
        }

        #endregion

        private void TabControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    ((MainViewModel)DataContext).SelectedViewModel = null;
                    break;
            }
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

        private void BasesAddButton_Click(object sender, RoutedEventArgs e)
        {
            AddBaseCommandKey.Command.Execute(null);
        }

        private void BaseClearButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).ClearBases();
        }

        private void BaseTreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is BaseTreeViewItem tb && tb.DataContext is Base3DViewModel viewModel)
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

        private void ScatterGraphsAddButton_Click(object sender, RoutedEventArgs e)
        {
            AddScatterGraphCommandKey.Command.Execute(null);
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
                        UnselectCrtlItem(scatterGraphViewModel);
                    }
                    else
                    {
                        SelectCrtlItem(scatterGraphViewModel);
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

        private void UnselectCrtlItem(ScatterGraphViewModel scatterGraphViewModel)
        {
            MainViewModel viewModel = (MainViewModel)DataContext;
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

        private void SelectCrtlItem(ScatterGraphViewModel scatterGraphViewModel)
        {
            MainViewModel viewModel = (MainViewModel)DataContext;
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

        #region Parts tree view

        private void PartTreeViewHeader_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    ((MainViewModel)DataContext).SelectedViewModel = null;
                    break;
            }
        }

        private void PartTreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is PartTreeViewHeader tb && tb.DataContext is PartViewModelBase viewModel)
            {
                ((MainViewModel)DataContext).SelectedViewModel = viewModel;
            }
        }

        private void PartTreeViewItem_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is TreeViewItem tb && tb.DataContext is ScatterGraphViewModel viewModel)
            {
                switch (e.Key)
                {
                    case Key.Delete:
                    case Key.Back:
                        viewModel.Part!.Remove(viewModel);
                        break;
                    case Key.Escape:
                        ((MainViewModel)DataContext).SelectedViewModel = null;
                        break;
                }
            }
        }

        private void PartScatterGraphTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is PartScatterGraphTreeViewItem vi && vi.DataContext is ScatterGraphViewModel graph)
            {
                MainViewModel.GetInstance().SelectedViewModel = graph;
            }
        }

        private void PartsAddButton_Click(object sender, RoutedEventArgs e)
        {
            AddPartCommandKey.Command.Execute(null);
        }

        private void PartsClearButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).ClearParts();
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

        private void CubeHorizontalPositionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem.Tag != null && Tools.TryParse(menuItem.Tag!.ToString(), out HorizontalAlignment result))
            {
                _viewPort.ViewCubeHorizontalPosition = result;
            }
        }

        private void CubeVerticalPositionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem.Tag != null && Tools.TryParse(menuItem.Tag!.ToString(), out VerticalAlignment result))
            {
                _viewPort.ViewCubeVerticalPosition = result;
            }
        }

        private void CoordinateSystemHorizontalPositionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem.Tag != null && Tools.TryParse(menuItem.Tag!.ToString(), out HorizontalAlignment result))
            {
                _viewPort.CoordinateSystemHorizontalPosition = result;
            }
        }

        private void CoordinateSystemVerticalPositionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem.Tag != null && Tools.TryParse(menuItem.Tag!.ToString(), out VerticalAlignment result))
            {
                _viewPort.CoordinateSystemVerticalPosition = result;
            }
        }

        private void CameraHomePositionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetCamera(new CameraConfiguration(new Point3D(2.0, 16.0, 20.0), new Point3D()));
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

        private bool IsAxisLineModel(GeometryModel3D hitgeo)
        {
            IEnumerable<LinesVisual3D> enumerable = _viewPort.Children.OfType<LinesVisual3D>();
            foreach (LinesVisual3D line in enumerable)
            {
                if (line is ModelVisual3D visual3D)
                {
                    if (visual3D.Content.Equals(hitgeo))
                    {
                        return true;
                    }
                }
            }
            return false;
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
                !IsAxisLineModel(hitgeo) &&
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
                ViewModelBase? viewModelBase = viewModel.SelectedViewModel;
                if (viewModelBase is ScatterGraphViewModel scatterGraphViewModel)
                {
                    scatterGraphViewModel.Unselect();
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

        private void ViewPort_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Cursor != Cursors.Arrow)
            {
                Cursor = Cursors.Arrow;
            }
            if (_geometryModel3DMouseOver != null)
            {
                ((MainViewModel)DataContext).UnselectMouseOverGeometry();
                _geometryModel3DMouseOver = null;
            }
        }

        #endregion
    }
}

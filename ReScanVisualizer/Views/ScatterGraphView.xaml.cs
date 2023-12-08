using ReScanVisualizer.Models;
using ReScanVisualizer.UserControls;
using ReScanVisualizer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

#nullable enable

namespace ReScanVisualizer.Views
{
    /// <summary>
    /// Logique d'interaction pour ScatterGraphView.xaml
    /// </summary>
    public partial class ScatterGraphView : UserControl
    {
        private Popup? _openedPopup;

        public ScatterGraphView()
        {
            InitializeComponent();
            _openedPopup = null;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        private void BarycenterVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphView)
            {
                scatterGraphView.Barycenter.InverseIsHidden();
            }
        }

        private void AveraglePlanVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphView)
            {
                scatterGraphView.AveragePlan.InverseIsHidden();
            }
        }

        private void Base3DVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphView)
            {
                scatterGraphView.Base3D.InverseIsHidden();
            }
        }

        private void PointsVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel viewModel)
            {
                if (viewModel.ArePointsHidden)
                {
                    viewModel.ShowPoints();
                }
                else
                {
                    viewModel.HidePoints();
                }
            }
        }

        private void VisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphView)
            {
                scatterGraphView.InverseIsHidden();
            }
        }

        private void ExportGraphButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphView)
            {
                scatterGraphView.Export();
            }
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle rect)
            {
                StackPanel panel = (StackPanel)rect.Parent;
                Popup popup = panel.Children.OfType<Popup>().First();

                if (popup.DataContext is ScatterGraphViewModel element)
                {
                    ClosePopup();
                    _openedPopup = popup;
                    popup.Child.MouseLeave += Child_MouseLeave;
                    popup.IsOpen = true;
                    if (popup.Child is ColorSelector selector)
                    {
                        if (BarycenterRectangle.Equals(rect))
                        {
                            selector.Color = element.Barycenter.Color.Color;
                        }
                        else if (AveragePlanRectangle.Equals(rect))
                        {
                            selector.Color = element.AveragePlan.Color.Color;
                        }
                        else if (PointsRectangle.Equals(rect))
                        {
                            selector.Color = element.Color.Color;
                        }
                    }
                }
            }
        }

        private void Child_MouseLeave(object sender, MouseEventArgs e)
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            if (_openedPopup != null)
            {
                _openedPopup.Child.MouseLeave -= Child_MouseLeave;
                _openedPopup.IsOpen = false;
            }
        }

        private void ColorPopup_Closed(object sender, EventArgs e)
        {
            _openedPopup = null;
        }

        private void ColorSelector_ColorChanged(object sender, Color e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphViewModel)
            {
                if (BarycenterColorSelector.Equals(sender))
                {
                    scatterGraphViewModel.Barycenter.Color.Set(e);
                }
                else if (AveragePlanColorSelector.Equals(sender))
                {
                    scatterGraphViewModel.AveragePlan.Color.Set(e);
                }
                else if (PointsColorSelector.Equals(sender))
                {
                    scatterGraphViewModel.Color.Set(e);
                }
            }
        }

        private void ColorSelector_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphViewModel)
            {
                if (BarycenterColorSelector.Equals(sender))
                {
                    BarycenterColorSelector.Color = scatterGraphViewModel.Barycenter.Color.Color;
                }
                else if (AveragePlanColorSelector.Equals(sender))
                {
                    AveragePlanColorSelector.Color = scatterGraphViewModel.AveragePlan.Color.Color;
                }
                else if (PointsColorSelector.Equals(sender))
                {
                    PointsColorSelector.Color = scatterGraphViewModel.Color.Color;
                }
            }
        }

        private void SelectBarycenterButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphView)
            {
                ((MainViewModel)Application.Current.MainWindow.DataContext).SelectedViewModel = scatterGraphView.Barycenter;
                scatterGraphView.Select();
            }
        }

        private void SelectAveragePlanButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphView)
            {
                ((MainViewModel)Application.Current.MainWindow.DataContext).SelectedViewModel = scatterGraphView.AveragePlan;
                scatterGraphView.Select();
            }
        }

        private void SelectBaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphView)
            {
                ((MainViewModel)Application.Current.MainWindow.DataContext).SelectedViewModel = new BaseViewModel(scatterGraphView.Base3D);
                scatterGraphView.Select();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel viewModel)
            {
                AddPointViewModel addPointViewModel = new AddPointViewModel(viewModel);
                AddPointWindow window = new AddPointWindow
                {
                    Owner = Application.Current.MainWindow,
                    DataContext = addPointViewModel
                };
                window.ShowDialog();
                addPointViewModel.Dispose();
            }
        }

        private void ReduceButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel viewModel)
            {
                ReduceScatterGraphViewModel reduceScatterGraphViewModel = new ReduceScatterGraphViewModel(viewModel);
                ReduceScatterGraphWindow window = new ReduceScatterGraphWindow
                {
                    Owner = Application.Current.MainWindow,
                    DataContext = reduceScatterGraphViewModel
                };
                window.ShowDialog();
                reduceScatterGraphViewModel.Dispose();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel viewModel)
            {
                viewModel.Clear();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphViewModel)
            {
                ((MainViewModel)Application.Current.MainWindow.DataContext).ScatterGraphs.Remove(scatterGraphViewModel);
            }
        }
    }
}

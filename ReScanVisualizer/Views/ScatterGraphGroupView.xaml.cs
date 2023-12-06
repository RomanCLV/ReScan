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
    /// Logique d'interaction pour ScatterGraphGroupView.xaml
    /// </summary>
    public partial class ScatterGraphGroupView : UserControl
    {
        private Popup? _openedPopup;

        public ScatterGraphGroupView()
        {
            InitializeComponent();
            _openedPopup = null;
        }

        private void VisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            ((ScatterGraphGroupViewModel)DataContext).InverseAreHidden();
        }

        private void BarycenterVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            ((ScatterGraphGroupViewModel)DataContext).InverseAreBarycentersHidden();
        }

        private void AveraglePlanVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            ((ScatterGraphGroupViewModel)DataContext).InverseArePlansHidden();
        }

        private void Base3DVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            ((ScatterGraphGroupViewModel)DataContext).InverseAreBasesHidden();
        }

        private void PointsVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            ((ScatterGraphGroupViewModel)DataContext).InverseArePointsHidden();
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle rect)
            {
                StackPanel panel = (StackPanel)rect.Parent;
                Popup popup = panel.Children.OfType<Popup>().First();

                if (popup.DataContext is ScatterGraphGroupViewModel element)
                {
                    ClosePopup();
                    _openedPopup = popup;
                    popup.Child.MouseLeave += Child_MouseLeave;
                    popup.IsOpen = true;
                    if (popup.Child is ColorSelector selector)
                    {
                        if (BarycenterRectangle.Equals(rect))
                        {
                            selector.Color = element.BarycentersColorViewModel.Color;
                        }
                        else if (AveragePlanRectangle.Equals(rect))
                        {
                            selector.Color = element.PlansColorViewModel.Color;
                        }
                        else if (PointsRectangle.Equals(rect))
                        {
                            selector.Color = element.PointsColorViewModel.Color;
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
            if (DataContext is ScatterGraphGroupViewModel scatterGraphGroupViewModel)
            {
                if (BarycenterColorSelector.Equals(sender))
                {
                    scatterGraphGroupViewModel.BarycentersColorViewModel.Set(e);
                }
                else if (AveragePlanColorSelector.Equals(sender))
                {
                    scatterGraphGroupViewModel.PlansColorViewModel.Set(e);
                }
                else if (PointsColorSelector.Equals(sender))
                {
                    scatterGraphGroupViewModel.PointsColorViewModel.Set(e);
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
    }
}

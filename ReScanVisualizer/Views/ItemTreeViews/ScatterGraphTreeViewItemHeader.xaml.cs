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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReScanVisualizer.UserControls;
using ReScanVisualizer.ViewModels;

#nullable enable

namespace ReScanVisualizer.Views.ItemTreeViews
{
    /// <summary>
    /// Logique d'interaction pour ScatterGraphItemTreeView.xaml
    /// </summary>
    public partial class ScatterGraphTreeViewItemHeader : UserControl
    {
        private Popup? _openedPopup;

        public ScatterGraphTreeViewItemHeader()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel graph)
            {
                double ratio = Math.Max(1.0, MainWindow.GetViewPortRatio());
                MainWindow.SetCamera(graph.GetCameraConfigurationToFocus(MainWindow.GetCamera()!.FieldOfView, ratio));
            }
        }

        private void ColorSelector_ColorChanged(object sender, Color e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphViewModel)
            {
                scatterGraphViewModel.Color.Color = e;
            }
        }

        private void ColorPopup_Closed(object sender, EventArgs e)
        {
            _openedPopup = null;
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle rect)
            {
                StackPanel panel = (StackPanel)rect.Parent;
                Popup popup = (Popup)panel.Children[1];
                if (popup.DataContext is ScatterGraphViewModel scatterGraphViewModel)
                {
                    _openedPopup = popup;
                    popup.Child.MouseLeave += Child_MouseLeave;
                    popup.IsOpen = true;
                    if (popup.Child is ColorSelector selector)
                    {
                        selector.Color = scatterGraphViewModel.Color.Color;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphViewModel)
            {
                scatterGraphViewModel.InverseIsHidden();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel scatterGraphViewModel && sender is Button)
            {
                MainViewModel.GetInstance().ScatterGraphs.Remove(scatterGraphViewModel);
            }
        }
    }
}

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
    /// Logique d'interaction pour PlanView.xaml
    /// </summary>
    public partial class PlanView : UserControl
    {
        private Popup? _openedPopup;

        public PlanView()
        {
            InitializeComponent();
            _openedPopup = null;
        }

        public void ColorSelector_ColorChanged(object sender, Color c)
        {
            if (DataContext is PlanViewModel planViewModel)
            {
                planViewModel.Color.Color = c;
            }
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle rect)
            {
                StackPanel panel = (StackPanel)rect.Parent;
                Popup popup = (Popup)panel.Children[2];

                if (popup.DataContext is PlanViewModel planViewModel)
                {
                    ClosePopup();
                    _openedPopup = popup;
                    popup.Child.MouseLeave += Child_MouseLeave;
                    popup.IsOpen = true;
                    if (popup.Child is ColorSelector selector)
                    {
                        selector.Color = planViewModel.Color.Color;
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

        private void VisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PlanViewModel viewModel)
            {
                viewModel.InverseIsHidden();
            }
        }
    }
}

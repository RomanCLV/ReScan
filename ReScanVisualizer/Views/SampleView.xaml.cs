using ReScanVisualizer.UserControls;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
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

namespace ReScanVisualizer.Views
{
    /// <summary>
    /// Logique d'interaction pour Point3DView.xaml
    /// </summary>
    public partial class SampleView : UserControl
    {
        public SampleView()
        {
            InitializeComponent();
        }
        

        public void ColorSelector_ColorChanged(object sender, Color c)
        {
            if (DataContext is SampleViewModel sampleViewModel)
            {
                sampleViewModel.Color.Color = c;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SampleViewModel viewModel)
            {
                viewModel.InvokeRemoveItem();
            }
        }

        private ScatterGraphBuilderBase? _selectedBuilder;
        private Popup? _openedPopup;


        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle rect)
            {
                StackPanel panel = (StackPanel)rect.Parent;
                Popup popup = (Popup)panel.Children[1];
                if (popup.DataContext is KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> item)
                {
                    ClosePopup();
                    _selectedBuilder = item.Key;
                    _openedPopup = popup;
                    popup.Child.MouseLeave += Child_MouseLeave;
                    popup.IsOpen = true;
                    if (popup.Child is ColorSelector selector)
                    {
                        selector.Color = item.Key.Color;
                    }
                }
                else
                {
                    _selectedBuilder = null;
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
            _selectedBuilder = null;
            _openedPopup = null;
        }
    }
}

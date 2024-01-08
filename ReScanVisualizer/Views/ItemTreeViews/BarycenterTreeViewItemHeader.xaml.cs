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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels.Samples;

namespace ReScanVisualizer.Views.ItemTreeViews
{
    /// <summary>
    /// Logique d'interaction pour BarycenterItemTreeView.xaml
    /// </summary>
    public partial class BarycenterTreeViewItemHeader : UserControl
    {
        public BarycenterTreeViewItemHeader()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ICameraFocusable cameraFocusable)
            {
                MainWindow.SetCamera(cameraFocusable.GetCameraConfigurationToFocus(MainWindow.GetCamera().FieldOfView));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SampleViewModel viewModel)
            {
                viewModel.InverseIsHidden();
            }
        }
    }
}

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

namespace ReScanVisualizer.Views.ItemTreeViews
{
    /// <summary>
    /// Logique d'interaction pour BaseTreeViewItemHeader.xaml
    /// </summary>
    public partial class BaseTreeViewItemHeader : UserControl
    {
        public BaseTreeViewItemHeader()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is Base3DViewModel base3D)
            {
                double ratio = Math.Max(1.0, MainWindow.GetViewPortRatio());
                MainWindow.SetCamera(base3D.GetCameraConfigurationToFocus(MainWindow.GetCamera()!.FieldOfView, ratio));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Base3DViewModel viewModel)
            {
                viewModel.InverseIsHidden();
            }
        }
    }
}

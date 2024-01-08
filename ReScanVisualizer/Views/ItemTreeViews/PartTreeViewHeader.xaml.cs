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
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.ViewModels.Parts;

namespace ReScanVisualizer.Views.ItemTreeViews
{
    /// <summary>
    /// Logique d'interaction pour PartTreeViewHeader.xaml
    /// </summary>
    public partial class PartTreeViewHeader : UserControl
    {
        public PartTreeViewHeader()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is PartViewModelBase partViewModel)
            {
                MainWindow.SetCamera(partViewModel.GetCameraConfigurationToFocus(MainWindow.GetCamera()!.FieldOfView));
            }
        }

        private void VisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartViewModelBase partViewModel)
            {
                partViewModel.InverseIsHidden();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartViewModelBase partViewModel)
            {
                MainViewModel.GetInstance().Parts.Remove(partViewModel);
            }
        }
    }
}

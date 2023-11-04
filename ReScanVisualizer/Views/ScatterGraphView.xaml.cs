using ReScanVisualizer.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReScanVisualizer.Views
{
    /// <summary>
    /// Logique d'interaction pour ScatterGraphView.xaml
    /// </summary>
    public partial class ScatterGraphView : UserControl
    {
        public ScatterGraphView()
        {
            InitializeComponent();
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
    }
}

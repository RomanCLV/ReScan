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

namespace ReScanVisualizer.Views.ItemTreeViews
{
    /// <summary>
    /// Logique d'interaction pour PointsItemTreeView.xaml
    /// </summary>
    public partial class SamplesTreeViewItemHeader : UserControl
    {
        public SamplesTreeViewItemHeader()
        {
            InitializeComponent();
        }

        private void VisibilityButton_Click(object sender, RoutedEventArgs e)
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
                ReduceScatterGraphViewModel reduceViewModel = new ReduceScatterGraphViewModel(viewModel);
                ReduceScatterGraphWindow window = new ReduceScatterGraphWindow
                {
                    Owner = Application.Current.MainWindow,
                    DataContext = reduceViewModel
                };
                window.ShowDialog();
                reduceViewModel.Dispose();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScatterGraphViewModel viewModel)
            {
                viewModel.Clear();
            }
        }
    }
}

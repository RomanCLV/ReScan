using HelixToolkit.Wpf;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.Views.ItemTreeViews;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

#nullable enable

namespace ReScanVisualizer.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BaseClearButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).Bases.Clear();
        }

        private void BaseTreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem tb && tb.DataContext is Base3DViewModel viewModel)
            {
                ((MainViewModel)DataContext).SelectedViewModel = new BaseViewModel(viewModel);
            }
        }

        private void BaseTreeViewItem_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is TreeViewItem tb && tb.DataContext is Base3DViewModel viewModel)
            {
                switch (e.Key)
                {
                    case Key.Delete:
                    case Key.Back:
                        ((MainViewModel)DataContext).Bases.Remove(viewModel);
                        break;
                }
            }
        }

        private void GraphClearButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).ScatterGraphs.Clear();
        }

        private void ScatterGraphTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedViewModel = ((ScatterGraphTreeViewItemHeader)sender).DataContext as ViewModelBase;
            }
        }

        private void ScatterGraphTreeViewItem_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is TreeViewItem tb && tb.DataContext is ScatterGraphViewModel viewModel)
            {
                switch (e.Key)
                {
                    case Key.Delete:
                    case Key.Back:
                        ((MainViewModel)DataContext).ScatterGraphs.Remove(viewModel);
                        break;
                }
            }
        }

        private void BarycenterTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedViewModel = (SampleViewModel) ((BarycenterTreeViewItemHeader)sender).DataContext;
            }
        }

        private void AveragePlanTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedViewModel = (PlanViewModel) ((AveragePlanTreeViewItemHeader)sender).DataContext;
            }
        }

        private void BaseTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                Base3DViewModel base3DviewModel = (Base3DViewModel)((BaseTreeViewItemHeader)sender).DataContext;
                viewModel.SelectedViewModel = new BaseViewModel(base3DviewModel);
            }
        }

        private void SampleTreeViewItemHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedViewModel = (SampleViewModel)((SampleTreeViewItemHeader)sender).DataContext;
            }
        }
    }
}

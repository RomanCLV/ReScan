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
using System.Windows.Shapes;

namespace ReScanVisualizer.Views
{
    /// <summary>
    /// Logique d'interaction pour ReduceScatterGraphWindow.xaml
    /// </summary>
    public partial class ReduceScatterGraphWindow : Window
    {
        public ReduceScatterGraphWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ReduceButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ReduceScatterGraphViewModel reduceScatterGraphViewModel)
            {
                reduceScatterGraphViewModel.Reduce();
                Close();
            }
        }
    }
}

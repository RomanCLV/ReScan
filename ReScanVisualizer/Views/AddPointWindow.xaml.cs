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
using ReScanVisualizer.ViewModels;

namespace ReScanVisualizer.Views
{
    /// <summary>
    /// Logique d'interaction pour AddPointWindow.xaml
    /// </summary>
    public partial class AddPointWindow : Window
    {
        public AddPointWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddPointViewModel viewModel)
            {
                viewModel.AddPoint();
                Close();
            }
        }
    }
}

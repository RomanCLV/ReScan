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

#nullable enable

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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                int index = comboBox.SelectedIndex;
                percentTextBox.Visibility = index == 0 ? Visibility.Visible : Visibility.Hidden;
                skippedTextBox.Visibility = index == 1 ? Visibility.Visible : Visibility.Hidden;
                maxPointsTextBox.Visibility = index == 2 ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}

using ReScanVisualizer.ViewModels.AddScatterGraph;
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

namespace ReScanVisualizer.Views.AddScatterGraphViews
{
    /// <summary>
    /// Logique d'interaction pour LoadScatterGraphView.xaml
    /// </summary>
    public partial class AddScatterGraphView : Window
    {
        public AddScatterGraphView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddScatterGraphViewModel viewModel)
            {
                viewModel.AddScatterGraphBuilderCommand.Execute(null);
            }
        }

        private void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            // TODO : ListView_KeyUp
        }
    }
}

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
    /// Logique d'interaction pour EditScatterGraphBuilderView.xaml
    /// </summary>
    public partial class EditScatterGraphBuilderView : Window
    {
        public EditScatterGraphBuilderView()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is EditScatterGraphViewModel viewModel)
            {
                if (!viewModel.Builder.CanBuild)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}

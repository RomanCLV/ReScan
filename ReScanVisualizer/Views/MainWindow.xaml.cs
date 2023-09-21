using ReScanVisualizer.Models;
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
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainViewModel dataContext = ((MainViewModel)DataContext);

            ScatterGraph scatterGraph1 = new ScatterGraph();
            ScatterGraph.PopulateWithRandomPoints(scatterGraph1, 7, -5, 5, -5, 5, -5, 5);

            ScatterGraph scatterGraph2 = new ScatterGraph();
            ScatterGraph.PopulateWithRandomPoints(scatterGraph2, 15, -5, 5, -5, 5, -5, 5);

            dataContext.ScatterGraphs.Add(new ScatterGraphViewModel(scatterGraph1));
            dataContext.ScatterGraphs.Add(new ScatterGraphViewModel(scatterGraph2));
        }
    }
}

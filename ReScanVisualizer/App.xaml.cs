using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.Views;

namespace ReScanVisualizer
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static DateTime StartedDate { get; private set; }

        public App()
        {
            StartedDate = DateTime.Now;

            ScatterGraph graph = new ScatterGraph();
            for (int i = 0; i < 1000; i++)
            {
                graph.AddPoint(new System.Windows.Media.Media3D.Point3D(i, i, i));
            }
            int[] numbers = new int[7] { 0, 10, 50, 33, 48, 95, 1005 };

            for (int i = 0; i < 7; i++)
            {
                ScatterGraph g = new ScatterGraph(graph);
                g.Reduce(numbers[i]);
                ScatterGraph.SaveCSV($"test {i} ({numbers[i]}).csv", g, true, true);
                g.Clear();
            }
            graph.Clear();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = MainViewModel.GetInstance()
            };
            MainWindow.Show();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            MainViewModel.GetInstance().Dispose();
            base.OnExit(e);
        }
    }
}

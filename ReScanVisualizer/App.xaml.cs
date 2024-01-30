using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

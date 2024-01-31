using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using ReScanVisualizer.Views;
using ReScanVisualizer.ViewModels;

#nullable enable

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

        protected override void OnExit(ExitEventArgs e)
        {
            MainViewModel mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Dispose();
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainViewModel mainViewModel = MainViewModel.GetInstance();
            MainWindow = new MainWindow()
            {
                DataContext = mainViewModel
            };
            if (e.Args.Length > 0)
            {
                mainViewModel.ModifierPipe.Pipe(e.Args);
            }
            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}

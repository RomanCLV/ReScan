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
using ReScanVisualizer.Models.CommandLineParser;
using ReScanVisualizer.Models.CommandLineParser.Options;

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
            /*
             -abs test902_bases_2024-01-29_15-51-32_cartesian.csv 0,02
             */

            CommandLineParser? commandLineParser = null;

            try
            {
                commandLineParser = CommandLineParser.Parse(e.Args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.InnerException.Message + "\n\nArguments: " + string.Join(" ", e.Args),
                    "Command line parsing error: " + ex.InnerException.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (commandLineParser != null && commandLineParser.HasValue<CommandLineOptionHelp>())
            {
                var help = commandLineParser.Get<CommandLineOptionHelp>();
                if (string.IsNullOrEmpty(help.Command))
                {
                    Help();
                }
                else
                {
                    HelpCommand(help.Command);
                }
            }

            MainWindow = new MainWindow()
            {
                DataContext = MainViewModel.GetInstance()
            };

            if (commandLineParser != null)
            {
                foreach (var item in commandLineParser.Items)
                {
                    if (item is CommandLineOptionAddBases vb)
                    {
                        ViewBases(vb);
                    }
                }
            }

            MainWindow.Show();
            base.OnStartup(e);
        }

        private void Help()
        {
            string content = CommandLineParser.Help();
            Dispatcher.InvokeAsync(() => MessageBox.Show(content, "Help", MessageBoxButton.OK, MessageBoxImage.Information));
        }

        private void HelpCommand(string option)
        {
            string content = string.Join("\n", CommandLineParser.HelpOption("-" + option).ToArray());
            Dispatcher.InvokeAsync(() => MessageBox.Show(content, "Option help", MessageBoxButton.OK, MessageBoxImage.Information));
        }

        private void ViewBases(CommandLineOptionAddBases vb)
        {
            ImportBasesViewModel importBasesViewModel = new ImportBasesViewModel(MainViewModel.GetInstance(), null)
            {
                FilePath = vb.FilePath,
                ScaleFactor = vb.ScaleFactor,
                ContainsHeader = vb.ContainsHeader,
                AxisScaleFactor = vb.AxisScaleFactor,
                RenderQuality = vb.RenderQuality
            };
            importBasesViewModel.ImportFile();
            System.Collections.ObjectModel.ObservableCollection<Base3DViewModel> bases = MainViewModel.GetInstance().Bases;
            List<Rect3D> rects = new List<Rect3D>(bases.Count);
            foreach (var base3D in bases)
            {
                if (base3D != null)
                {
                    Rect3D bounds = base3D.Model.Bounds;
                    if (!bounds.IsEmpty && !double.IsNaN(bounds.SizeX) && !double.IsNaN(bounds.SizeY) && !double.IsNaN(bounds.SizeZ))
                    {
                        rects.Add(bounds);
                    }
                }
            }

            if (rects.Count > 0)
            {
                Rect3D globalRect = Tools.GetGlobalRect(rects);
                Views.MainWindow.SetCamera(CameraHelper.GetCameraConfigurationToFocus(globalRect));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.Models.Parser.Options;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using ReScanVisualizer.Models.Parser;
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Net;

#nullable enable

namespace ReScanVisualizer.Models.Pipes
{
    public class CommandLinePipe : PipeBase
    {
        private readonly MainViewModel _mainViewModel;
        protected readonly Queue<string[]> _args;
        private readonly Task _task;

        public CommandLinePipe(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _args = new Queue<string[]>();
            _task = new Task(Run);
        }

        public override void Start()
        {
            if (!_isStarted)
            {
                base.Start();
                _task.Start();
            }
        }

        public void Pipe(string[] args)
        {
            _args.Enqueue(args);
        }

        public void Pipe(List<string> args)
        {
            _args.Enqueue(args.ToArray());
        }

        public void Clear()
        {
            _args.Clear();
        }

        protected override void Run()
        {
            while (_isStarted)
            {
                if (_args.Count > 0)
                {
                    CommandLineParser? commandLineParser = null;
                    string[] args = _args.Dequeue();
                    try
                    {
                        commandLineParser = CommandLineParser.Parse(args);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.InnerException.Message + "\n\nArguments: " + string.Join(" ", args),
                            "Command line parsing error: " + ex.InnerException.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    if (commandLineParser != null)
                    {
                        foreach (var item in commandLineParser.Items)
                        {
                            try
                            {
                                if (item is CommandLineOptionHelp help)
                                {
                                    ApplyHelp(help);
                                }
                                else if (item is CommandLineOptionAddBases abs)
                                {
                                    ApplyViewBases(abs);
                                }
                                else if (item is CommandLineOptionUDP udp)
                                {
                                    ApplyUDP(udp);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(
                                    "Command type: " + item.GetType().Name + "\n\n" + ex.Message,
                                    "Applying command error: " + ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                else
                {
                    Task.Delay(50).Wait();
                }
            }
        }

        private void ApplyHelp(CommandLineOptionHelp help)
        {
            if (string.IsNullOrEmpty(help.Command))
            {
                string content = CommandLineParser.Help();
                MessageBox.Show(content, "Help", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string content = string.Join("\n", CommandLineParser.HelpOption("-" + help.Command).ToArray());
                MessageBox.Show(content, "Option help", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ApplyViewBases(CommandLineOptionAddBases abs)
        {
            if (!File.Exists(abs.FilePath))
            {
                throw new FileNotFoundException("File not found", abs.FilePath);
            }

            ImportBasesViewModel importBasesViewModel = new ImportBasesViewModel(_mainViewModel, null)
            {
                FilePath = abs.FilePath,
                ScaleFactor = abs.ScaleFactor,
                ContainsHeader = abs.ContainsHeader,
                AxisScaleFactor = abs.AxisScaleFactor,
                RenderQuality = abs.RenderQuality
            };
            importBasesViewModel.ImportFile();
            ObservableCollection<Base3DViewModel> bases = _mainViewModel.Bases;
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

        private void ApplyUDP(CommandLineOptionUDP udp)
        {
            _mainViewModel.StartUDPPipe(udp.Port);
        }
    }
}

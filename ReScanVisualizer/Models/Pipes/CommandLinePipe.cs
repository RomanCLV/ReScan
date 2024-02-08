using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using ReScanVisualizer.Models.Parser;
using ReScanVisualizer.Models.Parser.Options;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders;

#nullable enable

namespace ReScanVisualizer.Models.Pipes
{
    public class CommandLinePipe : PipeBase
    {
        private readonly MainViewModel _mainViewModel;
        protected readonly Queue<string[]> _args;
        private readonly Task _task;

        private int _maxPoints;

        public CommandLinePipe(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _args = new Queue<string[]>();
            _task = new Task(Run);
            _maxPoints = -1;
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
                    for (int i = 0; i < args.Length; i++)
                    {
                        args[i] = args[i].Replace("\\\\", "\\").Replace('/', '\\');
                    }
                    try
                    {
                        commandLineParser = CommandLineParser.Parse(args);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            ex = ex.InnerException;
                        }
                        MessageBox.Show(
                            ex.Message + "\n\nArguments: " + string.Join(" ", args),
                            "Command line parsing error: " + ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
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
                                else if (item is CommandLineOptionUDP udp)
                                {
                                    ApplyUDP(udp);
                                }
                                else if (item is CommandLineOptionMaxPoints maxPoints)
                                {
                                    ApplyMaxPoints(maxPoints);
                                }
                                else if (item is CommandLineOptionAddGraph ag)
                                {
                                    ApplyAddGraph(ag);
                                }
                                else if (item is CommandLineOptionAddBases abs)
                                {
                                    ApplyAddBases(abs);
                                }
                                else if (item is CommandLineOptionClearGraphs)
                                {
                                    ApplyClearGraphs();
                                }
                                else if (item is CommandLineOptionClearBases)
                                {
                                    ApplyClearBases();
                                }
                                else if (item is CommandLineOptionKill)
                                {
                                    ApplyKill();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(
                                    "Command type: " + item.GetType().Name + "\n\n" + ex.Message + "\n\nArguments: " + string.Join(" ", args),
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

        private void ApplyUDP(CommandLineOptionUDP udp)
        {
            if (udp.IsToOpen)
            {
                _mainViewModel.StartUDPPipe(udp.Port);
            }
            else
            {
                _mainViewModel.StopUDPPipe(udp.Port);
            }
        }

        private void ApplyMaxPoints(CommandLineOptionMaxPoints maxPoints)
        {
            if (maxPoints.ToReset)
            {
                _maxPoints = -1;
            }
            else
            {
                _maxPoints = (int)maxPoints.MaxPoints;
            }
        }

        private async void ApplyAddGraph(CommandLineOptionAddGraph ag)
        {
            AddScatterGraphViewModel addScatterGraphViewModel = new AddScatterGraphViewModel(null, _mainViewModel);
            ScatterGraphFileBuilder scatterGraphFileBuilder = new ScatterGraphFileBuilder(ag.FilePath, Colors.White, ag.ContainsHeader);
            addScatterGraphViewModel.AddBuilder(scatterGraphFileBuilder);

            await addScatterGraphViewModel.BuildAllAsync();
            foreach (var item in addScatterGraphViewModel.Items)
            {
                if (!item.Value!.IsSuccess)
                {
                    MessageBox.Show($"{item.Key!.Details}\n{item.Value.Exception!.Message}", item.Value.Exception.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (_maxPoints > 0)
            {
                addScatterGraphViewModel.MaxPoints = (uint)_maxPoints;
                addScatterGraphViewModel.ApplyMaxPoints();
            }

            addScatterGraphViewModel.CommonScaleFactor = ag.ScaleFactor;
            addScatterGraphViewModel.CommonAxisScaleFactor = ag.AxisScaleFactor;
            addScatterGraphViewModel.CommonPointRadius = ag.PointRadius;
            addScatterGraphViewModel.CommonMaxPointsToDisplay = ag.MaxPointsDisplayed;
            addScatterGraphViewModel.CommonDisplayBarycenter = ag.DisplayBarycenter;
            addScatterGraphViewModel.CommonDisplayAveragePlan = ag.DisplayAveragePlan;
            addScatterGraphViewModel.CommonDisplayBase = ag.DisplayBase;
            addScatterGraphViewModel.CommonRenderQuality = ag.RenderQuality;

            addScatterGraphViewModel.ApplyCommonScaleFactor();
            addScatterGraphViewModel.ApplyCommonAxisScaleFactor();
            addScatterGraphViewModel.ApplyCommonPointRadius();
            addScatterGraphViewModel.ApplyCommonMaxPointsToDisplay();
            addScatterGraphViewModel.ApplyCommonDisplayBarycenter();
            addScatterGraphViewModel.ApplyCommonDisplayAveragePlan();
            addScatterGraphViewModel.ApplyCommonDisplayBase();
            addScatterGraphViewModel.ApplyCommonRenderQuality();

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    addScatterGraphViewModel.LoadAll();

                    Rect3D bounds = _mainViewModel.ScatterGraphs.Last().Model.Bounds;
                    if (!bounds.IsEmpty && !double.IsNaN(bounds.SizeX) && !double.IsNaN(bounds.SizeY) && !double.IsNaN(bounds.SizeZ))
                    {
                        Views.MainWindow.SetCamera(CameraHelper.GetCameraConfigurationToFocus(bounds));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                addScatterGraphViewModel.Dispose();
            });
        }

        private void ApplyAddBases(CommandLineOptionAddBases abs)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    if (!File.Exists(abs.FilePath))
                    {
                        throw new FileNotFoundException("File not found", abs.FilePath);
                    }

                    ImportBasesViewModel importBasesViewModel = new ImportBasesViewModel(_mainViewModel, null)
                    {
                        FilePath = abs.FilePath,
                        IsCartesianMode = abs.IsCartesian,
                        ContainsHeader = abs.ContainsHeader,
                        ScaleFactor = abs.ScaleFactor,
                        AxisScaleFactor = abs.AxisScaleFactor,
                        RenderQuality = abs.RenderQuality
                    };

                    importBasesViewModel.ImportFile();
                    importBasesViewModel.Dispose();

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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private void ApplyClearGraphs()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    _mainViewModel.ClearScatterGraphs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private void ApplyClearBases()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    _mainViewModel.ClearBases();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private void ApplyKill()
        {
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }
    }
}

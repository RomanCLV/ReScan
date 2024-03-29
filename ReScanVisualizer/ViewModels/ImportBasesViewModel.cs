﻿using Microsoft.Win32;
using ReScanVisualizer.Commands;
using ReScanVisualizer.Models;
using ReScanVisualizer.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class ImportBasesViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private readonly ImportBasesWindow? _importBasesWindow;

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => SetValue(ref _filePath, value);
        }

        private bool _containsHeader;
        public bool ContainsHeader
        {
            get => _containsHeader;
            set => SetValue(ref _containsHeader, value);
        }

        private bool _isCartesianMode;
        public bool IsCartesianMode
        {
            get => _isCartesianMode;
            set
            {
                if (SetValue(ref _isCartesianMode, value))
                {
                    if (_isEulerAnglesMode == _isCartesianMode)
                    {
                        _isEulerAnglesMode = !_isCartesianMode;
                        OnPropertyChanged(nameof(IsEulerAnglesMode));
                    }
                }
            }
        }

        private bool _isEulerAnglesMode;
        public bool IsEulerAnglesMode
        {
            get => _isEulerAnglesMode;
            set
            {
                if (SetValue(ref _isEulerAnglesMode, value))
                {
                    if (_isCartesianMode == _isEulerAnglesMode)
                    {
                        _isCartesianMode = !_isEulerAnglesMode;
                        OnPropertyChanged(nameof(IsEulerAnglesMode));
                    }
                }
            }
        }

        private static readonly string _cartesianHeaders = "o_x;o_y;o_z;x_x;x_y;x_z;y_x;y_y;y_z;z_x;z_y;z_z";
        private static readonly string _eulerAnglesHeaders = "o_x;o_y;o_z;a;b;c";

        private double _scaleFactor;
        public double ScaleFactor
        {
            get => _scaleFactor;
            set => SetValue(ref _scaleFactor, value);
        }

        private double _axisScaleFactor;
        public double AxisScaleFactor
        {
            get => _axisScaleFactor;
            set => SetValue(ref _axisScaleFactor, value);
        }

        private RenderQuality _renderQuality;
        public RenderQuality RenderQuality
        {
            get => _renderQuality;
            set => SetValue(ref _renderQuality, value);
        }

        public List<RenderQuality> RenderQualities { get; }

        public CommandKey ValidateCommand { get; private set; }

        public CommandKey CancelCommand { get; private set; }

        public ImportBasesViewModel(MainViewModel mainViewModel, ImportBasesWindow? importBasesWindow)
        {
            _filePath = string.Empty;
            _containsHeader = true;
            _isCartesianMode = true;
            _isEulerAnglesMode = false;
            _scaleFactor = 1.0;
            _axisScaleFactor = 1.0;
            _renderQuality = RenderQuality.High;
            _mainViewModel = mainViewModel;
            _importBasesWindow = importBasesWindow;
            RenderQualities = Tools.GetRenderQualitiesList();
            ValidateCommand = new CommandKey(new ValidateImportBasesCommand(this), Key.Enter, ModifierKeys.None, "Import");
            CancelCommand = new CommandKey(_importBasesWindow is null ? ActionCommand.DoNothing : new ActionCommand(_importBasesWindow.Close), Key.Escape, ModifierKeys.None, "Cancel");
        }

        ~ImportBasesViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                ValidateCommand.Dispose();
                CancelCommand.Dispose();
                base.Dispose();
            }
        }

        public void SelectFile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select file",
                Filter = "Fichiers csv (*.csv)|*.csv;|Tous les fichiers (*.*)|*.*",
                DefaultExt = ".csv",
                Multiselect = false
            };

            if (ofd.ShowDialog() != null)
            {
                FilePath = ofd.FileName;
            }
        }

        public void ImportFile()
        {
            if (!string.IsNullOrEmpty(_filePath) && File.Exists(_filePath))
            {
                bool isError = false;
                List<Base3D> bases = new List<Base3D>();
                int columnsNumber = _isCartesianMode ? 12 : 6; // 12 = origin xyz(3) + x xyz(3) + y xyz(3) + z xyz(3) | 6 = origin xyz(3) + abc(3)
                try
                {
                    using (StreamReader reader = new StreamReader(_filePath))
                    {
                        string line;
                        int lineIndex = 0;

                        if (_containsHeader)
                        {
                            line = reader.ReadLine();
                        }

                        while (!isError && (line = reader.ReadLine()) != null)
                        {
                            lineIndex++;

                            string[] cells = line.Split(';');
                            if (cells.Length != columnsNumber)
                            {
                                isError = true;
                                MessageBox.Show($"Wrong format at line {lineIndex}: {cells.Length} columns instead of {columnsNumber}. Columns must be: {(_isCartesianMode ? _cartesianHeaders : _eulerAnglesHeaders)}", "Format error", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            }

                            double[] doubles = new double[columnsNumber];
                            for (int i = 0; i < columnsNumber; i++)
                            {
                                if (Tools.TryParse(cells[i], out double value))
                                {
                                    doubles[i] = value;
                                }
                                else
                                {
                                    isError = true;
                                    MessageBox.Show($"Wrong format at line {lineIndex} - cell {i + 1}: {cells[i]} can't be parse into number.", "Format error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    break;
                                }
                            }

                            if (_isCartesianMode)
                            {
                                bases.Add(new Base3D(
                                    new Point3D(doubles[0], doubles[1], doubles[2]),
                                    new Vector3D(doubles[3], doubles[4], doubles[5]),
                                    new Vector3D(doubles[6], doubles[7], doubles[8]),
                                    new Vector3D(doubles[9], doubles[10], doubles[11])));
                            }
                            else
                            {
                                Base3D base3D = new Base3D(new Point3D(doubles[0], doubles[1], doubles[2]));
                                base3D.SetFromEulerAnglesZYX(doubles[3], doubles[4], doubles[5]);
                                bases.Add(base3D);
                            }
                        }
                    }
                }
                catch
                {
                    isError = true;
                }

                if (!isError)
                {
                    if (bases.Count == 0)
                    {
                        MessageBox.Show($"No base in file: {_filePath}", "No base loaded", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        for (int i = 0; i < bases.Count; i++)
                        {
                            Base3DViewModel base3DViewModel = new Base3DViewModel(bases[i], _scaleFactor, _axisScaleFactor, _renderQuality)
                            {
                                Name = $"{Path.GetFileNameWithoutExtension(_filePath)} {i + 1}"
                            };
                            _mainViewModel.Bases.Add(base3DViewModel);
                        }
                        _importBasesWindow?.Close();
                    }
                }
            }
        }
    }
}

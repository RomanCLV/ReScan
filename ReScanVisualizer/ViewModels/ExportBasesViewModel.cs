using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using ReScanVisualizer.Commands;
using ReScanVisualizer.Views;
using ReScanVisualizer.Models;

namespace ReScanVisualizer.ViewModels
{
    public class ExportBasesViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;

        public ObservableCollection<ExportItemViewModel<Base3DViewModel>> Items { get; private set; }

        private bool _isGraphSourcesSelected;
        public bool IsGraphSourcesSelected
        {
            get => _isGraphSourcesSelected;
            set
            {
                if (SetValue(ref _isGraphSourcesSelected, value))
                {
                    UpdateItems();
                }
            }
        }

        private bool _isAddedBasesSourceSelected;
        public bool IsAddedBasesSourceSelected
        {
            get => _isAddedBasesSourceSelected;
            set
            {
                if (SetValue(ref _isAddedBasesSourceSelected, value))
                {
                    UpdateItems();
                }
            }
        }

        private bool _includeEmptyBases;
        public bool IncludeEmptyBases
        {
            get => _includeEmptyBases;
            set
            {
                if (SetValue(ref _includeEmptyBases, value) && _isGraphSourcesSelected)
                {
                    UpdateItems();
                }
            }
        }

        private bool _writeEmptyBasesWith0;
        public bool WriteEmptyBasesWith0
        {
            get => _writeEmptyBasesWith0;
            set => SetValue(ref _writeEmptyBasesWith0, value);
        }

        private bool _isCartesianMode;
        public bool IsCartesianMode
        {
            get => _isCartesianMode;
            set
            {
                if (SetValue(ref _isCartesianMode, value))
                {
                    if (_isCartesianMode && _isEulerAnglesMode)
                    {
                        _isEulerAnglesMode = false;
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
                    if (_isEulerAnglesMode && _isCartesianMode)
                    {
                        _isCartesianMode = false;
                        OnPropertyChanged(nameof(IsEulerAnglesMode));
                    }
                }
            }
        }

        private bool _isDecimalCharDot;
        public bool IsDecimalCharDot
        {
            get => _isDecimalCharDot;
            set => SetValue(ref _isDecimalCharDot, value);
        }

        private bool _isSelectingAll;

        private bool _selectAll;
        public bool SelectAll
        {
            get => _selectAll;
            set
            {
                if (SetValue(ref _selectAll, value))
                {
                    _isSelectingAll = true;
                    foreach (var item in Items)
                    {
                        item.IsSelected = _selectAll;
                    }
                    _isSelectingAll = false;
                }
            }
        }

        public CommandKey ValidateCommand { get; private set; }
        public CommandKey CancelCommand { get; private set; }

        public ExportBasesViewModel(MainViewModel mainViewModel, ExportBasesWindow exportBasesWindow)
        {
            _isGraphSourcesSelected = true;
            _isAddedBasesSourceSelected = false;
            _includeEmptyBases = true;
            _writeEmptyBasesWith0 = false;
            _isCartesianMode = true;
            _isEulerAnglesMode = false;
            _isDecimalCharDot = false;
            _isSelectingAll = false;
            _mainViewModel = mainViewModel;

            Items = new ObservableCollection<ExportItemViewModel<Base3DViewModel>>();

            UpdateItems();

            ValidateCommand = new CommandKey(new ValidateExportBasesCommand(this, exportBasesWindow), Key.Enter, ModifierKeys.None, "Export");
            CancelCommand = new CommandKey(new ActionCommand(exportBasesWindow.Close), Key.Escape, ModifierKeys.None, "Cancel");
        }

        ~ExportBasesViewModel()
        {
            Dispose();
        }

        private void UpdateItems()
        {
            foreach (var item in Items)
            {
                item.PropertyChanged -= ExportItemViewModel_PropertyChanged;
            }
            Items.Clear();
            if (_isGraphSourcesSelected)
            {
                foreach (ScatterGraphViewModel scatterGraph in _mainViewModel.ScatterGraphs)
                {
                    if (scatterGraph.Samples.Count != 0 || _includeEmptyBases)
                    {
                        ExportItemViewModel<Base3DViewModel> exportItemViewModel = new ExportItemViewModel<Base3DViewModel>(scatterGraph.Base3D, b => b.Name, scatterGraph, s => $"{((ScatterGraphViewModel)s).Name} (Count {((ScatterGraphViewModel)s).Samples.Count})", _selectAll);
                        exportItemViewModel.PropertyChanged += ExportItemViewModel_PropertyChanged;
                        Items.Add(exportItemViewModel);
                    }
                }
            }
            if (_isAddedBasesSourceSelected)
            {
                foreach (Base3DViewModel base3D in _mainViewModel.Bases)
                {
                    ExportItemViewModel<Base3DViewModel> exportItemViewModel = new ExportItemViewModel<Base3DViewModel>(base3D, b => b.Name, base3D, b => ((Base3DViewModel)b).Name, _selectAll);
                    exportItemViewModel.PropertyChanged += ExportItemViewModel_PropertyChanged;
                    Items.Add(exportItemViewModel);
                }
            }
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                ValidateCommand.Dispose();
                CancelCommand.Dispose();
                foreach (var item in Items)
                {
                    item.PropertyChanged -= ExportItemViewModel_PropertyChanged;
                }
                Items.Clear();
                base.Dispose();
            }
        }

        private void ExportItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExportItemViewModel<ViewModelBase>.IsSelected))
            {
                if (!_isSelectingAll)
                {
                    _selectAll = Items.AsQueryable().All(i => i.IsSelected);
                    OnPropertyChanged(nameof(SelectAll));
                }
            }
        }

        public bool Export()
        {
            bool success = false;
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Export bases",
                Filter = "Fichiers CSV (*.csv)|*.csv",
                DefaultExt = ".csv"
            };
            saveFileDialog.ShowDialog();

            if (!string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        if (_isCartesianMode)
                        {
                            writer.WriteLine("o_x;o_y;o_z;x_x;x_y;x_z;y_x;y_y;y_z;z_x;z_y;z_z");
                            foreach (var item in Items)
                            {
                                if (item.IsSelected)
                                {
                                    string line =
                                           $"{item.Value.Origin.X};{item.Value.Origin.Y};{item.Value.Origin.Z};" +
                                           $"{item.Value.X.X};{item.Value.X.Y};{item.Value.X.Z};" +
                                           $"{item.Value.Y.X};{item.Value.Y.Y};{item.Value.Y.Z};" +
                                           $"{item.Value.Z.X};{item.Value.Z.Y};{item.Value.Z.Z}";

                                    if (_isDecimalCharDot)
                                    {
                                        line = line.Replace(',', '.');
                                    }
                                    else
                                    {
                                        line = line.Replace('.', ',');
                                    }

                                    if (item.Source is ScatterGraphViewModel scatterGraphViewModel && scatterGraphViewModel.Samples.Count == 0 && _writeEmptyBasesWith0)
                                    {
                                        line = "0;0;0;0;0;0;0;0;0;0;0;0";
                                    }
                                    writer.WriteLine(line);
                                }
                            }
                        }
                        else if (_isEulerAnglesMode)
                        {
                            writer.WriteLine("o_x;o_y;o_z;a;b;c");
                            // TODO: euler angles
                        }
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return success;
        }
    }
}

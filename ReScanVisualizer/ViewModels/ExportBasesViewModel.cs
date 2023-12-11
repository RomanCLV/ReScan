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

namespace ReScanVisualizer.ViewModels
{
    public class ExportBasesViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;

        public ObservableCollection<ExportItemViewModel<Base3DViewModel>> Items { get; private set; }

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
            _isSelectingAll = false;
            _mainViewModel = mainViewModel;

            Items = new ObservableCollection<ExportItemViewModel<Base3DViewModel>>();

            foreach (Base3DViewModel base3D in _mainViewModel.Bases)
            {
                ExportItemViewModel<Base3DViewModel> exportItemViewModel = new ExportItemViewModel<Base3DViewModel>(base3D, b => b.Name);
                exportItemViewModel.PropertyChanged += ExportItemViewModel_PropertyChanged;
                Items.Add(exportItemViewModel);
            }
            foreach (ScatterGraphViewModel scatterGraph in _mainViewModel.ScatterGraphs)
            {
                ExportItemViewModel<Base3DViewModel> exportItemViewModel = new ExportItemViewModel<Base3DViewModel>(scatterGraph.Base3D, b => b.Name);
                exportItemViewModel.PropertyChanged += ExportItemViewModel_PropertyChanged;
                Items.Add(exportItemViewModel);
            }

            ValidateCommand = new CommandKey(new ValidateExportBasesCommand(this, exportBasesWindow), Key.Enter, ModifierKeys.None, "Export");
            CancelCommand = new CommandKey(new ActionCommand(exportBasesWindow.Close), Key.Escape, ModifierKeys.None, "Cancel");
        }

        ~ExportBasesViewModel()
        {
            Dispose();
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
                                writer.WriteLine(line);
                            }
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

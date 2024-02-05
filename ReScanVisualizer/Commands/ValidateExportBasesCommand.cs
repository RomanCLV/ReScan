using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.Views;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class ValidateExportBasesCommand : CommandBase
    {
        private readonly ExportBasesViewModel _viewModel;
        private readonly ExportBasesWindow _window;
        private readonly List<ExportItemViewModel<Base3DViewModel>> _items;

        public ValidateExportBasesCommand(ExportBasesViewModel exportBasesViewModel, ExportBasesWindow exportBasesWindow) 
        {
            _viewModel = exportBasesViewModel;
            _window = exportBasesWindow;
            _items = new List<ExportItemViewModel<Base3DViewModel>>(_viewModel.Items.Count);

            foreach (var item in _viewModel.Items)
            {
                _items.Add(item);
                item.PropertyChanged += Item_PropertyChanged;
            }
            _viewModel.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (!_viewModel.Items.Contains(_items[i]))
                {
                    _items[i].PropertyChanged -= Item_PropertyChanged;
                    _items.RemoveAt(i);
                    i--;
                }
            }

            foreach (var item in _viewModel.Items)
            {
                _items.Add(item);
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        ~ValidateExportBasesCommand()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                _viewModel.Items.CollectionChanged -= Items_CollectionChanged;
                foreach (var item in _items)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
                _items.Clear();
                foreach (var item in _viewModel.Items)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
                base.Dispose();
            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter) && _viewModel.Items.Any(x => x.IsSelected);
        }

        public override void Execute(object? parameter)
        {
            if (_viewModel.Export())
            {
                _window.Close();
            }
        }
    }
}

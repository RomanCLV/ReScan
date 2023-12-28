using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReScanVisualizer.Commands;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels.AddPartModelViews.Builders;
using ReScanVisualizer.Views.AddPartViews;

#nullable enable

namespace ReScanVisualizer.ViewModels.AddPartModelViews
{
    public class AddPartViewModel : ViewModelBase
    {
        private readonly IPartSource _partSource;

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (SetValue(ref _selectedIndex, value))
                {
                    UpdateBuilder();
                }
            }
        }

        private PartBuilderBase? _builder;
        public PartBuilderBase? Builder
        {
            get => _builder;
            set
            {
                if (_builder == null || !_builder.Equals(value))
                {
                    _builder?.Dispose();
                    if (SetValue(ref _builder, value))
                    {
                        PartVisualizerViewModel.Builder = _builder;
                    }
                }
            }
        }

        public CommandKey ValidateCommand { get; }
        public CommandKey CancelCommand { get; }

        public PartVisualizerViewModel PartVisualizerViewModel { get; private set; }

        public AddPartViewModel(AddPartWindow addPartView, IPartSource partSource)
        {
            _partSource = partSource;
            _selectedIndex = 0;
            _builder = new PartPointBuilder();
            PartVisualizerViewModel = new PartVisualizerViewModel
            {
                Builder = _builder
            };
            ValidateCommand = new CommandKey(new ValidateAddingPartCommand(addPartView, this), Key.Enter, ModifierKeys.None, "Add part");
            CancelCommand = new CommandKey(new ActionCommand(addPartView.Close), Key.Escape, ModifierKeys.None, "Cancel");
        }

        ~AddPartViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                ValidateCommand.Dispose();
                CancelCommand.Dispose();
                _builder?.Dispose();
                PartVisualizerViewModel.Dispose();
                base.Dispose();
            }
        }

        private void UpdateBuilder()
        {
            if (_selectedIndex == 0)
            {
                Builder = new PartPointBuilder();
            }
            else
            {
                Builder = null;
            }
        }

        public void Build()
        {
            if (_builder != null)
            {
                _partSource.Parts.Add(_builder.Build());
            }
        }
    }
}

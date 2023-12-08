using ReScanVisualizer.Commands;
using ReScanVisualizer.Views.AddPartViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReScanVisualizer.ViewModels.AddPartModelViews
{
    public class AddPartViewModel : ViewModelBase
    {
        public CommandKey ValidateCommand { get; }
        public CommandKey CancelCommand { get; }

        public AddPartViewModel(AddPartWindow addPartView, MainViewModel mainViewModel)
        {
            ValidateCommand = new CommandKey(new ValidateAddingPartCommand(addPartView, mainViewModel), Key.Enter, ModifierKeys.None, "Add part");
            CancelCommand = new CommandKey(new ActionCommand(addPartView.Close), Key.Escape, ModifierKeys.None, "Cancel");
        }
    }
}

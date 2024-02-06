using ReScanVisualizer.Commands;
using ReScanVisualizer.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReScanVisualizer.ViewModels
{
    public class OpenUDPPipeViewModel : ViewModelBase
    {
        private readonly OpenUDPPipeWindow _openUDPPipeWindow;
        private readonly MainViewModel _mainViewModel;

        private ushort _port;
        public ushort Port
        {
            get => _port;
            set => SetValue(ref _port, value);
        }

        public CommandKey OpenCommand { get; }
        public CommandKey CancelCommand { get; }

        public OpenUDPPipeViewModel(OpenUDPPipeWindow openUDPPipeWindow, MainViewModel mainViewModel) 
        {
            _openUDPPipeWindow = openUDPPipeWindow;
            _mainViewModel = mainViewModel;
            _port = 0;
            OpenCommand = new CommandKey(new ActionCommand(OpenPipe), Key.Enter, ModifierKeys.None, "Open the UDP pipe");
            CancelCommand = new CommandKey(new ActionCommand(_openUDPPipeWindow.Close), Key.Escape, ModifierKeys.None, "Cancel");
        }

        ~OpenUDPPipeViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                OpenCommand.Dispose();
                CancelCommand.Dispose();
                base.Dispose();
            }
        }

        private void OpenPipe()
        {
            if (_mainViewModel.StartUDPPipe(_port))
            {
                _openUDPPipeWindow.Close();
            }
        }
    }
}

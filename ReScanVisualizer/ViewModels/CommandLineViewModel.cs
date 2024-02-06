using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReScanVisualizer.Commands;
using ReScanVisualizer.Models.Pipes;
using ReScanVisualizer.Views;

namespace ReScanVisualizer.ViewModels
{
    public class CommandLineViewModel : ViewModelBase
    {
        private readonly CommandLineWindow _commandLineWindow;
        private readonly CommandLinePipe _pipe;

        private readonly List<string> _sentCommands;
        private uint _sentCommandsIndex;
        private string _currentCommand;

        private string _commandLine;
        public string CommandLine
        {
            get => _commandLine;
            set => SetValue(ref _commandLine, value);
        }

        public CommandKey SendCommand { get; }
        public CommandKey CancelCommand { get; }
        public CommandKey HelpCommand { get; }
        public CommandKey PreviousCommand { get; }
        public CommandKey NextCommand { get; }

        public CommandLineViewModel(CommandLineWindow commandLineWindow, CommandLinePipe pipe)
        {
            _commandLineWindow = commandLineWindow;
            _pipe = pipe;
            _sentCommands = new List<string>();
            _sentCommandsIndex = 0;
            _currentCommand = string.Empty;
            _commandLine = string.Empty;

            SendCommand = new CommandKey(new ActionCommand(ExectuteCommand), Key.Enter, ModifierKeys.None, "Send the command");
            CancelCommand = new CommandKey(new ActionCommand(_commandLineWindow.Close), Key.Escape, ModifierKeys.None, "Cancel");
            HelpCommand = new CommandKey(new ActionCommand(() => ExectuteCommand("-h")), Key.H, ModifierKeys.Control, "Help");
            PreviousCommand = new CommandKey(new ActionCommand(PreviousSentCommand), Key.Up, ModifierKeys.None, "Previous command");
            NextCommand = new CommandKey(new ActionCommand(NextSentCommand), Key.Down, ModifierKeys.None, "Next command");
        }

        ~CommandLineViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                _sentCommands.Clear();
                _sentCommandsIndex = 0;
                SendCommand.Dispose();
                CancelCommand.Dispose();
                HelpCommand.Dispose();
                PreviousCommand.Dispose();
                NextCommand.Dispose();
                base.Dispose();
            }
        }

        private void PreviousSentCommand()
        {
            if (_sentCommandsIndex > 0)
            {
                if (_sentCommandsIndex == _sentCommands.Count)
                {
                    _currentCommand = _commandLine;
                }
                _sentCommandsIndex--;
                CommandLine = _sentCommands[(int)_sentCommandsIndex];
            }
        }

        private void NextSentCommand()
        {
            if (_sentCommandsIndex < _sentCommands.Count)
            {
                _sentCommandsIndex++;
                CommandLine = _sentCommandsIndex == _sentCommands.Count ? _currentCommand : _sentCommands[(int)_sentCommandsIndex];
            }
        }

        private void ExectuteCommand()
        {
            if (!string.IsNullOrEmpty(_commandLine) && !string.IsNullOrWhiteSpace(_commandLine))
            {
                ExectuteCommand(_commandLine);
                _sentCommands.Add(_commandLine);
                _sentCommandsIndex = (uint)_sentCommands.Count;
                CommandLine = string.Empty;
            }
        }

        private void ExectuteCommand(string command)
        {
            string[] args = command.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            _pipe.Pipe(args);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class CommandKey : ViewModelBase
    {
        public ICommand Command { get; }

        private ModifierKeys _modifiers;
        public ModifierKeys Modifiers
        {
            get { return _modifiers; }
            private set { SetValue(ref _modifiers, value); }
        }

        private Key _key;
        public Key Key
        {
            get { return _key; }
            private set { SetValue(ref _key, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            private set { SetValue(ref _description, value); }
        }

        public CommandKey(ICommand command)
        {
            Command = command;
            _key = Key.None;
            _modifiers = ModifierKeys.None;
            _description = string.Empty;
        }

        public CommandKey(ICommand command, string description) : this(command)
        {
            _description = description;
        }

        public CommandKey(ICommand command, Key key, ModifierKeys modifierKeys) : this(command)
        {
            _key = key;
            _modifiers = modifierKeys;
        }

        public CommandKey(ICommand command, Key key, ModifierKeys modifierKeys, string description) : this(command, key, modifierKeys)
        {
            _description = description;
        }

        ~CommandKey()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                if (Command is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                base.Dispose();
            }
        }

        public bool CanExecute(object parameter)
        {
            return Command.CanExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            Command.Execute(parameter);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

#nullable enable

namespace ReScanVisualizer.Commands
{
	public abstract class CommandBase : ICommand, IDisposable
	{
		public event EventHandler? CanExecuteChanged;
		private bool _isDisposed;
        public bool IsDisposed
        {
            get => _isDisposed;
            protected set => _isDisposed = value;
        }

        public CommandBase()
        {
            _isDisposed = false;
        }

        public virtual void Dispose()
        {
            if (!_isDisposed)
			{
				_isDisposed |= true;
			}
        }

        public virtual bool CanExecute(object? parameter)
		{
			return true;
		}

        public abstract void Execute(object? parameter);

		protected void OnCanExecuteChanged()
		{
            if (Application.Current.Dispatcher.CheckAccess())
            {
				CanExecuteChanged?.Invoke(this, new EventArgs());
            }
			else
			{
				Application.Current.Dispatcher.Invoke(() => CanExecuteChanged?.Invoke(this, new EventArgs()));
			}
		}
	}
}

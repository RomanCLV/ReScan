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
	public abstract class CommandBase : ICommand
	{
		public event EventHandler? CanExecuteChanged;

		public virtual bool CanExecute(object? parameter)
		{
			return true;
		}

		public abstract void Execute(object? parameter);

		protected void OnCanExecuteChanged()
		{
			Application.Current.Dispatcher.Invoke(() => CanExecuteChanged?.Invoke(this, new EventArgs()));
		}
	}
}

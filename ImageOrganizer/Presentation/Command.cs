using System;
using System.Windows.Input;

namespace ImageOrganizer.Presentation
{
	/// <summary>
	/// 
	/// </summary>
	public class Command : ICommand
	{
		private readonly Action _action;
		private readonly Func<bool> _canExecute;

		public Command(Action action, Func<bool> canExecute = null)
		{
			_action = action;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute.Invoke();
		}

		public void Execute(object parameter)
		{
			if (CanExecute(parameter))
				_action.Invoke();
		}

		/// <summary>
		/// 
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			var handler = CanExecuteChanged;
			if (handler != null)
				handler.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// 
		/// </summary>
		public event EventHandler CanExecuteChanged;
	}
}

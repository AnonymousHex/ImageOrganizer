using System;
using System.Windows.Input;

namespace ImageOrganizer.Presentation
{
	/// <summary>
	/// A command for no parameters.
	/// </summary>
	public class Command : ICommand
	{
		private readonly Action _action;
		private readonly Func<bool> _canExecute;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="canExecute"></param>
		public Command(Action action, Func<bool> canExecute = null)
		{
			_action = action;
			_canExecute = canExecute;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute.Invoke();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameter"></param>
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

	/// <summary>
	/// A command for type parameters.
	/// </summary>
	public class Command<T> : ICommand
	{
		private readonly Action<T> _action;
		private readonly Func<T, bool> _canExecute;

		public Command(Action<T> action, Func<T, bool> canExecute = null)
		{
			_action = action;
			_canExecute = canExecute;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public bool CanExecute(object parameter)
		{
			return CanExecute((T) parameter);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public bool CanExecute(T parameter)
		{
			return _canExecute == null || _canExecute.Invoke(parameter);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameter"></param>
		public void Execute(object parameter)
		{
			var asT = (T) parameter;
			if (CanExecute(asT))
				_action.Invoke(asT);
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

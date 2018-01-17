using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ImageOrganizer.Presentation
{
	/// <summary>
	/// 
	/// </summary>
	public class ObservableObject : INotifyPropertyChanged
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="checkForCommand"></param>
		protected void RaisePropertyChanged(string propertyName, bool checkForCommand = true)
		{
			OnPropertyChanged(propertyName, checkForCommand);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyName"></param>
		/// <param name="field"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual bool Set<T>(string propertyName, ref T field, T value)
		{
			if (field != null && field.Equals(value))
				return false;

			field = value;

			OnPropertyChanged(propertyName);
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises the property changed event.  If the property was an instance of a Command object, 
		/// the CanExecutedChanged event will be raised as well.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="checkForCommand">Whether or not to check that the property is a command to raise the CanExecuteChanged event.</param>
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null, bool checkForCommand = true)
		{
			var handler = PropertyChanged;
			if (handler != null) 
				handler(this, new PropertyChangedEventArgs(propertyName));

			if (checkForCommand == false || propertyName == null)
				return;

			PropertyInfo property = GetType().GetProperty(propertyName);
			if (property == null)
				return;

			var command = property.GetValue(this, BindingFlags.GetProperty, null, null, null) as Command;
			if (command != null)
				command.RaiseCanExecuteChanged();
		}
	}
}

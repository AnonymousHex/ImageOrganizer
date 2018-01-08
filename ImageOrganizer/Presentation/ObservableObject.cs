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
		protected void RaisePropertyChanged(string propertyName)
		{
			OnPropertyChanged(propertyName);
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
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) 
				handler(this, new PropertyChangedEventArgs(propertyName));

			if (propertyName == null)
				return;

			var property = GetType().GetProperty(propertyName);
			if (property == null)
				return;

			var r = property.GetValue(this, BindingFlags.GetProperty, null, null, null) as Command;
			if (r != null)
				r.RaiseCanExecuteChanged();
		}
	}
}

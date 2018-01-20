using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using ImageOrganizer.Presentation;

namespace ImageOrganizer.Organization
{
	/// <summary>
	/// 
	/// </summary>
	public class ObservableImageCollection : ObservableObject, IList<ImageItem>, INotifyCollectionChanged
	{
		private readonly List<ImageItem> _list;
		private readonly bool _multiThreaded;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="multiThreaded">Whether Add, Remove, etc calls will be on a thread other than the UI thread.</param>
		public ObservableImageCollection(bool multiThreaded)
		{
			_multiThreaded = multiThreaded;
			_list = new List<ImageItem>();
		}

		public IEnumerator<ImageItem> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Adds an item but does not raise the CollectionChanged event.
		/// </summary>
		/// <param name="item"></param>
		public void Add(ImageItem item)
		{
			_list.Add(item);
		}

		/// <summary>
		/// Adds an item, optionally raising the CollectionChanged event.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="raiseCollectionChanged"></param>
		public void Add(ImageItem item, bool raiseCollectionChanged)
		{
			_list.Add(item);
			if (raiseCollectionChanged == false)
				return;

			RaiseCollectionChanged(NotifyCollectionChangedAction.Reset, item, -1);
		}

		public void Clear()
		{
			_list.Clear();
			OnCollectionChanged(NotifyCollectionChangedAction.Reset, null);
		}

		public bool Contains(ImageItem item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(ImageItem[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public bool Remove(ImageItem item)
		{
			var index = _list.IndexOf(item);
			if (index < 0)
				return false;

			_list.RemoveAt(index);
			RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
			return true;
		}

		public int Count
		{
			get { return _list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public int IndexOf(ImageItem item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, ImageItem item)
		{
			_list.Insert(index, item);
			OnCollectionChanged(NotifyCollectionChangedAction.Replace, item);
		}

		public void RemoveAt(int index)
		{
			var item = _list[index];
			_list.RemoveAt(index);
			RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
		}

		public ImageItem this[int index]
		{
			get { return _list[index]; }
			set
			{
				_list[index] = value;
				OnCollectionChanged(NotifyCollectionChangedAction.Replace, value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="item"></param>
		/// <param name="index"></param>
		void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
		{
			if (_multiThreaded)
			{
				var app = Application.Current;
				if (app == null)
					return;

				app.Dispatcher.Invoke(() =>
				{
					OnCollectionChanged(action, item, index, true);
				});
			}
			else
			{
				OnCollectionChanged(action, item, index, true);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Raises the CollectionChanged event when a large amount of items have been added.
		/// </summary>
		public void OnAddedRange()
		{
			RaiseCollectionChanged(NotifyCollectionChangedAction.Reset, null, -1);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="item"></param>
		/// <param name="index"></param>
		/// <param name="countChanged"></param>
		void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index = -1, bool countChanged = false)
		{
			var handler = CollectionChanged;
			if (handler == null)
				return;

			NotifyCollectionChangedEventArgs args;
			switch (action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
					args = new NotifyCollectionChangedEventArgs(action, item, index);
					break;
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Move:
					args = new NotifyCollectionChangedEventArgs(action, item);
					break;
				case NotifyCollectionChangedAction.Reset:
					args = new NotifyCollectionChangedEventArgs(action);
					break;
				default:
					throw new ArgumentOutOfRangeException("action", action, null);
			}

			handler.Invoke(this, args);

			if (countChanged)
				RaisePropertyChanged("Count", false);
		}
	}
}

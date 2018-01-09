using System;
using ImageOrganizer.Presentation;

namespace ImageOrganizer.Organization
{
	public class Tag : ObservableObject
	{
		private Command _removeCommand;
		private Command _addToImageCommand;

		public Tag(string name)
		{
			Name = name;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public Command RemoveCommand
		{
			get { return _removeCommand ?? (_removeCommand = new Command(Remove)); }
		}

		/// <summary>
		/// 
		/// </summary>
		void Remove()
		{
			var handler = RemoveRequested;
			if (handler != null)
				handler.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// 
		/// </summary>
		public event EventHandler RemoveRequested;

		/// <summary>
		/// 
		/// </summary>
		public Command AddToImageCommand
		{
			get { return _addToImageCommand ?? (_addToImageCommand = new Command(AddToImage)); }
		}

		/// <summary>
		/// 
		/// </summary>
		void AddToImage()
		{
			var handler = AddToImageRequested;
			if (handler != null)
				handler.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// 
		/// </summary>
		public event EventHandler AddToImageRequested;
	}
}

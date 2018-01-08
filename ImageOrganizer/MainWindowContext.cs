using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ImageOrganizer.Organization;
using ImageOrganizer.Presentation;
using ImageOrganizer.Presentation.SelectFolder;

namespace ImageOrganizer
{
	public class MainWindowContext : ObservableObject
	{
		private Command _browseCommand;
		private Command _leftCommand;
		private Command _rightCommand;
		private Command _newTagCommand;

		private string _folderPath;
		private List<string> _files;
		private readonly ObservableCollection<Tag> _tags;
		private int _currentIndex;
		private string _newTagName;
		private readonly Dictionary<string, List<string>> _imageTags;

		public MainWindowContext()
		{
			_files = new List<string>();
			_tags = new ObservableCollection<Tag>();
			_imageTags = new Dictionary<string, List<string>>();

			var window = Application.Current.MainWindow;
			if (window == null)
				return;

			window.Closed += MainWindowOnClosed;
			window.Loaded += MainWindowOnLoaded;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="routedEventArgs"></param>
		private static void MainWindowOnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var window = (Window) sender;
			window.Loaded -= MainWindowOnLoaded;

			window.Left = Settings.Default.WindowLeft;
			window.Top = Settings.Default.WindowTop;
			window.WindowState = Settings.Default.WindowState;
			window.Width = Settings.Default.WindowWidth;
			window.Height = Settings.Default.WindowHeight;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="cancelEventArgs"></param>
		private static void MainWindowOnClosed(object sender, EventArgs cancelEventArgs)
		{
			var window = (Window)sender;
			window.Closed -= MainWindowOnClosed;
			Settings.Default.WindowLeft = window.Left;
			Settings.Default.WindowTop = window.Top;
			Settings.Default.WindowState = window.WindowState;
			Settings.Default.WindowWidth = window.Width;
			Settings.Default.WindowHeight = window.Height;
			Settings.Default.SaveSettings();
		}

		public string FolderPath
		{
			get { return _folderPath; }
			set { Set("FolderPath", ref _folderPath, value); }
		}

		public Command BrowseCommand
		{
			get { return _browseCommand ?? (_browseCommand = new Command(BrowseForFolder)); }
		}

		public Command LeftCommand
		{
			get { return _leftCommand ?? (_leftCommand = new Command(PreviousImage, CanGoBack)); }
		}

		public Command RightCommand
		{
			get { return _rightCommand ?? (_rightCommand = new Command(NextImage, CanMoveNext)); }
		}

		public Command NewTagCommand
		{
			get { return _newTagCommand ?? (_newTagCommand = new Command(MakeNewTag, CanMakeNewTag)); }
		}

		public List<string> Files
		{
			get { return _files; }
			set { Set("Files", ref _files, value); }
		}

		public ObservableCollection<Tag> Tags
		{
			get { return _tags; }
		}

		public Image CurrentImage
		{
			get { return new Image(_files[_currentIndex]); }
		}

		public int CurrentIndex
		{
			get { return _currentIndex; }
			set
			{
				Set("CurrentIndex", ref _currentIndex, value);
				RaiseCanChangeImageChanged();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string NewTagName
		{
			get { return _newTagName; }
			set
			{
				Set("NewTagName", ref _newTagName, value);
				RaisePropertyChanged("NewTagCommand");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		void BrowseForFolder()
		{
			var dlg = new FolderSelectDialog();
			if (dlg.ShowDialog() == false)
				return;

			Files = new DirectoryInfo(dlg.FolderName).EnumerateFiles("*", SearchOption.TopDirectoryOnly)
				.Where(p => Regex.IsMatch(p.Extension, ".jpg|.jpeg|.png"))
				.Select(p => p.FullName).ToList();

			FolderPath = dlg.FolderName;
			_currentIndex = 0;
			RaiseCanChangeImageChanged();
		}

		void RaiseCanChangeImageChanged()
		{
			RaisePropertyChanged("LeftCommand");
			RaisePropertyChanged("RightCommand");
			RaisePropertyChanged("CurrentImage");
		}

		bool CanGoBack()
		{
			return _currentIndex > 0;
		}

		void NextImage()
		{
			CurrentIndex++;
			RaiseCanChangeImageChanged();
		}

		bool CanMoveNext()
		{
			return _currentIndex < _files.Count - 1;
		}

		void PreviousImage()
		{
			CurrentIndex--;
			RaiseCanChangeImageChanged();
		}

		bool CanMakeNewTag()
		{
			return string.IsNullOrWhiteSpace(_newTagName) == false;
		}

		void MakeNewTag()
		{
			var tag = new Tag(_newTagName);

			tag.RemoveRequested += TagOnRemoveRequested;
			Tags.Add(tag);
			NewTagName = null;

			if (_files.Any() == false)
				return;

			if (_imageTags.ContainsKey(_files[_currentIndex]) == false)
				_imageTags[_files[_currentIndex]] = new List<string>{_newTagName};
			else if (_imageTags[_files[_currentIndex]].Contains(_newTagName) == false)
				_imageTags[_files[_currentIndex]].Add(_newTagName);
		}

		private void TagOnRemoveRequested(object sender, EventArgs eventArgs)
		{
			var tag = (Tag) sender;
			Tags.Remove(tag);
			tag.RemoveRequested -= TagOnRemoveRequested;
		}
	}
}

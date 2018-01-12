using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Serialization;
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

		private Dictionary<string, List<string>> _imageTags;
		private readonly ObservableCollection<Tag> _tags;
		private string _folderPath;
		private List<ImageItem> _files;
		private int _currentIndex;
		private string _newTagName;
		private string _tagSearch;

		public MainWindowContext()
		{
			_files = new List<ImageItem>();
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
		private void MainWindowOnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var window = (Window) sender;
			window.Loaded -= MainWindowOnLoaded;

			window.Left = Settings.Default.WindowLeft;
			window.Top = Settings.Default.WindowTop;
			window.WindowState = Settings.Default.WindowState;
			window.Width = Settings.Default.WindowWidth;
			window.Height = Settings.Default.WindowHeight;
			LoadTags();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="cancelEventArgs"></param>
		private void MainWindowOnClosed(object sender, EventArgs cancelEventArgs)
		{
			var window = (Window)sender;
			window.Closed -= MainWindowOnClosed;
			Settings.Default.WindowLeft = window.Left;
			Settings.Default.WindowTop = window.Top;
			Settings.Default.WindowState = window.WindowState;
			Settings.Default.WindowWidth = window.Width;
			Settings.Default.WindowHeight = window.Height;
			Settings.Default.SaveSettings();
			SaveTags();
		}

		/// <summary>
		/// 
		/// </summary>
		public string TagSearch
		{
			get { return _tagSearch; }
			set
			{
				Set("TagSearch", ref _tagSearch, value);
				RaisePropertyChanged("Tags");
			}
		}

		/// <summary>
		/// 
		/// </summary>
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

		public List<ImageItem> Files
		{
			get { return _files; }
			set { Set("Files", ref _files, value); }
		}

		public ObservableCollection<Tag> Tags
		{
			get
			{
				return string.IsNullOrWhiteSpace(_tagSearch) ? 
					_tags :
					new ObservableCollection<Tag>(_tags.Where(t => t.Name.Contains(_tagSearch)));
			}
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

			Files = new DirectoryInfo(dlg.FolderName)
				.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
				.Where(p => Regex.IsMatch(p.Extension, ".jpg|.jpeg|.png", RegexOptions.IgnoreCase))
				.AsParallel()
				.Select(p => new ImageItem(p.FullName)).ToList();

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

			RegisterTagEvents(tag, true);
			_tags.Add(tag);
			RaisePropertyChanged("Tags");
			NewTagName = null;

			if (_files.Any() == false)
				return;

			AddTagToImage(_newTagName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		void AddTagToImage(string name)
		{
			if (_imageTags.ContainsKey(_files[_currentIndex].FilePath) == false)
				_imageTags[_files[_currentIndex].FilePath] = new List<string> { name };
			else if (_imageTags[_files[_currentIndex].FilePath].Contains(name) == false)
				_imageTags[_files[_currentIndex].FilePath].Add(name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void TagOnRemoveRequested(object sender, EventArgs eventArgs)
		{
			var tag = (Tag) sender;
			_tags.Remove(tag);
			RaisePropertyChanged("Tags");
			RegisterTagEvents(tag, false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		void TagOnAddToImageRequested(object sender, EventArgs eventArgs)
		{
			var tag = (Tag)sender;
			AddTagToImage(tag.Name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="register"></param>
		void RegisterTagEvents(Tag tag, bool register)
		{
			if (register)
			{
				tag.RemoveRequested += TagOnRemoveRequested;
				tag.AddToImageRequested += TagOnAddToImageRequested;
			}
			else
			{
				tag.RemoveRequested -= TagOnRemoveRequested;
				tag.AddToImageRequested -= TagOnAddToImageRequested;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		void SaveTags()
		{
			using (var fs = new FileStream(Settings.TagsFilePath, FileMode.OpenOrCreate))
			{
				var serializer = new XmlSerializer(_imageTags.GetType());
				serializer.Serialize(fs, _imageTags);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		void LoadTags()
		{
			var path = Settings.TagsFilePath;
			if (File.Exists(path) == false)
				return;

			using (var fs = new FileStream(path, FileMode.Open))
			{
				var serializer = new XmlSerializer(_imageTags.GetType());
				_imageTags = (Dictionary<string, List<string>>) serializer.Deserialize(fs);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using ImageOrganizer.Organization;
using ImageOrganizer.Presentation;
using ImageOrganizer.Presentation.SelectFolder;

namespace ImageOrganizer
{
	/// <summary>
	/// 
	/// </summary>
	public class MainWindowContext : ObservableObject, IImageHost
	{
		private const int MaxImagesPerCreation = 50;

		private Command _browseCommand;
		private Command _newTagCommand;
		private Command _testCrashCommand;
		private Command<double> _handleScrollChangedCommand;

		private Dictionary<string, List<string>> _imageTags;
		private readonly ObservableImageCollection _files;
		private readonly ObservableCollection<Tag> _tags;
		private string _folderPath;
		private string _newTagName;
		private string _tagSearch;
		private ImageItem _selectedImage;
		private ImageSource _selectedImageSource;

		private IEnumerable<FileInfo> _allFiles;
		private IEnumerator<FileInfo> _enumerator;
		private readonly object _imageCreationLock = new object();
		private Thread _imageGenerationThread;
		private int _imagesCreatedSinceLastEvent;
		private bool _isCreatingImages;

		public MainWindowContext()
		{
			_files = new ObservableImageCollection(true);
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

		public Command NewTagCommand
		{
			get { return _newTagCommand ?? (_newTagCommand = new Command(MakeNewTag, CanMakeNewTag)); }
		}

		public Command TestCrashCommand
		{
			get { return _testCrashCommand ?? (_testCrashCommand = new Command(TestCrash)); }
		}

		public Command<double> HandleScrollChangedCommand
		{
			get { return _handleScrollChangedCommand ?? (_handleScrollChangedCommand = new Command<double>(HandleScrollChanged)); }
		}

		private static void TestCrash()
		{
			throw new Exception("This is a test of the crash handling functionality.");
		}

		public ObservableImageCollection Files
		{
			get
			{
				return _files;
			}
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
		public ImageItem SelectedImage
		{
			get { return _selectedImage; }
			set { Set("SelectedImage", ref _selectedImage, value); }
		}

		/// <summary>
		/// 
		/// </summary>
		public ImageSource SelectedImageSource
		{
			get { return _selectedImageSource; }
			set
			{
				Set("SelectedImageSource", ref _selectedImageSource, value);
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

			_imageGenerationThread = new Thread(CreateImageItems);
			_imageGenerationThread.Start(dlg.FolderName);

			FolderPath = dlg.FolderName;
			RaiseCanChangeImageChanged();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		void CreateImageItems(object state)
		{
			_isCreatingImages = true;

			var path = (string) state;
			if (string.IsNullOrEmpty(path) == false)
			{
				_allFiles = new DirectoryInfo(path)
					.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
					.Where(p => p != null && Regex.IsMatch(p.Extension, ".jpg|.jpeg|.png", RegexOptions.IgnoreCase));

				_enumerator = _allFiles.GetEnumerator();
			}

			CancellationTokenSource cts = new CancellationTokenSource();
			int count = 0;
			var waitHandles = new WaitHandle[MaxImagesPerCreation];

			while (cts.IsCancellationRequested == false && count < MaxImagesPerCreation)
			{
				if (_enumerator.MoveNext() == false)
				{
					cts.Cancel();
					break;
				}

				var info = _enumerator.Current;
				if (info == null)
					continue;

				var waitHandle = new ManualResetEvent(false);
				waitHandles[count] = waitHandle;
				var helper = new ImageCreationHelper(info.FullName, CreateImageItem, waitHandle);
				ThreadPool.QueueUserWorkItem(helper.Callback, helper);

				count++;
			}

			WaitHandle.WaitAll(waitHandles);

			//raise the collection changed on any remaining files that have been created.
			Files.OnAddedRange();

			//if we cancelled it means there are no more files left.
			if (cts.IsCancellationRequested)
			{
				_enumerator.Dispose();
				_enumerator = null;
			}

			_isCreatingImages = false;
		}

		/// <summary>
		/// Creates an image item and adds it to the collection of files.
		/// This is designed to run on a background thread.
		/// </summary>
		/// <param name="state"></param>
		void CreateImageItem(object state)
		{
			var helper = (ImageCreationHelper)state;

			lock (_imageCreationLock)
			{
				_imagesCreatedSinceLastEvent++;
				var raise = _imagesCreatedSinceLastEvent >= 10;
				if (raise)
					_imagesCreatedSinceLastEvent = 0;

				Files.Add(new ImageItem(helper.FilePath, this), raise);
			}

			//flag this worker as finished.
			helper.Handle.Set();
		}

		void HandleScrollChanged(double verticalOffset)
		{
			//TODO determine if we're past prev max offset, if so create more images.  if not do nothing because we have them cached.
			//TODO image items should have placeholder thumbnail or color before thumbnail has been generated.
			//TODO need list of folders accessed (save this) in left pane (clicking will load all images).
			//TODO save out grid splitter configurations.

			if (_isCreatingImages || _enumerator == null)
				return;

			if (verticalOffset < 0.0)
				return;

			CreateImageItems("");
		}

		/// <summary>
		/// 
		/// </summary>
		void RaiseCanChangeImageChanged()
		{
			RaisePropertyChanged("LeftCommand");
			RaisePropertyChanged("RightCommand");
			RaisePropertyChanged("CurrentImage");
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

			if (Files.Any() == false)
				return;

			AddTagToImage(_newTagName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		void AddTagToImage(string name)
		{
			if (_imageTags.ContainsKey(_selectedImage.FilePath) == false)
				_imageTags[_selectedImage.FilePath] = new List<string> { name };
			else if (_imageTags[_selectedImage.FilePath].Contains(name) == false)
				_imageTags[_selectedImage.FilePath].Add(name);
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public void SelectImage(ImageItem item)
		{
			SelectedImage = item;
			SelectedImageSource = new BitmapImage(new Uri(item.FilePath));
		}
	}
}

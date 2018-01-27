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
using ImageOrganizer.Organization;
using ImageOrganizer.Presentation;
using ImageOrganizer.Presentation.SelectFolder;
using ImageOrganizer.Utilities;

namespace ImageOrganizer
{
	/// <summary>
	/// 
	/// </summary>
	public class MainWindowContext : ObservableObject, IImageHost
	{
		private const int ImagesPerInitialCreation = 50;
		private const int ImagesPerSubsequentCreation = 10;

		private Command _browseCommand;
		private Command _newTagCommand;
		private Command _testCrashCommand;
		private Command<double> _handleScrollChangedCommand;

		//dictionary of tag names with lists of filepaths per tag
		private readonly Dictionary<string, List<Tag>> _imageTags = new Dictionary<string, List<Tag>>();

		//global tag collection
		private readonly ObservableCollection<Tag> _allTags = new ObservableCollection<Tag>();
		private Tag _selectedTag;
		private ObservableCollection<Tag> _currentTags;

		private readonly ObservableImageCollection _files;
		private readonly ObservableCollection<string> _folders = new ObservableCollection<string>();
		private string _folderPath;
		private string _newTagName;
		private string _tagSearch;
		private ImageItem _selectedImage;
		private ImageSource _selectedImageSource;
		private List<ImageItem> _selectedItems = new List<ImageItem>();

		private double _maxVerticalOffset;

		private IEnumerable<FileInfo> _allFiles;
		private IEnumerator<FileInfo> _enumerator;
		private readonly object _imageCreationLock = new object();
		private Thread _imageGenerationThread;
		private int _imagesCreatedSinceLastEvent;
		private bool _isCreatingImages;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="window"></param>
		public MainWindowContext(Window window)
		{
			_files = new ObservableImageCollection(true);

			window.Closed += MainWindowOnClosed;

			IOUtilities.LoadObject(Settings.TagsFilePath, ref _allTags);
			IOUtilities.LoadObject(Settings.ImagesFilePath, ref _imageTags);
			IOUtilities.LoadObject(Settings.DataFilePath, ref _folders);
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

			IOUtilities.SaveObject(Settings.TagsFilePath, _allTags);
			IOUtilities.SaveObject(Settings.ImagesFilePath, _imageTags);
			IOUtilities.SaveObject(Settings.DataFilePath, _folders);
		}

		/// <summary>
		/// 
		/// </summary>
		public Tag SelectedTag
		{
			get { return _selectedTag; }
			set
			{
				Set("SelectedTag", ref _selectedTag, value);
				AddTagToImage(value);
			}
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
				RaisePropertyChanged("CurrentTags");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string FolderPath
		{
			get { return _folderPath; }
			set
			{
				if (_folderPath == value)
					return;

				Set("FolderPath", ref _folderPath, value);
				if (_folders.Contains(value) == false)
					Folders.Add(value);

				ResetImageItems();
				BeginCreatingImageItems(value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<string> Folders
		{
			get { return _folders; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Command BrowseCommand
		{
			get { return _browseCommand ?? (_browseCommand = new Command(BrowseForFolder)); }
		}

		/// <summary>
		/// 
		/// </summary>
		public Command NewTagCommand
		{
			get { return _newTagCommand ?? (_newTagCommand = new Command(MakeNewTag, CanMakeNewTag)); }
		}
		
		/// <summary>
		/// 
		/// </summary>
		public Command TestCrashCommand
		{
			get { return _testCrashCommand ?? (_testCrashCommand = new Command(TestCrash)); }
		}

		/// <summary>
		/// 
		/// </summary>
		public Command<double> HandleScrollChangedCommand
		{
			get { return _handleScrollChangedCommand ?? (_handleScrollChangedCommand = new Command<double>(HandleScrollChanged)); }
		}

		/// <summary>
		/// 
		/// </summary>
		private static void TestCrash()
		{
			throw new Exception("This is a test of the crash handling functionality.");
		}

		/// <summary>
		/// 
		/// </summary>
		public ObservableImageCollection Files
		{
			get
			{
				return _files;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<Tag> CurrentTags
		{
			get
			{
				return _currentTags;
				//return string.IsNullOrWhiteSpace(_tagSearch) ?
				////todo enable filtering
				//if (_imageTags.ContainsKey(_selectedImage.FilePath) == false)
				//	_imageTags.Add(_selectedImage.FilePath, new List<Tag>());

				//return _imageTags[_selectedImage.FilePath];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<Tag> AllTags
		{
			get { return _allTags; }
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
		public List<ImageItem> SelectedImages
		{
			get { return _selectedItems; }
			set { Set("SelectedImages", ref _selectedItems, value); }
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

			FolderPath = dlg.FolderName;
		}

		/// <summary>
		/// 
		/// </summary>
		void ResetImageItems()
		{
			_files.Clear();
			CleanUpEnumerator();
			_allFiles = null;
			_maxVerticalOffset = 0;
			_imagesCreatedSinceLastEvent = 0;
			SelectedImageSource = null;
			SelectedImage = null;
		}

		/// <summary>
		/// Begins the image creation process on another thread.
		/// </summary>
		/// <param name="folderPath"></param>
		void BeginCreatingImageItems(string folderPath)
		{
			_imageGenerationThread = new Thread(CreateImageItems);
			_imageGenerationThread.Start(folderPath);
		}

		/// <summary>
		/// Creates a set of image items using a thread pool.
		/// </summary>
		/// <param name="state"></param>
		void CreateImageItems(object state)
		{
			_isCreatingImages = true;

			var path = (string) state;
			int imageCount = ImagesPerSubsequentCreation;
			if (string.IsNullOrEmpty(path) == false)
			{
				_allFiles = new DirectoryInfo(path)
					.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
					.Where(p => p != null && Regex.IsMatch(p.Extension, ".jpg|.jpeg|.png", RegexOptions.IgnoreCase));

				_enumerator = _allFiles.GetEnumerator();
				imageCount = ImagesPerInitialCreation;
			}

			CancellationTokenSource cts = new CancellationTokenSource();
			int count = 0;
			var waitHandles = new List<WaitHandle>(imageCount);

			while (cts.IsCancellationRequested == false && count < imageCount)
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
				waitHandles.Add(waitHandle);
				var helper = new ImageCreationHelper(info.FullName, CreateImageItem, waitHandle);
				ThreadPool.QueueUserWorkItem(helper.Callback, helper);

				count++;
			}

			if (cts.IsCancellationRequested)
				CleanUpEnumerator();

			var handles = waitHandles.Where(h => h != null).ToArray();
			if (handles.Any() == false)
				return;

			WaitHandle.WaitAll(handles);

			//raise the collection changed on any remaining files that have been created.
			Files.OnAddedRange();

			//if we cancelled it means there are no more files left.
			if (cts.IsCancellationRequested)
				CleanUpEnumerator();

			_isCreatingImages = false;
		}

		/// <summary>
		/// 
		/// </summary>
		void CleanUpEnumerator()
		{
			if (_enumerator == null)
				return;

			_enumerator.Dispose();
			_enumerator = null;
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="verticalOffset"></param>
		void HandleScrollChanged(double verticalOffset)
		{
			//TODO image items should have placeholder thumbnail or color before thumbnail has been generated.

			if (_isCreatingImages || _enumerator == null)
				return;

			if (verticalOffset < 0.0 || verticalOffset < _maxVerticalOffset)
				return;

			_maxVerticalOffset = verticalOffset;

			BeginCreatingImageItems("");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <param name="select"></param>
		public void ToggleImageSelection(ImageItem item, bool select)
		{
			//save the old image's tags.
			if (_currentTags != null && _selectedImage != null)
				_imageTags[_selectedImage.FilePath] = _currentTags.ToList();

			if (select)
			{
				_selectedItems.Add(item);
				SelectedImage = item;
			}
			else
			{
				_selectedItems.Remove(item);
				if (_selectedItems.Any())
				{
					SelectedImage =  _selectedItems.Last();
				}
				else
				{
					SelectedImage = null;
					SelectedImageSource = null;
				}
			}

			if (_selectedImage != null)
			{
				SelectedImageSource = new BitmapImage(new Uri(_selectedImage.FilePath));
				if (_imageTags.ContainsKey(_selectedImage.FilePath) == false)
					_imageTags[_selectedImage.FilePath] = new List<Tag>();

				_currentTags = new ObservableCollection<Tag>(_imageTags[_selectedImage.FilePath]);
				foreach (var tag in _currentTags)
					RegisterTagEvents(tag, true);
			}
			else
				_currentTags = null;

			RaisePropertyChanged("CurrentTags");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool CanMakeNewTag()
		{
			return string.IsNullOrWhiteSpace(_newTagName) == false;
		}

		/// <summary>
		/// Creates a new tag.
		/// </summary>
		void MakeNewTag()
		{
			if (_allTags.Any(t => t.Name == _newTagName))
				return;

			var tag = new Tag(_newTagName);
			_allTags.Add(tag);

			RegisterTagEvents(tag, true);
			RaisePropertyChanged("AllTags");
			NewTagName = null;

			//todo we may want to add the new tag to the current image later
			//if (_selectedImage == null)
			//	return;

			//AddTagToImage(_newTagName);
			//RaisePropertyChanged("CurrentTags");
		}

		/// <summary>
		/// Adds a tag to the current image.
		/// </summary>
		/// <param name="tag"></param>
		void AddTagToImage(Tag tag)
		{
			if (CurrentTags.Contains(tag))
				return;

			CurrentTags.Add(tag);
			foreach (var image in _selectedItems)
			{
				if (image == SelectedImage)
					continue;

				_imageTags[image.FilePath].Add(tag);
			}

			RegisterTagEvents(tag, true);
		}

		/// <summary>
		/// Removes the tag from its image.  The tag will still remain in the main collection of tags.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void Tag_RemoveRequested(object sender, EventArgs eventArgs)
		{
			var tag = (Tag) sender;
			CurrentTags.Remove(tag);

			RegisterTagEvents(tag, false);
		}

		/// <summary>
		/// Adds the tag to the current image.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void Tag_AddToImageRequested(object sender, string args)
		{
			if (_selectedImage == null)
				return;

			AddTagToImage((Tag)sender);
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
				tag.RemoveRequested += Tag_RemoveRequested;
				tag.AddToImageRequested += Tag_AddToImageRequested;
			}
			else
			{
				tag.RemoveRequested -= Tag_RemoveRequested;
				tag.AddToImageRequested -= Tag_AddToImageRequested;
			}
		}
	}
}

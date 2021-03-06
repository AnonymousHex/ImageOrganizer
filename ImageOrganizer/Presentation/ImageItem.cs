﻿using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageOrganizer.Organization;

namespace ImageOrganizer.Presentation
{
	/// <summary>
	/// 
	/// </summary>
	public class ImageItem : ObservableObject
	{
		private readonly IImageHost _host;
		private ImageSource _thumb;
		private readonly string _filePath;
		private bool _isSelected;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="host"></param>
		public ImageItem(string filePath, IImageHost host)
		{
			_host = host;
			_filePath = filePath;
			CreateThumbnail(_filePath);
		}

		/// <summary>
		/// Creates a thumbnail from an image file path.
		/// </summary>
		/// <param name="path"></param>
		void CreateThumbnail(string path)
		{
			FileStream stream = null;
			try
			{
				stream = new FileStream(path, FileMode.Open);
				var frame =
					BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.None)
						.Frames[0];

				BitmapSource thumbnail = frame.Thumbnail;
				if (thumbnail != null)
				{
					_thumb = thumbnail;
					return;
				}

				var transformedBitmap = new TransformedBitmap();
				transformedBitmap.BeginInit();
				transformedBitmap.Source = frame;

				int pixelH = frame.PixelHeight;
				int pixelW = frame.PixelWidth;

				int decodeH = 100;
				int decodeW = frame.PixelWidth * decodeH / pixelH;

				double scaleX = decodeW / (double) pixelW;
				double scaleY = decodeH / (double) pixelH;

				TransformGroup transformGroup = new TransformGroup();

				transformGroup.Children.Add(new ScaleTransform(scaleX, scaleY));
				transformedBitmap.Transform = transformGroup;
				transformedBitmap.EndInit();

				WriteableBitmap writable = new WriteableBitmap(transformedBitmap);
				writable.Freeze();

				_thumb = writable;
			}
			catch (IOException ex)
			{
				//TODO generate default image
			}
			finally
			{
				if (stream != null)
					stream.Dispose();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				Set("IsSelected", ref _isSelected, value);
				_host.ToggleImageSelection(this, value);
			}
		}

		/// <summary>
		/// The image's thumbnail.
		/// </summary>
		public ImageSource ThumbNail
		{
			get { return _thumb; }
		}

		/// <summary>
		/// The images file name.
		/// </summary>
		public string FileName
		{
			get { return Path.GetFileName(_filePath); }
		}

		/// <summary>
		/// The full path to the image.
		/// </summary>
		public string FilePath
		{
			get { return _filePath; }
		}
	}
}

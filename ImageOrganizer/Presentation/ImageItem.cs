﻿using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageOrganizer.Presentation
{
	/// <summary>
	/// 
	/// </summary>
	public class ImageItem : ObservableObject
	{
		private ImageSource _thumb;
		private readonly string _filePath;

		public ImageItem(string filePath)
		{
			_filePath = filePath;
			CreateThumbnail(_filePath);
		}

		/// <summary>
		/// Creates a thumbnail from an image file path.
		/// </summary>
		/// <param name="path"></param>
		void CreateThumbnail(string path)
		{
			var frame = 
				BitmapDecoder.Create(new FileStream(path, FileMode.Open), BitmapCreateOptions.None, BitmapCacheOption.None)
				.Frames[0];

			var thumbnail = frame.Thumbnail;
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

			double scaleX = decodeW / (double)pixelW;
			double scaleY = decodeH / (double)pixelH;

			TransformGroup transformGroup = new TransformGroup();

			transformGroup.Children.Add(new ScaleTransform(scaleX, scaleY));
			transformedBitmap.Transform = transformGroup;
			transformedBitmap.EndInit();

			WriteableBitmap writable = new WriteableBitmap(transformedBitmap);
			writable.Freeze();

			_thumb = writable;
		}

		/// <summary>
		/// 
		/// </summary>
		public ImageSource ThumbNail
		{
			get { return _thumb; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string FileName
		{
			get { return Path.GetFileName(_filePath); }
		}

		/// <summary>
		/// 
		/// </summary>
		public string FilePath
		{
			get { return _filePath; }
		}
	}
}

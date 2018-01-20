using System.Collections.Generic;
using System.IO;
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
		private readonly List<string> _tags = new List<string>();
		private Command _selectCommand;

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
		public Command SelectCommand
		{
			get { return _selectCommand ?? (_selectCommand = new Command(Select)); }
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

		/// <summary>
		/// 
		/// </summary>
		void Select()
		{
			_host.SelectImage(this);
		}
	}
}

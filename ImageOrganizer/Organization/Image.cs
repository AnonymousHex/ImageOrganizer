using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageOrganizer.Presentation;

namespace ImageOrganizer.Organization
{
	/// <summary>
	/// 
	/// </summary>
	public class Image : ObservableObject
	{
		private readonly string _path;
		private readonly List<string> _tags;
		private ImageSource _image;

		public Image(string fullPath, List<string> tags = null)
		{
			_path = fullPath;
			_tags = tags ?? new List<string>();
		}

		public string FullPath
		{
			get { return _path; }
		}

		public string FileName
		{
			get { return Path.GetFileName(_path); }
		}

		public List<string> Tags
		{
			get { return _tags; }
		}

		public ImageSource Source
		{	
			get { return _image ?? (_image = MakeImage(_path)); }
		}

		private static ImageSource MakeImage(string filePath)
		{
			return new BitmapImage(new Uri(filePath));
		}
	}
}

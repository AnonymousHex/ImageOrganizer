using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using ImageOrganizer.Presentation;

namespace ImageOrganizer
{
	[DataContract]
	public class Settings : ObservableObject
	{
		private const string SettingsFolderName = "ImageOrganizer";
		private const string TagsFileName = "tags";
		private const string SettingsFileName = "settings";


		/// <summary>
		/// The full path to the settings file.
		/// </summary>
		private static string SettingsPath
		{
			get
			{
				if (string.IsNullOrEmpty(SettingsFolderPath))
				{
					var dataFolder = Environment.GetFolderPath(
						Environment.SpecialFolder.CommonApplicationData,
						Environment.SpecialFolderOption.Create);

					SettingsFolderPath = Path.Combine(dataFolder, SettingsFolderName);
					Directory.CreateDirectory(SettingsFolderPath);
				}

				return Path.Combine(SettingsFolderPath, SettingsFileName);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static string TagsFilePath
		{
			get { return Path.Combine(SettingsFolderPath, TagsFileName); }
		}

		/// <summary>
		/// 
		/// </summary>
		public static string SettingsFolderPath { get; private set; }

		private static Settings _default;
		
		/// <summary>
		/// 
		/// </summary>
		public static Settings Default
		{
			get { return _default ?? (_default = LoadSettings());}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static Settings LoadSettings()
		{
			var path = SettingsPath;
			if (File.Exists(path) == false)
				return new Settings();

			try
			{
				using (var fs = new FileStream(path, FileMode.Open))
				{
					var serializer = new XmlSerializer(typeof(Settings));
					return (Settings)serializer.Deserialize(new XmlTextReader(fs));
				}
			}
			catch
			{
				return new Settings();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void SaveSettings()
		{
			var path = SettingsPath;
			using (var fs = new FileStream(path, FileMode.Create))
			{
				var serializer = new XmlSerializer(typeof(Settings));
				serializer.Serialize(fs, this);
			}
		}

		[DataMember]
		private double _windowLeft = 100;

		public double WindowLeft
		{
			get { return _windowLeft; }
			set { _windowLeft = value; }
		}

		[DataMember]
		private double _windowTop = 100;

		public double WindowTop
		{
			get { return _windowTop; }
			set { _windowTop = value; }
		}

		[DataMember]
		private WindowState _windowState = WindowState.Normal;

		public WindowState WindowState
		{
			get { return _windowState; }
			set { _windowState = value; }
		}

		[DataMember]
		private double _windowWidth = 400;

		public double WindowWidth
		{
			get { return _windowWidth; }
			set { _windowWidth = value; }
		}

		[DataMember] 
		private double _windowHeight = 250;

		public double WindowHeight
		{
			get { return _windowHeight; }
			set { _windowHeight = value; }
		}
	}
}

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml;
using ImageOrganizer.Presentation;

namespace ImageOrganizer
{
	[DataContract]
	public class Settings : ObservableObject
	{
		private const string SettingsFolderName = "ImageOrganizer";
		private const string TagsFileName = "tags";
		private const string SettingsFileName = "settings";

		private static string _settingsFolderPath;

		/// <summary>
		/// The full path to the settings file.
		/// </summary>
		private static string SettingsPath
		{
			get { return Path.Combine(SettingsFolderPath, SettingsFileName); }
		}

		/// <summary>
		/// The full path to the tags database file.
		/// </summary>
		public static string TagsFilePath
		{
			get { return Path.Combine(SettingsFolderPath, TagsFileName); }
		}

		/// <summary>
		/// 
		/// </summary>
		public static string SettingsFolderPath
		{
			get
			{
				if (string.IsNullOrEmpty(_settingsFolderPath))
				{
					var dataFolder = Environment.GetFolderPath(
						Environment.SpecialFolder.CommonApplicationData,
						Environment.SpecialFolderOption.Create);

					_settingsFolderPath = Path.Combine(dataFolder, SettingsFolderName);
					Directory.CreateDirectory(_settingsFolderPath);
				}

				return _settingsFolderPath;
			}
		}

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
					var serializer = new DataContractSerializer(typeof(Settings));
					return (Settings)serializer.ReadObject(new XmlTextReader(fs));
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
				var serializer = new DataContractSerializer(typeof(Settings));
				serializer.WriteObject(fs, this);
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
		private double _windowWidth = 1000;

		public double WindowWidth
		{
			get { return _windowWidth; }
			set { _windowWidth = value; }
		}

		[DataMember] 
		private double _windowHeight = 750;

		public double WindowHeight
		{
			get { return _windowHeight; }
			set { _windowHeight = value; }
		}

		[DataMember] 
		private double _column1Width;

		public double Column1Width
		{
			get { return _column1Width; }
			set { _column1Width = value; }
		}

		[DataMember] 
		private double _column2Width;

		public double Column2Width
		{
			get { return _column2Width; }
			set { _column2Width = value; }
		}
	}
}

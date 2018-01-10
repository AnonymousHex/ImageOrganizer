using System;
using System.Windows;

namespace ImageOrganizer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			//set the theme
			Resources.MergedDictionaries.Add(new ResourceDictionary
			{
				Source = new Uri("pack://application:,,,/ImageOrganizer;component/Themes/Dark/Theme.xaml")
			});
		}
	}
}

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
			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
			InitializeComponent();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			MessageBox.Show(
				"An unhandled exception has occured and the program must exit.  Exception:\n\n" + 
				GetOriginalException((Exception)args.ExceptionObject),
				"Error", MessageBoxButton.OK, MessageBoxImage.Error);

			Current.Shutdown();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		static Exception GetOriginalException(Exception ex)
		{
			Exception inner = ex;
			while (ex.InnerException != null)
				inner = ex;

			return inner;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
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

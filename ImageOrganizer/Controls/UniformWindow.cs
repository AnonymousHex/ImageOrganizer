using System.Windows;

namespace ImageOrganizer.Controls
{
	/// <summary>
	/// 
	/// </summary>
	[TemplatePart(Name = CloseButtonName, Type = typeof(UniformWindowButton))]
	[TemplatePart(Name = MinimizeButtonName, Type = typeof(UniformWindowButton))]
	[TemplatePart(Name = RestoreButtonName, Type = typeof(UniformWindowButton))]
	public class UniformWindow : Window
	{
		private const string CloseButtonName = "PART_CloseButton";
		private const string MinimizeButtonName = "PART_MinimizeButton";
		private const string RestoreButtonName = "PART_RestoreButton";

		/// <summary>
		/// 
		/// </summary>
		static UniformWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(UniformWindow), new FrameworkPropertyMetadata(typeof(UniformWindow)));
		}

		/// <summary>
		/// 
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var button = GetTemplateChild(CloseButtonName) as UniformWindowButton;
			if (button != null)
				button.Click += CloseButtonOnClick;

			button = GetTemplateChild(RestoreButtonName) as UniformWindowButton;
			if (button != null)
				button.Click += RestoreButtonOnClick;

			button = GetTemplateChild(MinimizeButtonName) as UniformWindowButton;
			if (button != null)
				button.Click += MinimizeButtonOnClick;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="routedEventArgs"></param>
		private static void MinimizeButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
		{
			if (Application.Current.MainWindow != null)
				Application.Current.MainWindow.WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="routedEventArgs"></param>
		private static void RestoreButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
		{
			if (Application.Current.MainWindow != null)
				Application.Current.MainWindow.WindowState = Application.Current.MainWindow.WindowState == WindowState.Maximized ?
					WindowState.Normal :
					WindowState.Maximized;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="routedEventArgs"></param>
		private static void CloseButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
		{
			((UniformWindowButton) sender).Click -= CloseButtonOnClick;

			if (Application.Current.MainWindow != null) 
				Application.Current.MainWindow.Close();
		}
	}
}

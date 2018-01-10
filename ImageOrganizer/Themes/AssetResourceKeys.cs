using System.Windows;
using System.Windows.Media;

namespace ImageOrganizer.Themes
{
	/// <summary>
	/// 
	/// </summary>
	public class AssetResourceKeys
	{
		private static ResourceKey _windowBackgroundBrush;
		public static ResourceKey WindowBackgroundBrush
		{
			get { return _windowBackgroundBrush ?? (_windowBackgroundBrush = new ComponentResourceKey(typeof(SolidColorBrush), "WindowBackgroundBrush")); }
		}

		private static ResourceKey _windowBorderBrush;
		public static ResourceKey WindowBorderBrush
		{
			get { return _windowBorderBrush ?? (_windowBorderBrush = new ComponentResourceKey(typeof(SolidColorBrush), "WindowBorderBrush")); }
		}

		private static ResourceKey _windowTitleBarBrush;
		public static ResourceKey WindowTitleBarBrush
		{
			get { return _windowTitleBarBrush ?? (_windowTitleBarBrush = new ComponentResourceKey(typeof(SolidColorBrush), "WindowTitleBarBrush")); }
		}

		private static ResourceKey _windowActionBarBrush;
		public static ResourceKey WindowActionBarBrush
		{
			get { return _windowActionBarBrush ?? (_windowActionBarBrush = new ComponentResourceKey(typeof(SolidColorBrush), "WindowActionBarBrush")); }
		}

		private static ResourceKey _closeButtonBackgroundBrush;
		public static ResourceKey CloseButtonBackgroundBrush
		{
			get { return _closeButtonBackgroundBrush ?? (_closeButtonBackgroundBrush = new ComponentResourceKey(typeof(SolidColorBrush), "CloseButtonBackgroundColor")); }
		}

		private static ResourceKey _windowButtonBackgroundBrush;
		public static ResourceKey WindowButtonBackgroundBrush
		{
			get { return _windowButtonBackgroundBrush ?? (_windowButtonBackgroundBrush = new ComponentResourceKey(typeof(SolidColorBrush), "WindowButtonBackgroundBrush")); }
		}
	}
}

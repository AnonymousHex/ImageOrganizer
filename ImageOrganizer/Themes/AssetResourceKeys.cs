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

		private static ResourceKey _uniformForegroundBrush;
		public static ResourceKey UniformForegroundBrush
		{
			get { return _uniformForegroundBrush ?? (_uniformForegroundBrush = new ComponentResourceKey(typeof(SolidColorBrush), "UniformForegroundBrush")); }
		}

		private static ResourceKey _uniformDisabledForegroundBrush;
		public static ResourceKey UniformDisabledForegroundBrush
		{
			get { return _uniformDisabledForegroundBrush ?? (_uniformDisabledForegroundBrush = new ComponentResourceKey(typeof(SolidColorBrush), "UniformDisabledForegroundBrush")); }
		}

		private static ResourceKey _closeButtonPressedBackgroundBrush;
		public static ResourceKey CloseButtonPressedBackgroundBrush
		{
			get { return _closeButtonPressedBackgroundBrush ?? (_closeButtonPressedBackgroundBrush = new ComponentResourceKey(typeof(SolidColorBrush), "CloseButtonPressedBackgroundBrush")); }
		}

		private static ResourceKey _windowButtonPressedBackgroundBrush;
		public static ResourceKey WindowButtonPressedBackgroundBrush
		{
			get { return _windowButtonPressedBackgroundBrush ?? (_windowButtonPressedBackgroundBrush = new ComponentResourceKey(typeof(SolidColorBrush), "WindowButtonPressedBackgroundBrush")); }
		}

		private static ResourceKey _textBoxBackgroundBrush;
		public static ResourceKey TextBoxBackgroundBrush
		{
			get { return _textBoxBackgroundBrush ?? (_textBoxBackgroundBrush = new ComponentResourceKey(typeof(SolidColorBrush), "TextBoxBackgroundBrush")); }
		}

		private static ResourceKey _textBoxBorderBrush;
		public static ResourceKey TextBoxBorderBrush
		{
			get { return _textBoxBorderBrush ?? (_textBoxBorderBrush = new ComponentResourceKey(typeof(SolidColorBrush), "TextBoxBorderBrush")); }
		}

		private static ResourceKey _textBoxReadOnlyBorderBrush;
		public static ResourceKey TextBoxReadOnlyBorderBrush
		{
			get { return _textBoxReadOnlyBorderBrush ?? (_textBoxReadOnlyBorderBrush = new ComponentResourceKey(typeof(SolidColorBrush), "TextBoxReadOnlyBorderBrush")); }
		}

		private static ResourceKey _textBoxReadOnlyBackgroundBrush;
		public static ResourceKey TextBoxReadOnlyBackgroundBrush
		{
			get { return _textBoxReadOnlyBackgroundBrush ?? (_textBoxReadOnlyBackgroundBrush = new ComponentResourceKey(typeof(SolidColorBrush), "TextBoxReadOnlyBackgroundBrush")); }
		}

		private static ResourceKey _textBoxDisabledBorderBrush;
		public static ResourceKey TextBoxDisabledBorderBrush
		{
			get { return _textBoxDisabledBorderBrush ?? (_textBoxDisabledBorderBrush = new ComponentResourceKey(typeof(SolidColorBrush), "TextBoxDisabledBorderBrush")); }
		}

		private static ResourceKey _textBoxDisabledBackgroundBrush;
		public static ResourceKey TextBoxDisabledBackgroundBrush
		{
			get { return _textBoxDisabledBackgroundBrush ?? (_textBoxDisabledBackgroundBrush = new ComponentResourceKey(typeof(SolidColorBrush), "TextBoxDisabledBackgroundBrush")); }
		}

		private static ResourceKey _gridSplitterBackgroundBrush;
		public static ResourceKey GridSplitterBackgroundBrush
		{
			get { return _gridSplitterBackgroundBrush ?? (_gridSplitterBackgroundBrush = new ComponentResourceKey(typeof(SolidColorBrush), "GridSplitterBackgroundBrush")); }
		}
	}
}

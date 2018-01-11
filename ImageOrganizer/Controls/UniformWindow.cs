using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Uri = System.Uri;

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
		private const string DragGripName = "PART_DragGrip";

		private UniformWindowButton _closeButton;
		private UniformWindowButton _restoreButton;
		private UniformWindowButton _minimizeButton;
		private Grid _dragGrip;

		private static readonly Dictionary<WindowState, ImageSource> StateIcons = new Dictionary<WindowState, ImageSource>();

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

			_closeButton = GetTemplateChild(CloseButtonName) as UniformWindowButton;
			if (_closeButton != null)
				_closeButton.Click += CloseButtonOnClick;

			_restoreButton = GetTemplateChild(RestoreButtonName) as UniformWindowButton;
			if (_restoreButton != null)
				_restoreButton.Click += RestoreButtonOnClick;

			_minimizeButton = GetTemplateChild(MinimizeButtonName) as UniformWindowButton;
			if (_minimizeButton != null)
				_minimizeButton.Click += MinimizeButtonOnClick;

			_dragGrip = GetTemplateChild(DragGripName) as Grid;
			if (_dragGrip != null)
				_dragGrip.MouseLeftButtonDown += DragGripOnMouseLeftButtonDown;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="mouseButtonEventArgs"></param>
		private void DragGripOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			DragMove();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="routedEventArgs"></param>
		private void MinimizeButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
		{
			WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="routedEventArgs"></param>
		private void RestoreButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
		{
			Uri source;
			if (WindowState == WindowState.Maximized)
			{
				WindowState = WindowState.Normal;
				source = new Uri("/Presentation/Resources/Maximize.png", UriKind.Relative);
			}
			else
			{
				WindowState = WindowState.Maximized;
				source = new Uri("/Presentation/Resources/Restore.png", UriKind.Relative);
			}

			if (StateIcons.ContainsKey(WindowState) == false)
				StateIcons[WindowState] = new BitmapImage(source)
				{
					DecodePixelWidth = 13,
					DecodePixelHeight = 11
				};

			_restoreButton.Icon = StateIcons[WindowState];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="routedEventArgs"></param>
		private void CloseButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
		{
			Close();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosed(EventArgs e)
		{
			if (_closeButton != null)
				_closeButton.Click -= CloseButtonOnClick;

			if (_restoreButton != null)
				_restoreButton.Click -= RestoreButtonOnClick;

			if (_minimizeButton != null)
				_minimizeButton.Click -= MinimizeButtonOnClick;

			base.OnClosed(e);
		}
	}
}

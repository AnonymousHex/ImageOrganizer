using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageOrganizer.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class UniformIconButton : Button
	{
		static UniformIconButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(UniformIconButton), new FrameworkPropertyMetadata(typeof(UniformIconButton)));
		}

		public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register(
			"HoveredBackground", typeof(SolidColorBrush), typeof(UniformIconButton), new PropertyMetadata(default(SolidColorBrush)));

		public SolidColorBrush HoveredBackground
		{
			get { return (SolidColorBrush)GetValue(HoveredBackgroundProperty); }
			set { SetValue(HoveredBackgroundProperty, value); }
		}

		public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register(
			"PressedBackground", typeof(SolidColorBrush), typeof(UniformIconButton), new PropertyMetadata(default(SolidColorBrush)));

		public SolidColorBrush PressedBackground
		{
			get { return (SolidColorBrush)GetValue(PressedBackgroundProperty); }
			set { SetValue(PressedBackgroundProperty, value); }
		}

		public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
			"Icon", typeof(ImageSource), typeof(UniformIconButton), new PropertyMetadata(default(ImageSource)));

		public ImageSource Icon
		{
			get { return (ImageSource) GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}
	}
}

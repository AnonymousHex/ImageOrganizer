using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageOrganizer.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class UniformWindowButton : Button
	{
		static UniformWindowButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(UniformWindowButton), new FrameworkPropertyMetadata(typeof(UniformWindowButton)));
		}

		public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register(
			"HoveredBackground", typeof(SolidColorBrush), typeof(UniformWindowButton), new PropertyMetadata(default(SolidColorBrush)));

		public SolidColorBrush HoveredBackground
		{
			get { return (SolidColorBrush)GetValue(HoveredBackgroundProperty); }
			set { SetValue(HoveredBackgroundProperty, value); }
		}
	}
}

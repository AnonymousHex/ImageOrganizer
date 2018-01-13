using System.Windows;
using System.Windows.Controls;

namespace ImageOrganizer.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class UniformButton : Button
	{
		static UniformButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(UniformButton), new FrameworkPropertyMetadata(typeof(UniformButton)));
		}
	}
}

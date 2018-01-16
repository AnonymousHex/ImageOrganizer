using System.Windows;
using System.Windows.Controls.Primitives;

namespace ImageOrganizer.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class UniformScrollBar : ScrollBar
	{
		static UniformScrollBar()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(UniformScrollBar), new FrameworkPropertyMetadata(typeof(UniformScrollBar)));
		}
	}
}

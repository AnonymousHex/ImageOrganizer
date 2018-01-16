using System.Windows;
using System.Windows.Controls;

namespace ImageOrganizer.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class UniformScrollViewer : ScrollViewer
	{
		static UniformScrollViewer()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(UniformScrollViewer), new FrameworkPropertyMetadata(typeof(UniformScrollViewer)));
		}
	}
}

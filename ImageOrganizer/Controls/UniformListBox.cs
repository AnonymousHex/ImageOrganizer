using System.Windows;
using System.Windows.Controls;

namespace ImageOrganizer.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class UniformListBox : ListBox
	{
		static UniformListBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(UniformListBox), new FrameworkPropertyMetadata(typeof(UniformListBox)));
		}
	}
}

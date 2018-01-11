using System.Windows;
using System.Windows.Controls;

namespace ImageOrganizer.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class UniformTextBox : TextBox
	{
		static UniformTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(UniformTextBox), new FrameworkPropertyMetadata(typeof(UniformTextBox)));
		}
	}
}

using ImageOrganizer.Controls;

namespace ImageOrganizer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : UniformWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new MainWindowContext();
		}
	}
}

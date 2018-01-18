using System.Windows;
using System.Windows.Controls.Primitives;
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
			DataContext = new MainWindowContext(this);

			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			Left = Settings.Default.WindowLeft;
			Top = Settings.Default.WindowTop;
			WindowState = Settings.Default.WindowState;
			Width = Settings.Default.WindowWidth;
			Height = Settings.Default.WindowHeight;

			RootGrid.ColumnDefinitions[0].Width = new GridLength(Settings.Default.Column2Width);
			RootGrid.ColumnDefinitions[2].Width = new GridLength(Settings.Default.Column1Width);
		}

		private void Column1_OnDragCompleted(object sender, DragCompletedEventArgs e)
		{
			Settings.Default.Column1Width = RootGrid.ColumnDefinitions[0].Width.Value;
		}

		private void Column2_OnDragCompleted(object sender, DragCompletedEventArgs e)
		{
			Settings.Default.Column1Width = RootGrid.ColumnDefinitions[2].Width.Value;
		}
	}
}

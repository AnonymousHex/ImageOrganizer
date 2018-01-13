using ImageOrganizer.Presentation;

namespace ImageOrganizer.Organization
{
	public interface IImageHost
	{
		/// <summary>
		/// 
		/// </summary>
		void SelectImage(ImageItem item);

		/// <summary>
		/// 
		/// </summary>
		ImageItem SelectedImage { get; }
	}
}

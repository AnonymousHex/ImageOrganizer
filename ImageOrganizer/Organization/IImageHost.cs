using System.Collections.ObjectModel;
using ImageOrganizer.Presentation;

namespace ImageOrganizer.Organization
{
	/// <summary>
	/// A host for images.
	/// </summary>
	public interface IImageHost
	{
		/// <summary>
		/// Selects an image.
		/// </summary>
		void ToggleImageSelection(ImageItem item, bool select);

		/// <summary>
		/// The currently selected image.
		/// </summary>
		ImageItem SelectedImage { get; }

		/// <summary>
		/// A collection of tags that belon to the current image.
		/// </summary>
		ObservableCollection<Tag> CurrentTags { get; }
	}
}

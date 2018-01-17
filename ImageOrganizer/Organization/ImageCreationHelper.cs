using System.Threading;

namespace ImageOrganizer.Organization
{
	internal class ImageCreationHelper
	{
		public ImageCreationHelper(string filePath, WaitCallback callback, ManualResetEvent handle)
		{
			FilePath = filePath;
			Callback = callback;
			Handle = handle;
		}

		public ManualResetEvent Handle { get; private set; }

		public WaitCallback Callback { get; private set; }

		public string FilePath { get; private set; }
	}
}
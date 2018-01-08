using System;
using System.Windows.Forms;

// ------------------------------------------------------------------
// Wraps System.Windows.Forms.OpenFileDialog to make it present
// a vista-style dialog.
// sourced from: https://www.lyquidity.com/devblog/?p=136
// No license per "There’s no license as such as you are free to take and do with the code what you will."
// ------------------------------------------------------------------

namespace ImageOrganizer.Presentation.SelectFolder
{
	/// <summary>
	/// Wraps System.Windows.Forms.OpenFileDialog to make it present
	/// a vista-style dialog.
	/// </summary>
	public class FolderSelectDialog
	{
		// Wrapped dialog
		readonly OpenFileDialog _dialog;

		/// <summary>
		/// 
		/// </summary>
		public FolderSelectDialog()
		{
			_dialog = new OpenFileDialog
			{
				// ReSharper disable once LocalizableElement
				Filter = "Folders|\n",
				AddExtension = false,
				CheckFileExists = false,
				DereferenceLinks = true,
				Multiselect = false
			};
		}

		/// <summary>
		/// Gets/Sets the initial folder to be selected. A null value selects the current directory.
		/// </summary>
		public string InitialDirectory
		{
			get { return _dialog.InitialDirectory; }
		}

		/// <summary>
		/// Gets/Sets the title to show in the dialog
		/// </summary>
		public string Title
		{
			get { return _dialog.Title; }
			set { _dialog.Title = value ?? "Select Folder"; }
		}

		/// <summary>
		/// Gets the selected folder.
		/// </summary>
		public string FolderName
		{
			get { return _dialog.FileName; }
		}

		/// <summary>
		/// Shows the dialog
		/// </summary>
		/// <returns>True if the user presses OK else false</returns>
		public bool ShowDialog()
		{
			return ShowDialog(IntPtr.Zero);
		}

		/// <summary>
		/// Shows the dialog
		/// </summary>
		/// <param name="hWndOwner">Handle of the control to be parent</param>
		/// <returns>True if the user presses OK else false</returns>
		public bool ShowDialog(IntPtr hWndOwner)
		{
			bool flag;

			if (Environment.OSVersion.Version.Major >= 6)
			{
				var r = new Reflector("System.Windows.Forms");

				uint num = 0;
				Type typeIFileDialog = r.GetType("FileDialogNative.IFileDialog");
				object dialog = r.Call(_dialog, "CreateVistaDialog");
				r.Call(_dialog, "OnBeforeVistaDialog", dialog);

				uint options = (uint)r.CallAs(typeof(FileDialog), _dialog, "GetOptions");
				options |= (uint)r.GetEnum("FileDialogNative.FOS", "FOS_PICKFOLDERS");
				r.CallAs(typeIFileDialog, dialog, "SetOptions", options);

				object pfde = r.New("FileDialog.VistaDialogEvents", _dialog);
				object[] parameters = { pfde, num };
				r.CallAs2(typeIFileDialog, dialog, "Advise", parameters);
				num = (uint)parameters[1];
				try
				{
					int num2 = (int)r.CallAs(typeIFileDialog, dialog, "Show", hWndOwner);
					flag = 0 == num2;
				}
				finally
				{
					r.CallAs(typeIFileDialog, dialog, "Unadvise", num);
					GC.KeepAlive(pfde);
				}
			}
			else
			{
				var fbd = new FolderBrowserDialog
				{
					Description = Title,
					SelectedPath = InitialDirectory,
					ShowNewFolderButton = false
				};

				if (fbd.ShowDialog(new WindowWrapper(hWndOwner)) != DialogResult.OK) 
					return false;

				_dialog.FileName = fbd.SelectedPath;
				flag = true;
			}

			return flag;
		}
	}

	/// <summary>
	/// Creates IWin32Window around an IntPtr
	/// </summary>
	internal class WindowWrapper : IWin32Window
	{
		private readonly IntPtr _hwnd;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="handle">Handle to wrap</param>
		public WindowWrapper(IntPtr handle)
		{
			_hwnd = handle;
		}

		/// <summary>
		/// Original ptr
		/// </summary>
		public IntPtr Handle
		{
			get { return _hwnd; }
		}
	}
}

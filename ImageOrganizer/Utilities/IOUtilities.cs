using System.IO;
using System.Runtime.Serialization;

namespace ImageOrganizer.Utilities
{
	// ReSharper disable once InconsistentNaming
	public class IOUtilities
	{
		/// <summary>
		/// Saves an object to a file using data contract serialization.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="data"></param>
		public static void SaveObject(string filePath, object data)
		{
			using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
			{
				var serializer = new DataContractSerializer(data.GetType());
				serializer.WriteObject(fs, data);
			}
		}

		/// <summary>
		/// Deserializes data from a file into the member.
		/// </summary>
		public static void LoadObject<T>(string filePath, ref T member)
		{
			if (File.Exists(filePath) == false)
				return;

			using (var fs = new FileStream(filePath, FileMode.Open))
			{
				var serializer = new DataContractSerializer(member.GetType());
				member = (T)serializer.ReadObject(fs);
			}
		}
	}
}

using System.IO;
using Wanderers.Utils;

namespace Wanderers.Storage
{
	public class Slot
	{
		private const string SavesFolderName = "Saves";

		private readonly int _index;

		public int Index
		{
			get { return _index; }
		}

		private static string SavesFolder
		{
			get
			{
				return Path.Combine(Files.ExecutableFolder, SavesFolderName);
			}
		}

		public string SaveFolder
		{
			get
			{
				return Path.Combine(SavesFolder, Index.ToString());
			}
		}

		private string CharacterFilePath
		{
			get
			{
				return Path.Combine(SaveFolder, "character.json");
			}
		}

		public CharacterData CharacterData { get; set; }

		public Slot(int index)
		{
			_index = index;

			if (File.Exists(CharacterFilePath))
			{
				CharacterData = Serialization.LoadFromPath<CharacterData>(CharacterFilePath);
			}
		}

		public void Save()
		{
			var savesFolder = SavesFolder;
			if (!Directory.Exists(savesFolder))
			{
				Directory.CreateDirectory(savesFolder);
			}

			var saveFolder = SaveFolder;
			if (!Directory.Exists(saveFolder))
			{
				Directory.CreateDirectory(saveFolder);
			}

			Serialization.SaveToPath(CharacterData, CharacterFilePath);
		}
	}
}
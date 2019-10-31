using System.IO;
using Wanderers.Utils;

namespace Wanderers.Storage
{
	public class Slot
	{
		private const string SavesFolderName = "saves";

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
				CharacterData = CharacterData.FromJson(File.ReadAllText(CharacterFilePath));
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

			var s = CharacterData.ToJson();
			File.WriteAllText(CharacterFilePath, s);
		}
	}
}
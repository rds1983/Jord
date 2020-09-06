using System.IO;
using TroublesOfJord.Utils;

namespace TroublesOfJord.Storage
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

		public string SaveFolder => BuildSaveFolder(Index);

		private string PlayerFilePath => BuildPlayerFilePath(Index);

		public PlayerData PlayerData { get; set; }

		public Slot(int index)
		{
			_index = index;

			if (File.Exists(PlayerFilePath))
			{
				PlayerData = PlayerData.FromJson(File.ReadAllText(PlayerFilePath));
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

			var s = PlayerData.ToJson();
			File.WriteAllText(PlayerFilePath, s);
		}

		private static string BuildSaveFolder(int slotIndex)
		{
			return Path.Combine(SavesFolder, slotIndex.ToString());
		}

		private static string BuildPlayerFilePath(int slotIndex)
		{
			return Path.Combine(BuildSaveFolder(slotIndex), "player.json");
		}

		public static bool Exists(int slotIndex)
		{
			return Directory.Exists(BuildSaveFolder(slotIndex)) && File.Exists(BuildPlayerFilePath(slotIndex));
		}
	}
}
using Jord.Core;
using Jord.Storage;

namespace Jord
{
	partial class TJ
	{
		public static Player LoadCurrentGame()
		{
			var slot = StorageService.Slots[SlotIndex.Value];
			return slot.PlayerData.CreateCharacter();
		}

		public static void SaveCurrentGame()
		{
			if (SlotIndex == null)
			{
				return;
			}

			StorageService.Slots[SlotIndex.Value].PlayerData = new PlayerData(TJ.Player);
			StorageService.Slots[SlotIndex.Value].Save();
		}
	}
}

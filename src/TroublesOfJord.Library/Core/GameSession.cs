using System.Collections.Generic;
using TroublesOfJord.Storage;

namespace TroublesOfJord.Core
{
	public class GameSession
	{
		public Slot Slot { get; }
		public Character Character { get; }
		public Player Player { get { return Character.Player; } }

		public GameSession(int slotIndex)
		{
			Slot = TJ.StorageService.Slots[slotIndex];

			Character = Slot.CharacterData.CreateCharacter();

			// Spawn player
			var map = TJ.Module.Maps[Slot.CharacterData.StartingMapId];
			Player.Place(map, map.SpawnSpot.Value);
			Player.Stats.Life.Restore();
		}
	}
}

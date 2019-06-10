using System;
using Wanderers.Storage;
using Wanderers.Utils;

namespace Wanderers.Core
{
	public class GameSession
	{
		private readonly Slot _slot;
		private readonly Character _character;

		public Slot Slot
		{
			get
			{
				return _slot;
			}
		}

		public Character Character
		{
			get
			{
				return _character;
			}
		}

		public Player Player
		{
			get
			{
				return _character.Player;
			}
		}

		public GameSession(int slotIndex)
		{
			_slot = TJ.StorageService.Slots[slotIndex];

			_character = _slot.CharacterData.CreateCharacter();

			// Spawn player
			// First spawn spot
			var placed = false;
			foreach (var pair in TJ.Module.Maps)
			{
				if (pair.Value.SpawnSpot != null)
				{
					var map = pair.Value;
					Player.Place(map, pair.Value.SpawnSpot.Value.ToVector2());
					placed = true;
					break;
				}
			}

			if (!placed)
			{
				throw new Exception("Could not find a player spawn spot");
			}
		}

		public void OnTimer()
		{
			Player.OnTimer();
		}
	}
}

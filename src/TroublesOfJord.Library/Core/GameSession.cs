using System.Collections.Generic;
using TroublesOfJord.Storage;

namespace TroublesOfJord.Core
{
	public class GameSession
	{
		private bool _activeCreaturesDirty = true;
		private readonly List<Creature> _activeCreatures = new List<Creature>();
		private readonly List<Creature> _activeCreaturesCopy = new List<Creature>();

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

		public void AddActiveCreature(Creature creature)
		{
			if (_activeCreatures.Contains(creature))
			{
				return;
			}

			_activeCreatures.Add(creature);
			_activeCreaturesDirty = true;
		}

		public void RemoveActiveCreature(Creature creature)
		{
			if (!_activeCreatures.Contains(creature))
			{
				return;
			}

			_activeCreatures.Remove(creature);
			_activeCreaturesDirty = true;
		}

		public void OnTimer()
		{
			if (_activeCreaturesDirty)
			{
				_activeCreaturesCopy.Clear();
				_activeCreaturesCopy.AddRange(_activeCreatures);
				_activeCreaturesDirty = false;
			}

			foreach(var creature in _activeCreaturesCopy)
			{
				creature.OnTimer();
			}
		}
	}
}

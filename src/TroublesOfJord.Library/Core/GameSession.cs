using GoRogue;
using GoRogue.MapViews;
using TroublesOfJord.Storage;
using TroublesOfJord.UI;
using TroublesOfJord.Utils;

namespace TroublesOfJord.Core
{
	public class GameSession
	{
		public Slot Slot { get; }
		public Player Player { get; }

		public MapNavigationBase MapNavigationBase;

		public GameSession(int slotIndex)
		{
			Slot = TJ.StorageService.Slots[slotIndex];

			Player = Slot.PlayerData.CreateCharacter();

			// Spawn player
			var map = TJ.Module.Maps[Slot.PlayerData.StartingMapId];
			Player.Place(map, map.SpawnSpot.Value);
			Player.Stats.Life.Restore();
		}

		public bool MovePlayer(MovementDirection direction, bool isRunning)
		{
			var result = Player.MoveTo(direction.GetDelta());

			if (result)
			{
				// Let npcs act
				var map = Player.Map;
				for (var x = 0; x < map.Width; ++x)
				{
					for (var y = 0; y < map.Height; ++y)
					{
						var npc = map[x, y].Creature as NonPlayer;
						if (npc == null)
						{
							continue;
						}

						npc.Act();
					}
				}

				UpdateTilesVisibility();
			}

			return result;
		}

		public void UpdateTilesVisibility()
		{
			var map = Player.Map;

			// Reset IsInFov for the whole map
			for(var x = 0; x < map.Width; ++x)
			{
				for(var y = 0; y < map.Height; ++y)
				{
					map[x, y].IsInFov = false;
				}
			}

			map.FieldOfView.Calculate(Player.Position.X, Player.Position.Y, 12);

			var mapDirty = false;
			foreach(var coord in map.FieldOfView.CurrentFOV)
			{
				var tile = map[coord];

				tile.IsInFov = true;
				if (!tile.IsExplored)
				{
					mapDirty = true;
					tile.IsExplored = true;
				}
			}

			if (mapDirty)
			{
				MapNavigationBase.InvalidateImage();
			}
		}
	}
}
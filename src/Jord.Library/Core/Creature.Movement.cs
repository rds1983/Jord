using Jord.Utils;
using Microsoft.Xna.Framework;

namespace Jord.Core
{
	partial class Creature
	{
		public bool CanEnter()
		{
			return Tile != null && Tile.Exit != null;
		}

		public bool Enter()
		{
/*			if (Tile == null || Tile.Exit == null)
			{
				return false;
			}

			Tile exitTile = null;

			var previousMap = Map;

			Map map;

			if (TJ.Database.Dungeons.ContainsKey(Tile.Exit.MapId))
			{
				var dungeon = TJ.Database.Dungeons.Ensure(Tile.Exit.MapId);

				var dungeonLevel = Tile.Exit.DungeonLevel == null ? 1 : Tile.Exit.DungeonLevel.Value;
				map = dungeon.Generate(dungeonLevel);
				for (var x = 0; x < map.Width; ++x)
				{
					for (var y = 0; y < map.Height; ++y)
					{
						var tile = map[x, y];
						if (tile.Exit == null)
						{
							continue;
						}

						if (tile.Exit.MapId == previousMap.Id && tile.Exit.DungeonLevel == previousMap.DungeonLevel)
						{
							// Found backwards exit
							exitTile = tile;
							goto found;
						}
					}
				}
			found:;
			}
			else
			{
				map = TJ.Database.Maps.Ensure(Tile.Exit.MapId);

				if (Tile.Exit.Position != null)
				{
					exitTile = map[Tile.Exit.Position.Value];
				}
				else
				{
					// If position isnt set explicitly
					// Then we search for a tile that has exit to the current creature map
					exitTile = map.EnsureExitTileById(previousMap.Id);
				}
			}

			Remove();
			Place(map, exitTile.Position);

			OnEntered();*/

			return true;
		}

		protected virtual void OnEntered()
		{
		}
	}
}

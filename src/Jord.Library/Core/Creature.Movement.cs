using Microsoft.Xna.Framework;

namespace Jord.Core
{
	partial class Creature
	{
		public void SetPosition(Point position)
		{
			var currentTile = Map[Position];
			var newTile = Map[position];

			if (currentTile != newTile)
			{
				newTile.Creature = currentTile.Creature;
				currentTile.Creature = null;
			}

			Position = position;
			DisplayPosition = position.ToVector2();
		}

		public bool MoveTo(Point delta)
		{
			if (delta == Point.Zero)
			{
				return false;
			}

			var newPosition = Position + delta;

			if (newPosition.X < 0 || newPosition.Y < 0 || newPosition.X >= Map.Width || newPosition.Y >= Map.Height)
			{
				return false;
			}

			var newTile = Map[newPosition];
			var creature = newTile.Creature;
			if (creature != null)
			{
				var asNonPlayer = creature as NonPlayer;
				if (asNonPlayer != null && asNonPlayer.Info.CreatureType == CreatureType.Enemy)
				{
					Attack(creature);

					return true;
				}
			}

			if (!newTile.Info.Passable || newTile.Object != null)
			{
				return false;
			}

			SetPosition(newPosition);

			return true;
		}

		public bool CanEnter()
		{
			return Tile != null && Tile.Exit != null;
		}

		public bool Enter()
		{
			if (Tile == null || Tile.Exit == null)
			{
				return false;
			}

			Tile exitTile = null;

			var previousMap = Map;
			
			Map map;
			
			if (TJ.Module.Dungeons.ContainsKey(Tile.Exit.MapId))
			{
				var dungeon = TJ.Module.Dungeons.Ensure(Tile.Exit.MapId);

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
				map = TJ.Module.Maps.Ensure(Tile.Exit.MapId);

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

			OnEntered();

			return true;
		}

		protected virtual void OnEntered()
		{
		}
	}
}

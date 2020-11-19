using Microsoft.Xna.Framework;

namespace TroublesOfJord.Core
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

			if (!newTile.Info.Passable)
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

			Map map = null;
			Tile exitTile = null;

			if (TJ.Module.MapTemplates.ContainsKey(Tile.Exit.MapId))
			{
				var mapTemplate = TJ.Module.MapTemplates.Ensure(Tile.Exit.MapId);
				map = mapTemplate.Generate();
				for(var x = 0; x < map.Width; ++x)
				{
					for(var y = 0; y < map.Height; ++y)
					{
						var tile = map[x, y];
						if (tile.Exit == null)
						{
							continue;
						}

						if (tile.Exit.MapId == Map.Id)
						{
							// Found backwards exit
							exitTile = tile;
							goto found;
						}
					}
				}
				found:;
			} else
			{
				map = TJ.Module.Maps.Ensure(Tile.Exit.MapId);

				if (Tile.Exit.Position != null)
				{
					exitTile = map[Tile.Exit.Position.Value];
				} else
				{
					exitTile = map.EnsureExitTileById(Tile.Exit.ExitMapId);
				}
			}

			Remove();
			Place(map, exitTile.Position);

			return true;
		}
	}
}

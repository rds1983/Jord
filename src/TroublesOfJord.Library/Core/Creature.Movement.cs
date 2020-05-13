using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TroublesOfJord.Utils;

namespace TroublesOfJord.Core
{
	partial class Creature
	{
		public void MoveTo(Point delta)
		{
			var newPosition = Position + delta;
			if (!TilePassable(newPosition) || delta == Point.Zero)
			{
				return;
			}

			SetPosition(newPosition);
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
				var mapTemplate = TJ.Module.EnsureMapTemplate(Tile.Exit.MapId);
				map = mapTemplate.Generate();
				for(var x = 0; x < map.Size.X; ++x)
				{
					for(var y = 0; y < map.Size.Y; ++y)
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
				map = TJ.Module.EnsureMap(Tile.Exit.MapId);

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

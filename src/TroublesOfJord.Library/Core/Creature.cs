using System;
using Microsoft.Xna.Framework;
using TroublesOfJord.Utils;

namespace TroublesOfJord.Core
{
	public abstract partial class Creature
	{
		public Map Map { get; set; }
		public Point Position { get; set; }
		public Vector2 DisplayPosition { get; set; }

		public Tile Tile
		{
			get
			{
				if (Map == null)
				{
					return null;
				}

				return Map[Position];
			}
		}

		public abstract Appearance Image { get; }
		public abstract CreatureStats Stats { get; }

		public string Name { get; set; }
		public int Gold { get; set; }

		public float Opacity = 1.0f;

		public Inventory Inventory { get; } = new Inventory();

		public bool IsPlaceable(Map map, Point pos)
		{
			if (pos.X < 0 || pos.X >= map.Size.X ||
				pos.Y < 0 || pos.Y >= map.Size.Y)
			{
				// Out of range
				return false;
			}

			var tile = map[pos];
			if (tile.Creature != null || tile.Info == null || !tile.Info.Passable)
			{
				return false;
			}

			return true;
		}

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

		public bool IsMoveable(Map map, Point pos)
		{
			var result = false;
			var x = pos.X;
			var y = pos.Y;

			if (x >= 0 && y >= 0 && x < map.Size.X && y < map.Size.Y)
			{
				var tile = map[pos];
				if (tile.Info.Passable &&
					(tile.Creature == null ||
					 (tile.Creature == this)))
				{
					result = true;
				}
			}

			return result;
		}

		public void Place(Map map, Point position)
		{
			if (position.X < 0 || position.X >= map.Size.X ||
				position.Y < 0 || position.Y >= map.Size.Y)
			{
				throw new ArgumentOutOfRangeException("position");
			}

			Map = map;
			Position = position;
			DisplayPosition = position.ToVector2();

			var tile = map[position];
			tile.Creature = this;
		}

		public bool Remove()
		{
			if (Map == null)
			{
				return false;
			}

			var tile = Map[Position];
			tile.Creature = null;

			Map = null;

			return true;
		}
	}
}
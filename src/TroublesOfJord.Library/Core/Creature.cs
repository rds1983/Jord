﻿using System;
using Microsoft.Xna.Framework;

namespace TroublesOfJord.Core
{
	public abstract partial class Creature
	{
		private Map _map;

		public Map Map
		{
			get
			{
				return _map;
			}

			set
			{
				if (_map == value)
				{
					return;
				}

				if (_map != null)
				{
					_map.Creatures.Remove(this);
				}

				_map = value;

				if (_map != null)
				{
					_map.Creatures.Add(this);
				}
			}
		}
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
			if (pos.X < 0 || pos.X >= map.Width ||
				pos.Y < 0 || pos.Y >= map.Height)
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

		public void Place(Map map, Point position)
		{
			if (position.X < 0 || position.X >= map.Width ||
				position.Y < 0 || position.Y >= map.Height)
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
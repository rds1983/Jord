using System;
using Microsoft.Xna.Framework;

namespace Wanderers.Core
{
	public enum CreatureState
	{
		Idle,
		Moving,
		Fighting
	}

	public abstract partial class Creature
	{
		private CreatureState _state;

		private DateTime _actionStart;

		private readonly Inventory _inventory = new Inventory();

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

				return Map.GetTileAt(Position);
			}
		}

		public abstract Appearance Image { get; }
		public abstract AttackInfo[] Attacks { get; }

		public string Name { get; set; }
		public int Gold { get; set; }

		private CreatureState State
		{
			get
			{
				return _state;
			}

			set
			{
				if (value == _state)
				{
					return;
				}

				_state = value;

				if (_state == CreatureState.Idle)
				{
					TJ.Session.RemoveActiveCreature(this);
				} else
				{
					TJ.Session.AddActiveCreature(this);
				}
			}
		}

		public Inventory Inventory
		{
			get
			{
				return _inventory;
			}
		}

		public Creature AttackTarget { get; set; }
		public int AttackDelayInMs { get; set; }

		public bool IsPlaceable(Map map, Vector2 pos)
		{
			if (pos.X < 0 || pos.X >= map.Size.X ||
				pos.Y < 0 || pos.Y >= map.Size.Y)
			{
				// Out of range
				return false;
			}

			var tile = map.GetTileAt(pos);
			if (tile.Creature != null || tile.Info == null || !tile.Info.Passable)
			{
				return false;
			}

			return true;
		}

		public void SetPosition(Point position)
		{
			var currentTile = Map.GetTileAt(Position);
			var newTile = Map.GetTileAt(position);

			if (currentTile != newTile)
			{
				newTile.Creature = currentTile.Creature;
				currentTile.Creature = null;
			}

			Position = position;
			DisplayPosition = position.ToVector2();
		}

		public bool IsMoveable(Map map, Vector2 pos)
		{
			var result = false;
			var x = (int)pos.X;
			var y = (int)pos.Y;

			if (x >= 0 && y >= 0 && x < map.Size.X && y < map.Size.Y)
			{
				var tile = map.GetTileAt(pos);
				if (tile.Info.Passable &&
					(tile.Creature == null ||
					 (tile.Creature == this)))
				{
					result = true;
				}
			}

			return result;
		}

		private bool TilePassable(Point pos)
		{
			var tile = Map.GetTileAt(pos);

			return tile.Info.Passable &&
				   (tile.Creature == null ||
					tile.Creature == this);
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

			var tile = map.GetTileAt(position);
			tile.Creature = this;
		}

		public bool Remove()
		{
			if (Map == null)
			{
				return false;
			}

			var tile = Map.GetTileAt(Position);
			tile.Creature = null;

			Map = null;

			return true;
		}

		public void OnTimer()
		{
			switch (State)
			{
				case CreatureState.Idle:
					// Nothing
					break;
				case CreatureState.Moving:
					ProcessMovement();
					break;
				case CreatureState.Fighting:
					ProcessFighting();
					break;
			}
		}
	}
}
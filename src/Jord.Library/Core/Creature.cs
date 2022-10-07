using System;
using System.Diagnostics;
using Jord.Core.Items;
using Jord.Utils;
using Microsoft.Xna.Framework;

namespace Jord.Core
{
	public abstract partial class Creature
	{
		private Map _map;
		private Point _position;

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
		public Point Position
		{
			get => _position;
			set
			{
				if (value == _position)
				{
					return;
				}

				_position = value;
				OnPositionChanged();
			}
		}

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

		public float Opacity { get; set; } = 1.0f;

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
			if (tile.Object == null || tile.Creature != null || tile.Info == null || !tile.Info.Passable)
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

		protected virtual void OnPositionChanged()
		{
		}

		public void RegenTurn()
		{
			if (Stats.Life.CurrentHP < Stats.Life.MaximumHP)
			{
				var hpRegen = Stats.Life.HpRegen;
				if (Stats.Life.CurrentHP + hpRegen > Stats.Life.MaximumHP)
				{
					hpRegen = Stats.Life.MaximumHP - Stats.Life.CurrentHP;
				}
				Stats.Life.CurrentHP += hpRegen;
				Debug.WriteLine("{0} regenerated {1} hit points.", Name, hpRegen);
			}

			if (Stats.Life.CurrentMana < Stats.Life.MaximumMana)
			{
				var manaRegen = Stats.Life.ManaRegen;
				if (Stats.Life.CurrentMana + manaRegen > Stats.Life.MaximumMana)
				{
					manaRegen = Stats.Life.MaximumMana - Stats.Life.CurrentMana;
				}
				Stats.Life.CurrentMana += manaRegen;
				Debug.WriteLine("{0} regenerated {1} mana.", Name, manaRegen);
			}

			if (Stats.Life.CurrentStamina < Stats.Life.MaximumStamina)
			{
				var staminaRegen = Stats.Life.StaminaRegen;
				if (Stats.Life.CurrentStamina + staminaRegen > Stats.Life.MaximumStamina)
				{
					staminaRegen = Stats.Life.MaximumStamina - Stats.Life.CurrentStamina;
				}
				Stats.Life.CurrentStamina += staminaRegen;
				Debug.WriteLine("{0} regenerated {1} stamina.", Name, staminaRegen);
			}
		}

		public void TakeLyingItem(int index, int count)
		{
			var item = Tile.Inventory.Items[index];

			Inventory.Add(item.Item, count);
			Tile.Inventory.Add(item.Item, -count);

			OnItemTaken(item.Item, count);
		}

		protected virtual void OnItemTaken(Item item, int count)
		{
		}
	}
}
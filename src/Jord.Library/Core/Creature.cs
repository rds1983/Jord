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


		public abstract Appearance Image { get; }
		public abstract CreatureStats Stats { get; }
		public abstract Inventory Inventory { get; }

		public string Name { get; set; }
		public int Gold { get; set; }

		public float Opacity { get; set; } = 1.0f;

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

		protected virtual void OnItemTaken(Item item, int count)
		{
		}
	}
}
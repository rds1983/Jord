using System;
using System.Collections.Generic;
using System.Linq;
using Jord.Core.Abilities;
using Jord.Core.Items;
using Jord.Utils;

namespace Jord.Core
{
	public class Player : Creature
	{
		private int _level;

		private readonly CreatureStats _stats = new CreatureStats();
		private readonly Inventory _inventory = new Inventory();

		private bool _dirty = true;

		public int Level
		{
			get
			{
				return _level;
			}

			set
			{
				if (_level == value)
				{
					return;
				}

				_level = value;
				Invalidate();
			}
		}

		public int Experience { get; set; }

		public Class Class { get; set; }

		public override Appearance Image => TJ.Database.Settings.PlayerAppearance;

		public Equipment Equipment { get; } = new Equipment();

		public AbilityInfo[] Abilities { get; set; }

		public override CreatureStats Stats
		{
			get
			{
				Update();
				return _stats;
			}
		}

		public override Inventory Inventory { get => _inventory; }

		public Player()
		{
			Equipment.Changed += (s, a) => Invalidate();
		}

		public int CalculateBonus(BonusType bonusType)
		{
			var result = 0;
/*			foreach (var perk in Perks)
			{
				if (perk.AddsEffects == null || perk.AddsEffects.Length == 0)
				{
					continue;
				}

				foreach (var effect in perk.AddsEffects)
				{
					var bonusValue = 0;
					effect.Bonuses.TryGetValue(bonusType, out bonusValue);
					result += bonusValue;
				}
			}*/

			return result;
		}

		private void UpdateStats()
		{
			var levelValue = (float)Math.Sqrt(Level);

			// Life
			_stats.Life.MaximumHP = (int)(Class.HpMultiplier * levelValue);
			_stats.Life.HpRegen = Class.HpRegenMultiplier * levelValue;

			_stats.Life.MaximumMana = (int)(Class.ManaMultiplier * levelValue);
			_stats.Life.ManaRegen = Class.ManaRegenMultiplier * levelValue;

			_stats.Life.MaximumStamina = (int)(Class.StaminaMultiplier * levelValue);
			_stats.Life.StaminaRegen = Class.StaminaRegenMultiplier * levelValue;

			// Battle
			var battleStats = _stats.Battle;
			battleStats.MeleeMastery = Class.MeleeMastery + CalculateBonus(BonusType.MeleeMastery);
			battleStats.ArmorClass = Class.ArmorClass + CalculateBonus(BonusType.ArmorClass);
			battleStats.EvasionRating = Class.EvasionRating + CalculateBonus(BonusType.EvasionRating);
			battleStats.BlockingRating = Class.BlockingRating + CalculateBonus(BonusType.BlockingRating);

			var weapon = Equipment.GetItemByType(EquipType.RightHand);

			AttackInfo attackInfo;
			if (weapon == null)
			{
				attackInfo = new AttackInfo(AttackType.Hit, 1, 4);
			}
			else
			{
				var weaponInfo = (WeaponInfo)weapon.Info;
				attackInfo = new AttackInfo(weaponInfo.AttackType, weaponInfo.MinDamage, weaponInfo.MaxDamage);
			}


			var additionalAttacks = CalculateBonus(BonusType.Attacks);
			var attacksCount = 1 + additionalAttacks;


			battleStats.Attacks = new AttackInfo[attacksCount];

			for (var i = 0; i < attacksCount; ++i)
			{
				battleStats.Attacks[i] = attackInfo;
			}

			foreach (var slot in Equipment.Items)
			{
				if (slot == null || slot.Item == null)
				{
					continue;
				}

				var info = (EquipInfo)slot.Item.Info;

				battleStats.ArmorClass += info.ArmorClass;
			}
		}

		private void UpdateLevel()
		{
			var nextLevel = TJ.Database.LevelCosts[Level + 1];

			if (nextLevel.Experience > Experience)
			{
				return;
			}

			// Level up
			Experience -= nextLevel.Experience;
			Level++;

			TJ.GameLog(Strings.BuildNextLevel(Level));
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			UpdateStats();

			_dirty = false;
		}

		public void Invalidate()
		{
			_dirty = true;
		}

		public AbilityInfo[] BuildFreeAbilities()
		{
			// TODO:
			return TJ.Database.Abilities.Values.ToArray();
		}

		public AbilityInfo[] BuildLearnedAbilities()
		{
			// TODO:
			return new AbilityInfo[0];
		}

		protected override void OnKilledTarget(Creature target)
		{
			base.OnKilledTarget(target);

			// Death message
			var nonPlayer = (NonPlayer)target;

			var message = Strings.GetNpcDeathMessage(target.Name);
			TJ.GameLog(message);

			// Experience award
			Experience += nonPlayer.Info.Experience;
			message = Strings.GetExpMessage(nonPlayer.Info.Experience);
			TJ.GameLog(message);

			// Generate loot
			var loot = nonPlayer.Info.Loot;
			if (loot != null && loot.Count > 0)
			{
				foreach (var lootEntry in loot)
				{
					var success = MathUtils.RollPercentage(lootEntry.Rate);
					if (success)
					{
						var item = new Item(lootEntry.ItemInfo);
						target.Tile.Inventory.Add(item, 1);
					}
				}
			}

			target.Remove();

			UpdateLevel();
		}

		protected override void OnPositionChanged()
		{
			base.OnPositionChanged();

			if (Tile == null || Tile.Inventory.Items.Count == 0)
			{
				return;
			}

			if (Tile.Inventory.Items.Count == 1)
			{
				TJ.GameLog(Strings.BuildItemLyingOnTheFloor(Tile.Inventory.Items[0].Item.Info.Name));
			}
			else
			{
				TJ.GameLog(Strings.SomeItemsAreLying);
			}
		}

		protected override void OnEntered()
		{
			base.OnEntered();

			TJ.GameLog(Strings.BuildEnteredMap(Map.Level));
		}

		protected override void OnItemTaken(Item item, int count)
		{
			base.OnItemTaken(item, count);

			TJ.GameLog(Strings.BuildPickedUp(item.Info.Name, count));
		}
	}
}
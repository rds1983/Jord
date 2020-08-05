using System.Linq;
using TroublesOfJord.Core.Abilities;
using TroublesOfJord.Core.Items;

namespace TroublesOfJord.Core
{
	public class Player : Creature
	{
		public const int PlayerRoundInMs = 6000;

		private readonly CreatureStats _stats = new CreatureStats();

		private bool _dirty = true;

		public int Level { get; set; }
		public int Experience { get; set; }

		public Class Class { get; set; }

		public override Appearance Image => TJ.Module.ModuleInfo.PlayerAppearance;

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

		public Player()
		{
			Equipment.Changed += (s, a) => Invalidate();
		}

		private void UpdateStats()
		{
			// Life
			_stats.Life.MaximumHP = _stats.Life.MaximumMana = _stats.Life.MaximumStamina = 50;

			// Battle
			var weapon = Equipment.GetItemByType(EquipType.Weapon);

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

			var attacksCount = 1;
			foreach(var ability in Abilities)
			{
				foreach(var pair in ability.Bonuses)
				{
					switch (pair.Key)
					{
						case BonusType.Attacks:
							attacksCount += pair.Value;
							break;
					}
				}
			}

			var battleStats = _stats.Battle;
			battleStats.Attacks = new AttackInfo[attacksCount];

			for(var i = 0; i < attacksCount; ++i)
			{
				battleStats.Attacks[i] = attackInfo;
			}

			battleStats.ArmorClass = 0;

			foreach (var slot in Equipment.Items)
			{
				if (slot == null || slot.Item == null)
				{
					continue;
				}

				var info = (EquipInfo)slot.Item.Info;

				battleStats.ArmorClass += info.ArmorClass;
			}

			battleStats.HitRoll = 2;
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
			return TJ.Module.Abilities.Values.ToArray();
		}

		public AbilityInfo[] BuildLearnedAbilities()
		{
			// TODO:
			return new AbilityInfo[0];
		}

		protected override void OnKilledTarget(Creature target)
		{
			base.OnKilledTarget(target);

			var nonPlayer = (NonPlayer)target;

			Experience += nonPlayer.Info.Experience;
			Gold += nonPlayer.Info.Gold;

			var message = Strings.GetNpcDeathMessage(target.Name);
			TJ.GameLog(message);

			message = Strings.GetExpGoldMessage(nonPlayer.Info.Experience, nonPlayer.Info.Gold);
			TJ.GameLog(message);

			target.Remove();
		}
	}
}
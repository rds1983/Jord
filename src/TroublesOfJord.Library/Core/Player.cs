using TroublesOfJord.Core.Items;
using TroublesOfJord.Core.Skills;

namespace TroublesOfJord.Core
{
	public class Player : Creature
	{
		public const int PlayerRoundInMs = 6000;

		private readonly CreatureStats _stats = new CreatureStats();
		private IUsableAbility[] _usableAbilities = null;

		private bool _dirty = true;

		public override Appearance Image => TJ.Module.ModuleInfo.PlayerAppearance;

		public Equipment Equipment { get; } = new Equipment();

		public override CreatureStats Stats
		{
			get
			{
				Update();
				return _stats;
			}
		}

		public IUsableAbility[] UsableAbilities
		{
			get
			{
				if (_usableAbilities == null)
				{
					_usableAbilities = new IUsableAbility[]
					{
						new Kick()
					};
				}

				return _usableAbilities;
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

			var delay = PlayerRoundInMs / 2;
			AttackInfo attackInfo = null;
			if (weapon == null)
			{
				attackInfo = new AttackInfo(AttackType.Hit, 1, 4, delay);
			}
			else
			{
				var weaponInfo = (WeaponInfo)weapon.Info;
				attackInfo = new AttackInfo(weaponInfo.AttackType, weaponInfo.MinDamage, weaponInfo.MaxDamage, delay);
			}

			var battleStats = _stats.Battle;
			battleStats.Attacks = new AttackInfo[]
			{
				attackInfo,
				attackInfo
			};

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
	}
}
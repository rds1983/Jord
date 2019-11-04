using Microsoft.Xna.Framework;
using Wanderers.Core.Items;

namespace Wanderers.Core
{
	public class Player : Creature
	{
		public const int PlayerRoundInMs = 6000;

		private readonly BattleStats _battleStats = new BattleStats();

		private readonly Appearance _playerAppearance = new Appearance('@', Color.White);
		private bool _dirty = true;

		public override Appearance Image => _playerAppearance;

		public Equipment Equipment { get; } = new Equipment();

		public override BattleStats BattleStats
		{
			get
			{
				Update();
				return _battleStats;
			}
		}

		public Player()
		{
			Equipment.Changed += (s, a) => Invalidate();
		}

		private void UpdateBattleStats()
		{
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

			_battleStats.Attacks = new AttackInfo[]
			{
				attackInfo,
				attackInfo
			};

			_battleStats.ArmorClass = 0;

			foreach (var slot in Equipment.Items)
			{
				if (slot == null || slot.Item == null)
				{
					continue;
				}

				var info = (EquipInfo)slot.Item.Info;

				_battleStats.ArmorClass += info.ArmorClass;
			}

			_battleStats.HitRoll = 2;
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			UpdateBattleStats();

			_dirty = false;
		}

		public void Invalidate()
		{
			_dirty = true;
		}
	}
}
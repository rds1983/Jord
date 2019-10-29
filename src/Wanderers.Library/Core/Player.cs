using Microsoft.Xna.Framework;
using Wanderers.Core.Items;

namespace Wanderers.Core
{
	public class Player : Creature
	{
		public const int AC = 100;

		public const int PlayerRoundInMs = 6000;

		private readonly Appearance _playerAppearance = new Appearance('@', Color.White);
		private bool _dirty = true;
		private AttackInfo[] _attacks = null;

		public override Appearance Image => _playerAppearance;
		public Equipment Equipment { get; } = new Equipment();
		public override AttackInfo[] Attacks
		{
			get
			{
				Update();
				return _attacks;
			}
		}

		public Player()
		{
			Equipment.Changed += (s, a) => Invalidate();
		}

		private void UpdateAttacks()
		{
			var weapon = Equipment.GetItemByType(EquipType.Weapon);

			var delay = PlayerRoundInMs;
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

			_attacks = new AttackInfo[]
			{
				attackInfo
			};
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			UpdateAttacks();

			_dirty = false;
		}

		public void Invalidate()
		{
			_dirty = true;
		}
	}
}
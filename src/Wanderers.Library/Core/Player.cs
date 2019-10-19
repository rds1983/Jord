using Microsoft.Xna.Framework;
using Wanderers.Core.Items;

namespace Wanderers.Core
{
	public class Player : Creature
	{
		private readonly Appearance _playerAppearance = new Appearance('@', Color.White);
		private readonly Equipment _equipment = new Equipment();
		private bool _dirty = true;
		private AttackInfo[] _attacks = null;

		public override Appearance Image => _playerAppearance;
		public Equipment Equipment => _equipment;
		public override AttackInfo[] Attacks
		{
			get
			{
				Update();
				return _attacks;
			}
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			_attacks = new AttackInfo[]
			{
				new AttackInfo(AttackType.Hit, 5, 10, 1000),
				new AttackInfo(AttackType.Slash, 10, 15, 2000)
			};

			_dirty = false;
		}
	}
}
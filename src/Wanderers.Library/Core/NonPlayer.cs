using System;

namespace Wanderers.Core
{
	public class NonPlayer : Creature
	{
		private bool _dirty = true;
		private readonly CreatureInfo _info;
		private AttackInfo[] _attacks = null;

		public override Appearance Image => _info.Image;
		public CreatureInfo Info => _info;
		public override AttackInfo[] Attacks
		{
			get
			{
				Update();
				return _attacks;
			}
		}

		public NonPlayer(CreatureInfo info)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			_info = info;
			Name = _info.Name;
			Gold = _info.Gold;

			foreach (var itemPile in info.Inventory.Items)
			{
				Inventory.Items.Add(itemPile.Clone());
			}
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}


			_attacks = _info.Attacks.ToArray();

			_dirty = false;
		}
	}
}
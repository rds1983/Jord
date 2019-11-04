using System;

namespace Wanderers.Core
{
	public class NonPlayer : Creature
	{
		private bool _dirty = true;
		private readonly BattleStats _battleStats = new BattleStats();

		public override Appearance Image => Info.Image;
		public CreatureInfo Info { get; }

		public override BattleStats BattleStats
		{
			get
			{
				Update();
				return _battleStats;
			}
		}

		public NonPlayer(CreatureInfo info)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			Info = info;
			Name = Info.Name;
			Gold = Info.Gold;

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

			_battleStats.Attacks = Info.Attacks.ToArray();
			_battleStats.ArmorClass = Info.ArmorClass;
			_battleStats.HitRoll = Info.HitRoll;

			_dirty = false;
		}
	}
}
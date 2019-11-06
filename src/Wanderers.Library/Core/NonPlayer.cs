using System;

namespace Wanderers.Core
{
	public class NonPlayer : Creature
	{
		private bool _dirty = true;
		private readonly CreatureStats _stats = new CreatureStats();

		public override Appearance Image => Info.Image;
		public CreatureInfo Info { get; }

		public override CreatureStats Stats
		{
			get
			{
				Update();
				return _stats;
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

			Stats.Life.Restore();
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			var lifeStats = _stats.Life;
			lifeStats.MaximumHP = Info.MaxHp;
			lifeStats.MaximumMana = Info.MaxMana;
			lifeStats.MaximumStamina = Info.MaxStamina;

			var battleStats = _stats.Battle;
			battleStats.Attacks = Info.Attacks.ToArray();
			battleStats.ArmorClass = Info.ArmorClass;
			battleStats.HitRoll = Info.HitRoll;

			_dirty = false;
		}
	}
}
using System;

namespace TroublesOfJord.Core
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

		public void Act()
		{
			if (Info.CreatureType == CreatureType.Npc)
			{
				return;
			}

			// Attack player if he is nearby
			for(var x = Math.Max(Position.X - 1, 0); x <= Math.Min(Position.X + 1, Map.Size.X - 1); ++x)
			{
				for (var y = Math.Max(Position.Y - 1, 0); y <= Math.Min(Position.Y + 1, Map.Size.Y - 1); ++y)
				{
					if (x == Position.X && y == Position.Y)
					{
						continue;
					}

					var player = Map[x, y].Creature as Player;
					if (player == null)
					{
						continue;
					}

					Attack(player);
				}
			}
		}
	}
}
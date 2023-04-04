using Jord.Utils;
using System;

namespace Jord.Core
{
	public partial class NonPlayer : Creature
	{
		private CreatureStats _stats;
		private Inventory _inventory;

		public override Appearance Image => Info.Image;
		public CreatureInfo Info { get; }

		public override CreatureStats Stats
		{
			get
			{
				if (_stats == null)
				{
					_stats = new CreatureStats();
					var lifeStats = _stats.Life;
					lifeStats.MaximumHP = Info.MaxHp;
					lifeStats.MaximumMana = Info.MaxMana;
					lifeStats.MaximumStamina = Info.MaxStamina;
					lifeStats.HpRegen = Info.HpRegen;

					var battleStats = _stats.Battle;
					battleStats.Attacks = Info.Attacks.ToArray();
					battleStats.ArmorClass = Info.ArmorClass;
					battleStats.HitRoll = Info.HitRoll;
				}

				return _stats;
			}
		}

		public override Inventory Inventory
		{
			get
			{
				if (_inventory == null)
				{
					_inventory = new Inventory();
					foreach (var itemPile in Info.Inventory.Items)
					{
						_inventory.Items.Add(itemPile.Clone());
					}
				}

				return _inventory;
			}
		}

		public Creature AttackTarget;

		public NonPlayer(CreatureInfo info)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			Info = info;
			Name = Info.Name;
			Gold = Info.Gold;

			Stats.Life.Restore();
		}

		public void Act()
		{
/*			if (Info.CreatureType != CreatureType.Enemy)
			{
				return;
			}

			if (AttackTarget == null)
			{

			}
			else
			{
				var attacked = false;

				// Attack player if he is nearby
				for (var x = Math.Max(Position.X - 1, 0); x <= Math.Min(Position.X + 1, Map.Width - 1); ++x)
				{
					for (var y = Math.Max(Position.Y - 1, 0); y <= Math.Min(Position.Y + 1, Map.Height - 1); ++y)
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
						attacked = true;
						goto finished;
					}
				}
			finished:;
				if (!attacked)
				{
					var path = Map.PathFinder.ShortestPath(Position.ToCoord(), AttackTarget.Position.ToCoord());
					if (path.Length > 0)
					{
						var delta = path.GetStep(0).ToPoint() - Position;
//						MoveTo(delta);
					}
				}
			}*/
		}
	}
}
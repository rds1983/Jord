﻿using System;
using System.Diagnostics;
using System.Linq;

namespace TroublesOfJord.Core
{
	public partial class NonPlayer : Creature
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
			if (Info.CreatureType != CreatureType.Enemy)
			{
				return;
			}

			if (AttackTarget == null)
			{
				// Check if player is visible
				if (Map.FieldOfView.CurrentFOV.Contains(Position))
				{
					TJ.GameLog(Strings.BuildRushesToAttack(Info.Name));
					AttackTarget = TJ.Player;
				}
			}

			if (AttackTarget != null)
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

						attacked = true;
						Attack(player);
						goto finished;
					}
				}
			finished:;
				if (!attacked)
				{
					var path = Map.PathFinder.ShortestPath(Position, AttackTarget.Position);
					if (path.Length > 0)
					{
						var delta = path.GetStep(0) - Position;
						MoveTo(delta);
					}
				}
			}
		}
	}
}
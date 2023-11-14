using System;
using System.Diagnostics;
using Jord.Utils;

namespace Jord.Core
{
	partial class Creature
	{
		private const int AttackDurationInMs = 100;

		public event EventHandler StartAttack;
		public event EventHandler EndAttack;

		protected virtual void OnKilledTarget(Creature target)
		{
		}

		public void Attack(Creature target)
		{
			var battleStats = Stats.Battle;

			var attacks = battleStats.Attacks;
			if (attacks == null || attacks.Length == 0)
			{
				return;
			}

			var targetBattleStats = target.Stats.Battle;
			var evasionRating = targetBattleStats.EvasionRating;
			foreach (var attack in attacks)
			{
				var armorClass = targetBattleStats.ArmorClass;

				// Hit roll
				var meleeMastery = 50 + battleStats.MeleeMastery;
				var hitRoll = meleeMastery - targetBattleStats.EvasionRating;
				Debug.WriteLine($"{Name} against {target.Name}'s hitRoll roll is {hitRoll}");
				if (!MathUtils.RollPercentage(hitRoll))
				{
					var message = Strings.GetEvadeMessage(Name, target.Name, attack.AttackType);
					TJ.GameLog(message);

					evasionRating = Math.Max(0, evasionRating - 15);
				} else
				{
					// Block roll
					var blockRoll = targetBattleStats.BlockingRating;
					Debug.WriteLine($"{Name} against {target.Name}'s blockRoll roll is {blockRoll}");
					var blocked = MathUtils.RollPercentage(blockRoll);

					var damage = MathUtils.Random.Next(attack.MinDamage, attack.MaxDamage + 1);
					Debug.WriteLine($"{Name} against {target.Name}'s initial damage is {damage}");

					meleeMastery = 100 + battleStats.MeleeMastery;
					var damageRoll = meleeMastery - targetBattleStats.ArmorClass;
					Debug.WriteLine($"{Name} against {target.Name}'s damageRoll roll is {damageRoll}");

					damage = (damage * damageRoll / 100);

					if (blocked)
					{
						damage /= 2;
					}
					Debug.WriteLine($"{Name} against {target.Name}'s actual damage is {damage}");

					if (damage == 0)
					{
						var message = Strings.GetArmorMessage(Name, target.Name, attack.AttackType, blocked);
						TJ.GameLog(message);
					} else
					{
						target.Stats.Life.CurrentHP -= damage;
						var message = Strings.GetAttackMessage(damage, Name, target.Name, attack.AttackType, blocked);
						TJ.GameLog(message);

						if (target.Stats.Life.CurrentHP <= 0 && target != TJ.Player)
						{
							OnKilledTarget(target);
							break;
						}
					}
				}
			}

			var oldPosition = Position.ToVector2();
			var halfPosition = oldPosition + (target.DisplayPosition - oldPosition) * 0.5f;

			void onUpdate(float part)
			{
				if (part < 0.5f)
				{
					// Movement towards target
					DisplayPosition = oldPosition + (target.DisplayPosition - oldPosition) * part;
				}
				else
				{
					// Movement back
					DisplayPosition = halfPosition + (oldPosition - halfPosition) * part;
				}
			}

			StartAttack.Invoke(this);

			TJ.ActivityService.AddOrderedActivity(onUpdate, () => { DisplayPosition = oldPosition; EndAttack.Invoke(this); }, AttackDurationInMs);
		}
	}
}
